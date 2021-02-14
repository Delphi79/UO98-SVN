using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpkick;

namespace Sharpkick_Tests
{
    partial class MockCore
    {
        private ITimeManager _TimeManager = new MockTimeManager();

        public ITimeManager TimeManager { get { return _TimeManager; } }

        class MockTimeManager : ITimeManager
        {
            #region ITimeManager Members

            public int PulseNum{get;private set;}

            #endregion

            OnPulseEventHandler thisHandler;
            public MockTimeManager()
            {
                thisHandler = new OnPulseEventHandler(TimeManager_Tick);
                Sharpkick.EventSink.OnPulse += thisHandler;
            }

            void TimeManager_Tick()
            {
                if (Server.TimeManager == this)
                    PulseNum++;
                else
                    Sharpkick.EventSink.OnPulse -= thisHandler;
            }

        }

    }
}
