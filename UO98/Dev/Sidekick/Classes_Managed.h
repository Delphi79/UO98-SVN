#pragma once

#include "Classes.h"
#include "StringPointerUtils.h"

#pragma managed

using namespace System;

public enum struct ALRReason : unsigned __int8
{
    /// <summary>This account doesn't exist. If you just created or updated the account on the web site, it may take a few minutes to be transmitted to Origin</summary>
    Invalid = 0x00,
    /// <summary>Someone is already using this account.</summary>
    InUse = 0x01,
    /// <summary>Your account has been blocked. Please visit http://ultima-registration.com for information on reactivating your account.</summary>
    Blocked = 0x02,
    /// <summary>Your account credentials are invalid. Check your user ID and password and try again.</summary>
    BadPass = 0x03,
    /// <summary>Sends user back to the Main Menu</summary>
    Idle = 0xFE,
    /// <summary>There is some problem communicating with Origin. Please restart Ultima Online and try again.</summary>
    BadComm = 0xFF
};

public enum struct Direction : unsigned __int8
{
    North = 0,
    Right = 1,
    East = 2,
    Down = 3,
    South = 4,
    Left = 5,
    West = 6,
    Up = 7,
    /// <summary>Running FLAG</summary>
    Running = 0x80
};

[Flags]
public enum struct TileFlag : unsigned __int32
{
    Background      = 0x1,
    Weapon          = 0x2,
    Transparent     = 0x4,
    Translucent     = 0x8,
    Wall            = 0x10,
    Damaging        = 0x20,
    Impassable      = 0x40,
    Wet             = 0x80,
    Unknown1        = 0x100,
    Surface         = 0x200,
    Bridge          = 0x400,
    Generic         = 0x800,
    Window          = 0x1000,
    NoShoot         = 0x2000,
    ArticleA        = 0x4000,
    ArticleAn       = 0x8000,
    Internal		= 0x00010000,
    Foliage			= 0x00020000,
    PartialHue		= 0x00040000,
    Unknown2		= 0x00080000,
    Map				= 0x00100000,
    Container		= 0x00200000,
    Wearable		= 0x00400000,
    LightSource		= 0x00800000,
    Animation		= 0x01000000,
    NoDiagonal		= 0x02000000,
    Unknown3		= 0x04000000,
    Armor			= 0x08000000,
    Roof			= 0x10000000,
    Door			= 0x20000000,
    StairBack		= 0x40000000,
    StairRight		= 0x80000000
};


[Flags] 
public enum struct MobileFlags : unsigned __int8
{
    None = 0x00,
    Frozen = 0x01,
    Female = 0x02,
    Poisoned = 0x04,        // Flying in post 7.x client
    Invulnerable = 0x08,    // Yellow health bar
    IgnoreMobiles = 0x10,
    Movable = 0x20,
    WarMode = 0x40,
    Hidden = 0x80
};
typedef MobileFlags _MobileFlags;

[Flags] 
public enum struct PlayerFlags : unsigned __int32
{
    LastMoveRej     = 0x00000001,  // ??
    IsGod           = 0x00000002,
    IsOnline        = 0x00000004,
    IsEditing       = 0x00000008,
    HackMove        = 0x00000010,
    IsManifesting   = 0x00000020,
    unk00000040     = 0x00000040,
    unk00000080     = 0x00000080,
    unk00000100     = 0x00000100,
    un0000k0200     = 0x00000200,
    InSeance        = 0x00000400,
    unk00000800     = 0x00000800,
    IsGM            = 0x00001000,
    unk00002000     = 0x00002000,     // something with login
    unk00004000     = 0x00004000,     // something with counselor, Can't request help via 0x9b either...
    IsCounselor     = 0x00008000,
    BankDefs        = 0x00010000,
    IsGoldAccount   = 0x00020000
};
typedef PlayerFlags _PlayerFlags;

[Flags]
public enum struct AccountAccessFlags
{
    Player=0,
    Editor=0x01,
    SaveWorld=0x02,
    Shutdown=0x04,
    Admin=Editor | SaveWorld | Shutdown,
};
typedef AccountAccessFlags _AccountAccessFlags;


public enum struct BookWriteableFlag : unsigned __int8
{
    ReadOnly=0,
    Writeable=1
};

typedef public value struct Serial sealed { 
private:
  unsigned int _serial;

  Serial(unsigned int serial)
  {
    _serial=serial;
  }

  Serial(int serial)
  {
    _serial=(unsigned int)serial;
  }

public:

  static operator unsigned int(Serial serial)
  {
    return serial._serial;
  }

  static operator int(Serial serial)
  {
    return serial._serial;
  }

  static operator Serial(unsigned int serial)
  {
    return Serial(serial);
  }

  static operator Serial(int serial)
  {
    return Serial(serial);
  }
} Serial;

typedef public value struct LocationDelta sealed { 
    def_class_Location 

    LocationDelta(short x, short y, short z)
    {
        X = x;
        Y = y;
        Z = z;
    }
} LocationDelta;

