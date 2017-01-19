using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;
using System.ComponentModel;

namespace N3ExportContabilidade
{
    class ExportadorDados
    {
        public CultureInfo cultureInfoUS = new CultureInfo("en-US");
        public double IPI = 0;
        public double ICMS = 0;
        public double PIS = 0;
        public double COFINS = 0;
        public double VENDASProdutoBen = 0;
        public double VENDASMercadoriaBen = 0;

        public bool existeContaContabilNaoCadastrada = false;
        private Dictionary<string, string> clientesSemConta =  new Dictionary<string, string>();
        public StringBuilder logCriacaoContasContabeis;
            

        private SqlConnection conexao;

        public SqlConnection getConexaoStoreProcedure() {
            if (conexao == null) {
                StringBuilder stringConexao = new StringBuilder();
                stringConexao.Append("server=" + Constantes.SERVIDOR_DB + ";");
                stringConexao.Append("database=" + Constantes.BANCO_DADOS + ";");
                stringConexao.Append("uid=" + Constantes.USUARIO + ";");
                stringConexao.Append("pwd=" + Constantes.SENHA + ";");
                stringConexao.Append("max pool size=10;min pool size=5");
                conexao = new SqlConnection(stringConexao.ToString());
            }
            if (conexao.State == ConnectionState.Closed || conexao.State == ConnectionState.Broken) {
                conexao.Open();
            }
            return conexao;
        }


        public SqlConnection getConexao() {
            SqlConnection conexao = null;
            StringBuilder stringConexao = new StringBuilder();
            stringConexao.Append("server=" + Constantes.SERVIDOR_DB + ";");
            stringConexao.Append("database=" + Constantes.BANCO_DADOS + ";");
            stringConexao.Append("uid=" + Constantes.USUARIO + ";");
            stringConexao.Append("pwd=" + Constantes.SENHA + ";");
            stringConexao.Append("max pool size=10;min pool size=5");
            conexao = new SqlConnection(stringConexao.ToString());
            if (conexao.State == ConnectionState.Closed || conexao.State == ConnectionState.Broken) {
                conexao.Open();
            }
            return conexao;
        }


        /**
         * 
         *  Linhas no formato Novo OK
         * 
         *  Constantes OK
         */
        public void geraArquivoExportacaoServicosImportacao(DateTime dataInicial, DateTime dataFinal, string caminhoExportacao, bool matriz, bool recife, bool fortaleza) {
            string clausulaFiliais = geraClausulaWhereFiliais(matriz, recife, fortaleza);
            // Obtem um objeto do tipo Command
            SqlCommand comando = getConexao().CreateCommand();
            comando.CommandText =
                "SELECT * FROM N3_EntradasContabil WHERE "
                + "N3_EntradasContabil.DATA_ENTRADA >= '" + dataInicial.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + "AND N3_EntradasContabil.DATA_ENTRADA <= '" + dataFinal.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + clausulaFiliais;

            StringBuilder conteudoArquivoFilial_1 = new StringBuilder();

            SqlDataReader sqlReader = comando.ExecuteReader();
            while (sqlReader.Read()) {
                string tipoCompra = sqlReader["CODTMV"].ToString();
                string tipoServicos = sqlReader["TIPOSERVICOS"].ToString();
                
                DateTime dataEntrada = DateTime.Parse(sqlReader["DATA_ENTRADA"].ToString());
                Decimal valorLiquidoNF = (Decimal)sqlReader["VALORLIQUIDO"];
                int codigoFilial = int.Parse(sqlReader["CODFILIAL"].ToString());

                // Servicos de Liberação de Importação
                //if (tipoCompra != null && tipoCompra.Equals("1.2.28")    antigo movimento de serviços de importação
                if (tipoCompra != null && tipoCompra.Equals("1.2.44")
                    && tipoServicos != null && tipoServicos.Equals("2")) {
                    if (codigoFilial == 1) {
                        geraLinhasServicosImportacao(conteudoArquivoFilial_1, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    } 
                }
            }

            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_SERVICOS_IMPORTACAO, caminhoExportacao, conteudoArquivoFilial_1);

            sqlReader.Close();
            comando.Connection.Close();
        }


        /**
          * 
          * Linhas no formato Novo OK
          * 
          * Constantes OK
          */
        private void geraLinhasServicosImportacao(StringBuilder conteudoArquivo, SqlDataReader sReaderCompras, DateTime dataEntrada, Decimal valorLiquidoNF, int codigoFilial) {
            string nfsServico = "\"NFS:" + sReaderCompras["NUMEROMOV"] + "\"";
            // Linha 1: Debito no valor da Nota Fiscal de Serviço
            string contaDebitoNFServico = null;
            string codigoNFServicoHistorico = Constantes.C_F1_SERVICOS_IMPORTACAO_HISTORICO;
            if (codigoFilial == 1) {
                contaDebitoNFServico = Constantes.C_F1_SERVICOS_IMPORTACAO_DEBITO;
            }
            string linhaDebitoValorContabil = geraLinhaTextoCreditoDebito(dataEntrada, contaDebitoNFServico, "0",
                valorLiquidoNF, codigoNFServicoHistorico, nfsServico);
            conteudoArquivo.Append(linhaDebitoValorContabil);

            // Linha 2: Crédito na conta do Fornecedor
            string linhaCreditoFornecedor = null;
            string contaContabilFornecedor = obtemContaContabilFornecedor(sReaderCompras["CODCFO"].ToString(), sReaderCompras["CODTMV"].ToString(), sReaderCompras["NUMEROMOV"].ToString(), codigoFilial);
            if (contaContabilFornecedor == null) {
                contaContabilFornecedor = "ERRO: FORNECEDOR NAO MAPEADO => " + sReaderCompras["CODCFO"].ToString();
            }
            linhaCreditoFornecedor = geraLinhaTextoCreditoDebito(dataEntrada, "0", contaContabilFornecedor,
                valorLiquidoNF, codigoNFServicoHistorico, nfsServico);
            conteudoArquivo.Append(linhaCreditoFornecedor);


            // Linha 3: Débito de impostos de PIS incidentes Nota Fiscal de Serviço
            Decimal valorPIS = (Decimal)sReaderCompras["PIS"];
            if (!valorPIS.Equals(Decimal.Zero)) {
                string linhaDebitoPIS = null;
                string contaDebitoPIS = null;
                string codigoPISHistorico = Constantes.C_F1_SERVICOS_IMPORTACAO_HISTORICO_PIS;
                // Código da conta de PIS a recuperar
                if (codigoFilial == 1) {
                    // Código da conta de Debito de PIS Matriz
                    contaDebitoPIS = Constantes.C_F1_SERVICOS_IMPORTACAO_DEBITO_PIS;
                }
                linhaDebitoPIS = geraLinhaTextoCreditoDebito(dataEntrada, contaDebitoPIS, "0",
                    valorPIS, codigoPISHistorico, nfsServico);
                conteudoArquivo.Append(linhaDebitoPIS);

                // Linha 6: Crédito de PIS na conta de Frete
                string linhaCreditoPIS = null;
                string contaCreditoPIS = null;
                // Código da conta de PIS a recuperar
                if (codigoFilial == 1) {
                    // Código da conta de Debito de PIS Matriz
                    contaCreditoPIS = Constantes.C_F1_SERVICOS_IMPORTACAO_CREDITO_PIS;
                }
                linhaCreditoPIS = geraLinhaTextoCreditoDebito(dataEntrada, "0", contaCreditoPIS,
                    valorPIS, codigoPISHistorico, nfsServico);
                conteudoArquivo.Append(linhaCreditoPIS);
            }

            Decimal valorCOFINS = (Decimal)sReaderCompras["COFINS"];
            if (!valorCOFINS.Equals(Decimal.Zero)) {
                // Linha 7: Débito de impostos de COFINS a recuperar
                string linhaDebitoCOFINS = null;
                string contaDebitoCOFINS = null;
                string codigoCOFINSHistorico = Constantes.C_F1_SERVICOS_IMPORTACAO_HISTORICO_COFINS;
                // Conta de COFINS a recuperar da Matriz/Filial
                if (codigoFilial == 1) {
                    // Código da conta de Debito de COFINS Matriz
                    contaDebitoCOFINS = Constantes.C_F1_SERVICOS_IMPORTACAO_DEBITO_COFINS;
                }
                linhaDebitoCOFINS = geraLinhaTextoCreditoDebito(dataEntrada, contaDebitoCOFINS, "0",
                    valorCOFINS, codigoCOFINSHistorico, nfsServico);
                conteudoArquivo.Append(linhaDebitoCOFINS);

                // Linha 8: Crédito de impostos e contribuições de COFINS
                string linhaCreditoCOFINS = null;
                string contaCreditoCOFINS = null;
                // Código da conta de Mercadoria para Revenda
                if (codigoFilial == 1) {
                    // Código da conta de Mercadoria para Revenda da Matriz
                    contaCreditoCOFINS = Constantes.C_F1_SERVICOS_IMPORTACAO_CREDITO_COFINS;
                }
                linhaCreditoCOFINS = geraLinhaTextoCreditoDebito(dataEntrada, "0", contaCreditoCOFINS,
                    valorCOFINS, codigoCOFINSHistorico, nfsServico);
                conteudoArquivo.Append(linhaCreditoCOFINS);
            }
        }        
        


        /**
         *
         *  Linhas no formato Novo OK
         * 
         *  Constantes OK
         * 
         */
        public void geraArquivoExportacaoFreteVendas(DateTime dataInicial, DateTime dataFinal, string caminhoExportacao, bool matriz, bool recife, bool fortaleza) {
            string clausulaFiliais = geraClausulaWhereFiliais(matriz, recife, fortaleza);
            // Obtem um objeto do tipo Command
            SqlCommand comando = getConexao().CreateCommand();
            comando.CommandText =
                "SELECT * FROM N3_EntradasContabil WHERE "
                + "N3_EntradasContabil.DATA_ENTRADA >= '" + dataInicial.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + "AND N3_EntradasContabil.DATA_ENTRADA <= '" + dataFinal.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + clausulaFiliais;

            StringBuilder conteudoArquivoFilial_1 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_2 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_3 = new StringBuilder();

            SqlDataReader sqlReader = comando.ExecuteReader();
            while (sqlReader.Read()) {
                string tipoCompra = sqlReader["CODTMV"].ToString();
                DateTime dataEntrada = DateTime.Parse(sqlReader["DATA_ENTRADA"].ToString());
                Decimal valorLiquidoNF = (Decimal)sqlReader["VALORLIQUIDO"];
                int codigoFilial = int.Parse(sqlReader["CODFILIAL"].ToString());

                // Frete de Vendas de Produtos e Mercadorias
                if (tipoCompra != null && tipoCompra.Equals("1.2.32")) {
                    if (codigoFilial == 1) {
                        geraLinhasFreteVendas(conteudoArquivoFilial_1, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    } else if (codigoFilial == 2) {
                        geraLinhasFreteVendas(conteudoArquivoFilial_2, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    } else if (codigoFilial == 3) {
                        geraLinhasFreteVendas(conteudoArquivoFilial_3, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    }
                } 
            }

            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_FRETES_VENDAS, caminhoExportacao, conteudoArquivoFilial_1);
            escreveArquivoExportacao("Filial_2_" + Constantes.ARQ_EXPORTACAO_FRETES_VENDAS, caminhoExportacao, conteudoArquivoFilial_2);
            escreveArquivoExportacao("Filial_3_" + Constantes.ARQ_EXPORTACAO_FRETES_VENDAS, caminhoExportacao, conteudoArquivoFilial_3);

            sqlReader.Close();
            comando.Connection.Close();
        }


        /**
         * 
         *  Linhas no formato Novo OK
         * 
         *  Constantes OK
         */
        public void geraArquivoExportacaoFreteCompras(DateTime dataInicial, DateTime dataFinal, string caminhoExportacao, bool matriz, bool recife, bool fortaleza) {
            string clausulaFiliais = geraClausulaWhereFiliais(matriz, recife, fortaleza);
            // Obtem um objeto do tipo Command
            SqlCommand comando = getConexao().CreateCommand();
            comando.CommandText =
                "SELECT * FROM N3_EntradasContabil WHERE "
                + "N3_EntradasContabil.DATA_ENTRADA >= '" + dataInicial.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + "AND N3_EntradasContabil.DATA_ENTRADA <= '" + dataFinal.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + clausulaFiliais;

            StringBuilder conteudoArquivoFilial_Nacional_1 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_Nacional_2 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_Nacional_3 = new StringBuilder();

            StringBuilder conteudoArquivoFilial_Importacao_1 = new StringBuilder();

            SqlDataReader sqlReader = comando.ExecuteReader();
            while (sqlReader.Read()) {
                string tipoCompra = sqlReader["CODTMV"].ToString();
                string cfop = sqlReader["CFOP"].ToString();
                DateTime dataEntrada = DateTime.Parse(sqlReader["DATA_ENTRADA"].ToString());
                Decimal valorLiquidoNF = (Decimal)sqlReader["VALORLIQUIDO"];
                int codigoFilial = int.Parse(sqlReader["CODFILIAL"].ToString());

                // Frete de Compras Nacionais de Produtos e Mercadorias
                if (tipoCompra != null && tipoCompra.Equals("1.2.33")
                        && cfop != null && (!cfop.EndsWith(".351.04") && !cfop.EndsWith(".351.05"))) {
                    if (codigoFilial == 1) {
                        geraLinhasFreteCompras(conteudoArquivoFilial_Nacional_1, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    } else if (codigoFilial == 2) {
                        geraLinhasFreteCompras(conteudoArquivoFilial_Nacional_2, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    } else if (codigoFilial == 3) {
                        geraLinhasFreteCompras(conteudoArquivoFilial_Nacional_3, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    }
                }
                else if (tipoCompra != null && tipoCompra.Equals("1.2.33")
                        && cfop != null && (cfop.EndsWith(".351.04") || cfop.EndsWith(".351.05")))
                {
                    if (codigoFilial == 1)
                    {
                        geraLinhasFreteCompras(conteudoArquivoFilial_Importacao_1, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    }
                }
            }

            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_FRETES_COMPRAS, caminhoExportacao, conteudoArquivoFilial_Nacional_1);
            escreveArquivoExportacao("Filial_2_" + Constantes.ARQ_EXPORTACAO_FRETES_COMPRAS, caminhoExportacao, conteudoArquivoFilial_Nacional_2);
            escreveArquivoExportacao("Filial_3_" + Constantes.ARQ_EXPORTACAO_FRETES_COMPRAS, caminhoExportacao, conteudoArquivoFilial_Nacional_3);
            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_FRETES_REMOCAO_IMPORTACAO, caminhoExportacao, conteudoArquivoFilial_Importacao_1);

            sqlReader.Close();
            comando.Connection.Close();
        }

        /**
         * 
         * Linhas no formato Novo OK
         * 
         * Constantes OK
         */
        private void geraLinhasFreteCompras(StringBuilder conteudoArquivo, SqlDataReader sReaderCompras, DateTime dataEntrada, Decimal valorLiquidoNF, int codigoFilial) {
            string ctrFrete = "\"CTR:" + sReaderCompras["NUMEROMOV"] + "\"";
            string cfop = sReaderCompras["CFOP"].ToString();
            // Linha 1: Debito no valor do Frete de Compras
            string contaDebitoFreteCompra = null;
            string codigoFreteCompraHistorico = Constantes.C_F2_FRETE_COMPRA_HISTORICO;
            if (codigoFilial == 1) {
                // Frete de Compras Nacionais
                if (cfop != null && (!cfop.EndsWith(".351.04") && !cfop.EndsWith(".351.05"))) {
                    contaDebitoFreteCompra = Constantes.C_F1_FRETE_COMPRA_DEBITO;
                }
                // Frete de Remocao de Importacao
                else if (cfop != null && (cfop.EndsWith(".351.04") || cfop.EndsWith(".351.05"))) {
                    contaDebitoFreteCompra = Constantes.C_F1_FRETE_REMOCAO_IMPORTACAO_DEBITO;
                }
            } else if (codigoFilial == 2) {
                contaDebitoFreteCompra = Constantes.C_F2_FRETE_COMPRA_DEBITO;
            } else if (codigoFilial == 3) {
                contaDebitoFreteCompra= Constantes.C_F3_FRETE_COMPRA_DEBITO;
            }
            string linhaDebitoValorContabil = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoFreteCompra,"0",
                valorLiquidoNF,codigoFreteCompraHistorico,ctrFrete);
            conteudoArquivo.Append(linhaDebitoValorContabil);

            // Linha 2: Crédito na conta do Fornecedor
            string linhaCreditoFornecedor = null;
            string contaContabilFornecedor = obtemContaContabilFornecedor(sReaderCompras["CODCFO"].ToString(), sReaderCompras["CODTMV"].ToString(), sReaderCompras["NUMEROMOV"].ToString(), codigoFilial);
            if (contaContabilFornecedor == null) {
                contaContabilFornecedor = "ERRO: FORNECEDOR NAO MAPEADO => " + sReaderCompras["CODCFO"].ToString();
            }
            linhaCreditoFornecedor = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaContabilFornecedor,
                valorLiquidoNF,codigoFreteCompraHistorico,ctrFrete);
            conteudoArquivo.Append(linhaCreditoFornecedor);

            Decimal valorICMS = (Decimal)sReaderCompras["ICMS"];
            if (!valorICMS.Equals(Decimal.Zero)) {
                // Linha 3: Débito de impostos de ICMS Fretes de Compra
                string linhaDebitoICMS = null;
                string contaDebitoICMS = null;
                string codigoICMSHistorico = Constantes.C_F1_FRETE_COMPRA_HISTORICO_ICMS;
                if (codigoFilial == 1) {
                    // Código da conta de ICMS da Matriz
                    // Frete de Compras Nacionais
                    if (cfop != null && (!cfop.EndsWith(".351.04") && !cfop.EndsWith(".351.05"))) {
                        contaDebitoICMS = Constantes.C_F1_FRETE_COMPRA_DEBITO_ICMS;
                    }
                    // Frete de Remocao de Importacao
                    else if (cfop != null && (cfop.EndsWith(".351.04") || cfop.EndsWith(".351.05"))) {
                        contaDebitoICMS = Constantes.C_F1_FRETE_REMOCAO_IMPORTACAO_DEBITO_ICMS;
                    }
                } else if (codigoFilial == 2) {
                    contaDebitoICMS = Constantes.C_F2_FRETE_COMPRA_DEBITO_ICMS;
                } else if (codigoFilial == 3) {
                    contaDebitoICMS = Constantes.C_F3_FRETE_COMPRA_DEBITO_ICMS;
                }
                linhaDebitoICMS = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoICMS,"0",
                    valorICMS,codigoICMSHistorico, ctrFrete);
                conteudoArquivo.Append(linhaDebitoICMS);

                // Linha 4: Crédito ICMS na Conta de Fretes
                string linhaCreditoICMS = null;
                string contaCreditoICMS = null;
                if (codigoFilial == 1) {
                    // Código da conta de Fretes de Compra
                    // Frete de Compras Nacionais
                    if (cfop != null && (!cfop.EndsWith(".351.04") && !cfop.EndsWith(".351.05"))) {
                        contaCreditoICMS = Constantes.C_F1_FRETE_COMPRA_CREDITO_ICMS;
                    }
                        // Frete de Remocao de Importacao
                    else if (cfop != null && (cfop.EndsWith(".351.04") || cfop.EndsWith(".351.05"))) {
                        contaCreditoICMS = Constantes.C_F1_FRETE_REMOCAO_IMPORTACAO_CREDITO_ICMS;
                    }
                } else if (codigoFilial == 2) {
                    contaCreditoICMS = Constantes.C_F2_FRETE_COMPRA_CREDITO_ICMS;
                } else if (codigoFilial == 3) {
                    contaCreditoICMS = Constantes.C_F3_FRETE_COMPRA_CREDITO_ICMS;
                }
                linhaCreditoICMS = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoICMS,
                    valorICMS,codigoICMSHistorico,ctrFrete);
                conteudoArquivo.Append(linhaCreditoICMS);
            }

            // Linha 5: Débito de impostos de PIS incidentes frete de Compra
            Decimal valorPIS = (Decimal)sReaderCompras["PIS"];
            if (!valorPIS.Equals(Decimal.Zero)) {
                string linhaDebitoPIS = null;
                string contaDebitoPIS = null;
                string codigoPISHistorico = Constantes.C_F1_FRETE_COMPRA_HISTORICO_PIS;
                // Código da conta de PIS a recuperar
                if (codigoFilial == 1) {
                    // Código da conta de Debito de PIS Matriz
                    // Frete de Compras Nacionais
                    if (cfop != null && (!cfop.EndsWith(".351.04") && !cfop.EndsWith(".351.05"))) {
                        contaDebitoPIS = Constantes.C_F1_FRETE_COMPRA_DEBITO_PIS;
                    }
                        // Frete de Remocao de Importacao
                    else if (cfop != null && (cfop.EndsWith(".351.04") || cfop.EndsWith(".351.05"))) {
                        contaDebitoPIS = Constantes.C_F1_FRETE_REMOCAO_IMPORTACAO_DEBITO_PIS;
                    }
                } else if (codigoFilial == 2) {
                    contaDebitoPIS = Constantes.C_F2_FRETE_COMPRA_DEBITO_PIS;
                } else if (codigoFilial == 3) {
                    contaDebitoPIS = Constantes.C_F3_FRETE_COMPRA_DEBITO_PIS;
                }
                linhaDebitoPIS = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoPIS,"0",
                    valorPIS,codigoPISHistorico,ctrFrete);
                conteudoArquivo.Append(linhaDebitoPIS);

                // Linha 6: Crédito de PIS na conta de Frete
                string linhaCreditoPIS = null;
                string contaCreditoPIS = null;
                // Código da conta de PIS a recuperar
                if (codigoFilial == 1) {
                    // Código da conta de Debito de PIS Matriz
                    // Frete de Compras Nacionais
                    if (cfop != null && (!cfop.EndsWith(".351.04") && !cfop.EndsWith(".351.05"))) {
                        contaCreditoPIS = Constantes.C_F1_FRETE_COMPRA_CREDITO_PIS;
                    }
                        // Frete de Remocao de Importacao
                    else if (cfop != null && (cfop.EndsWith(".351.04") || cfop.EndsWith(".351.05"))) {
                        contaCreditoPIS = Constantes.C_F1_FRETE_REMOCAO_IMPORTACAO_CREDITO_PIS;
                    }
                } else if (codigoFilial == 2) {
                    contaCreditoPIS = Constantes.C_F2_FRETE_COMPRA_CREDITO_PIS;
                } else if (codigoFilial == 3) {
                    contaCreditoPIS = Constantes.C_F3_FRETE_COMPRA_CREDITO_PIS;
                }
                linhaCreditoPIS = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoPIS,
                    valorPIS,codigoPISHistorico,ctrFrete);
                conteudoArquivo.Append(linhaCreditoPIS);
            }


