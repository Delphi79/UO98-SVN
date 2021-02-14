#pragma once

#include "stdafx.h"

#pragma once

#pragma warn -8002 // Restarting compile using assembly

// Functions to retrieve/get data
#define GetSByte(address, reladdress)  ((  signed char )(*(  signed char  *)((char*)address+reladdress)))
#define GetUByte(address, reladdress)  ((unsigned char )(*(unsigned char  *)((char*)address+reladdress)))
#define GetSWord(address, reladdress)  ((  signed short)(*(  signed short *)((char*)address+reladdress)))
#define GetUWord(address, reladdress)  ((unsigned short)(*(unsigned short *)((char*)address+reladdress)))
#define GetSDWord(address, reladdress) ((  signed long )(*(  signed long  *)((char*)address+reladdress)))
#define GetUDWord(address, reladdress) ((unsigned long )(*(unsigned long  *)((char*)address+reladdress)))

// Functions to change/set data
#define SetSByte(address, reladdress, value)  *(  signed char  *)((char*)address+reladdress) = (  signed char )value
#define SetUByte(address, reladdress, value)  *(unsigned char  *)((char*)address+reladdress) = (unsigned char )value
#define SetSWord(address, reladdress, value)  *(  signed short *)((char*)address+reladdress) = (  signed short)value
#define SetUWord(address, reladdress, value)  *(unsigned short *)((char*)address+reladdress) = (unsigned short)value
#define SetSDWord(address, reladdress, value) *(  signed long  *)((char*)address+reladdress) = (  signed long )value
#define SetUDWord(address, reladdress, value) *(unsigned long  *)((char*)address+reladdress) = (unsigned long )value

// THISCALL, always use this macro in your class functions as the first line!!!
#define GETTHIS(x) unsigned int x; __asm { mov x, ecx }

// It is critical for compatibility that structures be packed on 1 byte boundaries. Use compiler option: Struct member Alignment = 1 Byte (/Zp1)

namespace NativeMethods
{
  void* ConvertSerialToObject(unsigned int serial);
}

// VTables
#define GetVTable(baseaddress) (GetUDWord(baseaddress, 0))

#define VTABLE_ITEM             (0x005EF620)
#define VTABLE_ITEMONMAPOBJECT  (0x005EF988)
#define VTABLE_WEAPON           (0x005F00F8)
#define VTABLE_CONTAINEROBJECT  (0x005EE888)
#define VTABLE_CORPSEOBJECT     (0x005EF7C8)
#define VTABLE_BULLETINBOARD    (0x005EE690)
#define VTABLE_MOBILE           (0x005EEF48)
#define VTABLE_PLAYER           (0x005EEA50)
#define VTABLE_NPC              (0x005EF1F8)
#define VTABLE_GUARD            (0x005EECF0)
#define VTABLE_SHOPKEEPER       (0x005EFDE8)

// Class/Object Identification
#define IsItemObject(baseaddress)           ((DWORD)baseaddress && GetVTable(baseaddress) == VTABLE_ITEM)
#define IsItemOnMapObject(baseaddress)      ((DWORD)baseaddress && GetVTable(baseaddress) == VTABLE_ITEMONMAPOBJECT)
#define IsWeaponObject(baseaddress)         ((DWORD)baseaddress && GetVTable(baseaddress) == VTABLE_WEAPON)
#define IsContainerObject(baseaddress)      ((DWORD)baseaddress && GetVTable(baseaddress) == VTABLE_CONTAINEROBJECT)
#define IsCorspeObject(baseaddress)         ((DWORD)baseaddress && GetVTable(baseaddress) == VTABLE_CORPSEOBJECT)
#define IsBulletinBoardObject(baseaddress)  ((DWORD)baseaddress && GetVTable(baseaddress) == VTABLE_BULLETINBOARD)

