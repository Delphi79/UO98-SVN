#include "stdafx.h"
#include <stdio.h>
#include <io.h>

#include "UODemo.h"

// CLI Event invocation proxies for native code.

namespace NativeMethods
{
    void InvokeOnPulse()
    {
        UODemo::Core::InvokeGlobalOnPulse();
    }
    void InvokeOnAfterSave()
    {
        UODemo::Core::InvokeGlobalOnAfterSave();
    }
    void InvokeOnPacketReceived(unsigned char* pSocket, unsigned char PacketID, unsigned int PacketSize, int IsPacketDynamicSized)
    {
        UODemo::PacketEngine::InvokeGlobalOnPacketReceived(pSocket, PacketID, PacketSize, IsPacketDynamicSized!=0);
    }
    bool InvokeOnOutsideRangePacket(unsigned char* pSocket)
    {
        return UODemo::PacketEngine::InvokeGlobalOnOutsideRangePacket(pSocket);
    }
    void InvokeOnPacketSending(unsigned char *pSocket, unsigned char **ppData, unsigned int *pDataLen)
    {
        UODemo::PacketEngine::InvokeGlobalOnPacketSending(pSocket, ppData, pDataLen);
    }
}