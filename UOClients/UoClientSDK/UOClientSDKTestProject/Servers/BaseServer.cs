using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace UOClientSDKTestProject.Servers
{
    abstract class BaseServer : IDisposable
    {
        bool run = false;

        TcpListener listener;

        public int Port{get;private set;}

        public BaseServer(int port = 9999)
        {
            Port = port;
            listener = new TcpListener(IPAddress.Any, Port);
            listener.Start();
            Start();
        }

        async void Start()
        {
            if (run) return;

            Dictionary<TcpClient, NetworkStream> clients = new Dictionary<TcpClient, NetworkStream>();

            run = true;
            while (run)
            {
                if (listener.Pending())
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        clients.Add(client, client.GetStream());
                    }
                    catch (SocketException)
                    {

                    }
                }

                ProcessClients(clients);

                await TaskEx.Delay(1);
            }

            if (listener != null) listener.Stop();
            listener = null;
        }

        protected abstract void ProcessClients(Dictionary<TcpClient, NetworkStream> clients);

        public void Stop()
        {
            run = false;
        }

        public void Dispose()
        {
            if (listener != null) listener.Stop();
            Stop();
        }

        /// <summary>
        /// function to check if a port is in use
        /// </summary>
        /// <param name="port">the port to check</param>
        /// <returns>false if the port is in use or else return true</returns>
        //public static bool PortAvailable(int port)
        //{
        //    IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        //    TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
        //    System.Collections.IEnumerator myEnum = tcpConnInfoArray.GetEnumerator();
        //    while (myEnum.MoveNext())
        //    {
        //        TcpConnectionInformation tcpi = (TcpConnectionInformation)myEnum.Current;

        //        if (tcpi.LocalEndPoint.Port == port)
        //        {
        //            return false;
        //        }
        //    }

        //    return true;
        //}
    }
}
