#include "patcher.h"

void Patch(PATCHINFO *PatchInfo)
{
  // NOTE: ignore any errors
  DWORD OldProtect, TempProtect;
  DWORD NewProtect = PAGE_EXECUTE_READWRITE;

  // Modify
  VirtualProtect((LPVOID) PatchInfo->ExpectedAddress, 5, NewProtect, &OldProtect);

  // Compare
  if(memcmp((void *) PatchInfo->ExpectedAddress, PatchInfo->Required, PatchInfo->RequiredSize) == 0)
  {
    // Patch
    memcpy((void *) PatchInfo->ExpectedAddress, PatchInfo->Replacement, PatchInfo->ReplacementSize);
  }
  else
    printf("Patch failure! Bytes mismatch at 0x%08X\n", PatchInfo->ExpectedAddress);

  // Restore
  VirtualProtect((LPVOID) PatchInfo->ExpectedAddress, 5, OldProtect, &OldProtect);
}

void SetAbs32_AtRelPatch(PATCHINFO *PatchInfo, unsigned char Index, void *PatchFunction)
{
  // To be safe, make sure the relative index is within the Replacement range
  if(PatchInfo->ReplacementSize < 4 || Index > PatchInfo->ReplacementSize - 4)
  {
    printf("Patch failure! Index %u not in range (0-%u)\n", Index, PatchInfo->ReplacementSize - 1);
    return;
  }

  // Modify
  *(int *)&PatchInfo->Replacement[Index] = (int) PatchFunction;
}

void SetAbs32_AtPatch(PATCHINFO *PatchInfo, void *PatchFunction)
{
  SetAbs32_AtRelPatch(PatchInfo, 0, PatchFunction);
}

void SetRel32(PATCHINFO *PatchInfo, unsigned char E8orE9, unsigned int DemoAddress, void *PatchFunction)
{
  int Difference;

  // To be safe, make sure E8orE9 (relative index) is within the Replacement range
  if(PatchInfo->ReplacementSize < 5 || E8orE9 > PatchInfo->ReplacementSize - 5)
  {
    printf("Patch failure! Index %u not in range (0-%u)\n", E8orE9, PatchInfo->ReplacementSize - 1);
    return;
  }
  if(PatchInfo->Replacement[E8orE9] != 0xE8 && PatchInfo->Replacement[E8orE9] != 0xE9)
  {
    printf("Patch failure! Index %u is not 0XE8 or 0XE9 but 0X%02X\n", E8orE9, PatchInfo->Replacement[E8orE9]);
    return;
  }

  // Calculate the Rel32
  Difference = (int) ((unsigned int) PatchFunction - DemoAddress - 5);

  // Modify
  *(int *)&PatchInfo->Replacement[E8orE9 + 1] = Difference;
}

void SetRel32_AtRelPatch(PATCHINFO *PatchInfo, unsigned char E8orE9, void *PatchFunction)
{
  SetRel32(PatchInfo, E8orE9, PatchInfo->ExpectedAddress + E8orE9, PatchFunction);
}

void SetRel32_AtPatch(PATCHINFO *PatchInfo, void *PatchFunction)
{
  SetRel32_AtRelPatch(PatchInfo, 0, PatchFunction);
}
