using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpkick.Network;
using System.Runtime.InteropServices;

namespace Sharpkick_Tests
{
    unsafe class MockPacketEngine : IPacketEngine
    {
        Dictionary<uint, byte[]> LastSentPacketBySocketHandle = new Dictionary<uint, byte[]>();

        public void ClearLast(MockClient client)
        {
            if (LastSentPacketBySocketHandle.ContainsKey(client.SocketHandle))
                LastSentPacketBySocketHandle.Remove(client.SocketHandle);
        }

        public string VerifySent(MockClient client, byte[] data)
        {
            if (!LastSentPacketBySocketHandle.ContainsKey(client.SocketHandle))
                return "No packets have been sent to this handle.";

            byte[] sent = LastSentPacketBySocketHandle[client.SocketHandle];

            if (data.Length != sent.Length)
                return "Packet length mismatch.";

            for (int i = 0; i < data.Length; i++)
                if (data[i] != sent[i])
                    return string.Format("Data mismatch at position {0}", i);

            return string.Empty;
        }

        public int SocketObject_SendPacket(ClientSocketStruct* pSocket, byte* PacketData, uint DataSize)
        {
            UODemo.ISocket socket = new UODemo.Socket(this,(struct_ServerSocket*)pSocket); //ClientSocket(pSocket);
            uint handle = socket.SocketHandle;

            byte[] sentData = new byte[DataSize];
            for (int i = 0; i < DataSize; i++)
                sentData[i] = *(PacketData + i);

            LastSentPacketBySocketHandle[handle] = sentData;

            return 1;

        }

        public int SocketObject_RemoveFirstPacket(ClientSocketStruct* pSocket, uint Length)
        {
            if ((*pSocket).DataLength < Length) return 0;

            (*pSocket).DataLength-=Length;

            for (int i = 0; i < (*pSocket).DataLength; i++)
                *((*pSocket).Data + i) = *((*pSocket).Data + Length + i);
            
            return 1;
        }

        public int ReplaceServerPacketData(byte** pData, uint* pDataLen, byte* newData, uint newDataLength)
        {
            throw new NotImplementedException();
        }
    }


}
