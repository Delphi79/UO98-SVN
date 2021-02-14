// Guids.cs
// MUST match guids.h
using System;

namespace JoinUO.UOSL.Package
{
    static class GuidList
    {
        public const string guidUOSLLanguagePackagePkgString = "88d0297f-5caa-47f4-8ebf-be128f762011";
        public const string guidUOSLLanguagePackageCmdSetString = "b111d6ba-e3fa-43ac-b4d1-ceb06754390f";

        public static readonly Guid guidUOSLLanguagePackageCmdSet = new Guid(guidUOSLLanguagePackageCmdSetString);
    };
}