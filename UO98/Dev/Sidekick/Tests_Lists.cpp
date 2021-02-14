#pragma unmanaged

#include "TestsMain.h"
#include "Commands.h"

using namespace NativeMethods;

bool Tests_Lists_SimpleAppend(void);
bool Tests_Lists_Contains(void);
bool Tests_Lists_OfInteger(void);
bool Tests_Lists_OfLocation(void);
bool Tests_Lists_OfList(void);
bool Tests_Lists_RemoveAt(void);
bool Tests_Lists_RemoveSpecificItem(void);
bool Tests_Lists_ClearList(void);

void Tests_Lists_Execute()
{
  Tests_Lists_SimpleAppend();
  Tests_Lists_Contains();
  Tests_Lists_OfInteger();
  Tests_Lists_OfLocation();
  Tests_Lists_OfList();
  Tests_Lists_RemoveAt();
  Tests_Lists_RemoveSpecificItem();
  Tests_Lists_ClearList();
}

bool Tests_Lists_SimpleAppend()
{
  bool passed;

  int append_result;  
  int append_expected;
  bool append_passed;

  LocationObject testLoc;

  ListObject list;
  initList(&list);

  append_expected=(int)(&list);
  append_result = List_Append(&list, VARTYPE_Location, (int)&testLoc);
  append_passed = append_result == append_expected && list.Count==1;

  if(!append_passed)
    OnTestResult(false, "Lists: Tests_Lists_SimpleAppend failed. append_result=%u append_expected=%u list.Count=%u.", append_result, append_expected, list.Count);
   
  passed= append_passed;

  if(passed)
    OnTestResult(true, "Lists: Tests_Lists_SimpleAppend passed.");

  return passed;
}

bool Tests_Lists_Contains()
{
  bool passed;

  bool actual_contains;
  bool actual_notcontains;

  ListObject list;
  initList(&list);

  List_Append(&list, VARTYPE_Integer, 2);
  List_Append(&list, VARTYPE_Integer, 4);
  List_Append(&list, VARTYPE_Integer, 6);
  List_Append(&list, VARTYPE_Integer, 8);

  actual_contains = 
    List_Contains(&list, VARTYPE_Integer, 2) &&
    List_Contains(&list, VARTYPE_Integer, 4) &&
    List_Contains(&list, VARTYPE_Integer, 6) &&
    List_Contains(&list, VARTYPE_Integer, 8);

  if(!actual_contains)
    OnTestResult(false, "Lists: Tests_Lists_Contains failed. One or more of the expected elements were not found.");

  actual_notcontains = 
    List_Contains(&list, VARTYPE_Integer, 1) ||
    List_Contains(&list, VARTYPE_Integer, 3) ||
    List_Contains(&list, VARTYPE_Integer, 77) ||
    List_Contains(&list, VARTYPE_Integer, -1);

  if(actual_notcontains)
    OnTestResult(false, "Lists: Tests_Lists_Contains failed. One or more of the not-expected elements were found.");

  passed = actual_contains && !actual_notcontains;

  if(passed)
    OnTestResult(true, "Lists: Tests_Lists_Contains passed.");

  return passed;
}

