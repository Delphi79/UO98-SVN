#ifndef __PROCESS__
#define __PROCESS__

#include "pipes.hpp"

void WaitForProcessToEnd(HANDLE ProcessHandle);
void WaitForProcessToEnd(PROCESS_INFORMATION &process_info);
void WaitForProcessToEnd(PROCESS_INFORMATION &process_info);
bool CreateProcess(PROCESS_INFORMATION &process_info, PipeHandles &Handles, LPSTR CommandLine);
bool CreateProcess(PROCESS_INFORMATION &process_info, PipeHandles &Handles, LPCSTR CommandLine, LPCSTR Parameters);
bool CreateProcessAndWaitForItToEnd(PipeHandles &Handles, LPCSTR CommandLine, LPCSTR Parameters, LPCSTR InputToSend1, LPCSTR InputToSend2);

#endif __PROCESS__