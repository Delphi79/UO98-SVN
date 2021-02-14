using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UoClientSDK.Network.ServerPackets
{
    [ServerPacket(ServerPacketId.Damage, FixedLength)]
    public class DamagePacket : ServerPacket
    {
        const int FixedLength = 7;

        public Serial Serial { get; private set; }
        public ushort Amount { get; set; }

        public DamagePacket(Serial serial, ushort amount)
        {
            Serial = serial;
            Amount = amount;
        }

        internal static DamagePacket Instantiate(PacketReader reader)
        {
            if(!ReadHead<DamagePacket>(reader)) return null;
            return new DamagePacket(reader.ReadSerial(), reader.ReadUShort());
        }
    }

    [ServerPacket(ServerPacketId.GodEditTileData)]
    public class GodEditTileDataPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.StatusBarInfo)]
    public class StatusBarInfoPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.Follow, FixedLength)]
    public class FollowPacket : ServerPacket
    {
        const int FixedLength = 9;

    }

    [ServerPacket(ServerPacketId.HealthBarStatusUpdateKR, FixedLength)]
    public class HealthBarStatusUpdateKRPacket : ServerPacket
    {
        const int FixedLength = 12;
    }

    [ServerPacket(ServerPacketId.ObjectInfo)]
    public class ObjectInfoPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.CharLocaleAndBody, FixedLength)]
    public class CharLocaleAndBodyPacket : ServerPacket
    {
        const int FixedLength = 37;
    }

    [ServerPacket(ServerPacketId.SendSpeech)]
    public class SendSpeechPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.DeleteObject, FixedLength)]
    public class DeleteObjectPacket : ServerPacket
    {
        const int FixedLength = 5;
    }

    [ServerPacket(ServerPacketId.Explosion, FixedLength)]
    public class ExplosionPacket : ServerPacket
    {
        const int FixedLength = 8;
    }

    [ServerPacket(ServerPacketId.DrawGamePlayer, FixedLength)]
    public class DrawGamePlayerPacket : ServerPacket
    {
        const int FixedLength = 19;
    }

    [ServerPacket(ServerPacketId.CharMoveReject, FixedLength)]
    public class CharMoveRejectPacket : ServerPacket
    {
        const int FixedLength = 21;
    }

    [ServerPacket(ServerPacketId.CharMoveAck, FixedLength)]
    public class CharMoveAckPacket : ServerPacket
    {
        const int FixedLength = 3;
    }

    [ServerPacket(ServerPacketId.DragingOfItem, FixedLength)]
    public class DragingOfItemPacket : ServerPacket
    {
        const int FixedLength = 26;
    }

    [ServerPacket(ServerPacketId.DrawContainer, FixedLength)]
    public class DrawContainerPacket : ServerPacket
    {
        const int FixedLength = 7;
    }

    [ServerPacket(ServerPacketId.AddItemToContainer, FixedLength, startversion: null, endversion: "6.0.1.7")]
    public class AddItemToContainerPacket : ServerPacket
    {
        const int FixedLength = 20;
    }

    [ServerPacket(ServerPacketId.AddItemToContainer, fixedLength: FixedLength, startversion: "6.0.1.7")]
    public class AddItemToContainerNewPacket : AddItemToContainerPacket
    {
        const int FixedLength = 21;
    }

    [ServerPacket(ServerPacketId.KickPlayer, FixedLength)]
    public class KickPlayerPacket : ServerPacket
    {
        const int FixedLength = 5;
    }

    [ServerPacket(ServerPacketId.RejectMoveItemRequest, FixedLength)]
    public class RejectMoveItemRequestPacket : ServerPacket
    {
        const int FixedLength = 2;
    }

    [ServerPacket(ServerPacketId.DropItemFailed, FixedLength)]
    public class DropItemFailedPacket : ServerPacket
    {
        const int FixedLength = 5;
    }

    [ServerPacket(ServerPacketId.GodClearSquare, FixedLength, startversion: null, endversion: "0.0.0.0")]   // End Version TBD: http://www.joinuo.com/forums/viewtopic.php?f=28&t=1034
    public class GodClearSquarePacket : ServerPacket
    {
        const int FixedLength = 5;
    }

    [ServerPacket(ServerPacketId.DropItemOk, FixedLength)]
    public class DropItemOkPacket : ServerPacket
    {
        const int FixedLength = 1;
    }

    [ServerPacket(ServerPacketId.Blood, FixedLength)]
    public class BloodPacket : ServerPacket
    {
        const int FixedLength = 5;
    }

    [ServerPacket(ServerPacketId.GodMode, FixedLength)]
    public class GodModePacket : ServerPacket
    {
        const int FixedLength = 2;
    }

    [ServerPacket(ServerPacketId.ResurrectMenu, FixedLength)]
    public class ResurrectMenuPacket : ServerPacket
    {
        const int FixedLength = 2;
    }

    [ServerPacket(ServerPacketId.MobAttributes, FixedLength)]
    public class MobAttributesPacket : ServerPacket
    {
        const int FixedLength = 17;
    }

    [ServerPacket(ServerPacketId.WornItem, FixedLength)]
    public class WornItemPacket : ServerPacket
    {
        const int FixedLength = 15;
    }

    [ServerPacket(ServerPacketId.FightOccuring, FixedLength)]
    public class FightOccuringPacket : ServerPacket
    {
        const int FixedLength = 10;
    }

    [ServerPacket(ServerPacketId.AttackOk, FixedLength)]
    public class AttackOkPacket : ServerPacket
    {
        const int FixedLength = 5;
    }

    [ServerPacket(ServerPacketId.AttackEnded, FixedLength)]
    public class AttackEndedPacket : ServerPacket
    {
        const int FixedLength = 1;
    }

    [ServerPacket(ServerPacketId.Unknown0x32, FixedLength)]
    public class Unknown0x32Packet : ServerPacket
    {
        const int FixedLength = 2;
    }

    [ServerPacket(ServerPacketId.PauseClient, FixedLength)]
    public class PauseClientPacket : ServerPacket
    {
        const int FixedLength = 2;
    }

    [ServerPacket(ServerPacketId.GodResourceTileData)]
    public class GodResourceTileDataPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.AddMultipleItemsToContainer)]
    public class AddMultipleItemsToContainerPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.GodVersions, FixedLength)]
    public class GodVersionsPacket : ServerPacket
    {
        const int FixedLength = 37;
    }

    [ServerPacket(ServerPacketId.RemoveGroup, FixedLength)]
    public class RemoveGroupPacket : ServerPacket
    {
        const int FixedLength = 9;
    }

    [ServerPacket(ServerPacketId.Skills)]
    public class SkillsPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.GodUpdateStatics)]
    public class GodUpdateStaticsPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.PersonalLightLevel, FixedLength)]
    public class PersonalLightLevelPacket : ServerPacket
    {
        const int FixedLength = 6;
    }

    [ServerPacket(ServerPacketId.GlobalLightLevel, FixedLength)]
    public class GlobalLightLevelPacket : ServerPacket
    {
        const int FixedLength = 2;
    }

    [ServerPacket(ServerPacketId.RejectCharacterLogin, FixedLength)]
    public class RejectCharacterLoginPacket : ServerPacket
    {
        const int FixedLength = 2;
    }

    [ServerPacket(ServerPacketId.PlaySound, FixedLength)]
    public class PlaySoundPacket : ServerPacket
    {
        const int FixedLength = 12;
    }

    [ServerPacket(ServerPacketId.LoginComplete, FixedLength)]
    public class LoginCompletePacket : ServerPacket
    {
        const int FixedLength = 1;
    }

    [ServerPacket(ServerPacketId.MapEdit, FixedLength)]
    public class MapEditPacket : ServerPacket
    {
        const int FixedLength = 11;
    }

    [ServerPacket(ServerPacketId.Time, FixedLength)]
    public class TimePacket : ServerPacket
    {
        const int FixedLength = 4;
    }

    [ServerPacket(ServerPacketId.Weather, FixedLength)]
    public class WeatherPacket : ServerPacket
    {
        const int FixedLength = 4;
    }

    [ServerPacket(ServerPacketId.BookPageInfo)]
    public class BookPageInfoPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.TargetCommand, FixedLength)]
    public class TargetCommandPacket : ServerPacket
    {
        const int FixedLength = 19;
    }

    [ServerPacket(ServerPacketId.PlayMusic, FixedLength)]
    public class PlayMusicPacket : ServerPacket
    {
        const int FixedLength = 3;
    }

    [ServerPacket(ServerPacketId.CharacterAnimation, FixedLength)]
    public class CharacterAnimationPacket : ServerPacket
    {
        const int FixedLength = 14;
    }

    [ServerPacket(ServerPacketId.SecureTrade)]
    public class SecureTradePacket : ServerPacket { }

    [ServerPacket(ServerPacketId.GraphicalEffect, FixedLength)]
    public class GraphicalEffectPacket : ServerPacket
    {
        const int FixedLength = 28;
    }

    [ServerPacket(ServerPacketId.BulletinBoardMessages)]
    public class BulletinBoardMessagesPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.RequestWarMode, FixedLength)]
    public class RequestWarModePacket : ServerPacket
    {
        const int FixedLength = 5;
    }

    [ServerPacket(ServerPacketId.Ping, FixedLength)]
    public class PingPacket : ServerPacket
    {
        const int FixedLength = 2;
    }

    [ServerPacket(ServerPacketId.OpenBuyWindow)]
    public class OpenBuyWindowPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.NewSubserver, FixedLength)]
    public class NewSubserverPacket : ServerPacket
    {
        const int FixedLength = 16;
    }

    [ServerPacket(ServerPacketId.UpdatePlayer, FixedLength)]
    public class UpdatePlayerPacket : ServerPacket
    {
        const int FixedLength = 17;
    }

    [ServerPacket(ServerPacketId.DrawObject)]
    public class DrawObjectPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.OpenDialogBox)]
    public class OpenDialogBoxPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.LoginDenied, FixedLength)]
    public class LoginDeniedPacket : ServerPacket
    {
        const int FixedLength = 2;

        public LoginRejectReason Reason { get; private set; }

        public LoginDeniedPacket(LoginRejectReason reason)
        {
            Reason = reason;
        }

        internal static LoginDeniedPacket Instantiate(PacketReader reader)
        {
            if(!ReadHead<LoginDeniedPacket>(reader)) return null;
            return new LoginDeniedPacket((LoginRejectReason)reader.ReadByte());
        }
    }

    [ServerPacket(ServerPacketId.ResendCharactersAfterDelete)]
    public class ResendCharactersAfterDeletePacket : ServerPacket { }

    [ServerPacket(ServerPacketId.OpenPaperdoll, FixedLength)]
    public class OpenPaperdollPacket : ServerPacket
    {
        const int FixedLength = 66;
    }

    [ServerPacket(ServerPacketId.CorpseClothing)]
    public class CorpseClothingPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.ConnectToGameServer, FixedLength)]
    public class ConnectToGameServerPacket : ServerPacket
    {
        const int FixedLength = 11;
    }

    [ServerPacket(ServerPacketId.MapMessage, FixedLength)]
    public class MapMessagePacket : ServerPacket
    {
        const int FixedLength = 19;
    }

    [ServerPacket(ServerPacketId.BookHeaderOld, FixedLength)]
    public class BookHeaderOldPacket : ServerPacket
    {
        const int FixedLength = 99;
    }

    [ServerPacket(ServerPacketId.DyeWindow, FixedLength)]
    public class DyeWindowPacket : ServerPacket
    {
        const int FixedLength = 9;
    }

    [ServerPacket(ServerPacketId.AllNames)]
    public class AllNamesPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.MultiPlacementView, FixedLength)]
    public class MultiPlacementViewPacket : ServerPacket
    {
        const int FixedLength = 26;
    }

    [ServerPacket(ServerPacketId.ConsoleEntryPrompt)]
    public class ConsoleEntryPromptPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.RequestAssistance, FixedLength)]
    public class RequestAssistancePacket : ServerPacket
    {
        const int FixedLength = 53;
    }

    [ServerPacket(ServerPacketId.SellList)]
    public class SellListPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.UpdateHealth, FixedLength)]
    public class UpdateHealthPacket : ServerPacket
    {
        const int FixedLength = 9;
    }

    [ServerPacket(ServerPacketId.UpdateMana, FixedLength)]
    public class UpdateManaPacket : ServerPacket
    {
        const int FixedLength = 9;
    }

    [ServerPacket(ServerPacketId.UpdateStamina, FixedLength)]
    public class UpdateStaminaPacket : ServerPacket
    {
        const int FixedLength = 9;
    }

    [ServerPacket(ServerPacketId.OpenWebBrowser)]
    public class OpenWebBrowserPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.TipNoticeWindow)]
    public class TipNoticeWindowPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.GameServerList)]
    public class GameServerListPacket : ServerPacket
    {
        internal static GameServerListPacket Instantiate(PacketReader reader)
        {
            if(!ReadHead<GameServerListPacket>(reader)) return null;
            // TODO: This GameServerListPacket implementation is a stub.
            return new GameServerListPacket();
        }
    }

    [ServerPacket(ServerPacketId.CharsAndStartLocations)]
    public class CharsAndStartLocationsPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.AllowOrRefuseAttack, FixedLength)]
    public class AllowOrRefulseAttackPacket : ServerPacket
    {
        const int FixedLength = 5;
    }

    [ServerPacket(ServerPacketId.OpenTextEntryDialog)]
    public class OpenTextEntryDialogPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.UnicodeSpeech)]
    public class UnicodeSpeechPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.DisplayDeathAction, FixedLength)]
    public class DisplayDeathActionPacket : ServerPacket
    {
        const int FixedLength = 13;
    }

    [ServerPacket(ServerPacketId.OpenMenuDialog)]
    public class OpenMenuDialogPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.ChatMessage)]
    public class ChatMessagePacket : ServerPacket { }

    [ServerPacket(ServerPacketId.ChatText)]
    public class ChatTextPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.ToolTip)]
    public class ToolTipPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.CharProfile)]
    public class CharProfilePacket : ServerPacket { }

    [ServerPacket(ServerPacketId.ClientFeatures, fixedLength: FixedLength, startversion: null, endversion: "6.0.14.2")]
    public class ClientFeaturesOldPacket : ServerPacket
    {
        const int FixedLength = 3;
    }

    [ServerPacket(ServerPacketId.ClientFeatures, fixedLength: FixedLength, startversion: "6.0.14.2")]
    public class ClientFeaturesNewPacket : ServerPacket
    {
        const int FixedLength = 5;
    }

    [ServerPacket(ServerPacketId.QuestArrow, FixedLength)]
    public class QuestArrowPacket : ServerPacket
    {
        const int FixedLength = 6;
    }

    [ServerPacket(ServerPacketId.UltimaMessenger, FixedLength)]
    public class UltimaMessengerPacket : ServerPacket
    {
        const int FixedLength = 9;
    }

    [ServerPacket(ServerPacketId.SeasonInfo, FixedLength)]
    public class SeasonInfoPacket : ServerPacket
    {
        const int FixedLength = 3;
    }

    [ServerPacket(ServerPacketId.ClientVersion)]
    public class ClientVersionPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.AssistVersion)]
    public class AssistVersionPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.GeneralInfoPacketBF)]
    public class GeneralInfoPacketBFPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.GraphicalEffect2, FixedLength)]
    public class GraphicalEffect2Packet : ServerPacket
    {
        const int FixedLength = 36;
    }

    [ServerPacket(ServerPacketId.LocalizedMessage)]
    public class LocalizedMessagePacket : ServerPacket { }

    [ServerPacket(ServerPacketId.UnicodeTextEntry)]
    public class UnicodeTextEntryPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.SmurfIt, FixedLength)]
    public class SmurfItPacket : ServerPacket
    {
        const int FixedLength = 6;
    }

    [ServerPacket(ServerPacketId.InvalidMap, FixedLength)]
    public class InvalidMapPacket : ServerPacket
    {
        const int FixedLength = 1;
    }

    [ServerPacket(ServerPacketId.ParticleEffect3D, FixedLength)]
    public class ParticleEffect3DPacket : ServerPacket
    {
        const int FixedLength = 49;
    }

    [ServerPacket(ServerPacketId.ClientViewRange, FixedLength)]
    public class ClientViewRangePacket : ServerPacket
    {
        const int FixedLength = 2;
    }

    [ServerPacket(ServerPacketId.GetAreaServerPing, FixedLength)]
    public class GetAreaServerPingPacket : ServerPacket
    {
        const int FixedLength = 6;
    }

    [ServerPacket(ServerPacketId.GetUserServerPing, FixedLength)]
    public class GetUserServerPingPacket : ServerPacket
    {
        const int FixedLength = 6;
    }

    [ServerPacket(ServerPacketId.GodGlobalQueueCount, FixedLength)]
    public class GodGlobalQueueCountPacket : ServerPacket
    {
        const int FixedLength = 7;
    }

    [ServerPacket(ServerPacketId.MessageLocalizedAffix)]
    public class MessageLocalizedAffixPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.ConfigurationFile)]
    public class ConfigurationFilePacket : ServerPacket { }

    [ServerPacket(ServerPacketId.LogoutStatus, FixedLength)]
    public class LogoutStatusPacket : ServerPacket
    {
        const int FixedLength = 2;
    }

    [ServerPacket(ServerPacketId.Extended0x20, FixedLength)]
    public class Extended0x20Packet : ServerPacket
    {
        const int FixedLength = 25;
    }

    [ServerPacket(ServerPacketId.Extended0x78)]
    public class Extended0x78Packet : ServerPacket { }

    [ServerPacket(ServerPacketId.BookHeaderNew)]
    public class BookHeaderNewPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.MegaCliloc)]
    public class MegaClilocPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.GenericAOSCommands)]
    public class GenericAOSCommandsPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.CustomHouse)]
    public class CustomHousePacket : ServerPacket { }

    [ServerPacket(ServerPacketId.CharTransferLog)]
    public class CharTransferLogPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.SEIntroducedRevision, FixedLength)]
    public class SEIntroducedRevisionPacket : ServerPacket
    {
        const int FixedLength = 9;
    }

    [ServerPacket(ServerPacketId.CompressedGump)]
    public class CompressedGumpPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.UpdateMobileStatus)]
    public class UpdateMobileStatusPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.BuffInfo)]
    public class BuffInfoPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.KRMobileStatusAnimationUpdate, FixedLength)]
    public class KRMobileStatusAnimationUpdatePacket : ServerPacket
    {
        const int FixedLength = 10;
    }

    [ServerPacket(ServerPacketId.KREncryptionResponse)]
    public class KREncryptionResponsePacket : ServerPacket { }

    [ServerPacket(ServerPacketId.KrriosClientSpecial)]
    public class KrriosClientSpecialPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.FreeshardInfo)]
    public class FreeshardInfoPacket : ServerPacket { }

    [ServerPacket(ServerPacketId.SAObjectInfo, FixedLength)]
    public class SAObjectInfoPacket : ServerPacket
    {
        const int FixedLength = 24;
    }

}