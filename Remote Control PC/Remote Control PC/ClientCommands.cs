namespace Remote_Control_PC {
    public partial class Client {
        private bool CloseCommandLine(byte number) {
            if(number < COMMAND_LINES_PER_CLIENT && (commandLines[number]?.Close() ?? false)) {
                commandLines[number] = null;
                return true;
            }
            return false;
        }

        private bool AbortCommandLine(byte number) {
            return (number < COMMAND_LINES_PER_CLIENT && (commandLines[number]?.Abort() ?? false));
        }

        private bool IsRunning(byte number) {
            return commandLines[number]?.IsRunning ?? false;
        }

        private bool StartNewCommandLine(byte number) {
            if(number < COMMAND_LINES_PER_CLIENT && !IsRunning(number)) {
                commandLines[number] = new CommandLine(this, number);
                if(commandLines[number].Start())
                    return true;
                else {
                    commandLines[number] = null;
                    return false;
                }
            }
            return false;
        }

        private bool SendLastStrings() {
            for(byte i = 0; i < COMMAND_LINES_PER_CLIENT; i++) {
                if(commandLines[i] != null) {
                    SendText(commandLines[i].LastString, i);
                }
            }
            return true;
        }
    }
}
