#pragma unmanaged

#include <stdio.h>
#include "NativeEvents.h"
#include "Classes.h"

namespace NativeMethods
{
    #define FUNC_WriteDynamic0 0x4C8A5C
    void SaveWorld()
    {
        puts("Saving world...");
        __asm
        {
            mov eax, FUNC_WriteDynamic0
            call eax
        }
        puts("World saved!");
  
        InvokeOnAfterSave();
    }

    #define GLOBAL_TerminateServerFlag 0x6999E0
    void ShutdownServer()
    {
      __asm
      {
        mov edx, GLOBAL_TerminateServerFlag
        mov dword ptr [edx], 1
      }
    }

    #define GLOBAL_HelpEngineObject 0x6982D8
    void __cdecl MakeCounselor(PlayerObject *Target, int CounType)
    {
      __asm
      {
        push CounType
        push Target
        mov ecx, GLOBAL_HelpEngineObject
        mov eax, 0x44E039
        call eax
      }
    }

    void __cdecl UnmakeCounselor(PlayerObject *Target)
    {
      __asm
      {
        push Target
        mov ecx, GLOBAL_HelpEngineObject
        mov eax, 0x44DEE6
        call eax
      }
    }

}
