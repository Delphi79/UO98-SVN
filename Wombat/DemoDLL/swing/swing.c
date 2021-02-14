#include <windows.h>
#include <stdio.h>

#include "patcher.c"
#include "patches.c"

#include "classes.h"
#include "TimeManager.h"

#include "Mobile.c"
#include "Player.c"
#include "Weapon.c"

// ****************************************
// ****************************************
// EnableXXX variables -> FEEL FREE TO PLAY
//
// These variables with somehow change the
// behaviour of the generated DLL.
//
// Enjoy editing and playing with them...
// ****************************************
// ****************************************

int EnableInstaHit = 1;

// The following 3 settings will only work if Insta-Hit is disabled
int EnableEquipLogging = 1;
int EnableSwingAdvancementLogging = 1;
int EnableSwingTestLogging = 0;

unsigned int Swing_LastMobile = 0; // Helper variable, set by us in AdvanceSwingState

// ****************************************
// ****************************************
// OriginalXXX functions -> EDIT WITH CARE!
//
// These functions are a decompiled version
// of functions found in uodemo.exe.
//
// You may edit them but do so with care,
// so you do not disturb the original code
// flow of these core functions.
// ****************************************
// ****************************************

int LogAndCallAdvanceSwingState(unsigned int THIS_Mobile) // Alternative version of OriginalAdvanceSwingState for logging only
{
  int OldState, NewState;

  unsigned int WeaponInHand  = GetWeaponInHand(THIS_Mobile);
  unsigned int SwingSpeed    = GetSwingSpeed(THIS_Mobile);
  unsigned int SwingDuration = SwingSpeed * 2;

  OldState = GetSwingState(THIS_Mobile);
  NewState = AdvanceSwingState(THIS_Mobile);

  printf("M=%08X, W=%08X, Speed=%u, Duration=%u, State=%u, Counter=%u, NewState=%u\n", THIS_Mobile, WeaponInHand, SwingSpeed, SwingDuration, OldState, GetSwingCounter(THIS_Mobile), NewState);

  return NewState;
}

int OriginalAdvanceSwingState(unsigned int THIS_Mobile)
{
  int CompareNewState;

  int OldState, NewState;
  int DurationFirstState, DurationSecondState;

  int Compare = 1;

  unsigned int WeaponInHand  = GetWeaponInHand(THIS_Mobile);  
  int SwingSpeed    = GetSwingSpeed(THIS_Mobile);
  int SwingDuration = SwingSpeed * 2;
  int SwingCounter  = GetSwingCounter(THIS_Mobile);

  int AnimationDuration = WeaponInHand != 0 && IsRanged(WeaponInHand) ? 4 : 6;  

  DurationSecondState = SwingDuration > AnimationDuration ? SwingDuration - AnimationDuration : 0;
  DurationFirstState  = DurationSecondState > 4 ? DurationSecondState - 4 : 0;

  OldState = GetSwingState(THIS_Mobile);
  NewState = OldState;
  switch(OldState)
  {
    case 0:
    {
      if(SwingCounter >= DurationFirstState)
      {
        SwingCounter = DurationFirstState;
        NewState = 1;
      }
      break;
    }
    case 1:
    {
      if(SwingCounter >= DurationSecondState)
      {
        SwingCounter = DurationSecondState;
        NewState = 2;
      }
      break;
    }
    default:
    {
      if(SwingCounter > SwingDuration)
      {
        SwingCounter = 0;
        NewState = 3;
      }
    }
  }

  if(Compare)
  {
    // Execute the original code
    CompareNewState = AdvanceSwingState(THIS_Mobile);

    // Compare
    if((SwingCounter != GetSwingCounter(THIS_Mobile)) || (NewState != CompareNewState))
    {
      // Report error
      printf("SWING CODING ERROR! (WE versus CORE) Counter: %d versus %d & State: %d ? %d\n", SwingCounter, GetSwingCounter(THIS_Mobile), NewState, CompareNewState);

      // Fix
//    SwingCounter = GetSwingCounter(THIS_Mobile); // Not really needed here
      NewState     = CompareNewState;              // Important! We will return NewState to the caller
    }
  }
  else
  {
    // Set
    SetSwingCounter(THIS_Mobile, SwingCounter);
    SetSwingState(THIS_Mobile, NewState == 3 ? 0 : NewState);
  }

  return NewState;
}

void OriginalResetSwingCounter(unsigned int Weapon, unsigned int Mobile, char TargetEquipSlot)
{
  // Don't forget to reset! :-)
  SetSwingCounter(Mobile, 0);  
}

int OriginalIsActiveSwing(int OldState, int NewState)
{
  if(EnableSwingTestLogging)
    printf("IsActiveSwing(OldState=%d, NewState=%d) -> %d\n", OldState, NewState, OldState != NewState && NewState >= 2);
  return OldState != NewState && NewState >= 2;
}

