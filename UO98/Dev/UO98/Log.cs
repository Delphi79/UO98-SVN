using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UO98
{
    public class Log : TextWriter, IDisposable
    {
        private string m_FileName;
        private bool m_NewLine;
        public const string DateFormat = "[MMM d HH:mm:ss]: ";

        public string FileName { get { return m_FileName; } }

        public Log(string file)
            : this(file, false)
        {
        }

        public Log(string file, bool append)
        {
            m_FileName = file;
            using(FileStream fs = new FileStream(m_FileName, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read))
            using(StreamWriter writer = new StreamWriter(fs))
            {
                writer.WriteLine(">>>Logging started on {0}.", DateTime.Now.ToString("f")); //f = Tuesday, April 10, 2001 3:51 PM 
            }
            m_NewLine = true;
        }

        public override void Write(char ch)
        {
            using(StreamWriter writer = new StreamWriter(new FileStream(m_FileName, FileMode.Append, FileAccess.Write, FileShare.Read)))
            {
                if(m_NewLine)
                {
                    writer.Write(DateTime.Now.ToString(DateFormat));
                    m_NewLine = false;
                }
                writer.Write(ch);
            }
        }

        public override void Write(string str)
        {
            using(FileStream fs = new FileStream(m_FileName, FileMode.Append, FileAccess.Write, FileShare.Read))
            using(StreamWriter writer = new StreamWriter(fs))
            {
                if(m_NewLine)
                {
                    writer.Write(DateTime.Now.ToString(DateFormat));
                    m_NewLine = false;
                }
                writer.Write(str);
            }
        }

        public override void WriteLine(string line)
        {
            using(FileStream fs = new FileStream(m_FileName, FileMode.Append, FileAccess.Write, FileShare.Read))
            using(StreamWriter writer = new StreamWriter(fs))
            {
                if(m_NewLine)
                    writer.Write(DateTime.Now.ToString(DateFormat));
                writer.WriteLine(line);
                m_NewLine = true;
            }
        }

        public override System.Text.Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }
    }
}
