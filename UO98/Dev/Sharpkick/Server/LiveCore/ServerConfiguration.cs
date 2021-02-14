using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Sharpkick
{
    interface IServerConfiguration
    {
        int MapWidth { get; }
        int MapHeight { get; }
        int MapStartX { get; }
        int MapStartY { get; }

        string ServerName { get; }
        string ServerOwner { get; }

        string SavePath { get; }
        string DataPath { get; }
    }

    static partial class Server
    {
        public static IServerConfiguration ServerConfiguration
        {
            get { return Core.ServerConfiguration; }
        }

        unsafe private partial class LiveCore : ICore
        {
            private IServerConfiguration _ServerConfiguration = new LiveServerConfiguration();
            public IServerConfiguration ServerConfiguration { get { return _ServerConfiguration; } }

            unsafe private class LiveServerConfiguration : IServerConfiguration
            {
                /// <summary>
                /// This may be only partial, the core structure is much larger I think
                /// </summary>
                [StructLayout(LayoutKind.Sequential, Pack = 1)]
                unsafe struct ServerObject
                {
                    public int field_0;
                    public int staticsWritable;
                    public int terrainWritable;
                    public int artWritable;
                    public int tiledataWritable;
                    public int huesWritable;
                    public int resourcesWritable;
                    public int animdataWritable;
                    public int templatesWritable;
                    public int multisWritable;
                    public int ServerID;
                    public int SSO_Port;
                    public int MapWidth;
                    public int MapHeight;
                    public int MapStartX;
                    public int MapStartY;
                    public fixed byte ServerName[128];
                    public fixed byte ServerOwner[128];
                }

                private static ServerObject* GLOBAL_SERVEROBJECT = (ServerObject*)0x006982F0;

                public int MapWidth { get { return (*GLOBAL_SERVEROBJECT).MapWidth; } }
                public int MapHeight { get { return (*GLOBAL_SERVEROBJECT).MapHeight; } }
                public int MapStartX { get { return (*GLOBAL_SERVEROBJECT).MapStartX; } }
                public int MapStartY { get { return (*GLOBAL_SERVEROBJECT).MapStartY; } }

                private string m_Name = null;
                public string ServerName
                {
                    get
                    {
                        if (m_Name != null) return m_Name;
                        fixed (char* name = new char[128])
                        {
                            ASCIIEncoding.ASCII.GetChars((*GLOBAL_SERVEROBJECT).ServerName, 128, name, 128);
                            return m_Name = new string(name);
                        }
                    }
                }

                private string m_ServerOwner = null;
                public string ServerOwner
                {
                    get
                    {
                        if (m_ServerOwner != null) return m_Name;
                        fixed (char* name = new char[128])
                        {
                            ASCIIEncoding.ASCII.GetChars((*GLOBAL_SERVEROBJECT).ServerOwner, 128, name, 128);
                            return m_ServerOwner = new string(name);
                        }
                    }
                }

                public string SavePath { get { return string.Format("..\\rundir\\{0}", ServerName); } }
                public string DataPath { get { return "..\\Data"; } }

            }
        }
    }

}
