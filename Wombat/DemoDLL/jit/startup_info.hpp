#ifndef __STARTUP_INFO__
#define __STARTUP_INFO__

#include "pipes.hpp"

class ProcessStartupInfo
{
private:
	STARTUPINFO startup_info;

public:
	ProcessStartupInfo(const PipeHandles &Handles);
	LPSTARTUPINFO GetPointer();
};

#endif