#pragma unmanaged

#include "TestsMain.h"
#include "Commands.h"

using namespace NativeMethods;

const int ITEMID_cotton=3567;

bool Tests_ObjectVars_Int(void);
bool Tests_ObjectVars_String(void);
bool Tests_ObjectVars_Location(void);

void Tests_ObjectVars_Execute()
{
    InitTestLocations();

    Tests_ObjectVars_Int();
    Tests_ObjectVars_String();
    Tests_ObjectVars_Location();
}

bool Tests_ObjectVars_Int()
{
  bool passed;

  bool passed_set;
  bool passed_get;
  bool passed_has;
  bool passed_remove;

  int value_expected=20;
  int value_actual;

  int item_serial=createGlobalObjectAt(ITEMID_cotton, &location);

  passed_set=setObjVarInt(item_serial, "testVar", value_expected);
  if(!passed_set)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_Int failed to set.");

  value_actual=getObjVarInt(item_serial, "testVar");
  passed_get = (value_actual==value_expected) && (value_actual!=0);
  if(!passed_get)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_Int failed. value_actual=%u value_expected=%u", value_actual, value_expected);

  passed_has = hasObjVarOfType(item_serial, "testVar", VARTYPE_Integer);
  if(!passed_has)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_Int failed to find.");

  removeObjVar(item_serial, "testVar");
  passed_remove = !hasObjVarOfType(item_serial, "testVar", VARTYPE_Integer);
  if(!passed_remove)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_Int failed to remove.");

  deleteObject(item_serial);

  passed = passed_set && passed_get && passed_has && passed_remove;

  if(passed)
      OnTestResult(true, "ObjectVars: Tests_ObjectVars_Int passed.");

  return passed;


}

bool Tests_ObjectVars_String()
{
  bool passed;

  bool passed_set;
  bool passed_get;
  bool passed_has;
  bool passed_remove;

  const char* value_expected="My test string.";
  char* value_actual;

  int item_serial=createGlobalObjectAt(ITEMID_cotton, &location);

  passed_set=setObjVarString(item_serial, "testVar", value_expected);
  if(!passed_set)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_String failed to set. passed_set=%u", passed_set);

  value_actual=getObjVarString(item_serial, "testVar");

  passed_get =  (value_actual!=NULL) && (strcmp(value_actual, value_expected)==0);
  if(!passed_get)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_String failed. value_actual=\"%s\" value_expected=\"%s\"", value_actual, value_expected);

  passed_has = hasObjVarOfType(item_serial, "testVar", VARTYPE_String);
  if(!passed_has)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_String failed to find.");

  removeObjVar(item_serial, "testVar");
  passed_remove = !hasObjVarOfType(item_serial, "testVar", VARTYPE_String);
  if(!passed_remove)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_String failed to remove.");

  deleteObject(item_serial);

  passed = passed_set && passed_get && passed_has && passed_remove;

  if(passed)
      OnTestResult(true, "ObjectVars: Tests_ObjectVars_String passed.");

  return passed;
}

bool Tests_ObjectVars_Location()
{
  bool passed;

  bool passed_set;
  bool passed_get;
  bool passed_has;
  bool passed_remove;

  LocationObject value_expected;
  LocationObject value_actual;

  int item_serial=createGlobalObjectAt(ITEMID_cotton, &location);

  value_expected.X=445;
  value_expected.Y=1320;
  value_expected.Z=5;

  value_actual.X=value_actual.Y=value_actual.Z=0;

  passed_set=setObjVarLocation(item_serial, "testVar", &value_expected);
  if(!passed_set)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_Location failed to set. passed_set=%u", passed_set);

  passed_get = getObjVarLocation(item_serial, "testVar", &value_actual) && (IsEqualXYZ(&value_actual, &value_expected));
  if(!passed_get)
  {
    char strLocActual[20];
    char strLocExpected[20];

    Location_ToString(strLocActual, &value_actual);
    Location_ToString(strLocExpected, &value_expected);

    OnTestResult(false, "ObjectVars: Tests_ObjectVars_Location failed. value_actual=%s value_expected=%s", strLocActual, strLocExpected);
  }

  passed_has = hasObjVarOfType(item_serial, "testVar", VARTYPE_Location);
  if(!passed_has)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_Location failed to find.");

  removeObjVar(item_serial, "testVar");
  passed_remove = !hasObjVarOfType(item_serial, "testVar", VARTYPE_Location);
  if(!passed_remove)
    OnTestResult(false, "ObjectVars: Tests_ObjectVars_Location failed to remove.");

  deleteObject(item_serial);

  passed = passed_set && passed_get && passed_has && passed_remove;

  if(passed)
      OnTestResult(true, "ObjectVars: Tests_ObjectVars_Location passed.");

  return passed;
}
