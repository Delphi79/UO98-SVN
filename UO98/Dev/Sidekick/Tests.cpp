#pragma unmanaged

#include "TestsMain.h"

LocationObject location;
LocationObject locationPlus1;
LocationObject locationPlus8;


void DoTests()
{
  Tests_Lists_Execute();
  Tests_World_Execute();
  Tests_ObjectVars_Execute();
  Tests_ObjectScripts_Execute();
  Tests_Classes_Execute();
}

void InitTestLocations()
{
  location.X=33;
  location.Y=20;
  location.Z=0;

  locationPlus1.X=location.X+1;
  locationPlus1.Y=location.Y+0;
  locationPlus1.Z=location.Z;

  locationPlus8.X=location.X+8;
  locationPlus8.Y=location.Y+2;
  locationPlus8.Z=location.Z;
}


