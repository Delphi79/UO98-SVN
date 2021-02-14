#pragma unmanaged

#include "TestsMain.h"
#include "Commands.h"

using namespace NativeMethods;

const int ITEMID_door_closed=1655;
const int ITEMID_door_open=1656;
const int ITEMID_wall_1=589;
const int ITEMID_metalchest=2475;
const int ITEMID_smallcrate=2473;
const int ITEMID_pouch=2480;

const unsigned int TileFlag_Background  = 0x1;
const unsigned int TileFlag_Weapon      = 0x2;
const unsigned int TileFlag_Wall        = 0x10;
const unsigned int TileFlag_Impassable  = 0x40;
const unsigned int TileFlag_Internal	  = 0x00010000;
const unsigned int TileFlag_Map         = 0x00100000;
const unsigned int TileFlag_Container	  = 0x00200000;
const unsigned int TileFlag_Door			  = 0x20000000;

bool Test_World_createGlobalObjectAt(void);
bool Test_World_getFirstObjectOfType(void);
bool Test_World_getNextObjectOfType(void);
bool Test_World_getNumAllObjectsInRangewithFlags(void);
bool Test_World_getObjectsInRangeWithFlags(void);
bool Test_World_getObjectsAt(void);

char buffer1[100];
char buffer2[100];

int CountObjectsAt(LocationObject* location);
void DebugPrintItemsAtLocation(char* description, LocationObject* location);

void Tests_World_Execute()
{
  InitTestLocations();

  Test_World_createGlobalObjectAt();
  Test_World_getFirstObjectOfType();
  Test_World_getNextObjectOfType();
  Test_World_getNumAllObjectsInRangewithFlags();
  Test_World_getObjectsInRangeWithFlags();
  Test_World_getObjectsAt();
}

bool Test_World_createGlobalObjectAt()
{
  bool passed;

  bool passed_created;
  bool cleanedup;

  int serial=createGlobalObjectAt(ITEMID_door_closed, &location);
  
  passed_created = serial!=0;
  if(!passed_created)
    OnTestResult(false, "World: Test_World_createGlobalObjectAt failed. serial=%u", serial);

  cleanedup = deleteObject(serial);

  if(!cleanedup)
    OnTestResult(false, "World: Test_World_createGlobalObjectAt failed to cleanup. serial=%u", serial);
  
  passed = passed_created && cleanedup;

  if(passed)
    OnTestResult(true, "World: Test_World_createGlobalObjectAt passed.");

  return true;
}

bool Test_World_getFirstObjectOfType()
{
  int serialFound=0;
  int serialCreated_door=0;
  int serialCreated_wall=0;
  
  bool passed = true;

  serialCreated_wall=createGlobalObjectAt(ITEMID_wall_1, &location);

  serialFound=getFirstObjectOfType(&location, ITEMID_door_closed);
  if(serialFound!=0)
  {
    OnTestResult(false, "World: -Test_World_getFirstObjectOfType failed. wall=%u door=%u found=%u", serialCreated_wall, serialCreated_door, serialFound);
    passed = false;
  }

  serialCreated_door=createGlobalObjectAt(ITEMID_door_closed, &location);

  serialFound=getFirstObjectOfType(&location, ITEMID_door_closed);
  if(serialFound!=serialCreated_door)
  {
    OnTestResult(false, "World: +Test_World_getFirstObjectOfType failed. wall=%u door=%u found=%u", serialCreated_wall, serialCreated_door, serialFound);
    passed = false;
  }

  serialFound=getFirstObjectOfType(&location, ITEMID_wall_1);
  if(serialFound==0)
  {
    OnTestResult(false, "World: ++Test_World_getFirstObjectOfType failed. wall=%u door=%u found=%u", serialCreated_wall, serialCreated_door, serialFound);
    passed = false;
  }

  if(!deleteObject(serialCreated_door))
  {
    OnTestResult(false, "World: Test_World_getFirstObjectOfType failed to cleanup. serialCreated_door=%u", serialCreated_door);
    passed = false;
  }
  if(!deleteObject(serialCreated_wall))
  {
    OnTestResult(false, "World: Test_World_getFirstObjectOfType failed to cleanup. serialCreated_wall=%u", serialCreated_wall);
    passed = false;
  }

  if(passed)
    OnTestResult(true, "World: Test_World_getFirstObjectOfType passed.");

  return passed;
}

