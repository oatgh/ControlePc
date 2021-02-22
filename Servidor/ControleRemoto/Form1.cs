using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControleRemoto
{
    public partial class frmLoad : Form
    {
        public frmLoad()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            
        }

        public bool VerificaProgramaEmExecucao()
        {
            progressBar1.Value = 30;
            return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1;
        }


        private void frmLoad_Shown(object sender, EventArgs e)
        {

            

            progressBar1.Value = 10;
            if (VerificaProgramaEmExecucao()){
                MessageBox.Show("Programa ja em execucao, verifique a bandeja de sistema", 
                    "Erro");
                Application.Exit();
            }
            progressBar1.Value = 80;
           if (Properties.Settings.Default.hostIP.Equals("Undefined") || 
                Properties.Settings.Default.senha.Equals("Undefined"))
            {
                progressBar1.Value = 100;
                StatCfg frmConfigLoad = new StatCfg(0);
                frmConfigLoad.Show();
                this.Visible = false;
            }
            else
            {
                Maincs frmMainLoad = new Maincs();
                frmMainLoad.Show();
                this.Visible = false;
            }
        }

        private void frmLoad_Load(object sender, EventArgs e)
        {

        }
    }
}
