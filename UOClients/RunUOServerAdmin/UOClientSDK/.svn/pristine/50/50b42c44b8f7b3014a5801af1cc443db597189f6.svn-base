using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using UoClientSDK.Network;

namespace UOClientSDKTestProject.Servers
{
    class UOGPollResponder : BaseServer
    {

        public UOGPollResponder(int port = 2593) : base(port) { }

        List<TcpClient> SeededClients = new List<TcpClient>();

        const string UOGResponse = "Name=SecondAge Test, Age=0, Clients=1, Items=189, Chars=27, Mem=42125K, Ver=2";

        protected override void ProcessClients(Dictionary<System.Net.Sockets.TcpClient, System.Net.Sockets.NetworkStream> clients)
        {
            byte[] buffer = new byte[1024];

            foreach (TcpClient client in clients.Keys)
            {
                if (client.Connected && clients[client].CanWrite && clients[client].DataAvailable)
                {
                    if (!SeededClients.Contains(client))
                    {
                        byte[] seed = new byte[4];
                        int seedread = clients[client].Read(buffer, 0, 4);
                        if (seedread != 4)
                            throw new Exception("Expected 4 seed bytes");
                        SeededClients.Add(client);
                    }

                    int read = clients[client].Read(buffer, 0, 1024);
                    if (read == 4)
                    {
                        byte id = buffer[0];
                        ushort len = (ushort)((buffer[1] << 8) | buffer[2]);
                        byte cmd = buffer[3];

                        if (id == (byte)UoClientSDK.Network.ClientPackets.ClientPacketId.RemoteAdmin && len == 4)
                        {
                            if (cmd == (byte)UoClientSDK.Network.ClientPackets.RemoteAdminSubCommand.RequestUOGStatus)
                            {
                                try
                                {
                                    byte[] respBuffer = ASCIIEncoding.ASCII.GetBytes(UOGResponse);
                                    clients[client].Write(respBuffer, 0, respBuffer.Length);
                                }
                                catch (System.IO.IOException)
                                {
                                }
                                client.Close();
                            }
                            else if (cmd == (byte)UoClientSDK.Network.ClientPackets.RemoteAdminSubCommand.RequestUOGStatusCompact)
                            {
                                int Clients = 99;
                                int Items = 55;
                                int Mobiles = 33;
                                TimeSpan Age = TimeSpan.FromMinutes(4444);
                                long Memory = 1234567890;

                                PacketWriter writer = new PacketWriter(1000);

                                writer.Write((byte)UoClientSDK.Network.ServerPackets.ServerPacketId.AdminUOGStatusCompact);
                                writer.Write((ushort)27);
                                writer.Write(Clients);
                                writer.Write(Items);
                                writer.Write(Mobiles);
                                writer.Write((int)Age.TotalSeconds);
                                writer.Write((int)(Memory >> 32));
                                writer.Write((int)(Memory & 0xFFFF));

                                try
                                {
                                    clients[client].Write(writer.Packet, 0, writer.Length);
                                }
                                catch (System.IO.IOException)
                                {
                                }
                                client.Close();

                            }
                        }
                    }
                }
            }

        }

    }
}
