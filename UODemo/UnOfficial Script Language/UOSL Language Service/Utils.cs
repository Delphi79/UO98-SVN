using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JoinUO.UOSL.Service
{
    public static class Utils
    {
        public static LanguageOption DetermineFileLanguage(string fullpath, LanguageOption defaultlang = LanguageOption.Enhanced)
        {
            if(File.Exists(fullpath))
            {
                string line=null;
                using (StreamReader reader = new StreamReader(fullpath))
                    line = reader.ReadLine();
                LanguageOption read;
                if (line != null && line.StartsWith("//") && TryGetLanguageDeclaration(line, out read))
                    return read;
            }

            if (fullpath.EndsWith(".uosl")) return LanguageOption.Extended;
            //if (fullpath.EndsWith(".uosl.q")) return defaultlang;
            return defaultlang;
        }

        public static bool TryGetLanguageDeclaration(string line, out LanguageOption option)
        {
            int posUOSL;
            if ((posUOSL = line.IndexOf(" UOSL ")) > 0)
            {   // Try to extract language option from line
                int j = 0;
                for (int i = posUOSL + 6; i < line.Length; i++)
                    if (j == 0 && char.IsLetter(line[i]))
                        j = i;
                    else if (j > posUOSL && !char.IsLetter(line[i]))
                        return Enum.TryParse<LanguageOption>(line.Substring(j, i - j), true, out option);
            }
            option = 0;
            return false;
        }

        public static string FindFile(string path, string PathHint = null)
        {
            string trypath;
            if (File.Exists(path))
                return path;

            if (PathHint != null)
            {
                if (File.Exists(PathHint))
                    PathHint = (new FileInfo(PathHint)).DirectoryName;
                else if (Directory.Exists(PathHint))
                    PathHint = (new DirectoryInfo(PathHint)).FullName;
                else
                    PathHint = null;

                if (PathHint != null)
                    if (File.Exists(trypath = Path.Combine(PathHint, path)))
                        return trypath;
            }

            if (File.Exists(trypath = Path.Combine("..", path)))
                return trypath;

            return null;
        }

    }
}
