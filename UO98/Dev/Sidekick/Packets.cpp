#include "stdafx.h"
#include <stdlib.h>
#include <stdio.h>
#include "Classes.h"
#include "patcher.h"
#include "NativeEvents.h"

#include <WinSock2.h>
#pragma comment(lib, "ws2_32.lib")

#pragma unmanaged

namespace NativeMethods
{
    // Function Pointers for calls from Sharpkick
    typedef int  (_cdecl *FUNCPTR_SendPack)(void*, char*, unsigned int);
    typedef void (_cdecl *FUNCPTR_ReplaceClientPacket)(void*, unsigned int, unsigned char*, unsigned int);

    void RemovePacket(unsigned char *Data, unsigned int PacketSize, unsigned int *pTotalDataSize);
    void PrependEmptyPacket(unsigned char PacketID, unsigned char *Data, unsigned int PacketSize, unsigned int *pTotalDataSize);

    // Debug Settings:
    enum PACKETDEBUGLEVEL{ None, Error, Warning, Translation, All };
    enum PACKETDEBUGLEVEL packetDebugLevel=None; // This should be set to desired level in Configure()

    unsigned char localPacketBuffer[0xFFFF];

    // Packet functions
    #define FUNC_SocketObject_SendPacket 0x47F222
    int SocketObject_SendPacket(void* pClientSocket, unsigned __int8* PacketData, unsigned int DataSize)
    {
        int _EAX;
        __asm
        {
            mov ecx, DataSize
            push ecx
            mov edx, PacketData
            push edx
            mov ecx, pClientSocket
            mov eax, FUNC_SocketObject_SendPacket
            call eax
            mov _EAX, eax
        }
        return _EAX;
    }

    void SocketObject_RemoveFirstPacket(void* pClientSocket, unsigned int PacketLength)
    {
        unsigned int      *pTotalDataSize = (unsigned int *) ((char *) pClientSocket + 0x1002C);
        unsigned char     *Data           = (unsigned char *) pClientSocket + 0x28;
        unsigned char      PacketID       = *Data;

        RemovePacket(Data, PacketLength, pTotalDataSize);
        if(packetDebugLevel>=Translation)
            printf("PACKETS <- Removed packet %02X.\n", PacketID);
        //if(*pTotalDataSize <=0)	// Only need to append a packet if the buffer is now empty
        {
            PrependEmptyPacket(0xA4, Data, 149, pTotalDataSize);
            if(packetDebugLevel>=Translation)
                puts("PACKETS <- Added packet 0xA4.");
        }
    }

    void ReplaceServerPacketData(unsigned __int8 **ppCurrentPacketBuffer, unsigned int *pCurrentPacketLen, unsigned __int8 *newPacketBytes, unsigned int newPacketLength)
    {
        unsigned int currentPacketLength=*pCurrentPacketLen;

        unsigned char *originalStackAllocatedPacketBuffer=*ppCurrentPacketBuffer;
        unsigned char *bufferToUse;

            if(newPacketLength <= currentPacketLength)
                bufferToUse= originalStackAllocatedPacketBuffer;
            else
            bufferToUse= localPacketBuffer;
    
        memcpy(bufferToUse, newPacketBytes, newPacketLength);
        *pCurrentPacketLen=newPacketLength;
        *ppCurrentPacketBuffer=bufferToUse;
    }

    // Packet handling API functions:
    int _declspec(dllexport) IsDynamicSizedPacket(unsigned char PacketID)
    {
      // NOTE: the demo supports only up to 0xB5, any value above will cause invalid results!
      int _EAX;
      __asm
      {
        mov al, PacketID
        mov ecx, 0x47F320
        push eax
        call ecx
        pop ecx
        mov _EAX, eax
      }
      return _EAX & 0x8000 ? 1 : 0;
    }

    unsigned int _declspec(dllexport) GetFixedPacketSize(unsigned char PacketID)
    {
      // NOTE: the demo supports only up to 0xB5, any value above will cause invalid results!
      int _EAX;
      __asm
      {
        movzx eax, PacketID
        imul eax, 0x0A
        mov ax, [eax + 0x61F138]
        mov _EAX, eax
      }
      return _EAX;
    }

