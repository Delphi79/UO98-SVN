#include "List.h"

void initList(ListObject* list)
{
  list->FirstElement=0;
  list->LastElement=0;
  list->Count=0;
}

void List_DebugPrint(char* description, char* listname, ListObject* list)
{
  ListElementObject* ele;

  int i=0;
  for(ele = list->FirstElement; ele!=NULL && i<list->Count; ele=ele->NextElement)
  {
    unsigned short objID;
    ItemObject* item=NULL;

    if(ele->vartype == VARTYPE_Object)
    {
      item=ConvertSerialToObject(ele->refOrValue);

      if(IsAnyItem(item) || IsAnyMobile(item))
        objID=item->ObjectType;
    }


    printf("%s debug: %s[%u]: vartype=%u value=%u id=%u\n", description, listname, i, ele->vartype, ele->refOrValue, objID);
    i++;
  }
}
