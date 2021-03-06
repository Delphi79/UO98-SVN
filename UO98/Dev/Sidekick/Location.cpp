#include "Classes.h"
#include <stdio.h>

#pragma unmanaged

void initLocation(LocationObject* location)
{
  location->X=0;
  location->Y=0;
  location->Z=0;
}

void Location_ToString(char* buffer, LocationObject* location)
{
  if(location)
    sprintf(buffer,"(%d,%d,%d)", location->X, location->Y, location->Z);
  else
    sprintf(buffer,"(null)");
}

int __stdcall IsEqualXY(LocationObject *A, LocationObject *B)
{
  int _EAX;
  __asm
  {
    push B
    mov ecx, A
    mov eax, 0x4210D0
    call eax
    mov _EAX, eax
  }
  return _EAX;
}

int __stdcall IsEqualXYZ(LocationObject *A, LocationObject *B)
{
  int _EAX;
  __asm
  {
    push B
    mov ecx, A
    mov eax, 0x421120
    call eax
    mov _EAX, eax
  }
  return _EAX;
}
