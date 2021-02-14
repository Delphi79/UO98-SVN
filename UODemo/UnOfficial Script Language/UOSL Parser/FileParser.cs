using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using JoinUO.UOSL.Service;
using Irony.Parsing;
using JoinUO.UOSL.Service.ASTNodes;

namespace JoinUO.UOSL
{
    public static class FileParser
    {
        public static ParseResult Process(string InPath = null, LanguageOption? InSpec = null, string OutPath = null, string OutFile = null, bool OverwriteOut = false, LanguageOption OutSpec = LanguageOption.Native, bool CheckOnly = false, ParserErrorLevel detaillevel = (ParserErrorLevel)(-1))
        {
            DirectoryInfo diIn = null, diOut = null;

            FileInfo[] Files=null;

            if (InPath != null)
            {
                if (Directory.Exists(InPath))
                    diIn = new DirectoryInfo(InPath);
                else if (!InPath.Contains(Path.DirectorySeparatorChar))
                    diIn = new DirectoryInfo(Directory.GetCurrentDirectory());
                else
                    try { diIn = new DirectoryInfo(Path.GetDirectoryName(InPath) ?? Path.GetFullPath(InPath)); }
                    catch (ArgumentException ex) { throw new UOSLArgumentException("Invalid input path.", ex); }
                    catch (PathTooLongException ex) { throw new UOSLArgumentException("Input path too long.", ex); }

                string inspec = Directory.Exists(InPath) ? null : Path.GetFileName(InPath);
                if (string.IsNullOrEmpty(inspec)) inspec = InSpec == LanguageOption.Native ? "*.uosl.q" : "*.uosl";
                if (File.Exists(InPath))
                    Files = new FileInfo[] { new FileInfo(InPath) };
                else
                    try
                    {
                        Files = diIn.GetFiles(inspec);
                    }
                    catch (IOException ex)
                    {
                        throw new UOSLArgumentException("Could not read input files.", ex);
                    }

                if (Files.Length == 0)
                    throw new UOSLArgumentException(string.Format("No files found to process in {0}.", InPath));

                if (Files.Length > 1 && OutFile != null)
                    throw new UOSLArgumentException(string.Format("Multiple files found to process in {0}, an outfile cannot be specified.", InPath));
            }

            if (OutPath != null)
            {
                if (Directory.Exists(OutPath))
                    diOut = new DirectoryInfo(OutPath);
                else
                    diOut = Directory.CreateDirectory(OutPath);
            }

            if (OutFile != null)
            {
                if (diOut == null)
                    diOut = new DirectoryInfo(Directory.GetCurrentDirectory());
                if (!Path.IsPathRooted(OutFile))
                    OutFile = Path.Combine(diOut.FullName, OutFile);
            }

            if (UOSLMain.Debug)
            {
                Console.WriteLine("InPath: {0}", InPath ?? "-null-");
                Console.WriteLine("diIn: {0}", diIn == null ? "-null-" : diIn.FullName);
                Console.WriteLine();
                Console.WriteLine("Files:");
                int c = 0;
                foreach (FileInfo file in Files)
                {
                    if (c++ > 20)
                    {
                        Console.WriteLine("   Truncated: {0} more files...", Files.Length-20);
                        break;
                    }
                    Console.WriteLine("\t{0}", file.Name);
                }
                Console.WriteLine();
                Console.WriteLine("OutPath: {0}", OutPath ?? "-null-");
                Console.WriteLine("diOut: {0}", diOut != null ? diOut.FullName : "-null-");
                Console.WriteLine("outFile: {0}", OutFile ?? "-null-");
            }

            int totalErrors = 0, totalWarnings = 0, totalInfo = 0;

            if (Files == null) Files = new FileInfo[] { null }; // this will indicate to read from consolein
            foreach (FileInfo file in Files)
            {
                List<Stream> toDispose = new List<Stream>();
                try
                {
                    Stream outstream, instream, errorstream;

                    string outfile = OutFile;

                    if (outfile == null && diOut != null)
                    {
                        outfile = string.Format("{0}.{1}", file == null ? "output" : file.Name, OutSpec != LanguageOption.Extended ? "q" : "uosl");
                        outfile = outfile.Replace(".q.uosl", ".uosl").Replace(".uosl.uosl", ".uosl").Replace(".q.q", ".q");
                        outfile = Path.Combine(diOut.FullName, outfile);
                    }

                    if (outfile != null)
                    {
                        if (File.Exists(outfile) && !OverwriteOut)
                            throw new UOSLArgumentException(string.Format("Output File {0} exists, and -overwrite not specified.", outfile));

                        if (file != null && (new FileInfo(outfile)).FullName == file.FullName)
                            throw new UOSLArgumentException(string.Format("infile and outfile are the same: {0}",file.FullName));

                        outstream = new FileStream(outfile, FileMode.Create, FileAccess.Write);
                        if (UOSLMain.Debug) Console.WriteLine("Created output file {0}.", outfile);
                        toDispose.Add(outstream);
                        if (UOSLMain.Dots)
                            Console.Write('.');
                    }
                    else
                        outstream = Console.OpenStandardOutput();
                    
                    if (file != null)
                    {
                        instream = file.OpenRead();
                        toDispose.Add(instream);
                    }
                    else
                        instream = Console.OpenStandardInput();

                    errorstream = Console.OpenStandardError();

                    ParseResult parseresult = Process(instream, file == null ? Path.Combine(Directory.GetCurrentDirectory(), "STDIN") : file.FullName, outstream, errorstream, ref totalErrors, ref  totalWarnings, ref  totalInfo, InSpec , OutSpec, CheckOnly, detaillevel);
                    foreach (Stream s in toDispose)
                        s.Dispose();
                    toDispose.Clear();

                    if (parseresult.HasFlag(ParseResult.ProgramExecutionFailure) && outfile!=null && File.Exists(outfile))
                        File.Delete(outfile);
                }
                finally
                {
                    foreach (Stream s in toDispose)
                        s.Dispose();
                }
            }

            ParseResult result = ParseResult.OK;
            if (totalErrors > 0) result |= ParseResult.HadErrors;
            if (totalWarnings > 0) result |= ParseResult.HadWarnings;
            if (totalInfo > 0) result |= ParseResult.HadInfo;
            return result;
        }