int OriginalIsAmmoCheck(int NewState)
{
  if(EnableSwingTestLogging)
    printf("IsAmmoCheck(NewState=%d) -> %d\n", NewState, NewState == 2);
  return NewState == 2;
}

int OriginalIsAnimation(int NewState)
{
  if(EnableSwingTestLogging)
    printf("IsAnimation(NewState=%d) -> %d\n", NewState, NewState == 2);
  return NewState == 2;
}

int OriginalIsHit(int NewState)
{
  if(EnableSwingTestLogging)
    printf("IsHit(NewState=%d) -> %d\n", NewState, NewState != 2);
  return NewState != 2;
}

// ****************************************
// ****************************************
// InstaHitXXX functions -> EDIT WITH CARE!
//
// These functions are a reimplementation
// of the OriginalXXX functions to try to
// discover the correct implementation of
// Insta-Hit.
//
// You may edit them but do so with care,
// so you do not mess up the original code
// flow of the core functions.
// ****************************************
// ****************************************

int InstaHitAdvanceSwingState(unsigned int THIS_Mobile)
{
  int OldState, NewState;
  int DurationFirstState, DurationSecondState;

  unsigned int WeaponInHand  = GetWeaponInHand(THIS_Mobile);  
  int SwingDuration = GetSwingSpeed(THIS_Mobile) * 2;
  int SwingCounter  = GetSwingCounter(THIS_Mobile);

  int AnimationDuration = WeaponInHand != 0 && IsRanged(WeaponInHand) ? 4 : 6;  
  DurationFirstState  = AnimationDuration;
  DurationSecondState = SwingDuration > AnimationDuration ? SwingDuration - AnimationDuration : 0;

  OldState = GetSwingState(THIS_Mobile);
  NewState = OldState;
  switch(OldState)
  {
    case 0:
    {
      if(SwingCounter >= DurationFirstState)
      {
        SwingCounter = DurationFirstState;
        NewState = 1;
      }
      break;
    }
    case 1:
    {
      if(SwingCounter >= DurationSecondState)
      {
        SwingCounter = DurationSecondState;
        NewState = 2;
      }
      break;
    }
    default:
    {
      if(SwingCounter > SwingDuration)
      {
        SwingCounter = 0;
        NewState = 3;
      }
    }
  }

  // Set
  SetSwingCounter(THIS_Mobile, SwingCounter);
  SetSwingState(THIS_Mobile, NewState == 3 ? 0 : NewState);

  return NewState;
}

void InstaHitResetSwingCounter(unsigned int Weapon, unsigned int Mobile, char TargetEquipSlot)
{
  int Speed = 0;

  //Get speed from the mobile (dexterity or fatigue based)
  //Speed = GetSwingSpeed(Mobile) / 3;

  // Get fixed speed based on the weapon
  Speed = GetWeaponSpeed(Weapon);
  if(Speed > 0)
    Speed = 100 / Speed;

  // Reset
  SetSwingCounter(Mobile, -Speed);
}

int InstaHitIsActiveSwing(int OldState, int NewState)
{
  return OldState != NewState && NewState <= 2;
}

int InstaHitIsAmmoCheck(int NewState)
{
  return NewState == 1;
}

int InstaHitIsAnimation(int NewState)
{
  char buffer[256];

  sprintf(buffer, "Is Animation: %u, NewState(%d) == 0 -> %s", GetPulseNum(), NewState, NewState == 0 ? "yes" : "no");

  SendSystemMessage(Swing_LastMobile, buffer);

  return NewState == 1;
}

int InstaHitIsHit(int NewState)
{
  char buffer[256];

  sprintf(buffer, "Is Hit: %u, NewState(%d) == 0 -> %s", GetPulseNum(), NewState, NewState == 0 ? "yes" : "no");

  SendSystemMessage(Swing_LastMobile, buffer);

  return NewState == 1;
}

// ****************************************
// ****************************************
// MyXXX functions -> DO NOT EDIT!!!
//
// These are called directly from the core
// if the related patches are applied.
//
// Do not modify them unless you know
// assembler and realize the consequences.
// ****************************************
// ****************************************

int _stdcall MyAdvanceSwingState(void)
{
  int NewState; // Do NOT add any assignment here!!!

  // This must be the (coded) first line!!! UoDemo = thiscall!!!
  GETTHIS(THIS_Mobile);

  // Can be used by the other Swing Functions (except for ResetSwingCounter!)
  Swing_LastMobile = THIS_Mobile;

  // NOTE: use either LogAdvanceSwingState or OriginalAdvanceSwingState or InstaHitAdvanceSwingState
  //       make sure you don't update the state multiple times! 

  if(EnableInstaHit)
    NewState = InstaHitAdvanceSwingState(THIS_Mobile);
  else if(EnableSwingAdvancementLogging)
    NewState = LogAndCallAdvanceSwingState(THIS_Mobile);
  else
    NewState = OriginalAdvanceSwingState(THIS_Mobile);

  return NewState;
}

