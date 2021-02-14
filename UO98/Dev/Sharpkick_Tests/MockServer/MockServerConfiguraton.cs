using System;
using Sharpkick;

namespace Sharpkick_Tests
{
    unsafe partial class MockCore : Sharpkick.ICore
    {

        IServerConfiguration _ServerConfiguration = null;
        public IServerConfiguration ServerConfiguration
        { get { return _ServerConfiguration ?? (_ServerConfiguration = new MockServerConfiguraton()); } }

        class MockServerConfiguraton : IServerConfiguration
        {
            #region IServerConfiguration Members

            public int MapWidth
            {
                get { return 1000; }
            }

            public int MapHeight
            {
                get { return 1000; }
            }

            public int MapStartX
            {
                get { return 0; }
            }

            public int MapStartY
            {
                get { return 0; }
            }

            private string _ServerName = "Unknown?";
            public string ServerName
            {
                get { return _ServerName; }
                set { _ServerName = value; }
            }

            public string ServerOwner
            {
                get { throw new NotImplementedException(); }
            }

            public string SavePath { get { return "Save"; } }
            public string DataPath { get { return "Data"; } }

            #endregion
        }

    }
}