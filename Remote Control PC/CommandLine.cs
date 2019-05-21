using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Remote_Control_PC {
    public class CommandLine {
        public const int LAST_STRING_LENGTH = 1300;
        private const int VK_CONTROL = 0x11;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;

        public readonly Client Owner;
        public readonly Process Process;
        public string LastString { get; private set; } = "";
        public bool IsRunning => !Process.HasExited;

        private byte number;
        private bool started = false;
        private Thread outputReadingThread;
        private Thread errorReadingThread;
        private volatile string readingResult = string.Empty;

        public CommandLine(Client owner, byte number) {
            this.Owner = owner;
            this.number = number;
            ProcessStartInfo info = new ProcessStartInfo("cmd.exe") {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                ErrorDialog = false
            };
            Process = new Process() {
                StartInfo = info,
                EnableRaisingEvents = true
            };
            Process.Exited += ProcessExited;
        }

        private void ProcessExited(object sender, EventArgs e) {
            Owner?.SendCommand(Client.COMMAND_CLOSE_PROCESS, this.number);
            this.Close();
        }

        private string Encode(Encoding fromEncoding, Encoding toEncoding, string text) {
            var bytes = fromEncoding.GetBytes(text);
            return toEncoding.GetString(bytes);
        }

        private string NormalizeResult(string text) {
            return Encode(Process.StandardOutput.CurrentEncoding, Encoding.GetEncoding(866), text);
        }

        private void ReadingFromOutput() {
            char c;
            while(!Process.HasExited) {
                c = (char)Process.StandardOutput.Read();
                readingResult += c;
            }
        }

        private void ReadingFromError() {
            char c;
            while(!Process.HasExited) {
                c = (char)Process.StandardError.Read();
                readingResult += c;
            }
        }

        private string ReadFromCmd() {
            lock(readingResult) {
                string result = NormalizeResult(readingResult);
                readingResult = string.Empty;
                return result;
            }
        }

        public void Update() {
            if(!started)
                return;

            string output = ReadFromCmd();
            if(string.IsNullOrEmpty(output))
                return;

            LastString += output;
            int length = LastString.Length;
            if(length > LAST_STRING_LENGTH)
                LastString = LastString.Remove(0, length - LAST_STRING_LENGTH);

            Owner?.SendText(output, this.number);
        }

        private void InitThreads() {
            outputReadingThread = new Thread(ReadingFromOutput);
            outputReadingThread.IsBackground = true;
            outputReadingThread.Start();

            errorReadingThread = new Thread(ReadingFromError);
            errorReadingThread.IsBackground = true;
            errorReadingThread.Start();
        }

        public bool Start() {
            try {
                Process.Start();
                Process.StandardInput.AutoFlush = false;
                InitThreads();
                started = true;
                return true;
            }
            catch {
                return false;
            }
        }

        public void Write(string text) {
            if(Process != null && started) {
                text = Encode(Encoding.GetEncoding(866), Process.StandardInput.Encoding, text);
                Process.StandardInput.WriteLine(text);
                Process.StandardInput.Flush();
            }            
        }

        public bool Abort() {
            if(Process == null || !started)
                return false;
            Process.StandardInput.WriteLine("\x3");   // 3 char is end-of-line or Ctrl+C in the cmd
            Process.StandardInput.Flush();
            return true;
        }

        public bool Close() {
            try {
                started = false;
                outputReadingThread?.Abort();
                Process?.Kill();
                Process?.Dispose();
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
