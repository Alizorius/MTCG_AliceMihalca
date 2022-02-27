using MTCG.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTCG.Http
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
            string response = "Error: Tasks can't be processed!\n\r";
            try
            {
                var request = ReadRequest(stream);

                //Do not require the User to be logged in
                if (request.StartsWith("POST") && request.Contains("/users"))
                {
                    response = HttpRequestHandler.PostUsersRequest(request);
                    return;
                }
                else if (request.StartsWith("POST") && request.Contains("/sessions"))
                {
                    response = HttpRequestHandler.PostSessionsRequest(request);
                    return;
                }

                if (!HttpRequestHandler.UserLoggedInCheck(request))
                {
                    response = "Command can't be processed! You must log in first.\n\r";
                    return;
                }
                //GET
                if (request.StartsWith("GET") && request.Contains("/cards"))
                {
                    response = HttpRequestHandler.GetCardsRequest(request);
                }
                else if (request.StartsWith("GET") && request.Contains("/deck"))
                {
                    response = HttpRequestHandler.GetDeckRequest(request);
                }
                else if (request.StartsWith("GET") && request.Contains("/users"))
                {
                    response = HttpRequestHandler.GetUsersRequest(request);
                }
                else if (request.StartsWith("GET") && request.Contains("/stats"))
                {
                    response = HttpRequestHandler.GetStatsRequest(request);
                }
                else if (request.StartsWith("GET") && request.Contains("/score"))
                {
                    response = HttpRequestHandler.GetScoreRequest();
                }
                else if (request.StartsWith("GET") && request.Contains("/tradings"))
                {
                    response = HttpRequestHandler.GetTradingsRequest();
                }

                //POST
                else if (request.StartsWith("POST") && request.Contains("/transactions"))
                {
                    response = HttpRequestHandler.PostTransactionsRequest(request);
                }
                else if (request.StartsWith("POST") && request.Contains("/packages"))
                {
                    response = HttpRequestHandler.PostPackagesRequest(request);
                }
                else if (request.StartsWith("POST") && request.Contains("/tradings/"))
                {
                    response = HttpRequestHandler.PostAcceptTradeRequest(request);
                }
                else if (request.StartsWith("POST") && request.Contains("/tradings"))
                {
                    response = HttpRequestHandler.PostTradingsRequest(request);
                }
                else if (request.StartsWith("POST") && request.Contains("/battles"))
                {
                    HttpRequestHandler.PostBattlesRequest(stream, request);
                    response = "Battle finished!\n\r";
                }

                //PUT
                else if (request.StartsWith("PUT") && request.Contains("/deck"))
                {
                    response = HttpRequestHandler.PutDeckRequest(request);
                }
                else if (request.StartsWith("PUT") && request.Contains("/users"))
                {
                    response = HttpRequestHandler.PutUsersRequest(request);
                }

                //DELETE
                else if (request.StartsWith("DELETE") && request.Contains("/tradings"))
                {
                    response = HttpRequestHandler.DeleteTradingsRequest(request);
                }


            }
            finally
            {
                HttpResponse.SendMessage(stream, response);
                stream.Close();
                client.Close();
            }
        }


    }
}
