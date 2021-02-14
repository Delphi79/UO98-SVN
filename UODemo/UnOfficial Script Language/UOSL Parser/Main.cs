using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoinUO.UOSL.Service;
using System.Reflection;
using System.IO;
using Irony.Parsing;

namespace JoinUO.UOSL
{
    public class UOSLArgumentException : ArgumentException 
    {
        public UOSLArgumentException(string message) : base(message) { }
        public UOSLArgumentException(string message, Exception InnerException) : base(message, InnerException) { }
    }

    [Flags]
    public enum ParseResult
    {
        OK = 0,
        HadErrors = 0x01,
        HadWarnings = 0x02,
        HadInfo = 0x04,
        ProgramExecutionFailure = 0x80,
    }

    static partial class UOSLMain
    {
        public static string InPath { get; private set; }

        public static LanguageOption? InSpec { get; private set; }

        public static LanguageOption m_OutSpec = LanguageOption.Native;
        public static LanguageOption OutSpec { get { return m_OutSpec; } }

        public static string OutDir { get; private set; }
        public static string OutFile { get; private set; }
        public static bool Overwrite { get; private set; }
        public static bool CheckOnly { get; private set; }

        /// <summary>
        /// Adds dots to console as progressing through file list.
        /// </summary>
        public static bool Dots { get; private set; }

        public static ParserErrorLevel m_DetailLevel = ParserErrorLevel.Info;
        public static ParserErrorLevel DetailLevel { get { return m_DetailLevel; } }

        public static bool Debug { get; private set; }

        public static string FullName { get; private set; }
        public static string ShortName { get; private set; }

        static int Main(string[] args)
        {
            FullName = Assembly.GetExecutingAssembly().ToString();
            ShortName = FullName.Split(',')[0];
            FullName = string.Join(" ", FullName.Split(','), 0, 2);

            if (args.FirstOrDefault(arg => arg.Length == 2 && (arg[0] == '-' || arg[0] == '/') && (arg[1] == '?' || arg[1] == 'h')) != null)
            {   // Help
                PrintUsageAbout();
                Console.WriteLine();
                Console.WriteLine("Usage:");
                PrintUsageAll();
                return 0;
            }

            ParseResult result = ParseResult.OK;
            LanguageOption m_InSpec;

            if (args.Length > 0)
            { // parse normalize options
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i].EqualsCI("-debug"))
                        Debug = true;
                    else if (args[i].EqualsCI("-check"))
                        CheckOnly = true;
                    else if (args[i].EqualsCI("-overwrite"))
                        Overwrite = true;
                    else if (args[i].EqualsCI("-dots"))
                        Dots = true;
                    else if (args[i].EqualsCI("-detail"))
                    {
                        if (i + 1 >= args.Length || !Enum.TryParse<ParserErrorLevel>(args[i + 1], true, out m_DetailLevel))
                        {
                            Console.WriteLine("Invalid argument: -detail <level> is not valid.");
                            Console.WriteLine();
                            Console.WriteLine("Usage:");
                            if (CheckOnly)
                                PrintUsageCheck();
                            else
                                PrintUsageNormalize();
                            Console.WriteLine();
                            PrintUsageLanguage();
                            result |= ParseResult.ProgramExecutionFailure;
                            break;
                        }
                        else
                            i++;
                    }

                    else if (args[i].EqualsCI("-inspec"))
                    {
                        if (i + 1 >= args.Length || !Enum.TryParse<LanguageOption>(args[i + 1], true, out m_InSpec))
                        {
                            Console.WriteLine("Invalid argument: -inspec <format> is not valid.");
                            Console.WriteLine();
                            Console.WriteLine("Usage:");
                            PrintUsageNormalize();
                            Console.WriteLine();
                            PrintUsageLanguage();
                            result |= ParseResult.ProgramExecutionFailure;
                            break;
                        }
                        else
                        {
                            InSpec = m_InSpec;
                            i++;
                        }
                    }
                    else if (args[i].EqualsCI("-outspec"))
                    {
                        if (i + 1 >= args.Length || !Enum.TryParse<LanguageOption>(args[i + 1], true, out m_OutSpec))
                        {
                            Console.WriteLine("Invalid argument: -outspec <format> is not valid.");
                            Console.WriteLine();
                            Console.WriteLine("Usage:");
                            PrintUsageNormalize();
                            Console.WriteLine();
                            PrintUsageLanguage();
                            result |= ParseResult.ProgramExecutionFailure;
                            break;
                        }
                        else
                            i++;
                    }
                    else if (args[i].EqualsCI("-outdir"))
                    {
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("Invalid argument: -outdir <path> is not valid.");
                            Console.WriteLine();
                            Console.WriteLine("Usage:");
                            PrintUsageNormalize();
                            Console.WriteLine();
                            PrintUsageLanguage();
                            result |= ParseResult.ProgramExecutionFailure;
                            break;
                        }
                        else
                        {
                            OutDir = args[i + 1];
                            i++;
                        }
                    }
                    else if (args[i].EqualsCI("-outfile"))
                    {
                        if (i + 1 >= args.Length)
                        {
                            Console.WriteLine("Invalid argument: -outfile <name> is not valid.");
                            Console.WriteLine();
                            Console.WriteLine("Usage:");
                            PrintUsageNormalize();
                            Console.WriteLine();
                            PrintUsageLanguage();
                            result |= ParseResult.ProgramExecutionFailure;
                            break;
                        }
                        else
                        {
                            OutFile = args[i + 1];
                            i++;
                        }
                    }
                    else if (i == args.Length - 1)
                    {
                        InPath = args[args.Length - 1];
                    }
                    else
                    {
                        Console.WriteLine("Invalid command line option: {0}", args[i]);
                        Console.WriteLine("Use UOSL -h for usage information");
                        return (int)ParseResult.ProgramExecutionFailure;
                    }
                }
            }

            if (!result.HasFlag(ParseResult.ProgramExecutionFailure))
            {
                try { result = FileParser.Process(InPath, InSpec, OutDir, OutFile, Overwrite, OutSpec, CheckOnly, DetailLevel); }
                catch (UOSLArgumentException ex)
                {
                    ConsoleUtils.PushColor(ConsoleColor.Red);
                    Console.WriteLine(ex.Message);
                    if (ex.InnerException != null)
                        Console.WriteLine(ex.InnerException.Message);
                    ConsoleUtils.PopColor();
                    result |= ParseResult.ProgramExecutionFailure;
                }
            }
            return (int)result;
        }
    }
}