bool Tests_Lists_OfInteger()
{
  bool passed;

  int count_actual, count_expected;
  bool count_passed;

  int item0_expected, item1_expected, item2_expected, item3_expected, iteminvalid_expected;
  int item0_actual, item1_actual, item2_actual, item3_actual, iteminvalid_actual;
  bool item0_passed, item1_passed, item2_passed, item3_passed, iteminvalid_passed;

  ListObject list1;
  initList(&list1);

  item0_expected = -1;
  item1_expected = 42;
  item2_expected = 75;
  item3_expected = 7632;

  List_Append(&list1, VARTYPE_Integer, item0_expected);
  List_Append(&list1, VARTYPE_Integer, item1_expected);
  List_Append(&list1, VARTYPE_Integer, item2_expected);
  List_Append(&list1, VARTYPE_Integer, item3_expected);

  count_expected = 4;
  count_actual = list1.Count;
  count_passed = count_actual==count_expected;

  if(!count_passed)
    OnTestResult(false, "Lists: Tests_Lists_OfInteger failed. count_actual=%u count_expected=%u", count_actual, count_expected);

  item0_actual=List_GetInteger(&list1,0);
  item0_passed = item0_actual==item0_expected;
  if(!item0_passed) 
    OnTestResult(false, "Lists: Tests_Lists_OfInteger failed. item0_actual=%u item0_expected=%u", item0_actual, item0_expected);

  item1_actual=List_GetInteger(&list1,1);
  item1_passed = item1_actual==item1_expected;
  if(!item1_passed) 
    OnTestResult(false, "Lists: Tests_Lists_OfInteger failed. item1_actual=%u item1_expected=%u", item1_actual, item1_expected);

  item2_actual=List_GetInteger(&list1,2);
  item2_passed = item2_actual==item2_expected;
  if(!item2_passed) 
    OnTestResult(false, "Lists: Tests_Lists_OfInteger failed. item2_actual=%u item2_expected=%u", item2_actual, item2_expected);

  item3_actual=List_GetInteger(&list1,3);
  item3_passed = item3_actual==item3_expected;
  if(!item3_passed) 
    OnTestResult(false, "Lists: Tests_Lists_OfInteger failed. item3_actual=%u item3_expected=%u", item3_actual, item3_expected);

  iteminvalid_expected=0;
  iteminvalid_actual=List_GetInteger(&list1,4);
  iteminvalid_passed = iteminvalid_actual==iteminvalid_expected;
  if(!iteminvalid_passed) 
    OnTestResult(false, "Lists: Tests_Lists_OfInteger failed. iteminvalid_actual=%u iteminvalid_expected=%u", iteminvalid_actual, iteminvalid_expected);

  passed = count_passed && item0_passed && item1_passed && item2_passed && item3_passed;

  if(passed)
    OnTestResult(true, "Lists: Tests_Lists_OfInteger passed.");

  return passed;
}

bool Tests_Lists_OfLocation()
{
  bool passed;

  int count_actual, count_expected;
  bool count_passed;

  int X_actual, X_expected;  
  bool X_passed;

  bool IsEqualXYZ_passed;

  LocationObject loc_ToAdd;
  LocationObject loc_ToGet;

  ListObject list1;
  initList(&list1);

  initLocation(&loc_ToAdd);

  X_expected = 50;
  loc_ToAdd.X = X_expected;

  List_Append(&list1, VARTYPE_Location, (int)&loc_ToAdd);

  count_expected=1;
  count_actual = list1.Count;
  count_passed = count_actual==count_expected;

  if(!count_passed)
    OnTestResult(false, "Lists: Tests_Lists_OfLocation failed. count_actual=%u count_expected=%u", count_actual, count_expected);

  initLocation(&loc_ToGet);
  List_GetLocation(&loc_ToGet, &list1, 0);

  X_actual = loc_ToGet.X;
  X_passed = X_actual==X_expected;

  if(!X_passed)
    OnTestResult(false, "Lists: Tests_Lists_OfLocation failed. X_actual=%u X_expected=%u", X_actual, X_expected);
  
  IsEqualXYZ_passed = IsEqualXYZ(&loc_ToAdd, &loc_ToGet);
  if(!IsEqualXYZ_passed)
  {
    char sloc_ToAdd[20];
    char sloc_ToGet[20];

    Location_ToString(sloc_ToAdd, &loc_ToAdd);
    Location_ToString(sloc_ToGet, &loc_ToGet);

    OnTestResult(false, "Lists: Tests_Lists_OfLocation failed. added:%s got:%s", sloc_ToAdd, sloc_ToGet);
  }

  passed = count_passed && X_passed && IsEqualXYZ_passed;

  if(passed)
    OnTestResult(true, "Lists: Tests_Lists_OfLocation passed.");

  return passed;
}

