#pragma unmanaged

#include "Commands.h"
#include <stdio.h>

namespace NativeMethods
{
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
          item=(ItemObject*)ConvertSerialToObject(ele->Data);

          if(IsAnyItem(item) || IsAnyMobile(item))
            objID=item->ObjectType;
        }

        printf("%s debug: %s[%u]: vartype=%u value=%u id=%u\n", description, listname, i, ele->vartype, ele->Data, objID);
        i++;
      }
    }

    extern "C"
    {
        #define pCOMMAND__oprlist_cli 0x0040E863
        typedef void (_cdecl *FUNCPTR__oprlist_cli)(LocationObject* locationResult, ListObject* List, unsigned int index);
        FUNCPTR__oprlist_cli COMMAND__oprlist_cli = (FUNCPTR__oprlist_cli)pCOMMAND__oprlist_cli;
        void _declspec(dllexport) APIENTRY List_GetLocation(LocationObject* locationResult, ListObject* List, unsigned int index)
        {
            COMMAND__oprlist_cli(locationResult, List, index);
        }

        #define pCOMMAND__oprlist__ili 0x0040E733
        typedef int (_cdecl *FUNCPTR__oprlist__ili)(ListObject* List, unsigned int index);
        FUNCPTR__oprlist__ili COMMAND__oprlist__ili = (FUNCPTR__oprlist__ili)pCOMMAND__oprlist__ili;
        int _declspec(dllexport) APIENTRY List_GetInteger(ListObject* List, unsigned int index)
        {
            return COMMAND__oprlist__ili(List, index);
        }

        // List_GetList allocates and returns a new ListObject. Should implement FUNC_ListElementObject_Destructor?
        #define pCOMMAND__oprlist__lli 0x0040E698
        typedef ListObject* (_cdecl *FUNCPTR__oprlist__lli)(ListObject* List, unsigned int index);
        FUNCPTR__oprlist__lli COMMAND__oprlist__lli = (FUNCPTR__oprlist__lli)pCOMMAND__oprlist__lli;
        ListObject _declspec(dllexport) *APIENTRY List_GetList(ListObject* List, unsigned int index)
        {
            return COMMAND__oprlist__lli(List, index);
        }

        // Types of VARTYPE_List, VARTYPE_String, VARTYPE_2 and VARTYPE_Location are copied, VARTYPE_Object and other types above 5 (List) are referenced
        #define pCOMMAND_appendToList 0x0040DA53
        typedef int (_cdecl *FUNCPTR_appendToList)(ListObject* List, _VARTYPE ListElementType, int ListElementData);
        FUNCPTR_appendToList COMMAND_appendToList = (FUNCPTR_appendToList)pCOMMAND_appendToList;
        int _declspec(dllexport) APIENTRY List_Append(ListObject* List, _VARTYPE ListElementType, int ListElementData)
        {
            return COMMAND_appendToList(List, ListElementType, ListElementData);
        }

        #define pCOMMAND_isInList 0x0040E006
        typedef int (_cdecl *FUNCPTR_isInList)(ListObject *List, _VARTYPE ValueType, int ValueOrRef);
        FUNCPTR_isInList COMMAND_isInList = (FUNCPTR_isInList)pCOMMAND_isInList;
        int _declspec(dllexport) APIENTRY List_Contains(ListObject *List, _VARTYPE ValueType, int ValueOrRef)
        {
            return COMMAND_isInList(List, ValueType, ValueOrRef);
        }

        #define pCOMMAND_removeItem 0x0040E01B
        typedef int (_cdecl *FUNCPTR_removeItem)(ListObject* List, unsigned int index) ;
        FUNCPTR_removeItem COMMAND_removeItem= (FUNCPTR_removeItem)pCOMMAND_removeItem;
        int _declspec(dllexport) APIENTRY List_RemoveAt(ListObject* List, unsigned int index)
        {
            return COMMAND_removeItem(List, index);
        }

        #define pCOMMAND_removeSpecificItem 0x0040E06F
        typedef int (_cdecl *FUNCPTR_removeSpecificItem)(ListObject *List, _VARTYPE ValueType, int ValueOrRef);
        FUNCPTR_removeSpecificItem COMMAND_removeSpecificItem = (FUNCPTR_removeSpecificItem)pCOMMAND_removeSpecificItem;
        int _declspec(dllexport) APIENTRY List_RemoveSpecificItem(ListObject *List, _VARTYPE ValueType, int ValueOrRef)
        {
            return COMMAND_removeSpecificItem(List, ValueType, ValueOrRef);
        }

        #define pCOMMAND_clearList 0x0040E084
        typedef int (_cdecl *FUNCPTR_clearList)(ListObject *List);
        FUNCPTR_clearList COMMAND_clearList = (FUNCPTR_clearList)pCOMMAND_clearList;
        int _declspec(dllexport) APIENTRY List_Clear(ListObject *List)
        {
            return COMMAND_clearList(List);
        }

    }
}