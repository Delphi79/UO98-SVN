#ifndef UODEMODLL_WEAPON__H

#define UODEMODLL_WEAPON__H 1.0

#include "core.h"

// Addresses of the Weapon variables as in the Ultima Online demo
#define WEAPON_TEMPLATE (0x50)

// The functions
#define GetWeaponTemplate(weapon) (GetUByte(weapon, WEAPON_TEMPLATE))

extern int __stdcall IsRanged(WeaponObject *weapon);

extern unsigned char __stdcall GetWeaponSpeed(WeaponObject *weapon);

#endif