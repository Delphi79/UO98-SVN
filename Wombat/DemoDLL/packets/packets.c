#include <windows.h>
#include <stdio.h>

#pragma warn -8057
#pragma warn -8004
#pragma warn -8019

#include "patcher.c"
#include "patches.c"
  
#include "classes.c"
#include "TimeManager.h"

#include "Mobile.c"
#include "Player.c"
#include "Weapon.c"

int IsDynamicSizedPacket(unsigned char PacketID)
{
  // NOTE: the demo supports only up to 0xB5, any value above will cause invalid results!
  __asm
  {
    mov al, PacketID
    mov ecx, 0x47F320
    push eax
    call ecx
    pop ecx
  }
  return _EAX & 0x8000 ? 1 : 0;
}

unsigned int GetFixedPacketSize(unsigned char PacketID)
{
  // NOTE: the demo supports only up to 0xB5, any value above will cause invalid results!
  __asm
  {
    movzx eax, PacketID
    imul eax, 0x0A
    mov ax, [eax + 0x61F138]
  }
  return _EAX;
}

unsigned char *ExtractPacket(unsigned char *Data, unsigned int PacketSize)
{
  unsigned char *buffer = (unsigned char *) malloc(PacketSize);
  if(buffer == NULL)
    ; // CRASH!
  memcpy(buffer, Data, PacketSize);
  return buffer;
}

void RemovePacket(unsigned char *Data, unsigned int PacketSize, unsigned int *pTotalDataSize)
{
  *pTotalDataSize -= PacketSize;
  memmove(Data, Data + PacketSize, *pTotalDataSize);
}

void PrependEmptyPacket(unsigned char PacketID, unsigned char *Data, unsigned int PacketSize, unsigned int *pTotalDataSize)
{
  memmove(Data + PacketSize, Data, *pTotalDataSize);
  *pTotalDataSize += PacketSize;
  *Data = PacketID;
}

void HandleOutsideRangePacket(void *Socket)
{
  const unsigned int MaxBufferSize  = 0x10000;
  unsigned int      *pTotalDataSize = (unsigned int *) ((char *) Socket + 0x1002C);
  unsigned char     *Data           = (unsigned char *) Socket + 0x28;
  unsigned int       SocketNumber   = * (unsigned int *) ((char *) Socket + 0x0C);
  unsigned char      PacketID       = *Data;

  if(PacketID == 0xB6)
  {
    printf("%08X: Removing packet 0xB6\n", SocketNumber);

    // This packet is unknown by the demo, therefor remove it from the buffer
    // And replace by 0xA4 (System Information), which is handled and ignored safely by the demo server
    RemovePacket(Data, 9, pTotalDataSize);
    PrependEmptyPacket(0xA4, Data, 149, pTotalDataSize);
  }
  else
    printf("%08X: Invalid packet, client will most likely crash!\n", SocketNumber);
}

