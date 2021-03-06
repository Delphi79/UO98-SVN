#pragma once

#include "patcher.h"


extern PATCHINFO PI_ResetSwingState_CallAdvanceSwingState;
extern PATCHINFO PI_CombatHeartBeat_CallAdvanceSwingState;

extern PATCHINFO PI_SetSwingDividendTo40000;
extern PATCHINFO PI_SetSwingDividendTo60000;
extern PATCHINFO PI_SetFatigueBasedSwing;
extern PATCHINFO PI_SetDexterityBasedSwing;

extern PATCHINFO PI_OnEquipUnequip_ResetSwingCounter;

extern PATCHINFO PI_CombatHeartBeat_IsActiveSwing_SetOriginal;
extern PATCHINFO PI_CombatHeartBeat_IsActiveSwing_TakeControl;
extern PATCHINFO PI_CombatHeartBeat_IsAmmoCheck_SetOriginal;
extern PATCHINFO PI_CombatHeartBeat_IsAmmoCheck_TakeControl;
extern PATCHINFO PI_CombatHeartBeat_IsAnimationOrHit_SetCombinedOriginal;
extern PATCHINFO PI_CombatHeartBeat_IsAnimationOrHit_TakeCombinedControl;

extern PATCHINFO PI_CanSeeLoc1, PI_CanSeeLoc2, PI_CanSeeLoc3, PI_CanSeeLoc4, PI_CanSeeLoc5, PI_CanSeeLoc6;

extern PATCHINFO PI_PacketHandler_PostReceive;

// ****************************************
// ****************************************
// PI_XXX variables -> DO NOT EDIT!!!
//
// These are machine code (Intel IA-32)
// and at the same time the patches the
// DLL is able to implement.
//
// Do not modify these unless you know the
// difference between E8 and E9 etc.
// ****************************************
// ****************************************

PATCHINFO PI_ResetSwingState_CallAdvanceSwingState =
{
  0x445AEA, 
  5, {0xE8, 0x92, 0xFE, 0xFF, 0xFF},
  5, {0xE8, 0x00, 0x00, 0x00, 0x00},
};

PATCHINFO PI_CombatHeartBeat_CallAdvanceSwingState =
{
  0x445ED6, 
  5, {0xE8, 0xA6, 0xFA, 0xFF, 0xFF},
  5, {0xE8, 0x00, 0x00, 0x00, 0x00},
};

PATCHINFO PI_SetSwingDividendTo40000 =
{
  0x445960, 
  1, {0xB8},
  5, {0xB8, 0x40, 0x9C, 0x00, 0x00},
};

PATCHINFO PI_SetSwingDividendTo60000 =
{
  0x445960, 
  1, {0xB8},
  5, {0xB8, 0x60, 0xEA, 0x00, 0x00},
};

PATCHINFO PI_SetFatigueBasedSwing =
{
  0x44593F,
  3, {0x66, 0x8B, 0x91},
  7, {0x66, 0x8B, 0x91, 0x3E, 0x02, 0x00, 0x00},
};

PATCHINFO PI_SetDexterityBasedSwing =
{
  0x44593F,
  3, {0x66, 0x8B, 0x91},
  7, {0x66, 0x8B, 0x91, 0x5C, 0x02, 0x00, 0x00},
};

PATCHINFO PI_OnEquipUnequip_ResetSwingCounter =
{
  0x4DF707,
  18, {0x8B, 0x4D, 0x08, 0xC7, 0x81, 0x14, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xB8, 0x01, 0x00, 0x00, 0x00},
  18, {0x8B, 0x4D, 0xE4, 0xFF, 0x75, 0x0C, 0xFF, 0x75, 0x08, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x6A, 0x01, 0x90, 0x58},
};

PATCHINFO PI_CombatHeartBeat_IsActiveSwing_SetOriginal =
// return 1 -> Do The Swing Code -> OldState != NewState && NewState >= 2
{
  0x445EDB,
   3, {0x89, 0x45, 0xE0},
  22, {0x89, 0x45, 0xE0, 0x8B, 0x45, 0xEC, 0x3B, 0x45, 0xE0, 0x74, 0x06, 0x83, 0x7D, 0xE0, 0x02, 0x7D, 0x05, 0xE9, 0x9D, 0x07, 0x00, 0x00},
};

PATCHINFO PI_CombatHeartBeat_IsActiveSwing_TakeControl =
{
  0x445EDB,
   3, {0x89, 0x45, 0xE0},
  22, {0x89, 0x45, 0xE0, 0x50, 0xFF, 0x75, 0xEC, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x09, 0xC0, 0x75, 0x06, 0x90, 0xE9, 0x9D, 0x07, 0x00, 0x00},
};


PATCHINFO PI_CombatHeartBeat_IsAmmoCheck_SetOriginal =
// return 1 -> Do The Ammo Check -> NewState == 2
{
  0x446070,
   7, {0x75, 0x05, 0xE9, 0x17, 0x06, 0x00, 0x00},
  27, {0x75, 0x05, 0xE9, 0x17, 0x06, 0x00, 0x00, 0x83, 0x7D, 0xE0, 0x02, 0x75, 0x6B, 0x8B, 0x8D, 0x64, 0xFF, 0xFF, 0xFF, 0x8B, 0x11, 0x8B, 0x8D, 0x64, 0xFF, 0xFF, 0xFF},
};

