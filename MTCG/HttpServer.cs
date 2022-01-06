﻿using System;
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
            while (true)
            {
                TcpClient client = _listener.AcceptTcpClient();
                NetworkStream stream = client.GetStream();

                try
                {
                    var request = ReadRequest(stream);

                    //unterscheiden zwischen GET,POST,PUT,DELETE?
                    //parse curl script anfrage - zb. /users

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
