using System;
using System.Collections.Generic;
using System.Text;

namespace N3ExportContabilidade
{
    static class Constantes
    {

        public const string SERVIDOR_DB = "192.168.0.8";
        public const string BANCO_DADOS = "CorporeRM_Compunet";
        public const string USUARIO = "sa";
        public const string SENHA = "dbatin3@762102ma";
        public const string FORMATO_DATA_SQL = "dd-MM-yyyy";
        
        //public const string SERVIDOR_DB = "CLOVISNB";
        //public const string BANCO_DADOS = "CorporeRM_Compunet_Virada";
        //public const string USUARIO = "sa";
        //public const string SENHA = "colovis";
        //public const string FORMATO_DATA_SQL = "yyyy-MM-dd";
        
        public const string FORMATO_DATA_CONTABIL = "dd/MM/yy";

        public const string ARQ_EXPORTACAO_VENDAS = "Exportacao_Contabil_Vendas";
        public const string ARQ_EXPORTACAO_COMPRAS = "Exportacao_Contabil_Compras";
        public const string ARQ_EXPORTACAO_DEVOLUCAO = "Exportacao_Contabil_Devolucao_Vendas";
        public const string ARQ_EXPORTACAO_IMPORTACAO = "Exportacao_Contabil_Importacao_Direta";
        public const string ARQ_EXPORTACAO_FRETES_VENDAS = "Exportacao_Contabil_Fretes_Vendas";
        public const string ARQ_EXPORTACAO_FRETES_COMPRAS = "Exportacao_Contabil_Fretes_Compras";
        public const string ARQ_EXPORTACAO_FRETES_REMOCAO_IMPORTACAO = "Exportacao_Contabil_Fretes_Remocao_Importacao";
        public const string ARQ_EXPORTACAO_COMPRAS_USO_CONSUMO = "Exportacao_Contabil_Compras_Uso_Consumo";
        public const string ARQ_EXPORTACAO_TRANSFERENCIAS_ENVIADA = "Exportacao_Contabil_Transf_Enviada";
        public const string ARQ_EXPORTACAO_TRANSFERENCIAS_RECEBIDA = "Exportacao_Contabil_Transf_Recebida";
        public const string ARQ_EXPORTACAO_SERVICOS_IMPORTACAO = "Exportacao_Contabil_Servicos_Importacao";
        public const string EXT_ARQS = ".txt";
        public const string CAMINHO_ARQS_EXPORTACAO = "C:\\";
        public const string SEPARADOR_TOKEN = ",";

        // Constantes para operação de TRANSFERENCIA ENVIADA
        // FILIAL 2 = RECIFE
        public const string C_F2_TRANSF_ENVIADA_DEBITO = "14835";//"5671";
        public const string C_F2_TRANSF_ENVIADA_CREDITO = "6847";//"5641";
        public const string C_F2_TRANSF_ENVIADA_HISTORICO = "871";//"706";
        public const string C_F2_TRANSF_ENVIADA_DEBITO_ICMS = "6541";//"6209";
        public const string C_F2_TRANSF_ENVIADA_CREDITO_ICMS = "5836";//"2625";
        public const string C_F2_TRANSF_ENVIADA_HISTORICO_ICMS = "711";
        public const string C_F2_TRANSF_ENVIADA_DEBITO_IPI = "6623";//"6126";
        public const string C_F2_TRANSF_ENVIADA_CREDITO_IPI = "5859";//"2625";
        public const string C_F2_TRANSF_ENVIADA_HISTORICO_IPI = "711";

        // FILIAL 3 = FORTALEZA
        public const string C_F3_TRANSF_ENVIADA_DEBITO = "10671";
        public const string C_F3_TRANSF_ENVIADA_CREDITO = "10636";
        public const string C_F3_TRANSF_ENVIADA_HISTORICO = "871";//"706";
        public const string C_F3_TRANSF_ENVIADA_DEBITO_ICMS = "10955";
        public const string C_F3_TRANSF_ENVIADA_CREDITO_ICMS = "10895";
        public const string C_F3_TRANSF_ENVIADA_HISTORICO_ICMS = "711";
        public const string C_F3_TRANSF_ENVIADA_DEBITO_IPI = "10949";
        public const string C_F3_TRANSF_ENVIADA_CREDITO_IPI = "12693";
        public const string C_F3_TRANSF_ENVIADA_HISTORICO_IPI = "711";

        // FILIAL 1 = MATRIZ
        public const string C_F1_TRANSF_ENVIADA_DEBITO = "14812";//"5523";
        public const string C_F1_TRANSF_ENVIADA_CREDITO = "6758";//"521";
        public const string C_F1_TRANSF_ENVIADA_HISTORICO = "871";//"706";
        public const string C_F1_TRANSF_ENVIADA_DEBITO_ICMS = "6534";//"6191";
        public const string C_F1_TRANSF_ENVIADA_CREDITO_ICMS = "5820";//"2619";
        public const string C_F1_TRANSF_ENVIADA_HISTORICO_ICMS = "711";
        public const string C_F1_TRANSF_ENVIADA_DEBITO_IPI = "6617";//"6110";
        public const string C_F1_TRANSF_ENVIADA_CREDITO_IPI = "5842";//"2654";
        public const string C_F1_TRANSF_ENVIADA_HISTORICO_IPI = "711";

