#include <windows.h>
#include <stdio.h>

#pragma warn -8057

#include "patcher.c"
#include "patches.c"
  
#include "classes.c"
#include "TimeManager.h"

#include "Mobile.c"
#include "Player.c"
#include "Weapon.c"

typedef void (__cdecl *SCOMMAND_FUNC)(PlayerObject *Player, unsigned int PlayerSerial, const char *Command, int AlwaysMinus1);

// DO NOT CHANGE THE DEFINITION OF THIS STRUCTURE, DOING SO WILL CRASH THE CORE
typedef struct SCOMMAND_DEF
{
  const char *Command;
  SCOMMAND_FUNC Function;
} SCOMMAND_DEF;

// NOTE: it's your responsibility to avoid buffer-overruns in TargetBuffer!
//       the demo is old-school and doesn't handle those well :)
const char *ExtractString_HandleQuotes(const char *Command, char *TargetBuffer)
{
  __asm
  {
    // Time to go inside uodemo.exe and call the function that does the real magic
    // (this is easier than reimplementing our own ExtractString function)
    // (but then again, by implementing our own we could get rid of the buffer-overrun problem)
    push TargetBuffer
    push Command
    mov eax, 0x4475AB
    call eax
    add esp, 8
  }
  return (const char*) _EAX;
}

void AddScript(unsigned int Serial, const char *Script)
{
  __asm
  {
    lea edx, Script
    push edx
    push Serial
    mov eax, 0x4103EA
    call eax
    add esp, 8
  }
}

void __cdecl Test(PlayerObject *Player, unsigned int PlayerSerial, const char *Command, int AlwaysMinus1)
{
  SendSystemMessage(Player, "doSCommand(...): Test OK!");
}

void __cdecl MakeCounselor(PlayerObject *Player, unsigned int PlayerSerial, const char *Command, int AlwaysMinus1)
{
  char buffer[512];

  PlayerObject *Target;
  unsigned int TargetSerial;
  int Type;

  Command = ExtractString_HandleQuotes(Command, buffer);
  if(strcmpi(buffer, "me") == 0)
  {
    TargetSerial = PlayerSerial;
  }
  else
  {
    TargetSerial = (unsigned int) atoi(buffer);
  }
  if(TargetSerial == 0)
  {
    SendSystemMessage(Player, "Usage: coun me/<serial> [type]!");
    return;
  }

  /*Command =*/ ExtractString_HandleQuotes(Command, buffer);
  Type = atoi(buffer);

  Target = ConvertSerialToObject(TargetSerial);
  if((Target == NULL) || !IsPlayerObject(Target))
  {
    SendSystemMessage(Player, "coun: You must target a player!");
    return;
  }

  __asm
  {
    // Time to go inside uodemo.exe and call the function that does the real magic
    push Type
    push Target
    mov ecx, 0x6982D8
    mov eax, 0x44E039
    call eax
  }

  if(Target != Player)
  {
    SendSystemMessage(Target, "You've become a counselor!");
    SendSystemMessage(Player, "You've made him/her a counselor!");
  }
  else
    SendSystemMessage(Player, "You've made yourself a counselor!");
}

void __cdecl UnmakeCounselor(PlayerObject *Player, unsigned int PlayerSerial, const char *Command, int AlwaysMinus1)
{
  char buffer[512];

  PlayerObject *Target;
  unsigned int TargetSerial;

  /*Command =*/ ExtractString_HandleQuotes(Command, buffer);
  if(strcmpi(buffer, "me") == 0)
  {
    TargetSerial = PlayerSerial;
  }
  else
  {
    TargetSerial = (unsigned int) atoi(buffer);
  }
  if(TargetSerial == 0)
  {
    SendSystemMessage(Player, "Usage: ucoun me/<serial>!");
    return;
  }

  Target = ConvertSerialToObject(TargetSerial);
  if((Target == NULL) || !IsPlayerObject(Target))
  {
    SendSystemMessage(Player, "ucoun: You must target a player!");
    return;
  }

  __asm
  {
    // Time to go inside uodemo.exe and call the function that does the real magic
    push Target
    mov ecx, 0x6982D8
    mov eax, 0x44DEE6
    call eax
  }

  if(Target != Player)
  {
    SendSystemMessage(Target, "You've lost your counselor powers!");
    SendSystemMessage(Player, "You've unmade him/her a counselor!");
  }
  else
    SendSystemMessage(Player, "You've unmade yourself a counselor!");
}

void __cdecl SaveWorldState(PlayerObject *Player, unsigned int PlayerSerial, const char *Command, int AlwaysMinus1)
{
  SendSystemMessage(Player, "Saving world...");

  __asm
  {
    // Time to go inside uodemo.exe and call the function that does the real magic
    mov eax, 0x4C8A5C
    call eax
  }

  SendSystemMessage(Player, "World saved!");
}

void __cdecl ShutdownServer(PlayerObject *Player, unsigned int PlayerSerial, const char *Command, int AlwaysMinus1)
{
  __asm
  {
    // Time to go inside uodemo.exe and call the function that does the real magic
    mov edx, 0x6999E0
    mov dword ptr [edx], 1
  }
}

// This function is and must be declared naked!!!
// Do not modify this function unless you know enough assembler
// And also unless you know deep down in your heart what "naked" functions are
int __declspec(naked) AllowCounselorSpeechEvent()
{
  __asm
  {
    // Return 0 if we are not a counselor
    mov eax, [ecx + 0x3A8]
    and eax, 0x40000
    mov edx, [ebp + 0x000000FC]
    jz done
    // Return 0 if we are a counselor and the speech does not start with "[" or "]"
    cmp byte ptr [edx], '['
    mov eax, 0
    je good
    cmp byte ptr [edx], ']'
    jne done
good:
    // Return 1 if we are a counselor and the speech starts with "[" or "]"
    inc eax
done:
    ret
  }
  return _EAX; // Gets ignored but satifies the compiler
}

SCOMMAND_DEF MySCommandTable[] = 
{
  {"test", Test},
  {"coun", MakeCounselor},
  {"ucoun", UnmakeCounselor},
  {"save", SaveWorldState},
  {"shutdown", ShutdownServer},
};

PATCHINFO PI_AllowCounselorSpeechEvent =
{
 0x493C49,
 5, {0xE8, 0x96, 0x2A, 0xFC, 0xFF},
 5, {0xE8, 0x00, 0x00, 0x00, 0x00},
};

// ****************************************
// ****************************************
// DllEntryPoint -> EDIT WITH CARE!!!
//
// This is the code called when uodemo+.exe
// executes LoadLibrary to load this DLL.
//
// The patches will be set-up and applied.
//
// Do not edit the SetAbs32_XXX calls!
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
    SetAbs32_AtRelPatch(&PI_SCommandTable_C1, 3, (void *) &MySCommandTable[0].Command);
    SetAbs32_AtRelPatch(&PI_SCommandTable_C2, 3, (void *) &MySCommandTable[0].Command);
    SetAbs32_AtRelPatch(&PI_SCommandTable_F1, 3, (void *) &MySCommandTable[0].Function);
    SetAbs32_AtRelPatch(&PI_SCommandTable_F2, 3, (void *) &MySCommandTable[0].Function);
    SetRel32_AtPatch(&PI_AllowCounselorSpeechEvent, AllowCounselorSpeechEvent);

    // Apply the patches
    Patch(&PI_SCommandTable_C1);
    Patch(&PI_SCommandTable_C2);
    Patch(&PI_SCommandTable_F1);
    Patch(&PI_SCommandTable_F2);
    Patch(&PI_AllowCounselorSpeechEvent);
    
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
