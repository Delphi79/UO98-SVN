#include "Commands.h"

#pragma unmanaged

#include "Classes.h"

namespace NativeMethods
{
    #define pCOMMAND_getLocation 0x00413884
    typedef void (_cdecl *FUNCPTR_getLocation)(LocationObject*, int);
    FUNCPTR_getLocation COMMAND_getLocation = (FUNCPTR_getLocation)pCOMMAND_getLocation;
    void getLocation(LocationObject* outLocationObject, int itemSerial)
    {
        COMMAND_getLocation(outLocationObject, itemSerial);
    }

    #define pCOMMAND_setHue 0x004124F3
    typedef int  (_cdecl *FUNCPTR_setHue)(int, short);
    FUNCPTR_setHue COMMAND_setHue = (FUNCPTR_setHue)pCOMMAND_setHue;
    int setHue(int serial, short hue)
    {
        return COMMAND_setHue(serial, hue);
    }

    #define pFUNC_getValueByFunctionFromObject 0x00411319
    typedef int  (_cdecl *FUNCPTR_getValueByFunctionFromObject)(int, void*, const char*);
    FUNCPTR_getValueByFunctionFromObject FUNC_getValueByFunctionFromObject = (FUNCPTR_getValueByFunctionFromObject)pFUNC_getValueByFunctionFromObject;
    int getValueByFunctionFromObject(int serial, void* function, const char* debugCallString)
    {
        return FUNC_getValueByFunctionFromObject(serial, (void*)function, debugCallString);
    }

    #define pFUNC_ItemObject_setOverloadedWeight 0x00490C37
    typedef int  (_cdecl *FUNCPTR_setOverloadedWeight)(int, int);
    FUNCPTR_setOverloadedWeight FUNC_ItemObject_setOverloadedWeight = (FUNCPTR_setOverloadedWeight)pFUNC_ItemObject_setOverloadedWeight;
    int setOverloadedWeight(int serial, int weight)
    {
        ItemObject *subject = (ItemObject*)ConvertSerialToObject(serial);
        if(IsAnyItem(subject))
        {
            int _EAX;
            _asm
            {
               push weight
               mov ecx, subject
               mov eax, pFUNC_ItemObject_setOverloadedWeight
               call eax
               mov _EAX, eax
            }
            return _EAX;
        }
        return 0;
    }

    #define pCOMMAND_deleteObject 0x00411D3C
    typedef int (_cdecl *FUNCPTR_deleteObject)(int);
    FUNCPTR_deleteObject COMMAND_deleteObject = (FUNCPTR_deleteObject)pCOMMAND_deleteObject;
    int deleteObject(int serial)
    {
    	return COMMAND_deleteObject(serial);
    }
}
