using System;
using SocketEx;
using System.Threading;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace Remote_Control {
    public struct Message<TValue> {
        public readonly TValue Value;
        public readonly byte ProcessNumber;
        public Message(TValue value, byte processNumber) {
            Value = value;
            ProcessNumber = processNumber;
        }
    }

    public class Client {
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

        public event EventHandler<Message<string>> TextReceived;
        public event EventHandler<Message<byte>> CommandReceived;
        public event PropertyChangedEventHandler ConnectedChanged;

        public TcpClient Tcp { get; set; }
        public NetworkStream Stream { get; set; }
        public BinaryReader Reader { get; set; }
        public BinaryWriter Writer { get; set; }
        private bool mConnected;
        public bool Connected {
            get {
                return mConnected;
            }
            private set {
                if(mConnected != value) {
                    mConnected = value;
                    ConnectedChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Connected)));
                }
            }
        }
        private Thread dataReceiveThread;

        public void Init(IPEndPoint address, Action onConnected) {
            try {
                Close();
                Tcp?.Dispose();
                Tcp = new TcpClient();
                Tcp.BeginConnect(address.Address, address.Port, e => onConnected(), null);
                Stream = Tcp.GetStream() as NetworkStream;
                Reader = new BinaryReader(Stream);
                Writer = new BinaryWriter(Stream);
            }
            catch {
            }
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
                        case READY_BYTE:
                            Writer.Write(READY_BYTE);
                            break;

                        case COMMAND_BYTE:
                            byte command = Reader.ReadByte();
                            byte processNumber = Reader.ReadByte();
                            CommandReceived?.Invoke(this, new Message<byte>(command, processNumber));
                            break;

                        case TEXT_BYTE:
                            string text = Reader.ReadString();
                            processNumber = Reader.ReadByte();
                            TextReceived?.Invoke(this, new Message<string>(text, processNumber));
                            break;
                    }
                }
                catch {
                    Connected = false;
                    return;
                }
            }
            Connected = false;
        }

        public bool EstablishConnection() {
            try {
                Writer?.Write(READY_BYTE);
                if(Reader?.ReadByte() == READY_BYTE) {
                    dataReceiveThread = new Thread(DataReceive);
                    dataReceiveThread.Start();
                    Connected = true;
                    return true;
                }
            }
            catch {
            }
            return false;
        }

        public void CloseProcess(byte number) {
            SendCommand(COMMAND_CLOSE_PROCESS, number);
        }

        public void AddProcess(byte number) {
            SendCommand(COMMAND_ADD_PROCESS, number);
        }

        public void GetLastStrings() {
            SendCommand(COMMAND_GET_LAST_STRINGS, 0);
        }

        public void Abort(byte number) {
            SendCommand(COMMAND_ABORT, number);
        }

        public void Close() {
            dataReceiveThread?.Abort();
            Stream?.Close();
            Reader?.Close();
            Writer?.Close();            
        }
    }
}
