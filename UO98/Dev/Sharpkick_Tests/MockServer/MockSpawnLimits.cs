using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharpkick;

namespace Sharpkick_Tests
{
    unsafe partial class MockCore : Sharpkick.ICore
    {

        ISpawnLimits _SpawnLimits = null;
        public ISpawnLimits SpawnLimits
        { get { return _SpawnLimits ?? (_SpawnLimits = new MockSpawnLimits()); } }

        class MockSpawnLimits : ISpawnLimits
        {
            #region ISpawnLimits Members

            public ulong MaxNormalMobiles { get; set; }

            #endregion
        }
    }
}
