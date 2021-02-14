using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick
{
    unsafe interface ICore : IUOServer
    {
        void SendSystemMessage(class_Player* player, byte* message);
        int createGlobalObjectAt(int ItemID, Location* Location);

        int setObjVarInt(int serial, byte* name, int value);
        int setObjVarString(int serial, byte* name, byte* value);
        int setObjVarLocation(int serial, byte* name, Location* value);
        void removeObjVar(int serial, byte* varName);
        bool hasObjVarOfType(int serial, byte* varName, VariableType varType);
        int getObjVarInt(int serial, byte* varName);
        byte* getObjVarString(int serial, byte* varName);
        bool getObjVarLocation(int serial, byte* varName, Location* locationResult);

        byte* addScript(int serial, byte* scriptName, int executeCreation);
        bool hasScript(int serial, byte* scriptName);
        bool detachScript(int serial, byte* scriptName);

        int getFirstObjectOfType(Location* location, int itemId);
        int getNextObjectOfType(Location* location, int itemId, int lastItemSerial);

        void MakeGameMaster(PlayerObject* Target);
        void UnmakeGameMaster(PlayerObject* Target);
        int IsGameMaster(PlayerObject* Target);
        void OpenBank(class_Player* Target, class_Player* ShowTo);

        ITimeManager TimeManager { get; }
        IServerConfiguration ServerConfiguration { get; }
        ISpawnLimits SpawnLimits { get; }
        IResources Resources { get; }
        ISkillsObject SkillsObject { get; }
    }
}
