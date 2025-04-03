using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace RouletteApp
{
    public class TCPListenerTCPClientController
    {
        private TCPClient client = new TCPClient();
        private TCPListener listener = new TCPListener();

        public JObject jsonToPassDown = new JObject();

        public bool passDownRequest = false;
        public void RunServerAndClient()
        {
            string jsonRequest =
            """
            {
            "activePlayer":33,
            "biggestMultiplier":666
            }
            """;

            client.Connect("192.168.1.100", jsonRequest);
            listener.StartListener();
        }

        public TCPListenerTCPClientController()
        {
            RunServerAndClient();
        }
    }
}