        // Constantes para operação de TRANSFERENCIA RECEBIDA
        // FILIAL 2 = RECIFE
        public const string C_F2_TRANSF_RECEBIDA_DEBITO = "6824";//"5641";
        public const string C_F2_TRANSF_RECEBIDA_CREDITO = "14841";//"5664";
        public const string C_F2_TRANSF_RECEBIDA_HISTORICO = "871";//"706";
        public const string C_F2_TRANSF_RECEBIDA_DEBITO_ICMS = "359";//"8616";
        public const string C_F2_TRANSF_RECEBIDA_CREDITO_ICMS = "6541";//"6209";
        public const string C_F2_TRANSF_RECEBIDA_HISTORICO_ICMS = "711";
        public const string C_F2_TRANSF_RECEBIDA_DEBITO_IPI = "336";//"12196";
        public const string C_F2_TRANSF_RECEBIDA_CREDITO_IPI = "6623";//"6126";
        public const string C_F2_TRANSF_RECEBIDA_HISTORICO_IPI = "711";

        // FILIAL 3 = FORTALEZA
        public const string C_F3_TRANSF_RECEBIDA_DEBITO = "10636";
        public const string C_F3_TRANSF_RECEBIDA_CREDITO = "10665";
        public const string C_F3_TRANSF_RECEBIDA_HISTORICO = "871";//"706";
        public const string C_F3_TRANSF_RECEBIDA_DEBITO_ICMS = "11966";
        public const string C_F3_TRANSF_RECEBIDA_CREDITO_ICMS = "10955";
        public const string C_F3_TRANSF_RECEBIDA_HISTORICO_ICMS = "711";
        public const string C_F3_TRANSF_RECEBIDA_DEBITO_IPI = "11966";
        public const string C_F3_TRANSF_RECEBIDA_CREDITO_IPI = "10949";
        public const string C_F3_TRANSF_RECEBIDA_HISTORICO_IPI = "711";

        // FILIAL 1 = MATRIZ
        public const string C_F1_TRANSF_RECEBIDA_DEBITO = "6735";//"521";
        public const string C_F1_TRANSF_RECEBIDA_CREDITO = "14829";//"5501";
        public const string C_F1_TRANSF_RECEBIDA_HISTORICO = "871";//"706";
        public const string C_F1_TRANSF_RECEBIDA_DEBITO_ICMS = "342";//"5731";
        public const string C_F1_TRANSF_RECEBIDA_CREDITO_ICMS = "6534";//"6191";
        public const string C_F1_TRANSF_RECEBIDA_HISTORICO_ICMS = "711";
        public const string C_F1_TRANSF_RECEBIDA_DEBITO_IPI = "320";//"922";
        public const string C_F1_TRANSF_RECEBIDA_CREDITO_IPI = "6617";//"6110";
        public const string C_F1_TRANSF_RECEBIDA_HISTORICO_IPI = "711";


        // Constantes para operação de COMPRAS PARA REVENDA
        // FILIAL 2 = RECIFE
        public const string C_F2_COMPRA_REVENDA_DEBITO = "6847";//"5641";
        public const string C_F2_COMPRA_REVENDA_HISTORICO = "660";
        public const string C_F2_COMPRA_REVENDA_DEBITO_ICMS = "359";//"8616";
        public const string C_F2_COMPRA_REVENDA_CREDITO_ICMS = "6847";//"5641";
        public const string C_F2_COMPRA_REVENDA_HISTORICO_ICMS = "711";
        public const string C_F2_COMPRA_REVENDA_DEBITO_PIS = "282";//"900";
        public const string C_F2_COMPRA_REVENDA_CREDITO_PIS = "6847";//"5641";
        public const string C_F2_COMPRA_REVENDA_HISTORICO_PIS = "711";
        public const string C_F2_COMPRA_REVENDA_DEBITO_COFINS = "307";//"916";
        public const string C_F2_COMPRA_REVENDA_CREDITO_COFINS = "6847";//"5641";
        public const string C_F2_COMPRA_REVENDA_HISTORICO_COFINS = "711";
        public const string C_F2_COMPRA_REVENDA_DEBITO_IPI = "336";//"12196";
        public const string C_F2_COMPRA_REVENDA_CREDITO_IPI = "6847";//"5641";
        public const string C_F2_COMPRA_REVENDA_HISTORICO_IPI = "711";
        // FILIAL 3 = FORTALEZA
        public const string C_F3_COMPRA_REVENDA_DEBITO = "10613";
        public const string C_F3_COMPRA_REVENDA_HISTORICO = "660";
        public const string C_F3_COMPRA_REVENDA_DEBITO_ICMS = "11966";
        public const string C_F3_COMPRA_REVENDA_CREDITO_ICMS = "10613";
        public const string C_F3_COMPRA_REVENDA_HISTORICO_ICMS = "711";
        public const string C_F3_COMPRA_REVENDA_DEBITO_PIS = "282";//"900";
        public const string C_F3_COMPRA_REVENDA_CREDITO_PIS = "10613";
        public const string C_F3_COMPRA_REVENDA_HISTORICO_PIS = "711";
        public const string C_F3_COMPRA_REVENDA_DEBITO_COFINS = "307";//"916";
        public const string C_F3_COMPRA_REVENDA_CREDITO_COFINS = "10613";
        public const string C_F3_COMPRA_REVENDA_HISTORICO_COFINS = "711";
        public const string C_F3_COMPRA_REVENDA_DEBITO_IPI = "12687";
        public const string C_F3_COMPRA_REVENDA_CREDITO_IPI = "10636";
        public const string C_F3_COMPRA_REVENDA_HISTORICO_IPI = "711";
        // FILIAL 1 = MATRIZ
        public const string C_F1_COMPRA_REVENDA_DEBITO = "6712";//"521";
        public const string C_F1_COMPRA_REVENDA_HISTORICO = "660";
        public const string C_F1_COMPRA_REVENDA_DEBITO_ICMS = "342";//"5731";
        public const string C_F1_COMPRA_REVENDA_CREDITO_ICMS = "6712";//"521";
        public const string C_F1_COMPRA_REVENDA_HISTORICO_ICMS = "711";
        public const string C_F1_COMPRA_REVENDA_DEBITO_PIS = "282";//"900";
        public const string C_F1_COMPRA_REVENDA_CREDITO_PIS = "6712";//"521";
        public const string C_F1_COMPRA_REVENDA_HISTORICO_PIS = "711";
        public const string C_F1_COMPRA_REVENDA_DEBITO_COFINS = "307";//"916";
        public const string C_F1_COMPRA_REVENDA_CREDITO_COFINS = "6712";//"521";
        public const string C_F1_COMPRA_REVENDA_HISTORICO_COFINS = "711";
        public const string C_F1_COMPRA_REVENDA_DEBITO_IPI = "";
        public const string C_F1_COMPRA_REVENDA_CREDITO_IPI = "";
        public const string C_F1_COMPRA_REVENDA_HISTORICO_IPI = "711";

