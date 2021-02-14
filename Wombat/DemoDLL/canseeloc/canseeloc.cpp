#include <windows.h>
#include <stdio.h>

#include "patcher.c"
#include "patches.c"

#include "classes.h"
#include "TimeManager.h"

#include "Location.c"

#pragma warn -8027

class LOCATION
{
public:
  short X, Y, Z;

  LOCATION()
  {
    Set(-1, -1, -1);    
  }

  LOCATION(short _X, short _Y, short _Z)
  {
    Set(_X, _Y, _Z);
  }

  LOCATION(LOCATION &CopyFrom)
  {
    Set(CopyFrom.X, CopyFrom.Y, CopyFrom.Z);    
  }

  void Set(short _X, short _Y, short _Z)
  {
    X = _X;
    Y = _Y;
    Z = _Z;
  }

  int IsEqualXY(LOCATION *XY)
  {
    return this->X == XY->X && this->Y == XY->Y;
  }

  int IsEqualXYZ(LOCATION *XYZ)
  {
    return this->X == XYZ->X && this->Y == XYZ->Y && this->Z == XYZ->Z;
  }
};

class ITEM
{
public:
  LocationObject *GetLocation();
  int GetHeight();
  int GetObjectFlags();
  ITEM *NextObject();
  ITEM *NextStaticObject();
};

LocationObject *ITEM::GetLocation()
{
  __asm
  {
    mov ecx, this
    mov eax, 0x491210
    call eax
  }
  return (LocationObject *) _EAX;
}

int ITEM::GetHeight()
{
  __asm
  {
    mov ecx, this
    mov eax, [ecx]
    call dword ptr [eax + 0x28]
  }
  return _EAX;
}

int ITEM::GetObjectFlags()
{
  __asm
  {
    mov ecx, this
    mov eax, [ecx]
    call dword ptr [eax + 0x30]
  }
  return _EAX;
}

ITEM *ITEM::NextObject()
{
  return (ITEM *) GetUDWord(this, 0x20);
}

ITEM *ITEM::NextStaticObject()
{
  return (ITEM *) GetUDWord(this, 0x10);
}

class Value
{
public:
  int V;

  Value(int _V)
  {
    V = _V;    
  }
};

class ZandFlags : public Value
{
public:
  int Flags;

  ZandFlags(int _Z, int _Flags) : Value(_Z)
  {
    Flags = _Flags;
  }

  int CheckZ(LOCATION *Location, ITEM *Item)
  {
    LOCATION ItemLoc;
    LocationObject *DemoLoc;

    // Get the location and convert from demo-class to local C++ class
    DemoLoc = Item->GetLocation();
    ItemLoc.Set(LocationGetX(DemoLoc), LocationGetY(DemoLoc), LocationGetZ(DemoLoc));

    // NOTE: this->V is HighestZ or Z from MyCanSeeLoc
    if( ItemLoc.Z < this->V )
      if( ItemLoc.Z + Item->GetHeight() > Location->Z )
        return 1;
    return 0;
  }

  int CheckZandFlags(LOCATION *Location, ITEM *Item)
  {
    if( CheckZ(Location, Item) )
      if( Item->GetObjectFlags() & this->Flags )
        return 1;
    return 0;
  }
};

int GetChunkIndexByLocation(LOCATION *Location)
{
  // Convert from local C++ class to demo-class
  LocationObject DemoLoc;
  LocationSetX(&DemoLoc, Location->X);
  LocationSetY(&DemoLoc, Location->Y);
  LocationSetZ(&DemoLoc, Location->Z);

  __asm
  {
    push 0
    lea edx, DemoLoc
    push edx
    mov ecx, 0x6933D8
    mov eax, 0x42F2CF
    call eax
  }
  return _EAX;
}

int GetElevationAt(int X, int Y)
{
  __asm
  {
    push Y;
    push X;
    mov ecx, 0x697A4C
    mov eax, 0x46A783
    call eax
  }
  return _EAX;
}

