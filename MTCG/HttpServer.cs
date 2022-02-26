using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTCG
{
    class HttpServer
    {
        Thread _serverThread = null;
        TcpListener _listener;

        public void Start(int port = 10001)
        {
            if (_serverThread == null)
            {
                DB.CreateDBIfNotPresent();
                IPAddress ipAddress = new IPAddress(0);
                _listener = new TcpListener(ipAddress, port);
                _serverThread = new Thread(ServerHandler);
                _serverThread.Start();
            }
        }

        public void Stop()
        {
            if (_serverThread != null)
            {
                _serverThread = null;
            }
        }

        String ReadRequest(NetworkStream stream)
        {
            MemoryStream contents = new MemoryStream();
            var buffer = new byte[2048];
            do
            {
                var size = stream.Read(buffer, 0, buffer.Length);
                if (size == 0)
                {
                    return null;
                }
                contents.Write(buffer, 0, size);
            } while (stream.DataAvailable);
            var retVal = Encoding.UTF8.GetString(contents.ToArray());
            Console.WriteLine(retVal);
            return retVal;
        }

        void ServerHandler(Object o)
        {
            _listener.Start();
            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                Thread t = new Thread(() => requestThread(client));
                t.Start();
            }
        }

        void requestThread(TcpClient client)
        {
            
            NetworkStream stream = client.GetStream();

            try
            {
                var request = ReadRequest(stream);
                bool format = false;

                if (request.StartsWith("GET"))
                {
                    if (request.Contains("/cards"))
                    {
                        HttpResponse.SendCards(format, stream, DBCard.GetAllUserCards(request));

                    }
                    else if (request.Contains("/deck"))
                    {
                        if (request.Contains("format=plain"))
                        {
                            format = true;
                        }
                        HttpResponse.SendCards(format, stream, DBCard.GetDeck(request));
                    }
                    else if (request.Contains("/users"))
                    {
                        if (Helper.ExtractUsername(request).Equals(Helper.ExtractUsernameToken(request)))
                        {
                            DBUser.GetUser(request);
                        }
                        //error
                    }
                    else if (request.Contains("/stats"))
                    {
                        HttpResponse.SendStats(stream, DBScore.GetStats(request));
                    }
                    else if (request.Contains("/score"))
                    {
                        HttpResponse.SendScoreboard(stream, DBScore.GetScoreBoard());
                    }
                    else if (request.Contains("/tradings"))
                    {

                    }
                }
                else if (request.StartsWith("POST"))
                {
                    if (request.Contains("/users"))
                    {
                        DBUser.AddUser(request);
                        DBScore.SetDefaultStats(request);
                    }
                    else if (request.Contains("/sessions"))
                    {

                    }
                    else if (request.Contains("/transactions"))
                    {
                        DBPackage.AcquirePackage(request);
                    }
                    else if (request.Contains("/packages"))
                    {
                        DBPackage.AddPackage(request);
                    }
                    else if (request.Contains("/tradings"))
                    {

                    }
                    else if (request.Contains("/battles"))
                    {
                        BattleRequests.AddRequestToPool(stream, request);
                        BattleRequests.startMatch(request);
                    }
                }
                else if (request.StartsWith("PUT"))
                {
                    if (request.Contains("/deck"))
                    {
                        DBCard.ConfigureDeck(request);
                    }
                    else if (request.Contains("/users"))
                    {
                        if (Helper.ExtractUsername(request).Equals(Helper.ExtractUsernameToken(request)))
                        {
                            DBUser.UpdateUserData(request);
                        }
                    }
                }
                else if (request.StartsWith("DELETE"))
                {
                    if (request.Contains("/tradings"))
                    {

                    }
                }
                //else?

            }
            finally
            {
                stream.Close();
                client.Close();
            }
        }
    }
}
