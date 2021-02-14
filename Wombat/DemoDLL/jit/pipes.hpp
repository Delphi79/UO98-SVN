#ifndef __PIPES__
#define __PIPES__

#include "security_attributes.hpp"

// Pipe-stuff
typedef struct
{
	HANDLE Read;
	HANDLE Write;
} PipeHandlePair;

typedef struct
{
	PipeHandlePair input; 
	PipeHandlePair output;
	PipeHandlePair error;
} PipeHandles;

void CreatePipes(PipeHandles &Handles);
void CreatePipe(PipeHandlePair &PipePair, SecurityAttributes &TheSecurityAttributes);

void SendTextMessageToPipe(HANDLE PipeHandle, LPCSTR Message);
DWORD GetAmountOfDataAvailableOnPipe(HANDLE PipeHandle);
DWORD ReadDataFromPipe(HANDLE PipeHandle, LPBYTE Buffer, DWORD BytesToRead);
LPSTR ReadStringDataFromPipe(HANDLE PipeHandle, DWORD StringLength);
LPSTR ReadAvailableDataFromPipeAsString(HANDLE PipeHandle);

#endif