void _stdcall MyResetSwingCounter(unsigned int Mobile, char TargetEquipSlot)
{
  // This must be the first line!!! UoDemo = thiscall!!!
  GETTHIS(THIS_Weapon);

  // To avoid spam, we will only report equip changes for players!
  if(EnableEquipLogging && IsPlayerObject(Mobile))
  {
    // Log
    if(TargetEquipSlot == 0)
      printf("%08X lifted weapon %08X\n", Mobile, THIS_Weapon);
    else
      printf("%08X equiped weapon %08X in slot %d\n", Mobile, THIS_Weapon, TargetEquipSlot);
  }

  if(EnableInstaHit)
    InstaHitResetSwingCounter(THIS_Weapon, Mobile, TargetEquipSlot);
  else
    OriginalResetSwingCounter(THIS_Weapon, Mobile, TargetEquipSlot);

  // Log
  if(EnableEquipLogging && IsPlayerObject(Mobile))
    printf("-> Template=%u Speed=%u SwingCounter=%d\n", GetWeaponTemplate(THIS_Weapon), GetWeaponSpeed(THIS_Weapon), GetSwingCounter(Mobile));
}

int _stdcall MyIsActiveSwing(int OldState, int NewState)
{
  if(EnableInstaHit)
    return InstaHitIsActiveSwing(OldState, NewState);
  else
    return OriginalIsActiveSwing(OldState, NewState);
}

int _stdcall MyIsAmmoCheck(int NewState)
{
  if(EnableInstaHit)
    return InstaHitIsAmmoCheck(NewState);
  else
    return OriginalIsAmmoCheck(NewState);
}

int _stdcall MyIsAnimation(int NewState)
{
  if(EnableInstaHit)
    return InstaHitIsAnimation(NewState);
  else
    return OriginalIsAnimation(NewState);
}

int _stdcall MyIsHit(int NewState)
{
  if(EnableInstaHit)
    return InstaHitIsHit(NewState);
  else
    return OriginalIsHit(NewState);
}

// ****************************************
// ****************************************
// DllEntryPoint -> EDIT WITH CARE!!!
//
// This is the code called when uodemo+.exe
// executes LoadLibrary to load this DLL.
//
// The patches will be set-up and applied
//
// Do not edit the SetRel32_XXX calls!
// Feel free to fool around with the rest
// but at your own risk.
// ****************************************
// ****************************************

BOOL WINAPI DllEntryPoint(HINSTANCE hInstance, ULONG ulReason, LPVOID pv)
{
  HANDLE hOutput;

  if(ulReason == DLL_PROCESS_ATTACH)
  {
    // Tell Windows that we are not interested in thread events
    DisableThreadLibraryCalls(hInstance);

    // Create a console which we can use for debug output
    AllocConsole();
    hOutput = CreateFile("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, NULL, OPEN_EXISTING, 0, NULL);
    SetStdHandle(STD_OUTPUT_HANDLE, hOutput);
    SetConsoleTitle("UoDemo+ DLL by Batlin");

    puts("Welcome to the Demo Cheat Console!");

    // Prepare the patches
    SetRel32_AtPatch(&PI_ResetSwingState_CallAdvanceSwingState, MyAdvanceSwingState);
    SetRel32_AtPatch(&PI_CombatHeartBeat_CallAdvanceSwingState, MyAdvanceSwingState);
    SetRel32_AtRelPatch(&PI_OnEquipUnequip_ResetSwingCounter, 9, MyResetSwingCounter);
    SetRel32_AtRelPatch(&PI_CombatHeartBeat_IsAmmoCheck_TakeControl, 10, MyIsAmmoCheck);
    SetRel32_AtRelPatch(&PI_CombatHeartBeat_IsActiveSwing_TakeControl, 7, MyIsActiveSwing);
    SetRel32_AtRelPatch(&PI_CombatHeartBeat_IsAnimationOrHit_TakeCombinedControl,  8, MyIsAnimation);
    SetRel32_AtRelPatch(&PI_CombatHeartBeat_IsAnimationOrHit_TakeCombinedControl, 73, MyIsHit);

    // Apply the patches
    Patch(&PI_CombatHeartBeat_CallAdvanceSwingState);
    Patch(&PI_ResetSwingState_CallAdvanceSwingState);
    Patch(&PI_OnEquipUnequip_ResetSwingCounter);
    Patch(&PI_CombatHeartBeat_IsAmmoCheck_TakeControl);
    Patch(&PI_CombatHeartBeat_IsActiveSwing_TakeControl);
    Patch(&PI_CombatHeartBeat_IsAnimationOrHit_TakeCombinedControl);

    // Optional patches for insta-hit
    if(EnableInstaHit)
    {
      Patch(&PI_SetFatigueBasedSwing); 
      Patch(&PI_SetSwingDividendTo60000);
    }
    
    return TRUE;
  }
  if(ulReason == DLL_PROCESS_DETACH)
  {
    // At this point we could undo the patches made.
    // But I'm lazy and don't care
    return TRUE;
  }

  return FALSE;
}
