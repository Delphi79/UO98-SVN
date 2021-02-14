#pragma unmanaged

#include "Commands.h"

namespace NativeMethods
{
    extern "C"
    {
        typedef int (_cdecl *FUNCPTR_createGlobalObjectAt)(int, void*);
        #define pCOMMAND_createGlobalObjectAt 0x0041DDE4
        FUNCPTR_createGlobalObjectAt COMMAND_createGlobalObjectAt = (FUNCPTR_createGlobalObjectAt)pCOMMAND_createGlobalObjectAt;
        int _declspec(dllexport) APIENTRY createGlobalObjectAt(int ItemID, void* Location)
        {
            return COMMAND_createGlobalObjectAt(ItemID, Location);
        }

        typedef int  (_cdecl *FUNCPTR_getFirstObjectOfType)(void*, int);
        #define pCOMMAND_getFirstObjectOfType 0x00417B86
        FUNCPTR_getFirstObjectOfType COMMAND_getFirstObjectOfType = (FUNCPTR_getFirstObjectOfType)pCOMMAND_getFirstObjectOfType;
        int _declspec(dllexport) APIENTRY getFirstObjectOfType(void* location, int itemId)
        {
            return COMMAND_getFirstObjectOfType(location, itemId);
        }

        typedef int  (_cdecl *FUNCPTR_getNextObjectOfType)(void*, int, int);
        #define pCOMMAND_getNextObjectOfType 0x00417C0C
        FUNCPTR_getNextObjectOfType COMMAND_getNextObjectOfType = (FUNCPTR_getNextObjectOfType)pCOMMAND_getNextObjectOfType;
        int _declspec(dllexport) APIENTRY getNextObjectOfType(void* location, int itemId, int lastItemSerial)
        {
            return COMMAND_getNextObjectOfType(location, itemId, lastItemSerial);
        }

        // Returns the count of items within the XY range of the location matching the flags
        // flags: 0 is not understood and should be avoided
        typedef int  (_cdecl *FUNCPTR_getNumAllObjectsInRangewithFlags)(LocationObject* location, int range, unsigned int flags);
        #define pCOMMAND_getNumAllObjectsInRangewithFlags 0x00416BDD
        FUNCPTR_getNumAllObjectsInRangewithFlags COMMAND_getNumAllObjectsInRangewithFlags = (FUNCPTR_getNumAllObjectsInRangewithFlags)pCOMMAND_getNumAllObjectsInRangewithFlags;
        int _declspec(dllexport) APIENTRY getNumAllObjectsInRangewithFlags(LocationObject* location, int range, unsigned int flags)
        {
            return COMMAND_getNumAllObjectsInRangewithFlags(location, range, flags);
        }

        // Populates the passed list<int> with the serials within the XY range of the location matching the flags
        // flags: 0 is not understood and should be avoided
        // BUG: Items returned from this function may not be in the requested range. CalculateDistance (which is called by other similar functions) is not performed 
        //    on the result from FUNC_MapObject_GetObjectsInRange
        typedef int  (_cdecl *FUNCPTR_getObjectsInRangeWithFlags)(void* resultList, LocationObject* location, int range, unsigned int flags);
        #define pCOMMAND_getObjectsInRangeWithFlags 0x00416B16
        FUNCPTR_getObjectsInRangeWithFlags COMMAND_getObjectsInRangeWithFlags = (FUNCPTR_getObjectsInRangeWithFlags)pCOMMAND_getObjectsInRangeWithFlags;
        void _declspec(dllexport) APIENTRY getObjectsInRangeWithFlags(void* resultList, LocationObject* location, int range, unsigned int flags)
        {
            COMMAND_getObjectsInRangeWithFlags(resultList, location, range, flags);
        }

        // Populates the passed list<int> with the serials at the XY location
        typedef int  (_cdecl *FUNCPTR_getObjectsAt)(void* resultList, LocationObject* location);
        #define pCOMMAND_getObjectsAt 0x00417A23
        FUNCPTR_getObjectsAt COMMAND_getObjectsAt = (FUNCPTR_getObjectsAt)pCOMMAND_getObjectsAt;
        void _declspec(dllexport) APIENTRY getObjectsAt(void* resultList, LocationObject* location)
        {
            COMMAND_getObjectsAt(resultList, location);
        }
    }
}
