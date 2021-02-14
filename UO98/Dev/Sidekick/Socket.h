#pragma once

#include "UODemo.h"

#using <System.dll>

using namespace System;
using namespace System::Net;
using namespace System::Net::Sockets;

namespace UODemo
{

    public interface class IServerPacket
    {
    public:
        property array<unsigned __int8>^ Data { array<unsigned __int8>^ get(); }
        property __int32 Length { __int32 get(); }
    };

    public interface class ISocket
    {
    public:
        property unsigned __int32 AccountNumber { unsigned __int32 get(); }
        property unsigned __int8* Data { unsigned __int8* get(); }
        property unsigned __int32 DataLength { unsigned __int32 get(); }
        property IPAddress^ ClientAddress { IPAddress^ get(); }
        property bool IsEditing { bool get(); void set(bool value); }
        property bool IsGm { bool get(); }
        property bool IsGod { bool get(); void set(bool value); }
        property bool IsInternal { bool get(); }
        property _PlayerFlags PlayerFlags { _PlayerFlags get(); }
        property class_Player* PlayerObject { class_Player* get(); }
        property unsigned __int32 SocketHandle { unsigned __int32 get(); }
        property String^ SocketIdentifierString { String^ get(); }
        property ClientVersionStruct Version { ClientVersionStruct get(); }
        property bool VerifyGod { bool get(); }

        void RemoveBytes(unsigned __int16 start, unsigned __int16 amount);

        /// <summary>
        /// Removes the top packet in the Sockets incoming packet buffer
        /// </summary>
        /// <param name="Length">The size of the packet</param>
        void RemoveFirstPacket(__int32 Length);

        void Replace(array<Byte>^ newData, unsigned __int32 oldLen);
        __int32 SendPacket(IServerPacket^ packet);
        __int32 SendPacket(array<Byte>^ PacketData, unsigned __int32 Packetlength);
        void SetClientVersion(ClientVersionStruct vStruct);
    };

    using namespace System::Collections::Generic;