int GetLandTileByLocation(LOCATION *Location)
{
  // Convert from local C++ class to demo-class
  LocationObject DemoLoc;
  LocationSetX(&DemoLoc, Location->X);
  LocationSetY(&DemoLoc, Location->Y);
  LocationSetZ(&DemoLoc, Location->Z);

  int Tile;

  __asm
  {
    lea edx, DemoLoc
    push dword ptr [edx+4]
    push dword ptr [edx+0]
    mov ecx, 0x697A4C
    mov eax, 0x46A9E6
    call eax
    mov Tile, eax
  }

//  printf("Tile=%d", Tile);

  return Tile;
}

void *GetLandTilePointer(int Tile) // Land
{
  __asm
  {
     mov eax, 0x1C
     mov ecx, 0x64B368
     imul Tile
     add eax, [ecx]
  }
  return (void *) _EAX;
}

unsigned int GetLandFlags(void *ptr)
{
  unsigned int Flags = GetUDWord(ptr, 0);
//  printf("Flags=%X & 0x10=%d", Flags, Flags & 0x10);
  return Flags;
}

void *GetChunkPointer(int Chunk)
{
  __asm
  {
     mov eax, 0x11C
     mov ecx, 0x6933E4
     imul Chunk
     add eax, [ecx]
  }
  return (void *) _EAX;
}

ITEM *GetObjectsAtChunk(void *ptr)
{
  return (ITEM *) GetUDWord(ptr, 0x104);
}

ITEM *GetStaticsObjectsAtChunk(void *ptr)
{
  return (ITEM *) GetUDWord(ptr, 0x100);
}

int AreObjectsAtChunkBlockingSight(LOCATION *Location, ZandFlags ZandFlags)
{
  int ChunkIndex = GetChunkIndexByLocation(Location);
  if( ChunkIndex == -1 )
    return 0;

  LOCATION ItemLoc;
  LocationObject *DemoLoc;

  ITEM *ItemWalkerNext = GetObjectsAtChunk(GetChunkPointer(ChunkIndex));
  for(; ItemWalkerNext != NULL; ItemWalkerNext = ItemWalkerNext->NextObject())
  {
    // Get the location and convert from demo-class to local C++ class
    DemoLoc = ItemWalkerNext->GetLocation();
    ItemLoc.Set(LocationGetX(DemoLoc), LocationGetY(DemoLoc), LocationGetZ(DemoLoc));

    // Compare
    if(ItemLoc.IsEqualXY(Location) )
      if( ZandFlags.CheckZandFlags(Location, ItemWalkerNext) )
        return 1;
  }

  ITEM *ItemWalkerPrevious = GetStaticsObjectsAtChunk(GetChunkPointer(ChunkIndex));
  for(; ItemWalkerPrevious != NULL; ItemWalkerPrevious = ItemWalkerPrevious->NextStaticObject())
  {
    // Get the location and convert from demo-class to local C++ class
    DemoLoc = ItemWalkerPrevious->GetLocation();
    ItemLoc.Set(LocationGetX(DemoLoc), LocationGetY(DemoLoc), LocationGetZ(DemoLoc));

    // Compare
    if( ItemLoc.IsEqualXY(Location) )
      if( ZandFlags.CheckZandFlags(Location, ItemWalkerPrevious) )
        return 1;
  }

  return 0;
}

