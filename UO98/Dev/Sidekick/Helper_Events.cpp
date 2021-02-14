#pragma unmanaged

#include "stdafx.h"
#include "NativeEvents.h"

using namespace NativeMethods;

extern "C"
{
  void _declspec(dllexport) APIENTRY Test_EventsInvoke_OnPulse()
  {
    InvokeOnPulse();
  }

  void _declspec(dllexport) APIENTRY Test_EventInvoke_OnAfterSave()
  {
    InvokeOnAfterSave();
  }

  void _declspec(dllexport) APIENTRY Test_EventInvoke_OnPacketReceived(unsigned char* pSocket, unsigned char PacketID, unsigned int PacketSize, int IsPacketDynamicSized)
  {
    InvokeOnPacketReceived(pSocket, PacketID, PacketSize, IsPacketDynamicSized);
  }

  void _declspec(dllexport) APIENTRY Test_EventInvoke_OnOutsideRangePacketd(unsigned char* pSocket)
  {
    InvokeOnOutsideRangePacket(pSocket);
  }

  void _declspec(dllexport) APIENTRY Test_EventInvoke_OnPacketSending(unsigned char *pSocket, unsigned char **ppData, unsigned int *pDataLen)
  {
    InvokeOnPacketSending(pSocket, ppData, pDataLen);
  }

}
