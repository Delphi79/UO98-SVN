#pragma unmanaged

#include "TestsMain.h"
#include "Commands.h"

using namespace NativeMethods;

const int ITEMID_sackofflour=4153;

bool Tests_ObjectScripts_Item(void);

void Tests_ObjectScripts_Execute()
{
    InitTestLocations();

    Tests_ObjectScripts_Item();
}

bool Tests_ObjectScripts_Item()
{
  bool passed;

  bool passed_has0;
  bool passed_has1;
  bool passed_has2;
  bool passed_attach;
  bool passed_has3;
  bool passed_detach;
  bool passed_has4;
  bool passed_has5;
  bool passed_hasnot;

  char* addResult;

  int item_serial=createGlobalObjectAt(ITEMID_sackofflour, &location);

  passed_has0 = !hasScript(item_serial, "doesnotexist");
  if(!passed_has0)
    OnTestResult(false, "ObjectVars: Tests_ObjectScripts_Item failed. Found invalid script \"doesnotexist\".");

  passed_has1 = hasScript(item_serial, "4153");
  if(!passed_has1)
    OnTestResult(false, "ObjectVars: Tests_ObjectScripts_Item failed to find script 4153 after creation.");

  passed_has2 = !hasScript(item_serial, "test");
  if(!passed_has2)
    OnTestResult(false, "ObjectVars: Tests_ObjectScripts_Item found script \"test\" before attach.");

  addResult = addScript(item_serial, "test", true);
  passed_attach = addResult==NULL;
  if(!passed_attach)
    OnTestResult(false, "ObjectVars: Tests_ObjectScripts_Item attachScript returned: %s.", addResult);

  passed_has3 = hasScript(item_serial, "test");
  if(!passed_has3)
    OnTestResult(false, "ObjectVars: Tests_ObjectScripts_Item failed to find script \"test\" after attach.");

  passed_detach = detachScript(item_serial, "test") && !hasScript(item_serial, "test");
  if(!passed_detach)
    OnTestResult(false, "ObjectVars: Tests_ObjectScripts_Item detachScript returned false.");

  passed_has4 = !hasScript(item_serial, "test");
  if(!passed_has4)
    OnTestResult(false, "ObjectVars: Tests_ObjectScripts_Item found script \"test\" after detatch.");

  passed_has5 = hasScript(item_serial, "4153");
  if(!passed_has5)
    OnTestResult(false, "ObjectVars: Tests_ObjectScripts_Item failed to find script 4153 after detatch.");

  passed_hasnot=!hasScript(0,"test");
  if(!passed_hasnot)
    OnTestResult(false, "ObjectVars: Tests_ObjectScripts_Item Found a script on item with serial zero.");

  passed = passed_has0 && passed_has1 && passed_has2 && passed_attach && passed_has3 && passed_detach && passed_has4 && passed_has5 && passed_hasnot;

  if(passed)
    OnTestResult(true, "ObjectScripts: Tests_ObjectScripts_Item passed.");

  return passed;

}