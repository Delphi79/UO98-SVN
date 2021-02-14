#pragma once

#include "Classes.h"

namespace NativeMethods
{

    // Structure
    typedef struct PATCHINFO
    {
      unsigned int ExpectedAddress;
      unsigned char RequiredSize;
      unsigned char Required[256];
      unsigned char ReplacementSize;
      unsigned char Replacement[256];
    } PATCHINFO;

    // Functions
    extern void Patch(PATCHINFO *PatchInfo);
    extern void SetAbs32_AtRelPatch(PATCHINFO *PatchInfo, unsigned char Index, void *PatchFunction);
    extern void SetAbs32_AtPatch(PATCHINFO *PatchInfo, void *PatchFunction);
    extern void SetRel32(PATCHINFO *PatchInfo, unsigned char E8orE9, unsigned int DemoAddress, void *PatchFunction);
    extern void SetRel32_AtRelPatch(PATCHINFO *PatchInfo, unsigned char E8orE9, void *PatchFunction);
    extern void SetRel32_AtPatch(PATCHINFO *PatchInfo, void *PatchFunction);
}