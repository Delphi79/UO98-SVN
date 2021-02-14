#include "Commands.h"

#pragma unmanaged

namespace NativeMethods
{
    extern "C"
    {
        #define pFUNC_AttachScriptToDynamicItemObject 0x00425F34
        typedef char* (_cdecl *FUNCPTR_AttachScriptToDynamicItemObject)(ItemObject *subject, const char* scriptName, int executeCreation);
        FUNCPTR_AttachScriptToDynamicItemObject FUNC_AttachScriptToDynamicItemObject = (FUNCPTR_AttachScriptToDynamicItemObject)pFUNC_AttachScriptToDynamicItemObject;
        char _declspec(dllexport) *APIENTRY addScript(int serial, const char* scriptName, int executeCreation)
        {
            ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
            if(subject)
                return FUNC_AttachScriptToDynamicItemObject(subject, scriptName, executeCreation);
            return "Item not found";
        }

        #define pGLOBAL_Global148andStringLookupObject 0x00698988
        #define pFUNC_FindScriptAndParseIfNeeded 0x00426106
        #define pFUNC_ItemObject_HasScript 0x004CDF4B
        int _declspec(dllexport) APIENTRY hasScript(int serial, const char* scriptName)
        {
          ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
          if(IsAnyItem(subject) || IsAnyMobile(subject))
          {
            int _EAX;
            _asm
            {
                push  scriptName
                mov   ecx, [pGLOBAL_Global148andStringLookupObject]
                mov   eax, pFUNC_FindScriptAndParseIfNeeded
                call  eax                                     // Find the script object if exists, pointer returned in EAX
                test  eax, eax
                jz    loc_return                              // Jump to return if script was not found
                push  eax
                mov   ecx, subject
                mov   eax, pFUNC_ItemObject_HasScript         // Check for this script on the subject, using the ScriptObject obtained above as the param.
                call  eax
              loc_return:
                mov _EAX, eax
            }
             return _EAX;                                    // Zero if the script was invalid, or not attached to the subject
          }
          return 0;
        }

        #define pFUNC_ItemObject_DetachScript 0x004CDDF7
        int _declspec(dllexport) APIENTRY detachScript(int serial, const char* scriptName)
        {
          ItemObject* subject = (ItemObject*)ConvertSerialToObject(serial);
          if(IsAnyItem(subject) || IsAnyMobile(subject))
          {
            _asm
            {
              push scriptName
              mov ecx, subject
              mov eax, pFUNC_ItemObject_DetachScript
              call eax
            }
            return 1;
          }
            return 0;
        }

    }
}