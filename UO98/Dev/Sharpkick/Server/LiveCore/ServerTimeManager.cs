using System;
using System.Runtime.InteropServices;

namespace Sharpkick
{
    interface ITimeManager
    {
        int PulseNum { get; }
    }

    static partial class Server
    {
        public static ITimeManager TimeManager
        {
            get { return Core.TimeManager; }
        }

        unsafe private partial class LiveCore : ICore
        {
            private ITimeManager _TimeManager = new LiveTimeManager();
            public ITimeManager TimeManager { get { return _TimeManager; } }

            private class LiveTimeManager : ITimeManager
            {
                [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 0x6C)]
                struct TimeManagerObject
                {
                    [FieldOffset(0x0C)] public int _TotalPulses;
                    [FieldOffset(0x10)] public int _Month;
                    [FieldOffset(0x14)] public int _Week;
                    [FieldOffset(0x18)] public int _DayOfWeek;
                    [FieldOffset(0x1C)] public int _Hour;
                    [FieldOffset(0x20)] public int _Minute;
                    [FieldOffset(0x24)] public int _SecondsDiv3;
                    [FieldOffset(0x28)] public int _Year;
                }

                private static TimeManagerObject* GLOBAL_TIMEMANAGER = (TimeManagerObject*)0x006482A8;

                public int PulseNum { get { return (*GLOBAL_TIMEMANAGER)._TotalPulses; } }

            }
        }
    }
}