PATCHINFO PI_CombatHeartBeat_IsAmmoCheck_TakeControl =
{
  0x446070,
   7, {0x75, 0x05, 0xE9, 0x17, 0x06, 0x00, 0x00},
  27, {0X75, 0x05, 0xE9, 0x17, 0x06, 0x00, 0x00, 0xFF, 0x75, 0xE0, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x09, 0xC0, 0x74, 0x65, 0x8B, 0x8D, 0x64, 0xFF, 0xFF, 0xFF, 0x8B, 0x11},
};

PATCHINFO PI_CombatHeartBeat_IsAnimationOrHit_SetCombinedOriginal =
// return 1 -> Do The Animation -> NewState == 2
// return 1 -> Do The Hit       -> NewState != 2
{
  0x446111,
  5, {0xE8, 0x27, 0xC9, 0xFF, 0xFF},
 84, {0xE8, 0x27, 0xC9, 0xFF, 0xFF, 0x83, 0x7D, 0xE0, 0x02, 0x75, 0x45, 0x8B, 0x45, 0xB8, 0x50, 0x8B, 0x4D, 0xBC, 0x51, 0x8B, 0x95, 0x64, 0xFF, 0xFF, 0xFF, 0x52, 0xE8, 0x8C, 0xDD, 0xFF, 0xFF, 0x83, 0xC4, 0x0C, 0x83, 0x7D, 0xBC, 0x00, 0x74, 0x23, 0x8B, 0x4D, 0xBC, 0xE8, 0x6C, 0x92, 0x09, 0x00, 0x85, 0xC0, 0x74, 0x17, 0x8B, 0x45, 0xBC, 0x50, 0x8B, 0x4D, 0xB8, 0x51, 0x8B, 0x95, 0x64, 0xFF, 0xFF, 0xFF, 0x52, 0xE8, 0x4F, 0xD5, 0xFF, 0xFF, 0x83, 0xC4, 0x0C, 0xE9, 0x2D, 0x05, 0x00, 0x00, 0x8B, 0x45, 0xBC, 0x50},
};

PATCHINFO PI_CombatHeartBeat_IsAnimationOrHit_TakeCombinedControl =
{
  0x446111,
  5, {0xE8, 0x27, 0xC9, 0xFF, 0xFF},
 84, {0xE8, 0x27, 0xC9, 0xFF, 0xFF, 0xFF, 0x75, 0xE0, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x09, 0xC0, 0x75, 0x35, 0xFF, 0x75, 0xB8, 0xFF, 0x75, 0xBC, 0xFF, 0xB5, 0x64, 0xFF, 0xFF, 0xFF, 0xE8, 0x89, 0xDD, 0xFF, 0xFF, 0x83, 0xC4, 0x0C, 0x8B, 0x4D, 0xBC, 0xE3, 0x1C, 0xE8, 0x6D, 0x92, 0x09, 0x00, 0x91, 0xE3, 0x14, 0xFF, 0x75, 0xBC, 0xFF, 0x75, 0xB8, 0xFF, 0xB5, 0x64, 0xFF, 0xFF, 0xFF, 0xE8, 0x54, 0xD5, 0xFF, 0xFF, 0x83, 0xC4, 0x0C, 0xFF, 0x75, 0xE0, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x91, 0xE3, 0x81, 0xFF, 0x75, 0xBC},
};

PATCHINFO PI_CanSeeLoc1 =
{
  0x41908B,
  5, {0xE8, 0x15, 0x1D, 0x05, 0x00},
  5, {0xE8, 0x15, 0x1D, 0x05, 0x00},
};

PATCHINFO PI_CanSeeLoc2 =
{
  0x44FF86,
  5, {0xE8, 0x1A, 0xAE, 0x01, 0x00},
  5, {0xE8, 0x1A, 0xAE, 0x01, 0x00},
};

PATCHINFO PI_CanSeeLoc3 =
{
  0x46B26B,
  5, {0xE8, 0x35, 0xFB, 0xFF, 0xFF},
  5, {0xE8, 0x35, 0xFB, 0xFF, 0xFF},
};

PATCHINFO PI_CanSeeLoc4 =
{
  0x47E34F,
  5, {0xE8, 0x51, 0xCA, 0xFE, 0xFF},
  5, {0xE8, 0x51, 0xCA, 0xFE, 0xFF},
};

PATCHINFO PI_CanSeeLoc5 =
{
  0x4945BC,
  5, {0xE8, 0xE4, 0x67, 0xFD, 0xFF},
  5, {0xE8, 0xE4, 0x67, 0xFD, 0xFF},
};

PATCHINFO PI_CanSeeLoc6 =
{
  0x4950E4,
  5, {0xE8, 0xBC, 0x5C, 0xFD, 0xFF},
  5, {0xE8, 0xBC, 0x5C, 0xFD, 0xFF},
};

PATCHINFO PI_PacketHandler_PostReceive =
{
  0x47F138,
 21, {0x09, 0xC0, 0x89, 0x45, 0xFC, 0x0F, 0x84, 0xDB, 0x00, 0x00, 0x00, 0x90, 0x90, 0x90, 0x90, 0x83, 0x7D, 0xFC, 0xFF, 0x75, 0x0F},
 21, {0x50, 0x8B, 0x4D, 0xE8, 0xE8, 0x00, 0x00, 0x00, 0x00, 0x59, 0x01, 0xC8, 0x89, 0x45, 0xFC, 0x74, 0x0E, 0x90, 0x40, 0x75, 0x0F},
};

