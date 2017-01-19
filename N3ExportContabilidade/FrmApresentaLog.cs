using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace N3ExportContabilidade
{
    public partial class FrmApresentaLog: Form
    {
        public FrmApresentaLog()
        {
            InitializeComponent();
        }

        public void setTextoLog(string conteudoLog) {
            if (conteudoLog != null) {
                txtLog.Text = conteudoLog;
            }
        }
    }
}