int MyCanSeeLoc(LOCATION *Looker, LOCATION *LookedAt, int Flags)
{
  // The "looker" can't see the "LookedAt" if one of them is higher than or equal of the elevation of the tile (but not both)!
  // REMEMBER: ^ = XOR = if only one statement is false, then the expression evaluates to true
  int LookerElevation = GetElevationAt(Looker->X, Looker->Y);
  int LookedAtElevation = GetElevationAt(LookedAt->X, LookedAt->Y);
  if( Looker->Z >= LookerElevation ^ LookedAt->Z >= LookedAtElevation )
  {
    if(Looker->Z >= LookerElevation)
      printf("  (Looker-Z:%d >= ...-Elevation:%d) ^ (LookerAt-Z:%d >= ...-Elevation:%d) =>\n  1 ^ 0 => return 0\n", Looker->Z, LookerElevation, LookedAt->Z, LookedAtElevation);
    else
      printf("  (Looker-Z:%d >= ...-Elevation:%d) ^ (LookerAt-Z:%d >= ...-Elevation:%d) =>\n  0 ^ 1 => return 0\n", Looker->Z, LookerElevation, LookedAt->Z, LookedAtElevation);
    return 0;
  }

  // The flags are used when calling AreObjectsAtChunkBlockingSight
  int CheckFlags = 0;
  if(Flags & 0x01)
    CheckFlags |= 0x3000; // NoShoot OR Window
  if(Flags & 0x02)
    CheckFlags |= 0x40;   // Impassable

  printf("  Flags:0x%X => CheckFlags:0x%X\n", Flags, CheckFlags);

  int DiffX = LookedAt->X - Looker->X;
  int DiffY = LookedAt->Y - Looker->Y;
  int DiffZ = LookedAt->Z - Looker->Z;

  int DiffAbsX = abs(DiffX);
  int DiffAbsY = abs(DiffY);

  int HighestDiffXorY = DiffAbsX;
  if( HighestDiffXorY < DiffAbsY )
    HighestDiffXorY = DiffAbsY;

  int LowestZ, HighestZ;
  if( LookedAt->Z < Looker->Z )
  {
    HighestZ = Looker->Z;
    LowestZ = LookedAt->Z;
  }
  else
  {
    HighestZ = LookedAt->Z;
    LowestZ = Looker->Z;
  }

  printf("  HighestDiffXorY:%d %s 1 => %s test...\n", HighestDiffXorY, HighestDiffXorY <= 1 ? "<=" : ">", HighestDiffXorY <= 1 ? "near" : "far");

  if( HighestDiffXorY <= 1)
  {
    // near check
    // =========
    if( abs(DiffZ) < 16 )
    {
      printf("  abs(DiffZ:%d):%u < 16 => return 1\n", DiffZ, abs(DiffZ));
      return 1;
    }
    printf("  abs(DiffZ:%d):%u >= 16 => continue\n", DiffZ, abs(DiffZ));

    ZandFlags ZandFlags_Looker(HighestZ, CheckFlags);
    if( AreObjectsAtChunkBlockingSight(Looker, ZandFlags_Looker) ) // rofl, copy-constructor on the stack
    {
      printf("  AreObjectsBlockingSight(Looker, HighestZ:%d, CheckFlags) != 0 => return 0\n", HighestZ);
      return 0;
    }
    printf("  AreObjectsBlockingSight(Looker, HighestZ:%d, CheckFlags) == 0 => continue\n", HighestZ);

    if( HighestDiffXorY == 0 )
    {
      printf("  HighestDiffXorY == 0 => return 1\n");
      return 1;
    }

    ZandFlags ZandFlags_LookedAt(HighestZ, CheckFlags);
    if( AreObjectsAtChunkBlockingSight(LookedAt, ZandFlags_LookedAt) ) // rofl, copy-constructor on the stack
    {
      printf("  AreObjectsBlockingSight(LookedAt, HighestZ:%d, CheckFlags) != 0 => return 0\n", HighestZ);
      return 0;
    }
    printf("  AreObjectsBlockingSight(LookedAt, HighestZ:%d, CheckFlags) == 0 => return 1\n", HighestZ);

    return 1;
  }
  else
  {
    // far check
    // ========= 
    DiffX = ( (signed int) ((unsigned int) DiffX) * 0x10000 ) / HighestDiffXorY;
    DiffY = ( (signed int) ((unsigned int) DiffY) * 0x10000 ) / HighestDiffXorY;
    DiffZ = ( (signed int) ((unsigned int) DiffZ) * 0x10000 ) / (HighestDiffXorY - 1);

    int ModifiedLookerZ = (Looker->Z * 0x10000 + 0x8000);
    int ModifiedLookerX = (Looker->X * 0x10000 + 0x8000) + (DiffY / 4);
    int ModifiedLookerY = (Looker->Y * 0x10000 + 0x8000) - (DiffX / 4);

    int ModifiedLookerXminusHalvedDiffY = ModifiedLookerX - (DiffY / 2);
    int ModifiedLookerYminusHalvedDiffX = ModifiedLookerY + (DiffX / 2);

    printf("  Status[0]=1, Status[1]=1 => continue\n");

    // Assume all tests succeeded and start "casting" our "light-of-sight ray"
    int StatusArray[2] = {1, 1};
    for(int XorY = 0; XorY < HighestDiffXorY - 1; XorY ++)
    {
      ModifiedLookerX += DiffX;
      ModifiedLookerY += DiffY;
      ModifiedLookerXminusHalvedDiffY += DiffX;
      ModifiedLookerYminusHalvedDiffX += DiffY;

      LOCATION LocationArray[2];

      LocationArray[0].Set(ModifiedLookerX / 0x10000, ModifiedLookerY / 0x10000, ModifiedLookerZ / 0x10000);
      LocationArray[1].Set(ModifiedLookerXminusHalvedDiffY / 0x10000, ModifiedLookerYminusHalvedDiffX / 0x10000, ModifiedLookerZ / 0x10000);

      ModifiedLookerZ += DiffZ;

      LOCATION LastLocationTested(-1, -1, 0);
      printf("   LastLocation=(-1,-1,0)\n");

      // Test the two locations
      int Status = 0;
      for(int SubCounter = 0; SubCounter < 2; SubCounter ++)
      {
        // Only test a location if it hasn't failed yet
        if( StatusArray[SubCounter] != 0 )
        {
          // Don't test locations we already tested
          if( ! LocationArray[SubCounter].IsEqualXY(&LastLocationTested) )
          {
            printf("   LocationArray[%d].X,Y != LastLocation.X,Y => continue\n", SubCounter, SubCounter);

            LastLocationTested = LocationArray[SubCounter];
            printf("    LastLocation=(%d,%d,%d)\n", LastLocationTested.X, LastLocationTested.Y, LastLocationTested.Z);

            // We are at a new test location, so assume "succeed"
            Status = 1;

            if( GetLandFlags(GetLandTilePointer(GetLandTileByLocation(&LastLocationTested))) & 0x10 ) // WALL TEST
            {
              printf("    GetLandTileFlagsAtLocation(LastLocation) IS WALL => Status[%d]=0\n");

              // We hit a static wall on the land
              Status = 0;
            }
            else
            {
              printf("    GetLandTileFlagsAtLocation(LastLocation) IS NOT WALL => continue\n");

              LOCATION Temp = LastLocationTested;

              // We didn't hit a wall so go check for other things that could block the sight
              int Z = ModifiedLookerZ / 0x10000;
              if( Z < LastLocationTested.Z )
              {
                LastLocationTested.Z = Z;
                Z = LocationArray[SubCounter].Z;
              }
              if( LastLocationTested.Z > LowestZ )
                LastLocationTested.Z --;
              if( Z < HighestZ )
                Z ++;

              if(!Temp.IsEqualXYZ(&LastLocationTested))
                printf("    LastLocation=(%d,%d,%d) // CHANGED DUE TO Z\n", LastLocationTested.X, LastLocationTested.Y, LastLocationTested.Z);
              ZandFlags ZandFlags(Z, CheckFlags);
              if( AreObjectsAtChunkBlockingSight(&LastLocationTested, ZandFlags) ) // rofl, copy-constructor on the stack
              {
                printf("    AreObjectsBlockingSight(LastLocation, Z:%d, CheckFlags) != 0 => Status[%d]=0\n", Z, SubCounter);
                Status = 0;
              }
              else
                printf("    AreObjectsBlockingSight(LastLocation, Z:%d, CheckFlags) == 0 => Status[%d]=1\n", Z, SubCounter);
            }
          }
          else
            printf("   LocationArray[%d].X,Y == LastLocation.X,Y => Status[%d]=%d\n", SubCounter, SubCounter, Status);

          // Update the status
          // NOTE: if the location remained the same then we will use the status from the last test
          StatusArray[SubCounter] = Status;
        }
      }

      // Only return "false" when both location tests failed
      if( StatusArray[0] == 0 && StatusArray[1] == 0 )
      {
        printf("  Status[0]==0 && Status[1]==0 => return 0\n");
        return 0;
      }
      else
        printf("  Status[0]:%d, Status[1]:%d => continue\n", StatusArray[0], StatusArray[1]);
    }

    printf("  return 1\n");
    return 1;
  }
}