#define IsGuardObject(baseaddress)          ((DWORD)baseaddress && GetVTable(baseaddress) == VTABLE_GUARD)
#define IsShopKeeperObject(baseaddress)     ((DWORD)baseaddress && GetVTable(baseaddress) == VTABLE_SHOPKEEPER)
#define IsNPCobject(baseaddress)            ((DWORD)baseaddress && (GetVTable(baseaddress) == VTABLE_NPC || IsGuardObject(baseaddress) || IsShopKeeperObject(baseaddress)))
#define IsPlayerObject(baseaddress)         ((DWORD)baseaddress && GetVTable(baseaddress) == VTABLE_PLAYER)
#define IsMobileObject(baseaddress)         ((DWORD)baseaddress && (GetVTable(baseaddress) == VTABLE_MOBILE || IsPlayerObject(baseaddress) || IsNPCobject(baseaddress)))

#define IsAnyItem(baseaddress)    (IsItemObject(baseaddress) || IsItemOnMapObject(baseaddress) || IsWeaponObject(baseaddress) || IsContainerObject(baseaddress) || IsCorspeObject(baseaddress) || IsBulletinBoardObject(baseaddress))
#define IsAnyNPC(baseaddress)     (IsGuardObject(baseaddress) || IsShopKeeperObject(baseaddress) || IsNPCobject(baseaddress) )
#define IsAnyMobile(baseaddress)  (IsAnyNPC(baseaddress) || IsPlayerObject(baseaddress))

/// Enums
typedef public enum _VARTYPE
{
  VARTYPE_Integer = 0,
  VARTYPE_String  = 1,
  VARTYPE_UNKNOWN_2  = 2,
  VARTYPE_Location  = 3,
  VARTYPE_Object  = 4,
  VARTYPE_List  = 5,
  VARTYPE_UNKNOWN_6  = 6,
  VARTYPE_Unknown  = 7
} _VARTYPE;

// ListElement size=0x10
typedef public struct class_ListElement       
{ 
  struct class_ListElement* NextElement; 
  struct class_ListElement* PreviousElement; 
  _VARTYPE vartype;
  unsigned int Data;
} ListElementObject;

// ListObject size=0xC
typedef struct class_List
{ 
  struct class_ListElement* FirstElement; 
  struct class_ListElement* LastElement; 
  unsigned __int32 Count;
} ListObject;

struct class_Double3
{
  unsigned __int32 A1;
  unsigned __int32 A2;
  unsigned __int32 B1;
  unsigned __int32 B2;
  unsigned __int32 C1;
  unsigned __int32 C2;
};

struct struct_SkillObject
{
  unsigned __int8 Name[80];
  unsigned __int8 Script[80];
  unsigned __int32 StrReq;
  unsigned __int32 DexReq;
  unsigned __int32 IntReq;
  unsigned __int32 Strength;
  unsigned __int32 Dexterity;
  unsigned __int32 Intelligence;
  class_Double3 AdvRate;
  unsigned __int32 StatAdvRate;
  unsigned __int32 SkillStat;
  unsigned __int32 CanUse;
  unsigned __int32 SkillWeight;
  unsigned __int32 IsEnabled__Always1;
  unsigned __int32 Version;
};

struct class_SkillsObject
{
  unsigned __int32 PerSkillSomeValue[50];
  unsigned __int32 PerSkillUsageCounter[50];
  unsigned __int32 AllSkillSomeTotal;
  unsigned __int32 AllSkillUsageCounter;
  struct_SkillObject SkillArray;
  unsigned __int32 SkillCount;
};

