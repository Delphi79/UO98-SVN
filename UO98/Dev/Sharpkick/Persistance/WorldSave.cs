using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick
{
    class WorldSave
    {
        public static void Configure()
        {
            Server.Core.OnPulse += new OnPulseEventHandler(EventSink_OnPulse);
        }

        static void EventSink_OnPulse()
        {
            if(MyServerConfig.AutoSaveEnabled && Server.TimeManager.PulseNum % ((int)MyServerConfig.SaveFreq.TotalSeconds * 4) == 0) 
                Server.SaveWorld();
        }
    }
}
