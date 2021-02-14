using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace JoinUO.UOSL
{
    static partial class UOSLMain
    {
        static void PrintUsageAll()
        {
            PrintUsageHelp();
            Console.WriteLine();
            PrintUsageCheck();
            Console.WriteLine();
            PrintUsageNormalize();
            Console.WriteLine();
            PrintUsageLanguage();
        }

        static void PrintUsageAbout()
        {
            Console.Write(((AssemblyTitleAttribute)(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), true)[0])).Title + " ");
            Console.WriteLine(((AssemblyCopyrightAttribute)(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true)[0])).Copyright);
            Console.WriteLine(((AssemblyDescriptionAttribute)(Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true)[0])).Description);
            Console.WriteLine(FullName);
        }

        static void PrintUsageHelp()
        {
            Console.WriteLine("{0} -h", ShortName);
            Console.WriteLine(" Prints this message.");
        }

        static void PrintUsageCheck()
        {
            Console.WriteLine("{0} -check [Options...] [<filespec>]", ShortName);
            Console.WriteLine(" Parses and reports messages on a single file or group of files.");
            Console.WriteLine("  Options:");
            Console.WriteLine("   -detail <level>    Level of detail in err msgs (Error, Warning, Info)");
            Console.WriteLine("   -inspec <format>   Force Input Language Specification. Default: Detect");
            Console.WriteLine("   <filespec>         File or path specification for input files. Reads");
            Console.WriteLine("                      from Console if omitted, terminate input with EOF.");
        }

        static void PrintUsageNormalize()
        {
            Console.WriteLine("{0} [Options...] [<filespec>]", ShortName);
            Console.WriteLine(" Normalizes or converts the input between language specifications.");
            Console.WriteLine("  Options:");
            Console.WriteLine("   -detail <level>    Level of detail in err msgs (Error, Warning, Info)");
            Console.WriteLine("   -dots              Prints dots to the console to show file list progress");
            Console.WriteLine("   -inspec  <format>  Force Input Language Specification. Default: Detect");
            Console.WriteLine("   -outspec <format>  Output Language Specification. Default: Native");
            Console.WriteLine("   -outdir <path>     Folder path for output files.*");
            Console.WriteLine("   -outfile <name>    Single output filename. Only if filespec is folder.*");
            Console.WriteLine("   -overwrite         Output file ok to overwrite, otherwise error if exists");
            Console.WriteLine("         If outdir and outfile are omitted, all output is written to standard");
            Console.WriteLine("         out, with EOF's for multiple files, and any parse error, warning or");
            Console.WriteLine("         Info messages will be written to stderr.");
            Console.WriteLine("   <filespec>         File or path specification for input files. Reads from");
            Console.WriteLine("                      Console if omitted, terminate input with EOF.");
            Console.WriteLine();
            Console.WriteLine(" *All filenames and paths must be quoted if they contain whitespace.");
        }

        static void PrintUsageLanguage()
        {
            Console.WriteLine("Language Specifications: (format)");
            Console.WriteLine("   Native:");
            Console.WriteLine("      The native UOSL language + optional comments.");
            Console.WriteLine("   Enhanced:");
            Console.WriteLine("      Supports/Enforces enhanced trigger statements");
            Console.WriteLine("   Extended:");
            Console.WriteLine("      Supports Enhanced constructs plus constants, unbraced loops/if statements");
        }

    }
}
