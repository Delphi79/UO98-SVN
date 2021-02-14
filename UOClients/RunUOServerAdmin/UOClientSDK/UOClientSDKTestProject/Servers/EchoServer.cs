using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace UOClientSDKTestProject.Servers
{
    class EchoServer : BaseServer
    {
        protected override void ProcessClients(Dictionary<System.Net.Sockets.TcpClient, System.Net.Sockets.NetworkStream> clients)
        {
            byte[] buffer = new byte[1024];

            foreach (TcpClient client in clients.Keys)
            {
                if (client.Connected && clients[client].CanWrite && clients[client].DataAvailable)
                {
                    int read = clients[client].Read(buffer, 0, 1024);
                    try
                    {
                        clients[client].Write(buffer, 0, read);
                    }
                    catch (System.IO.IOException)
                    {
                        client.Close();
                    }
                }
            }

        }
    }
}
