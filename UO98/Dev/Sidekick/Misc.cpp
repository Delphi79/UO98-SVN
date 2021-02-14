// UO98: UoDemo Server
// Author: Derrick
// 12/07/2011
// Misc patches that don't fit into any other category.

#pragma unmanaged

#include <time.h>
#include "patcher.h"

namespace NativeMethods
{

    //-=-=-=-=-
    void Initialize_misc(void);
    void PatchAllTimeGetTime(void);
    void PatchTimeGetTimeAtAddress(unsigned int address);
    //-=-=-=-=-

    int GetTimeInSeconds()
    {
        time_t ltime;
        time(&ltime);
        return (int)ltime;
    }

    // This is a patch for the timing warning regarding DoTick duration. 
    // The function seems to expect time in Seconds, but gets time in MS due to a bug which is inconsistent with other uses of the function in the UODemo. 
    // These patches replace the existing call returning milliseconds to a call to a local function returning seconds.
    PATCHINFO PI_GetTime =
    {
        0,
        6, {0xFF, 0x15, 0x5C, 0x58, 0x9A, 0x00},  // call    ds:timeGetTime
        6, {0xE8, 0x00, 0x00, 0x00, 0x00, 0x90},  // call    GetTimeInSeconds + nop
    };


    void Initialize_misc()
    {
        PatchAllTimeGetTime();
    }

    void PatchAllTimeGetTime()
    {
        PatchTimeGetTimeAtAddress(0x004682D1); // ServerWinMain                                    call    ds:timeGetTime
        PatchTimeGetTimeAtAddress(0x004682BB); // ServerWinMain                                    call    ds:timeGetTime
    }

    void PatchTimeGetTimeAtAddress(unsigned int address)
    {
        (&PI_GetTime)->ExpectedAddress=address;
        SetRel32_AtPatch(&PI_GetTime,GetTimeInSeconds);
        Patch(&PI_GetTime);
    }
}