            Decimal valorCOFINS = (Decimal)sReaderCompras["COFINS"];
            if (!valorCOFINS.Equals(Decimal.Zero)) {
                // Linha 7: Débito de impostos de COFINS a recuperar
                string linhaDebitoCOFINS = null;
                string contaDebitoCOFINS = null;
                string codigoCOFINSHistorico = Constantes.C_F1_FRETE_COMPRA_HISTORICO_COFINS;
                // Conta de COFINS a recuperar da Matriz/Filial
                if (codigoFilial == 1) {
                    // Código da conta de Debito de COFINS Matriz
                    // Frete de Compras Nacionais
                    if (cfop != null && (!cfop.EndsWith(".351.04") && !cfop.EndsWith(".351.05"))) {
                        contaDebitoCOFINS = Constantes.C_F1_FRETE_COMPRA_DEBITO_COFINS;
                    }
                    // Frete de Remocao de Importacao
                    else if (cfop != null && (cfop.EndsWith(".351.04") || cfop.EndsWith(".351.05"))) {
                        contaDebitoCOFINS = Constantes.C_F1_FRETE_REMOCAO_IMPORTACAO_DEBITO_COFINS;
                    }
                } else if (codigoFilial == 2) {
                    contaDebitoCOFINS = Constantes.C_F2_FRETE_COMPRA_DEBITO_COFINS;
                } else if (codigoFilial == 3) {
                    contaDebitoCOFINS = Constantes.C_F3_FRETE_COMPRA_DEBITO_COFINS;
                }
                linhaDebitoCOFINS = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoCOFINS,"0",
                    valorCOFINS,codigoCOFINSHistorico,ctrFrete);
                conteudoArquivo.Append(linhaDebitoCOFINS);

