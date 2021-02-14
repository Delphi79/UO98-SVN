#include "Mobile.h"

int __stdcall AdvanceSwingState(unsigned int mobile)
{
  __asm
  {
    mov ecx, mobile
    mov eax, 0x445981
    call eax
  }
  return _EAX;
}

void __stdcall ResetSwingState(unsigned int mobile, unsigned int action)
{
  __asm
  {
    push action
    mov ecx, mobile
    mov eax, 0x445981
    call eax
  }
}

unsigned int __stdcall GetWeaponInHand(unsigned int mobile)
{
  __asm
  {
    mov ecx, mobile
    mov eax, 0x46D7A5
    call eax
  }
  return _EAX;
}

unsigned int __stdcall GetSwingSpeed(unsigned int mobile)
{
  __asm
  {
    mov ecx, mobile
    mov eax, 0x445901
    call eax
  }
  return _EAX;
}