bool Tests_Lists_OfList()
{
  bool passed;

  int list0_item2_expected, list0_item2_actual;
  bool list0_item2_passed;

  int list1_item1_expected, list1_item1_actual;
  bool list1_item1_passed;

  ListObject* plist_outofrange;
  bool list_outofrange_notnull_passed;
  bool list_outofrange_countzero_passed;

  ListObject list_outer;
  ListObject list_inner0;
  ListObject list_inner1;

  ListObject* plist_result_0;
  ListObject* plist_result_1;

  initList(&list_outer);
  initList(&list_inner0);
  initList(&list_inner1);

  list0_item2_expected = 77;
  list1_item1_expected = 832;

  List_Append(&list_inner0, VARTYPE_Integer, 0);
  List_Append(&list_inner0, VARTYPE_Integer, 1);
  List_Append(&list_inner0, VARTYPE_Integer, list0_item2_expected);
  List_Append(&list_inner0, VARTYPE_Integer, 3);

  List_Append(&list_inner1, VARTYPE_Integer, 4);
  List_Append(&list_inner1, VARTYPE_Integer, list1_item1_expected);
  List_Append(&list_inner1, VARTYPE_Integer, 6);

  List_Append(&list_outer, VARTYPE_List, (int)&list_inner0);
  List_Append(&list_outer, VARTYPE_List, (int)&list_inner1);

  plist_result_0 = List_GetList(&list_outer, 0);
  plist_result_1 = List_GetList(&list_outer, 1);

  list0_item2_actual=List_GetInteger(plist_result_0, 2);
  list0_item2_passed = list0_item2_actual==list0_item2_expected;
  if(!list0_item2_passed) 
    OnTestResult(false, "Lists: Tests_Lists_OfList failed. list0_item2_actual=%u list0_item2_expected=%u", list0_item2_actual, list0_item2_expected);

  list1_item1_actual=List_GetInteger(plist_result_1, 1);
  list1_item1_passed = list1_item1_actual==list1_item1_expected;
  if(!list1_item1_passed) 
    OnTestResult(false, "Lists: Tests_Lists_OfList failed. list1_item1_actual=%u list1_item1_expected=%u", list1_item1_actual, list1_item1_expected);


  plist_outofrange = List_GetList(&list_outer, list_outer.Count+882);
  
  list_outofrange_notnull_passed=plist_outofrange!=NULL;
  
  if(!list_outofrange_notnull_passed) 
    OnTestResult(false, "Lists: Tests_Lists_OfList failed. plist_outofrange is NULL");
  else
  {
    list_outofrange_countzero_passed = plist_outofrange->Count==0;
    if(!list_outofrange_countzero_passed) 
      OnTestResult(false, "Lists: Tests_Lists_OfList failed. plist_outofrange->count=%u", plist_outofrange->Count);
  }

  passed = list0_item2_passed && list1_item1_passed && list_outofrange_notnull_passed && list_outofrange_countzero_passed;

  if(passed)
    OnTestResult(true, "Lists: Tests_Lists_OfList passed.");

  return passed;
}

bool Tests_Lists_RemoveAt()
{
  bool passed;

  int value_expected1, value_actual1;
  bool value_passed1;

  int value_expected2, value_actual2;
  bool value_passed2;

  int count_expected1, count_actual1;
  bool count_passed1;

  int count_expected2, count_actual2;
  bool count_passed2;

  ListObject list1;
  initList(&list1);

  value_expected1 = 22;
  value_expected2 = 88;

  List_Append(&list1, VARTYPE_Integer, 1);
  List_Append(&list1, VARTYPE_Integer, value_expected1);
  List_Append(&list1, VARTYPE_Integer, value_expected2);

  count_expected1 = 3;
  count_actual1 = list1.Count;

  List_RemoveAt(&list1, 0);

  count_expected2 = 2;
  count_actual2 = list1.Count;

  value_actual1 = List_GetInteger(&list1, 0);
  value_actual2 = List_GetInteger(&list1, 1);

  count_passed1 = count_expected1==count_actual1;
  if(!count_passed1) 
    OnTestResult(false, "Lists: Tests_Lists_RemoveAt failed. count_expected1=%u count_actual1=%u", count_expected1, count_actual1);
  
  count_passed2 = count_expected2==count_actual2;
  if(!count_passed2) 
    OnTestResult(false, "Lists: Tests_Lists_RemoveAt failed. count_expected2=%u count_actual2=%u", count_expected2, count_actual2);

  value_passed1 = value_expected1==value_actual1;
  if(!value_passed1) 
    OnTestResult(false, "Lists: Tests_Lists_RemoveAt failed. value_expected1=%u value_actual1=%u", value_expected1, value_actual1);

  value_passed2 = value_expected2==value_actual2;
  if(!value_passed2) 
    OnTestResult(false, "Lists: Tests_Lists_RemoveAt failed. value_expected2=%u value_actual2=%u", value_expected2, value_actual2);

  passed = count_passed1 && count_passed2 && value_passed1 && value_passed2;

  if(passed)
    OnTestResult(true, "Lists: Tests_Lists_RemoveAt passed.");

  return passed;
}

