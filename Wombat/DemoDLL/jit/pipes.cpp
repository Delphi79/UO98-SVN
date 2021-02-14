#include "pipes.hpp"

void CreatePipes(PipeHandles &Handles)
{
	SecurityAttributes TheSecurityAttributes;
	CreatePipe(Handles.output, TheSecurityAttributes); 
	CreatePipe(Handles.input, TheSecurityAttributes); 
	CreatePipe(Handles.error, TheSecurityAttributes); 
}

void CreatePipe(PipeHandlePair &PipePair, SecurityAttributes &TheSecurityAttributes)
{
	CreatePipe(&PipePair.Read, &PipePair.Write, TheSecurityAttributes.GetPointer(), 128 * 1024);
}

void SendTextMessageToPipe(HANDLE PipeHandle, LPCSTR Message)
{
	if(Message && *Message)
	{
		DWORD bytes_written = 0;
		WriteFile(PipeHandle, Message, strlen(Message), &bytes_written, NULL); 
	} 
}

LPSTR ReadAvailableDataFromPipeAsString(HANDLE PipeHandle)
{
	DWORD BytesAvailable = GetAmountOfDataAvailableOnPipe(PipeHandle);
	LPSTR StringData = ReadStringDataFromPipe(PipeHandle, BytesAvailable);
	return StringData;
}

DWORD GetAmountOfDataAvailableOnPipe(HANDLE PipeHandle)
{
	DWORD BytesToRead = 0;
	if(!PeekNamedPipe(PipeHandle, NULL, 0, NULL, &BytesToRead, NULL))
		return 0;
	return BytesToRead;
}

LPSTR ReadStringDataFromPipe(HANDLE PipeHandle, DWORD StringLength)
{
	LPSTR ReturnBuffer = new CHAR[StringLength + 1];
	if(ReturnBuffer)
	{
		DWORD BytesRead = ReadDataFromPipe(PipeHandle, (LPBYTE) ReturnBuffer, StringLength);
		ReturnBuffer[BytesRead] = 0;
	}
	return ReturnBuffer;
}

DWORD ReadDataFromPipe(HANDLE PipeHandle, LPBYTE Buffer, DWORD BytesToRead)
{
	DWORD BytesRead = 0;
	if(BytesToRead)
		ReadFile(PipeHandle, Buffer, BytesToRead, &BytesRead, NULL); 
	return BytesRead;
}