        private static Dictionary<LanguageOption, UOSLGrammar> grammars = new Dictionary<LanguageOption, UOSLGrammar>();

        public static ParseResult Process(Stream InputStream, string InFileName, Stream OutputStream, Stream ErrorStream, ref int totalErrors, ref int totalWarnings, ref int totalInfo, LanguageOption? InSpec = null, LanguageOption OutSpec = LanguageOption.Native, bool CheckOnly = false, ParserErrorLevel detaillevel = (ParserErrorLevel)(-1))
        {

            using (StreamWriter error = new StreamWriter(ErrorStream))
            {
                List<string> lines = null;
                using (StreamReader reader = new StreamReader(InputStream))
                {
                    lines = new List<string>();
                    string line;
                    int linenum = 0;

                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();


                        if (InSpec == null && linenum == 0 && line.StartsWith("//")) // check first line for language spec
                        {
                            LanguageOption inspec;
                            if (Utils.TryGetLanguageDeclaration(line, out inspec))
                                InSpec = inspec;
                            else if (InFileName != null)
                            {
                                if (InFileName.EndsWith(".q")) InSpec = LanguageOption.Native;
                            }
                        }
                        else
                        {
                            if (line.Length > 0 && line[line.Length - 1] == (char)0x0C)
                            {
                                if (line.Length > 1)
                                    lines.Add(line.Split((char)0x0C)[0]);
                                break; // FF
                            }
                            lines.Add(line);
                        }
                        linenum++;
                    }
                }

                if (InSpec == null)
                    InSpec = LanguageOption.Extended;

                if (detaillevel == ParserErrorLevel.Info)
                    error.WriteLine("Parsing {0} as {1}.", Path.GetFileName(InFileName), InSpec.ToString());

                UOSLGrammar grammar;
                if (!grammars.TryGetValue((LanguageOption)InSpec, out grammar))
                    grammar = grammars[(LanguageOption)InSpec] = new UOSLGrammar((LanguageOption)InSpec);

                Parser parser = new Parser(grammar);

                ParseResult result = ParseResult.OK;

                ParseTree tree = parser.Parse(string.Join("\n",lines), InFileName);

                IEnumerable<ParserMessage> messages = tree.ParserMessages.Where(msg => msg.Level >= detaillevel && (!(msg is ExternalErrorMessage) || ((ExternalErrorMessage)msg).FilePath==InFileName ));

                IEnumerable<string> externErrorFiles =
                    tree.ParserMessages.Where(msg => msg.Level >= ParserErrorLevel.Error && msg is ExternalErrorMessage && ((ExternalErrorMessage)msg).FilePath != InFileName)
                            .Select(msg => Path.GetFileName(((ExternalErrorMessage)msg).FilePath)).Distinct();

                int extCount = externErrorFiles.Count();

                int msgcount = messages.Count();
                if (msgcount > 0)
                {
                    error.Write("{0}: ", Path.GetFileName(InFileName));

                    error.WriteLine("{0} Messages:", msgcount + extCount);

                    if (extCount > 0)
                    {
                        ConsoleUtils.PushColor(ConsoleColor.Blue);
                        error.WriteLine("The following inherited files generated parser errors: {0}", string.Join(", ", externErrorFiles.ToArray()));
                        ConsoleUtils.PopColor();
                        result |= ParseResult.HadErrors;
                    }

                    foreach (ParserMessage errormsg in messages)
                    {
                        string head = string.Format("{0} {1}", errormsg.Level, errormsg.Location.ToUiString());

                        ConsoleColor color;
                        switch (errormsg.Level)
                        {
                            case ParserErrorLevel.Error: totalErrors++; color = ConsoleColor.Red; result |= ParseResult.HadErrors; break;
                            case ParserErrorLevel.Warning: totalWarnings++; color = ConsoleColor.Yellow; result |= ParseResult.HadWarnings; break;
                            default: totalInfo++; color = ConsoleColor.Green; result |= ParseResult.HadInfo; break;
                        }
                        ConsoleUtils.PushColor(color);
                        string msg = errormsg.Message;
                        while (msg.Length > 76)
                        {
                            int i = msg.Substring(0, 76).LastIndexOf(' ');
                            if (i < 60) i = 76;
                            error.WriteLine("  {0}", msg.Substring(0, i));
                            msg = msg.Substring(i).Trim();
                        }
                        error.WriteLine("  {0}", msg);

                        ConsoleUtils.PopColor();
                        if (lines != null)
                        {
                            string toout = errormsg.Location.Line < lines.Count ? lines[errormsg.Location.Line] : "INVALID ERROR LOCATION";
                            int offset = 0;
                            int maxlen = 71 - head.Length;
                            if (toout.Length > maxlen)
                            {
                                while (errormsg.Location.Column - offset > 60)
                                    offset += 20;
                                if (offset > 0)
                                {
                                    toout = "..." + toout.Substring(offset + 3);
                                    offset -= 3;
                                }
                                if (toout.Length > maxlen)
                                    toout = toout.Substring(0, maxlen - 3) + "...";
                            }

                            error.WriteLine("    {0}: {1}", head, toout);
                            for (int i = -2 - 4 - head.Length; i < errormsg.Location.Column - offset; i++)
                                error.Write(' ');
                            error.WriteLine('^');
                        }
                    }
                }

                if (!CheckOnly)
                {
                    if (tree == null || tree.Root == null || tree.Root.AstNode == null)
                    {
                        error.WriteLine("Failed to parse file {0}, cannot proceed with normalize", Path.GetFileName(InFileName));
                        result |= ParseResult.ProgramExecutionFailure;
                    }
                    else
                        using (StreamWriter writer = new StreamWriter(OutputStream))
                            writer.WriteLine(((ScopedNode)tree.Root.AstNode).GenerateScript(OutSpec));
                }

                return result;
            }
        }
    }
}
