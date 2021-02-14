#pragma once

#include "Classes.h"

// Packets.cpp
namespace NativeMethods
{
    int SocketObject_SendPacket(void* pClientSocket, unsigned __int8* PacketData, unsigned int DataSize);
    void SocketObject_RemoveFirstPacket(void* pClientSocket, unsigned int PacketLength);
    void ReplaceServerPacketData(unsigned __int8 **ppCurrentPacketBuffer, unsigned int *pCurrentPacketLen, unsigned __int8 *newPacketBytes, unsigned int newPacketLength);
}

// Commands.cpp
namespace NativeMethods
{
    void SaveWorld(void);
    void ShutdownServer(void);
    void MakeCounselor(PlayerObject *Target, int CounType);
    void UnmakeCounselor(PlayerObject *Target);
}

// GameMaster.cpp
namespace NativeMethods
{
    void SendInfoWindowOrDoPlayerShadow(void* InfoStruct);
    void SendInfoWindowToGodClient(unsigned int beholderSerial, unsigned int beheldSerial);
}

// ItemObject.cpp
namespace NativeMethods
{
    void getLocation(LocationObject* outLocationObject, int itemSerial);
    int setHue(int serial, short hue);
    int getValueByFunctionFromObject(int serial, void* function, const char* debugCallString);
    int setOverloadedWeight(int serial, int weight);
    int deleteObject(int serial);
}

// Lists.cpp

namespace NativeMethods
{
    void initList(ListObject* list);
    void List_DebugPrint(char* description, char* listname, ListObject* list);

    extern "C"
    {
        void _declspec(dllexport) APIENTRY List_GetLocation(LocationObject* locationResult, ListObject* List, unsigned int index);
        int _declspec(dllexport) APIENTRY List_GetInteger(ListObject* List, unsigned int index);
        ListObject _declspec(dllexport) *APIENTRY List_GetList(ListObject* List, unsigned int index);
        int _declspec(dllexport) APIENTRY List_Append(ListObject* List, _VARTYPE ListElementType, int ListElementData);
        int _declspec(dllexport) APIENTRY List_Contains(ListObject *List, _VARTYPE ValueType, int ValueOrRef);
        int _declspec(dllexport) APIENTRY List_RemoveAt(ListObject* List, unsigned int index);
        int _declspec(dllexport) APIENTRY List_RemoveSpecificItem(ListObject *List, _VARTYPE ValueType, int ValueOrRef);
        int _declspec(dllexport) APIENTRY List_Clear(ListObject *List);
    }
}

// ObjectScripts.cpp
namespace NativeMethods
{
    extern "C"
    {
        char _declspec(dllexport) *APIENTRY addScript(int serial, const char* scriptName, int executeCreation);
        int _declspec(dllexport) APIENTRY hasScript(int serial, const char* scriptName);
        int _declspec(dllexport) APIENTRY detachScript(int serial, const char* scriptName);
    }
}

// ObjVars.cpp
namespace NativeMethods
{
    extern "C"
    {
      int _declspec(dllexport) APIENTRY setObjVarInt(int serial, const char *varName, int value);
      int _declspec(dllexport) APIENTRY setObjVarString(int serial, const char *varName, const char* value);
      int _declspec(dllexport) APIENTRY setObjVarLocation(int serial, const char *varName, void* location);
      void _declspec(dllexport) APIENTRY removeObjVar(int serial, const char *varName);
      int _declspec(dllexport) APIENTRY hasObjVarOfType(int serial, const char *varName, int varType);
      int _declspec(dllexport) APIENTRY getObjVarInt(int serial, const char *varName);
      char _declspec(dllexport) *APIENTRY getObjVarString(int serial, const char *varName);
      int _declspec(dllexport) APIENTRY getObjVarLocation(int serial, const char *varName, void* locationResult);
    }
}

// Player.cpp
namespace NativeMethods
{
    extern "C"
    {
      void _declspec(dllexport) APIENTRY SendSystemMessage(PlayerObject *player, const char *message);
      void _declspec(dllexport) APIENTRY MakeGameMaster(PlayerObject *player);
      void _declspec(dllexport) APIENTRY UnmakeGameMaster(PlayerObject *player);
      int _declspec(dllexport) APIENTRY IsGameMaster(PlayerObject *player);
    }
}

// World.cpp
namespace NativeMethods
{
    extern "C"
    {
        int _declspec(dllexport) APIENTRY createGlobalObjectAt(int ItemID, void* Location);
        int _declspec(dllexport) APIENTRY getFirstObjectOfType(void* location, int itemId);
        int _declspec(dllexport) APIENTRY getNextObjectOfType(void* location, int itemId, int lastItemSerial);
        int _declspec(dllexport) APIENTRY getNumAllObjectsInRangewithFlags(LocationObject* location, int range, unsigned int flags);
        void _declspec(dllexport) APIENTRY getObjectsInRangeWithFlags(void* resultList, LocationObject* location, int range, unsigned int flags);
        void _declspec(dllexport) APIENTRY getObjectsAt(void* resultList, LocationObject* location);

        int _declspec(dllexport) APIENTRY IsItem(void* object);
        int _declspec(dllexport) APIENTRY IsMobile(void* object);
        int _declspec(dllexport) APIENTRY IsPlayer(void* object);
    }
}