int _stdcall DemoCanSeeLoc(LocationObject Looker, LocationObject LookedAt, int Flags)
{
  __asm
  {
    // The CanSeeLoc function does not takes a pointer but 2 "complete" (8-bytes) structures ;)
    push Flags
    lea edx, LookedAt
    push dword ptr [edx+4]
    push dword ptr [edx+0]
    lea edx, Looker
    push dword ptr [edx+4]
    push dword ptr [edx+0]
    mov ecx, 0x697A4C
    mov eax, 0x46ADA5
    call eax
  }
  return _EAX;
}

int _stdcall CanSeeLoc(LocationObject Looker, LocationObject LookedAt, int Flags)
{
  // This function is being called from the demo!
  // So we must convert the class to a local class
  LOCATION MyLooker, MyLookedAt;
  MyLooker.Set(LocationGetX(&Looker), LocationGetY(&Looker), LocationGetZ(&Looker));
  MyLookedAt.Set(LocationGetX(&LookedAt), LocationGetY(&LookedAt), LocationGetZ(&LookedAt));

  printf("CanSeeLoc(Looker:(%d,%d,%d), LookedAt:(%d,%d,%d), Flags:0x%X)?\n", MyLooker.X, MyLooker.Y, MyLooker.Z, MyLookedAt.X, MyLookedAt.Y, MyLookedAt.Z, Flags);

  // Execute both the original implementation and our re-implementation
  int DemoResult = DemoCanSeeLoc(Looker, LookedAt, Flags);
  int MyResult = MyCanSeeLoc(&MyLooker, &MyLookedAt, Flags);

  printf("=> DemoCanSeeLoc = %d, MyCanSeeLoc = %d => %s\n", DemoResult, MyResult, DemoResult == MyResult ? "OK" : "FAIL");
  if(DemoResult != MyResult)
    MessageBox(HWND_DESKTOP, "MyCanSeeLoc Failed!", "CANSEELOC.DLL", MB_OK);

  return DemoResult;
}

