#include "process.hpp"
#include "startup_info.hpp"

bool CreateProcess(PROCESS_INFORMATION &process_info, PipeHandles &Handles, LPSTR CommandLine)
{
	ProcessStartupInfo StartupInfo(Handles);
	return CreateProcess(NULL, (LPSTR) CommandLine, NULL, NULL, TRUE, 0, NULL, NULL, StartupInfo.GetPointer(), &process_info) != FALSE;
}

bool CreateProcess(PROCESS_INFORMATION &process_info, PipeHandles &Handles, LPCSTR CommandLine, LPCSTR Parameters)
{
	if(Parameters == NULL)
		Parameters = "";

	const int FullCommandLineLength = 1 + strlen(CommandLine) + 2 + strlen(Parameters);
	
	LPSTR FullCommandLine = new CHAR[FullCommandLineLength + 1];
	if(FullCommandLine == NULL)
		return false;
	strcpy(FullCommandLine, "\"");
	strcat(FullCommandLine, CommandLine);
	strcat(FullCommandLine, "\" ");
	strcat(FullCommandLine, Parameters);
	
	bool result = CreateProcess(process_info, Handles, FullCommandLine);
	
	delete [] FullCommandLine;
	
	return result;
}

bool CreateProcessAndWaitForItToEnd(PipeHandles &Handles, LPCSTR CommandLine, LPCSTR Parameters, LPCSTR InputToSend1, LPCSTR InputToSend2)
{
	PROCESS_INFORMATION process_info;
	if(!CreateProcess(process_info, Handles, CommandLine, Parameters))
		return false;
	SendTextMessageToPipe(Handles.input.Write, InputToSend1);	
	SendTextMessageToPipe(Handles.input.Write, InputToSend2);
	FlushFileBuffers(Handles.input.Write);
	WaitForProcessToEnd(process_info.hProcess);
	return true;
}

void WaitForProcessToEnd(HANDLE ProcessHandle)
{
	WaitForSingleObject(ProcessHandle, INFINITE);
}
