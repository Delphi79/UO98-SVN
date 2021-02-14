// UO98: UoDemo Server
// Author: Derrick
// 03/12/2011
// Hooks pulses

#pragma unmanaged

#include "stdafx.h"
#include "patcher.h"
#include "NativeEvents.h"

namespace NativeMethods
{
    #define pFUNC_OnPulse 0x00004D8A9A	// Address of their time handler (called every 250ms tick)
    FUNCPTR_OnPulse	FUNC_OnPulse = (FUNCPTR_OnPulse)pFUNC_OnPulse;

    PATCHINFO PI_OnPulse =
    {
     0x004D83AD, //  call    FUNC_OnPulse
     5, {0xE8, 0xE8, 0x06, 0x00, 0x00},
     5, {0xE8, 0x00, 0x00, 0x00, 0x00},
    };

    void __cdecl OnPulse()
    {
      int THIS_GlobalTimeManagerObject;
      _asm mov THIS_GlobalTimeManagerObject, ecx
	    InvokeOnPulse();
      _asm mov ecx, THIS_GlobalTimeManagerObject
	    FUNC_OnPulse();
    }

    void Initialize_timer()
    {
      Initialize_timer(OnPulse);
    }

    void Initialize_timer(FUNCPTR_OnPulse func)
    {
	    SetRel32_AtPatch(&PI_OnPulse, func);
	    Patch(&PI_OnPulse);
    }
}