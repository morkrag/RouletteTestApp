using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Nodes;
using System.Text.Json;
using TCPListener;
using System.Runtime.InteropServices.JavaScript;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RouletteApp
{
    public class TCPListener
    {
        public bool requestToProceed = false;
        public JObject jsonToPassDown = new JObject();
        public void StartListener()
        {
            Console.WriteLine("Starting TCP listener and TCP Client...");
            TcpListener server = null;
            try
            {
                Int32 port = 4948;
                IPAddress localAddr = IPAddress.Parse("192.168.1.100");

                server = new TcpListener(localAddr, port);

                server.Start();

                Byte[] bytes = new Byte[256];
                string data = null;

                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    using TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    data = null;

                    NetworkStream stream = client.GetStream();

                    int i;

                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        ConvertMessageToJOBject(data);
                    }
                    
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        private void ConvertMessageToJOBject(string message)
        {
            JObject messageContainerObject = JObject.Parse(message);
            if (CheckIfProccessableParametersAreSent(messageContainerObject))
            {
                requestToProceed = true; // I know that its not the way, but can't invent anything better
                jsonToPassDown = messageContainerObject;
            } else
            {
                throw new Exception("Incorrect parameters provided");
            }
        }

        private static bool CheckIfProccessableParametersAreSent(JObject message)
        {
            var count = message.Count;
            if (count > 0 && count < 3)
            {
                foreach (var key in message.Properties())
                {
                    if (key.Name != "activePlayers" || key.Name != "biggestMultiplier")
                    {
                        Console.WriteLine($"{key}");
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}
