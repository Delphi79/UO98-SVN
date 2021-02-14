#include <windows.h>
#include <stdio.h>

#pragma warn -8057

// ****************************************
// ****************************************
// DllEntryPoint -> EDIT WITH CARE!!!
//
// This is the code called when uodemo+.exe
// executes LoadLibrary to load this DLL.
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
