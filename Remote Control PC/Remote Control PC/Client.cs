using System;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Linq;
using System.Net;

namespace Remote_Control_PC {
    public partial class Client {
        public const byte COMMAND_LINES_PER_CLIENT = 8;

        public const byte READY_BYTE = 0x01;
        public const byte COMMAND_BYTE = 0x02;
        public const byte TEXT_BYTE = 0x03;

        public const byte COMMAND_FAIL = 0;
        public const byte COMMAND_SUCCESS = 1;
        public const byte COMMAND_ADD_PROCESS = 2;
        public const byte COMMAND_ABORT = 3;
        public const byte COMMAND_CLOSE_PROCESS = 4;
        public const byte COMMAND_GET_LAST_STRINGS = 5;
        public const byte COMMAND_GET_OPENED_CMDS = 6;

        public event EventHandler ConnectionClosed;

        public TcpClient Tcp { get; set; }
        public NetworkStream Stream { get; set; }
        public BinaryReader Reader { get; set; }
        public BinaryWriter Writer { get; set; }
        public IPAddress IP => (Tcp?.Client?.RemoteEndPoint as IPEndPoint)?.Address;

        private Thread dataReceiveThread;
        private CommandLine[] commandLines = new CommandLine[COMMAND_LINES_PER_CLIENT];
        private byte lastSentCommand;

        public Client(TcpClient tcpClient) {            
            Init(tcpClient);
        }

        public void Init(TcpClient tcpClient) {
            Tcp = tcpClient;
            Stream = Tcp?.GetStream();
            Reader = new BinaryReader(Stream);
            Writer = new BinaryWriter(Stream);
        }

        public void UpdateCommandLines() {
            commandLines.ToList().ForEach(cmd => cmd?.Update());
        }

        public void GetOpenedCmds() {
            SendCommand(COMMAND_GET_OPENED_CMDS, 0);
        }

        public void SendText(string text, byte processNumber) {
            if(Tcp?.Connected ?? false) {
                try {
                    Writer.Write(TEXT_BYTE);
                    Writer.Write(text);
                    Writer.Write(processNumber);
                }
                catch {
                }
            }
        }

        public void SendCommand(byte command, byte processNumber) {
            if(Tcp?.Connected ?? false) {
                try {
                    Writer.Write(COMMAND_BYTE);
                    Writer.Write(command);
                    Writer.Write(processNumber);
                    lastSentCommand = command;
                }
                catch {
                }
            }
        }

        private void DataReceive() {
            while(Tcp?.Connected ?? false) {
                try {
                    byte message = Reader.ReadByte();
                    switch(message) {
                        case COMMAND_BYTE:
                            byte command = Reader.ReadByte();
                            byte processNumber = Reader.ReadByte();
                            CommandReceived(command, processNumber);
                            break;

                        case TEXT_BYTE:
                            string text = Reader.ReadString();
                            processNumber = Reader.ReadByte();
                            if(!string.IsNullOrEmpty(text))
                                TextReceived(text, processNumber);
                            break;
                    }
                }
                catch {
                    ConnectionClosed?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
            ConnectionClosed?.Invoke(this, EventArgs.Empty);
        }

        private void CommandReceived(byte command, byte processNumber) {
            bool success;
            switch(command) {
                case COMMAND_ADD_PROCESS:
                    success = StartNewCommandLine(processNumber);
                    break;

                case COMMAND_ABORT:
                    success = AbortCommandLine(processNumber);
                    break;

                case COMMAND_CLOSE_PROCESS:
                    success = CloseCommandLine(processNumber);
                    break;

                case COMMAND_GET_LAST_STRINGS:
                    success = SendLastStrings();
                    break;

                case COMMAND_GET_OPENED_CMDS:
                    bool opened;
                    success = true;
                    for(byte i = 0; i < COMMAND_LINES_PER_CLIENT; i++) {
                        opened = Convert.ToBoolean((processNumber >> i) & 1);
                        if(opened) success &= StartNewCommandLine(i);
                    }
                    break;

                default:
                    success = false;
                    break;
            }
            SendCommand(success ? COMMAND_SUCCESS : COMMAND_FAIL, processNumber);
        }

        private void TextReceived(string text, byte number) {
            if(number < COMMAND_LINES_PER_CLIENT) {
                commandLines[number]?.Write(text);
            }
        }
        
        public bool EstablishConnection() {
            try {
                if(Reader?.ReadByte() == READY_BYTE) {
                    Writer?.Write(READY_BYTE);
                    dataReceiveThread = new Thread(DataReceive);
                    dataReceiveThread.Start();
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        public void Close() {
            dataReceiveThread?.Abort();
            Stream?.Close();
            Reader?.Close();
            Writer?.Close();
        }
    }
}
