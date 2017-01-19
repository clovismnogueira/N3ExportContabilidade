using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace N3ExportContabilidade
{
    public partial class FrmConfigContasCliFor: Form
    {
        private ExportadorDados exportadorDados;

        public FrmConfigContasCliFor()
        {
            InitializeComponent();
            exportadorDados = new ExportadorDados();
        }

        private void btnBuscar_Click(object sender, EventArgs e) {
            if (!txtBusca.Text.Equals("")) {
                string[] resultadoBusca = null;
                resultadoBusca = exportadorDados.consultaContaContabil(txtBusca.Text);
                if (resultadoBusca != null) {
                    lblCodigoResultadoConsulta.Text = resultadoBusca[0];
                    lblContaResultadoConsulta.Text = resultadoBusca[1];
                    lblRazaoResultadoConsulta.Text = resultadoBusca[2];
                } else {
                    MessageBox.Show(this, "Não existe conta contábil para o Código informado!!", "Erro");
                    lblCodigoResultadoConsulta.Text = "";
                    lblContaResultadoConsulta.Text = "";
                    lblRazaoResultadoConsulta.Text = "";
                }
            } else {
                MessageBox.Show(this, "Forneça um código para Consulta!", "Erro");
            }
        }

        private void btnCadastrar_Click(object sender, EventArgs e) {
            if (!txtCodigo.Text.Equals("")) {
                if (!txtContaContabil.Text.Equals("")) {
                    if (!txtRazaoSocial.Text.Equals("")) {
                        if (!exportadorDados.existeContaContabilCliFor(txtCodigo.Text)) {
                            bool cadastroComSucesso = exportadorDados.insereContaContabilCliFor(txtCodigo.Text, 
                                txtContaContabil.Text, txtRazaoSocial.Text);
                            if (cadastroComSucesso) {
                                MessageBox.Show(this, "Cadastro com Sucesso!", "Mensagem");
                            } else {
                                MessageBox.Show(this, "Dados não cadastrados!", "Erro");
                            }
                        } else {
                            MessageBox.Show(this, "Cli/For já cadastrado!", "Erro");
                        }
                    } else {
                        MessageBox.Show(this, "Razão Social não pode ficar em BRANCO!", "Erro");
                    }
                } else {
                    MessageBox.Show(this, "Conta Contábil não pode ficar em BRANCO!", "Erro");
                }
            } else {
                MessageBox.Show(this, "Código do Cli/For não pode ficar em BRANCO!", "Erro");
            }
        }

        /*
         * 
         * 
         * 
         **/
        private void btnAtualizar_Click(object sender, EventArgs e) {
            if (!txtCodigo.Text.Equals("")) {
                if (!txtContaContabil.Text.Equals("")) {
                    if (!txtRazaoSocial.Text.Equals("")) {
                        if (exportadorDados.existeContaContabilCliFor(txtCodigo.Text)) {
                            bool atualizacaoComSucesso = exportadorDados.atualizaContaContabilCliFor(txtCodigo.Text, 
                                txtContaContabil.Text, txtRazaoSocial.Text);
                            if (atualizacaoComSucesso) {
                                MessageBox.Show(this, "Atualização de Cadastro com Sucesso!", "Mensagem");
                            } else {
                                MessageBox.Show(this, "Dados não Atualizados!", "Erro");
                            }
                        } else {
                            MessageBox.Show(this, "Cli/For não Cadastrado para atualização!", "Erro");
                        }
                    } else {
                        MessageBox.Show(this, "Razão Social não pode ficar em BRANCO!", "Erro");
                    }
                } else {
                    MessageBox.Show(this, "Conta Contábil não pode ficar em BRANCO!", "Erro");
                }
            } else {
                MessageBox.Show(this, "Código do Cli/For não pode ficar em BRANCO!", "Erro");
            }
        }

        /*
         * 
         * 
         * 
         **/
        private void btnRemover_Click(object sender, EventArgs e) {
            if (!txtCodigo.Text.Equals("")) {
                if (exportadorDados.existeContaContabilCliFor(txtCodigo.Text)) {
                    bool remocaoComSucesso = exportadorDados.removeContaContabilCliFor(txtCodigo.Text);
                    if (remocaoComSucesso) {
                        MessageBox.Show(this, "Conta para o Cli/For " + txtCodigo.Text +
                            " removido com sucesso!" , "Mensagem");
                    } else {
                        MessageBox.Show(this, "Dados não Removidos!", "Erro");
                    }
                } else {
                    MessageBox.Show(this, "Não há conta cadastrada para esse Cli/For!", "Erro");
                }
            } else {
                MessageBox.Show(this, "Código do Cli/For não pode ser vazio!", "Erro");
            }
            txtCodigo.Text = "";
            txtContaContabil.Text = "";
            txtRazaoSocial.Text = "";
        }
    }
}