using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Sharpkick
{
    interface ISpawnLimits
    {
        ulong MaxNormalMobiles { get; set; }
    }

    static partial class Server
    {
        public static ISpawnLimits SpawnLimits
        {
            get { return Core.SpawnLimits; }
        }

        unsafe private partial class LiveCore : ICore
        {
            ISpawnLimits _SpawnLimits = new LiveCore.LiveSpawnLimits();
            public ISpawnLimits SpawnLimits { get { return _SpawnLimits; } }

            public class LiveSpawnLimits : ISpawnLimits
            {
                public LiveSpawnLimits()
                {
                    MaxNormalMobiles = MyServerConfig.MaxNonNormalMobilesSpawned;
                }

                [StructLayout(LayoutKind.Sequential, Pack = 1)]
                struct SpawnerParameterObjectStructure
                {
                    public ulong MaxNormalMobiles;  // Pretty sure. This is the primary spawn limiter apparently. May exclude shopkeeps, definitely excludes invulnerables.
                    ulong field_4;
                    ulong field_8;
                    ulong field_C;
                    ulong field_10;
                    ulong field_14;
                    ulong field_18;
                    ulong field_1C;
                    ulong MaxPlayerCount; // unsure at this time
                    ulong field_24;
                    ulong field_28;
                    ulong field_2C;
                    ulong field_30;
                    ulong MinPlayerCount;  // unsure at this time
                }

                unsafe private static SpawnerParameterObjectStructure* GLOBAL_SPAWNERPARAMETEROBJECT = (SpawnerParameterObjectStructure*)0x00699968;

                public unsafe ulong MaxNormalMobiles
                {
                    get { return (ushort)(*GLOBAL_SPAWNERPARAMETEROBJECT).MaxNormalMobiles; }
                    set { (*GLOBAL_SPAWNERPARAMETEROBJECT).MaxNormalMobiles = Math.Min(ushort.MaxValue, value); }
                }

            }
        }
    }
}