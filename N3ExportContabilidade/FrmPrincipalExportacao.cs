using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace N3ExportContabilidade
{
    public partial class FrmPrincipalExportacao : Form
    {
        private ExportadorDados exportadorDados;

        public FrmPrincipalExportacao()
        {
            InitializeComponent();
            exportadorDados = new ExportadorDados();
            txtDiretorio.Text = Constantes.CAMINHO_ARQS_EXPORTACAO;
        }

        private void btnGerarArquivos_Click(object sender, EventArgs e) {
            try {
                DateTime dataInicial = datePickerInicial.Value;
                DateTime dataFinal = datePickerFinal.Value;
                bool selecionou = false;
                bool erroData = false;
                Cursor.Current = Cursors.WaitCursor;

                if (chkBoxArquivosCompras.Checked) {
                    if (dataInicial.CompareTo(dataFinal) < 0) {
                        exportadorDados.geraArquivoExportacaoCompras(dataInicial, dataFinal, txtDiretorio.Text, 
                            chkBoxFilialMatriz.Checked, chkBoxFilialRecife.Checked, chkBoxFortaleza.Checked);
                    } else {
                        MessageBox.Show(this, "Data inicial precisa ser anterior a data final!", "Erro");

                    }
                    selecionou = true;
                }
                if (chkBoxArquivosVendas.Checked) {
                    if (dataInicial.CompareTo(dataFinal) < 0) {
                        exportadorDados.geraArquivoExportacaoVendas(dataInicial, dataFinal, txtDiretorio.Text,
                            chkBoxFilialMatriz.Checked, chkBoxFilialRecife.Checked, chkBoxFortaleza.Checked);
                    } else {
                        MessageBox.Show(this, "Data inicial precisa ser anterior a data final!", "Erro");
                    }
                    selecionou = true;
                }
                if (chkBoxArquivosImportacao.Checked) {
                    if (dataInicial.CompareTo(dataFinal) < 0) {
                        exportadorDados.geraArquivoExportacaoImportacao(dataInicial, dataFinal, txtDiretorio.Text,
                            chkBoxFilialMatriz.Checked, chkBoxFilialRecife.Checked, chkBoxFortaleza.Checked);
                    } else {
                        MessageBox.Show(this, "Data inicial precisa ser anterior a data final!", "Erro");
                    }
                    selecionou = true;
                }
                if (chkBoxArquivosDevolucao.Checked) {
                    if (dataInicial.CompareTo(dataFinal) < 0) {
                        exportadorDados.geraArquivoExportacaoDevolucaoVendas(dataInicial, dataFinal, txtDiretorio.Text,
                            chkBoxFilialMatriz.Checked, chkBoxFilialRecife.Checked, chkBoxFortaleza.Checked);
                    } else {
                        MessageBox.Show(this, "Data inicial precisa ser anterior a data final!", "Erro");
                    }
                    selecionou = true;
                }
                if (chkBoxArquivoFretesVendas.Checked) {
                    if (dataInicial.CompareTo(dataFinal) < 0) {
                        exportadorDados.geraArquivoExportacaoFreteVendas(dataInicial, dataFinal, txtDiretorio.Text,
                            chkBoxFilialMatriz.Checked, chkBoxFilialRecife.Checked, chkBoxFortaleza.Checked);
                    } else {
                        MessageBox.Show(this, "Data inicial precisa ser anterior a data final!", "Erro");
                    }
                    selecionou = true;
                }
                if (chkBoxArquivoFretesCompras.Checked) {
                    if (dataInicial.CompareTo(dataFinal) < 0) {
                        exportadorDados.geraArquivoExportacaoFreteCompras(dataInicial, dataFinal, txtDiretorio.Text,
                            chkBoxFilialMatriz.Checked, chkBoxFilialRecife.Checked, chkBoxFortaleza.Checked);
                    } else {
                        MessageBox.Show(this, "Data inicial precisa ser anterior a data final!", "Erro");
                    }
                    selecionou = true;
                }

                if (chkBoxArquivoTrasferenciasRecebidas.Checked) {
                    if (dataInicial.CompareTo(dataFinal) < 0) {
                        exportadorDados.geraArquivoExportacaoTransferenciaRecebida(dataInicial, dataFinal, txtDiretorio.Text,
                            chkBoxFilialMatriz.Checked, chkBoxFilialRecife.Checked, chkBoxFortaleza.Checked);
                    } else {
                        MessageBox.Show(this, "Data inicial precisa ser anterior a data final!", "Erro");
                    }
                    selecionou = true;
                }

                if (chkBoxArquivoTrasferenciasEnviadas.Checked) {
                    if (dataInicial.CompareTo(dataFinal) < 0) {
                        exportadorDados.geraArquivoExportacaoTransferenciaEnviada(dataInicial, dataFinal, txtDiretorio.Text,
                            chkBoxFilialMatriz.Checked, chkBoxFilialRecife.Checked, chkBoxFortaleza.Checked);
                    } else {
                        MessageBox.Show(this, "Data inicial precisa ser anterior a data final!", "Erro");
                    }
                    selecionou = true;
                }


                if (chkBoxArquivoComprasUsoConsumo.Checked) {
                    if (dataInicial.CompareTo(dataFinal) < 0) {
                        exportadorDados.geraArquivoExportacaoComprasUsoConsumo(dataInicial, dataFinal, txtDiretorio.Text,
                            chkBoxFilialMatriz.Checked, chkBoxFilialRecife.Checked, chkBoxFortaleza.Checked);
                    } else {
                        MessageBox.Show(this, "Data inicial precisa ser anterior a data final!", "Erro");
                    }
                    selecionou = true;
                }

                if (chkBoxArquivosServicoImportacao.Checked) {
                    if (dataInicial.CompareTo(dataFinal) < 0) {
                        exportadorDados.geraArquivoExportacaoServicosImportacao(dataInicial, dataFinal, txtDiretorio.Text,
                            chkBoxFilialMatriz.Checked, chkBoxFilialRecife.Checked, chkBoxFortaleza.Checked);
                    } else {
                        MessageBox.Show(this, "Data inicial precisa ser anterior a data final!", "Erro");
                    }
                    selecionou = true;
                }


                Cursor.Current = Cursors.Default;
                // Apresenta eventual log para criação de novas contas contábeis
                if (!selecionou) {
                    MessageBox.Show(this, "Selecione pelo menos uma opção de exportação!", "Erro");
                } else if (exportadorDados.existeContaContabilNaoCadastrada) {
                    MessageBox.Show(this, "Verifique o LOG apresentado e gere os arquivos selecionados novamente!!!", "Mensagem");
                } else {
                    MessageBox.Show(this, "Exportação com sucesso!", "Mensagem");
                }

                if (exportadorDados.existeContaContabilNaoCadastrada) {
                    FrmApresentaLog frmLog = new FrmApresentaLog();
                    frmLog.setTextoLog(exportadorDados.logCriacaoContasContabeis.ToString());
                    frmLog.Show();
                    // Limpa para uma nova exportação
                    exportadorDados.limpaLogContasContabeis();
                }


            } catch(Exception ex) {
                Cursor.Current = Cursors.Default;

                MessageBox.Show(this, ex.StackTrace, "Erro");
            }

        }

        private void btnDiretorio_Click(object sender, EventArgs e) {
            folderBrowserDialog.ShowDialog();
            txtDiretorio.Text = folderBrowserDialog.SelectedPath;
        }

        private void toolStripButton1_Click(object sender, EventArgs e) {
            FrmConfigContasCliFor frmConfigContasCliFor = new FrmConfigContasCliFor();
            frmConfigContasCliFor.ShowDialog();
        }

    }
}