bool Test_World_getNextObjectOfType()
{
  int serialFoundFirst=0;
  int serialFoundNext=0;
  int serialFoundNext2=0;
  int serialCreated_wall=0;
  int serialCreated_door=0;
  int serialCreated_door2=0;

  bool passed = true;

  serialCreated_wall=createGlobalObjectAt(ITEMID_wall_1, &location); // extra object that should not be found

  serialCreated_door=createGlobalObjectAt(ITEMID_door_closed, &location);
  serialFoundFirst=getFirstObjectOfType(&location, ITEMID_door_closed);

  if(serialFoundFirst==0)
  {
    OnTestResult(false, "World: Test_World_getNextObjectOfType failed. Failed to find FIRST. found=%u", serialFoundFirst);
    passed = false;
  }

  serialFoundNext=getNextObjectOfType(&location, ITEMID_door_closed, serialFoundFirst);
  if(serialFoundNext!=0)
  {
    OnTestResult(false, "World: -Test_World_getNextObjectOftype failed. door=%u found=%u", serialCreated_door, serialFoundNext);
    passed = false;
  }

  serialCreated_door2=createGlobalObjectAt(ITEMID_door_closed, &location);
  serialFoundFirst=getFirstObjectOfType(&location, ITEMID_door_closed);
  serialFoundNext=getNextObjectOfType(&location, ITEMID_door_closed, serialFoundFirst);
  if(serialFoundFirst==0 || serialFoundNext==0 || serialFoundNext==serialFoundFirst || (serialFoundNext!=serialCreated_door && serialFoundNext!=serialCreated_door2) )
  {
    OnTestResult(false, "World: +Test_World_getNextObjectOftype failed. door=%u door2=%u First=%u Next=%u", serialCreated_door, serialCreated_door2, serialFoundFirst, serialFoundNext);
    passed = false;
  }

  serialFoundNext2=getNextObjectOfType(&location, ITEMID_door_closed, serialFoundNext);
  if(serialFoundNext2!=0)
  {
    OnTestResult(false, "World: --Test_World_getNextObjectOftype failed. door=%u door2=%u First=%u Next=%u  Next2=%u", serialCreated_door, serialCreated_door2, serialFoundFirst, serialFoundNext, serialFoundNext2);
    passed = false;
  }

  if(!deleteObject(serialCreated_wall))
  {
    OnTestResult(false, "World: Test_World_getNextObjectOfType failed to cleanup. serialCreated_wall=%u", serialCreated_wall);
    passed = false;
  }
  if(!deleteObject(serialCreated_door))
  {
    OnTestResult(false, "World: Test_World_getNextObjectOfType failed to cleanup. serialCreated_door=%u", serialCreated_door);
    passed = false;
  }
  if(!deleteObject(serialCreated_door2))
  {
    OnTestResult(false, "World: Test_World_getNextObjectOfType failed to cleanup. serialCreated_door2=%u", serialCreated_door2);
    passed = false;
  }

  if(passed)
    OnTestResult(true, "World: Test_World_getNextObjectOfType passed.");
  
  return passed;
}

