namespace N3ExportContabilidade
{
    partial class FrmConfigContasCliFor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblCampoBusca = new System.Windows.Forms.Label();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblRazaoResultadoConsulta = new System.Windows.Forms.Label();
            this.lblRazaoConsulta = new System.Windows.Forms.Label();
            this.lblContaResultadoConsulta = new System.Windows.Forms.Label();
            this.lblContaConsulta = new System.Windows.Forms.Label();
            this.lblCodigoResultadoConsulta = new System.Windows.Forms.Label();
            this.lblCodigoConsulta = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtRazaoSocial = new System.Windows.Forms.TextBox();
            this.lblRazaoSocialCadastro = new System.Windows.Forms.Label();
            this.lblContaCadastro = new System.Windows.Forms.Label();
            this.lblCodigoCadastro = new System.Windows.Forms.Label();
            this.btnCadastrar = new System.Windows.Forms.Button();
            this.btnAtualizar = new System.Windows.Forms.Button();
            this.btnRemover = new System.Windows.Forms.Button();
            this.txtCodigo = new System.Windows.Forms.MaskedTextBox();
            this.txtContaContabil = new System.Windows.Forms.MaskedTextBox();
            this.txtBusca = new System.Windows.Forms.MaskedTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblCampoBusca
            // 
            this.lblCampoBusca.AutoSize = true;
            this.lblCampoBusca.Location = new System.Drawing.Point(15, 31);
            this.lblCampoBusca.Name = "lblCampoBusca";
            this.lblCampoBusca.Size = new System.Drawing.Size(83, 13);
            this.lblCampoBusca.TabIndex = 0;
            this.lblCampoBusca.Text = "Código Cli / For:";
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(26, 74);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(186, 35);
            this.btnBuscar.TabIndex = 2;
            this.btnBuscar.Text = "Buscar";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBusca);
            this.groupBox1.Controls.Add(this.lblRazaoResultadoConsulta);
            this.groupBox1.Controls.Add(this.lblRazaoConsulta);
            this.groupBox1.Controls.Add(this.lblContaResultadoConsulta);
            this.groupBox1.Controls.Add(this.lblContaConsulta);
            this.groupBox1.Controls.Add(this.lblCodigoResultadoConsulta);
            this.groupBox1.Controls.Add(this.lblCodigoConsulta);
            this.groupBox1.Controls.Add(this.btnBuscar);
            this.groupBox1.Controls.Add(this.lblCampoBusca);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(504, 137);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Consulta Contas Contábeis";
            // 
            // lblRazaoResultadoConsulta
            // 
            this.lblRazaoResultadoConsulta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRazaoResultadoConsulta.Location = new System.Drawing.Point(231, 100);
            this.lblRazaoResultadoConsulta.Name = "lblRazaoResultadoConsulta";
            this.lblRazaoResultadoConsulta.Size = new System.Drawing.Size(263, 20);
            this.lblRazaoResultadoConsulta.TabIndex = 10;
            // 
            // lblRazaoConsulta
            // 
            this.lblRazaoConsulta.AutoSize = true;
            this.lblRazaoConsulta.Location = new System.Drawing.Point(228, 85);
            this.lblRazaoConsulta.Name = "lblRazaoConsulta";
            this.lblRazaoConsulta.Size = new System.Drawing.Size(70, 13);
            this.lblRazaoConsulta.TabIndex = 9;
            this.lblRazaoConsulta.Text = "Razão Social";
            // 
            // lblContaResultadoConsulta
            // 
            this.lblContaResultadoConsulta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblContaResultadoConsulta.Location = new System.Drawing.Point(231, 61);
            this.lblContaResultadoConsulta.Name = "lblContaResultadoConsulta";
            this.lblContaResultadoConsulta.Size = new System.Drawing.Size(57, 20);
            this.lblContaResultadoConsulta.TabIndex = 8;
            // 
            // lblContaConsulta
            // 
            this.lblContaConsulta.AutoSize = true;
            this.lblContaConsulta.Location = new System.Drawing.Point(228, 46);
            this.lblContaConsulta.Name = "lblContaConsulta";
            this.lblContaConsulta.Size = new System.Drawing.Size(76, 13);
            this.lblContaConsulta.TabIndex = 7;
            this.lblContaConsulta.Text = "Conta Contábil";
            // 
            // lblCodigoResultadoConsulta
            // 
            this.lblCodigoResultadoConsulta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCodigoResultadoConsulta.Location = new System.Drawing.Point(231, 25);
            this.lblCodigoResultadoConsulta.Name = "lblCodigoResultadoConsulta";
            this.lblCodigoResultadoConsulta.Size = new System.Drawing.Size(57, 20);
            this.lblCodigoResultadoConsulta.TabIndex = 6;
            // 
            // lblCodigoConsulta
            // 
            this.lblCodigoConsulta.AutoSize = true;
            this.lblCodigoConsulta.Location = new System.Drawing.Point(228, 10);
            this.lblCodigoConsulta.Name = "lblCodigoConsulta";
            this.lblCodigoConsulta.Size = new System.Drawing.Size(40, 13);
            this.lblCodigoConsulta.TabIndex = 5;
            this.lblCodigoConsulta.Text = "Código";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtContaContabil);
            this.groupBox2.Controls.Add(this.txtCodigo);
            this.groupBox2.Controls.Add(this.btnRemover);
            this.groupBox2.Controls.Add(this.btnAtualizar);
            this.groupBox2.Controls.Add(this.txtRazaoSocial);
            this.groupBox2.Controls.Add(this.lblRazaoSocialCadastro);
            this.groupBox2.Controls.Add(this.lblContaCadastro);
            this.groupBox2.Controls.Add(this.lblCodigoCadastro);
            this.groupBox2.Controls.Add(this.btnCadastrar);
            this.groupBox2.Location = new System.Drawing.Point(12, 155);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(504, 125);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cadastro de Contas Contábeis";
            // 
            // txtRazaoSocial
            // 
            this.txtRazaoSocial.Location = new System.Drawing.Point(187, 45);
            this.txtRazaoSocial.Name = "txtRazaoSocial";
            this.txtRazaoSocial.Size = new System.Drawing.Size(291, 20);
            this.txtRazaoSocial.TabIndex = 5;
            // 
            // lblRazaoSocialCadastro
            // 
            this.lblRazaoSocialCadastro.AutoSize = true;
            this.lblRazaoSocialCadastro.Location = new System.Drawing.Point(184, 28);
            this.lblRazaoSocialCadastro.Name = "lblRazaoSocialCadastro";
            this.lblRazaoSocialCadastro.Size = new System.Drawing.Size(70, 13);
            this.lblRazaoSocialCadastro.TabIndex = 9;
            this.lblRazaoSocialCadastro.Text = "Razão Social";
            // 
            // lblContaCadastro
            // 
            this.lblContaCadastro.AutoSize = true;
            this.lblContaCadastro.Location = new System.Drawing.Point(89, 28);
            this.lblContaCadastro.Name = "lblContaCadastro";
            this.lblContaCadastro.Size = new System.Drawing.Size(76, 13);
            this.lblContaCadastro.TabIndex = 7;
            this.lblContaCadastro.Text = "Conta Contábil";
            // 
            // lblCodigoCadastro
            // 
            this.lblCodigoCadastro.AutoSize = true;
            this.lblCodigoCadastro.Location = new System.Drawing.Point(15, 28);
            this.lblCodigoCadastro.Name = "lblCodigoCadastro";
            this.lblCodigoCadastro.Size = new System.Drawing.Size(40, 13);
            this.lblCodigoCadastro.TabIndex = 5;
            this.lblCodigoCadastro.Text = "Código";
            // 
            // btnCadastrar
            // 
            this.btnCadastrar.Location = new System.Drawing.Point(162, 80);
            this.btnCadastrar.Name = "btnCadastrar";
            this.btnCadastrar.Size = new System.Drawing.Size(61, 31);
            this.btnCadastrar.TabIndex = 6;
            this.btnCadastrar.Text = "Cadastrar";
            this.btnCadastrar.UseVisualStyleBackColor = true;
            this.btnCadastrar.Click += new System.EventHandler(this.btnCadastrar_Click);
            // 
            // btnAtualizar
            // 
            this.btnAtualizar.Location = new System.Drawing.Point(229, 80);
            this.btnAtualizar.Name = "btnAtualizar";
            this.btnAtualizar.Size = new System.Drawing.Size(59, 31);
            this.btnAtualizar.TabIndex = 7;
            this.btnAtualizar.Text = "Atualizar";
            this.btnAtualizar.UseVisualStyleBackColor = true;
            this.btnAtualizar.Click += new System.EventHandler(this.btnAtualizar_Click);
            // 
            // btnRemover
            // 
            this.btnRemover.Location = new System.Drawing.Point(294, 80);
            this.btnRemover.Name = "btnRemover";
            this.btnRemover.Size = new System.Drawing.Size(59, 31);
            this.btnRemover.TabIndex = 8;
            this.btnRemover.Text = "Remover";
            this.btnRemover.UseVisualStyleBackColor = true;
            this.btnRemover.Click += new System.EventHandler(this.btnRemover_Click);
            // 
            // txtCodigo
            // 
            this.txtCodigo.Location = new System.Drawing.Point(18, 45);
            this.txtCodigo.Mask = "L00000";
            this.txtCodigo.Name = "txtCodigo";
            this.txtCodigo.Size = new System.Drawing.Size(52, 20);
            this.txtCodigo.TabIndex = 3;
            this.txtCodigo.ValidatingType = typeof(int);
            // 
            // txtContaContabil
            // 
            this.txtContaContabil.Location = new System.Drawing.Point(92, 45);
            this.txtContaContabil.Mask = "00000000";
            this.txtContaContabil.Name = "txtContaContabil";
            this.txtContaContabil.Size = new System.Drawing.Size(73, 20);
            this.txtContaContabil.TabIndex = 4;
            // 
            // txtBusca
            // 
            this.txtBusca.Location = new System.Drawing.Point(112, 28);
            this.txtBusca.Mask = "L00000";
            this.txtBusca.Name = "txtBusca";
            this.txtBusca.Size = new System.Drawing.Size(100, 20);
            this.txtBusca.TabIndex = 1;
            // 
            // FrmConfigContasCliFor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(527, 292);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmConfigContasCliFor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuração de Contas Contábeis de Cli / For";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblCampoBusca;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblRazaoResultadoConsulta;
        private System.Windows.Forms.Label lblRazaoConsulta;
        private System.Windows.Forms.Label lblContaResultadoConsulta;
        private System.Windows.Forms.Label lblContaConsulta;
        private System.Windows.Forms.Label lblCodigoResultadoConsulta;
        private System.Windows.Forms.Label lblCodigoConsulta;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtRazaoSocial;
        private System.Windows.Forms.Label lblRazaoSocialCadastro;
        private System.Windows.Forms.Label lblContaCadastro;
        private System.Windows.Forms.Label lblCodigoCadastro;
        private System.Windows.Forms.Button btnCadastrar;
        private System.Windows.Forms.Button btnRemover;
        private System.Windows.Forms.Button btnAtualizar;
        private System.Windows.Forms.MaskedTextBox txtCodigo;
        private System.Windows.Forms.MaskedTextBox txtContaContabil;
        private System.Windows.Forms.MaskedTextBox txtBusca;
    }
}