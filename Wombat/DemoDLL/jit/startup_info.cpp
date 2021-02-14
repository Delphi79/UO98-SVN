#include "startup_info.hpp"

ProcessStartupInfo::ProcessStartupInfo(const PipeHandles &Handles)
{
	ZeroMemory(&startup_info, sizeof(STARTUPINFO)); 
	startup_info.cb = sizeof(STARTUPINFO); 
	startup_info.hStdInput = Handles.input.Read; 
	startup_info.hStdOutput = Handles.output.Write; 
	startup_info.hStdError = Handles.error.Write; 
	startup_info.dwFlags = STARTF_USESTDHANDLES; 
}

LPSTARTUPINFO ProcessStartupInfo::GetPointer()
{
	return &startup_info;
}