        // Constantes para operação de VENDAS MERCADORIA COM INCENTIVO
        // FILIAL 2 = RECIFE
        public const string C_F2_VENDAS_MPBEM_DEBITO = "31591";//"715";
        public const string C_F2_VENDAS_MPBEM_CREDITO = "8065";
        public const string C_F2_VENDAS_MPBEM_HISTORICO = "674";
        public const string C_F2_VENDAS_MPBEM_DEBITO_ICMS = "6541";//"6209";
        public const string C_F2_VENDAS_MPBEM_CREDITO_ICMS = "5836";//"2625";
        public const string C_F2_VENDAS_MPBEM_HISTORICO_ICMS = "711";
        public const string C_F2_VENDAS_MPBEM_DEBITO_CUSTO = "6601";
        public const string C_F2_VENDAS_MPBEM_CREDITO_CUSTO = "5718";
        public const string C_F2_VENDAS_MPBEM_HISTORICO_CUSTO = "871";
        public const string C_F2_VENDAS_MPBEM_DEBITO_IPI = "6623";//"6126";
        public const string C_F2_VENDAS_MPBEM_CREDITO_IPI = "12180";
        public const string C_F2_VENDAS_MPBEM_HISTORICO_IPI = "711";
        // FILIAL 3 = FORTALEZA
        public const string C_F3_VENDAS_MPBEM_DEBITO = "31591";//"715";
        public const string C_F3_VENDAS_MPBEM_CREDITO = "10910";
        public const string C_F3_VENDAS_MPBEM_HISTORICO = "674";
        public const string C_F3_VENDAS_MPBEM_DEBITO_ICMS = "10955";
        public const string C_F3_VENDAS_MPBEM_CREDITO_ICMS = "10895";
        public const string C_F3_VENDAS_MPBEM_HISTORICO_ICMS = "711";
        public const string C_F3_VENDAS_MPBEM_DEBITO_CUSTO = "11038";
        public const string C_F3_VENDAS_MPBEM_CREDITO_CUSTO = "10719";
        public const string C_F3_VENDAS_MPBEM_HISTORICO_CUSTO = "871";
        public const string C_F3_VENDAS_MPBEM_DEBITO_IPI = "10949";
        public const string C_F3_VENDAS_MPBEM_CREDITO_IPI = "12693";
        public const string C_F3_VENDAS_MPBEM_HISTORICO_IPI = "711";
        // FILIAL 1 = MATRIZ
        public const string C_F1_VENDAS_MPBEM_DEBITO = "31591";//"715";
        public const string C_F1_VENDAS_MPBEM_CREDITO = "6445";
        public const string C_F1_VENDAS_MPBEM_HISTORICO = "674";
        public const string C_F1_VENDAS_MPBEM_DEBITO_ICMS = "";
        public const string C_F1_VENDAS_MPBEM_CREDITO_ICMS = "";
        public const string C_F1_VENDAS_MPBEM_HISTORICO_ICMS = "711";
        public const string C_F1_VENDAS_MPBEM_DEBITO_CUSTO = "6592";
        public const string C_F1_VENDAS_MPBEM_CREDITO_CUSTO = "5581";
        public const string C_F1_VENDAS_MPBEM_HISTORICO_CUSTO = "871";
        public const string C_F1_VENDAS_MPBEM_DEBITO_IPI = "";
        public const string C_F1_VENDAS_MPBEM_CREDITO_IPI = "";
        public const string C_F1_VENDAS_MPBEM_HISTORICO_IPI = "711";

