#include "UODemo.h"
#include "Commands.h"
#include <stdio.h>
#include "Socket.h"

namespace UODemo
{
    Core::Core()
    {
        PulseHandler=gcnew OnPulseEventHandler(this, &Core::InvokeOnPulse);
        GlobalOnPulse += PulseHandler;
        OnAfterSaveHandler=gcnew OnAfterSaveEventHandler(this, &Core::InvokeOnAfterSave);
        GlobalOnAfterSave += OnAfterSaveHandler;
    }

    Core::~Core()
    {
        GlobalOnPulse -= PulseHandler;
    }

    IPacketEngine^ Core::PacketEngine::get()
    {
        return m_PacketEngine==nullptr ? (m_PacketEngine=gcnew _PacketEngine()) : m_PacketEngine;
    }

    void Core::InvokeGlobalOnPulse()
    {
        if(!Initialized)
        {
            UODemo::Core::InitializeSharpkick();
            Initialized=true;
        }

        GlobalOnPulse();
    }

    void Core::InvokeGlobalOnAfterSave()
    {
        GlobalOnAfterSave();
    }

    void Core::InvokeOnPulse()
    {
        OnPulse();
    }

    void Core::InvokeOnAfterSave()
    {
        OnAfterSave();
    }

     void Core::InvokeOnGetAccess(GetAccountAccessArgs% args)
    {
        OnGetAccess(args);
    }

   void Core::SaveWorld() 
    {
        NativeMethods::SaveWorld();      
    }
    
    void Core::Shutdown()  
    {
        NativeMethods::ShutdownServer(); 
    }

    void Core::MakeCounselor(void *PlayerObjectTarget, int CounType)
    {
        NativeMethods::MakeCounselor((PlayerObject*)PlayerObjectTarget, CounType);
    }

    void Core::UnmakeCounselor(void *PlayerObjectTarget)
    {
        NativeMethods::UnmakeCounselor((PlayerObject*) PlayerObjectTarget);
    }

    void Core::OpenInfoWindow(Serial gmserial, Serial playerserial)
    {
        NativeMethods::SendInfoWindowToGodClient(gmserial, playerserial);
    }

    ItemObject* Core::ConvertSerialToItem(Serial serial)
    {
        return (ItemObject*)NativeMethods::ConvertSerialToObject(serial);
    }

    MobileObject* Core::ConvertSerialToMobile(Serial serial)
    {
        MobileObject* mobile = (MobileObject*)NativeMethods::ConvertSerialToObject(serial);
        if(mobile==NULL || !IsMobile(mobile))
            return NULL;
        return mobile;
    }

    PlayerObject* Core::ConvertSerialToPlayer(Serial serial)
    {
        PlayerObject* player = (PlayerObject*)NativeMethods::ConvertSerialToObject(serial);
        if(player==NULL || !IsPlayer(player))
            return NULL;
        return player;
    }

    bool Core::IsItem(void* object)
    {
        return IsItemObject(object);
    }

    bool Core::IsNPC(void* object)
    {
        return IsAnyNPC(object);
    }

    bool Core::IsMobile(void* object)
    {
        return IsAnyMobile(object);
    }

    bool Core::IsPlayer(void* object)
    {
        return IsPlayerObject(object);
    }        

    Location Core::getLocation(Serial itemSerial)
    {
        class_Location Loc;
        NativeMethods::getLocation(&Loc, itemSerial);
        return Loc;
    }

    int Core::setHue(Serial itemSerial, short hue)
    {
        return NativeMethods::setHue(itemSerial, hue);
    }

    #define FUNC_ItemObject_GetQuantity 0x004854F2
    int Core::getQuantity(Serial serial)
    {
        return NativeMethods::getValueByFunctionFromObject(serial, (void*)FUNC_ItemObject_GetQuantity, "getQuantity");
    }

    #define FUNC_ItemObject_GetWeight 0x00489FAB
    int Core::getWeight(Serial serial)
    {
        return NativeMethods::getValueByFunctionFromObject(serial, (void*)FUNC_ItemObject_GetWeight, "getWeight");
    }

    int Core::setOverloadedWeight(Serial serial, int weight)
    {
        return NativeMethods::setOverloadedWeight(serial, weight);
    }

    bool Core::deleteObject(Serial serial)
    {
        return NativeMethods::deleteObject(serial) != 0;
    }

    void Core::InitializeSharpkick()
    {
        if(aSharpkick==nullptr)
            aSharpkick=Assembly::LoadFrom("Sharpkick.dll");

        if(aSharpkick!=nullptr)
        {
            Type^ tMain=aSharpkick->GetType("Sharpkick.Main");
            MethodInfo^ mInit = tMain->GetMethod("Initialize");
            mInit->Invoke(nullptr, nullptr);

            puts("UODemo.Core Initialized");
        }
        else
            puts("UODemo.Core Initialize Fail: Could not load Assembly Sharpkick.");
    }

}