bool Tests_Lists_RemoveSpecificItem()
{
  bool passed;

  int count_expected1, count_actual1;
  bool count_passed1;

  int count_expected2, count_actual2;
  bool count_passed2;

  int value_expected1, value_actual1;
  bool value_passed1;

  int value_expected2, value_actual2;
  bool value_passed2;

  ListObject list1;
  initList(&list1);

  List_Append(&list1, VARTYPE_Integer, 1);
  List_Append(&list1, VARTYPE_Integer, 2);
  List_Append(&list1, VARTYPE_Integer, 3);
  List_Append(&list1, VARTYPE_Integer, 7);
  List_Append(&list1, VARTYPE_Integer, 4);
  List_Append(&list1, VARTYPE_Integer, 3);
  List_Append(&list1, VARTYPE_Integer, 2);

  List_RemoveSpecificItem(&list1, VARTYPE_Integer, 3);

  count_expected1 = 6;
  count_actual1 = list1.Count;
  count_passed1 = count_expected1==count_actual1;

  if(!count_passed1) 
    OnTestResult(false, "Lists: Tests_Lists_RemoveSpecificItem failed. count_expected1=%u count_actual1=%u", count_expected1, count_actual1);

  List_RemoveSpecificItem(&list1, VARTYPE_Integer, 2);

  count_expected2 = 5;
  count_actual2 = list1.Count;
  count_passed2 = count_expected2==count_actual2;

  count_passed2 = count_expected2==count_actual2;
  if(!count_passed2) 
    OnTestResult(false, "Lists: Tests_Lists_RemoveSpecificItem failed. count_expected2=%u count_actual2=%u", count_expected2, count_actual2);

  value_expected1=7;
  value_expected2=4;

  value_actual1 = List_GetInteger(&list1, 1);
  value_actual2 = List_GetInteger(&list1, 2);

  value_passed1 = value_expected1==value_actual1;
  if(!value_passed1) 
    OnTestResult(false, "Lists: Tests_Lists_RemoveSpecificItem failed. value_expected1=%u value_actual1=%u", value_expected1, value_actual1);

  value_passed2 = value_expected2==value_actual2;
  if(!value_passed2) 
    OnTestResult(false, "Lists: Tests_Lists_RemoveSpecificItem failed. value_expected2=%u value_actual2=%u", value_expected2, value_actual2);

  passed = count_passed1 && count_passed2 && value_passed1 && value_passed2;

  if(passed)
    OnTestResult(true, "Lists: Tests_Lists_RemoveSpecificItem passed.");

  return passed;
}

bool Tests_Lists_ClearList()
{
  bool passed=0;

  int count_before_expected, count_before_actual;
  bool count_before_passed;

  int count_after_expected, count_after_actual;
  bool count__after_passed;

  ListObject list1;
  initList(&list1);

  List_Append(&list1, VARTYPE_Integer, 1);
  List_Append(&list1, VARTYPE_Integer, 2);
  List_Append(&list1, VARTYPE_Integer, 3);
  List_Append(&list1, VARTYPE_Integer, 7);

  count_before_expected=4;
  count_before_actual=list1.Count;
  
  List_Clear(&list1);
  
  count_after_expected=0;
  count_after_actual=list1.Count;

  count_before_passed = count_before_actual==count_before_expected;
  if(!count_before_passed) 
    OnTestResult(false, "Lists: Tests_Lists_ClearList failed. count_before_actual=%u count_before_expected=%u", count_before_actual, count_before_expected);

  count__after_passed = count_after_actual==count_after_expected;
  if(!count__after_passed) 
    OnTestResult(false, "Lists: Tests_Lists_ClearList failed. count_after_actual=%u count_after_expected=%u", count_after_actual, count_after_expected);

  passed = count_before_passed && count__after_passed;

  if(passed)
    OnTestResult(true, "Lists: Tests_Lists_ClearList passed.");

  return passed;

}