bool Test_World_getNumAllObjectsInRangewithFlags()
{
  int itemsAtStart_expected = 0;
  int itemsAtStart_actual = CountObjectsAt(&location);

  int item1_door1_wall1_impassable1 =createGlobalObjectAt(ITEMID_door_open, &location);
  int item2_door2_wall2_impassable2 =createGlobalObjectAt(ITEMID_door_open, &location);
  int item3_door3_wall3_impassable3 =createGlobalObjectAt(ITEMID_door_closed, &location);
  int item4_wall4_impassable_4      =createGlobalObjectAt(ITEMID_wall_1, &location);
  int item5_container1_impassable5  =createGlobalObjectAt(ITEMID_metalchest, &location);
  int item6_container2_impassable6  =createGlobalObjectAt(ITEMID_smallcrate, &location);
  int item7_container3              =createGlobalObjectAt(ITEMID_pouch, &location);

  int none_expected = 0;
  int door_expected =3;
  int wall_expected =4;
  int container_expected =3;
  int impassable_expected =6;
  int outofrange_expected=0;
  int range1_expected=impassable_expected;

  int none_actual       =getNumAllObjectsInRangewithFlags(&location, 0, TileFlag_Weapon);
  int door_actual       =getNumAllObjectsInRangewithFlags(&location, 0, TileFlag_Door);
  int wall_actual       =getNumAllObjectsInRangewithFlags(&location, 0, TileFlag_Wall);
  int container_actual  =getNumAllObjectsInRangewithFlags(&location, 0, TileFlag_Container);
  int impassable_actual =getNumAllObjectsInRangewithFlags(&location, 0, TileFlag_Impassable);
  int outofrange_actual =getNumAllObjectsInRangewithFlags(&locationPlus1, 0, TileFlag_Impassable);
  int range1_actual     =getNumAllObjectsInRangewithFlags(&locationPlus1, 1, TileFlag_Impassable);

  bool none_ok      = none_expected==none_actual;
  bool door_ok      = door_expected==door_actual;
  bool wall_ok      = wall_expected==wall_actual;
  bool container_ok = container_expected==container_actual;
  bool impassable_ok= impassable_expected==impassable_actual;
  bool outofrange_ok= outofrange_expected==outofrange_actual;
  bool range1_ok    = range1_expected==range1_actual;
  
  bool cleanedup=false;
  bool passed=false;

  if(itemsAtStart_expected!=itemsAtStart_actual)
  {
    OnTestResult(false, "World: Test_World_getNumAllObjectsInRangewithFlags halted: itemsAtStart_expected=%u itemsAtStart_actual=%u", itemsAtStart_expected, itemsAtStart_actual);
    return false;
  }

  if(!none_ok) OnTestResult(false, "World: +Test_World_getNumAllObjectsInRangewithFlags failed: none_actual=%u none_expected=%u", none_actual, none_expected);
  if(!door_ok) OnTestResult(false, "World: +Test_World_getNumAllObjectsInRangewithFlags failed: door_actual=%u door_expected=%u", door_actual, door_expected);
  if(!wall_ok) OnTestResult(false, "World: +Test_World_getNumAllObjectsInRangewithFlags failed: wall_actual=%u wall_expected=%u", wall_actual, wall_expected);
  if(!container_ok) OnTestResult(false, "World: +Test_World_getNumAllObjectsInRangewithFlags failed: container_actual=%u container_expected=%u", container_actual, container_expected);
  if(!impassable_ok) OnTestResult(false, "World: +Test_World_getNumAllObjectsInRangewithFlags failed: impassable_actual=%u impassable_expected=%u", impassable_actual, impassable_expected);

  if(!outofrange_ok) OnTestResult(false, "World: +Test_World_getNumAllObjectsInRangewithFlags failed: outofrange_actual=%u outofrange_expected=%u", outofrange_actual, outofrange_expected);
  if(!range1_ok) OnTestResult(false, "World: +Test_World_getNumAllObjectsInRangewithFlags failed: range1_actual=%u range1_expected=%u", range1_actual, range1_expected);

  cleanedup=
    deleteObject(item1_door1_wall1_impassable1) &&
    deleteObject(item2_door2_wall2_impassable2) &&
    deleteObject(item3_door3_wall3_impassable3) &&
    deleteObject(item4_wall4_impassable_4) &&
    deleteObject(item5_container1_impassable5) &&
    deleteObject(item6_container2_impassable6) &&
    deleteObject(item7_container3);

  if(!cleanedup)
    OnTestResult(false, "World: Test_World_getNumAllObjectsInRangewithFlags failed to cleanup one or more items.");

  passed = none_ok && door_ok && wall_ok && container_ok && impassable_ok && outofrange_ok && range1_ok && cleanedup;

  if(passed)
      OnTestResult(true, "World: Test_World_getNumAllObjectsInRangewithFlags passed.");

  return passed;

}