        // Constantes para operação de VENDAS MERCADORIA SEM INCENTIVO
        // FILIAL 2 = RECIFE
        public const string C_F2_VENDAS_MERC_DEBITO = "31591";//"715";
        public const string C_F2_VENDAS_MERC_CREDITO = "8071";
        public const string C_F2_VENDAS_MERC_HISTORICO = "674";
        public const string C_F2_VENDAS_MERC_DEBITO_ICMS = "6541";//"6209";
        public const string C_F2_VENDAS_MERC_CREDITO_ICMS = "5836";//"2625";
        public const string C_F2_VENDAS_MERC_HISTORICO_ICMS = "711";
        public const string C_F2_VENDAS_MERC_DEBITO_PIS = "6557";//"6215";
        public const string C_F2_VENDAS_MERC_CREDITO_PIS = "5865";//"2631";
        public const string C_F2_VENDAS_MERC_HISTORICO_PIS = "711";
        public const string C_F2_VENDAS_MERC_DEBITO_COFINS = "6244";
        public const string C_F2_VENDAS_MERC_CREDITO_COFINS = "5871";//"2648";
        public const string C_F2_VENDAS_MERC_HISTORICO_COFINS = "711";
        public const string C_F2_VENDAS_MERC_DEBITO_IPI = "6623";//"6126";
        public const string C_F2_VENDAS_MERC_CREDITO_IPI = "12180";
        public const string C_F2_VENDAS_MERC_HISTORICO_IPI = "711";
        public const string C_F2_VENDAS_MERC_DEBITO_CUSTO = "6623";
        public const string C_F2_VENDAS_MERC_CREDITO_CUSTO = "5724";
        public const string C_F2_VENDAS_MERC_HISTORICO_CUSTO = "871";
        // FILIAL 3 = FORTALEZA
        public const string C_F3_VENDAS_MERC_DEBITO = "31591";//"715";
        public const string C_F3_VENDAS_MERC_CREDITO = "10926";
        public const string C_F3_VENDAS_MERC_HISTORICO = "674";
        public const string C_F3_VENDAS_MERC_DEBITO_ICMS = "10955";
        public const string C_F3_VENDAS_MERC_CREDITO_ICMS = "10895";
        public const string C_F3_VENDAS_MERC_HISTORICO_ICMS = "711";
        public const string C_F3_VENDAS_MERC_DEBITO_PIS = "10961";
        public const string C_F3_VENDAS_MERC_CREDITO_PIS = "5865";//"2631";
        public const string C_F3_VENDAS_MERC_HISTORICO_PIS = "711";
        public const string C_F3_VENDAS_MERC_DEBITO_COFINS = "10978";
        public const string C_F3_VENDAS_MERC_CREDITO_COFINS = "5871";//"2648";
        public const string C_F3_VENDAS_MERC_HISTORICO_COFINS = "711";
        public const string C_F3_VENDAS_MERC_DEBITO_IPI = "10949";
        public const string C_F3_VENDAS_MERC_CREDITO_IPI = "12693";
        public const string C_F3_VENDAS_MERC_HISTORICO_IPI = "711";
        public const string C_F3_VENDAS_MERC_DEBITO_CUSTO = "11044";
        public const string C_F3_VENDAS_MERC_CREDITO_CUSTO = "10725";
        public const string C_F3_VENDAS_MERC_HISTORICO_CUSTO = "871";
        // FILIAL 1 = MATRIZ
        public const string C_F1_VENDAS_MERC_DEBITO = "31591";//"715";
        public const string C_F1_VENDAS_MERC_CREDITO = "6445";//"8059";
        public const string C_F1_VENDAS_MERC_HISTORICO = "674";
        public const string C_F1_VENDAS_MERC_DEBITO_ICMS = "6534";//"6191";
        public const string C_F1_VENDAS_MERC_CREDITO_ICMS = "5820";//"2619";
        public const string C_F1_VENDAS_MERC_HISTORICO_ICMS = "711";
        public const string C_F1_VENDAS_MERC_DEBITO_PIS = "6557";//"6215";
        public const string C_F1_VENDAS_MERC_CREDITO_PIS = "5865";//"2631";
        public const string C_F1_VENDAS_MERC_HISTORICO_PIS = "711";
        public const string C_F1_VENDAS_MERC_DEBITO_COFINS = "6570";//"6238";
        public const string C_F1_VENDAS_MERC_CREDITO_COFINS = "5871";//"2648";
        public const string C_F1_VENDAS_MERC_HISTORICO_COFINS = "711";
        public const string C_F1_VENDAS_MERC_DEBITO_IPI = "6617";//"6110";
        public const string C_F1_VENDAS_MERC_CREDITO_IPI = "5842";//"2654";
        public const string C_F1_VENDAS_MERC_HISTORICO_IPI = "711";
        public const string C_F1_VENDAS_MERC_DEBITO_CUSTO = "6617";
        public const string C_F1_VENDAS_MERC_CREDITO_CUSTO = "5598";
        public const string C_F1_VENDAS_MERC_HISTORICO_CUSTO = "871";