// ****************************************
// ****************************************
// DllEntryPoint -> EDIT WITH CARE!!!
//
// This is the code called when uodemo+.exe
// executes LoadLibrary to load this DLL.
//
// The patches will be set-up and applied
//
// Do not edit the SetRel32_XXX calls!
// Feel free to fool around with the rest
// but at your own risk.
// ****************************************
// ****************************************

#pragma warn -8057
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
    SetRel32_AtPatch(&PI_CanSeeLoc1, CanSeeLoc);
    SetRel32_AtPatch(&PI_CanSeeLoc2, CanSeeLoc);
    SetRel32_AtPatch(&PI_CanSeeLoc3, CanSeeLoc);
    SetRel32_AtPatch(&PI_CanSeeLoc4, CanSeeLoc);
    SetRel32_AtPatch(&PI_CanSeeLoc5, CanSeeLoc);
    SetRel32_AtPatch(&PI_CanSeeLoc6, CanSeeLoc);

    // Apply the patches
    Patch(&PI_CanSeeLoc1);
    Patch(&PI_CanSeeLoc2);
    Patch(&PI_CanSeeLoc3);
    Patch(&PI_CanSeeLoc4);
    Patch(&PI_CanSeeLoc5);
    Patch(&PI_CanSeeLoc6);
    
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
