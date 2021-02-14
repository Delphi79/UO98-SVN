// dllmain.cpp : Defines the entry point for the DLL application.
// Creates console and calls interop Initialize.

#include "stdafx.h"
#include <stdio.h>
#include <fcntl.h>
#include <io.h>
#include <iostream>
#include <fstream>
#ifndef _USE_OLD_IOSTREAMS
using namespace std;
#endif

namespace NativeMethods
{
    void Initialize();
    void Uninitialize();

    #pragma unmanaged

    static const WORD MAX_CONSOLE_LINES = 5000;

    void RedirectIOToConsole()
    {
        int hConHandle;
        long lStdHandle;
        CONSOLE_SCREEN_BUFFER_INFO coninfo;
        FILE *fp;
        // allocate a console for this app
        AllocConsole();
        SetConsoleTitle("UoDemo+ DLL by Batlin");

        // set the screen buffer to be big enough to let us scroll text
        GetConsoleScreenBufferInfo(GetStdHandle(STD_OUTPUT_HANDLE), &coninfo);
        coninfo.dwSize.Y = MAX_CONSOLE_LINES;
        SetConsoleScreenBufferSize(GetStdHandle(STD_OUTPUT_HANDLE), coninfo.dwSize);

        // redirect unbuffered STDOUT to the console
        lStdHandle = (long)GetStdHandle(STD_OUTPUT_HANDLE);
        hConHandle = _open_osfhandle(lStdHandle, _O_TEXT);
        fp = _fdopen( hConHandle, "w" );
        *stdout = *fp;
        setvbuf( stdout, NULL, _IONBF, 0 );

        // redirect unbuffered STDIN to the console
        lStdHandle = (long)GetStdHandle(STD_INPUT_HANDLE);
        hConHandle = _open_osfhandle(lStdHandle, _O_TEXT);
        fp = _fdopen( hConHandle, "r" );
        *stdin = *fp;
        setvbuf( stdin, NULL, _IONBF, 0 );

        // redirect unbuffered STDERR to the console
        lStdHandle = (long)GetStdHandle(STD_ERROR_HANDLE);
        hConHandle = _open_osfhandle(lStdHandle, _O_TEXT);
        fp = _fdopen( hConHandle, "w" );
        *stderr = *fp;
        setvbuf( stderr, NULL, _IONBF, 0 );

        // make cout, wcout, cin, wcin, wcerr, cerr, wclog and clog 
        // point to console as well
        ios::sync_with_stdio();
    }

    bool GetEnvToggle(const char* key)
    {
        char * pValue;
        pValue = getenv(key);

        return pValue!=NULL && *pValue!=0  && (_stricmp(pValue,"YES")==0 || _stricmp(pValue,"TRUE")==0);
    }

}

using namespace NativeMethods;

BOOL APIENTRY DllMain( HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
            {
                //bridge_Initialize();
                // Tell Windows that we are not interested in thread events
                DisableThreadLibraryCalls(hModule);

                // Create console to redirect unbuffered STDOUT
                if (!GetEnvToggle("NOCONSOLE")) RedirectIOToConsole();

                puts("Welcome to the UO:98 Console!");
                puts(" http://joinuo.com | http://uo98.org");

                Initialize();

                break;
            }
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
            break;
        case DLL_PROCESS_DETACH:
            Uninitialize();
            break;
    }
    return TRUE;
}

