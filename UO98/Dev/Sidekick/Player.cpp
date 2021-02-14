#pragma unmanaged

#include "Classes.h"
#include "stdafx.h"

namespace NativeMethods
{
    extern "C"
    {
      void _declspec(dllexport) APIENTRY SendSystemMessage(void *player, const char *message)  
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
      int _declspec(dllexport) APIENTRY MakeGameMaster(void* player)
      {
        __asm
        {
          mov ecx, player
          mov eax, pFUNC_PlayerObject_MakeGameMaster
          call eax
        }
      }

      #define pFUNC_PlayerObject_UnmakeGameMaster 0x00454DC0
      int _declspec(dllexport) APIENTRY UnmakeGameMaster(void* player)
      {
        __asm
        {
          mov ecx, player
          mov eax, pFUNC_PlayerObject_UnmakeGameMaster
          call eax
        }
      }

      #define pFUNC_PlayerObject_IsGameMaster_A 0x00454E03
      int _declspec(dllexport) APIENTRY IsGameMaster(void* player)
      {
        int _EAX;
        __asm
        {
          mov ecx, player
          mov eax, pFUNC_PlayerObject_IsGameMaster_A
          call eax
          mov _EAX, eax
        }
        return _EAX;
      }

      #define pFUNC_MobileObject_OpenBank 0x0047212F
      int _declspec(dllexport) OpenBank(PlayerObject *PlayerObjectWhoseBankToOpen, PlayerObject *MobileObjectWhereBankWillBeShown)
      {
        int _EAX;
        __asm
        {
          push MobileObjectWhereBankWillBeShown
          mov  ecx, PlayerObjectWhoseBankToOpen
          mov  eax, 0x47212F
          call eax
          mov _EAX, eax
        }
        return _EAX;
      }

    }
}
