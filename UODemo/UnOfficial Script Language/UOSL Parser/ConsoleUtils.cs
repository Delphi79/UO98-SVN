using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoinUO.UOSL
{
    static class ConsoleUtils
    {
        private static Stack<ConsoleColor> m_ConsoleColors = new Stack<ConsoleColor>();

        public static void PushColor(ConsoleColor color)
        {
            try
            {
                m_ConsoleColors.Push(Console.ForegroundColor);
                Console.ForegroundColor = color;
            }
            catch { }
        }

        public static void PopColor()
        {
            try
            {
                Console.ForegroundColor = m_ConsoleColors.Pop();
            }
            catch { }
        }
    }
}
