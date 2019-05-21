using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Remote_Control_PC {
    public partial class EmptyForm : Form {
        private const string CONFIG_FILENAME = "port";

        private bool run = true;
        private int port = 0;
        private List<Client> clients = new List<Client>();
        private TcpListener listener;
        private QRCodeForm qrCodeForm;

        public EmptyForm() {
            InitializeComponent();

            if(!InitNetwork()) {
                Clenanup();
                Application.Exit();
            }
            updateTimer.Start();
        }

        private void OnExitClick(object sender, EventArgs e) {
            Clenanup();
            Application.Exit();
        }

        private void OnSyncClick(object sender, EventArgs e) {
            qrCodeForm?.Close();
            qrCodeForm?.Dispose();
            qrCodeForm = new QRCodeForm(port);
            qrCodeForm.Visible = false;
            qrCodeForm.Show(this);
        }

        private bool InitNetwork() {
            if(File.Exists(CONFIG_FILENAME)) {
                string text = File.ReadAllText(CONFIG_FILENAME);
                if(!int.TryParse(text, out port))
                    port = 0;
            }
            int oldPort = port;
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            port = (listener.LocalEndpoint as IPEndPoint).Port;
            if(port != oldPort) {
                File.WriteAllText(CONFIG_FILENAME, port.ToString());
            }
            WaitForConnection();
            return true;
        }

        private Client ConnectedBefore(TcpClient tcpClient) {
            IPAddress ip = (tcpClient?.Client?.RemoteEndPoint as IPEndPoint)?.Address;
            foreach(var client in clients) {
                if(ip.Equals(client.IP))
                    return client;
            }
            return null;
        }

        private async void WaitForConnection() {
            while(run) {
                try {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    Client client = ConnectedBefore(tcpClient);
                    bool newClient = false;
                    if(client != null)
                        client.Init(tcpClient);
                    else {
                        client = new Client(tcpClient);
                        newClient = true;
                    }
                    if(client.EstablishConnection()) {
                        client.ConnectionClosed += (s, e) => client.EstablishConnection();
                        lock(clients) {
                            clients.Add(client);
                        }
                        if(newClient)
                            client.GetOpenedCmds();
                        qrCodeForm?.Close();
                    } 
                }
                catch {
                }
            }
        }

        private void Clenanup() {
            run = false;
            clients?.ForEach(client => client?.Close());
            listener?.Stop();
            qrCodeForm?.Dispose();
        }

        private void EmptyForm_FormClosed(object sender, FormClosedEventArgs e) {
            Clenanup();
        }

        private void updateTimer_Tick(object sender, EventArgs e) {            
            lock (clients) {
                clients.ForEach(client => client?.UpdateCommandLines());
            }
        }
    }
}