                // Linha 8: Crédito de impostos e contribuições de COFINS
                string linhaCreditoCOFINS = null;
                string contaCreditoCOFINS = null;
                // Código da conta de Mercadoria para Revenda
                if (codigoFilial == 1) {
                    // Código da conta de Mercadoria para Revenda da Matriz
                    // Frete de Compras Nacionais
                    if (cfop != null && (!cfop.EndsWith(".351.04") && !cfop.EndsWith(".351.05"))) {
                        contaCreditoCOFINS = Constantes.C_F1_FRETE_COMPRA_CREDITO_COFINS;
                    }
                        // Frete de Remocao de Importacao
                    else if (cfop != null && (cfop.EndsWith(".351.04") || cfop.EndsWith(".351.05"))) {
                        contaCreditoCOFINS = Constantes.C_F1_FRETE_REMOCAO_IMPORTACAO_CREDITO_COFINS;
                    }
                } else if (codigoFilial == 2) {
                    contaCreditoCOFINS = Constantes.C_F2_FRETE_COMPRA_CREDITO_COFINS;
                } else if (codigoFilial == 3) {
                    contaCreditoCOFINS = Constantes.C_F3_FRETE_COMPRA_CREDITO_COFINS;
                }
                linhaCreditoCOFINS = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoCOFINS,
                    valorCOFINS,codigoCOFINSHistorico,ctrFrete);
                conteudoArquivo.Append(linhaCreditoCOFINS);
            }
        }        
        
        
        /**
         * 
         * Linhas no formato Novo OK
         * 
         * Constantes OK
         */
        private void geraLinhasFreteVendas(StringBuilder conteudoArquivo, SqlDataReader sReaderCompras, DateTime dataEntrada, Decimal valorLiquidoNF, int codigoFilial) {
            string ctrFrete = "\"CTR:" + sReaderCompras["NUMEROMOV"] + "\"";
            string contaDebitoFreteVenda = null;
            string codigoFreteVendaHistorico = Constantes.C_F1_FRETE_VENDA_HISTORICO;
            // Linha 1: Debito no valor do frete em Despesas com Vendas
            if (codigoFilial == 1) {
                contaDebitoFreteVenda = Constantes.C_F1_FRETE_VENDA_DEBITO;
            } else if (codigoFilial == 2) {
                contaDebitoFreteVenda = Constantes.C_F2_FRETE_VENDA_DEBITO;
            } else if (codigoFilial == 3) {
                contaDebitoFreteVenda = Constantes.C_F3_FRETE_VENDA_DEBITO;
            }
            
            string linhaDebito = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoFreteVenda,"0",valorLiquidoNF,
                codigoFreteVendaHistorico,ctrFrete);
            conteudoArquivo.Append(linhaDebito);

            // Linha 2: Crédito na conta do Fornecedor
            // Busca a conta do fornecedor
            string contaContabilFornecedor = obtemContaContabilFornecedor(sReaderCompras["CODCFO"].ToString(), sReaderCompras["CODTMV"].ToString(), sReaderCompras["NUMEROMOV"].ToString(), codigoFilial);
            if (contaContabilFornecedor == null) {
                contaContabilFornecedor = "ERRO: FORNECEDOR NAO MAPEADO => " + sReaderCompras["CODCFO"].ToString();
            }

            string linhaCredito = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaContabilFornecedor,
                valorLiquidoNF,codigoFreteVendaHistorico,ctrFrete);
            conteudoArquivo.Append(linhaCredito);

            // Linha 3: Débito de impostos de PIS incidentes frete de Compra
            Decimal valorPIS = (Decimal)sReaderCompras["PIS"];
            if (!valorPIS.Equals(Decimal.Zero)) {
                string linhaDebitoPIS = null;
                string contaDebitoPIS = null;
                string codigoPISHistorico = Constantes.C_F1_FRETE_VENDA_HISTORICO_PIS;
                // Código da conta de PIS a recuperar
                if (codigoFilial == 1) {
                    // Código da conta de Debito de PIS Matriz
                    contaDebitoPIS = Constantes.C_F1_FRETE_VENDA_DEBITO_PIS;
                        //900
                } else if (codigoFilial == 2) {
                    contaDebitoPIS = Constantes.C_F2_FRETE_VENDA_DEBITO_PIS;
                } else if (codigoFilial == 3) {
                    contaDebitoPIS = Constantes.C_F3_FRETE_VENDA_DEBITO_PIS;
                }
                linhaDebitoPIS = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoPIS,"0",
                    valorPIS,codigoPISHistorico,ctrFrete);
                conteudoArquivo.Append(linhaDebitoPIS);

                // Linha 4: Crédito de PIS na conta de Frete
                string linhaCreditoPIS = null;
                string contaCreditoPIS = null;
                // Código da conta de PIS a recuperar
                if (codigoFilial == 1) {
                    // Código da conta de Debito de PIS Matriz
                    contaCreditoPIS = Constantes.C_F1_FRETE_VENDA_CREDITO_PIS;
                        //"537";
                } else if (codigoFilial == 2) {
                    contaCreditoPIS = Constantes.C_F2_FRETE_VENDA_CREDITO_PIS;
                } else if (codigoFilial == 3) {
                    contaCreditoPIS = Constantes.C_F3_FRETE_VENDA_CREDITO_PIS;
                }
                linhaCreditoPIS = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoPIS,
                    valorPIS,codigoPISHistorico,ctrFrete);
                conteudoArquivo.Append(linhaCreditoPIS);
            }

            Decimal valorCOFINS = (Decimal)sReaderCompras["COFINS"];
            if (!valorCOFINS.Equals(Decimal.Zero)) {
                // Linha 5: Débito de impostos de COFINS a recuperar
                string linhaDebitoCOFINS = null;
                string contaDebitoCOFINS = null;
                string codigoCOFINSHistorico = Constantes.C_F1_FRETE_VENDA_HISTORICO_COFINS;
                // Conta de COFINS a recuperar da Matriz/Filial
                if (codigoFilial == 1) {
                    // Código da conta de Debito de COFINS Matriz
                    contaDebitoCOFINS = Constantes.C_F1_FRETE_VENDA_DEBITO_COFINS;
                        //"916";
                } else if (codigoFilial == 2) {
                    contaDebitoCOFINS = Constantes.C_F2_FRETE_VENDA_DEBITO_COFINS;
                } else if (codigoFilial == 3) {
                    contaDebitoCOFINS = Constantes.C_F3_FRETE_VENDA_DEBITO_COFINS;
                }
                linhaDebitoCOFINS = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoCOFINS,"0",
                    valorCOFINS,codigoCOFINSHistorico,ctrFrete);
                conteudoArquivo.Append(linhaDebitoCOFINS);

                // Linha 6: Crédito de impostos e contribuições de COFINS
                string linhaCreditoCOFINS = null;
                string contaCreditoCOFINS = null;
                // Código da conta de Mercadoria para Revenda
                if (codigoFilial == 1) {
                    // Código da conta de Mercadoria para Revenda da Matriz
                    contaCreditoCOFINS = Constantes.C_F1_FRETE_VENDA_CREDITO_COFINS;
                        //"537";
                } else if (codigoFilial == 2) {
                    contaCreditoCOFINS = Constantes.C_F2_FRETE_VENDA_CREDITO_COFINS;
                } else if (codigoFilial == 3) {
                    contaCreditoCOFINS = Constantes.C_F3_FRETE_VENDA_CREDITO_COFINS;
                }
                linhaCreditoCOFINS = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoCOFINS,
                    valorCOFINS,codigoCOFINSHistorico,ctrFrete);
                conteudoArquivo.Append(linhaCreditoCOFINS);
            }
        }


        /**
         * 
         * Linhas Novo Formato OK
         * 
         * Constantes OK
         */
        public void geraArquivoExportacaoImportacao(DateTime dataInicial, DateTime dataFinal, string caminhoExportacao, bool matriz, bool recife, bool fortaleza) {
            string clausulaFiliais = geraClausulaWhereFiliais(matriz, recife, fortaleza);
            // Obtem um objeto do tipo Command
            SqlCommand comando = getConexao().CreateCommand();
            comando.CommandText =
                "SELECT * FROM N3_EntradasContabil WHERE "
                + "N3_EntradasContabil.DATA_ENTRADA >= '" + dataInicial.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + "AND N3_EntradasContabil.DATA_ENTRADA <= '" + dataFinal.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + clausulaFiliais;

            StringBuilder conteudoArquivo = new StringBuilder();
            SqlDataReader sqlReader = comando.ExecuteReader();
            while (sqlReader.Read()) {
                string tipoCompra = sqlReader["CODTMV"].ToString();
                DateTime dataEntrada = DateTime.Parse(sqlReader["DATA_ENTRADA"].ToString());
                Decimal valorLiquidoNF = (Decimal)sqlReader["VALORLIQUIDO"];
                int codigoFilial = int.Parse(sqlReader["CODFILIAL"].ToString());

                // Importação Direta N3 Computadores
                if (tipoCompra != null && tipoCompra.Equals("1.2.20") && codigoFilial ==  1) {
                    geraLinhasImportacao(conteudoArquivo, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    geraLinhasICMSCompras(conteudoArquivo, sqlReader, dataEntrada, codigoFilial, tipoCompra);
                    geraLinhasPISCompras(conteudoArquivo, sqlReader, dataEntrada, codigoFilial, tipoCompra);
                    geraLinhasCOFINSCompras(conteudoArquivo, sqlReader, dataEntrada, codigoFilial, tipoCompra);
                    geraLinhasIPIComprasImportacao(conteudoArquivo, sqlReader, dataEntrada, codigoFilial, tipoCompra);
                } else {
                    Console.WriteLine(tipoCompra + "  => Operação não mapeada");
                }
            }

            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_IMPORTACAO, caminhoExportacao, conteudoArquivo);

            sqlReader.Close();
            comando.Connection.Close();
        }

        /**
         * 
         *  Linhas novo Formato OK
         * 
         *  Constantes OK
         */
        private void geraLinhasImportacao(StringBuilder conteudoArquivo, SqlDataReader sqlReader, DateTime dataEntrada, Decimal valorLiquidoNF, int codigoFilial) {
            string notaFiscal = "\"NF:" + sqlReader["NUMEROMOV"] + "\"";
            // Linha 1: Debito no valor na conta de Materia Prima
            string contaMateriaPrima = null;
            string codigoMateriaPrimaHistorico = Constantes.C_F1_IMPORTACAO_HISTORICO;
            if (codigoFilial == 1) {
                contaMateriaPrima = Constantes.C_F1_IMPORTACAO_DEBITO;
                    //"490";
            } 

            string linhaDebitoMateriaPrima = geraLinhaTextoCreditoDebito(dataEntrada,contaMateriaPrima,"0",
                valorLiquidoNF,codigoMateriaPrimaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaDebitoMateriaPrima);

            // Linha 2: Crédito na conta do Fornecedor
            string contaFornecedorN3 = null;
            if (codigoFilial == 1) {
                contaFornecedorN3 = Constantes.C_F1_IMPORTACAO_CREDITO;
                    //"2281";
            } 

            string linhaCreditoFornecedor = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaFornecedorN3,
                valorLiquidoNF,codigoMateriaPrimaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaCreditoFornecedor);
        }

        /**
         * 
         *  Ajustado parcial, falta contas de estoque para devolução
         *  Linhas Novo Formato OK
         * 
         *  Constantes OK
         */
        public void geraArquivoExportacaoDevolucaoVendas(DateTime dataInicial, DateTime dataFinal, string caminhoExportacao, bool matriz, bool recife, bool fortaleza) {
            string clausulaFiliais = geraClausulaWhereFiliais(matriz, recife, fortaleza);
            //// Obtem um objeto do tipo Command
            SqlCommand comando = getConexao().CreateCommand();
            comando.CommandText =
                "SELECT * FROM N3_EntradasContabil WHERE "
                + "N3_EntradasContabil.DATA_ENTRADA >= '" + dataInicial.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + "AND N3_EntradasContabil.DATA_ENTRADA <= '" + dataFinal.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + clausulaFiliais;

            StringBuilder conteudoArquivoFilial_1 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_2 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_3 = new StringBuilder();

            SqlDataReader sqlReader = comando.ExecuteReader();
            while (sqlReader.Read()) {
                string tipoDevolucao = sqlReader["CODTMV"].ToString();
                DateTime dataEntrada = DateTime.Parse(sqlReader["DATA_ENTRADA"].ToString());
                Decimal valorLiquidoNF = (Decimal)sqlReader["VALORLIQUIDO"];
                int codigoFilial = int.Parse(sqlReader["CODFILIAL"].ToString());

    
                // Devolução de Vendas
                if (tipoDevolucao != null && ( tipoDevolucao.Equals("1.3.01") || tipoDevolucao.Equals("1.3.02")
                    || tipoDevolucao.Equals("1.3.03") || tipoDevolucao.Equals("1.3.04")
                    || tipoDevolucao.Equals("1.3.05") || tipoDevolucao.Equals("1.3.06")
                    || tipoDevolucao.Equals("1.3.07") || tipoDevolucao.Equals("1.3.08")
                    || tipoDevolucao.Equals("1.2.81") || tipoDevolucao.Equals("1.2.82")
                    || tipoDevolucao.Equals("1.2.83") || tipoDevolucao.Equals("1.2.84")))
                {
                    if (codigoFilial == 1) {
                        geraLinhasDevolucaoVendas(conteudoArquivoFilial_1, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial, tipoDevolucao);
                    } else if (codigoFilial == 2) {
                        geraLinhasDevolucaoVendas(conteudoArquivoFilial_2, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial, tipoDevolucao);
                    } else if (codigoFilial == 3) {
                        geraLinhasDevolucaoVendas(conteudoArquivoFilial_3, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial, tipoDevolucao);
                    }

                } 
            }

            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_DEVOLUCAO, caminhoExportacao, conteudoArquivoFilial_1);
            escreveArquivoExportacao("Filial_2_" + Constantes.ARQ_EXPORTACAO_DEVOLUCAO, caminhoExportacao, conteudoArquivoFilial_2);
            escreveArquivoExportacao("Filial_3_" + Constantes.ARQ_EXPORTACAO_DEVOLUCAO, caminhoExportacao, conteudoArquivoFilial_3);

            sqlReader.Close();
            comando.Connection.Close();
        }

        /**
         * 
         *  
         * 
         *  
         */
        public void geraArquivoExportacaoTransferenciaRecebida(DateTime dataInicial, DateTime dataFinal, string caminhoExportacao, bool matriz, bool recife, bool fortaleza) {
            string clausulaFiliais = geraClausulaWhereFiliais(matriz, recife, fortaleza);
            // Obtem um objeto do tipo Command
            SqlCommand comando = getConexao().CreateCommand();
            comando.CommandText =
                "SELECT * FROM N3_EntradasContabil WHERE "
                + "N3_EntradasContabil.DATA_ENTRADA >= '" + dataInicial.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + "AND N3_EntradasContabil.DATA_ENTRADA <= '" + dataFinal.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + clausulaFiliais;

            StringBuilder conteudoArquivoFilial_1 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_2 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_3 = new StringBuilder();

            SqlDataReader sqlReader = comando.ExecuteReader();
            while (sqlReader.Read()) {
                string tipoMovimento = sqlReader["CODTMV"].ToString();
                DateTime dataEntrada = DateTime.Parse(sqlReader["DATA_ENTRADA"].ToString());
                Decimal valorLiquidoNF = (Decimal)sqlReader["VALORLIQUIDO"];
                int codigoFilial = int.Parse(sqlReader["CODFILIAL"].ToString());
                // Devolução de Vendas
                if (tipoMovimento != null && (tipoMovimento.Equals("1.2.50") || tipoMovimento.Equals("1.2.51") || tipoMovimento.Equals("1.2.52")))  {
                    if (codigoFilial == 1) {
                        geraLinhasTransferenciaRecebida(conteudoArquivoFilial_1, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial, tipoMovimento);
                    } else if (codigoFilial == 2) {
                        geraLinhasTransferenciaRecebida(conteudoArquivoFilial_2, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial, tipoMovimento);
                    } else if (codigoFilial == 3) {
                        geraLinhasTransferenciaRecebida(conteudoArquivoFilial_3, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial, tipoMovimento);
                    }
                } 
            }
            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_TRANSFERENCIAS_RECEBIDA, caminhoExportacao, conteudoArquivoFilial_1);
            escreveArquivoExportacao("Filial_2_" + Constantes.ARQ_EXPORTACAO_TRANSFERENCIAS_RECEBIDA, caminhoExportacao, conteudoArquivoFilial_2);
            escreveArquivoExportacao("Filial_3_" + Constantes.ARQ_EXPORTACAO_TRANSFERENCIAS_RECEBIDA, caminhoExportacao, conteudoArquivoFilial_3);

            sqlReader.Close();
            comando.Connection.Close();
        }

        /*
         * 
         * 
         * 
         **/
        public void geraArquivoExportacaoTransferenciaEnviada(DateTime dataInicial, DateTime dataFinal, string caminhoExportacao, bool matriz, bool recife, bool fortaleza) {
            string clausulaFiliais = geraClausulaWhereFiliais(matriz, recife, fortaleza);
            // Obtem um objeto do tipo Command
            SqlCommand comando = getConexao().CreateCommand();
            comando.CommandText =
                "SELECT * FROM N3_SaidasContabil WHERE "
                + "N3_SaidasContabil.DATAEMISSAO >= '" + dataInicial.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + "AND N3_SaidasContabil.DATAEMISSAO <= '" + dataFinal.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + clausulaFiliais;

            StringBuilder conteudoArquivoFilial_1 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_2 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_3 = new StringBuilder();

            SqlDataReader sqlReader = comando.ExecuteReader();
            while (sqlReader.Read()) {
                string tipoMovimento = sqlReader["CODTMV"].ToString();
                DateTime dataEntrada = DateTime.Parse(sqlReader["DATAEMISSAO"].ToString());
                Decimal valorLiquidoNF = (Decimal)sqlReader["VALORLIQUIDO"];
                int codigoFilial = int.Parse(sqlReader["CODFILIAL"].ToString());
                // Devolução de Vendas
                if (tipoMovimento != null && (tipoMovimento.Equals("2.2.05") || tipoMovimento.Equals("2.2.17"))) {
                    if (codigoFilial == 1) {
                        geraLinhasTransferenciaEnviada(conteudoArquivoFilial_1, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial, tipoMovimento);
                    } else if (codigoFilial == 2) {
                        geraLinhasTransferenciaEnviada(conteudoArquivoFilial_2, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial, tipoMovimento);
                    } else if (codigoFilial == 3) {
                        geraLinhasTransferenciaEnviada(conteudoArquivoFilial_3, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial, tipoMovimento);
                    }
                } 
            }
            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_TRANSFERENCIAS_ENVIADA, caminhoExportacao, conteudoArquivoFilial_1);
            escreveArquivoExportacao("Filial_2_" + Constantes.ARQ_EXPORTACAO_TRANSFERENCIAS_ENVIADA, caminhoExportacao, conteudoArquivoFilial_2);
            escreveArquivoExportacao("Filial_3_" + Constantes.ARQ_EXPORTACAO_TRANSFERENCIAS_ENVIADA, caminhoExportacao, conteudoArquivoFilial_3);

            sqlReader.Close();
            comando.Connection.Close();
        }

        /*
         * Linhas novo Formato OK
         * 
         * Constantes OK
         * 
         **/
        private void geraLinhasTransferenciaEnviada(StringBuilder conteudoArquivo, SqlDataReader sqlReader, DateTime dataEntrada, decimal valorLiquidoNF, int codigoFilial, string tipoMovimento) {
            string notaFiscal = "\"NF:" + sqlReader["NUMEROMOV"] + "\"";
            // Linha 1: Debito no valor da Transferencia
             string contaDebitoTransferencia = null;
             string codigoTransferenciaHistorico = Constantes.C_F1_TRANSF_ENVIADA_HISTORICO;
             // Transferencia Enviada ou Recebida pela Matriz
            if (codigoFilial == 1) {
                contaDebitoTransferencia = Constantes.C_F1_TRANSF_ENVIADA_DEBITO;
            // Filial Recife
            } else if (codigoFilial == 2) {
                contaDebitoTransferencia = Constantes.C_F2_TRANSF_ENVIADA_DEBITO;
            // Filial FORTALEZA
            } else if (codigoFilial == 3) {
                contaDebitoTransferencia = Constantes.C_F3_TRANSF_ENVIADA_DEBITO;
            }
            string linhaDebitoTransferencia = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoTransferencia,"0",
                valorLiquidoNF,codigoTransferenciaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaDebitoTransferencia);


            // Linha 2: Credito no valor da Transferencia
             string contaCreditoTransferencia = null;
             // Transferencia Enviada ou Recebida pela Matriz
            if (codigoFilial == 1) {
                contaCreditoTransferencia = Constantes.C_F1_TRANSF_ENVIADA_CREDITO;
            // Filial Recife
            } else if (codigoFilial == 2) {
                contaCreditoTransferencia = Constantes.C_F2_TRANSF_ENVIADA_CREDITO;
            // Filial FORTALEZA
            } else if (codigoFilial == 3) {
                contaCreditoTransferencia = Constantes.C_F3_TRANSF_ENVIADA_CREDITO;
            }
            string linhaCreditoTransferencia = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoTransferencia,
                valorLiquidoNF,codigoTransferenciaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaCreditoTransferencia);

            Decimal valorICMS = (Decimal)sqlReader["ICMS"];
            if (!valorICMS.Equals(Decimal.Zero)) {
                // Linha 3: Débito na conta de ICMS a Recuperar
                string codigoICMSFaturadoHistorico = Constantes.C_F1_TRANSF_ENVIADA_HISTORICO_ICMS;
                string contaDebitoICMSRecuperar = null;
                if (codigoFilial == 1) {
                    contaDebitoICMSRecuperar = Constantes.C_F1_TRANSF_ENVIADA_DEBITO_ICMS;
                } else if (codigoFilial == 2) {
                    contaDebitoICMSRecuperar = Constantes.C_F2_TRANSF_ENVIADA_DEBITO_ICMS;
                } else if (codigoFilial == 3) {
                    contaDebitoICMSRecuperar = Constantes.C_F3_TRANSF_ENVIADA_DEBITO_ICMS;
                }
                string linhaDebitoICMSRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoICMSRecuperar,"0",
                    valorICMS,codigoICMSFaturadoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoICMSRecuperar);

                // Linha 4: Crédito na conta de ICMS Faturado
                string contaCreditoICMSFaturado = null;
                if (codigoFilial == 1) {
                    contaCreditoICMSFaturado = Constantes.C_F1_TRANSF_ENVIADA_CREDITO_ICMS;
                } else if (codigoFilial == 2) {
                    contaCreditoICMSFaturado = Constantes.C_F2_TRANSF_ENVIADA_CREDITO_ICMS;
                } else if (codigoFilial == 3) {
                    contaCreditoICMSFaturado = Constantes.C_F3_TRANSF_ENVIADA_CREDITO_ICMS;
                }
                string linhaCreditoICMSFaturado = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoICMSFaturado,
                    valorICMS,codigoICMSFaturadoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoICMSFaturado);

            }

            // Linha 5: Débito na conta de IPI a Recuperar
            Decimal valorIPI = (Decimal)sqlReader["IPI"];
            if (!valorIPI.Equals(Decimal.Zero)) {
                string contaDebitoIPIFaturado = null;
                string codigoIPIFaturadoHistorico = Constantes.C_F1_DEVOLUCAO_VENDA_HISTORICO_IPI;
                if (codigoFilial == 1) {
                    contaDebitoIPIFaturado = Constantes.C_F1_TRANSF_ENVIADA_DEBITO_IPI;
                } else if (codigoFilial == 2) {
                    contaDebitoIPIFaturado = Constantes.C_F2_TRANSF_ENVIADA_DEBITO_IPI;
                } else if (codigoFilial == 3) {
                    contaDebitoIPIFaturado = Constantes.C_F3_TRANSF_ENVIADA_DEBITO_IPI;
                }
                string linhaDebitoIPIRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoIPIFaturado,"0",
                    valorIPI,codigoIPIFaturadoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoIPIRecuperar);

                // Linha 6: Crédito na conta de IPI Faturado
                string contaCreditoIPIFaturado = null;
                if (codigoFilial == 1) {
                    contaCreditoIPIFaturado = Constantes.C_F1_TRANSF_ENVIADA_CREDITO_IPI;
                } else if (codigoFilial == 2) {
                    contaCreditoIPIFaturado = Constantes.C_F2_TRANSF_ENVIADA_CREDITO_IPI;
                } else if (codigoFilial == 3) {
                    contaCreditoIPIFaturado = Constantes.C_F3_TRANSF_ENVIADA_CREDITO_IPI;
                }
                string linhaCreditoIPIFaturado = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoIPIFaturado,
                    valorIPI,codigoIPIFaturadoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoIPIFaturado);
            }        
        }



        /*
         * Linhas novo Formato OK
         * 
         * Constantes OK
         * 
         **/
        private void geraLinhasTransferenciaRecebida(StringBuilder conteudoArquivo, SqlDataReader sqlReader, DateTime dataEntrada, decimal valorLiquidoNF, int codigoFilial, string tipoMovimento) {
            string notaFiscal = "\"NF:" + sqlReader["NUMEROMOV"] + "\"";
            // Linha 1: Debito no valor da Transferencia
             string contaDebitoTransferencia = null;
             string codigoTransferenciaHistorico = Constantes.C_F1_TRANSF_ENVIADA_HISTORICO;
             // Transferencia Enviada ou Recebida pela Matriz
            if (codigoFilial == 1) {
                contaDebitoTransferencia = Constantes.C_F1_TRANSF_RECEBIDA_DEBITO;
            // Filial Recife
            } else if (codigoFilial == 2) {     
                contaDebitoTransferencia = Constantes.C_F2_TRANSF_RECEBIDA_DEBITO;
            // Filial FORTALEZA
            } else if (codigoFilial == 3) {
                contaDebitoTransferencia = Constantes.C_F3_TRANSF_RECEBIDA_DEBITO;
            }
            string linhaDebitoTransferencia = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoTransferencia,"0",
                valorLiquidoNF,codigoTransferenciaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaDebitoTransferencia);


            // Linha 2: Credito no valor da Transferencia
             string contaCreditoTransferencia = null;
             // Transferencia Enviada ou Recebida pela Matriz
            if (codigoFilial == 1) {
                contaCreditoTransferencia = Constantes.C_F1_TRANSF_RECEBIDA_CREDITO;
            // Filial Recife
            } else if (codigoFilial == 2) {
                contaCreditoTransferencia = Constantes.C_F2_TRANSF_RECEBIDA_CREDITO;
            // Filial FORTALEZA
            } else if (codigoFilial == 3) {
                contaCreditoTransferencia = Constantes.C_F3_TRANSF_RECEBIDA_CREDITO;
            }
            string linhaCreditoTransferencia = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoTransferencia,
                valorLiquidoNF,codigoTransferenciaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaCreditoTransferencia);

            Decimal valorICMS = (Decimal)sqlReader["ICMS"];
            if (!valorICMS.Equals(Decimal.Zero)) {
                // Linha 3: Débito na conta de ICMS a Recuperar
                string codigoICMSFaturadoHistorico = Constantes.C_F1_TRANSF_ENVIADA_HISTORICO_ICMS;
                string contaDebitoICMSRecuperar = null;
                if (codigoFilial == 1) {
                    contaDebitoICMSRecuperar = Constantes.C_F1_TRANSF_RECEBIDA_DEBITO_ICMS;
                } else if (codigoFilial == 2) {
                    contaDebitoICMSRecuperar = Constantes.C_F2_TRANSF_RECEBIDA_DEBITO_ICMS;
                } else if (codigoFilial == 3) {
                    contaDebitoICMSRecuperar = Constantes.C_F3_TRANSF_RECEBIDA_DEBITO_ICMS;
                }
                string linhaDebitoICMSRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoICMSRecuperar,"0",
                    valorICMS,codigoICMSFaturadoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoICMSRecuperar);

                // Linha 4: Crédito na conta de ICMS Faturado
                string contaCreditoICMSFaturado = null;
                if (codigoFilial == 1) {
                    contaCreditoICMSFaturado = Constantes.C_F1_TRANSF_RECEBIDA_CREDITO_ICMS;
                } else if (codigoFilial == 2) {
                    contaCreditoICMSFaturado = Constantes.C_F2_TRANSF_RECEBIDA_CREDITO_ICMS;
                } else if (codigoFilial == 3) {
                    contaCreditoICMSFaturado = Constantes.C_F3_TRANSF_RECEBIDA_CREDITO_ICMS;
                }
                string linhaCreditoICMSFaturado = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoICMSFaturado,
                    valorICMS,codigoICMSFaturadoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoICMSFaturado);

            }

            // Linha 5: Débito na conta de IPI a Recuperar
            Decimal valorIPI = (Decimal)sqlReader["IPI"];
            if (!valorIPI.Equals(Decimal.Zero)) {
                string contaDebitoIPIFaturado = null;
                string codigoIPIFaturadoHistorico = Constantes.C_F1_TRANSF_RECEBIDA_HISTORICO_IPI;
                if (codigoFilial == 1) {
                    contaDebitoIPIFaturado = Constantes.C_F1_TRANSF_RECEBIDA_DEBITO_IPI;
                } else if (codigoFilial == 2) {
                    contaDebitoIPIFaturado = Constantes.C_F2_TRANSF_RECEBIDA_DEBITO_IPI;
                } else if (codigoFilial == 3) {
                    contaDebitoIPIFaturado = Constantes.C_F3_TRANSF_RECEBIDA_DEBITO_IPI;
                }
                string linhaDebitoIPIRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoIPIFaturado,"0",
                    valorIPI,codigoIPIFaturadoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoIPIRecuperar);

                // Linha 6: Crédito na conta de IPI Faturado
                string contaCreditoIPIFaturado = null;
                if (codigoFilial == 1) {
                    contaCreditoIPIFaturado = Constantes.C_F1_TRANSF_RECEBIDA_CREDITO_IPI;
                } else if (codigoFilial == 2) {
                    contaCreditoIPIFaturado = Constantes.C_F2_TRANSF_RECEBIDA_CREDITO_IPI;
                } else if (codigoFilial == 3) {
                    contaCreditoIPIFaturado = Constantes.C_F3_TRANSF_RECEBIDA_CREDITO_IPI;
                }
                string linhaCreditoIPIFaturado = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoIPIFaturado,
                    valorIPI,codigoIPIFaturadoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoIPIFaturado);
            }        
        }



        /**
         * 
         *  
         * 
         *  
         */
        public void geraArquivoExportacaoComprasUsoConsumo(DateTime dataInicial, DateTime dataFinal, string caminhoExportacao, bool matriz, bool recife, bool fortaleza) {
            string clausulaFiliais = geraClausulaWhereFiliais(matriz, recife, fortaleza);
            //// Obtem um objeto do tipo Command
            SqlCommand comando = getConexao().CreateCommand();
            comando.CommandText =
                "SELECT * FROM N3_EntradasContabil WHERE "
                + "N3_EntradasContabil.DATA_ENTRADA >= '" + dataInicial.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + "AND N3_EntradasContabil.DATA_ENTRADA <= '" + dataFinal.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + clausulaFiliais;

            StringBuilder conteudoArquivoFilial_1 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_2 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_3 = new StringBuilder();

            SqlDataReader sqlReader = comando.ExecuteReader();
            while (sqlReader.Read()) {
                string tipoMovimento = sqlReader["CODTMV"].ToString();
                DateTime dataEntrada = DateTime.Parse(sqlReader["DATA_ENTRADA"].ToString());
                Decimal valorLiquidoNF = (Decimal)sqlReader["VALORLIQUIDO"];
                int codigoFilial = int.Parse(sqlReader["CODFILIAL"].ToString());
                // Devolução de Vendas
                if (tipoMovimento != null && tipoMovimento.Equals("1.2.31")) {
                    if (codigoFilial == 1) {
                        geraLinhasComprasUsoConsumo(conteudoArquivoFilial_1, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    } else if (codigoFilial == 2) {
                        geraLinhasComprasUsoConsumo(conteudoArquivoFilial_2, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    } else if (codigoFilial == 3) {
                        geraLinhasComprasUsoConsumo(conteudoArquivoFilial_3, sqlReader, dataEntrada, valorLiquidoNF, codigoFilial);
                    }

                } 
            }

            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_COMPRAS_USO_CONSUMO, caminhoExportacao, conteudoArquivoFilial_1);
            escreveArquivoExportacao("Filial_2_" + Constantes.ARQ_EXPORTACAO_COMPRAS_USO_CONSUMO, caminhoExportacao, conteudoArquivoFilial_2);
            escreveArquivoExportacao("Filial_3_" + Constantes.ARQ_EXPORTACAO_COMPRAS_USO_CONSUMO, caminhoExportacao, conteudoArquivoFilial_3);

            sqlReader.Close();
            comando.Connection.Close();
        }

        /*
         * 
         * 
         * 
         * 
         **/
        private void geraLinhasComprasUsoConsumo(StringBuilder conteudoArquivo,SqlDataReader sqlReader,DateTime dataEntrada,decimal valorLiquidoNF,int codigoFilial) {
            if (valorLiquidoNF != null && !valorLiquidoNF.Equals(Decimal.Zero)) {
                string notaFiscal = "\"NF:" + sqlReader["NUMEROMOV"] + "\"";
                // Linha 1: Debito no valor da compra de Uso e Consumo
                string contaDebitoUsoConsumo = null;
                string codigoUsoConsumoHistorico = Constantes.C_F1_USO_CONSUMO_HISTORICO;
                // Compra para Revenda da Matriz Campina Grande
                if (codigoFilial == 1) {
                    contaDebitoUsoConsumo = Constantes.C_F1_USO_CONSUMO_DEBITO;
                        //"521";
                // Compra para Revenda na Filial Recife
                } else if (codigoFilial == 2) {
                    contaDebitoUsoConsumo = Constantes.C_F2_USO_CONSUMO_DEBITO;
                // Compra para Revenda na FILIAL Fortaleza
                } else if (codigoFilial == 3) {
                    contaDebitoUsoConsumo = Constantes.C_F3_USO_CONSUMO_DEBITO;
                }
                string linhaDebitoUsoConsumo = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoUsoConsumo,"0",
                    valorLiquidoNF,codigoUsoConsumoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoUsoConsumo);

                // Linha 2: Crédito na conta do Fornecedor
                string contaCreditoFornecedor = null;
                // Busca a conta do fornecedor
                contaCreditoFornecedor = obtemContaContabilFornecedor(sqlReader["CODCFO"].ToString(), sqlReader["CODTMV"].ToString(), sqlReader["NUMEROMOV"].ToString(), codigoFilial);
                if (contaCreditoFornecedor == null) {
                    contaCreditoFornecedor = "ERRO: FORNECEDOR NAO MAPEADO => " + sqlReader["CODCFO"].ToString();
                }
                string linhaCreditoFornecedor = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoFornecedor,
                    valorLiquidoNF,codigoUsoConsumoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoFornecedor);
            }
        }

        /**
         * 
         *  Ainda falta ajustes na conta de estoque da devolução
         *  Linhas Novo Formato OK
         * 
         *  Constantes OK
         */
        private void geraLinhasDevolucaoVendas(StringBuilder conteudoArquivo, SqlDataReader sqlReader, DateTime dataEntrada, Decimal valorLiquidoNF, int codigoFilial, string tipoDevolucao) {
            string notaFiscal = "\"NF:" + sqlReader["NUMEROMOV"] + "\"";
            // Linha 1: Debito no valor da NF em Vendas Canceladas
             string contaDebitoVendaCancelada = null;
             string codigoVendaCanceladaHistorico = Constantes.C_F1_DEVOLUCAO_VENDA_HISTORICO;
             // Compra para Revenda da Matriz Campina Grande
            if (codigoFilial == 1) {
                contaDebitoVendaCancelada = Constantes.C_F1_DEVOLUCAO_VENDA_DEBITO;
                    //"6280";
            // Venda Cancelada da Filial Recife
            } else if (codigoFilial == 2) {
                contaDebitoVendaCancelada = Constantes.C_F2_DEVOLUCAO_VENDA_DEBITO;
            // Venda Cancelada da Filial FORTALEZA
            } else if (codigoFilial == 3) {
                contaDebitoVendaCancelada = Constantes.C_F3_DEVOLUCAO_VENDA_DEBITO;
            }
            string linhaDebitoVendaCancelada = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoVendaCancelada,"0",
                valorLiquidoNF,codigoVendaCanceladaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaDebitoVendaCancelada);

            // Linha 2: Crédito na conta de Clientes
            string linhaCreditoClientes = geraLinhaTextoCreditoDebito(dataEntrada,"0",Constantes.C_F1_DEVOLUCAO_VENDA_CREDITO,
                valorLiquidoNF,codigoVendaCanceladaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaCreditoClientes);

            // Linha 3: Crédito na conta de IPI Faturado
            Decimal valorIPI = (Decimal)sqlReader["IPI"];
            if (!valorIPI.Equals(Decimal.Zero)) {
                string contaCreditoIPIFaturado = null;
                string codigoIPIFaturadoHistorico = Constantes.C_F1_DEVOLUCAO_VENDA_HISTORICO_IPI;
                if (codigoFilial == 1) {
                    contaCreditoIPIFaturado = Constantes.C_F1_DEVOLUCAO_VENDA_CREDITO_IPI;
                        //"6110";
                } else if (codigoFilial == 2) {
                    contaCreditoIPIFaturado = Constantes.C_F2_DEVOLUCAO_VENDA_CREDITO_IPI;
                } else if (codigoFilial == 3) {
                    contaCreditoIPIFaturado = Constantes.C_F3_DEVOLUCAO_VENDA_CREDITO_IPI;
                }
                string linhaCreditoIPIFaturado = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoIPIFaturado,
                    valorIPI,codigoIPIFaturadoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoIPIFaturado);


                // Linha 4: Débito na conta de IPI a Recuperar
                string contaDebitoIPIRecuperar = null;
                if (codigoFilial == 1) {
                    contaDebitoIPIRecuperar = Constantes.C_F1_DEVOLUCAO_VENDA_DEBITO_IPI;
                        //"922";
                } else if (codigoFilial == 2) {
                    contaDebitoIPIRecuperar = Constantes.C_F2_DEVOLUCAO_VENDA_DEBITO_IPI;
                } else if (codigoFilial == 3) {
                    contaDebitoIPIRecuperar = Constantes.C_F3_DEVOLUCAO_VENDA_DEBITO_IPI;
                }
                string linhaDebitoIPIRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoIPIRecuperar,"0",
                    valorIPI,codigoIPIFaturadoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoIPIRecuperar);
            }



            Decimal valorPIS = (Decimal)sqlReader["PIS"];
            if (!valorPIS.Equals(Decimal.Zero)) {
                string codigoPISDevolucaoHistorico = Constantes.C_F1_DEVOLUCAO_VENDA_HISTORICO_PIS;
                // Linha 5: Débito na conta de PIS a Recuperar
                string contaDebitoPISDevolucao = null;
                if (codigoFilial == 1) {
                    contaDebitoPISDevolucao = Constantes.C_F1_DEVOLUCAO_VENDA_DEBITO_PIS;
                        //"900";
                } else if (codigoFilial == 2) {
                    contaDebitoPISDevolucao = Constantes.C_F2_DEVOLUCAO_VENDA_DEBITO_PIS;
                } else if (codigoFilial == 3) {
                    contaDebitoPISDevolucao = Constantes.C_F3_DEVOLUCAO_VENDA_DEBITO_PIS;
                }
                string linhaDebitoPISRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoPISDevolucao,"0",
                    valorPIS,codigoPISDevolucaoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoPISRecuperar);

                
                // Linha 6: Crédito na conta de PIS devolução                
                string contaCreditoPISDevolucao = null;
                if (codigoFilial == 1) {
                    contaCreditoPISDevolucao = Constantes.C_F1_DEVOLUCAO_VENDA_CREDITO_PIS;
                        //"6280";
                } else if (codigoFilial == 2) {
                    contaCreditoPISDevolucao = Constantes.C_F2_DEVOLUCAO_VENDA_CREDITO_PIS;
                } else if (codigoFilial == 3) {
                    contaCreditoPISDevolucao = Constantes.C_F3_DEVOLUCAO_VENDA_CREDITO_PIS;
                }
                string linhaCreditoPISDevolucao = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoPISDevolucao,
                    valorPIS,codigoPISDevolucaoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoPISDevolucao);
            }


            
            Decimal valorCOFINS = (Decimal)sqlReader["COFINS"];
            if (!valorCOFINS.Equals(Decimal.Zero)) {
                string codigoCOFINSDevolucaoHistorico = Constantes.C_F1_DEVOLUCAO_VENDA_HISTORICO_COFINS;                
                // Linha 7: Débito na conta de COFINS a Recuperar
                string contaDebitoCOFINSRecuperar = null;
                if (codigoFilial == 1) {
                    contaDebitoCOFINSRecuperar = Constantes.C_F1_DEVOLUCAO_VENDA_DEBITO_COFINS;
                } else if (codigoFilial == 2) {
                    contaDebitoCOFINSRecuperar = Constantes.C_F2_DEVOLUCAO_VENDA_DEBITO_COFINS;
                } else if (codigoFilial == 3) {
                    contaDebitoCOFINSRecuperar = Constantes.C_F3_DEVOLUCAO_VENDA_DEBITO_COFINS;
                }
                string linhaDebitoCOFINSRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoCOFINSRecuperar,"0",
                    valorCOFINS,codigoCOFINSDevolucaoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoCOFINSRecuperar);

                // Linha 8: Crédito na conta de COFINS Devolução
                string contaCreditoCOFINSDevolucao = null;
                if (codigoFilial == 1) {
                    contaCreditoCOFINSDevolucao = Constantes.C_F1_DEVOLUCAO_VENDA_CREDITO_COFINS;
                        //"6280";
                } else if (codigoFilial == 2) {
                    contaCreditoCOFINSDevolucao = Constantes.C_F2_DEVOLUCAO_VENDA_CREDITO_COFINS;
                } else if (codigoFilial == 3) {
                    contaCreditoCOFINSDevolucao = Constantes.C_F3_DEVOLUCAO_VENDA_CREDITO_COFINS;
                }
                string linhaCreditoCOFINSDevolucao = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoCOFINSDevolucao,
                    valorCOFINS,codigoCOFINSDevolucaoHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoCOFINSDevolucao);
            }


            Decimal valorICMS = (Decimal)sqlReader["ICMS"];
            if (!valorICMS.Equals(Decimal.Zero)) {
                string codigoICMSRecuperarHistorico = Constantes.C_F1_DEVOLUCAO_VENDA_HISTORICO_ICMS;
                // Linha 9: Débito na conta de ICMS a Recuperar
                string contaDebitoICMSRecuperar = null;
                if (codigoFilial == 1) {
                    contaDebitoICMSRecuperar = Constantes.C_F1_DEVOLUCAO_VENDA_DEBITO_ICMS;
                        //"5731";
                } else if (codigoFilial == 2) {
                    contaDebitoICMSRecuperar = Constantes.C_F2_DEVOLUCAO_VENDA_DEBITO_ICMS;
                } else if (codigoFilial == 3) {
                    contaDebitoICMSRecuperar = Constantes.C_F3_DEVOLUCAO_VENDA_DEBITO_ICMS;
                }
                string linhaDebitoICMSRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoICMSRecuperar,"0",
                    valorICMS,codigoICMSRecuperarHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoICMSRecuperar);

                // Linha 10: Crédito na conta de ICMS Devolução
                string contaCreditoICMSDevolucao = null;
                if (codigoFilial == 1) {
                    contaCreditoICMSDevolucao = Constantes.C_F1_DEVOLUCAO_VENDA_CREDITO_ICMS;
                        //"6191";
                } else if (codigoFilial == 2) {
                    contaCreditoICMSDevolucao = Constantes.C_F2_DEVOLUCAO_VENDA_CREDITO_ICMS;
                } else if (codigoFilial == 3) {
                    contaCreditoICMSDevolucao = Constantes.C_F3_DEVOLUCAO_VENDA_CREDITO_ICMS;
                }
                string linhaCreditoICMSDevolucao = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoICMSDevolucao,
                    valorICMS,codigoICMSRecuperarHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoICMSDevolucao);
            }

            /*
            // Linha 10: Debita Estoque de Origem no Custo de Venda
            Decimal valorCUSTOENTRADA = (Decimal)sqlReader["CUSTO_ENTRADA"];
            // O Custo da devolução não estando ZERADO pode-se fazer o lançamento
            if (!valorCUSTOENTRADA.Equals(Decimal.Zero)) {
                string contaDebitoCustoVenda = null;
                string codigoCustoVendaHistorico = Constantes.C_F1_DEVOLUCAO_VENDA_HISTORICO_CUSTO;
                // Devolução de Industrialização
                if (tipoDevolucao != null && ( tipoDevolucao.Equals("1.3.01") || tipoDevolucao.Equals("1.3.02")
                        || tipoDevolucao.Equals("1.3.03") || tipoDevolucao.Equals("1.3.04")
                        || tipoDevolucao.Equals("1.2.81") || tipoDevolucao.Equals("1.2.82"))) {
                    // Devolução de Produto Incentivado
                    if (valorPIS.Equals(Decimal.Zero) && valorCOFINS.Equals(Decimal.Zero)) {
                        contaDebitoCustoVenda = Constantes.C_F1_DEVOLUCAO_VENDA_PROD_MPBEM_DEBITO_CUSTO;
                            //"5569";
                    // Devolução de Produto não incentivado
                    } else {
                        contaDebitoCustoVenda = Constantes.C_F1_DEVOLUCAO_VENDA_PROD_DEBITO_CUSTO;
                            //"5575";
                    }
                    // Devolução de Comercialização
                } else if (tipoDevolucao != null && ( tipoDevolucao.Equals("1.3.05") || tipoDevolucao.Equals("1.3.06")
                        || tipoDevolucao.Equals("1.3.07") || tipoDevolucao.Equals("1.3.08")
                        || tipoDevolucao.Equals("1.2.83") || tipoDevolucao.Equals("1.2.84")))
                {
                    // Devolução de Mercadoria Incentivado
                    if (valorPIS.Equals(Decimal.Zero) && valorCOFINS.Equals(Decimal.Zero)) {
                        if (codigoFilial == 1) {
                            contaDebitoCustoVenda = Constantes.C_F1_DEVOLUCAO_VENDA_MPBEM_DEBITO_CUSTO;
                                //"5581";
                        } else if (codigoFilial == 2) {
                            //contaDebitoCustoVenda = Constantes.C_F2_DEVOLUCAO_VENDA_MERC_MPBEM_CREDITO_CUSTO;
                        } else if (codigoFilial == 3) {
                            //contaDebitoCustoVenda = Constantes.C_F3_DEVOLUCAO_VENDA_MERC_MPBEM_CREDITO_CUSTO;
                        }
                        // Devolução de Mercadoria não incentivado
                    } else {
                        if (codigoFilial == 1) {
                            contaDebitoCustoVenda = Constantes.C_F1_DEVOLUCAO_VENDA_DEBITO_CUSTO;
                            //"5598";
                        } else if (codigoFilial == 2) {
                            contaDebitoCustoVenda = Constantes.C_F2_DEVOLUCAO_VENDA_DEBITO_CUSTO;
                        } else if (codigoFilial == 3) {
                            contaDebitoCustoVenda = Constantes.C_F3_DEVOLUCAO_VENDA_DEBITO_CUSTO;
                        }
                    }
                }
                string linhaDebitoCustoVenda = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoCustoVenda,"0",
                    valorCUSTOENTRADA,codigoCustoVendaHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoCustoVenda);


                // Linha 11: Credita Custo da Mercadoria/Produto
                // Devolução de Industrialização
                string contaCreditoCustoVenda = null;
                if (tipoDevolucao != null && ( tipoDevolucao.Equals("1.3.01") || tipoDevolucao.Equals("1.3.02")
                        || tipoDevolucao.Equals("1.3.03") || tipoDevolucao.Equals("1.3.04") )) {
                    // Devolução de Produto Incentivado
                    if (valorPIS.Equals(Decimal.Zero) && valorCOFINS.Equals(Decimal.Zero)) {
                        contaCreditoCustoVenda = Constantes.C_F1_DEVOLUCAO_VENDA_PROD_MPBEM_CREDITO_CUSTO;
                            //"6511";
                        // Devolução de Produto não incentivado
                    } else {
                        contaCreditoCustoVenda = Constantes.C_F1_DEVOLUCAO_VENDA_PROD_CREDITO_CUSTO;
                            //"6541";
                    }
                    // Devolução de Comercialização
                } else if (tipoDevolucao != null && ( tipoDevolucao.Equals("1.3.05") || tipoDevolucao.Equals("1.3.06")
                        || tipoDevolucao.Equals("1.3.07") || tipoDevolucao.Equals("1.3.08") )) {
                    // Devolução de Mercadoria Incentivado
                    if (valorPIS.Equals(Decimal.Zero) && valorCOFINS.Equals(Decimal.Zero)) {
                        if (codigoFilial == 1) {
                            contaCreditoCustoVenda = Constantes.C_F1_DEVOLUCAO_VENDA_MPBEM_CREDITO_CUSTO;
                                //"6592";
                        } else if (codigoFilial == 2) {
                            //contaCreditoCustoVenda = "6592";
                         } else if (codigoFilial == 3) {
                            //contaCreditoCustoVenda = "6592";
                         }
                    } else {
                        if (codigoFilial == 1) {
                            contaCreditoCustoVenda = Constantes.C_F1_DEVOLUCAO_VENDA_CREDITO_CUSTO;
                                //"6617";
                            // Devolução de Mercadoria não incentivado
                        } else if (codigoFilial == 2) {
                            contaCreditoCustoVenda = Constantes.C_F2_DEVOLUCAO_VENDA_CREDITO_CUSTO;
                         } else if (codigoFilial == 3) {
                            contaCreditoCustoVenda = Constantes.C_F3_DEVOLUCAO_VENDA_CREDITO_CUSTO;
                         }
                    }
                }
                string linhaCreditoCustoVenda = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoCustoVenda,
                    valorCUSTOENTRADA,codigoCustoVendaHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoCustoVenda);
            }
             */
        }


        /**
         * 
         *  AJustado
         *  Linhas Novo Formato OK
         * 
         *  Constantes OK
         */
        public void geraArquivoExportacaoCompras(DateTime dataInicial, DateTime dataFinal, string caminhoExportacao, bool matriz, bool recife, bool fortaleza) {
            string clausulaFiliais = geraClausulaWhereFiliais(matriz, recife, fortaleza);
            // Obtem um objeto do tipo Command
            SqlCommand comando = getConexao().CreateCommand();
            comando.CommandText =
                "SELECT * FROM N3_EntradasContabil WHERE "
                + "N3_EntradasContabil.DATA_ENTRADA >= '" + dataInicial.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + "AND N3_EntradasContabil.DATA_ENTRADA <= '" + dataFinal.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + clausulaFiliais;

            StringBuilder conteudoArquivoFilial_1 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_2 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_3 = new StringBuilder();
            SqlDataReader sReaderCompras = comando.ExecuteReader();
            while (sReaderCompras.Read()) {
                string tipoCompra = sReaderCompras["CODTMV"].ToString();
                DateTime dataEntrada = DateTime.Parse(sReaderCompras["DATA_ENTRADA"].ToString());
                Decimal valorLiquidoNF = (Decimal)sReaderCompras["VALORLIQUIDO"];
                int codigoFilial = int.Parse(sReaderCompras["CODFILIAL"].ToString());
                // Compras para Comercialização
                if (tipoCompra != null && tipoCompra.Equals("1.2.01")) {
                    if (codigoFilial == 1) {
                        geraLinhasComprasRevenda(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, valorLiquidoNF, codigoFilial);
                        // Novo Regime Especial, compras não tem crédito de ICMS - Alteração em 10/10/2014
                        //geraLinhasICMSCompras(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasPISCompras(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasCOFINSCompras(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasIPIComprasImportacao(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                    } else if (codigoFilial == 2) {
                        geraLinhasComprasRevenda(conteudoArquivoFilial_2, sReaderCompras, dataEntrada, valorLiquidoNF, codigoFilial);
                        geraLinhasICMSCompras(conteudoArquivoFilial_2, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasPISCompras(conteudoArquivoFilial_2, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasCOFINSCompras(conteudoArquivoFilial_2, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasIPIComprasImportacao(conteudoArquivoFilial_2, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                    } else if (codigoFilial == 3) {
                        geraLinhasComprasRevenda(conteudoArquivoFilial_3, sReaderCompras, dataEntrada, valorLiquidoNF, codigoFilial);
                        geraLinhasICMSCompras(conteudoArquivoFilial_3, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasPISCompras(conteudoArquivoFilial_3, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasCOFINSCompras(conteudoArquivoFilial_3, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasIPIComprasImportacao(conteudoArquivoFilial_3, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                    }
                // Compras para Industrialização
                } else if (tipoCompra != null && tipoCompra.Equals("1.2.05")) {
                    if (codigoFilial == 1) {
                        geraLinhasComprasIndustrializacao(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, valorLiquidoNF, codigoFilial);
                        // Novo Regime Especial, compras não tem crédito de ICMS - Alteração em 10/10/2014
                        //geraLinhasICMSCompras(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasPISCompras(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasCOFINSCompras(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasIPIComprasImportacao(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                    }
                // Compras Bonificadas
                } else if (tipoCompra != null && tipoCompra.Equals("1.2.16")) {
                    if (codigoFilial == 1) {
                        geraLinhasComprasBonificadas(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, valorLiquidoNF, codigoFilial);
                        // Novo Regime Especial, compras não tem crédito de ICMS - Alteração em 10/10/2014
                        //geraLinhasICMSCompras(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasPISCompras(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasCOFINSCompras(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasIPIComprasImportacao(conteudoArquivoFilial_1, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                    } else if (codigoFilial == 2) {
                        geraLinhasComprasBonificadas(conteudoArquivoFilial_2, sReaderCompras, dataEntrada, valorLiquidoNF, codigoFilial);
                        geraLinhasICMSCompras(conteudoArquivoFilial_2, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasPISCompras(conteudoArquivoFilial_2, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasCOFINSCompras(conteudoArquivoFilial_2, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasIPIComprasImportacao(conteudoArquivoFilial_2, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                    } else if (codigoFilial == 3) {
                        geraLinhasComprasBonificadas(conteudoArquivoFilial_3, sReaderCompras, dataEntrada, valorLiquidoNF, codigoFilial);
                        geraLinhasICMSCompras(conteudoArquivoFilial_3, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasPISCompras(conteudoArquivoFilial_3, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasCOFINSCompras(conteudoArquivoFilial_3, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                        geraLinhasIPIComprasImportacao(conteudoArquivoFilial_3, sReaderCompras, dataEntrada, codigoFilial, tipoCompra);
                    } 
                }
            }

            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_COMPRAS, caminhoExportacao, conteudoArquivoFilial_1);
            escreveArquivoExportacao("Filial_2_" + Constantes.ARQ_EXPORTACAO_COMPRAS, caminhoExportacao, conteudoArquivoFilial_2);
            escreveArquivoExportacao("Filial_3_" + Constantes.ARQ_EXPORTACAO_COMPRAS, caminhoExportacao, conteudoArquivoFilial_3);

            sReaderCompras.Close();
            comando.Connection.Close();
        }

        /**
         * 
         *  Revisado a verfica a compra bonificada das filiais
         *  Linhas Formato Novo OK
         * 
         *  Constantes OK
         */
        private void geraLinhasComprasBonificadas(StringBuilder conteudoArquivo, SqlDataReader sReaderCompras, DateTime dataEntrada, Decimal valorLiquidoNF, int codigoFilial) {
            string notaFiscal = "\"NF:" + sReaderCompras["NUMEROMOV"] + "\"";
            // Linha 1: Debito no valor do estoque de mercadoria para Revenda
            string contaDebitoEstoqueMerc = null;
            string codigoEstoqueMercHistorico = Constantes.C_F1_COMPRA_BONIFICADA_HISTORICO;
            if (codigoFilial == 1) {
                contaDebitoEstoqueMerc = Constantes.C_F1_COMPRA_BONIFICADA_DEBITO;
                    //"521";
            } else if (codigoFilial == 2) {
                contaDebitoEstoqueMerc = Constantes.C_F2_COMPRA_BONIFICADA_DEBITO;
            } else if (codigoFilial == 3) {
                contaDebitoEstoqueMerc = Constantes.C_F3_COMPRA_BONIFICADA_DEBITO;
            }
            string linhaDebitoEstoqueMerc = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoEstoqueMerc,"0",
                valorLiquidoNF,codigoEstoqueMercHistorico,notaFiscal);
            conteudoArquivo.Append(linhaDebitoEstoqueMerc);

            // Linha 2: Crédito na conta do Fornecedor
            string contaCreditoFornecedor = null;
            if (codigoFilial == 1) {
                contaCreditoFornecedor = Constantes.C_F1_COMPRA_BONIFICADA_CREDITO;
                //"6468";
            } else if (codigoFilial == 2) {
                contaCreditoFornecedor = Constantes.C_F2_COMPRA_BONIFICADA_CREDITO;
            } else if (codigoFilial == 3) {
                contaCreditoFornecedor = Constantes.C_F3_COMPRA_BONIFICADA_CREDITO;
            }
            string linhaCreditoEstoqueMerc = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoFornecedor,
                valorLiquidoNF,codigoEstoqueMercHistorico,notaFiscal);
            conteudoArquivo.Append(linhaCreditoEstoqueMerc);
        }


        /**
         * 
         * Ajustado, verificar código da mensagem
         * Linhas Formato Novo OK
         * 
         *  Constantes OK
         */
        private void geraLinhasComprasRevenda(StringBuilder conteudoArquivo, SqlDataReader sReaderCompras, DateTime dataEntrada, Decimal valorLiquidoNF, int codigoFilial) {
            string notaFiscal = "\"NF:" + sReaderCompras["NUMEROMOV"] + "\"";
            // Linha 1: Debito no valor do estoque de mercadoria para Revenda
            string contaDebitoEstoqueRevenda = null;
            string codigoEstoqueRevendaHistorico = Constantes.C_F1_COMPRA_REVENDA_HISTORICO;
            // Compra para Revenda da Matriz Campina Grande
            if (codigoFilial == 1) {
                contaDebitoEstoqueRevenda = Constantes.C_F1_COMPRA_REVENDA_DEBITO;
                    //"521";
            // Compra para Revenda na Filial Recife
            } else if (codigoFilial == 2) {
                contaDebitoEstoqueRevenda = Constantes.C_F2_COMPRA_REVENDA_DEBITO;
            // Compra para Revenda na FILIAL Fortaleza
            } else if (codigoFilial == 3) {
                contaDebitoEstoqueRevenda = Constantes.C_F3_COMPRA_REVENDA_DEBITO;
            }
            string linhaDebitoEstoqueRevenda = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoEstoqueRevenda,"0",
                valorLiquidoNF,codigoEstoqueRevendaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaDebitoEstoqueRevenda);

            // Linha 2: Crédito na conta do Fornecedor
            string contaCreditoFornecedor = null;
            // Busca a conta do fornecedor
            contaCreditoFornecedor = obtemContaContabilFornecedor(sReaderCompras["CODCFO"].ToString(), sReaderCompras["CODTMV"].ToString(), sReaderCompras["NUMEROMOV"].ToString(), codigoFilial);
            if (contaCreditoFornecedor == null) {
                contaCreditoFornecedor = "ERRO: FORNECEDOR NAO MAPEADO => " + sReaderCompras["CODCFO"].ToString();
            }
            string linhaCreditoFornecedor = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoFornecedor,
                valorLiquidoNF,codigoEstoqueRevendaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaCreditoFornecedor);
        }

        /**
         * 
         *  Ajustado
         *  Linhas Formato Novo OK
         * 
         *  Constantes OK
         */
        private void geraLinhasComprasIndustrializacao(StringBuilder conteudoArquivo, SqlDataReader sReaderCompras, DateTime dataEntrada, Decimal valorLiquidoNF, int codigoFilial) {
            // Linha 1: Debito no valor do estoque de mercadoria para Industrialização
            string notaFiscal = "\"NF:" + sReaderCompras["NUMEROMOV"] + "\"";
            string contaDebitoEstoqueInd = null;
            string codigoEstoqueIndHistorico = Constantes.C_F1_COMPRA_INDUST_HISTORICO;
            if (codigoFilial == 1) {
                contaDebitoEstoqueInd = Constantes.C_F1_COMPRA_INDUST_CREDITO;
                    //"483";
            } 
            string linhaDebitoEstoqueInd = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoEstoqueInd,"0",
                valorLiquidoNF,codigoEstoqueIndHistorico,notaFiscal);
            conteudoArquivo.Append(linhaDebitoEstoqueInd);

            // Linha 2: Crédito na conta do Fornecedor
            string contaCreditoFornecedor = null;
            // Busca a conta do fornecedor
            contaCreditoFornecedor = obtemContaContabilFornecedor(sReaderCompras["CODCFO"].ToString(), sReaderCompras["CODTMV"].ToString(), sReaderCompras["NUMEROMOV"].ToString(), codigoFilial);
            if (contaCreditoFornecedor == null) {
                contaCreditoFornecedor = "ERRO: FORNECEDOR NAO MAPEADO => " + sReaderCompras["CODCFO"].ToString();
            }
            string linhaCreditoFornecedor = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoFornecedor,
                valorLiquidoNF,codigoEstoqueIndHistorico,notaFiscal);
            conteudoArquivo.Append(linhaCreditoFornecedor);
         }

        /**
         * 
         *   Linhas Formato Novo OK
         * 
         *  Constantes OK
         */
        private void geraLinhasICMSCompras(StringBuilder conteudoArquivo, SqlDataReader sReaderCompras, DateTime dataEntrada, int codigoFilial, string tipoCompra) {
            string notaFiscal = "\"NF:" + sReaderCompras["NUMEROMOV"] + "\"";
            Decimal valorICMS = (Decimal)sReaderCompras["ICMS"];
            if (!valorICMS.Equals(Decimal.Zero)) {
                // Linha 3: Débito de impostos de ICMS Revenda a recuperar
                string contaDebitoICMSRecuperar = null;
                string codigoICMSRecuperarHistorico = Constantes.C_F1_COMPRA_REVENDA_HISTORICO_ICMS;
                // Compra para Revenda da Matriz Campina Grande
                if (codigoFilial == 1) {
                    // Código da conta de ICMS da Matriz
                    contaDebitoICMSRecuperar = Constantes.C_F1_COMPRA_REVENDA_DEBITO_ICMS;
                        //"5731";
                    // Faturamento da Filial Recife
                } else if (codigoFilial == 2) {
                    // Código da conta de ICMS da Filial
                    contaDebitoICMSRecuperar = Constantes.C_F2_COMPRA_REVENDA_DEBITO_ICMS;
                        //"8616";
                } else if (codigoFilial == 3) {
                    // Código da conta de ICMS da Filial
                    contaDebitoICMSRecuperar = Constantes.C_F3_COMPRA_REVENDA_DEBITO_ICMS;
                }

                string linhaDebitoICMSRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoICMSRecuperar,"0",
                    valorICMS,codigoICMSRecuperarHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoICMSRecuperar);

                // Linha 4: Crédito de impostos na Conta de Estoque de Mercadorias para Revenda
                string contaCreditoEstoqueMerc = null;
                // Compra para Revenda da Matriz Campina Grande
                if (codigoFilial == 1 && tipoCompra.Equals("1.2.01")) {
                    // Código da conta Mercadoria para Revenda da Matriz
                    contaCreditoEstoqueMerc = Constantes.C_F1_COMPRA_REVENDA_CREDITO_ICMS;
                        //"521";
                } else if (codigoFilial == 2 && tipoCompra.Equals("1.2.01")) {
                    // Código da conta Mercadoria para Revenda da Filial RECIFE
                    contaCreditoEstoqueMerc = Constantes.C_F2_COMPRA_REVENDA_CREDITO_ICMS;
                        //"5641";
                } else if (codigoFilial == 3 && tipoCompra.Equals("1.2.01")) {
                    // Código da conta Mercadoria para Revenda da Filial FORTALEZA
                    contaCreditoEstoqueMerc = Constantes.C_F3_COMPRA_REVENDA_CREDITO_ICMS;
                        //"10613";
                } else if (codigoFilial == 1 && tipoCompra.Equals("1.2.16")) {
                    // Código da conta Materia Prima Nacional revenda, bonificada
                    contaCreditoEstoqueMerc = Constantes.C_F1_COMPRA_BONIFICADA_CREDITO_ICMS;
                        //"521";
                    // Código de Filial Inválida
                } else if (codigoFilial == 2 && tipoCompra.Equals("1.2.16")) {
                    // Código da conta Materia Prima Nacional revenda, bonificada
                    contaCreditoEstoqueMerc = Constantes.C_F2_COMPRA_BONIFICADA_CREDITO_ICMS;
                    // Código de Filial Inválida
                } else if (codigoFilial == 3 && tipoCompra.Equals("1.2.16")) {
                    // Código da conta Materia Prima Nacional revenda, bonificada
                    contaCreditoEstoqueMerc = Constantes.C_F3_COMPRA_BONIFICADA_CREDITO_ICMS;
                    // Código de Filial Inválida
                } else if (codigoFilial == 1 && tipoCompra.Equals("1.2.05")) {
                    // Código da conta Materia Prima Nacional para Industria
                    contaCreditoEstoqueMerc = Constantes.C_F1_COMPRA_INDUST_CREDITO;
                    // Código de Filial Inválida
                } else if (codigoFilial == 1 && tipoCompra.Equals("1.2.20")) {
                    // Código da conta Materia Prima Internacional para Industria
                    contaCreditoEstoqueMerc = Constantes.C_F1_COMPRA_REVENDA_CREDITO_ICMS;
                }

                string linhaCreditoEstoqueMerc = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoEstoqueMerc,
                    valorICMS,codigoICMSRecuperarHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoEstoqueMerc);
            }
        }

        /**
         * 
         *   Linhas Formato Novo OK
         * 
         *  Constantes OK
         * 
         */
        private void geraLinhasPISCompras(StringBuilder conteudoArquivo, SqlDataReader sReaderCompras, DateTime dataEntrada, int codigoFilial, string tipoCompra) {
            string notaFiscal = "\"NF:" + sReaderCompras["NUMEROMOV"] + "\"";
            string codigoPISRecuperarHistorico = Constantes.C_F1_COMPRA_REVENDA_HISTORICO_PIS;
            // Linha 5: Débito de impostos de PIS incidentes sobre Venda
            Decimal valorPIS = (Decimal)sReaderCompras["PIS"];
            if (!valorPIS.Equals(Decimal.Zero)) {
                // Código da conta de PIS a recuperar
                string contaDebitoPISRecuperar = null;
                if (codigoFilial == 1) {
                    contaDebitoPISRecuperar = Constantes.C_F1_COMPRA_REVENDA_DEBITO_PIS;
                        //"900";
                } else if (codigoFilial == 2) {
                    contaDebitoPISRecuperar = Constantes.C_F2_COMPRA_REVENDA_DEBITO_PIS;
                } else if (codigoFilial == 3) {
                    contaDebitoPISRecuperar = Constantes.C_F3_COMPRA_REVENDA_DEBITO_PIS;
                }

                string linhaDebitoPISRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoPISRecuperar,"0",
                    valorPIS,codigoPISRecuperarHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoPISRecuperar);

                // Linha 6: Crédito de PIS na conta de Mercadoria para Revenda
                string contaCreditoPISMerc = null;
                // Código da conta Mercadorias para Revenda da Matriz
                if (codigoFilial == 1 && tipoCompra.Equals("1.2.01")) {
                    contaCreditoPISMerc = Constantes.C_F1_COMPRA_REVENDA_CREDITO_PIS;
                        //"521";
                    // Código da conta Mercadorias para Revenda da Filial RECIFE
                } else if (codigoFilial == 2 && tipoCompra.Equals("1.2.01")) {
                    contaCreditoPISMerc = Constantes.C_F2_COMPRA_REVENDA_CREDITO_PIS;
                    // Código da conta Mercadorias para Revenda da Filial FORTALEZA
                } else if (codigoFilial == 3 && tipoCompra.Equals("1.2.01")) {
                    contaCreditoPISMerc = Constantes.C_F3_COMPRA_REVENDA_CREDITO_PIS;
                    // Código da conta Materia Prima Nacional para Industrialização
                } else if (codigoFilial == 1 && tipoCompra.Equals("1.2.05")) {
                    contaCreditoPISMerc = Constantes.C_F1_COMPRA_INDUST_CREDITO_PIS;
                        //"483";
                } else if (codigoFilial == 1 && tipoCompra.Equals("1.2.20")) {
                    contaCreditoPISMerc = Constantes.C_F1_IMPORTACAO_CREDITO_PIS;
                        //"490";
                } else if (codigoFilial == 1 && tipoCompra.Equals("1.2.16")) {
                    contaCreditoPISMerc = Constantes.C_F1_COMPRA_BONIFICADA_CREDITO_PIS;
                        //"521";
                } else if (codigoFilial == 2 && tipoCompra.Equals("1.2.16")) {
                    contaCreditoPISMerc = Constantes.C_F2_COMPRA_BONIFICADA_CREDITO_PIS;
                } else if (codigoFilial == 3 && tipoCompra.Equals("1.2.16")) {
                    contaCreditoPISMerc = Constantes.C_F3_COMPRA_BONIFICADA_CREDITO_PIS;
                }

                string linhaCreditoPISMerc = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoPISMerc,
                    valorPIS,codigoPISRecuperarHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoPISMerc);
            }
        }

        /**
         * 
         *  Linhas Formato Novo OK
         * 
         *  Constantes OK
         */
        private void geraLinhasCOFINSCompras(StringBuilder conteudoArquivo, SqlDataReader sReaderCompras, DateTime dataEntrada, int codigoFilial, string tipoCompra) {
            string notaFiscal = "\"NF:" + sReaderCompras["NUMEROMOV"] + "\"";
            // Linha 7: Débito de impostos de COFINS a recuperar
            Decimal valorCOFINS = (Decimal)sReaderCompras["COFINS"];
            if (!valorCOFINS.Equals(Decimal.Zero)) {
               // Conta de COFINS a recuperar da Matriz/Filial
                string contaDebitoCOFINSRecuperar = null;
                string codigoCOFINSRecuperarHistorico = Constantes.C_F1_COMPRA_REVENDA_HISTORICO_COFINS;
                if (codigoFilial == 1) {
                    contaDebitoCOFINSRecuperar = Constantes.C_F1_COMPRA_REVENDA_DEBITO_COFINS;
                        //"916";
                } else if (codigoFilial == 2) {
                    contaDebitoCOFINSRecuperar = Constantes.C_F2_COMPRA_REVENDA_DEBITO_COFINS;
                } else if (codigoFilial == 3) {
                    contaDebitoCOFINSRecuperar = Constantes.C_F3_COMPRA_REVENDA_DEBITO_COFINS;
                }

                string linhaDebitoCOFINSRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoCOFINSRecuperar,"0",
                    valorCOFINS,codigoCOFINSRecuperarHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoCOFINSRecuperar);

                // Linha 8: Crédito de impostos e contribuições de COFINS
                string contaCreditoCOFINSMerc = null;
                // Código da conta Compras de Mercadoria para Revenda
                // Matriz Campina Grande
                if (codigoFilial == 1 && tipoCompra.Equals("1.2.01")) {
                    // Código da conta de Mercadoria para Revenda da Matriz
                    contaCreditoCOFINSMerc = Constantes.C_F1_COMPRA_REVENDA_CREDITO_COFINS;
                        //"521";
                    // Filial Recife
                } else if (codigoFilial == 2 && tipoCompra.Equals("1.2.01")) {
                    // Código da conta de Mercadoria para Revenda da Filial RECIFE
                    contaCreditoCOFINSMerc = Constantes.C_F2_COMPRA_REVENDA_CREDITO_COFINS;
                } else if (codigoFilial == 3 && tipoCompra.Equals("1.2.01")) {
                    // Código da conta de Mercadoria para Revenda da Filial FORTALEZA
                    contaCreditoCOFINSMerc = Constantes.C_F3_COMPRA_REVENDA_CREDITO_COFINS;
                    // Código da conta Materia Prima Nacional para Industrialização
                } else if (codigoFilial == 1 && tipoCompra.Equals("1.2.05")) {
                    contaCreditoCOFINSMerc = Constantes.C_F1_COMPRA_INDUST_CREDITO_COFINS;
                        //"483";
                } else if (codigoFilial == 1 && tipoCompra.Equals("1.2.20")) {
                    contaCreditoCOFINSMerc = Constantes.C_F1_IMPORTACAO_CREDITO_COFINS;
                        //"490";
                } else if (codigoFilial == 1 && tipoCompra.Equals("1.2.16")) {
                    contaCreditoCOFINSMerc = Constantes.C_F1_COMPRA_BONIFICADA_CREDITO_COFINS;
                        //"521";
                } else if (codigoFilial == 2 && tipoCompra.Equals("1.2.16")) {
                    contaCreditoCOFINSMerc = Constantes.C_F2_COMPRA_BONIFICADA_CREDITO_COFINS;
                } else if (codigoFilial == 3 && tipoCompra.Equals("1.2.16")) {
                    contaCreditoCOFINSMerc = Constantes.C_F3_COMPRA_BONIFICADA_CREDITO_COFINS;
                }
                string linhaCreditoCOFINSMerc = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoCOFINSMerc,
                    valorCOFINS,codigoCOFINSRecuperarHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoCOFINSMerc);
            }
        }


        /**
         * 
         *  verificar caso IPI para as Filiais
         *  Linhas Novo Formato OK
         * 
         *  Constantes OK
         */
        private void geraLinhasIPIComprasImportacao(StringBuilder conteudoArquivo, SqlDataReader sReaderCompras, DateTime dataEntrada, int codigoFilial, string tipoCompra) {
            string notaFiscal = "\"NF:" + sReaderCompras["NUMEROMOV"] + "\"";
            // Linha 7: Débito de impostos de IPI a recuperar, apenas para Importação ou Compras para Industrialização
            Decimal valorIPI = (Decimal)sReaderCompras["IPI"];
            if ((tipoCompra.Equals("1.2.05") || tipoCompra.Equals("1.2.20")) && !valorIPI.Equals(Decimal.Zero)) {
               // Conta de IPI a recuperar da Matriz/Filial
                string contaDebitoIPIRecuperar = null;
                string codigoIPIRecuperarHistorico = Constantes.C_F1_IMPORTACAO_COMPRA_INDUST_HISTORICO;
                if (codigoFilial == 1) {
                    contaDebitoIPIRecuperar = Constantes.C_F1_IMPORTACAO_COMPRA_INDUST_DEBITO_IPI;
                        //"922";
                } 
                string linhaDebitoIPIRecuperar = geraLinhaTextoCreditoDebito(dataEntrada,contaDebitoIPIRecuperar,"0",
                    valorIPI,codigoIPIRecuperarHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoIPIRecuperar);

                // Linha 8: Crédito de impostos e contribuições de IPI
                string contaCreditoIPIMerc = null;
                // Matriz e compra para Industrialização
                if (codigoFilial == 1 && tipoCompra.Equals("1.2.05")) {
                    // Código da conta de Materia Prima Nacional para Industrialização
                    contaCreditoIPIMerc = Constantes.C_F1_COMPRA_INDUST_CREDITO_IPI;
                        //"483";
                    // Matriz e Importação para Industrialização
                } else if (codigoFilial == 1 && tipoCompra.Equals("1.2.20")) {
                    // Código da conta de Materia Prima Importada para Industrialização
                    contaCreditoIPIMerc = Constantes.C_F1_IMPORTACAO_CREDITO_IPI;
                        //"490";
                }
                string linhaCreditoIPIMerc = geraLinhaTextoCreditoDebito(dataEntrada,"0",contaCreditoIPIMerc,
                    valorIPI,codigoIPIRecuperarHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoIPIMerc);
            }
        }


        /**
         * 
         *  Ajustado, ainda falta revisar escrita do arquivo e alguns codigos de contas
         *  Linhas Novo Formato OK
         * 
         *  Constantes OK
         * 
         */
        public void geraArquivoExportacaoVendas(DateTime dataInicial, DateTime dataFinal, string caminhoExportacao, bool matriz, bool recife, bool fortaleza) {
            string clausulaFiliais = geraClausulaWhereFiliais(matriz, recife, fortaleza);
            // Obtem um objeto do tipo Command
            SqlCommand comando = getConexao().CreateCommand();
            comando.CommandText =
                "SELECT * FROM N3_SaidasContabil WHERE "
                + "N3_SaidasContabil.DATAEMISSAO >= '" + dataInicial.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + "AND N3_SaidasContabil.DATAEMISSAO <= '" + dataFinal.ToString(Constantes.FORMATO_DATA_SQL) + "' "
                + clausulaFiliais;

            StringBuilder conteudoArquivoFilial_1 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_2 = new StringBuilder();
            StringBuilder conteudoArquivoFilial_3 = new StringBuilder();

            SqlDataReader sReaderVendas = comando.ExecuteReader();
            while (sReaderVendas.Read()) {
                string tipoVenda = sReaderVendas["CODTMV"].ToString();
                DateTime dataEmissao = DateTime.Parse(sReaderVendas["DATAEMISSAO"].ToString());
                Decimal valorLiquidoNF = (Decimal)sReaderVendas["VALORLIQUIDO"];
                int codigoFilial = int.Parse(sReaderVendas["CODFILIAL"].ToString());
                // Vendas de Mercadorias
                if (tipoVenda != null && tipoVenda.Equals("2.2.01")) {
                    if (codigoFilial == 1) {
                        geraLinhasFaturamentoVendasMercadoria(conteudoArquivoFilial_1, sReaderVendas, dataEmissao, valorLiquidoNF, codigoFilial);
                        geraLinhasICMSVendas(conteudoArquivoFilial_1, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasPISVendas(conteudoArquivoFilial_1, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasCOFINSVendas(conteudoArquivoFilial_1, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasIPIVendas(conteudoArquivoFilial_1, sReaderVendas, dataEmissao, codigoFilial);
                    } else if (codigoFilial == 2) {
                        geraLinhasFaturamentoVendasMercadoria(conteudoArquivoFilial_2, sReaderVendas, dataEmissao, valorLiquidoNF, codigoFilial);
                        geraLinhasICMSVendas(conteudoArquivoFilial_2, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasPISVendas(conteudoArquivoFilial_2, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasCOFINSVendas(conteudoArquivoFilial_2, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasIPIVendas(conteudoArquivoFilial_2, sReaderVendas, dataEmissao, codigoFilial);
                    } else if (codigoFilial == 3) {
                        geraLinhasFaturamentoVendasMercadoria(conteudoArquivoFilial_3, sReaderVendas, dataEmissao, valorLiquidoNF, codigoFilial);
                        geraLinhasICMSVendas(conteudoArquivoFilial_3, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasPISVendas(conteudoArquivoFilial_3, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasCOFINSVendas(conteudoArquivoFilial_3, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasIPIVendas(conteudoArquivoFilial_3, sReaderVendas, dataEmissao, codigoFilial);
                    }
                // Vendas de Produtos
                } else if (tipoVenda != null && tipoVenda.Equals("2.2.02")) {
                    if (codigoFilial == 1) {
                        geraLinhasFaturamentoVendasProduto(conteudoArquivoFilial_1, sReaderVendas, dataEmissao, valorLiquidoNF, codigoFilial);
                        geraLinhasICMSVendas(conteudoArquivoFilial_1, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasPISVendas(conteudoArquivoFilial_1, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasCOFINSVendas(conteudoArquivoFilial_1, sReaderVendas, dataEmissao, codigoFilial);
                        geraLinhasIPIVendas(conteudoArquivoFilial_1, sReaderVendas, dataEmissao, codigoFilial);
                    }
                }
            }

            escreveArquivoExportacao("Filial_1_" + Constantes.ARQ_EXPORTACAO_VENDAS, caminhoExportacao, conteudoArquivoFilial_1);
            escreveArquivoExportacao("Filial_2_" + Constantes.ARQ_EXPORTACAO_VENDAS, caminhoExportacao, conteudoArquivoFilial_2);
            escreveArquivoExportacao("Filial_3_" + Constantes.ARQ_EXPORTACAO_VENDAS, caminhoExportacao, conteudoArquivoFilial_3);

            sReaderVendas.Close();
            comando.Connection.Close();
        }

        /**
         * 
         *  Ajustado
         *  Linhas Novo Formato OK
         * 
         *  Constantes OK
         */
        private void geraLinhasFaturamentoVendasProduto(StringBuilder conteudoArquivo, SqlDataReader sReaderVendas, DateTime dataEmissao, Decimal valorLiquidoNF, int codigoFilial) {
            string notaFiscal = "\"NF:" + sReaderVendas["NUMEROMOV"] + "\"";
            // Linha 1: Debito nas Duplicatas a Receber
            string contaDebitoDuplicatasReceber = null;
            string codigoDuplicatasReceberHistorico = Constantes.C_F1_VENDAS_PROD_HISTORICO;
            if (codigoFilial == 1) {
                contaDebitoDuplicatasReceber = Constantes.C_F1_VENDAS_PROD_DEBITO;
            } 
            string linhaDebitoDuplicatasReceber = geraLinhaTextoCreditoDebito(dataEmissao,contaDebitoDuplicatasReceber,"0",
                valorLiquidoNF,codigoDuplicatasReceberHistorico,notaFiscal);
            conteudoArquivo.Append(linhaDebitoDuplicatasReceber);

            // Linha 2: Crédito no Faturamento de Produtos Incentivadas ou Não
            string contaCreditoProdutoInc = null;
            Decimal pis = (Decimal)sReaderVendas["PIS"];
            Decimal cofins = (Decimal)sReaderVendas["COFINS"];
            // Faturamento da Matriz Campina Grande
            if (codigoFilial == 1) {
                // Verificando a condição de PIS e COFINS iguais a ZERO o faturamento é beneficiado
                if (pis == Decimal.Zero && cofins == Decimal.Zero) {
                    // Código da conta de faturamento Matriz Produto COM incentivo
                    contaCreditoProdutoInc = Constantes.C_F1_VENDAS_PROD_MPBEM_CREDITO;
                        //"8020";
                    VENDASProdutoBen = VENDASProdutoBen + Decimal.ToDouble(valorLiquidoNF);
                } else {
                    // Código da conta de faturamento Matriz Produto SEM incentivo
                    contaCreditoProdutoInc = Constantes.C_F1_VENDAS_PROD_CREDITO;
                        //"8036";
                }
            } else {
                contaCreditoProdutoInc = "######### Codigo de FILIAL inválida para Operação !!!!#########";
                return;
            }
            string linhaCreditoProdutoInc = geraLinhaTextoCreditoDebito(dataEmissao,"0",contaCreditoProdutoInc,
                valorLiquidoNF,codigoDuplicatasReceberHistorico,notaFiscal);
            conteudoArquivo.Append(linhaCreditoProdutoInc);

            /*
           // Lança custos de venda de Produto   
            string contaDebitoCustoMerc = null;
            Decimal valorPIS = (Decimal)sReaderVendas["PIS"];
            Decimal valorCOFINS = (Decimal)sReaderVendas["COFINS"];
            Decimal valorCUSTOPRODUTO = (Decimal)sReaderVendas["CUSTO_PRODUTO"];
            // O Custo da venda não estando ZERADO pode-se fazer o lançamento
            if (!valorCUSTOPRODUTO.Equals(Decimal.Zero)) {
                // Linha 3: Debita Custo da Mercadoria/Produto
                // Vendas de Produto Incentivado
                if (valorPIS.Equals(Decimal.Zero) && valorCOFINS.Equals(Decimal.Zero)) {
                    if (codigoFilial == 1) {
                        contaDebitoCustoMerc = Constantes.C_F1_VENDAS_PROD_MPBEM_DEBITO_CUSTO;
                            //"6511";
                    } 
                // Vendas de Produto não incentivado
                } else {
                    if (codigoFilial == 1) {
                        contaDebitoCustoMerc = Constantes.C_F1_VENDAS_PROD_DEBITO_CUSTO;
                            //"6541";
                    }
                }
                string linhaDebitoCustoMerc = geraLinhaTextoCreditoDebito(dataEmissao,contaDebitoCustoMerc,"0",
                    valorCUSTOPRODUTO,codigoDuplicatasReceberHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoCustoMerc);

                // Linha 4: Credita Estoque de Origem no Custo de Venda
                string contaCreditoEstoque = null;
                //Venda de Produto Incentivado
                if (valorPIS.Equals(Decimal.Zero) && valorCOFINS.Equals(Decimal.Zero)) {
                    if (codigoFilial == 1) {
                        contaCreditoEstoque = Constantes.C_F1_VENDAS_PROD_MPBEM_CREDITO_CUSTO;
                            //"5569";
                    }                       
                // Venda de Produto não incentivado
                } else {
                    if (codigoFilial == 1) {
                        contaCreditoEstoque = Constantes.C_F1_VENDAS_PROD_CREDITO_CUSTO;
                            //"5575";
                    }                    
                }
                string linhaCreditoEstoque = geraLinhaTextoCreditoDebito(dataEmissao,"0",contaCreditoEstoque,
                    valorCUSTOPRODUTO,codigoDuplicatasReceberHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoEstoque);
            }
             */
        }

        /**
         * 
         * Ajustada
         * Linhas Novo Formato OK
         * 
         * Constantes OK
         */
        private void geraLinhasFaturamentoVendasMercadoria(StringBuilder conteudoArquivo, SqlDataReader sReaderVendas, DateTime dataEmissao, Decimal valorLiquidoNF, int codigoFilial) {
            string notaFiscal = "\"NF:" + sReaderVendas["NUMEROMOV"] + "\"";
            // Linha 1: Debito nas Duplicatas a Receber
            string contaDebitoDuplicatasReceber = null;
            string codigoDebitoMensagemDuplicatasReceber = Constantes.C_F1_VENDAS_MERC_HISTORICO;
            if (codigoFilial == 1) {
                contaDebitoDuplicatasReceber = Constantes.C_F1_VENDAS_MERC_DEBITO;
            } else if (codigoFilial == 2) {
                contaDebitoDuplicatasReceber = Constantes.C_F2_VENDAS_MERC_DEBITO;
            } else if (codigoFilial == 3) {
                contaDebitoDuplicatasReceber = Constantes.C_F3_VENDAS_MERC_DEBITO;
            }
            string linhaDebitoDuplicatasReceber = geraLinhaTextoCreditoDebito(dataEmissao,contaDebitoDuplicatasReceber,"0",
                valorLiquidoNF,codigoDebitoMensagemDuplicatasReceber,notaFiscal);
            conteudoArquivo.Append(linhaDebitoDuplicatasReceber);


            // Linha 2: Crédito no Faturamento de Mercadorias Incentivadas ou Não
            string contaCreditoFaturamentoMerc = null;
            string codigoCreditoMensagemFaturamentoMerc = Constantes.C_F1_VENDAS_MERC_HISTORICO;;
            Decimal pis = (Decimal)sReaderVendas["PIS"];
            Decimal cofins = (Decimal)sReaderVendas["COFINS"];
            // Faturamento da Matriz Campina Grande
            if (codigoFilial == 1) {
                // Verificando a condição de PIS e COFINS iguais a ZERO o faturamento é beneficiado
                if (pis == Decimal.Zero && cofins == Decimal.Zero) {
                    // Código da conta de faturamento Matriz mercadoria COM incentivo
                    //contaCreditoFaturamentoMerc = "8042";
                    contaCreditoFaturamentoMerc = Constantes.C_F1_VENDAS_MPBEM_CREDITO;
                    VENDASMercadoriaBen = VENDASMercadoriaBen + Decimal.ToDouble(valorLiquidoNF);
                } else {
                    // Código da conta de faturamento Matriz mercadoria SEM incentivo
                    //contaCreditoFaturamentoMerc = "8059";
                    contaCreditoFaturamentoMerc = Constantes.C_F1_VENDAS_MERC_CREDITO;
                }
                // Faturamento da Filial Recife
            } else if (codigoFilial == 2) {
                // Verificando a condição de PIS e COFINS iguais a ZERO o faturamento é beneficiado
                if (pis == Decimal.Zero && cofins == Decimal.Zero) {
                    // Código da conta de faturamento Filial mercadoria COM incentivo
                    contaCreditoFaturamentoMerc = Constantes.C_F2_VENDAS_MPBEM_CREDITO;
                } else {
                    // Código da conta de faturamento Filial mercadoria SEM incentivo
                    contaCreditoFaturamentoMerc = Constantes.C_F2_VENDAS_MERC_CREDITO;
                }
            } else if (codigoFilial == 3) {
                // Verificando a condição de PIS e COFINS iguais a ZERO o faturamento é beneficiado
                if (pis == Decimal.Zero && cofins == Decimal.Zero) {
                    // Código da conta de faturamento Filial mercadoria COM incentivo
                    contaCreditoFaturamentoMerc = Constantes.C_F3_VENDAS_MPBEM_CREDITO;
                } else {
                    // Código da conta de faturamento Filial mercadoria SEM incentivo
                    contaCreditoFaturamentoMerc = Constantes.C_F3_VENDAS_MERC_CREDITO;
                }
            }
            string linhaCreditoFaturamentoMerc = geraLinhaTextoCreditoDebito(dataEmissao,"0",contaCreditoFaturamentoMerc,
                valorLiquidoNF,codigoCreditoMensagemFaturamentoMerc,notaFiscal);
            conteudoArquivo.Append(linhaCreditoFaturamentoMerc);

            // Lançamento de Custos
            /*
            Decimal valorPIS = (Decimal)sReaderVendas["PIS"];
            Decimal valorCOFINS = (Decimal)sReaderVendas["COFINS"];
            Decimal valorCUSTOMERCADORIA = (Decimal)sReaderVendas["CUSTO_MERCADORIA"];
            string codigoDebitoMensagemCusto = Constantes.C_F1_VENDAS_MERC_HISTORICO_CUSTO;
            // O Custo da venda não estando ZERADO pode-se fazer o lançamento
            if (!valorCUSTOMERCADORIA.Equals(Decimal.Zero)) {
                // Vendas de Mercadoria Incentivada
                // Linha 3: Debita Custo da Mercadoria/Produto
                string contaDebitoCustoMercProd = null;
                if (valorPIS.Equals(Decimal.Zero) && valorCOFINS.Equals(Decimal.Zero)) {
                    if (codigoFilial == 1) {
                        contaDebitoCustoMercProd = Constantes.C_F1_VENDAS_MPBEM_DEBITO_CUSTO;
                        //"6592";
                    } else if (codigoFilial == 2) {
                        contaDebitoCustoMercProd = Constantes.C_F2_VENDAS_MPBEM_DEBITO_CUSTO;
                    } else if (codigoFilial == 3) {
                        contaDebitoCustoMercProd = Constantes.C_F3_VENDAS_MPBEM_DEBITO_CUSTO;
                    }
                    // Vendas de Mercadoria não incentivado
                } else {
                    if (codigoFilial == 1) {
                        contaDebitoCustoMercProd = Constantes.C_F1_VENDAS_MERC_DEBITO_CUSTO;
                            //"6617";
                    } else if (codigoFilial == 2) {
                        contaDebitoCustoMercProd = Constantes.C_F2_VENDAS_MERC_DEBITO_CUSTO;
                    } else if (codigoFilial == 3) {
                        contaDebitoCustoMercProd = Constantes.C_F3_VENDAS_MERC_DEBITO_CUSTO;
                    }
                }
                string linhaDebitoCustoMercProd = geraLinhaTextoCreditoDebito(dataEmissao,contaDebitoCustoMercProd,"0",
                    valorCUSTOMERCADORIA,codigoDebitoMensagemCusto,notaFiscal);
                conteudoArquivo.Append(linhaDebitoCustoMercProd);

                // Linha 4: Credita Estoque de Origem no Custo de Venda
                // Venda de Mercadoria Incentivado
                string codigoCreditoMensagemCusto = Constantes.C_F1_VENDAS_MERC_HISTORICO_CUSTO;
                string contaCreditoCustoVenda = null;
                if (valorPIS.Equals(Decimal.Zero) && valorCOFINS.Equals(Decimal.Zero)) {
                    if (codigoFilial == 1) {
                        contaCreditoCustoVenda = Constantes.C_F1_VENDAS_MPBEM_CREDITO_CUSTO;
                            //"5581";
                    } else if (codigoFilial == 2) {
                        contaCreditoCustoVenda = Constantes.C_F2_VENDAS_MPBEM_CREDITO_CUSTO;
                    } else if (codigoFilial == 3) {
                        contaCreditoCustoVenda = Constantes.C_F3_VENDAS_MPBEM_CREDITO_CUSTO;
                    }
                    // Venda de Mercadoria não incentivado
                } else {
                    if (codigoFilial == 1) {
                        contaCreditoCustoVenda = Constantes.C_F1_VENDAS_MERC_CREDITO_CUSTO;
                            //"5598";
                    } else if (codigoFilial == 2) {
                        contaCreditoCustoVenda = Constantes.C_F2_VENDAS_MERC_CREDITO_CUSTO;
                    } else if (codigoFilial == 3) {
                        contaCreditoCustoVenda = Constantes.C_F3_VENDAS_MERC_CREDITO_CUSTO;
                    }
                }
                string linhaCreditoCustoVenda = geraLinhaTextoCreditoDebito(dataEmissao,"0",contaCreditoCustoVenda,
                    valorCUSTOMERCADORIA,codigoCreditoMensagemCusto,notaFiscal);
                conteudoArquivo.Append(linhaCreditoCustoVenda);
            }
             */
        }

        /**
         * 
         *   Não definido pelo Carlos para as Filiais!!!!
         *   Linhas Novo Formato OK
         * 
         *   Constantes OK
         */
        private void geraLinhasIPIVendas(StringBuilder conteudoArquivo, SqlDataReader sReaderVendas, DateTime dataEmissao, int codigoFilial) {
            string notaFiscal = "\"NF:" + sReaderVendas["NUMEROMOV"] + "\"";
            // Linha 9: Débito de impostos de IPI incidentes sobre Venda
            Decimal valorIPI = (Decimal)sReaderVendas["IPI"];
            // Se Não for ZERO o lançamento é gerado
            if (!valorIPI.Equals(Decimal.Zero)) {
                string contaDebitoIPIVenda = null;
                string contaCreditoIPIVenda = null;
                string codigoIPIVendaHistorico = Constantes.C_F1_VENDAS_MERC_HISTORICO_IPI;
                // Faturamento da Matriz Campina Grande
                if (codigoFilial == 1) {
                    // Código da conta de IPI da Matriz
                    contaDebitoIPIVenda = Constantes.C_F1_VENDAS_MERC_DEBITO_IPI;
                    contaCreditoIPIVenda = Constantes.C_F1_VENDAS_MERC_CREDITO_IPI;
                        //"6110";
                    // Faturamento da Filial Recife
                } else if (codigoFilial == 2) {
                    // Código da conta de IPI da Filial
                    contaDebitoIPIVenda = Constantes.C_F2_VENDAS_MERC_DEBITO_IPI;
                    contaCreditoIPIVenda = Constantes.C_F2_VENDAS_MERC_CREDITO_IPI;
                        //"6126";
                } else if (codigoFilial == 3) {
                    // Código da conta de IPI da Filial Fortaleza
                    contaDebitoIPIVenda = Constantes.C_F3_VENDAS_MERC_DEBITO_IPI;
                    contaCreditoIPIVenda = Constantes.C_F3_VENDAS_MERC_CREDITO_IPI;
                        //"6126";
                }
                string linhaDebitoIPIVenda = geraLinhaTextoCreditoDebito(dataEmissao,contaDebitoIPIVenda,"0",
                    valorIPI,codigoIPIVendaHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoIPIVenda);

                // Linha 10: Crédito de impostos e contribuições de IPI
                string linhaCreditoIPIContrib = geraLinhaTextoCreditoDebito(dataEmissao,"0",contaCreditoIPIVenda,
                    valorIPI,codigoIPIVendaHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoIPIContrib);
            }
            IPI = IPI + Decimal.ToDouble(valorIPI);
            //Console.WriteLine(valorIPI.ToString("f", cultureInfoUS));
        }

        /**
         * 
         *  AJustado, mas necessita confirmar código das Filiais ainda
         *  Linhas Novo Formato OK
         * 
         *  Constantes OK
         * 
         */
        private void geraLinhasCOFINSVendas(StringBuilder conteudoArquivo, SqlDataReader sReaderVendas, DateTime dataEmissao, int codigoFilial) {
            string notaFiscal = "\"NF:" + sReaderVendas["NUMEROMOV"] + "\"";
            // Linha 7: Débito de impostos de COFINS incidentes sobre Venda
            Decimal valorCOFINS = (Decimal)sReaderVendas["COFINS"];
            // Se Não for ZERO o lançamento é gerado
            if (!valorCOFINS.Equals(Decimal.Zero)) {
                string contaDebitoCOFINSVenda = null;
                string codigoDebitoCOFINSVendaHistorico = Constantes.C_F1_VENDAS_MERC_HISTORICO_COFINS;
                // Faturamento da Matriz Campina Grande
                if (codigoFilial == 1) {
                    // Código da conta de PIS da Matriz
                    contaDebitoCOFINSVenda = Constantes.C_F1_VENDAS_MERC_DEBITO_COFINS;
                    //"6238";
                    // Faturamento da Filial Recife
                } else if (codigoFilial == 2) {
                    // Código da conta de PIS da Filial
                    contaDebitoCOFINSVenda = Constantes.C_F2_VENDAS_MERC_DEBITO_COFINS;
                        //"6244";
                } else if (codigoFilial == 3) {
                    // Código da conta de PIS da Filial
                    contaDebitoCOFINSVenda = Constantes.C_F3_VENDAS_MERC_DEBITO_COFINS;
                        //"6238";
                } 
                string linhaDebitoCOFINSVenda = geraLinhaTextoCreditoDebito(dataEmissao,contaDebitoCOFINSVenda,"0",
                    valorCOFINS,codigoDebitoCOFINSVendaHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoCOFINSVenda);

                // Linha 8: Crédito de impostos e contribuições de COFINS
                // Código da conta Impostos e Contribuições de COFINS da Matriz e Filial (Unico)
                string linhaCreditoCOFINSContrib = geraLinhaTextoCreditoDebito(dataEmissao,"0",Constantes.C_F1_VENDAS_MERC_CREDITO_COFINS,
                    valorCOFINS,codigoDebitoCOFINSVendaHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoCOFINSContrib);
            }
            COFINS = COFINS + Decimal.ToDouble(valorCOFINS);
        }

        /**
         * 
         *  AJustado, mas necessita confirmar código das Filiais ainda
         *  Linhas Novo Formato OK
         * 
         *  Constantes OK
         */
        private void geraLinhasPISVendas(StringBuilder conteudoArquivo, SqlDataReader sReaderVendas, DateTime dataEmissao, int codigoFilial) {
            string notaFiscal = "\"NF:" + sReaderVendas["NUMEROMOV"] + "\"";
            // Linha 5: Débito de impostos de PIS incidentes sobre Venda
            // Se Não for ZERO o lançamento é gerado
            Decimal valorPIS = (Decimal)sReaderVendas["PIS"];
            if (!valorPIS.Equals(Decimal.Zero)) {
                string codigoDebitoPISVendaHistorico = Constantes.C_F1_VENDAS_MERC_HISTORICO_PIS;                
                string contaDebitoPISVenda = null;
                // Faturamento da Matriz Campina Grande
                if (codigoFilial == 1) {
                    // Código da conta de PIS da Matriz
                    contaDebitoPISVenda = Constantes.C_F1_VENDAS_MERC_DEBITO_PIS;
                        //"6215";
                    // Faturamento da Filial Recife
                } else if (codigoFilial == 2) {
                    // Código da conta de PIS da Filial
                    contaDebitoPISVenda = Constantes.C_F2_VENDAS_MERC_DEBITO_PIS;
                } else if (codigoFilial == 3) {
                    // Código da conta de PIS da Filial
                    contaDebitoPISVenda = Constantes.C_F3_VENDAS_MERC_DEBITO_PIS;
                }
                string linhaDebitoPISVenda = geraLinhaTextoCreditoDebito(dataEmissao,contaDebitoPISVenda,"0",
                    valorPIS,codigoDebitoPISVendaHistorico,notaFiscal);
                conteudoArquivo.Append(linhaDebitoPISVenda);

                // Linha 6: Crédito de impostos e contribuições de PIS
                // Código da conta Impostos e Contribuições de PIS da Matriz e Filial (Unico)
                string linhaCreditoPISContrib = geraLinhaTextoCreditoDebito(dataEmissao,"0",Constantes.C_F1_VENDAS_MERC_CREDITO_PIS,
                    valorPIS,codigoDebitoPISVendaHistorico,notaFiscal);
                conteudoArquivo.Append(linhaCreditoPISContrib);
            }
            PIS = PIS + Decimal.ToDouble(valorPIS);
        }
        
        /**
         * 
         *  Ajustado
         *  Linhas Novo Formato OK
         * 
         *  Constantes OK 
         * 
         */
        private void geraLinhasICMSVendas(StringBuilder conteudoArquivo, SqlDataReader sReaderVendas, DateTime dataEmissao, int codigoFilial) {
            string notaFiscal = "\"NF:" + sReaderVendas["NUMEROMOV"] + "\"";
            // Linha 3: Débito de impostos de ICMS incidentes sobre Venda
            // Faturamento da Matriz Campina Grande
            Decimal valorICMS = (Decimal)sReaderVendas["ICMS"];
            string contaDebitoICMSVenda = null;
            string codigoDebitoICMSVendaHistorico = Constantes.C_F1_VENDAS_MERC_HISTORICO_ICMS;
            if (codigoFilial == 1) {
                // Código da conta de ICMS da Matriz
                contaDebitoICMSVenda = Constantes.C_F1_VENDAS_MERC_DEBITO_ICMS;
                //"6191";
                // Faturamento da Filial Recife
            } else if (codigoFilial == 2) {
                // Código da conta de ICMS da Filial
                contaDebitoICMSVenda = Constantes.C_F2_VENDAS_MERC_DEBITO_ICMS;
            } else if (codigoFilial == 3) {
                // Código da conta de ICMS da Filial
                contaDebitoICMSVenda = Constantes.C_F3_VENDAS_MERC_DEBITO_ICMS;
            }
            string linhaDebitoICMSVenda = geraLinhaTextoCreditoDebito(dataEmissao,contaDebitoICMSVenda,"0",
                valorICMS,codigoDebitoICMSVendaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaDebitoICMSVenda);

            // Linha 4: Crédito de impostos e contribuições de ICMS
            // Faturamento da Matriz Campina Grande
            string contaCreditoICMSContrib = null;
            string codigoCreditoICMSVendaHistorico = Constantes.C_F1_VENDAS_MERC_HISTORICO_ICMS;
            if (codigoFilial == 1) {
                // Código da conta Impostos e Contribuições de ICMS da Matriz
                contaCreditoICMSContrib = Constantes.C_F1_VENDAS_MERC_CREDITO_ICMS;
                    //"2619";
                // Faturamento da Filial Recife
            } else if (codigoFilial == 2) {
                // Código da conta Impostos e Contribuições de ICMS da Filial
                contaCreditoICMSContrib = Constantes.C_F2_VENDAS_MERC_CREDITO_ICMS;
            } else if (codigoFilial == 3) {
                // Código da conta Impostos e Contribuições de ICMS da Filial
                contaCreditoICMSContrib = Constantes.C_F3_VENDAS_MERC_CREDITO_ICMS;
            }
            string linhaCreditoICMSContrib = geraLinhaTextoCreditoDebito(dataEmissao,"0",contaCreditoICMSContrib,
                valorICMS,codigoCreditoICMSVendaHistorico,notaFiscal);
            conteudoArquivo.Append(linhaCreditoICMSContrib);

            ICMS = ICMS + Decimal.ToDouble(valorICMS);
        }
        
        /**
         * 
         * 
         * 
         */
        public StreamWriter obtemArquivoExportacao(string nomeArquivo, string caminho) {
            StreamWriter sw = null;
            if (nomeArquivo != null) {
                sw = File.CreateText(caminho + "\\" + nomeArquivo + DateTime.UtcNow.ToString("_dd-MM-yy_hh-mm-ss") + Constantes.EXT_ARQS);
            }
            return sw;
        }


        /**
         * 
         *   Método padrão para gerar linhas de exportação de dados contábeis
         * 
         */
       private string geraLinhaTextoCreditoDebito(DateTime dataEmissao, String contaDebito, String contaCredito, 
           Decimal valor, String codigoMensagem, String observacao) {
            StringBuilder conteudoLinha = new StringBuilder();
            // Data de Emissao do Lançamento
            conteudoLinha.Append(dataEmissao.ToString("dd/MM/yy"));
            conteudoLinha.Append(Constantes.SEPARADOR_TOKEN);
            // Conta para Débito do Lançamento
            conteudoLinha.Append(contaDebito);
            conteudoLinha.Append(Constantes.SEPARADOR_TOKEN);
            // Conta para Crédito do Lançamento
            conteudoLinha.Append(contaCredito);
            conteudoLinha.Append(Constantes.SEPARADOR_TOKEN);
            // Valor do lançamento
            conteudoLinha.Append(valor.ToString("f", cultureInfoUS));
            conteudoLinha.Append(Constantes.SEPARADOR_TOKEN);
            // Código da Mensagem do Lançamento
            conteudoLinha.Append(codigoMensagem);
            conteudoLinha.Append(Constantes.SEPARADOR_TOKEN);
            // Observação do Lançamento Contábil
            conteudoLinha.Append(observacao);
            conteudoLinha.Append("\n");
            
           return conteudoLinha.ToString();
        }

        /**
         * 
         * 
         * 
         */
        public string obtemContaContabilFornecedor(string codcfo, string codtmv, string notaFiscal, int codFilial) {
            // Obtem um objeto do tipo Command
            SqlCommand comandoSelectContaContabil = getConexao().CreateCommand();
            // Select para Buscar conta contabil do fornecedor
            comandoSelectContaContabil.CommandText = "SELECT * FROM N3_MAPEAMENTOFORNECEDOR WHERE CODCFO = '" + codcfo + "'";

            string contaContabil = null;
            SqlDataReader sReaderContaContabil = comandoSelectContaContabil.ExecuteReader();
            while (sReaderContaContabil.Read()) {
                contaContabil = sReaderContaContabil["CONTACONTABIL"].ToString();
            }

            if (contaContabil == null || "".Equals(contaContabil)) {
                if (!clientesSemConta.ContainsKey(codcfo)) {
                    // Registra como já  adicionado
                    clientesSemConta.Add(codcfo,codcfo);
                    // Obtem conexao
                    SqlCommand comandoSelectCliFor = getConexao().CreateCommand();
                    SqlDataReader sReaderCliFor = null;
                    // Marca que existe conta contabil a ser cadastrada
                    existeContaContabilNaoCadastrada = true;
                    
                    // Verifica se precisa criar o buffer de log de mensagem para criação de conta
                    if (logCriacaoContasContabeis == null) {
                        logCriacaoContasContabeis = new StringBuilder();
                        logCriacaoContasContabeis.AppendLine("Os seguintes Clientes/Fornecedores necessitam de contas contábeis criadas:");
                        logCriacaoContasContabeis.AppendLine("");
                    }

                    comandoSelectCliFor.CommandText = "SELECT CODCFO, NOMEFANTASIA, NOME, CGCCFO FROM FCFO WHERE CODCOLIGADA = 2 AND CODCFO = '" + codcfo + "'";
                    sReaderCliFor = comandoSelectCliFor.ExecuteReader();
                    string nomefantasia = null;
                    string razaosocial = null;
                    string cnpj = null;

                    while (sReaderCliFor.Read()) {
                        nomefantasia = sReaderCliFor["NOMEFANTASIA"].ToString();
                        razaosocial = sReaderCliFor["NOME"].ToString();
                        cnpj = sReaderCliFor["CGCCFO"].ToString();

                        logCriacaoContasContabeis.AppendLine("Codigo: " + codcfo + " -> Filial: " + codFilial + " -> Cod. Movimento: " + codtmv + " -> Nota Fiscal: " + notaFiscal);
                        logCriacaoContasContabeis.AppendLine("     CNPJ: " + cnpj);
                        logCriacaoContasContabeis.AppendLine("     Nome Fantasia: " + nomefantasia);
                        logCriacaoContasContabeis.AppendLine("     Razão Social: " + razaosocial);
                        logCriacaoContasContabeis.AppendLine("     Conta Contábil: ??????");
                        logCriacaoContasContabeis.AppendLine("");
                    }
                    sReaderCliFor.Close();
                    comandoSelectCliFor.Connection.Close();
                }
            }

            sReaderContaContabil.Close();
            comandoSelectContaContabil.Connection.Close();
            return contaContabil;
        }

        /*
         * 
         * 
         * 
         **/
        private void escreveArquivoExportacao(string nomeArquivo, string caminho, StringBuilder conteudo) {
            if (conteudo.Length > 0) {
                StreamWriter swArquivoExportacao = obtemArquivoExportacao(nomeArquivo, caminho);
                swArquivoExportacao.Write(conteudo.ToString());
                swArquivoExportacao.Flush();
                swArquivoExportacao.Close();
                swArquivoExportacao.Dispose();
            }
        }


        /**
         * 
         **/
        private string geraClausulaWhereFiliais(bool matriz, bool recife, bool fortaleza) {
            string clausula = "";
            if (matriz || recife || fortaleza) {
                clausula = "AND CODFILIAL IN (";
                if (matriz && (recife || fortaleza)) {
                    clausula = clausula + "'1',";
                } else if (matriz && !recife && !fortaleza) {
                    clausula = clausula + "'1')";
                }

                if (recife && fortaleza) {
                    clausula = clausula + "'2',";
                } else if (recife && !fortaleza) {
                    clausula = clausula + "'2')";
                }

                if (fortaleza) {
                    clausula = clausula + "'3')";
                }
            }
            return clausula;
        }

        /**
         * 
         **/
        public void limpaLogContasContabeis() {
            logCriacaoContasContabeis = new StringBuilder();
            clientesSemConta.Clear();   
            existeContaContabilNaoCadastrada = false;
        }

        /**
         * 
         * 
         **/
        public string[] consultaContaContabil(string codcfo) {
            string[] resultadoConsulta = null;
            // Obtem um objeto do tipo Command
            SqlCommand comandoSelectContaContabil = getConexao().CreateCommand();
            // Select para Buscar conta contabil do fornecedor
            comandoSelectContaContabil.CommandText = "SELECT * FROM N3_MapeamentoFornecedor WHERE CODCFO = '" + codcfo + "'";

            SqlDataReader sReaderContaContabil = comandoSelectContaContabil.ExecuteReader();
            while (sReaderContaContabil.Read()) {
                resultadoConsulta = new string[3];
                resultadoConsulta[0] = sReaderContaContabil["CODCFO"].ToString();
                resultadoConsulta[1] = sReaderContaContabil["CONTACONTABIL"].ToString();
                resultadoConsulta[2] = sReaderContaContabil["RAZAOSOCIAL"].ToString();
            }
            sReaderContaContabil.Close();
            comandoSelectContaContabil.Connection.Close();
            return resultadoConsulta;
        }
        
        /*
         * 
         * 
         **/
        public bool existeContaContabilCliFor(string codcfo) {
            bool contaContabilJaExistente = false;
            // Obtem um objeto do tipo Command
            SqlCommand comandoSelectContaContabil = getConexao().CreateCommand();
            // Select para Buscar conta contabil do fornecedor
            comandoSelectContaContabil.CommandText = "SELECT * FROM N3_MapeamentoFornecedor WHERE CODCFO = '" + codcfo + "'";

            SqlDataReader sReaderContaContabil = comandoSelectContaContabil.ExecuteReader();
            while (sReaderContaContabil.Read()) {
                contaContabilJaExistente = true;
            }
            sReaderContaContabil.Close();
            comandoSelectContaContabil.Connection.Close();
            return contaContabilJaExistente;
        }

        /*
         * 
         * 
         **/
        public bool insereContaContabilCliFor(string codcfo, string contaContabil, string razaoSocial) {
            bool cadastroComSucesso = false;
            // Obtem um objeto do tipo Command
            SqlCommand comandoInsereContaContabil = getConexao().CreateCommand();
            // Insert de conta contabil do fornecedor
            comandoInsereContaContabil.CommandText = "INSERT INTO N3_MapeamentoFornecedor(CODCFO, CONTACONTABIL, RAZAOSOCIAL) VALUES ('" 
                + codcfo + "','" + contaContabil + "','" + razaoSocial + "')" ;

            int linhasAfetadas = comandoInsereContaContabil.ExecuteNonQuery();
            if (linhasAfetadas == 1) {
                cadastroComSucesso = true;
            }
            comandoInsereContaContabil.Connection.Close();
            return cadastroComSucesso;
        }

        /*
         * 
         * 
         **/
        public bool removeContaContabilCliFor(string codcfo) {
            bool remocaoComSucesso = false;
            // Obtem um objeto do tipo Command
            SqlCommand comandoRemoveContaContabil = getConexao().CreateCommand();
            // Delete de conta contabil do fornecedor
            comandoRemoveContaContabil.CommandText = "DELETE FROM N3_MapeamentoFornecedor WHERE CODCFO = '" + codcfo + "'";
            int linhasAfetadas = comandoRemoveContaContabil.ExecuteNonQuery();
            if (linhasAfetadas == 1) {
                remocaoComSucesso = true;
            }
            comandoRemoveContaContabil.Connection.Close();
            return remocaoComSucesso;
        }


        /*
         * 
         * 
         **/
        public bool atualizaContaContabilCliFor(string codcfo, string contaContabil, string razaoSocial) {
            bool atualizacaoComSucesso = false;
            // Obtem um objeto do tipo Command
            SqlCommand comandoAtualizaContaContabil = getConexao().CreateCommand();
            // Delete de conta contabil do fornecedor
            comandoAtualizaContaContabil.CommandText = "UPDATE N3_MapeamentoFornecedor SET CODCFO = '" 
                + codcfo + "', CONTACONTABIL = '" 
                + contaContabil 
                + "', RAZAOSOCIAL = '" + razaoSocial + "'"
                + " WHERE CODCFO = '" + codcfo + "'";
            int linhasAfetadas = comandoAtualizaContaContabil.ExecuteNonQuery();
            if (linhasAfetadas == 1) {
                atualizacaoComSucesso = true;
            }
            comandoAtualizaContaContabil.Connection.Close();
            return atualizacaoComSucesso;
        }
    }
}