struct vtable_ItemClass
{
  unsigned __int32 Destructor;
  unsigned __int32 PlaceSomewhere;  // ?
  unsigned __int32 field_8;
  unsigned __int32 field_C;
  unsigned __int32 NextStaticItem;
  unsigned __int32 field_14;
  void *IsPlayer;
  unsigned __int32 field_1C;
  unsigned __int32 IsGeneric;
  unsigned __int32 CalculateBaseValue;
  unsigned __int32 GetHeight;
  unsigned __int32 GetSurfaceHeight;
  unsigned __int32 GetObjectFlags;
  unsigned __int32 GetRealName;
  unsigned __int32 IsHair;
  unsigned __int32 field_3C;
  unsigned __int16 field_40;
  unsigned __int8 __unnamed76[10];
  unsigned __int32 GetName;
  unsigned __int32 Bark;
  unsigned __int32 SuperBark;
  unsigned __int32 eBarkUnicode;
  unsigned __int32 eBark;
  unsigned __int32 BarkToHued1;
  unsigned __int32 BarkToHued0;
  unsigned __int32 BarkToUnicode;
  unsigned __int32 BarkTo;
  unsigned __int8 __unnamed120[8];
  unsigned __int32 eBarkToUnicode;
  unsigned __int32 eBarkTo;
  unsigned __int32 GetLocationObject;
  unsigned __int8 __unnamed144[12];
  unsigned __int32 Delete;
  unsigned __int32 GetMovementType;
  unsigned __int32 GetQuantity;
  unsigned __int8 __unnamed172[16];
  unsigned __int32 field_AC;
  unsigned __int8 __unnamed180[4];
  unsigned __int32 SetLocationOfContainedObject;
  unsigned __int8 __unnamed192[8];
  unsigned __int32 LiftEquip;
  unsigned __int8 __unnamed200[4];
  unsigned __int32 BuildSaveList;
  unsigned __int8 __unnamed208[4];
  unsigned __int32 IsMobile;
  unsigned __int32 IsContainer;
  unsigned __int32 IsRealContainer;
  unsigned __int8 __unnamed224[4];
  unsigned __int32 IsMap;
  unsigned __int32 IsNPC;
  unsigned __int32 IsShopKeeper;
  unsigned __int32 IsGuard;
  unsigned __int8 __unnamed244[4];
  unsigned __int32 IsDead;
  unsigned __int32 IsWeapon;
  unsigned __int32 IsCorpse;
  unsigned __int32 IsSpellbook;
  unsigned __int32 IsInContainer;
  unsigned __int32 IsEquipped;
  unsigned __int8 __unnamed272[4];
  unsigned __int32 IsMoveable;
  unsigned __int32 IsFreelyUsable;
  unsigned __int32 IsFreelyViewable;
  unsigned __int8 __unnamed288[4];
  unsigned __int32 GetWeight;
  unsigned __int32 GetWeightInStones;
  unsigned __int8 __unnamed304[8];
  unsigned __int32 field_130;
  unsigned __int32 TellClientAboutObject;
  unsigned __int8 __unnamed340[28];
  unsigned __int32 RecalcWeight;
  unsigned __int8 __unnamed372[28];
  unsigned __int32 IsHidden;
  unsigned __int32 SetHidden;
  unsigned __int32 SetMurderCount;
  unsigned __int8 __unnamed388[4];
  unsigned __int32 RefreshAggression;
  unsigned __int8 __unnamed404[12];
  unsigned __int32 ReceiveAggressionFrom;
  unsigned __int32 ReceiveUnhealthyActionFrom;
  unsigned __int8 __unnamed416[4];
  unsigned __int32 ChangeReputation;
  unsigned __int32 GetCanCarry;
  unsigned __int8 __unnamed428[4];
  unsigned __int32 DestroyContents;
  unsigned __int8 __unnamed448[16];
  unsigned __int32 SetCurHP;
  unsigned __int32 SetMaxHP;
  unsigned __int32 SetCurFatigue;
  unsigned __int32 SetMaxFatigue;
  unsigned __int32 SetCurMana;
  unsigned __int32 SetMaxMana;
  unsigned __int32 field_1D8;
  unsigned __int32 ModifyMaxHP;
  unsigned __int32 field_1E0;
  unsigned __int32 ModifyMaxFatigue;
  unsigned __int32 field_1E8;
  unsigned __int32 ModifyMaxMana;
  unsigned __int32 SetRealStat;
  unsigned __int32 ModifyRealStat;
  unsigned __int32 ModifyStatMod;
  unsigned __int32 SetStatMod;
  unsigned __int32 SetNotoriety;
  unsigned __int32 GetTitledName;
  unsigned __int8 __unnamed532[12];
  unsigned __int32 GetModifiedDexterity;
  unsigned __int8 __unnamed540[4];
  unsigned __int32 IsFatigued;
  unsigned __int32 LoseFatigueByMoving;
  unsigned __int32 GainFatigue;
  unsigned __int32 IsHealthy;
  unsigned __int32 HandleHealthGain;
  unsigned __int32 MakeInvulnerable;
  unsigned __int32 MakeVulnerable;
  unsigned __int32 CheckStatLimit;
  unsigned __int32 GainStatAndCheckLimit;
  unsigned __int32 PostSkillGain;
  unsigned __int8 __unnamed584[4];
  unsigned __int32 SetFlags;
  unsigned __int32 ClearFlags__OR__ReceiveHelpfulActionFrom;
  unsigned __int32 field_250;
};

