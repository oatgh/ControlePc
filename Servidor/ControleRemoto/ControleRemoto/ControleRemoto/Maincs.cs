using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using ZXing;
using ZXing.Common;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.IO;
using Microsoft.Win32;

namespace ControleRemoto
{
    public partial class Maincs : Form
    {
        String diretory = Directory.GetCurrentDirectory();
        public Maincs()
        {
            InitializeComponent();
        }


        private void Maincs_Shown(object sender, EventArgs e)
        {
            refreshQR();
            verificaInici();
            notifyIcon1.Visible = false;
        }

        private void verificaInici()
        {
            if (Properties.Settings.Default.starServ == CheckState.Checked)
            {
                startServer();
            }
        }
        public void refreshQR()
        {
            Dictionary<String, dynamic> decodedSend = new Dictionary<string, dynamic>();

            decodedSend.Add("pass", Properties.Settings.Default.senha);
            decodedSend.Add("host", Properties.Settings.Default.hostIP);
            decodedSend.Add("port", Properties.Settings.Default.port);

            String encodedSend = JsonConvert.SerializeObject(decodedSend);

            try
            {
                lblHost.Text = Properties.Settings.Default.hostIP;
                lblPort.Text = Properties.Settings.Default.port.ToString();
                lblPass.Text = Properties.Settings.Default.senha;
                var qrCodeGen = new BarcodeWriter();
                var enOpts = new EncodingOptions()
                {
                    Width = 200,
                    Height = 200,
                    Margin = 0
                };

                qrCodeGen.Options = enOpts;
                qrCodeGen.Format = BarcodeFormat.QR_CODE;
                qrImage.Image = new Bitmap(qrCodeGen.Write(encodedSend));


            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void Maincs_Load(object sender, EventArgs e)
        {

        }

        private void Maincs_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            StatCfg frmConfigLoad = new StatCfg(1);
            frmConfigLoad.Show();
            this.Dispose();

        }
        bool isOn = false;
        private void button2_Click(object sender, EventArgs e)
        {

            //new Thread(() => startServer()).Start();
            startServer();
        }
        static TcpListener socket = new TcpListener(IPAddress.Parse(Properties.Settings.Default.hostIP), Properties.Settings.Default.port);
        private void startServer()
        {
            if (!isOn)
            {
                try
                {

                    socket.Start();
                    isOn = true;
                    txtStatus.Text = "Servidor inciado";
                    btnStart.Text = "Desligar";
                    new Thread(() => {
                        if (isOn)
                        {
                            do
                            {
                                try
                                {
                                    aceptClient();
                                }
                                catch { }
                            } while (!aceptClient());
                        }
                    }).Start();
                }
                catch (Exception e) { txtStatus.Text = e.Message; }
            }
            else
            {
                socket.Stop();
                isOn = false;
                txtStatus.Text = "";
                btnStart.Text = "Ligar";
            }
        }

        static bool aceptClient()
        {
            

            
            TcpClient client = socket.AcceptTcpClient();

            try
            {

                NetworkStream network = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesReceived = network.Read(buffer, 0, buffer.Length);
                string Decoded = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(Decoded);
                try
                {
                    var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(Decoded);
                    try
                    {
                        if (json != null && json["msg"].Equals("message"))
                        {
                            MessageBox.Show(json["args"], "Mensagem do cliente");
                            Console.WriteLine(json["args"]);
                        }
                        else if (json != null && json["msg"].Equals("shutdown"))
                        {
                            Process.Start(json["msg"].Trim(), json["args"]);
                        }
                    }
                    catch
                    {
                        //socket.Stop();
                        return false;
                    }

                }
                catch
                {
                    //socket.Stop();
                    return false;
                }
            }
            catch
            {
                //socket.Stop();
                return false;
            }
            //socket.Stop();
            return false;
        }




        /*
        private static TcpListener Server = new TcpListener(IPAddress.Parse(Properties.Settings.Default.hostIP)
            , Properties.Settings.Default.port);


        private void startServer()
        {
            if (!isOn)
            {
                try
                {

                    Server.Start();
                    isOn = true;
                    txtStatus.Text = "Servidor inciado";
                    btnStart.Text = "Desligar";
                    new Thread(() => AcceptClient()).Start();
                }
                catch (Exception e) { txtStatus.Text = e.Message; }
            }
            else 
            {
                Server.Stop();
                isOn = false;
                txtStatus.Text = "";
                btnStart.Text = "Ligar";
            }
        }

        async private static void AcceptClient()
        {
            do
            {
                try
                {
                    TcpClient client = await Server.AcceptTcpClientAsync();

                    new Thread(() => ReceivBuffer(client)).Start();
                }
                catch
                {
                    
                    break;
                }
                

            }while(true);
        }

        async static void ReceivBuffer(TcpClient client)
        {
            

            NetworkStream clientStream = client.GetStream();

            byte[] buffer = new byte[1024];
            int bytesReceived = await clientStream.ReadAsync(buffer, 0, buffer.Length);

            byte[] novoBuffer = new byte[bytesReceived];
            Buffer.BlockCopy(buffer, 0, novoBuffer, 0, bytesReceived);
            do
            {
                
                if (bytesReceived > 0) 
                {
                    string Decoded = Encoding.UTF8.GetString(buffer);
                    Dictionary<string, string> dict;
                    try
                    {
                        dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(Decoded);
                        if (Properties.Settings.Default.forcePass == CheckState.Checked)
                        {
                            if (dict["pass"].Equals(Properties.Settings.Default.senha))
                            {

                                String Command = dict["msg"].Substring(0, dict["msg"].IndexOf(' '));
                                String Argment = dict["msg"].Substring(dict["msg"].IndexOf(' '),
                                    dict["msg"].Length - dict["msg"].IndexOf(' '));

                                Process.Start(Command, Argment.Trim());
                            }
                            else
                            {
                                MessageBox.Show("Senha incorreta", "Erro");
                            }
                        }
                        else if (Properties.Settings.Default.forcePass == CheckState.Unchecked)
                        {
                            try
                            {
                                String Command = dict["msg"].Substring(0, dict["msg"].IndexOf(' '));
                                String Argment = dict["msg"].Substring(dict["msg"].IndexOf(' '),
                                    dict["msg"].Length - dict["msg"].IndexOf(' '));

                                Process.Start(Command, Argment.Trim());
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show(e.Message);
                            }

                            MessageBox.Show(dict["msg"], "Mensagem enviada!");

                        }
                    }
                    catch(Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                    

                    
                    

                   
                }
            } while (true);
        }
        */
        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.Hide();
            notifyIcon1.Visible = true;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            notifyIcon1.Visible = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(Properties.Settings.Default.startSys == CheckState.Checked)
            {
                Properties.Settings.Default.startSys = CheckState.Unchecked;
                Properties.Settings.Default.Save();
                try
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    key.DeleteValue("ControleRemotoServidor", false);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else if(Properties.Settings.Default.startSys == CheckState.Unchecked)
            {
                Properties.Settings.Default.startSys = CheckState.Checked;
                Properties.Settings.Default.Save();
                try
                {
                    RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    key.SetValue("ControleRemotoServidor", Application.ExecutablePath.ToString());

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.starServ == CheckState.Checked)
            {
                Properties.Settings.Default.starServ = CheckState.Unchecked;
                Properties.Settings.Default.Save();
            }
            else if(Properties.Settings.Default.starServ == CheckState.Unchecked)
            {
                Properties.Settings.Default.starServ = CheckState.Checked;
                Properties.Settings.Default.Save();
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.forcePass == CheckState.Checked)
            {
                Properties.Settings.Default.forcePass = CheckState.Unchecked;
                Properties.Settings.Default.Save();
            }
            else if (Properties.Settings.Default.forcePass == CheckState.Unchecked)
            {
                Properties.Settings.Default.forcePass = CheckState.Checked;
                Properties.Settings.Default.Save();
            }
        }

        private void process1_Exited(object sender, EventArgs e)
        {

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            
        }
        /*
private void DefinirTexto(string texto)
{
this.txtStatus.Text = texto;
}
delegate void SetTextCallback(string texto);

private void server()
{
TcpListener server = new TcpListener(IPAddress.Parse(Properties.Settings.Default.hostIP),
Properties.Settings.Default.port);
try
{
server.Start();
txtStsInvoke("Escutando com o ip " + Properties.Settings.Default.hostIP
+ " Na porta " + Properties.Settings.Default.port.ToString()
+ "\n Esperando conexão de clientes");

TcpClient cliente = server.AcceptTcpClient();
txtStsInvoke("Cliente conectado" + cliente.Connected);

BinaryReader leitor = new BinaryReader(cliente.GetStream());
txtStsInvoke("Esperando mensagem do cliente");

}
catch (Exception e)
{
MessageBox.Show(e.Message);
}


}
private void txtStsInvoke(String texto)
{

if (this.txtStatus.InvokeRequired)
{
SetTextCallback d = new SetTextCallback(DefinirTexto);
this.Invoke(d, new object[] { texto });

}
}
*/
    }
}
