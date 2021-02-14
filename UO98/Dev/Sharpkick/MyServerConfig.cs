using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick
{
    static class MyServerConfig
    {
        #region Spawner

        public const ushort MaxNonNormalMobilesSpawned=15000;

        public static bool PacketDebug { get { return false; } }
        public static bool ResourceDebug { get { return false; } }
        public static bool ResourceGrowthFastMode { get { return false; } }

        #endregion

        #region Saves

        /// <summary>The frequency at which the world should save</summary>
        public static TimeSpan SaveFreq = TimeSpan.FromMinutes(60.0);
        public static bool AutoSaveEnabled { get { return true; } }

        #endregion

        #region Decoration

        public static bool DecorationEnabled { get { return true; } }
        public static bool DecorationAddSkaraFerry { get { return false; } }

        #endregion

    }
}
