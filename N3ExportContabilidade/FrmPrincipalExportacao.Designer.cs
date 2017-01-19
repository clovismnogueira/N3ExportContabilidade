namespace N3ExportContabilidade
{
    partial class FrmPrincipalExportacao
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPrincipalExportacao));
            this.grpOpcoes = new System.Windows.Forms.GroupBox();
            this.chkBoxArquivosServicoImportacao = new System.Windows.Forms.CheckBox();
            this.chkBoxArquivoTrasferenciasEnviadas = new System.Windows.Forms.CheckBox();
            this.chkBoxArquivoComprasUsoConsumo = new System.Windows.Forms.CheckBox();
            this.chkBoxArquivoTrasferenciasRecebidas = new System.Windows.Forms.CheckBox();
            this.chkBoxArquivoFretesCompras = new System.Windows.Forms.CheckBox();
            this.chkBoxArquivoFretesVendas = new System.Windows.Forms.CheckBox();
            this.chkBoxArquivosDevolucao = new System.Windows.Forms.CheckBox();
            this.chkBoxArquivosImportacao = new System.Windows.Forms.CheckBox();
            this.chkBoxArquivosCompras = new System.Windows.Forms.CheckBox();
            this.chkBoxArquivosVendas = new System.Windows.Forms.CheckBox();
            this.txtDiretorio = new System.Windows.Forms.TextBox();
            this.btnDiretorio = new System.Windows.Forms.Button();
            this.lblDataFinal = new System.Windows.Forms.Label();
            this.lblDataInicial = new System.Windows.Forms.Label();
            this.datePickerFinal = new System.Windows.Forms.DateTimePicker();
            this.datePickerInicial = new System.Windows.Forms.DateTimePicker();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnGerarArquivos = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.grpConfiguracoes = new System.Windows.Forms.GroupBox();
            this.chkBoxFortaleza = new System.Windows.Forms.CheckBox();
            this.chkBoxFilialRecife = new System.Windows.Forms.CheckBox();
            this.chkBoxFilialMatriz = new System.Windows.Forms.CheckBox();
            this.grpOpcoes.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.grpConfiguracoes.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpOpcoes
            // 
            this.grpOpcoes.Controls.Add(this.chkBoxArquivosServicoImportacao);
            this.grpOpcoes.Controls.Add(this.chkBoxArquivoTrasferenciasEnviadas);
            this.grpOpcoes.Controls.Add(this.chkBoxArquivoComprasUsoConsumo);
            this.grpOpcoes.Controls.Add(this.chkBoxArquivoTrasferenciasRecebidas);
            this.grpOpcoes.Controls.Add(this.chkBoxArquivoFretesCompras);
            this.grpOpcoes.Controls.Add(this.chkBoxArquivoFretesVendas);
            this.grpOpcoes.Controls.Add(this.chkBoxArquivosDevolucao);
            this.grpOpcoes.Controls.Add(this.chkBoxArquivosImportacao);
            this.grpOpcoes.Controls.Add(this.chkBoxArquivosCompras);
            this.grpOpcoes.Controls.Add(this.chkBoxArquivosVendas);
            this.grpOpcoes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpOpcoes.Location = new System.Drawing.Point(12, 54);
            this.grpOpcoes.Name = "grpOpcoes";
            this.grpOpcoes.Size = new System.Drawing.Size(355, 297);
            this.grpOpcoes.TabIndex = 0;
            this.grpOpcoes.TabStop = false;
            this.grpOpcoes.Text = "Operações para Exportação";
            // 
            // chkBoxArquivosServicoImportacao
            // 
            this.chkBoxArquivosServicoImportacao.AutoSize = true;
            this.chkBoxArquivosServicoImportacao.Location = new System.Drawing.Point(25, 89);
            this.chkBoxArquivosServicoImportacao.Name = "chkBoxArquivosServicoImportacao";
            this.chkBoxArquivosServicoImportacao.Size = new System.Drawing.Size(249, 17);
            this.chkBoxArquivosServicoImportacao.TabIndex = 17;
            this.chkBoxArquivosServicoImportacao.Text = "Entradas de Serviço Relac. Importação";
            this.chkBoxArquivosServicoImportacao.UseVisualStyleBackColor = true;
            // 
            // chkBoxArquivoTrasferenciasEnviadas
            // 
            this.chkBoxArquivoTrasferenciasEnviadas.AutoSize = true;
            this.chkBoxArquivoTrasferenciasEnviadas.Location = new System.Drawing.Point(25, 204);
            this.chkBoxArquivoTrasferenciasEnviadas.Name = "chkBoxArquivoTrasferenciasEnviadas";
            this.chkBoxArquivoTrasferenciasEnviadas.Size = new System.Drawing.Size(214, 17);
            this.chkBoxArquivoTrasferenciasEnviadas.TabIndex = 15;
            this.chkBoxArquivoTrasferenciasEnviadas.Text = "Transferências de Filial Enviadas";
            this.chkBoxArquivoTrasferenciasEnviadas.UseVisualStyleBackColor = true;
            // 
            // chkBoxArquivoComprasUsoConsumo
            // 
            this.chkBoxArquivoComprasUsoConsumo.AutoSize = true;
            this.chkBoxArquivoComprasUsoConsumo.Enabled = false;
            this.chkBoxArquivoComprasUsoConsumo.Location = new System.Drawing.Point(25, 227);
            this.chkBoxArquivoComprasUsoConsumo.Name = "chkBoxArquivoComprasUsoConsumo";
            this.chkBoxArquivoComprasUsoConsumo.Size = new System.Drawing.Size(327, 17);
            this.chkBoxArquivoComprasUsoConsumo.TabIndex = 14;
            this.chkBoxArquivoComprasUsoConsumo.Text = "(Não Usar) Compras de Material para Uso e Consumo";
            this.chkBoxArquivoComprasUsoConsumo.UseVisualStyleBackColor = true;
            // 
            // chkBoxArquivoTrasferenciasRecebidas
            // 
            this.chkBoxArquivoTrasferenciasRecebidas.AutoSize = true;
            this.chkBoxArquivoTrasferenciasRecebidas.Location = new System.Drawing.Point(25, 181);
            this.chkBoxArquivoTrasferenciasRecebidas.Name = "chkBoxArquivoTrasferenciasRecebidas";
            this.chkBoxArquivoTrasferenciasRecebidas.Size = new System.Drawing.Size(222, 17);
            this.chkBoxArquivoTrasferenciasRecebidas.TabIndex = 12;
            this.chkBoxArquivoTrasferenciasRecebidas.Text = "Transferências de Filial Recebidas";
            this.chkBoxArquivoTrasferenciasRecebidas.UseVisualStyleBackColor = true;
            // 
            // chkBoxArquivoFretesCompras
            // 
            this.chkBoxArquivoFretesCompras.AutoSize = true;
            this.chkBoxArquivoFretesCompras.Location = new System.Drawing.Point(26, 158);
            this.chkBoxArquivoFretesCompras.Name = "chkBoxArquivoFretesCompras";
            this.chkBoxArquivoFretesCompras.Size = new System.Drawing.Size(314, 17);
            this.chkBoxArquivoFretesCompras.TabIndex = 11;
            this.chkBoxArquivoFretesCompras.Text = "Entradas de Frete Compras e Remoção Importação";
            this.chkBoxArquivoFretesCompras.UseVisualStyleBackColor = true;
            // 
            // chkBoxArquivoFretesVendas
            // 
            this.chkBoxArquivoFretesVendas.AutoSize = true;
            this.chkBoxArquivoFretesVendas.Location = new System.Drawing.Point(26, 135);
            this.chkBoxArquivoFretesVendas.Name = "chkBoxArquivoFretesVendas";
            this.chkBoxArquivoFretesVendas.Size = new System.Drawing.Size(191, 17);
            this.chkBoxArquivoFretesVendas.TabIndex = 10;
            this.chkBoxArquivoFretesVendas.Text = "Entradas de Frete de Vendas";
            this.chkBoxArquivoFretesVendas.UseVisualStyleBackColor = true;
            // 
            // chkBoxArquivosDevolucao
            // 
            this.chkBoxArquivosDevolucao.AutoSize = true;
            this.chkBoxArquivosDevolucao.Location = new System.Drawing.Point(26, 112);
            this.chkBoxArquivosDevolucao.Name = "chkBoxArquivosDevolucao";
            this.chkBoxArquivosDevolucao.Size = new System.Drawing.Size(205, 17);
            this.chkBoxArquivosDevolucao.TabIndex = 9;
            this.chkBoxArquivosDevolucao.Text = "Entradas de Devolução Vendas";
            this.chkBoxArquivosDevolucao.UseVisualStyleBackColor = true;
            // 
            // chkBoxArquivosImportacao
            // 
            this.chkBoxArquivosImportacao.AutoSize = true;
            this.chkBoxArquivosImportacao.Location = new System.Drawing.Point(25, 67);
            this.chkBoxArquivosImportacao.Name = "chkBoxArquivosImportacao";
            this.chkBoxArquivosImportacao.Size = new System.Drawing.Size(199, 17);
            this.chkBoxArquivosImportacao.TabIndex = 8;
            this.chkBoxArquivosImportacao.Text = "Entradas de Importação Direta";
            this.chkBoxArquivosImportacao.UseVisualStyleBackColor = true;
            // 
            // chkBoxArquivosCompras
            // 
            this.chkBoxArquivosCompras.AutoSize = true;
            this.chkBoxArquivosCompras.Location = new System.Drawing.Point(25, 44);
            this.chkBoxArquivosCompras.Name = "chkBoxArquivosCompras";
            this.chkBoxArquivosCompras.Size = new System.Drawing.Size(286, 17);
            this.chkBoxArquivosCompras.TabIndex = 1;
            this.chkBoxArquivosCompras.Text = "Compras para Comerc. / Indust. / Bonificadas";
            this.chkBoxArquivosCompras.UseVisualStyleBackColor = true;
            // 
            // chkBoxArquivosVendas
            // 
            this.chkBoxArquivosVendas.AutoSize = true;
            this.chkBoxArquivosVendas.Location = new System.Drawing.Point(25, 22);
            this.chkBoxArquivosVendas.Name = "chkBoxArquivosVendas";
            this.chkBoxArquivosVendas.Size = new System.Drawing.Size(329, 17);
            this.chkBoxArquivosVendas.TabIndex = 0;
            this.chkBoxArquivosVendas.Text = "Vendas de Mercadoria e Produto (Normal ou MPBEM)";
            this.chkBoxArquivosVendas.UseVisualStyleBackColor = true;
            // 
            // txtDiretorio
            // 
            this.txtDiretorio.Location = new System.Drawing.Point(111, 192);
            this.txtDiretorio.Name = "txtDiretorio";
            this.txtDiretorio.ReadOnly = true;
            this.txtDiretorio.Size = new System.Drawing.Size(218, 20);
            this.txtDiretorio.TabIndex = 7;
            // 
            // btnDiretorio
            // 
            this.btnDiretorio.Location = new System.Drawing.Point(25, 190);
            this.btnDiretorio.Name = "btnDiretorio";
            this.btnDiretorio.Size = new System.Drawing.Size(75, 23);
            this.btnDiretorio.TabIndex = 6;
            this.btnDiretorio.Text = "Diretório";
            this.btnDiretorio.UseVisualStyleBackColor = true;
            this.btnDiretorio.Click += new System.EventHandler(this.btnDiretorio_Click);
            // 
            // lblDataFinal
            // 
            this.lblDataFinal.AutoSize = true;
            this.lblDataFinal.Location = new System.Drawing.Point(22, 154);
            this.lblDataFinal.Name = "lblDataFinal";
            this.lblDataFinal.Size = new System.Drawing.Size(65, 13);
            this.lblDataFinal.TabIndex = 5;
            this.lblDataFinal.Text = "Data Final";
            // 
            // lblDataInicial
            // 
            this.lblDataInicial.AutoSize = true;
            this.lblDataInicial.Location = new System.Drawing.Point(22, 127);
            this.lblDataInicial.Name = "lblDataInicial";
            this.lblDataInicial.Size = new System.Drawing.Size(72, 13);
            this.lblDataInicial.TabIndex = 4;
            this.lblDataInicial.Text = "Data Inicial";
            // 
            // datePickerFinal
            // 
            this.datePickerFinal.CustomFormat = "dd/MM/yyyy";
            this.datePickerFinal.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datePickerFinal.ImeMode = System.Windows.Forms.ImeMode.On;
            this.datePickerFinal.Location = new System.Drawing.Point(110, 150);
            this.datePickerFinal.MinDate = new System.DateTime(2005, 1, 1, 0, 0, 0, 0);
            this.datePickerFinal.Name = "datePickerFinal";
            this.datePickerFinal.Size = new System.Drawing.Size(100, 20);
            this.datePickerFinal.TabIndex = 3;
            this.datePickerFinal.Value = new System.DateTime(2014, 10, 10, 0, 0, 0, 0);
            // 
            // datePickerInicial
            // 
            this.datePickerInicial.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.datePickerInicial.CustomFormat = "dd/MM/yyyy";
            this.datePickerInicial.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datePickerInicial.Location = new System.Drawing.Point(111, 123);
            this.datePickerInicial.Name = "datePickerInicial";
            this.datePickerInicial.Size = new System.Drawing.Size(99, 20);
            this.datePickerInicial.TabIndex = 2;
            this.datePickerInicial.Value = new System.DateTime(2014, 10, 10, 0, 0, 0, 0);
            // 
            // btnGerarArquivos
            // 
            this.btnGerarArquivos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGerarArquivos.Location = new System.Drawing.Point(317, 377);
            this.btnGerarArquivos.Name = "btnGerarArquivos";
            this.btnGerarArquivos.Size = new System.Drawing.Size(150, 23);
            this.btnGerarArquivos.TabIndex = 1;
            this.btnGerarArquivos.Text = "Gerar Arquivos";
            this.btnGerarArquivos.UseVisualStyleBackColor = true;
            this.btnGerarArquivos.Click += new System.EventHandler(this.btnGerarArquivos_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(801, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(161, 22);
            this.toolStripButton1.Text = "Config. de Contas Cli/For";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // grpConfiguracoes
            // 
            this.grpConfiguracoes.Controls.Add(this.chkBoxFortaleza);
            this.grpConfiguracoes.Controls.Add(this.chkBoxFilialRecife);
            this.grpConfiguracoes.Controls.Add(this.chkBoxFilialMatriz);
            this.grpConfiguracoes.Controls.Add(this.lblDataInicial);
            this.grpConfiguracoes.Controls.Add(this.datePickerInicial);
            this.grpConfiguracoes.Controls.Add(this.lblDataFinal);
            this.grpConfiguracoes.Controls.Add(this.datePickerFinal);
            this.grpConfiguracoes.Controls.Add(this.txtDiretorio);
            this.grpConfiguracoes.Controls.Add(this.btnDiretorio);
            this.grpConfiguracoes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpConfiguracoes.Location = new System.Drawing.Point(373, 54);
            this.grpConfiguracoes.Name = "grpConfiguracoes";
            this.grpConfiguracoes.Size = new System.Drawing.Size(360, 297);
            this.grpConfiguracoes.TabIndex = 15;
            this.grpConfiguracoes.TabStop = false;
            this.grpConfiguracoes.Text = "Configuração da Exportação";
            // 
            // chkBoxFortaleza
            // 
            this.chkBoxFortaleza.AutoSize = true;
            this.chkBoxFortaleza.Checked = true;
            this.chkBoxFortaleza.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxFortaleza.Location = new System.Drawing.Point(25, 67);
            this.chkBoxFortaleza.Name = "chkBoxFortaleza";
            this.chkBoxFortaleza.Size = new System.Drawing.Size(189, 17);
            this.chkBoxFortaleza.TabIndex = 8;
            this.chkBoxFortaleza.Text = "Filial 3: Filial - Fortaleza - CE";
            this.chkBoxFortaleza.UseVisualStyleBackColor = true;
            // 
            // chkBoxFilialRecife
            // 
            this.chkBoxFilialRecife.AutoSize = true;
            this.chkBoxFilialRecife.Checked = true;
            this.chkBoxFilialRecife.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxFilialRecife.Location = new System.Drawing.Point(25, 44);
            this.chkBoxFilialRecife.Name = "chkBoxFilialRecife";
            this.chkBoxFilialRecife.Size = new System.Drawing.Size(174, 17);
            this.chkBoxFilialRecife.TabIndex = 1;
            this.chkBoxFilialRecife.Text = "Filial 2: Filial - Recife - PB";
            this.chkBoxFilialRecife.UseVisualStyleBackColor = true;
            // 
            // chkBoxFilialMatriz
            // 
            this.chkBoxFilialMatriz.AutoSize = true;
            this.chkBoxFilialMatriz.Checked = true;
            this.chkBoxFilialMatriz.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBoxFilialMatriz.Location = new System.Drawing.Point(25, 22);
            this.chkBoxFilialMatriz.Name = "chkBoxFilialMatriz";
            this.chkBoxFilialMatriz.Size = new System.Drawing.Size(238, 17);
            this.chkBoxFilialMatriz.TabIndex = 0;
            this.chkBoxFilialMatriz.Text = "Filial 1: Matriz - Campina Grande - PB";
            this.chkBoxFilialMatriz.UseVisualStyleBackColor = true;
            // 
            // FrmPrincipalExportacao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 417);
            this.Controls.Add(this.grpConfiguracoes);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.btnGerarArquivos);
            this.Controls.Add(this.grpOpcoes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Name = "FrmPrincipalExportacao";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "8.0 - 10/10/2014 -N3 Exportação Contabilidade TECJUR";
            this.grpOpcoes.ResumeLayout(false);
            this.grpOpcoes.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.grpConfiguracoes.ResumeLayout(false);
            this.grpConfiguracoes.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpOpcoes;
        private System.Windows.Forms.DateTimePicker datePickerFinal;
        private System.Windows.Forms.DateTimePicker datePickerInicial;
        private System.Windows.Forms.CheckBox chkBoxArquivosCompras;
        private System.Windows.Forms.CheckBox chkBoxArquivosVendas;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Label lblDataFinal;
        private System.Windows.Forms.Label lblDataInicial;
        private System.Windows.Forms.TextBox txtDiretorio;
        private System.Windows.Forms.Button btnDiretorio;
        private System.Windows.Forms.Button btnGerarArquivos;
        private System.Windows.Forms.CheckBox chkBoxArquivosDevolucao;
        private System.Windows.Forms.CheckBox chkBoxArquivosImportacao;
        private System.Windows.Forms.CheckBox chkBoxArquivoFretesVendas;
        private System.Windows.Forms.CheckBox chkBoxArquivoFretesCompras;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.CheckBox chkBoxArquivoComprasUsoConsumo;
        private System.Windows.Forms.CheckBox chkBoxArquivoTrasferenciasRecebidas;
        private System.Windows.Forms.GroupBox grpConfiguracoes;
        private System.Windows.Forms.CheckBox chkBoxFortaleza;
        private System.Windows.Forms.CheckBox chkBoxFilialRecife;
        private System.Windows.Forms.CheckBox chkBoxFilialMatriz;
        private System.Windows.Forms.CheckBox chkBoxArquivoTrasferenciasEnviadas;
        private System.Windows.Forms.CheckBox chkBoxArquivosServicoImportacao;
    }
}