// Location size=0x06
#define def_class_Location    __int16 X; \
                              __int16 Y; \
                              __int16 Z;
typedef struct class_Location { def_class_Location } LocationObject;

// Location Functions
#define LocationGetX(location) (GetSWord(location, LOCATION_X))
#define LocationGetY(location) (GetSWord(location, LOCATION_Y))
#define LocationGetZ(location) (GetSWord(location, LOCATION_Z))
#define LocationSetX(location, value) (SetSWord(location, LOCATION_X, value))
#define LocationSetY(location, value) (SetSWord(location, LOCATION_Y, value))
#define LocationSetZ(location, value) (SetSWord(location, LOCATION_Z, value))

extern int __stdcall IsEqualXY (LocationObject *A, LocationObject *B);
extern int __stdcall IsEqualXYZ(LocationObject *A, LocationObject *B);

void initLocation(LocationObject* location);
void Location_ToString(char* buffer, LocationObject* location);

public struct class_Entity
{
  vtable_ItemClass *vtable;
  unsigned __int16 ObjectType;
  unsigned __int8 field_6;
  unsigned __int8 __unnamed8;
  unsigned __int16 Hue;
  class_Location Location;
};

public struct class_ResourceEntity : class_Entity
{
  //class_Entity Entity;
  class_Location CreationLocation;
  unsigned __int16 Template;
  unsigned __int32 Resources;
};

// ItemObject size=0x50
typedef public struct class_DynamicItem : class_ResourceEntity
{
  //class_ResourceEntity Resource;
  unsigned __int32 CallbackList;
  unsigned __int32 NextItemObjectOnChunk;   // ?
  unsigned __int32 field_24;
  unsigned __int32 ContainedByObject;
  unsigned __int32 field_2C;
  unsigned __int32 field_30;
  unsigned __int32 field_34;
  unsigned __int8 __unnamed64[8];
  unsigned __int32 MyOwnSerial;
  unsigned __int8 Status;
  unsigned __int8 DecayCount;
  unsigned __int8 __unnamed72[2];
  unsigned __int32 AttachmentsObject;
  unsigned __int32 MultiObject;
} ItemObject;

// ContainerObject size=0x5C
typedef public struct class_Container : class_DynamicItem
{
  //class_DynamicItem Item;
  unsigned __int32 field_50;
  unsigned __int32 field_54;
  unsigned __int16 WeightInStones;
  unsigned __int16 unknown;
} ContainerObject;

// DiceObject  size=0x03
typedef struct class_Dice
{
  unsigned __int8 A;
  unsigned __int8 B;
  unsigned __int8 C;
} DiceObject;

