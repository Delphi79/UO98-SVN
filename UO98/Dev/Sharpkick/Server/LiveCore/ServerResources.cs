using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick
{
    interface IResources
    {
        bool ResourceGrowthRunning { get; set; }
        bool ResourceGrowthFastMode { get; set; }
        int ChunksGenerated { get; }
        int TotalChunks { get; }
    }

    static partial class Server
    {
        public static IResources Resources { get { return Core.Resources; } }

        unsafe private partial class LiveCore : ICore
        {
            IResources _Resources = new LiveCore.LiveResources();
            public IResources Resources { get { return _Resources; } }

            public class LiveResources : IResources
            {
                unsafe static byte* MaybeMapObject = (byte*)0x6DBD60;
                unsafe static int* MaybeMapObjectChunksGenerated =  (int*)(MaybeMapObject + 0x438);

                unsafe static byte* GLOBAL_ResGrowthFastTime = (byte*)0x6E7654;
                unsafe static byte* GLOBAL_ResGrowthRunning = (byte*)0x621398;
                unsafe static int* GLOBAL_ResTotalChunks = (int*)0x6933E0;

                public unsafe bool ResourceGrowthRunning
                {
                    get { return *GLOBAL_ResGrowthRunning != 0; }
                    set { *GLOBAL_ResGrowthRunning = (byte)(value ? -1 : 0); }
                }

                public unsafe bool ResourceGrowthFastMode
                {
                    get { return *GLOBAL_ResGrowthFastTime != 0; }
                    set { *GLOBAL_ResGrowthFastTime = (byte)(value ? -1 : 0); }
                }

                unsafe public int ChunksGenerated { get { return *MaybeMapObjectChunksGenerated; } }
                unsafe public int TotalChunks { get { return *GLOBAL_ResTotalChunks; } }

                public LiveResources()
                {
                    ResourceGrowthFastMode = MyServerConfig.ResourceGrowthFastMode;

                    if (MyServerConfig.ResourceDebug)
                        Server.Core.OnPulse += new OnPulseEventHandler(EventSink_OnPulse);
                }

                void EventSink_OnPulse()
                {
                    if (Server.TimeManager.PulseNum % 40 == 0)
                        Console.WriteLine("RESOURCES: Running:{0} Fast:{1} Generated:{2}/{3}", ResourceGrowthRunning, ResourceGrowthFastMode, ChunksGenerated, TotalChunks);
                }

            }
        }
    }
}