        // Constantes para operação VENDA PRODUÇÃO BENEFICIADAS e NÃO BENEFICIDAS
        // FILIAL 1 = MATRIZ
        public const string C_F1_VENDAS_PROD_DEBITO = "31591";//"715";
        public const string C_F1_VENDAS_PROD_CREDITO = "6400";//"8036";
        public const string C_F1_VENDAS_PROD_MPBEM_CREDITO = "6391";//"8020";
        public const string C_F1_VENDAS_PROD_HISTORICO = "674";

        public const string C_F1_VENDAS_PROD_DEBITO_CUSTO = "6541";
        public const string C_F1_VENDAS_PROD_CREDITO_CUSTO = "5575";
        
        public const string C_F1_VENDAS_PROD_MPBEM_DEBITO_CUSTO = "6511";
        public const string C_F1_VENDAS_PROD_MPBEM_CREDITO_CUSTO = "5569";


        // Constantes para Operação de IMPORTAÇÃO e COMPRA PARA INDUSTRIALIZAÇÃO
        // FILIAL 1 = MATRIZ
        public const string C_F1_IMPORTACAO_DEBITO = "7249";//"490";
        public const string C_F1_IMPORTACAO_CREDITO = "2281";
        public const string C_F1_IMPORTACAO_HISTORICO = "660";
        public const string C_F1_IMPORTACAO_CREDITO_IPI = "7249";//"490";
        public const string C_F1_IMPORTACAO_CREDITO_PIS = "7249";//"490";
        public const string C_F1_IMPORTACAO_CREDITO_COFINS = "7249";//"490";
        public const string C_F1_IMPORTACAO_COMPRA_INDUST_HISTORICO = "711";
        public const string C_F1_IMPORTACAO_COMPRA_INDUST_DEBITO_IPI = "320";//"922";

        public const string C_F1_COMPRA_INDUST_CREDITO = "7232";//"483";
        public const string C_F1_COMPRA_INDUST_DEBITO = "7249";//"490";
        public const string C_F1_COMPRA_INDUST_HISTORICO = "660";
        public const string C_F1_COMPRA_INDUST_CREDITO_IPI = "7232";//"483";
        public const string C_F1_COMPRA_INDUST_CREDITO_PIS = "7232";//"483";
        public const string C_F1_COMPRA_INDUST_CREDITO_COFINS = "7232";//"483";



        // Constantes para operação de COMPRAS PARA USO E CONSUMO
        // FILIAL 2 = RECIFE
        public const string C_F2_USO_CONSUMO_DEBITO = "7380";
        public const string C_F2_USO_CONSUMO_HISTORICO = "660";
        // FILIAL 3 = FORTALEZA
        public const string C_F3_USO_CONSUMO_DEBITO = "11771";
        public const string C_F3_USO_CONSUMO_HISTORICO = "660";
        // FILIAL 1 = MATRIZ
        public const string C_F1_USO_CONSUMO_DEBITO = "6920";
        public const string C_F1_USO_CONSUMO_HISTORICO = "660";

        // Constantes para operação de ENTRADA DE FRETE DE VENDAS
        // FILIAL 2 = RECIFE
        public const string C_F2_FRETE_VENDA_DEBITO = "7440";
        public const string C_F2_FRETE_VENDA_HISTORICO = "660";
        public const string C_F2_FRETE_VENDA_DEBITO_PIS = "282";//"900";
        public const string C_F2_FRETE_VENDA_CREDITO_PIS = "6221";
        public const string C_F2_FRETE_VENDA_HISTORICO_PIS = "711";
        public const string C_F2_FRETE_VENDA_DEBITO_COFINS = "307";//"916";
        public const string C_F2_FRETE_VENDA_CREDITO_COFINS = "6244";
        public const string C_F2_FRETE_VENDA_HISTORICO_COFINS = "711";
        // FILIAL 3 = FORTALEZA
        public const string C_F3_FRETE_VENDA_DEBITO = "11854";
        public const string C_F3_FRETE_VENDA_HISTORICO = "660";
        public const string C_F3_FRETE_VENDA_DEBITO_PIS = "282";//"900";
        public const string C_F3_FRETE_VENDA_CREDITO_PIS = "10961";
        public const string C_F3_FRETE_VENDA_HISTORICO_PIS = "711";
        public const string C_F3_FRETE_VENDA_DEBITO_COFINS = "307";//"916";
        public const string C_F3_FRETE_VENDA_CREDITO_COFINS = "10978";
        public const string C_F3_FRETE_VENDA_HISTORICO_COFINS = "711";
        // FILIAL 1 = MATRIZ
        public const string C_F1_FRETE_VENDA_DEBITO = "25282";//"7232";
        public const string C_F1_FRETE_VENDA_HISTORICO = "660";
        public const string C_F1_FRETE_VENDA_DEBITO_PIS = "282";//"900";
        public const string C_F1_FRETE_VENDA_CREDITO_PIS = "6557";//"6215";
        public const string C_F1_FRETE_VENDA_HISTORICO_PIS = "711";
        public const string C_F1_FRETE_VENDA_DEBITO_COFINS = "307";//"916";
        public const string C_F1_FRETE_VENDA_CREDITO_COFINS = "6570";//"6238";
        public const string C_F1_FRETE_VENDA_HISTORICO_COFINS = "711";

