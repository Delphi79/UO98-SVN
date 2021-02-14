#ifndef UODEMODLL_MOBILE__H

#define UODEMODLL_MOBILE__H 1.0

#include "classes.h"

// Addresses of the Mobile variables as in the Ultima Online demo
#define MOBILE_SWINGCOUNTER (0x314)
#define MOBILE_SWINGSTATE   (0x318)

// The functions
#define GetSwingState(mobile)   (GetSDWord(mobile, MOBILE_SWINGSTATE  ))
#define GetSwingCounter(mobile) (GetSDWord(mobile, MOBILE_SWINGCOUNTER))

#define SetSwingState(mobile, value)   SetSDWord(mobile, MOBILE_SWINGSTATE  , value)
#define SetSwingCounter(mobile, value) SetSDWord(mobile, MOBILE_SWINGCOUNTER, value)

extern int  __stdcall AdvanceSwingState(unsigned int);
extern void __stdcall ResetSwingState(unsigned int mobile, unsigned int action);

unsigned int __stdcall GetWeaponInHand(unsigned int mobile);
unsigned int __stdcall GetSwingSpeed(unsigned int mobile);

#endif