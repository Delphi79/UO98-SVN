#ifndef UODEMODLL_CORE__H

#define UODEMODLL_CORE__H 1.0

#pragma warn -8002 // Restarting compile using assembly

// Functions to retrieve/get data
#define GetSByte(address, reladdress)  ((  signed char )(*(  signed char  *)((char*)address+reladdress)))
#define GetUByte(address, reladdress)  ((unsigned char )(*(unsigned char  *)((char*)address+reladdress)))
#define GetSWord(address, reladdress)  ((  signed short)(*(  signed short *)((char*)address+reladdress)))
#define GetUWord(address, reladdress)  ((unsigned short)(*(unsigned short *)((char*)address+reladdress)))
#define GetSDWord(address, reladdress) ((  signed long )(*(  signed long  *)((char*)address+reladdress)))
#define GetUDWord(address, reladdress) ((unsigned long )(*(unsigned long  *)((char*)address+reladdress)))

// Functions to change/set data
#define SetSByte(address, reladdress, value)  *(  signed char  *)((char*)address+reladdress) = (  signed char )value
#define SetUByte(address, reladdress, value)  *(unsigned char  *)((char*)address+reladdress) = (unsigned char )value
#define SetSWord(address, reladdress, value)  *(  signed short *)((char*)address+reladdress) = (  signed short)value
#define SetUWord(address, reladdress, value)  *(unsigned short *)((char*)address+reladdress) = (unsigned short)value
#define SetSDWord(address, reladdress, value) *(  signed long  *)((char*)address+reladdress) = (  signed long )value
#define SetUDWord(address, reladdress, value) *(unsigned long  *)((char*)address+reladdress) = (unsigned long )value

// THISCALL, always use this macro in your class functions as the first line!!!
#define GETTHIS(x) unsigned int x; __asm { mov x, ecx }

#endif