    public ref class Socket : public ISocket
    {
        static Queue<Socket^>^ pool= gcnew Queue<Socket^>();

        IUOServer^ _uoServer;
        struct_ServerSocket* _realSocket;

        String^ m_SocketIdentifierString;
        IPAddress^ m_ClientAddress;

        void Init(IUOServer^ uoServer, struct_ServerSocket* realSocket)
        {
            _uoServer=uoServer;
            _realSocket=realSocket;
            m_SocketIdentifierString=nullptr;
            m_ClientAddress=nullptr;
        }

        Socket(IUOServer^ uoServer, struct_ServerSocket* realSocket)
        {
            Init(uoServer, realSocket);
        }

    public:

        static Socket^ Acquire(IUOServer^ uoServer, struct_ServerSocket* realSocket)
        {
            if(pool->Count > 0)
            {
                Socket^ toReturn=pool->Dequeue();
                toReturn->Init(uoServer,realSocket);
                return toReturn;
            }
            else
                return gcnew Socket(uoServer,realSocket);
        }

        static void Free(ISocket^ isocket)
        {
            if(isocket->GetType() == Socket::typeid)
            {
                Socket^ socket=(Socket^)isocket;
                socket->_uoServer=nullptr;
                socket->_realSocket=NULL;
                socket->m_SocketIdentifierString=nullptr;
                socket->m_ClientAddress=nullptr;
                pool->Enqueue(socket);
            }
        }

        virtual property unsigned __int32 SocketHandle { unsigned __int32 get() { return _realSocket->SocketHandle; } }
        virtual property unsigned __int8* Data { unsigned __int8* get() { return _realSocket->Data_or_IPport; } }
        virtual property unsigned __int32 AccountNumber { unsigned __int32 get() { return _realSocket->AccountNumber__PATCH; } }
        
        virtual property unsigned __int32 DataLength 
        { 
            unsigned __int32 get() { return _realSocket->BytesRead; } 
        private:
            void set( unsigned __int32 value) { _realSocket->BytesRead = value; }
        }
        
        virtual property class_Player* PlayerObject { class_Player* get() { return _realSocket->PlayerObject; } }
        virtual property ClientVersionStruct Version { ClientVersionStruct get() { return _realSocket->ClientVersion; } }

        virtual property bool IsInternal { bool get() { return SocketHandle == 0xBEEF; } }

        virtual property String^ SocketIdentifierString
        { 
            String^ get()  
            {
                if(m_SocketIdentifierString==nullptr)
                {
                    if(_realSocket->SocketIdentifierString == NULL) m_SocketIdentifierString = String::Empty;
                    else m_SocketIdentifierString = gcnew String(_realSocket->SocketIdentifierString);
                }
                return m_SocketIdentifierString;
            }
        }

        virtual property IPAddress^ ClientAddress 
        {
            IPAddress^ get()
            {
                if(m_ClientAddress==nullptr && _realSocket->IPaddress!=NULL)
                    m_ClientAddress = gcnew IPAddress(_realSocket->IPaddress);

                return m_ClientAddress;
            }
        }

        virtual property _PlayerFlags PlayerFlags { _PlayerFlags get() { return Player::GetPlayerFlags(PlayerObject); } }
        
        virtual property bool IsEditing
        {
            bool get() { return Player::GetPlayerFlag(PlayerObject, _PlayerFlags::IsEditing); }
            void set(bool value) { Player::SetPlayerFlag(PlayerObject, _PlayerFlags::IsEditing, value); }
        }

        virtual property bool IsGm
        {
            bool get() { return Player::GetPlayerFlag(PlayerObject, _PlayerFlags::IsGM); }
        }

        virtual property bool IsGod
        {
            bool get() { return Player::GetPlayerFlag(PlayerObject, _PlayerFlags::IsGod); }
            void set(bool value) { Player::SetPlayerFlag(PlayerObject, _PlayerFlags::IsGod, value); }
        }

        virtual property bool VerifyGod 
        { 
            bool get() 
            {
                GetAccountAccessArgs args;
                args.AccountNumber=AccountNumber;
                args.Handled=false;
                args.AccessFlags=_AccountAccessFlags::Player;

                _uoServer->InvokeOnGetAccess(args);

                return args.Handled && args.AccessFlags.HasFlag(_AccountAccessFlags::Editor);
            }
        }


        virtual void RemoveBytes(unsigned __int16 start, unsigned __int16 amount)
        {
            for (unsigned __int16 i = start; i < DataLength; i++)
                *(Data + i) = *(Data + amount + i);
            DataLength -= amount;
        }
        
        virtual void RemoveFirstPacket(__int32 Length)
        {
            _uoServer->PacketEngine->SocketObject_RemoveFirstPacket(_realSocket, (unsigned __int32)Length);
        }

        virtual void Replace(array<Byte>^ newData, unsigned __int32 oldLen)
        {
            // Make room after current packet
            for (unsigned __int32 i = oldLen; i < DataLength; i++)
                *(Data + i + newData->Length) = *(Data + i);
            // Write packet after the existing one
            for (__int32 i = 0; i < newData->Length; i++)
                *(Data + oldLen + i) = newData[i];
            // Remove first packet
            for (unsigned __int32 i = 0; i < DataLength + newData->Length - oldLen; i++)
                *(Data + i) = *(Data + oldLen + i);
            DataLength = DataLength - oldLen + (unsigned __int32)newData->Length;
        }
        
        virtual __int32 SendPacket(IServerPacket^ packet)
        {
            return SendPacket(packet->Data, (unsigned __int32)packet->Length);
        }
        
        virtual __int32 SendPacket(array<Byte>^ PacketData, unsigned __int32 Packetlength)
        {
            cli::pin_ptr<unsigned __int8> pData = &PacketData[0];
                return _uoServer->PacketEngine->SocketObject_SendPacket(_realSocket, pData, Packetlength);
        }
        
        virtual void SetClientVersion(ClientVersionStruct vStruct)
        {
            _realSocket->ClientVersion.Major = vStruct.Major;
            _realSocket->ClientVersion.Minor = vStruct.Minor;
            _realSocket->ClientVersion.Build = vStruct.Build;
            _realSocket->ClientVersion.Revision = vStruct.Revision;
        }
    };

}