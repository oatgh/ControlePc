using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControleRemoto
{
    public partial class StatCfg : Form
    {
        int whatsCalling = 0;
        public StatCfg(int whtCall)
        {
            InitializeComponent();
            whatsCalling = whtCall;
        }
        
        private void StatCfg_Load(object sender, EventArgs e)
        {

            atualizarComboBox();
    
        }


        private void atualizarComboBox()
        {
            IPAddress[] ipList = Dns.GetHostAddresses(Dns.GetHostName());
            
            for(int i = 0; i < ipList.Length; i++)
            {

                if(ipList[i].ToString().StartsWith("10.") ||
                    ipList[i].ToString().StartsWith("192."))
                {
                    ipCmBx.Items.Add(ipList[i].ToString());
                    ipCmBx.SelectedIndex = 0;
                }

            }
        }
        
        private void salvarPrefs()
        {
            Properties.Settings.Default.hostIP = ipCmBx.SelectedItem.ToString();
            Properties.Settings.Default.port = Convert.ToInt32(txtPort.Text);
            Properties.Settings.Default.senha = txtPass.Text;
            Properties.Settings.Default.Save();
            Maincs frmMainLoad = new Maincs();
            frmMainLoad.Show();
            this.Visible = false;
            this.Dispose();
        }

        private void StatCfg_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            String adcIP = Interaction.InputBox("Digite o IP", "Adcionar manualmente");
            if (!(adcIP.Length == 0))
            {
                ipCmBx.Items.Add(adcIP);
                ipCmBx.SelectedIndex = ipCmBx.Items.IndexOf(adcIP);
            }
            else
            {
                MessageBox.Show("Voce não digitou nada!");
            }
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            Maincs frmMain = new Maincs();
            frmMain.refreshQR();
            salvarPrefs();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (whatsCalling)
            {
                case 0:
                    Application.Exit();
                    break;
                case 1:
                    Maincs frmMain = new Maincs();
                    frmMain.Show();
                    this.Visible = false;
                    this.Dispose();
                    break;
            }
           
            
        }

        private void ipCmBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.hostIP = ipCmBx.SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }
    }


}
