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
                    MessageBox.Show(this, "N�o existe conta cont�bil para o C�digo informado!!", "Erro");
                    lblCodigoResultadoConsulta.Text = "";
                    lblContaResultadoConsulta.Text = "";
                    lblRazaoResultadoConsulta.Text = "";
                }
            } else {
                MessageBox.Show(this, "Forne�a um c�digo para Consulta!", "Erro");
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
                                MessageBox.Show(this, "Dados n�o cadastrados!", "Erro");
                            }
                        } else {
                            MessageBox.Show(this, "Cli/For j� cadastrado!", "Erro");
                        }
                    } else {
                        MessageBox.Show(this, "Raz�o Social n�o pode ficar em BRANCO!", "Erro");
                    }
                } else {
                    MessageBox.Show(this, "Conta Cont�bil n�o pode ficar em BRANCO!", "Erro");
                }
            } else {
                MessageBox.Show(this, "C�digo do Cli/For n�o pode ficar em BRANCO!", "Erro");
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
                                MessageBox.Show(this, "Atualiza��o de Cadastro com Sucesso!", "Mensagem");
                            } else {
                                MessageBox.Show(this, "Dados n�o Atualizados!", "Erro");
                            }
                        } else {
                            MessageBox.Show(this, "Cli/For n�o Cadastrado para atualiza��o!", "Erro");
                        }
                    } else {
                        MessageBox.Show(this, "Raz�o Social n�o pode ficar em BRANCO!", "Erro");
                    }
                } else {
                    MessageBox.Show(this, "Conta Cont�bil n�o pode ficar em BRANCO!", "Erro");
                }
            } else {
                MessageBox.Show(this, "C�digo do Cli/For n�o pode ficar em BRANCO!", "Erro");
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
                        MessageBox.Show(this, "Dados n�o Removidos!", "Erro");
                    }
                } else {
                    MessageBox.Show(this, "N�o h� conta cadastrada para esse Cli/For!", "Erro");
                }
            } else {
                MessageBox.Show(this, "C�digo do Cli/For n�o pode ser vazio!", "Erro");
            }
            txtCodigo.Text = "";
            txtContaContabil.Text = "";
            txtRazaoSocial.Text = "";
        }
    }
}