typedef public value struct Location sealed { 
    def_class_Location 

    Location(short x, short y, short z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    static property Location Zero
    {
        Location get()
        {
            Location toReturn=Location();
            toReturn.X=toReturn.Y=toReturn.Z=0;
            return toReturn;
        }
    }

    static bool operator== (Location^ A, Object^ B)
    {
        return A->Equals(B);
    }

    bool operator!= (const Location& toCompare)
    {
        return !(this->Equals(toCompare));
    }

    virtual int GetHashCode() override
    {
        return (int)((X & 0xFFF00000) << 20) + ((Y & 0xFFF00) << 8) + (Z & 0xFF);
    }

    virtual bool Equals(Object^ obj) override
    {
        if(obj->GetType() == Location::typeid)
        {
            Location^ toCompare = (Location^)obj;
            return
                X == toCompare->X &&
                Y == toCompare->Y &&
                Z == toCompare->Z;
        }
        return false;
    }

    static LocationDelta operator- (Location^ A, Location^ B)
    {
        return LocationDelta((short)(A->X - B->X), (short)(A->Y - B->Y), (short)(A->Z - B->Z));
    }

    static Location operator+ (Location^ A, LocationDelta^ B)
    {
        return Location((short)(A->X + B->X), (short)(A->Y + B->Y), (short)(A->Z + B->Z));
    }

    virtual String^ ToString() override
    {
        return X + " " + Y + " " + Z;
    }

    static operator Location(LocationObject loc)
    {
      return Location(loc.X, loc.Y, loc.Z);
    }

    static operator Location(LocationObject* loc)
    {
      return Location(loc->X, loc->Y, loc->Z);
    }

} _Location;

public value struct ClientVersionStruct sealed 
{ 
    def_struct_ClientVersion 

    ClientVersionStruct(unsigned __int8 major,
                        unsigned __int8 minor,
                        unsigned __int8 build,
                        unsigned __int8 revision) :
    Major(major), Minor(minor), Build(build), Revision(revision){}

    static operator ClientVersionStruct(struct_ClientVersion cv)
    {
      return ClientVersionStruct(cv.Major,cv.Minor,cv.Build,cv.Revision);
    }

};

public ref class Entity abstract  
{
protected:
    bool WeOwnThis;
    class_Entity* _entity;

    Entity(class_Entity* base, bool shouldDelete) 
    {
        _entity = base; 
        WeOwnThis=shouldDelete;
    }

public:

    Entity() 
    { 
        _entity = new class_Entity();
        WeOwnThis=true;
    }

    Entity(class_Entity* base) 
    { 
        _entity = base; 
        WeOwnThis=false;
    }

    !Entity() 
    {
        if(WeOwnThis)
        {
            delete _entity;
            WeOwnThis=false;
        }
    }

    property unsigned __int16   ObjectType    { unsigned __int16 get()  { return _entity->ObjectType; } }
    property unsigned __int16   Hue           { unsigned __int16 get()  { return _entity->Hue; } }
    property _Location          Location      { _Location        get()  { return _entity->Location; } }
};

public ref class ResourceEntity abstract : Entity 
{ 
protected:
    ResourceEntity(class_Entity* base, bool shouldDelete) : Entity(base,shouldDelete){}
public:
    ResourceEntity() : Entity(new class_ResourceEntity(), true){}
    ResourceEntity(class_ResourceEntity* base) : Entity(base){}

    property _Location          CreationLocation    { _Location        get()  { return ((class_ResourceEntity*)_entity)->Location; } }
    property unsigned __int16   Template            { unsigned __int16 get()  { return ((class_ResourceEntity*)_entity)->Template; } }
    property unsigned __int32   Resources           { unsigned __int32 get()  { return ((class_ResourceEntity*)_entity)->Resources; } }
};

public ref class Item : ResourceEntity 
{ 
protected:
    Item(class_DynamicItem* base, bool shouldDelete) : ResourceEntity(base,shouldDelete){}
public:
    Item() : ResourceEntity(new class_DynamicItem(), true){}
    Item(class_DynamicItem* base) : ResourceEntity(base){}

    property unsigned __int32   Serial              { unsigned __int32 get()  { return ((class_DynamicItem*)_entity)->MyOwnSerial; } }

    static operator Item^(class_DynamicItem* pItem)
    {
      if(pItem==NULL)return nullptr;
      return gcnew Item(pItem);
    }

    static operator class_DynamicItem*(Item item)
    {
      return (class_DynamicItem*)(item._entity);
    }
};

public ref class Container : Item 
{ 
protected:
    Container(class_Container* base, bool shouldDelete) : Item(base,shouldDelete){}
public:
    Container() : Item(new class_Container(), true){}
    Container(class_Container* base) : Item(base){}

    property unsigned __int16   TotalWeight        { unsigned __int16 get()  { return ((class_Container*)_entity)->WeightInStones; } }

    static operator Container^(class_Container* pItem)
    {
      if(pItem==NULL)return nullptr;
      return gcnew Container(pItem);
    }

    static operator class_DynamicItem*(Container item)
    {
      return (class_Container*)(item._entity);
    }
};

public ref class Mobile : Container 
{ 
protected:
    Mobile(class_Mobile* base, bool shouldDelete) : Container(base,shouldDelete){}
public:
    Mobile() : Container(new class_Mobile(), true){}
    Mobile(class_Mobile* base) : Container(base){}

    property unsigned __int16   Body        { unsigned __int16 get()  { return ((class_Mobile*)_entity)->ObjectType; } }
    property unsigned __int16   Str         { unsigned __int16 get()  { return ((class_Mobile*)_entity)->str; } }
    property unsigned __int16   Dex         { unsigned __int16 get()  { return ((class_Mobile*)_entity)->dex; } }
    property unsigned __int16   Int         { unsigned __int16 get()  { return ((class_Mobile*)_entity)->intel; } }
    property unsigned __int16   StrMod      { unsigned __int16 get()  { return ((class_Mobile*)_entity)->strmod; } }
    property unsigned __int16   DexMod      { unsigned __int16 get()  { return ((class_Mobile*)_entity)->dexmod; } }
    property unsigned __int16   IntMod      { unsigned __int16 get()  { return ((class_Mobile*)_entity)->intmod; } }
    property unsigned __int32   CurHP       { unsigned __int32 get()  { return ((class_Mobile*)_entity)->CurHP; } }
    property unsigned __int32   MaxHP       { unsigned __int32 get()  { return ((class_Mobile*)_entity)->MaxHP; } }
    property unsigned __int32   CurFat      { unsigned __int32 get()  { return ((class_Mobile*)_entity)->CurFat; } }
    property unsigned __int32   MaxFat      { unsigned __int32 get()  { return ((class_Mobile*)_entity)->MaxFat; } }
    property unsigned __int32   CurMana     { unsigned __int32 get()  { return ((class_Mobile*)_entity)->CurMana; } }
    property unsigned __int32   MaxMana     { unsigned __int32 get()  { return ((class_Mobile*)_entity)->MaxMana; } }
    property unsigned __int16   Fame        { unsigned __int16 get()  { return ((class_Mobile*)_entity)->Fame; } }
    property unsigned __int16   Karma       { unsigned __int16 get()  { return ((class_Mobile*)_entity)->Karma; } }
    property unsigned __int16   Satiety     { unsigned __int16 get()  { return ((class_Mobile*)_entity)->Satiety; } }
    
    property _MobileFlags       MobileFlags { _MobileFlags     get()  { return (_MobileFlags)((class_Mobile*)_entity)->MobFlag; } }

    property String^ RealName
    { 
        String^ get()  
        {
            return StringPointerUtils::GetAsciiString(((class_Mobile*)_entity)->RealName, 30);
        }
    }

    static operator Mobile^(class_Mobile* pItem)
    {
      if(pItem==NULL)return nullptr;
      return gcnew Mobile(pItem);
    }

    static operator class_Mobile*(Mobile item)
    {
      return (class_Mobile*)(item._entity);
    }

};

public ref class Player sealed : Mobile 
{ 
//protected:
//    Player(class_Player* base, bool shouldDelete) : Mobile(base,shouldDelete){}
public:
    Player() : Mobile(new class_Player(), true){}
    Player(class_Player* base) : Mobile(base){}

    property unsigned __int32   AccountNumber   { unsigned __int32 get()  { return ((class_Player*)_entity)->account_number; } }
    property unsigned __int32   CharacterNumber { unsigned __int32 get()  { return ((class_Player*)_entity)->character_number; } }
    property _PlayerFlags       PlayerFlags     { _PlayerFlags     get()  { return (_PlayerFlags)((class_Player*)_entity)->pflags; } }

    static _PlayerFlags GetPlayerFlags(PlayerObject* player)
    {
        if (player == nullptr) return (_PlayerFlags)0;
        return (_PlayerFlags)((*player).pflags);
    }

    static bool GetPlayerFlag(PlayerObject* player, _PlayerFlags toGet)
    {
        if (player == nullptr) return false;
        return ((*player).pflags & (unsigned __int32)toGet) == (unsigned __int32)toGet;
    }

    static void SetPlayerFlag(PlayerObject* player, _PlayerFlags toSet)
    {
      SetPlayerFlag(player, toSet, true);
    }

    static void SetPlayerFlag(PlayerObject* player, _PlayerFlags toSet, bool Value)
    {
      if (player != nullptr)
      {
          if (Value) (*player).pflags |= (unsigned __int32)toSet;
          else (*player).pflags &= ~(unsigned __int32)toSet;
      }
    }

    static void ClearPlayerFlag(PlayerObject* player, _PlayerFlags toClear)
    {
        SetPlayerFlag(player, toClear, false);
    }

    bool GetPlayerFlag(_PlayerFlags toGet)
    {
      return GetPlayerFlag((PlayerObject*)_entity, toGet);
    }

    static operator Player^(class_Player* pItem)
    {
      if(pItem==NULL)return nullptr;
      return gcnew Player(pItem);
    }

    static operator class_Player*(Player item)
    {
      return (class_Player*)(item._entity);
    }


};

