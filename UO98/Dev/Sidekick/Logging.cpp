// UO98: UoDemo Server
// Author: Derrick
// 03/10/2011
// Sends server trace messages to console.

#pragma unmanaged
#include <stdlib.h>
#include <stdio.h>
#include "patcher.h"

namespace NativeMethods
{
    //-=-=-=-=-
    void Initialize_logging(void);
    //-=-=-=-=-

    bool TraceOutput=true;	// True to hook trace output to console.

    #define ptFUNC_DebugPrint 0x0054B960		// This is the address for the core function "DebugPrint" (made up the name) which is hooked within FUNC_DebugWriteInfo (0x0046CDB4)
    typedef void (_cdecl *FUNCPTR_DebugPrint)(char* format, ...);
    FUNCPTR_DebugPrint FUNC_DebugPrint = (FUNCPTR_DebugPrint)ptFUNC_DebugPrint;	// This is the prototype for the core function hooked by PI_DebugWriteInfo.

    // WARNING! 0x0046CDAC does not exist in the original uodemo.exe, this is a UoDemo+, Publish 7+ function
    PATCHINFO PI_DebugWriteInfo =
    {
     0x0046CDAC, // call    FUNC_DebugPrint
     5, {0xE8, 0xAF, 0xEB, 0x0D, 0x00},
     5, {0xE8, 0x00, 0x00, 0x00, 0x00},
    };

    void __cdecl DebugWriteInfo(char* format, int int1, int int2, int int3, int uint1, char* str1, char* str2, char* str3, char* str4)
    {
        if(TraceOutput)
        {
            const char* debugFormat="0x%06X: 0x%08X|0x%08X|%u|%s|%s|%s|%s";
            printf(debugFormat,int1, int2, int3, uint1, str1, str2, str3, str4);
            puts(""); // newline
        }

        // The trace functionality produces an exception for trapping traces, this cannot be allowed under managed code unless these are handled. For now the original Trace output is disabled. -=Derrick=-
        //FUNC_DebugPrint(format, int1, int2, int3, uint1, str1, str2, str3, str4); // make the original trace call that we overrode here
    }

    void Initialize_logging()
    {
        TraceOutput=true;			  // true to hook "debug" trace output to console.

        SetRel32_AtPatch(&PI_DebugWriteInfo,DebugWriteInfo);
        Patch(&PI_DebugWriteInfo);
    }
}