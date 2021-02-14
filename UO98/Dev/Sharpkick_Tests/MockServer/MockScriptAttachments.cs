using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sharpkick_Tests
{
    static class MockScriptAttachments
    {
        static Dictionary<int, List<string>> Scripts = new Dictionary<int, List<string>>();

        public static void AddScript(int serial, string name)
        {
            if (!Scripts.ContainsKey(serial))
                Scripts[serial] = new List<string>();
            if (Has(serial, name))
                Remove(serial, name);
            Scripts[serial].Add(name);
        }

        public static void DeleteAllFor(int serial)
        {
            if (Scripts.ContainsKey(serial))
                Scripts.Remove(serial);
        }

        public static void Remove(int serial, string name)
        {
            if (Scripts.ContainsKey(serial))
            {
                Scripts[serial].RemoveAll(att => att.Equals(name));
            }
        }

        public static bool Has(int serial, string name)
        {
            if (!Scripts.ContainsKey(serial))
                return false;

            IEnumerable<string> list = Scripts[serial].Where(att => att == name);
            return list.Count() > 0;
        }

        static DirectoryInfo ScriptsFolder = GetScriptsFolder();

        public static bool IsValidScriptName(string script)
        {
            if(ScriptsFolder==null)
                return script!="doesnotexist";
            else
                return ScriptsFolder.GetFiles(string.Format("{0}.*",script)).Length > 0;
        }

        public static DirectoryInfo GetScriptsFolder()
        {
            string path = FindScriptsFolder(new DirectoryInfo(Directory.GetCurrentDirectory()));
            if (path != null)
                return new DirectoryInfo(path);
            else
                return null;
        }

        const string scriptsFolderName = "rundir\\scripts.uosl";
        public static string FindScriptsFolder(DirectoryInfo workUpFromPath)
        {
            string path = Path.Combine(workUpFromPath.FullName, scriptsFolderName);
            if (Directory.Exists(path))
                return path;
            else if (workUpFromPath == workUpFromPath.Root)
                return null;
            else
                return FindScriptsFolder(workUpFromPath.Parent);
        }
    }
}
