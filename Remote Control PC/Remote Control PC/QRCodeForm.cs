using System;
using System.Windows.Forms;
using System.Drawing;
using System.Net;
using System.Net.Sockets;

using QRCoder;
using static QRCoder.QRCodeGenerator;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Linq;

namespace Remote_Control_PC
{
    public partial class QRCodeForm : Form
    {
        private const string CHECKWORD = "RCSync";
        private const int PICTUREBOX_OFFSETS = 10;
        private const int FORM_ADDITIONAL_HEIGHT = 100;
        private const int FORM_OFFSET_X = 20;
        private const int FORM_OFFSET_Y = 50;

        private int port;
        private AddressEntry[] addresses;

        private void GenerateQRCode(IPAddress ip, int port)
        {
            QRCodeGenerator generator = new QRCodeGenerator();
            QRCode qr = generator.CreateQrCode($"{CHECKWORD}@{ip}:{port}", ECCLevel.L);
            Bitmap bitmap = qr.GetGraphic(pixelsPerModule: 7);
            qrPictureBox.Width = bitmap.Width;
            qrPictureBox.Height = bitmap.Height;
            this.Height = qrPictureBox.Height + 2 * PICTUREBOX_OFFSETS + FORM_ADDITIONAL_HEIGHT;
            this.Location = new Point(Screen.PrimaryScreen.Bounds.Width - this.Width - FORM_OFFSET_X,
                                      Screen.PrimaryScreen.Bounds.Height - this.Height - FORM_OFFSET_Y);
            qrPictureBox.BackgroundImage = bitmap;
        }

        public QRCodeForm(int port)
        {
            this.port = port;
            addresses = GetLocalAddresses().ToArray();

            InitializeComponent();
            addressComboBox.Items.AddRange(addresses);
            addressComboBox.SelectedIndex = 0;
            GenerateQRForSelectedAddress();
            Visible = true;
        }

        private void GenerateQRForSelectedAddress()
        {
            GenerateQRCode((addressComboBox.SelectedItem as AddressEntry).IP, port);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private static IEnumerable<AddressEntry> GetLocalAddresses()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }
                foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                {
                    if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        yield return new AddressEntry(ip.Address, ni.Name);
                    }
                }
            }
        }

        private class AddressEntry
        {
            public readonly IPAddress IP;
            public readonly string Name;
            public AddressEntry(IPAddress IP, string Name)
            {
                this.IP = IP;
                this.Name = Name;
            }

            public override string ToString()
            {
                return $"{IP} ({Name})";
            }
        }

        private void AddressComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            GenerateQRForSelectedAddress();
        }
    }
}
