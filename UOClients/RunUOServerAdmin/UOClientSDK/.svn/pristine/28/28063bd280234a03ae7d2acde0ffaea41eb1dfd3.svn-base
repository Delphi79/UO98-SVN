using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace UOClientSDKTestProject.Servers
{
    class CharServer : BaseServer
    {
        private Queue<byte[]> charQueue;

        public CharServer()
        {
            InitPacketQueue();
        }

        void InitPacketQueue()
        {
            charQueue = new Queue<byte[]>();
            for (byte i = (byte)'a'; i <= (byte)'z'; i++)
                charQueue.Enqueue(new byte[] { i });
        }

        protected override void ProcessClients(Dictionary<System.Net.Sockets.TcpClient, System.Net.Sockets.NetworkStream> clients)
        {
            byte[] toWrite = charQueue.Dequeue();
            charQueue.Enqueue(toWrite);
            foreach (TcpClient client in clients.Keys)
            {
                if (client.Connected && clients[client].CanWrite)
                {
                    try
                    {
                        clients[client].Write(toWrite, 0, 1);
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
