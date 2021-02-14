#ifndef UODEMODLL_CLASSES__H

#define UODEMODLL_CLASSES__H 1.0

#include "core.h"

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
#define IsItemObject(baseaddress)           (baseaddress && GetVTable(baseaddress) == VTABLE_ITEM)
#define IsItemOnMapObject(baseaddress)      (baseaddress && GetVTable(baseaddress) == VTABLE_ITEMONMAPOBJECT)
#define IsWeaponObject(baseaddress)         (baseaddress && GetVTable(baseaddress) == VTABLE_WEAPON)
#define IsContainerObject(baseaddress)      (baseaddress && GetVTable(baseaddress) == VTABLE_CONTAINEROBJECT)
#define IsCorspeObject(baseaddress)         (baseaddress && GetVTable(baseaddress) == VTABLE_CORPSEOBJECT)
#define IsBulletinBoardObject(baseaddress)  (baseaddress && GetVTable(baseaddress) == VTABLE_BULLETINBOARD)

#define IsGuardObject(baseaddress)          (baseaddress && GetVTable(baseaddress) == VTABLE_GUARD)
#define IsShopKeeperObject(baseaddress)     (baseaddress && GetVTable(baseaddress) == VTABLE_SHOPKEEPER)
#define IsNPCobject(baseaddress)            (baseaddress && (GetVTable(baseaddress) == VTABLE_NPC || IsGuardObject(baseaddress) || IsShopKeeperObject(baseaddress)))
#define IsPlayerObject(baseaddress)         (baseaddress && GetVTable(baseaddress) == VTABLE_PLAYER)
#define IsMobileObject(baseaddress)         (baseaddress && (GetVTable(baseaddress) == VTABLE_MOBILE || IsPlayerObject(baseaddress) || IsNPCobject(baseaddress)))

#define IsAnyItem(baseaddress)    (IsItemObject(baseaddress) || IsItemOnMapObject(baseaddress) || IsWeaponObject(baseaddress) || IsContainerObject(baseaddress) || IsCorspeObject(baseaddress) || IsBulletinBoardObject(baseaddress))
#define IsAnyNPC(baseaddress)     (IsGuardObject(baseaddress) || IsShopKeeperObject(baseaddress) || IsNPCobject(baseaddress) )
#define IsAnyMobile(baseaddress)  (IsAnyNPC(baseaddress) || IsPlayerObject(baseaddress))

// Place Holder
typedef struct WeaponObject     { char __foo[0x37C]; } WeaponObject;
typedef struct MobileObject     { char __foo[0x37C]; } MobileObject;
typedef struct PlayerObject     { char __foo[0x458]; } PlayerObject;
typedef struct NPCobject        { char __foo[0x474]; } NPCobject;
typedef struct GuardObject      { char __foo[0x480]; } GuardObject;
typedef struct ShopKeeperObject { char __foo[0x480]; } ShopKeeperObject;

// ItemObject
typedef struct ItemObject // size=0x50
{ 
  void* VTable;
  unsigned short ObjectType;

  char __foo[0x50-6]; 

} ItemObject;

// Location
typedef struct LocationObject   
{   
  short X;
  short Y;
  short Z;
} LocationObject;

// List
typedef enum
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

typedef struct ListElementObject       
{ 
  struct ListElementObject* NextElement; 
  struct ListElementObject* PreviousElement; 
  _VARTYPE vartype;
  unsigned int refOrValue;
} ListElementObject;

typedef struct ListObject       
{ 
  struct ListElementObject* FirstElement; 
  struct ListElementObject* LastElement; 
  int Count;
} ListObject;

//
extern void* __cdecl ConvertSerialToObject(unsigned int serial);

#endif