bool Test_World_getObjectsInRangeWithFlags()
{
  bool passed;

  int item1_door1_wall1_impassable1;
  int item2_door2_wall2_impassable2;
  int item3_door3_wall3_impassable3;
  int item4_wall4_impassable_4;
  int item5_container1_impassable5;
  int item6_container2_impassable6;
  int item7_container3;

  int none_expected = 0;
  int door_expected =3;
  int wall_expected =4;
  int container_expected =3;
  int impassable_expected =6;
  int outofrange_expected=0;
  int range1_expected=wall_expected;

  int none_count_actual;
  int door_count_actual;
  int wall_count_actual;
  int container_count_actual;
  int impassable_count_actual;
  int outofrange_count_actual;
  int range1_count_actual;

  bool none_ok;
  bool door_ok;
  bool wall_ok;
  bool container_ok;
  bool impassable_ok;
  bool outofrange_ok;
  bool range1_ok;

  bool passed_founddoor;
  bool passed_foundpouch;
  bool passed_nopouch;

  bool cleanedup=false;

  ListObject list_none;
  ListObject list_door;
  ListObject list_wall;
  ListObject list_container;
  ListObject list_impassable;
  ListObject list_oor;
  ListObject list_range1;

  int itemsAtStart_expected = 0;
  int itemsAtStart_actual = CountObjectsAt(&location);

  if(itemsAtStart_expected!=itemsAtStart_actual)
  {
    OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags halted: itemsAtStart_expected=%u itemsAtStart_actual=%u", itemsAtStart_expected, itemsAtStart_actual);
    return false;
  }

  initList(&list_none);
  initList(&list_door);
  initList(&list_wall);
  initList(&list_container);
  initList(&list_impassable);
  initList(&list_oor);
  initList(&list_range1);

  item1_door1_wall1_impassable1 =createGlobalObjectAt(ITEMID_door_open, &location);
  item2_door2_wall2_impassable2 =createGlobalObjectAt(ITEMID_door_open, &location);
  item3_door3_wall3_impassable3 =createGlobalObjectAt(ITEMID_door_closed, &location);
  item4_wall4_impassable_4      =createGlobalObjectAt(ITEMID_wall_1, &location);
  item5_container1_impassable5  =createGlobalObjectAt(ITEMID_metalchest, &location);
  item6_container2_impassable6  =createGlobalObjectAt(ITEMID_smallcrate, &location);
  item7_container3              =createGlobalObjectAt(ITEMID_pouch, &location);

  getObjectsInRangeWithFlags(&list_none, &location, 0, TileFlag_Weapon);
  getObjectsInRangeWithFlags(&list_door, &location, 0, TileFlag_Door);
  getObjectsInRangeWithFlags(&list_wall, &location, 0, TileFlag_Wall);
  getObjectsInRangeWithFlags(&list_container, &location, 0, TileFlag_Container);
  getObjectsInRangeWithFlags(&list_impassable, &location, 0, TileFlag_Impassable);
  getObjectsInRangeWithFlags(&list_oor, &locationPlus8, 0, TileFlag_Wall);
  getObjectsInRangeWithFlags(&list_range1, &locationPlus8, 8, TileFlag_Wall);

  none_count_actual       =list_none.Count;
  door_count_actual       =list_door.Count;
  wall_count_actual       =list_wall.Count;
  container_count_actual  =list_container.Count;
  impassable_count_actual =list_impassable.Count;
  outofrange_count_actual =list_oor.Count;
  range1_count_actual     =list_range1.Count;

  none_ok      = none_expected==none_count_actual;
  door_ok      = door_expected==door_count_actual;
  wall_ok      = wall_expected==wall_count_actual;
  container_ok = container_expected==container_count_actual;
  impassable_ok= impassable_expected==impassable_count_actual;
  outofrange_ok= outofrange_expected==outofrange_count_actual;
  range1_ok    = range1_expected==range1_count_actual;

  passed_founddoor  = List_Contains(&list_wall, VARTYPE_Object, item3_door3_wall3_impassable3);
  passed_foundpouch = List_Contains(&list_container, VARTYPE_Object, item7_container3);
  passed_nopouch    = !List_Contains(&list_wall, VARTYPE_Object, item7_container3);

  if(!none_ok) OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed: none_count_actual=%u none_expected=%u", none_count_actual, none_expected);
  if(!door_ok) OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed: door_count_actual=%u door_expected=%u", door_count_actual, door_expected);
  if(!wall_ok) OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed: wall_count_actual=%u wall_expected=%u", wall_count_actual, wall_expected);
  if(!container_ok) OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed: container_count_actual=%u container_expected=%u", container_count_actual, container_expected);
  if(!impassable_ok) OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed: impassable_count_actual=%u impassable_expected=%u", impassable_count_actual, impassable_expected);

  if(!outofrange_ok) OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed: outofrange_count_actual=%u outofrange_expected=%u", outofrange_count_actual, outofrange_expected);
  if(!range1_ok) OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed: range1_count_actual=%u range1_expected=%u", range1_count_actual, range1_expected);

  if(!passed_founddoor) OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed on passed_founddoor.");
  if(!passed_foundpouch) OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed on passed_foundpouch.");
  if(!passed_nopouch) OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed on passed_nopouch.");

  cleanedup=
    deleteObject(item1_door1_wall1_impassable1) &&
    deleteObject(item2_door2_wall2_impassable2) &&
    deleteObject(item3_door3_wall3_impassable3) &&
    deleteObject(item4_wall4_impassable_4) &&
    deleteObject(item5_container1_impassable5) &&
    deleteObject(item6_container2_impassable6) &&
    deleteObject(item7_container3);

  if(!cleanedup)
    OnTestResult(false, "World: Test_World_getObjectsInRangeWithFlags failed to cleanup one or more items.");

  passed = none_ok && door_ok && wall_ok && container_ok && impassable_ok && outofrange_ok && range1_ok && passed_founddoor && passed_foundpouch && passed_nopouch && cleanedup;

  if(passed)
    OnTestResult(true, "World: Test_World_getObjectsInRangeWithFlags passed.");
  
  return passed;

}