    void _declspec(dllexport)ReplaceClientPacket(void *Socket, unsigned int CurPacketSize, unsigned char *NewPacket, unsigned int NewPacketSize)
    {
      unsigned int      *pTotalDataSize = (unsigned int *) ((char *) Socket + 0x1002C);
      unsigned char     *Data           = (unsigned char *) Socket + 0x28;

      RemovePacket(Data, CurPacketSize, pTotalDataSize);

      memmove(Data + NewPacketSize, Data, *pTotalDataSize);
      *pTotalDataSize += NewPacketSize;
      memcpy(Data, NewPacket, NewPacketSize);
    }

    //Outgoing Packets:

    #define ptFUNC_AddPacketToSendQueue 0x47FE9C
    typedef void (_cdecl *FUNCPTR_AddPacketToSendQueue)(void* Socket, unsigned char* Data, unsigned int DataLen);
    FUNCPTR_AddPacketToSendQueue FUNC_AddPacketToSendQueue = (FUNCPTR_AddPacketToSendQueue)ptFUNC_AddPacketToSendQueue;
    // This intercepts packets sent to the client. If the packet is desired to be replaced, you must free(Data), and allocate a new(buffer) for your packet.
    void __cdecl Hook_PacketSend(void *Socket, unsigned char* Data, unsigned int DataLen)
    {
        unsigned int SocketNumber = *(unsigned int *)((char *) Socket + 0x0C);

      if(packetDebugLevel>=All)
          printf("PACKETS -> %08X: ID=%02X, PS=%3u\n",SocketNumber,*Data,DataLen);
    
      InvokeOnPacketSending((unsigned char*)Socket, &Data, &DataLen);
    
      FUNC_AddPacketToSendQueue(Socket,Data,DataLen);
    }

    //Incoming packets:

    // Make a copy of packet
    unsigned char *ExtractPacket(unsigned char *Data, unsigned int PacketSize)
    {
      unsigned char *buffer = (unsigned char *) malloc(PacketSize);
      // If unable to allocate, we will just crash on the next line.
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

    // Socket operations:
    #define FUNC_SendPacket_PostLogin 0x47E0D1
    int _cdecl SendPacket_PostLogin(void* Player, char* PacketData, unsigned int DataSize) // PacketData is copied new a new allocated buffer
    {
        int _EAX;
        __asm
        {
            mov ecx, DataSize
            push ecx
            mov edx, PacketData
            push edx
            mov ecx, Player							//this
            push ecx
            mov eax, FUNC_SendPacket_PostLogin
            call eax
            add esp, 0Ch
            mov _EAX, eax
        }
        return _EAX;
    }

    #define FUNC_SendPacket_PreLogin 0x47E42D
    int _cdecl SendPacket_PreLogin(void* Socket, char* PacketData, unsigned int DataSize) // PacketData is copied new a new allocated buffer
    {
        int _EAX;
        __asm
        {
            mov ecx, DataSize
            push ecx
            mov edx, PacketData
            push edx
            mov ecx, Socket							//this
            push ecx
            mov eax, FUNC_SendPacket_PreLogin
            call eax
            add esp, 0Ch
            mov _EAX, eax
        }
        return _EAX;
    }

    void HandleOutsideRangePacket(void *Socket)
    {
      const unsigned int MaxBufferSize  = 0x10000;
      unsigned int      *pTotalDataSize = (unsigned int *) ((char *) Socket + 0x1002C);
      unsigned char     *Data           = (unsigned char *) Socket + 0x28;
      unsigned int       SocketNumber   = * (unsigned int *) ((char *) Socket + 0x0C);
      unsigned char      PacketID       = *Data;

      if(InvokeOnOutsideRangePacket((unsigned char*)Socket))	// if defined, this is responsible for handling of this packet.
        return;
      else if(PacketID == 0xB6)
      {
        if(packetDebugLevel>=Translation)
            printf("PACKETS <- %08X: Removing packet 0xB6\n", SocketNumber);

        // This packet is unknown by the demo, therefore remove it from the buffer
        // And replace by 0xA4 (System Information), which is handled and ignored safely by the demo server
        RemovePacket(Data, 9, pTotalDataSize);
        PrependEmptyPacket(0xA4, Data, 149, pTotalDataSize);
      }
      else if(packetDebugLevel>=Error)
        printf("PACKETS <- %08X: Invalid packet, client will most likely crash!\n", SocketNumber);
    }

    /*
      WARNING! When this function exits you must make sure at least one valid packet is inside the buffer!
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

      if(packetDebugLevel>=All)
        printf("PACKETS <- %08X: ID=%02X, PS=%3u, TDS=%3u, DS=%d, CP=%d, OP=%d, RS=%u, PV=%d\n", SocketNumber, PacketID, *pTotalDataSize, PacketSize, IsPacketDynamicSized, IsPacketComplete, IsOnlyPacketInBuffer, SizeRemainingInBuffer, IsPacketValid);

      if(!IsPacketValid)
      {
        if(!PacketIsWithinRange)
          HandleOutsideRangePacket(Socket);
        return (void *) Data;
      }

      if(PacketID == 0xAD)
      {
        int c;
        int textstart=0;
        int SpeechLength = PacketSize - 12;
        int UTF8=0;

        PacketCopy = ExtractPacket(Data, PacketSize);						// make a copy of the packet
        RemovePacket(Data, PacketSize, pTotalDataSize);						// remove the packet from buffer
 
        // I am unsure of the packet format in client 1.25
        if(GetUByte(Data, 3) == 0xC0)
        {
            int value=0;
            int count=0;
            int hold=0;
            int i=0;

            value = htons(GetUWord(Data, 12));			textstart+=2;
            count = (value & 0xFFF0) >> 4;
            hold = value & 0xF;

            if(packetDebugLevel>=All)
                printf("PACKETS <- %08X: 0xAD mode: 0xC0 count:%d value:%d hold %d\n", SocketNumber, count, value, hold);

            for ( i = 0; i < count; ++i )
            {
                if ( (i & 1) == 0 )
                {
                    hold <<= 8;
                    hold |= GetUByte(Data, 12 + textstart);		textstart++;
                    hold = 0;
                }
                else
                {
                    value = GetUWord(Data, 12 + textstart);			textstart+=2;
                    hold = value & 0xF;
                }
            }

            if(packetDebugLevel>=Translation)
                printf("PACKETS <- %08X: Handled Unsupported 0xAD mode: 0xC0\n", SocketNumber);

            UTF8=1;
            SpeechLength-=textstart;
        }
        else
        {
            if(packetDebugLevel>=Translation)
                printf("PACKETS <- %08X: Converting 0xAD to 0x03\n", SocketNumber);
        }

        if(SpeechLength < 0)
          SpeechLength = 0;
        else if(!UTF8)
          SpeechLength /= 2;

        // Convert UNICODE speech to ASCII speech
        PrependEmptyPacket(0x03, Data, 8 + SpeechLength, pTotalDataSize);	// Insert packet 0x3 into the buffer
        SetUWord(Data, 1, htons(SpeechLength + 8)); // PacketSize			// Set packet values
        SetUByte(Data, 3, UTF8 ? 0x00 : GetUByte(PacketCopy, 3)); // Mode
        SetUWord(Data, 4, GetUWord(PacketCopy, 4)); // TextColor
        SetUWord(Data, 6, GetUWord(PacketCopy, 6)); // Font
        for(c = 0; c < SpeechLength; c ++)									// Unicode -> Ascii
          SetSByte(Data, 8 + c, GetSByte(PacketCopy, textstart + 12 + (UTF8 ? c : (1 + c * 2))));
      }

      if(PacketCopy != NULL)												// Free memory allocated by ExtractPacket
        free(PacketCopy);

      InvokeOnPacketReceived((unsigned char *)Socket, PacketID, PacketSize, IsPacketDynamicSized);

      return (void *) Data;
    }

    /*
      WARNING! When this function exits you must make sure at least one valid packet is inside the buffer!
    */
    void * __cdecl PreviewAllPackets(unsigned char *Socket)
    {
      const unsigned int MaxDataSize    = 0x10000;
      unsigned int      *pTotalDataSize = (unsigned int *) (Socket + 0x1002C);
      unsigned char     *Data           = (unsigned char *)(Socket + 0x28);

      unsigned char		FirstPacketID            = *Data;
      int				FirstPacketIsWithinRange = FirstPacketID <= 0xB5;
      unsigned int      SocketNumber   = * (unsigned int *) ((char *) Socket + 0x0C);
      unsigned int IpAddress;
      PlayerObject *player;

      if(SocketNumber!=0xBEEF) // Detect Version numbers and Seed packets if not the local internal client.
      {
          // check for "Seed" sent from later de-encrypted (RICE'd) clients. 
          // If First 4 bytes are equal to the IPAddress when PlayerObject is Null, remove them.
          player=*(PlayerObject**)(Socket + 0x10038);
          IpAddress=htonl(*(unsigned int*)(Socket + 0x10030));	// Socket stores IP in host byte order, we want network order (currently unused)
          if(!player)
          {   // is Pre-login packet
              FirstPacketID            = *Data;

              // A 4 byte packet is assumed to be a seed packet. If the size exceeds 4, we check for a 0x80 login packet, or a 0x91 play server packet.
              // The seed is is usually the IpAddress, although this cannot be relied upon.
              if((*pTotalDataSize==4) || (*pTotalDataSize >= 5 && (*(Data+4)==0x80 || *(Data+4)==0x91)) )
              {
                  NativeMethods::SocketObject_RemoveFirstPacket(Socket,4);
                  puts("Removed seed.");
          
                  FirstPacketID            = *Data;
                  FirstPacketIsWithinRange = FirstPacketID <= 0xB5;

                  // If we received a seed, we will assume client version 1.26.4i until told different.
                  if(*(int*)(Socket + 0x1003C)==0) // if version is 0.0.0.0
                  {
                      *(Socket + 0x1003C + 0)=1;
                      *(Socket + 0x1003C + 1)=26;
                      *(Socket + 0x1003C + 2)=4;
                      *(Socket + 0x1003C + 3)=9; // i
                  }
              }
              else if(*(Socket + 0x1003C + 0)==0 )
              { // Assume 1.25.35
                *(Socket + 0x1003C + 0)=1;
                *(Socket + 0x1003C + 1)=25;
                *(Socket + 0x1003C + 2)=35;
                *(Socket + 0x1003C + 3)=0;
              }
          }
      }
      else if(*(Socket + 0x1003C + 0)==0) // set Internal client version
      {
                    *(Socket + 0x1003C + 0)=1;
                    *(Socket + 0x1003C + 1)=23;
                    *(Socket + 0x1003C + 2)=0;
                    *(Socket + 0x1003C + 3)=0;
      }
      // end Version detection


      // This is important!
      // Due to a bug in the server code, the demo will not really handle packets starting 0xB6 and above
      // NOTE: only the first packet will cause a crash, all other invalid packets in the queue are handled later better the code
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

    #define POI_CreateSocketForTheClient 0x47EF3F
    PATCHINFO PI_Socket_ExpandStructure =			// Adds 4 bytes to socket Structure length, will use for client version
    {
     POI_CreateSocketForTheClient,
     5, {0x68, 0x3C, 0x00, 0x01, 0x00},	// push    1003Ch
     5, {0x68, 0x40, 0x00, 0x01, 0x00}, // push    10040h
    };

    PATCHINFO PI_Hook_PacketSend =			// Hooks PacketSend function
    {
     0x47F266,
     5, {0xE8, 0x0D, 0x0C, 0x00, 0x00},	// call    FUNC_AddPacketToSendQueue
     5, {0xE8, 0x0D, 0x0C, 0x00, 0x00},
    };

    PATCHINFO PI_Init_SocketData =			// Expands the initialization of FUNC_ServerSocket_CUserSocket_Constructor to include the end of the struct, incl our 4 byte version patch above
    {
     0x0047EFD1,
     5, {0x68, 0x00, 0x00, 0x01, 0x00},	//  push    10000h                          ; Size
     5, {0x68, 0x18, 0x00, 0x01, 0x00}, //  push    10018h                          ; Size
    };


    void Initialize_packets()
    {
      packetDebugLevel=Warning;	  // Level of packet diagnostic information written to console. Values: None, Error, Warning, Translation, All

      // Prepare the patches
      SetRel32_AtRelPatch(&PI_Packets_ChangePacketFunction, 4, (void *) PreviewPacket);
      SetRel32_AtRelPatch(&PI_Packets_ChangeAllPacketsFunction, 4, (void *) PreviewAllPackets);
        SetRel32_AtPatch(&PI_Hook_PacketSend, &Hook_PacketSend);

      // Apply the patches
      Patch(&PI_Packets_ChangePacketFunction);
      Patch(&PI_Packets_ChangeAllPacketsFunction);
      Patch(&PI_Hook_PacketSend);
      Patch(&PI_Socket_ExpandStructure);		// Adds 4 bytes to socket Structure length, will use for client version
      Patch(&PI_Init_SocketData);				// Ensures the 4 bytes above are initialized
    }
}