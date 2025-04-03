using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace RouletteApp
{
    public class TCPController
    {
        //TCP Listener and JSON checker. Validates that sent message contains appropriate parameter and values. Check if sent message is JSON.
        //If message is inaproriate (not json, too many or few params, unexpected params, unexpected values) message is not forwarded.

        private TcpListener _listener;
        private int _port;
        private bool _isRunning;
        private Thread _serverThread;

        public event EventHandler<JObject> AppropriateMessageReceived;

        public TCPController(int serverPort = 4948)
        {
            _port = serverPort;
            _isRunning = false;
        }

        public void Start()
        {
            if (_isRunning)
            {
                return;
            }

            try
            {
                _listener = new TcpListener(IPAddress.Any, _port);
                _listener.Start();
                _isRunning = true;
                _serverThread = new Thread(RunServer);
                _serverThread.IsBackground = true;
                _serverThread.Start();
            }
            catch (Exception ex)
            {
                throw new Exception("Error starting server: " + ex.Message);
            }
        }

        private void RunServer()
        {
            try
            {
                while (_isRunning)
                {
                    TcpClient client = _listener.AcceptTcpClient();
                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                if (_isRunning)
                {
                    throw new Exception("Server error: " + ex.Message);
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (_isRunning && client.Connected)
                {
                    if (stream.DataAvailable)
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            break;
                        }
                        string receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        if (IsValid(receivedText))
                        {
                            JObject tempObj = JObject.Parse(receivedText);
                            if (CheckIfRequestIsAcceptable(tempObj))
                            {
                                AppropriateMessageReceived.Invoke(this, tempObj);
                            }
                        }                     

                        if (receivedText != null)
                        {
                            byte[] responseData = Encoding.UTF8.GetBytes(receivedText);
                            stream.Write(responseData, 0, responseData.Length);
                        }
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error handling client: " + ex.Message);
            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }

        public void Stop()
        {
            if (!_isRunning)
            {
                return;
            }
            _isRunning = false;

            try
            {
                _listener.Stop();
                if (_serverThread != null && _serverThread.IsAlive)
                {
                    _serverThread.Join(1000);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error stopping server: " + ex.Message);
            }
        }

        private bool CheckIfRequestIsAcceptable(JObject request)
        {
            if (request.Count == 1 && (request.ContainsKey("activePlayers") || request.ContainsKey("biggestMultiplier")))
            {
                if (request.ContainsKey("activePlayers"))
                {
                    return IsValueAppropriate(request, "activePlayers");
                }
                else
                {
                    return IsValueAppropriate(request, "biggestMultiplier");
                }
            }
            else if (request.Count == 2 && request.ContainsKey("activePlayers") && request.ContainsKey("biggestMultiplier"))
            {
                return IsValueAppropriate(request, "activePlayers") && IsValueAppropriate(request, "biggestMultiplier");
            }
            return false;
        }

        private bool IsValueAppropriate(JObject request, string key)
        {
            string x = request[key].ToString();
            double ifInt = 0;
            if (x.All(char.IsDigit))
            {
                if (double.TryParse(x, out ifInt))
                {
                    return double.IsInteger(ifInt);
                }
            }
            return false;
        }

        private bool IsValid(string json)
        {
            string[] list = json.Split(',');
            if (list.Length <= 2)
            {
                try
                {
                    JObject.Parse(json);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }
            return false;

        }

    }

}