// MobileObject size=0x37C
typedef public struct class_Mobile : class_Container
{
  //class_Container Container;
  unsigned __int32 NextMobile;
  unsigned __int32 PreviousMobile;
  unsigned __int8 __unnamed104[4];
  unsigned __int16 flags;
  unsigned __int8 __unnamed116[10];
  unsigned __int16 skilllevel[50];
  unsigned __int16 skillmod[50];
  unsigned __int32 Tasks[50];
  unsigned __int8 TasksPercentages[50];
  unsigned __int8 __unnamed568[2];
  unsigned __int32 skillslush;
  unsigned __int16 str;
  unsigned __int16 dex;
  unsigned __int16 intel;   // int
  unsigned __int16 strmod;
  unsigned __int16 dexmod;
  unsigned __int16 intmod;
  unsigned __int8 __unnamed596[12];
  unsigned __int32 CurHP;
  unsigned __int32 MaxHP;
  unsigned __int32 CurFat;
  unsigned __int32 MaxFat;
  unsigned __int32 CurMana;
  unsigned __int32 MaxMana;
  unsigned __int32 lifeclock;
  unsigned __int32 clockA;
  unsigned __int32 clockB;
  unsigned __int32 clockC;
  unsigned __int32 clockD;
  unsigned __int32 ActionCounter;
  unsigned __int8 not;
  unsigned __int8 __unnamed646;
  unsigned __int16 Fame;
  unsigned __int16 Karma;
  unsigned __int8 Satiety;
  unsigned __int8 att;
  unsigned __int16 deftexthue;
  unsigned __int8 __unnamed656[2];
  unsigned __int32 EquipmentArray;
  unsigned __int32 LeftHand;
  unsigned __int32 RightHand;
  unsigned __int32 field_29C;
  unsigned __int32 field_2A0;
  unsigned __int32 field_2A4;
  unsigned __int32 field_2A8;
  unsigned __int32 field_2AC;
  unsigned __int32 field_2B0;
  unsigned __int32 field_2B4;
  unsigned __int32 field_2B8;
  unsigned __int32 field_2BC;
  unsigned __int32 field_2C0;
  unsigned __int32 field_2C4;
  unsigned __int32 field_2C8;
  unsigned __int32 field_2CC;
  unsigned __int32 field_2D0;
  unsigned __int32 field_2D4;
  unsigned __int32 field_2D8;
  unsigned __int32 field_2DC;
  unsigned __int32 field_2E0;
  unsigned __int32 field_2E4;
  unsigned __int32 field_2E8;
  unsigned __int32 field_2EC;
  unsigned __int32 field_2F0;
  unsigned __int32 field_2F4;
  unsigned __int32 field_2F8;
  unsigned __int32 field_2FC;
  unsigned __int8 __unnamed772[4];
  unsigned __int32 BankObject;
  unsigned __int32 dir;
  unsigned __int32 stclk;
  unsigned __int32 NotorietyCounter;
  unsigned __int32 SwingCounter;
  unsigned __int32 SwingState;
  unsigned __int8 stom;
  unsigned __int8 sex;
  unsigned __int8 lt__LightTime;
  unsigned __int8 lv__LightValue;
  unsigned __int8 __unnamed812[12];
  unsigned __int16 sfxnotice;
  unsigned __int16 sfxidle;
  unsigned __int16 sfxhit;
  unsigned __int16 sfxwashit;
  unsigned __int16 sfxdie;
  unsigned __int8 __unnamed828[6];
  unsigned __int8 CombatMode;
  unsigned __int8 fs_;
  unsigned __int8 fa_;
  unsigned __int8 fw_;
  unsigned __int8 __unnamed836[4];
  unsigned __int32 Attacking;
  unsigned __int32 field_348;
  unsigned __int8 field_34C;
  unsigned __int8 __unnamed848[3];
  unsigned __int8 AttackedByList;
  unsigned __int8 __unnamed860[11];
  unsigned __int32 MountSerial;
  unsigned __int8 __unnamed868[4];
  unsigned char* RealName;
  class_Dice DiceObject;
  unsigned __int8 __unnamed876;
  unsigned __int32 NaturalAC;
  unsigned __int8 MovementType;
  unsigned __int8 __unnamed888[7];
  unsigned __int8 MobFlag;
  unsigned __int8 State;
  unsigned __int8 __unnamed891;
  unsigned __int8 field_0;
} MobileObject;

