using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Resources;
using System.Collections;
using System.Data.Odbc;

namespace N3ExportContabilidade
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
    
        [STAThread]
        public static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmPrincipalExportacao());
        }


    }
}