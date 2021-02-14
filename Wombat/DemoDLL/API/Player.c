#include "Player.h"

void __cdecl _declspec(dllexport) SendSystemMessage(PlayerObject *player, const char *message)
{
  if((player != NULL) && IsPlayerObject(player))
    __asm
    {
      push message
      mov ecx, player
      mov eax, 0x04516BF
      call eax
    } 
}

#define pFUNC_PlayerObject_MakeGameMaster 0x00454D7D
void __cdecl _declspec(dllexport) MakeGameMaster(PlayerObject *Target)
{
	__asm
	{
		mov ecx, Target
		mov eax, pFUNC_PlayerObject_MakeGameMaster
		call eax
	}
}

#define pFUNC_PlayerObject_UnmakeGameMaster 0x00454DC0
void __cdecl _declspec(dllexport) UnmakeGameMaster(PlayerObject *Target)
{
	__asm
	{
		mov ecx, Target
		mov eax, pFUNC_PlayerObject_UnmakeGameMaster
		call eax
	}
}

#define pFUNC_PlayerObject_IsGameMaster_A 0x00454E03
int __cdecl _declspec(dllexport) IsGameMaster(PlayerObject *Target)
{
	__asm
	{
		mov ecx, Target
		mov eax, pFUNC_PlayerObject_IsGameMaster_A
		call eax
	}
	return _EAX;
}

#define pFUNC_MobileObject_OpenBank 0x0047212F
int __cdecl _declspec(dllexport) OpenBank(PlayerObject *PlayerObjectWhoseBankToOpen, PlayerObject *MobileObjectWhereBankWillBeShown)
{
	__asm
	{
	  push MobileObjectWhereBankWillBeShown
	  mov  ecx, PlayerObjectWhoseBankToOpen
	  mov  eax, 0x47212F
	  call eax
	}
	return _EAX;
}


