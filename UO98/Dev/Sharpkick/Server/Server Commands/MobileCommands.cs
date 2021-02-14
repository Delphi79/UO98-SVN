using System;
using System.Text;

namespace Sharpkick
{
    static partial class Server
    {
        unsafe public static void MakeGameMaster(PlayerObject* Target) { Core.MakeGameMaster(Target); }
        unsafe public static void UnmakeGameMaster(PlayerObject* Target) { Core.UnmakeGameMaster(Target); }
        unsafe public static bool IsGameMaster(PlayerObject* Target) { return Core.IsGameMaster(Target) != 0; }
        unsafe public static void OpenBank(int serialofplayer, class_Player* ShowTo)
        {
            class_Player* player = Server.Core.ConvertSerialToPlayer(serialofplayer);
            if (player != null)
                Core.OpenBank(player, ShowTo);
        }

        /// <summary>
        /// Sends a system message to player
        /// </summary>
        unsafe public static void SendSystemMessage(class_Player* player, string message)
        {
            if (player != null)
                fixed (byte* chars = ASCIIEncoding.ASCII.GetBytes(message))
                    Core.SendSystemMessage(player, chars);
        }

        /// <summary>
        /// Sends a system message to player
        /// </summary>
        unsafe public static void SendSystemMessage(int serialofplayer, string message)
        {
            class_Player* player = Server.Core.ConvertSerialToPlayer(serialofplayer);
            if (player != null)
                SendSystemMessage(player, message);
        }


    }
}
