using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Sharpkick
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct SkillsObjectStruct
    {
        public fixed int PerSkillSomeValue[50];
        public fixed int PerSkillUsageCounter[50];
        public int AllSkillSomeTotal;
        public int AllSkillUsageCounter;
        public fixed byte SkillArray[50 * 0xE8]; // SkillObjectStruct[50]
        public int SkillCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct SkillObjectStruct
    {
        public fixed byte bytesName[80];
        public fixed byte bytesScript[80];
        public int StrReq;
        public int DexReq;
        public int IntReq;
        public int Strength;
        public int Dexterity;
        public int Intelligence;
        public fixed byte AdvRate[24];  // Double3Struct
        public int StatAdvRate;
        public int SkillStat;
        public int CanUse;
        public int SkillWeight;
        public int IsEnabled_Always1;
        public int Version;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Double3Struct
    {
        uint A1;
        uint A2;
        uint B1;
        uint B2;
        uint C1;
        uint C2;
    }

    interface ISkillsObject
    {
        int SkillCount { get; }
        SkillObjectStruct Skill(int skillnum);
    }

    static partial class Server
    {
        public static ISkillsObject SkillsObject
        {
            get { return Core.SkillsObject; }
        }

        unsafe private partial class LiveCore : ICore
        {
            private ISkillsObject _SkillsObject = new GlobalSkillsObject();
            public ISkillsObject SkillsObject { get { return _SkillsObject; } }

            unsafe class GlobalSkillsObject : ISkillsObject
            {
                private SkillsObjectStruct* pGLOBAL_SkillsObject = (SkillsObjectStruct*)0x00648418;
                public SkillsObjectStruct GLOBAL_SkillsObject { get { return *pGLOBAL_SkillsObject; } }

                public int SkillCount { get { return GLOBAL_SkillsObject.SkillCount; } }

                public SkillObjectStruct Skill(int skillnum) //(SkillObjectStruct*)Skills.GLOBAL_SkillsObject.SkillArray
                {
                    SkillObjectStruct* pSkills = ((SkillObjectStruct*)((*pGLOBAL_SkillsObject).SkillArray));
                    pSkills += skillnum;
                    return *pSkills;
                }
            }

        }
    }

    static class SkillsExtensions
    {
        unsafe public static string GetName(this SkillObjectStruct skill)
        {
            fixed (char* pChars = new char[50])
            {
                ASCIIEncoding.ASCII.GetChars(skill.bytesName, 50, pChars, 50);
            return new string(pChars);
            }
        }
        unsafe public static string GetScript(this SkillObjectStruct skill)
        {
            fixed (char* pChars = new char[50])
            {
                ASCIIEncoding.ASCII.GetChars(skill.bytesScript, 50, pChars, 50);
                return new string(pChars);
            }
        }
    }
}