/*
  WARNING! When this function exits you must make sure atleast one valid packet is inside the buffer!
*/
void * __cdecl PreviewPacket(void *Socket)
{
  const unsigned int MaxBufferSize  = 0x10000;
  unsigned int      *pTotalDataSize = (unsigned int *) ((char *) Socket + 0x1002C);
  unsigned char     *Data           = (unsigned char *) Socket + 0x28;
  unsigned int       SocketNumber   = * (unsigned int *) ((char *) Socket + 0x0C);

  unsigned char *PacketCopy = NULL;
  unsigned char PacketID    = *Data;
  int PacketIsWithinRange   = PacketID <= 0xB5;
  int IsPacketDynamicSized  = PacketIsWithinRange ? IsDynamicSizedPacket(PacketID) : 0;
  unsigned int PacketSize   = PacketIsWithinRange ? ( IsPacketDynamicSized ? ntohs(*(unsigned short *)(Data + 1)) : GetFixedPacketSize(PacketID) ) : 1;
  int IsPacketComplete      = IsPacketDynamicSized ? (*pTotalDataSize >= 3 && PacketSize <= *pTotalDataSize) : PacketSize <= *pTotalDataSize;
  int IsPacketValid         = PacketIsWithinRange && IsPacketComplete;
  int IsOnlyPacketInBuffer  = *pTotalDataSize == PacketSize;
  unsigned int SizeRemainingInBuffer = MaxBufferSize - *pTotalDataSize;

  printf("%08X: ID=%02X, PS=%3u, TDS=%3u, IDS=%d, ICP=%d, OPB=%d, RS=%u, IPV=%d\n", SocketNumber, PacketID, *pTotalDataSize, PacketSize, IsPacketDynamicSized, IsPacketComplete, IsOnlyPacketInBuffer, SizeRemainingInBuffer, IsPacketValid);

  if(!IsPacketValid)
  {
    if(!PacketIsWithinRange)
      HandleOutsideRangePacket(Socket);
    return (void *) Data;
  }

  if(PacketID == 0xAD)
  {
    int c;
    int SpeechLength = PacketSize - 12;
    if(SpeechLength < 0)
      SpeechLength = 0;
    else
      SpeechLength /= 2;

    // I am unsure of the packet format in client 1.25
    if(GetUByte(Data, 3) == 0xC0)
      printf("%08X: WARNING! Unsupported 0xAD mode\n", SocketNumber);
    else
      printf("%08X: Converting 0xAD to 0x03\n", SocketNumber);

    // Convert UNICODE speech to ASCII speech
    PacketCopy = ExtractPacket(Data, PacketSize);
    RemovePacket(Data, PacketSize, pTotalDataSize);
    PrependEmptyPacket(0x03, Data, 8 + SpeechLength, pTotalDataSize);
    SetUWord(Data, 1, htons(SpeechLength + 8)); // PacketSize
    SetUByte(Data, 3, GetUByte(PacketCopy, 3)); // Mode
    SetUWord(Data, 4, GetUWord(PacketCopy, 4)); // TextColor
    SetUWord(Data, 6, GetUWord(PacketCopy, 6)); // Font
    for(c = 0; c < SpeechLength; c ++)
      SetSByte(Data, 8 + c, GetSByte(PacketCopy, 12 + 1 + c * 2));
  }

  if(PacketCopy != NULL)
    free(PacketCopy);

  return (void *) Data;
}

/*
  WARNING! When this function exits you must make sure atleast one valid packet is inside the buffer!
*/
void * __cdecl PreviewAllPackets(void *Socket)
{
  const unsigned int MaxDataSize    = 0x10000;
  unsigned int      *pTotalDataSize = (unsigned int *) ((char *) Socket + 0x1002C);
  unsigned char     *Data           = (unsigned char *) Socket + 0x28;

  unsigned char FirstPacketID            = *Data;
  int           FirstPacketIsWithinRange = FirstPacketID <= 0xB5;

  // This is important!
  // Due to a bug in the server code, the demo will not really handle packets starting 0xB6 and above
  // NOTE: only the first packet will cause a crash, all other invalid packets are handled later better the code
  if(!FirstPacketIsWithinRange)
    HandleOutsideRangePacket(Socket);

  return (void *) Data;
}

PATCHINFO PI_Packets_ChangePacketFunction =
{
 0x47EB52,
 9, {0x8B, 0x4D, 0xFC, 0x51, 0xE8, 0xE5, 0x07, 0x00, 0x00},
 9, {0x8B, 0x4D, 0x08, 0x51, 0xE8, 0x00, 0x00, 0x00, 0x00},
};

PATCHINFO PI_Packets_ChangeAllPacketsFunction =
{
 0x47F18B,
 9, {0x8B, 0x4D, 0xF8, 0x51, 0xE8, 0xAC, 0x01, 0x00, 0x00},
 9, {0x8B, 0x4D, 0xE8, 0x51, 0xE8, 0x00, 0x00, 0x00, 0x00},
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
    SetRel32_AtRelPatch(&PI_Packets_ChangePacketFunction, 4, (void *) PreviewPacket);
    SetRel32_AtRelPatch(&PI_Packets_ChangeAllPacketsFunction, 4, (void *) PreviewAllPackets);

    // Apply the patches
    Patch(&PI_Packets_ChangePacketFunction);
    Patch(&PI_Packets_ChangeAllPacketsFunction);
    
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
