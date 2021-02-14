#ifndef UODEMODLL_LOCATION__H

#define UODEMODLL_LOCATION__H 1.0

#include "classes.h"

// Addresses of the Location variables as in the Ultima Online demo
#define LOCATION_X (0x0)
#define LOCATION_Y (0x2)
#define LOCATION_Z (0x4)

// The API functions
#define LocationGetX(location) (GetSWord(location, LOCATION_X))
#define LocationGetY(location) (GetSWord(location, LOCATION_Y))
#define LocationGetZ(location) (GetSWord(location, LOCATION_Z))
#define LocationSetX(location, value) (SetSWord(location, LOCATION_X, value))
#define LocationSetY(location, value) (SetSWord(location, LOCATION_Y, value))
#define LocationSetZ(location, value) (SetSWord(location, LOCATION_Z, value))

extern int __stdcall IsEqualXY (LocationObject *A, LocationObject *B);
extern int __stdcall IsEqualXYZ(LocationObject *A, LocationObject *B);

// Support functions
void initLocation(LocationObject* location);
void Location_ToString(char* buffer, LocationObject* location);

#endif