// NPCObject size=0x474
typedef struct class_NPC : class_Mobile
{
  //class_Mobile Mobile;
  unsigned __int32 NextNPCobject;
  unsigned __int8 __unnamed900[4];
  unsigned __int32 flags;
  unsigned __int8 __unnamed910[6];
  class_Location desireloc;
  class_Location lastdesireloc;
  unsigned __int8 field_39A;
  unsigned __int8 __unnamed928[5];
  class_Location HomeLocationObject;
  unsigned __int8 __unnamed936[2];
  unsigned __int32 CurrentState;
  unsigned __int32 PostSleepState;
  unsigned __int8 __unnamed948[4];
  unsigned __int32 PreviousState;
  unsigned __int8 __unnamed956[4];
  unsigned __int32 Target;          // ?
  unsigned __int32 LeaderSerial;
  unsigned __int32 FollowSerial;    // ?
  unsigned __int8 __unnamed976[8];
  unsigned __int32 SleepCounter;
  unsigned __int32 FollowDuration;
  unsigned __int8 __unnamed996[12];
  unsigned __int32 field_3E4;
  unsigned __int8 __unnamed1004[4];
  unsigned __int32 StomMax;         // ???
  unsigned __int8 __unnamed1092[84];
  unsigned __int32 Job;
  unsigned __int32 Town;
  unsigned __int8 __unnamed1115[15];
  unsigned __int8 field_45B;
  unsigned __int8 __unnamed1117;
  unsigned __int8 npcflee;
  unsigned __int8 __unnamed1128[10];
  unsigned __int32 ActivePathData;    // ?
  unsigned __int8 __unnamed1136[4];
  unsigned __int32 ActivePathIndex;   // ?
} NPCObject;

// GuardObject size=0x480
typedef struct class_Guard : class_NPC
{
  //class_NPC NPCObject;
  unsigned __int32 PreviousGuardObject;
  unsigned __int32 NextGuardObject;
  unsigned __int16 field_47C;
  unsigned __int16 Unused;
} GuardObject;

// CorpseItem size=0xCC
typedef struct class_CorpseItem : class_Container
{
  //class_Container field_0;
  unsigned __int32 field_5C;
  unsigned __int32 field_60;
  unsigned __int32 field_64;
  unsigned __int8 field_68;
  unsigned __int32 field_69;
  unsigned __int32 field_6D;
  unsigned __int32 field_71;
  unsigned __int32 field_75;
  unsigned __int32 field_79;
  unsigned __int32 field_7D;
  unsigned __int32 field_81;
  unsigned __int32 field_85;
  unsigned __int32 field_89;
  unsigned __int32 field_8D;
  unsigned __int32 field_91;
  unsigned __int32 field_95;
  unsigned __int32 field_99;
  unsigned __int32 field_9D;
  unsigned __int32 field_A1;
  unsigned __int32 field_A5;
  unsigned __int32 field_A9;
  unsigned __int32 field_AD;
  unsigned __int32 field_B1;
  unsigned __int32 field_B5;
  unsigned __int8 field_B9;
  unsigned __int32 field_BA;
  unsigned __int32 field_BE;
  unsigned __int16 field_C2;
  unsigned __int16 CorpseBodyType;
  unsigned __int32 field_C6;
  unsigned __int16 field_CA;
} CorpseItem;

struct class_String
{
  unsigned __int32 Memory;
  unsigned __int32 StringLength;
  unsigned __int32 ReservedSize;
  unsigned __int32 MemorySize;
};

struct class_HelpRequest
{
  unsigned __int32 player_serial;
  class_String player_name;
  unsigned __int8 call_number;
  unsigned __int8 mode;
  unsigned __int8 __unnamed24[2];
  class_String server_name;
  class_String help_text;
};

