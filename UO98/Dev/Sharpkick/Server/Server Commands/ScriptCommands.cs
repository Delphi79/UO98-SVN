using System;
using System.Text;

namespace Sharpkick
{
    static partial class Server
    {
        /// <summary>
        /// Add script by name to object
        /// </summary>
        /// <param name="serial">The serial of the object to add script to</param>
        /// <param name="scriptName">the name of the script without extension</param>
        /// <param name="executeCreation">Should the creation method of the script be called?</param>
        /// <returns>Error message or null if successful</returns>
        unsafe public static string addScript(int serial, string scriptName, bool executeCreation = true)
        {
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(scriptName))
            {
                byte* result = Core.addScript(serial, pName, executeCreation ? 1 : 0);
                if (result == null) return null;
                return StringPointerUtils.GetAsciiString(result);
            }
        }

        unsafe public static bool hasScript(int serial, string scriptName)
        {
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(scriptName))
            {
                return Core.hasScript(serial, pName);
            }
        }

        unsafe public static bool detachScript(int serial, string scriptName)
        {
            fixed (byte* pName = ASCIIEncoding.ASCII.GetBytes(scriptName))
            {
                return Core.detachScript(serial, pName);
            }
        }

    }
}
