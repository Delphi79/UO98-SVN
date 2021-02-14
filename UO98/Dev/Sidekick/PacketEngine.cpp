#include "UODemo.h"
#include "Commands.h"

namespace UODemo
{
    PacketEngine::PacketEngine()
    {
        OnPacketReceivedHandler = gcnew OnPacketReceivedEventHandler(this, &PacketEngine::InvokeOnPacketReceived);
        GlobalOnPacketReceived += OnPacketReceivedHandler;

        OnOutsideRangePacketHandler = gcnew OnOutsideRangePacketEventHandler(this, &PacketEngine::InvokeOnOutsideRangePacket);
        GlobalOnOutsideRangePacket += OnOutsideRangePacketHandler;

        OnPacketSendingHandler = gcnew OnPacketSendingEventHandler(this, &PacketEngine::InvokeOnPacketSending);
        GlobalOnPacketSending += OnPacketSendingHandler;
    }

    PacketEngine::~PacketEngine()
    {
        GlobalOnPacketReceived -= OnPacketReceivedHandler;
    }

    void PacketEngine::InvokeGlobalOnPacketReceived(unsigned char* pSocket, unsigned char PacketID, unsigned int PacketSize, int IsPacketDynamicSized)
    {
        GlobalOnPacketReceived(pSocket, PacketID, PacketSize, IsPacketDynamicSized);
    }

    void PacketEngine::InvokeOnPacketReceived(unsigned char* pSocket, unsigned char PacketID, unsigned int PacketSize, int IsPacketDynamicSized)
    {
        OnPacketReceived(pSocket, PacketID, PacketSize, IsPacketDynamicSized);
    }

    bool PacketEngine::InvokeGlobalOnOutsideRangePacket(unsigned char* pSocket)
    {
        return GlobalOnOutsideRangePacket(pSocket);
    }

    bool PacketEngine::InvokeOnOutsideRangePacket(unsigned char* pSocket)
    {
        return OnOutsideRangePacket(pSocket);
    }

    void PacketEngine::InvokeGlobalOnPacketSending(unsigned char *pSocket, unsigned char **ppData, unsigned int *pDataLen)
    {
        GlobalOnPacketSending(pSocket, ppData, pDataLen);
    }

    void PacketEngine::InvokeOnPacketSending(unsigned char *pSocket, unsigned char **ppData, unsigned int *pDataLen)
    {
        OnPacketSending(pSocket, ppData, pDataLen);
    }

    int PacketEngine::SocketObject_SendPacket(void* pSocket, unsigned __int8* PacketData, unsigned int DataSize)
    {
        return NativeMethods::SocketObject_SendPacket(pSocket, PacketData, DataSize);
    }

    void PacketEngine::SocketObject_RemoveFirstPacket(void* pSocket, unsigned int Length)
    {
        NativeMethods::SocketObject_RemoveFirstPacket(pSocket, Length);
    }

    void PacketEngine::ReplaceServerPacketData(unsigned __int8** pData, unsigned int* pDataLen, unsigned __int8* newData, unsigned int newDataLength)
    {
        NativeMethods::ReplaceServerPacketData(pData, pDataLen, newData, newDataLength);
    }
}