bool Test_World_getObjectsAt()
{
  bool passed;

  bool created_passed;
  
  bool count_passed;
  int count_expected, count_actual;

  bool founditems_passed;

  bool cleanedup;

  int serial_door_closed = createGlobalObjectAt(ITEMID_door_closed, &location);
  int serial_door_open = createGlobalObjectAt(ITEMID_door_open, &location);
  int serial_pouch = createGlobalObjectAt(ITEMID_pouch, &location);
  
  ListObject list;
  initList(&list);

  created_passed =
    serial_door_closed!=0 &&
    serial_door_open!=0 &&
    serial_pouch!=0;

  if(!created_passed)
    OnTestResult(false, "World: Test_World_getObjectsAt failed. Not all items were created");

  getObjectsAt(&list, &location);

  count_expected=3;
  count_actual=list.Count;
  count_passed= count_expected==count_actual;
  if(!count_passed)
    OnTestResult(false, "World: Test_World_getObjectsAt failed: count_expected=%u count_actual=%u", count_expected, count_actual);

  founditems_passed = 
    List_Contains(&list, VARTYPE_Object, serial_door_closed) &&
    List_Contains(&list, VARTYPE_Object, serial_door_open) &&
    List_Contains(&list, VARTYPE_Object, serial_pouch);
  if(!founditems_passed)
    OnTestResult(false, "World: Test_World_getObjectsAt failed: Not all items were returned.");

  cleanedup = 
    deleteObject(serial_door_closed) &&
    deleteObject(serial_door_open) &&
    deleteObject(serial_pouch);

  if(!cleanedup)
    OnTestResult(false, "World: Test_World_getObjectsAt failed to cleanup.");
  
  passed = created_passed && count_passed && founditems_passed && cleanedup;

  if(passed)
    OnTestResult(true, "World: Test_World_getObjectsAt passed.");

  return true;
}


// Debugging functions
int CountObjectsAt(LocationObject* location)
{
  int count;

  ListObject list_all;
  initList(&list_all);
  getObjectsAt(&list_all, location);
  count=list_all.Count;
  List_Clear(&list_all);
  return count;
}

void DebugPrintItemsAtLocation(char* description, LocationObject* location)
{
  ListObject list_all;
  initList(&list_all);
  getObjectsAt(&list_all, location);
  List_DebugPrint(description, "list_all", &list_all);
  List_Clear(&list_all);
}
