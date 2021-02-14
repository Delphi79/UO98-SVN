#ifndef UODEMODLL_PLAYER__H

#define UODEMODLL_PLAYER__H 1.0

#include "mobile.h"

// Addresses of the Player variables as in the Ultima Online demo

// The functions
void __cdecl   SendSystemMessage(PlayerObject *player, const char *message);
void __cdecl   MakeGameMaster(PlayerObject *Target);
void __cdecl   UnmakeGameMaster(PlayerObject *Target);
int  __cdecl   IsGameMaster(PlayerObject *Target);

#endif