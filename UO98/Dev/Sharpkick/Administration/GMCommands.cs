using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpkick.Administration
{
    class InvalidCommand : GMCommand
    {
        public InvalidCommand(GMCommand.CommandParameters CommandParams) : base(CommandParams) { }

        public override void Execute()
        {
            if (string.IsNullOrEmpty(Command))
                Server.SendSystemMessage(GMSerial, "Invalid command syntax: Missing command text.");
            else
                Server.SendSystemMessage(GMSerial, string.Format("GM Command {0} is not implemented.", Command));
        }
    }

    abstract class GMCommand
    {
        public struct CommandParameters
        {
            public int GmSerial;
            public string Command;
            public string[] Arguments;
        }

       public static GMCommand Instantiate(int gmSerial, string command)
        {
            GMCommandFactory factory = new GMCommandFactory(command);
            return factory.ConstructCommandInstance(gmSerial);
        }

        public abstract void Execute();

        public int GMSerial { get; private set; }
        public string Command { get; private set; }
        public string[] Arguments { get; private set; }

        protected GMCommand(GMCommand.CommandParameters CommandParams)
        {
            GMSerial = CommandParams.GmSerial;
            Command = CommandParams.Command;
            Arguments = CommandParams.Arguments;
        }

        private class GMCommandFactory
        {
            private static char[] CommandDelimiterCharacters = new char[] { ' ', '\t' };
            private string[] commandArray;

            public GMCommandFactory(string command)
            {
                commandArray = command.Split(CommandDelimiterCharacters, StringSplitOptions.RemoveEmptyEntries);
            }

            public GMCommand ConstructCommandInstance(int gmSerial)
            {
                CommandParameters CommandParams = new CommandParameters();
                CommandParams.GmSerial = gmSerial;
                CommandParams.Command = Command;
                CommandParams.Arguments = Arguments;

                switch (Command)
                {
                    case "invisible":
                        return new InvisibleCommand(CommandParams);
                    case "freeze":
                        return new FreezeCommand(CommandParams);
                    case "squelch":
                        return new SquelchCommand(CommandParams);
                    case "invulnerable":
                        return new InvulnerableCommand(CommandParams);
                    case "slay":
                        return new SlayCommand(CommandParams);
                    case "transfer":
                        return new TransferCommand(CommandParams);
                    case "delete":
                        return new DeleteCommand(CommandParams);
                    case "resurrect":
                        return new ResurrectCommand(CommandParams);
                    case "lightning":
                        return new LightningCommand(CommandParams);
                    case "zap":
                        return new ZapCommand(CommandParams);
                    case "kick":
                        return new KickCommand(CommandParams);
                    case "info":
                        return new PlayerInfoCommand(CommandParams);
                    default:
                        return new InvalidCommand(CommandParams);
                }
            }

            public string Command
            {
                get
                {
                    if (commandArray.Length > 0)
                        return commandArray[0];
                    else
                        return string.Empty;
                }
            }

            public string[] Arguments
            {
                get
                {
                    if (commandArray.Length > 1)
                        return commandArray.Skip(1).ToArray();
                    else
                        return null;
                }
            }
        }
    
    }
}
