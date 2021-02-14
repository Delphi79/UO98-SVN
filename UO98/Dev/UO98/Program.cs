using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;

namespace UO98
{
    class Program : IDisposable
    {
        private static bool isclosing = false;

        const string ScriptCompilerFileName="uosl.exe";
        const string ScriptCompilerArguments=@"-outspec Enhanced -outdir ..\rundir\scripts.uosl -overwrite ..\rundir\scripts.uosl\*.uosl";

        static ServerProcess process;

        private static string _workingdir = null;
        public static string WorkingDirectory
        {
            get { return _workingdir ?? (_workingdir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)); }
        }

        static void Main(string[] args)
        {
            #region Service Control

            if (args.Length == 1 && (Insensitive.Equals(args[0], "-service")
                || Insensitive.Equals(args[0], "installservice")
                || Insensitive.Equals(args[0], "uninstallservice")
                || Insensitive.Equals(args[0], "restartservice")
                || Insensitive.Equals(args[0], "stopservice")
                || Insensitive.Equals(args[0], "startservice")))
            {
                if (Insensitive.Equals(args[0], "-service"))
                {
                    Directory.SetCurrentDirectory((new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName));
                    UO98Service.Main();
                    return;
                }
                else if (Insensitive.Equals(args[0], "restartservice"))
                {
                    UO98Service.ServiceControlRestart();
                }
                else if (Insensitive.Equals(args[0], "stopservice"))
                {
                    UO98Service.ServiceControlStop();
                }
                else if (Insensitive.Equals(args[0], "startservice"))
                {
                    UO98Service.ServiceControlStart();
                }
                else if (Insensitive.Equals(args[0], "installservice") )
                {
                    UO98Service.Install();
                }
                else if (Insensitive.Equals(args[0], "uninstallservice"))
                {
                    UO98Service.UnInstall();
                }
            }
            #endregion

            bool respawn=false;
            if(args.Length >= 1)
            {
                if(args.Length == 1 && Insensitive.Equals(args[0], "respawn"))
                    respawn = true;
                else
                {
                    Console.WriteLine("Usage: UO98 [respawn|installservice|uninstallservice|startservice|stopservice|restartservice]");

                    return;
                }
            }

            SetConsoleCtrlHandler(ConsoleCtrlCheck, true);

            Run(respawn);
        }

        internal static void Run()
        {
            Run(false);
        }

        private static MultiTextWriter m_MultiConOut=null;

        internal static UnhandledExceptionEventHandler UnhandledExceptionHandler = null;

        internal static void Run(bool respawn)
        {
            if(m_MultiConOut == null)
                Console.SetOut(m_MultiConOut = new MultiTextWriter(Console.Out, new Log(GetLogNameAndBackupExisting("..\\Logs","Console.log"))));

            if(UnhandledExceptionHandler == null)
                AppDomain.CurrentDomain.UnhandledException += (UnhandledExceptionHandler = new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException));

            CompileScripts();

            if(process == null)
            {
                process = new ServerProcess(WorkingDirectory);
                process.OnProcessExited += new EventHandler<ServerProcess.OnExitedEventArgs>(process_OnProcessExited);
            }

            try
            {
                do
                {
                    process.Start();
                    while(process.IsRunning && !isclosing)
                    {
                        Thread.Sleep(250);
                    }
                } while(respawn && !isclosing);
            }
            finally
            {
                process.Stop();
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Console.WriteLine("Unhandled Exception:\n{0}",ex.ToString());
        }

        static void CompileScripts()
        {
            RunSimpleProcess(ScriptCompilerFileName, ScriptCompilerArguments);
        }

        static void RunSimpleProcess(string exeName, string arguments = null)
        {
            System.Diagnostics.Process compileProcess=new System.Diagnostics.Process();
            compileProcess.StartInfo.UseShellExecute = true;
            compileProcess.StartInfo.FileName = exeName;
            compileProcess.StartInfo.Arguments = arguments;
            compileProcess.StartInfo.WorkingDirectory = WorkingDirectory;
            compileProcess.Start();
            compileProcess.WaitForExit();
        }

        public class OnEventLogMessageArgs : EventArgs { public string Message { get; set; } }
        public static event EventHandler<OnEventLogMessageArgs> OnEventLogMessage;
        public static event EventHandler<ServerProcess.OnExitedEventArgs> OnProcessExited;

        static void process_OnProcessExited(object sender, ServerProcess.OnExitedEventArgs e)
        {
            string message = string.Format("Process exited with code {0}", e.ExitCode);
            if(OnEventLogMessage != null) OnEventLogMessage(sender, new OnEventLogMessageArgs() { Message = message });
            if(OnProcessExited != null) OnProcessExited(sender, e);
        }

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public delegate bool HandlerRoutine(CtrlTypes CtrlType);

        public enum CtrlTypes
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        private static bool ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                case CtrlTypes.CTRL_BREAK_EVENT:
                case CtrlTypes.CTRL_CLOSE_EVENT:
                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    process.Stop();
                    isclosing = true;
                    Console.WriteLine("Exiting.");
                    break;
            }
            return true;
        }

        public static string GetLogNameAndBackupExisting(string LogPath, string LogFilename)
        {
            if(!Directory.Exists(LogPath))
                Directory.CreateDirectory(LogPath);

            string file = Path.Combine(LogPath, LogFilename);

            if(File.Exists(file))
            {
                FileInfo fi = new FileInfo(file);
                DateTime logdate = fi.CreationTime;
                string DateCode = MakeDateCode(DateTime.Now);
                string ext = Path.GetExtension(file);
                int i = 0;
                string newfile;
                while(File.Exists(newfile = string.Format("{0}{1}{2}{3}", file.Substring(0, file.LastIndexOf(ext)), DateCode, i == 0 ? "" : string.Format("({0})", i.ToString("D2")), ext)))
                    i++;

                File.Move(file, newfile);
            }

            return (file);
        }

        public static string MakeDateCode(DateTime datetime)
        {
            return datetime.ToString("yyyyMMdd");
        }
        public static string MakeDateTimeCode(DateTime datetime)
        {
            return datetime.ToString("yyyyMMddHHmmss");
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (process != null)
                process.Stop();
        }

        #endregion
    }

}
