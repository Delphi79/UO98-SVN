#ifndef UODEMODLL_TIMEMANAGER__H

#define UODEMODLL_TIMEMANAGER__H 1.0

#include "core.h"

// Address of the static Time Manager object in the Ultima Online demo
#define GLOBAL_TIMEMANAGER (0x006482A8)

// The functions
#define GetPulseNum() GetUDWord(GLOBAL_TIMEMANAGER, 0x0C)

#endif