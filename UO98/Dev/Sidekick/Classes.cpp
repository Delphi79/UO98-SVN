#pragma unmanaged

#include "stdafx.h"

namespace NativeMethods
{
  void* ConvertSerialToObject(unsigned int serial)
  {
    int _EAX;
    __asm
    {
      push serial
      mov ecx, 0x64B350
      mov eax, 0x48B8CA
      call eax
      mov _EAX, eax
    }
    return (void*)_EAX;
  }
}