struct class_ZandFlags
{
  unsigned __int32 Z;
  unsigned __int32 Flags;
};

struct class_Attachment
{
  unsigned __int32 Scripts__Script1C;
  unsigned __int32 field_4;
  unsigned __int8 field_8[4088];
};

// ShopKeeperObject size=0x480
typedef struct class_ShopKeeper : class_NPC
{
  //class_NPC NPCObject;
  unsigned __int32 RestockCounter;
  unsigned __int8 __unnamed1148[4];
  unsigned __int32 field_47C;
} ShopKeeperObject;

struct class_AttachmentHelper
{
  unsigned __int32 Count;
  class_Attachment AttachmentObject;
};

struct class_PlayerList
{
  unsigned __int32 LastPlayerObject;
  unsigned __int32 field_4;
  unsigned __int32 field_8;
  unsigned __int8 field_C[1424];
  unsigned __int32 field_59C;
};

struct class_ArrayOrList
{
  unsigned __int8 Invalid;
  unsigned __int8 __unnamed4[3];
  unsigned __int32 Memory;
  unsigned __int32 Zero;
};

struct class_WeaponTemplate
{
  unsigned __int8 field_0;
  class_Dice field_1;
  class_Dice field_4;
  class_Dice field_7;
  unsigned __int8 field_A;
  unsigned __int8 field_B;
  unsigned __int8 field_C;
  unsigned __int8 field_D;
  unsigned __int16 field_E;
  unsigned __int8 field_10;
  unsigned __int8 field_11;
  unsigned __int8 field_12;
  unsigned __int8 __unnamed20;
  unsigned __int16 field_14;
  unsigned __int16 field_16;
  unsigned __int32 field_18;
};

struct class_TemplatedArray
{
  unsigned __int8 Invalid;
  unsigned __int8 __unnamed4[3];
  unsigned __int32 field_4;
  unsigned __int32 Count;
  unsigned __int32 field_C;
};

struct class_SDB
{
  class_TemplatedArray StringArray;
};

// PlayerObject size=0x458
typedef public struct class_Player : class_Mobile
{
  //class_Mobile Mobile;
  unsigned __int32 NextPlayerObject;
  unsigned __int32 PreviousPlayerObject;
  char Password[30];
  unsigned __int8 __unnamed932[2];
  unsigned __int32 SocketObject;
  unsigned __int32 pflags;
  unsigned __int16 ltype;
  unsigned __int8 __unnamed964[22];
  unsigned __int32 TimestampOfFirstMovementInCamp;
  unsigned __int8 __unnamed984[16];
  unsigned __int32 IdleCounter;
  unsigned __int8 __unnamed1008[20];
  unsigned __int32 IsInGuardZone;
  unsigned __int8 GuardZoneCounter;
  unsigned __int8 __unnamed1014;
  class_Location lastvalidloc;
  unsigned __int32 account_number;
  unsigned __int32 character_number;
  unsigned __int8 PulseIndex;
  unsigned __int8 __unnamed1032[3];
  unsigned __int32 PulseTable[5];
  unsigned __int32 ClientCreationTickcount;
  unsigned __int32 sid;
  unsigned __int8 __unnamed1075[15];
  unsigned __int8 field_433;
  unsigned __int8 __unnamed1088[12];
  unsigned __int32 dc;
  unsigned __int8 __unnamed1096[4];
  unsigned __int32 playage;
  unsigned __int8 unknown;
  unsigned __int8 __unnamed1104[3];
  unsigned __int32 StateFixCounter;
  unsigned __int32 AttackeeSerial;
} PlayerObject;

struct class_ResourceType
{
  unsigned __int32 Next;
  unsigned __int32 field_4;
  unsigned __int8 InternalName[128];
  unsigned __int8 FoodName[128];
  unsigned __int8 ShelterName[128];
  unsigned __int8 DesireName[128];
  unsigned __int8 ProductionName[128];
  unsigned __int32 field_288;
  unsigned __int32 field_28C;
};

