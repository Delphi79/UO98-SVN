using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Administration
{
    class PlayerInfoCommand : GMCommand
    {
        public PlayerInfoCommand(GMCommand.CommandParameters CommandParams) : base(CommandParams) { }

        public override void Execute()
        {
            int playerserial;
            if(Arguments.Length != 1 || !int.TryParse(Arguments[0], out playerserial))
            {
                Server.SendSystemMessage(GMSerial, "Invalid command syntax, usage: info <serial>");
            }
            else
            {
                Server.OpenInfoWindow(GMSerial, playerserial);
            }
        }
    }
}
