#include "uosl.hpp"
#include <sys/stat.h>

#include "process.hpp"

static bool fexist(const char *path)
{
  struct stat buffer;
  return stat(path, &buffer) == 0;
}

static char *GetPathToUOSL()
{
	char *path = getenv("UOSL");
	if(!path)
		path = ".\\uosl.exe";	
	return path;
}

static char *GetValidatedPathToUOSL()
{
	char *path = GetPathToUOSL();
	if(fexist(path))
		return path;
	return NULL;
}

InterfaceToUOSL::InterfaceToUOSL()
{
	PathToUOSL = GetValidatedPathToUOSL();
}

const char *InterfaceToUOSL::GetPath()
{
	return PathToUOSL;
}

bool InterfaceToUOSL::IsActive()
{
	return PathToUOSL != NULL;
}

char *InterfaceToUOSL::TranslateScript(const char *Input)
{
	PipeHandles Handles;
	CreatePipes(Handles);
	if(CreateProcessAndWaitForItToEnd(Handles, PathToUOSL, "-detail error -outspec Extended", Input, "\r\n\x0C\r\n"))
	{
		CHAR *OutputString = ReadAvailableDataFromPipeAsString(Handles.output.Read);
		CHAR *ErrorString = ReadAvailableDataFromPipeAsString(Handles.error.Read);
		return *ErrorString ? LPSTR(int(ErrorString) | 0x80000000) : OutputString;
	}
	return NULL;
}