        // Constantes para operação de ENTRADA DE FRETE DE COMPRAS
        // FILIAL 2 = RECIFE
        public const string C_F2_FRETE_COMPRA_DEBITO = "27909";//"7775";
        public const string C_F2_FRETE_COMPRA_HISTORICO = "660";
        public const string C_F2_FRETE_COMPRA_DEBITO_ICMS = "359";//"8616";
        public const string C_F2_FRETE_COMPRA_CREDITO_ICMS = "27909";//"7775";
        public const string C_F2_FRETE_COMPRA_HISTORICO_ICMS = "711";
        public const string C_F2_FRETE_COMPRA_DEBITO_PIS = "282";//"900";
        public const string C_F2_FRETE_COMPRA_CREDITO_PIS = "27909";//"7775";
        public const string C_F2_FRETE_COMPRA_HISTORICO_PIS = "711";
        public const string C_F2_FRETE_COMPRA_DEBITO_COFINS = "307";//"916";
        public const string C_F2_FRETE_COMPRA_CREDITO_COFINS = "27909";//"7775";
        public const string C_F2_FRETE_COMPRA_HISTORICO_COFINS = "711";
        // FILIAL 3 = FORTALEZA
        public const string C_F3_FRETE_COMPRA_DEBITO = "10642";
        public const string C_F3_FRETE_COMPRA_HISTORICO = "660";
        public const string C_F3_FRETE_COMPRA_DEBITO_ICMS = "11966";
        public const string C_F3_FRETE_COMPRA_CREDITO_ICMS = "10642";
        public const string C_F3_FRETE_COMPRA_HISTORICO_ICMS = "711";
        public const string C_F3_FRETE_COMPRA_DEBITO_PIS = "282";//"900";
        public const string C_F3_FRETE_COMPRA_CREDITO_PIS = "10642";
        public const string C_F3_FRETE_COMPRA_HISTORICO_PIS = "711";
        public const string C_F3_FRETE_COMPRA_DEBITO_COFINS = "307";//"916";
        public const string C_F3_FRETE_COMPRA_CREDITO_COFINS = "10642";
        public const string C_F3_FRETE_COMPRA_HISTORICO_COFINS = "711";
        // FILIAL 1 = MATRIZ
        public const string C_F1_FRETE_COMPRA_DEBITO = "27891";//"537";
        public const string C_F1_FRETE_COMPRA_HISTORICO = "660";
        public const string C_F1_FRETE_COMPRA_DEBITO_ICMS = "342";//"5731";
        public const string C_F1_FRETE_COMPRA_CREDITO_ICMS = "27891";//"537";
        public const string C_F1_FRETE_COMPRA_HISTORICO_ICMS = "711";
        public const string C_F1_FRETE_COMPRA_DEBITO_PIS = "282";//"900";
        public const string C_F1_FRETE_COMPRA_CREDITO_PIS = "27891";//"537";
        public const string C_F1_FRETE_COMPRA_HISTORICO_PIS = "711";
        public const string C_F1_FRETE_COMPRA_DEBITO_COFINS = "307";//"916";
        public const string C_F1_FRETE_COMPRA_CREDITO_COFINS = "27891";//"537";
        public const string C_F1_FRETE_COMPRA_HISTORICO_COFINS = "711";

        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_DEBITO = "7249";//"490";
        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_HISTORICO = "660";
        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_DEBITO_ICMS = "342";//"8616";
        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_CREDITO_ICMS = "7249";//"490";
        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_HISTORICO_ICMS = "711";
        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_DEBITO_PIS = "282";//"900";
        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_CREDITO_PIS = "7249";//"490";
        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_HISTORICO_PIS = "711";
        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_DEBITO_COFINS = "307";//"916";
        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_CREDITO_COFINS = "7249";//"490";
        public const string C_F1_FRETE_REMOCAO_IMPORTACAO_HISTORICO_COFINS = "711";

        // Constantes para operação de SERVICOS DE IMPORTACAO
        public const string C_F1_SERVICOS_IMPORTACAO_DEBITO = "7249";//"490";
        public const string C_F1_SERVICOS_IMPORTACAO_HISTORICO = "660";
        public const string C_F1_SERVICOS_IMPORTACAO_DEBITO_PIS = "282";//"900";
        public const string C_F1_SERVICOS_IMPORTACAO_CREDITO_PIS = "7249";//"490";
        public const string C_F1_SERVICOS_IMPORTACAO_HISTORICO_PIS = "711";
        public const string C_F1_SERVICOS_IMPORTACAO_DEBITO_COFINS = "307";//"916";
        public const string C_F1_SERVICOS_IMPORTACAO_CREDITO_COFINS = "7249";//"490";
        public const string C_F1_SERVICOS_IMPORTACAO_HISTORICO_COFINS = "711";

