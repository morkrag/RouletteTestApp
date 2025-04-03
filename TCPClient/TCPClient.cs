using System.Net.Sockets;
using System.Text;

namespace TCPClient
{
    public class TCPClient
    {
        // Made for TCPListener testing and values application on the UI side as well as JSON parsin testing. Able to generate semi-random JSON requests.

        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _clientThread;
        private string _serverAddress;
        private int _serverPort;
        private bool _isConnected;

        public static void Main(string[] args) { }

        public List<string> _jsonTestingMessages = new List<string>
        {
            """
            {
            "activePlayers": 33,
            "biggestMultiplier": 666
            }
            """,
            """
            {
            "activePlayers":-53,
            "biggestMultiplier": 132
            }
            """,
            """
            {
            "activePlayers": 21,
            "y": 19
            }
            """,
            """
            {
            "activePlayers": 21,
            "x": 19,
            "y":"boom"
            }
            """,
            """
            {
            1: -21,
            "x": 19.21,
            "y":"boom"
            }
            """
        };

        public TCPClient(string address = "192.168.1.100", int port = 4948)
        {
            _serverAddress = address;
            _serverPort = port;
            _isConnected = false;
            _clientThread = new Thread(() =>
            {
                while (true)
                {
                    if (Connect())
                    {
                        SendMessage(GenerateRandomJsonRequest());
                        Disconnect();
                    }
                    Thread.Sleep(10);
                }
            });
            _clientThread.IsBackground = true;
            _clientThread.Start();
        }

        public bool Connect()
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(_serverAddress, _serverPort);

                _stream = _client.GetStream();
                _isConnected = true;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string SendMessage(string message)
        {
            if (!_isConnected)
            {
                return null;
            }

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                _stream.Write(data, 0, data.Length);
                Thread.Sleep(100);
                byte[] buffer = new byte[1024];
                int bytesRead = 0;

                if (_stream.DataAvailable)
                {
                    bytesRead = _stream.Read(buffer, 0, buffer.Length);
                }

                if (bytesRead > 0)
                {
                    return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                }

                return null;
            }
            catch (Exception ex)
            {
                _isConnected = false;
                return null;
            }
        }

        public void Disconnect()
        {
            if (!_isConnected)
            {
                return;
            }

            try
            {
                _stream.Close();
                _client.Close();
                _isConnected = false;
            } catch (Exception ex)
            {

            }
        }

        public string GenerateRandomJsonRequest()
        {
            string json = "{\r\n";

            string[] paramPool = { "activePlayers", "biggestMultiplier", "x", "y", "sunday", "happyHour", "fml", "CodeNameShitCode","etc" };
            string[] stringValuePool = { "monday", "tuesday", "wednesdsay", "alpha", "bravo", "charlie", "delta" };

            Random rng = new Random();

            int paramAmount = 0;
            paramAmount += rng.Next(0, 3); // if more than 2 should not be accepted on TCPListener Side. Leaving 1-3 pool of values to avoid UI stuck for too long

            for (int i = 0; i < paramAmount; i++)
            {
                int valuePoolChosenIndex = 0;
                valuePoolChosenIndex += rng.Next(0, 3);
                json += "\"" + paramPool[rng.Next(0, paramPool.Length)] + "\":";

                if (valuePoolChosenIndex == 0)
                {
                    json += rng.Next(-1000000, 1000000);

                } else if (valuePoolChosenIndex == 1)
                {
                    json += rng.NextDouble();

                } else if (valuePoolChosenIndex == 2)
                {
                    json += stringValuePool[rng.Next(0, stringValuePool.Length)];
                }

                if (i + 1 != paramAmount)
                {
                    json += ",\r\n";
                }
            }
            json += "\r\n}";
            return json;
        }
    }
}
