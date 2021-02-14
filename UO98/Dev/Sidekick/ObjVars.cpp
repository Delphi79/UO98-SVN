#include "Commands.h"

#pragma unmanaged

#define pFUNC_SetWomVar 0x004C8EEB
typedef bool (_cdecl *FUNCPTR_SetWomVar)(ItemObject *ARG_subject, const char *ARG_VarName, _VARTYPE ARG_VarType, int ARG_ValueOrPointer);
FUNCPTR_SetWomVar FUNC_SetWomVar = (FUNCPTR_SetWomVar)pFUNC_SetWomVar;

#define pFUNC_ItemObject_RemoveObjVar 0x004CDEAC
#define pFUNC_ItemObject_HasObjVarOfType 0x004CDCD4
#define pFUNC_ItemObject_FindAndGetObjVarValue 0x004CDD01
#define pFUNC_ItemObject_GetObjVarString 0x004CDD7C
#define pFUNC_ItemObject_GetObjVarLocation 0x004CDD53

namespace NativeMethods
{
    extern "C"
    {
        int _declspec(dllexport) APIENTRY setObjVarInt(int serial, const char *varName, int value)
        {
            ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
            if(IsAnyItem(subject) || IsAnyMobile(subject))
            {
                FUNC_SetWomVar(subject, varName, VARTYPE_Integer, value);
                return 1;
            }
            return 0;
        }

        int _declspec(dllexport) APIENTRY setObjVarString(int serial, const char *varName, const char* value)
        {
            ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
            if(IsAnyItem(subject) || IsAnyMobile(subject))
            {
                FUNC_SetWomVar(subject, varName, VARTYPE_String, (int)value);
                return 1;
            }
            return 0;
        }

        int _declspec(dllexport) APIENTRY setObjVarLocation(int serial, const char *varName, void* location)
        {
            ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
            if(IsAnyItem(subject) || IsAnyMobile(subject))
            {
                FUNC_SetWomVar(subject, varName, VARTYPE_Location, (int)location);
                return 1;
            }
            return 0;
        }

        void _declspec(dllexport) APIENTRY removeObjVar(int serial, const char *varName)
        {
            ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
            if(IsAnyItem(subject) || IsAnyMobile(subject))
            {
                _asm
                {
                    push varName
                        mov ecx, subject
                        mov eax, pFUNC_ItemObject_RemoveObjVar
                        call eax
                }
            }
        }

        int _declspec(dllexport) APIENTRY hasObjVarOfType(int serial, const char *varName, int varType)
        {
            ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
            if(IsAnyItem(subject) || IsAnyMobile(subject))
            {
                int _EAX;
                _asm
                {
                    push varType
                    push varName
                    mov ecx, subject
                    mov eax, pFUNC_ItemObject_HasObjVarOfType
                    call eax
                    mov _EAX, eax
                }
                return _EAX;
            }
            return 0;
        }

        int _declspec(dllexport) APIENTRY getObjVarInt(int serial, const char *varName)
        {
            if(hasObjVarOfType(serial, varName, VARTYPE_Integer))
            {
                ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
                if( (IsAnyItem(subject)||IsAnyMobile(subject)) )
                {
                    int result;
                    int* pResult=&result;

                    _asm
                    {
                        push pResult
                        push varName
                        mov ecx, subject
                        mov eax, pFUNC_ItemObject_FindAndGetObjVarValue
                        call eax
                    }
                    return result;
                }
            }
            return 0;
        }

        char _declspec(dllexport) *APIENTRY getObjVarString(int serial, const char *varName)
        {
            if(hasObjVarOfType(serial, varName, VARTYPE_String))
            {
                ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
                if( (IsAnyItem(subject)||IsAnyMobile(subject)) )
                {
                    int _EAX;
                    _asm
                    {
                        push varName
                        mov ecx, subject
                        mov eax, pFUNC_ItemObject_GetObjVarString
                        call eax
                        mov _EAX, eax
                    }
                    return *(char**)_EAX; // StringObject->StringMemory
                }
            }
            return 0;
        }

        int _declspec(dllexport) APIENTRY getObjVarLocation(int serial, const char *varName, void* locationResult)
        {
            ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
            if( (IsAnyItem(subject)||IsAnyMobile(subject)) )
            {
                int _EAX;
                _asm
                {
                    push locationResult
                    push varName
                    mov ecx, subject
                    mov eax, pFUNC_ItemObject_GetObjVarLocation
                    call eax
                    mov _EAX, eax
                }
                return _EAX;
            }
            return 0;
        }
    }
}