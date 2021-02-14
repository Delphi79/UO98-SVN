using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace UO98
{
    sealed class ServerProcess
    {
        readonly string BinDirectory = string.Empty;
        readonly string UODemoPlusFilename = "UODemo+.exe";

        static readonly Dictionary<string, string> EnvVars = new Dictionary<string, string>()
        {
            {"REGKEYNAME","UO98"},
            {"SERVERNAME","UO98"},
            {"RUNDIR","rundir"},
            {"REALDAMAGE","YES"},
            {"USEACCOUNTNAME","YES"},
            {"SAVEDYNAMIC0","NO"},
            {"UODEMODLL","Sidekick.dll"},
            {"NOCONSOLE","YES"},
        };

        bool DoExit { get; set; }

        Process MyProcess;

        public ServerProcess(string workingfolder)
        {
            BinDirectory = workingfolder;
            EnvVars["UODEMODLL"] = Path.Combine(BinDirectory, EnvVars["UODEMODLL"]);
        }

        public StreamReader StdOut
        {
            get
            {
                try
                {
                    return MyProcess == null ? null : MyProcess.StandardOutput;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
            }
        }

        EventHandler ExitEventHandler = null;

        object lockStartStop = new object();
        public void Start()
        {
            lock (lockStartStop)
            {
                if (Running) return;
                Running = true;
                DoExit = false;

                if(MyProcess != null && ExitEventHandler != null)
                    MyProcess.Exited -= ExitEventHandler;

                MyProcess = CreateProcess();
                lastExitCode = 0;
                ExitEventHandler = new EventHandler(MyProcess_Exited);
                MyProcess.Exited += ExitEventHandler;
                MyProcess.OutputDataReceived += new DataReceivedEventHandler(OutputDataReceived);
                MyProcess.Start();
                MyProcess.BeginOutputReadLine();
            }
        }

        void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Console.WriteLine(e.Data);
        }

        public int lastExitCode { get; private set; }

        public class OnExitedEventArgs : EventArgs { public int ExitCode { get; set; } }
        public event EventHandler<OnExitedEventArgs> OnProcessExited;

        void MyProcess_Exited(object sender, EventArgs e)
        {
            if(sender == MyProcess)
            {
                lastExitCode = MyProcess.ExitCode;
                if(OnProcessExited!=null)
                {
                    OnExitedEventArgs args = new OnExitedEventArgs() { ExitCode = lastExitCode };
                    OnProcessExited(this, args);
                }
            }
        }

        public void Stop()
        {
            lock (lockStartStop)
            {
                DoExit = true;
            
                if (MyProcess != null && !MyProcess.HasExited)
                {
                    MyProcess.OutputDataReceived -= OutputDataReceived;

                    MyProcess.Kill();
                    MyProcess.WaitForExit(1000);
                    MyProcess = null;
                }
                Running = false;
            }
        }

        bool Running { get; set; }
        public bool IsRunning
        {
            get
            {
                lock (lockStartStop)
                {
                    if (MyProcess == null || MyProcess.HasExited)
                        Running = false;
                    return Running;
                }
            }
        }

        Process CreateProcess()
        {
            var p = new Process();
            p.StartInfo.FileName = Path.Combine(BinDirectory, UODemoPlusFilename);
            Console.Write("Initializing runtime environment for {0} with ", UODemoPlusFilename);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.WorkingDirectory = BinDirectory;
            SetUpEnvironment(p.StartInfo);
            return p;
        }

        void SetUpEnvironment(ProcessStartInfo pInfo)
        {
            Console.WriteLine("options:");
            foreach(KeyValuePair<string, string> var in EnvVars)
            {
                Console.WriteLine("\t{0}={1}", var.Key, var.Value);
                pInfo.EnvironmentVariables[var.Key] = var.Value;
            }
        }

    }
}