        // Constantes para operação de COMPRAS BONIFICADAS
        // FILIAL 2 = RECIFE
        public const string C_F2_COMPRA_BONIFICADA_DEBITO = "6847";//"5641";
        public const string C_F2_COMPRA_BONIFICADA_CREDITO = "6468";
        public const string C_F2_COMPRA_BONIFICADA_HISTORICO = "660";
        public const string C_F2_COMPRA_BONIFICADA_DEBITO_ICMS = "359";//"8616";
        public const string C_F2_COMPRA_BONIFICADA_CREDITO_ICMS = "6847";//"5641";
        public const string C_F2_COMPRA_BONIFICADA_HISTORICO_ICMS = "711";
        public const string C_F2_COMPRA_BONIFICADA_DEBITO_PIS = "282";//"900";
        public const string C_F2_COMPRA_BONIFICADA_CREDITO_PIS = "6847";//"5641";
        public const string C_F2_COMPRA_BONIFICADA_HISTORICO_PIS = "711";
        public const string C_F2_COMPRA_BONIFICADA_DEBITO_COFINS = "307";//"916";
        public const string C_F2_COMPRA_BONIFICADA_CREDITO_COFINS = "6847";//"5641";
        public const string C_F2_COMPRA_BONIFICADA_HISTORICO_COFINS = "711";
        public const string C_F2_COMPRA_BONIFICADA_DEBITO_IPI = "336";//"12196";
        public const string C_F2_COMPRA_BONIFICADA_CREDITO_IPI = "6847";//"5641";
        public const string C_F2_COMPRA_BONIFICADA_HISTORICO_IPI = "711";
        // FILIAL 3 = FORTALEZA
        public const string C_F3_COMPRA_BONIFICADA_DEBITO = "10636";
        public const string C_F3_COMPRA_BONIFICADA_CREDITO = "6468";
        public const string C_F3_COMPRA_BONIFICADA_HISTORICO = "660";
        public const string C_F3_COMPRA_BONIFICADA_DEBITO_ICMS = "11966";
        public const string C_F3_COMPRA_BONIFICADA_CREDITO_ICMS = "10636";
        public const string C_F3_COMPRA_BONIFICADA_HISTORICO_ICMS = "711";
        public const string C_F3_COMPRA_BONIFICADA_DEBITO_PIS = "282";//"900";
        public const string C_F3_COMPRA_BONIFICADA_CREDITO_PIS = "10636";
        public const string C_F3_COMPRA_BONIFICADA_HISTORICO_PIS = "711";
        public const string C_F3_COMPRA_BONIFICADA_DEBITO_COFINS = "307";//"916";
        public const string C_F3_COMPRA_BONIFICADA_CREDITO_COFINS = "10636";
        public const string C_F3_COMPRA_BONIFICADA_HISTORICO_COFINS = "711";
        public const string C_F3_COMPRA_BONIFICADA_DEBITO_IPI = "12687";
        public const string C_F3_COMPRA_BONIFICADA_CREDITO_IPI = "10636";
        public const string C_F3_COMPRA_BONIFICADA_HISTORICO_IPI = "711";
        // FILIAL 1 = MATRIZ
        public const string C_F1_COMPRA_BONIFICADA_DEBITO = "6712";//"521";
        public const string C_F1_COMPRA_BONIFICADA_CREDITO = "6468";
        public const string C_F1_COMPRA_BONIFICADA_HISTORICO = "660";

        public const string C_F1_COMPRA_BONIFICADA_DEBITO_ICMS = "";
        public const string C_F1_COMPRA_BONIFICADA_CREDITO_ICMS = "6712";//"521";
        public const string C_F1_COMPRA_BONIFICADA_HISTORICO_ICMS = "711";
        public const string C_F1_COMPRA_BONIFICADA_DEBITO_PIS = "";
        public const string C_F1_COMPRA_BONIFICADA_CREDITO_PIS = "6712";//"521";
        public const string C_F1_COMPRA_BONIFICADA_HISTORICO_PIS = "711";
        public const string C_F1_COMPRA_BONIFICADA_DEBITO_COFINS = "";
        public const string C_F1_COMPRA_BONIFICADA_CREDITO_COFINS = "6712";//"521";
        public const string C_F1_COMPRA_BONIFICADA_HISTORICO_COFINS = "711";
        public const string C_F1_COMPRA_BONIFICADA_DEBITO_IPI = "";
        public const string C_F1_COMPRA_BONIFICADA_CREDITO_IPI = "";
        public const string C_F1_COMPRA_BONIFICADA_HISTORICO_IPI = "711";

        // Constantes para operação de DEVOLUÇÃO DE VENDAS
        // FILIAL 2 = RECIFE
        public const string C_F2_DEVOLUCAO_VENDA_DEBITO = "6296";
        public const string C_F2_DEVOLUCAO_VENDA_CREDITO = "31591";//"715";
        public const string C_F2_DEVOLUCAO_VENDA_HISTORICO = "693";
        public const string C_F2_DEVOLUCAO_VENDA_DEBITO_ICMS = "359";//"8616";
        public const string C_F2_DEVOLUCAO_VENDA_CREDITO_ICMS = "6541";//"6209";
        public const string C_F2_DEVOLUCAO_VENDA_HISTORICO_ICMS = "711";
        public const string C_F2_DEVOLUCAO_VENDA_DEBITO_PIS = "282";//"900";
        public const string C_F2_DEVOLUCAO_VENDA_CREDITO_PIS = "6221";
        public const string C_F2_DEVOLUCAO_VENDA_HISTORICO_PIS = "711";
        public const string C_F2_DEVOLUCAO_VENDA_DEBITO_COFINS = "307";//"916";
        public const string C_F2_DEVOLUCAO_VENDA_CREDITO_COFINS = "6244";
        public const string C_F2_DEVOLUCAO_VENDA_HISTORICO_COFINS = "711";
        public const string C_F2_DEVOLUCAO_VENDA_DEBITO_IPI = "6617";//"320";//"922";
        public const string C_F2_DEVOLUCAO_VENDA_CREDITO_IPI = "6623";//"6126";
        public const string C_F2_DEVOLUCAO_VENDA_HISTORICO_IPI = "711";
        public const string C_F2_DEVOLUCAO_VENDA_DEBITO_CUSTO = "5724";
        public const string C_F2_DEVOLUCAO_VENDA_CREDITO_CUSTO = "6617";
        public const string C_F2_DEVOLUCAO_VENDA_HISTORICO_CUSTO = "871";
        public const string C_F2_DEVOLUCAO_VENDA_MERC_MPBEM_CREDITO_CUSTO = "";
        
