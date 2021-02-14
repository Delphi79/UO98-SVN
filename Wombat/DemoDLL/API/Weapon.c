#include "Weapon.h"

int __stdcall IsRanged(WeaponObject *weapon)
{
  __asm
  {
    mov ecx, weapon
    mov eax, 0x4DF3AD
    call eax
  }
  return _EAX;
}

unsigned char __stdcall GetWeaponSpeed(WeaponObject *weapon)
{
  __asm
  {
    mov ecx, weapon
    mov eax, 0x4DFA7F
    call eax
  }
  return _AL;
}