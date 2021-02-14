#include "classes.h"

void* __cdecl _declspec(dllexport) ConvertSerialToObject(unsigned int serial)
{
  __asm
  {
    push serial
    mov ecx, 0x64B350
    mov eax, 0x48B8CA
    call eax
  }
  return (void *) _EAX;
}