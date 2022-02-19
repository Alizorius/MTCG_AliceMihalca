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
            DB.CreateDBIfNotPresent();
            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                try
                {
                    var request = ReadRequest(stream);
                    bool format = false;

                    if (request.StartsWith("GET"))
                    {
                        if (request.Contains("/cards"))
                        {
                            HttpResponse.SendCards(format, stream, DBCard.GetAllUserCards(Helper.ExtractUsernameToken(request)));

                        }
                        else if (request.Contains("/deck"))
                        {
                            if (request.Contains("format=plain"))
                            {
                                format = true;
                            }
                            HttpResponse.SendCards(format, stream, DBCard.GetDeck(Helper.ExtractUsernameToken(request)));
                        }
                        else if (request.Contains("/users"))
                        {
                            if (Helper.ExtractUsername(request).Equals(Helper.ExtractUsernameToken(request)))
                            {
                                DBUser.GetUser(Helper.ExtractUsername(request));
                            }
                            //error
                        }
                        else if (request.Contains("/stats"))
                        {

                        }
                        else if (request.Contains("/score"))
                        {

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
                        }
                        else if (request.Contains("/sessions"))
                        {

                        }
                        else if (request.Contains("/transactions"))
                        {
                            DBPackage.AcquirePackage(Helper.ExtractUsernameToken(request));
                        }
                        else if (request.Contains("/packages"))
                        {
                            DBPackage.AddPackage(Helper.ExtractCards(request));
                        }
                        else if (request.Contains("/tradings"))
                        {

                        }
                    }
                    else if (request.StartsWith("PUT"))
                    {
                        if (request.Contains("/deck"))
                        {
                            DBCard.ConfigureDeck(Helper.ExtractUsernameToken(request), Helper.ExtractCardIds(request));
                        }
                        else if (request.Contains("/users"))
                        {

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
}