        // FILIAL 3 = FORTALEZA
        public const string C_F3_DEVOLUCAO_VENDA_DEBITO = "10991";
        public const string C_F3_DEVOLUCAO_VENDA_CREDITO = "31591";//"715";
        public const string C_F3_DEVOLUCAO_VENDA_HISTORICO = "693";
        public const string C_F3_DEVOLUCAO_VENDA_DEBITO_ICMS = "11966";
        public const string C_F3_DEVOLUCAO_VENDA_CREDITO_ICMS = "10955";
        public const string C_F3_DEVOLUCAO_VENDA_HISTORICO_ICMS = "711";
        public const string C_F3_DEVOLUCAO_VENDA_DEBITO_PIS = "282";//"900";
        public const string C_F3_DEVOLUCAO_VENDA_CREDITO_PIS = "6557";//"6215";
        public const string C_F3_DEVOLUCAO_VENDA_HISTORICO_PIS = "711";
        public const string C_F3_DEVOLUCAO_VENDA_DEBITO_COFINS = "307";//"916";
        public const string C_F3_DEVOLUCAO_VENDA_CREDITO_COFINS = "6570";//"6238";
        public const string C_F3_DEVOLUCAO_VENDA_HISTORICO_COFINS = "711";
        public const string C_F3_DEVOLUCAO_VENDA_DEBITO_IPI = "12687";
        public const string C_F3_DEVOLUCAO_VENDA_CREDITO_IPI = "10949";
        public const string C_F3_DEVOLUCAO_VENDA_HISTORICO_IPI = "711";
        public const string C_F3_DEVOLUCAO_VENDA_DEBITO_CUSTO = "10725";
        public const string C_F3_DEVOLUCAO_VENDA_CREDITO_CUSTO = "11044";
        public const string C_F3_DEVOLUCAO_VENDA_HISTORICO_CUSTO = "871";

        public const string C_F3_DEVOLUCAO_VENDA_MERC_MPBEM_CREDITO_CUSTO = "";

        // FILIAL 1 = MATRIZ
        public const string C_F1_DEVOLUCAO_VENDA_DEBITO = "6646";//"6280";
        public const string C_F1_DEVOLUCAO_VENDA_CREDITO = "31591";//"715";
        public const string C_F1_DEVOLUCAO_VENDA_HISTORICO = "693";
        public const string C_F1_DEVOLUCAO_VENDA_DEBITO_ICMS = "342";//"5731";
        public const string C_F1_DEVOLUCAO_VENDA_CREDITO_ICMS = "6534";//"6191";
        public const string C_F1_DEVOLUCAO_VENDA_HISTORICO_ICMS = "711";
        public const string C_F1_DEVOLUCAO_VENDA_DEBITO_PIS = "282";//"900";
        public const string C_F1_DEVOLUCAO_VENDA_CREDITO_PIS = "6557";//"6280";
        public const string C_F1_DEVOLUCAO_VENDA_HISTORICO_PIS = "711";
        public const string C_F1_DEVOLUCAO_VENDA_DEBITO_COFINS = "307";//"916";
        public const string C_F1_DEVOLUCAO_VENDA_CREDITO_COFINS = "6570";//"6280";
        public const string C_F1_DEVOLUCAO_VENDA_HISTORICO_COFINS = "711";
        public const string C_F1_DEVOLUCAO_VENDA_DEBITO_IPI = "320";//"6110";
        public const string C_F1_DEVOLUCAO_VENDA_CREDITO_IPI = "6617";//"922";
        public const string C_F1_DEVOLUCAO_VENDA_HISTORICO_IPI = "711";

        //public const string C_F1_DEVOLUCAO_VENDA_DEBITO_CUSTO = "5598";
        //public const string C_F1_DEVOLUCAO_VENDA_CREDITO_CUSTO = "6617";
        //public const string C_F1_DEVOLUCAO_VENDA_HISTORICO_CUSTO = "871";

        //public const string C_F1_DEVOLUCAO_VENDA_PROD_MPBEM_CREDITO_CUSTO = "6511";
        //public const string C_F1_DEVOLUCAO_VENDA_PROD_MPBEM_DEBITO_CUSTO = "5569";
        //public const string C_F1_DEVOLUCAO_VENDA_PROD_CREDITO_CUSTO = "6541";
        //public const string C_F1_DEVOLUCAO_VENDA_PROD_DEBITO_CUSTO = "5575";
        //public const string C_F1_DEVOLUCAO_VENDA_MPBEM_CREDITO_CUSTO = "6592";
        //public const string C_F1_DEVOLUCAO_VENDA_MPBEM_DEBITO_CUSTO = "5581";
    }
}
