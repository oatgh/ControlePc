using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Testando_recebimento_tcp
{
    class Program
    {

        
        static bool ligado = false;
        static void Main(string[] args)
        {

            do
            {
                ligandoServer();
            } while (!ligandoServer());
            
        }


        static bool ligandoServer()
        {
            TcpListener socket;

            socket = new TcpListener(IPAddress.Any, 2000);


            try
            {
                socket.Start();
            }
            catch
            {
                return false;
            }
            Console.WriteLine("Servidor escutando");
            TcpClient client = socket.AcceptTcpClient();
            Console.WriteLine("Cliente conectado " + client.Client);

            try{

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
                            Console.WriteLine(json["msg"] + json["args"]);
                        }else if (json != null && json["msg"].Equals("shutdown"))
                        {
                            Process.Start(json["msg"].Trim(), json["args"]);
                        }
                    }
                    catch
                    {
                        socket.Stop();
                        return false;
                    }
                    
                }
                catch
                {
                    socket.Stop();
                    return false;
                }
            }
            catch
            {
                socket.Stop();
                return false;
            }
            socket.Stop();
            return false;
           
        }

        static void aceitaCliente()
        {
            
        }

        static void networkStream(TcpClient tcpClient)
        {
            try
            {
                NetworkStream network = tcpClient.GetStream();
                byte[] buffer = new byte[1024];
                int bytesReceived = network.Read(buffer, 0, buffer.Length);
                string Decoded = Encoding.UTF8.GetString(buffer);
                Console.WriteLine(Decoded);

                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(Decoded);

                Console.WriteLine(json["msg"] + json["args"]);
            }
            catch { }
        }
    }
}
