using System;
using Sharpkick;
using Sharpkick.Network;
using System.Runtime.InteropServices;
using System.Net;

namespace Sharpkick_Tests
{
    unsafe class MockClient : IDisposable
    {
        public ClientSocketStruct* Socket { get; private set; }

        public uint SocketDataLength
        {
            get { return (*(Socket)).DataLength; }
            set { (*(Socket)).DataLength = value; }
        }

        public ClientVersionStruct Version
        {
            get { return (*(Socket)).VersionInfo; }
            set { (*(Socket)).VersionInfo = value; }
        }

        public uint SocketHandle
        {
            get { return (*(Socket)).Handle; }
        }

        public void Enqueue(BaseClientPacketMock packet)
        {
            for (int i = 0; i < packet.Length; i++)
                *((*Socket).Data + SocketDataLength + i) = packet.PacketData[i];
            SocketDataLength += packet.Length;
        }

        public ClientPacketSafe ProcessOnly(BaseClientPacketMock expected)
        {
            ClientPacketSafe result = ClientPacket.Instantiate((byte*)Socket, Socket->Data[0], expected.Length, expected.Dynamic);
            CoreEvents.OnPacketReceived((byte*)Socket, expected.PacketID, expected.Length, expected.Dynamic);
            return result;
        }

        public ClientPacketSafe ProcessAndCheck(BaseClientPacketMock expected)
        {
            ClientPacketSafe result = ProcessOnly(expected);
            expected.VerifyTransform(result);

            return result;
        }

        public MockClient(uint handle, IPAddress ipaddress)
        {
            IntPtr socketPointer = Marshal.AllocHGlobal(sizeof(ClientSocketStruct));
            IntPtr playerPointer = Marshal.AllocHGlobal(sizeof(PlayerObject));
            
            Socket = (ClientSocketStruct*)socketPointer;
            (*Socket) = new ClientSocketStruct();

            (*Socket).PlayerObject = (PlayerObject*)playerPointer;
            (*((*Socket).PlayerObject)) = new PlayerObject();

            (*Socket).Handle = handle;
            (*Socket).IpAddressLong = IPToInt(ipaddress);
        }

        private uint IPToInt(IPAddress ip)
        {
            byte[] bytes = ip.GetAddressBytes();
            return (uint)((bytes[0] << (3*8)) + (bytes[1] << (2*8)) + (bytes[2] << (1*8)) + bytes[3]);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr)((*Socket).PlayerObject));
            Marshal.FreeHGlobal((IntPtr)Socket);
        }
    }
}