struct class_StaticItem : class_Entity
{
  // class_Entity Entity;
  unsigned __int32 NextStaticItem;
  unsigned __int32 field_14;
};

struct class_SaveList
{
  unsigned __int32 Memory;
  unsigned __int32 UsedSize;
  unsigned __int32 MaxSize;
};

struct class_Egg
{
  class_DynamicItem GameObject;
};

// size=0x64
typedef struct class_BulletinBoard : class_Container
{
  //class_Container ContainerObject;
  unsigned __int32 PreviousBulletinBoard;
  unsigned __int32 NextBulletinBoard;
} BulletinBoard;

// WeaponObject size=0x58
typedef struct class_Weapon : class_DynamicItem
{
  //class_DynamicItem Item;
  unsigned __int8 WeaponTemplate;
  class_Dice WeaponClass;
  unsigned __int8 MaxAC;
  unsigned __int8 CurHP;
  unsigned __int8 MaxHP;
  unsigned __int8 field_57;
} WeaponObject;

struct class_Regions
{
  unsigned __int32 field_0;
  unsigned __int8 field_4[1608];
  unsigned __int32 RegionsHelper;
};

struct class_WeaponTemplates
{
  unsigned __int32 field_0;
};

// MultiObject size=0x30
typedef struct struct_MultiObject
{
  unsigned __int32 field_0;
  unsigned __int32 SlaveId;
  unsigned __int32 ComponentOffset_LocationObject;
  unsigned __int32 field_C;
  unsigned __int32 field_10;
  unsigned __int32 field_14;
  unsigned __int32 field_18;
  unsigned __int32 field_1C;
  unsigned __int32 field_20;
  unsigned __int32 field_24;
  unsigned __int32 field_28;
  unsigned __int32 field_2C;
} MultiObject;

// PlayerHelpInfo size=0x3B
typedef struct class_PlayerHelpInfo
{
  unsigned __int32 mode;        // ?
  unsigned __int32 field_4;
  unsigned __int32 gm_serial;
  unsigned __int32 player_serial;
  class_Location LocationObject;
  unsigned __int8 __unnamed24[2];
  unsigned __int32 account_number;
  unsigned __int8 character_number;
  unsigned __int8 CharacterRealName[30];
} PlayerHelpInfo;

// PlayerHelpInfoArgs size=0x36
typedef public struct struct_PlayerHelpInfoArgs
{
public:
  unsigned __int8 mode;             // ?
  unsigned __int32 field_1;
  unsigned __int32 gm_serial;
  unsigned __int32 player_serial;
  class_Location locationObject;
  unsigned __int32 account_number;
  unsigned __int8 character_number;
  char CharacterRealName[30];
} PlayerHelpInfoArgs;

#define def_class_Location    __int16 X; \
                              __int16 Y; \
                              __int16 Z;

#define def_struct_ClientVersion  unsigned __int8 Major;  \
                                  unsigned __int8 Minor;  \
                                  unsigned __int8 Build;  \
                                  unsigned __int8 Revision;
struct struct_ClientVersion { def_struct_ClientVersion };

public struct struct_ServerSocket
{
  unsigned __int32 PointerToFunctions;
  struct_ServerSocket* NextServerSocket;
  struct_ServerSocket* PreviousServerSocket;
  unsigned __int32 SocketHandle;
  unsigned __int32 field_10;
  unsigned __int32 field_14;
  unsigned __int32 field_18;
  unsigned __int32 field_1C;
  unsigned __int32 field_20;
           char*   SocketIdentifierString;
  unsigned __int8  Data_or_IPport[65536];
  unsigned __int32 AccountNumber__PATCH;
  unsigned __int32 BytesRead;
  unsigned __int32 IPaddress;
  unsigned __int32 field_10034;
  class_Player*    PlayerObject;
  struct_ClientVersion ClientVersion;
};



