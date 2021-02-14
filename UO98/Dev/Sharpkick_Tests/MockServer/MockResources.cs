using System;
using Sharpkick;

namespace Sharpkick_Tests
{
    unsafe partial class MockCore : ICore
    {

        IResources _Resources = null;
        public IResources Resources
        { get { return _Resources ?? (_Resources = new MockResources()); } }


        class MockResources : IResources
        {
            #region IResources Members

            public bool ResourceGrowthRunning { get; set; }

            public bool ResourceGrowthFastMode { get; set; }

            public int ChunksGenerated
            {
                get { throw new NotImplementedException(); }
            }

            public int TotalChunks
            {
                get { throw new NotImplementedException(); }
            }

            public void Configure()
            {
                
            }

            #endregion
        }
    }
}