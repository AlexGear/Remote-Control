using Microsoft.Phone.Controls;
using SocketEx;
using System.Net;
using System;
using System.Windows.Threading;
using Microsoft.Phone.Shell;
using System.Windows;
using System.Windows.Navigation;
using Windows.Storage;
using System.Windows.Media;
using System.Windows.Controls;
using System.Linq;

namespace Remote_Control {
    public partial class MainPage : PhoneApplicationPage {
        private const string ADDRESS_VALUE_NAME = "address";
        public const byte COMMAND_LINES_PER_CLIENT = 8;
        private readonly SolidColorBrush NO_CONNECTION_BRUSH = new SolidColorBrush(Colors.White);
        private readonly SolidColorBrush SYNC_REQUIRED_BRUSH = new SolidColorBrush(Colors.Yellow);
        private readonly SolidColorBrush CONNECTED_BRUSH = new SolidColorBrush(Colors.Red);

        private DispatcherTimer timer;
        private IPEndPoint serverAddress;
        private Client client;
        private volatile bool synchronized = false;
        private bool Connected => client?.Connected ?? false;
        private bool first = true;
        private Pager[] pagers = new Pager[COMMAND_LINES_PER_CLIENT];
        private bool[] openedCmds = new bool[COMMAND_LINES_PER_CLIENT];

        private ApplicationBarMenuItem CloseButton => ApplicationBar.MenuItems[1] as ApplicationBarMenuItem;
        private ApplicationBarIconButton AddCmdButton => ApplicationBar.Buttons[0] as ApplicationBarIconButton;
        private ApplicationBarIconButton AbortButton => ApplicationBar.Buttons[1] as ApplicationBarIconButton;

        private void UpdateControls() {
            bool connected = Connected;
            for(byte i = 0; i < COMMAND_LINES_PER_CLIENT; i++) {
                GetInputBox(i).SetValue(IsEnabledProperty, connected && openedCmds[i]);
            }
            AddCmdButton.IsEnabled = Connected && openedCmds.Contains(false);  // if connected & there is not opened yet cmds
            AbortButton.IsEnabled = Connected;
            CloseButton.IsEnabled = Connected;
            Dispatcher.BeginInvoke(UpdateConnectionStatus);
        }

        private void TextReceived(string text, byte number) {
            pagers[number].AddText(text);
            OpenCmd(number);
        }

        private void CommandReceived(byte command, byte number) {
            switch(command) {
                case Client.COMMAND_CLOSE_PROCESS:
                    break;

                case Client.COMMAND_GET_OPENED_CMDS:
                    int result = 0;
                    for(byte i = 0; i < COMMAND_LINES_PER_CLIENT; i++) {
                        result |= Convert.ToByte(openedCmds[i]) << i;
                    }
                    client.SendCommand(Client.COMMAND_GET_OPENED_CMDS, Convert.ToByte(result));
                    break;
            }
        }
        
        private PivotItem GetCmd(byte number) {
            return FindName($"cmd{number + 1}") as PivotItem;
        }

        private ListBox GetPagesListBox(byte number) {
            return FindName($"pagesListBox{number + 1}") as ListBox;
        }
        
        private TextBox GetInputBox(byte number) {
            return FindName($"inputBox{number + 1}") as TextBox;
        }

        private void OpenCmd(byte number) {
            openedCmds[number] = true;
            GetCmd(number).IsEnabled = true;
            UpdateControls();
        }

        private void CloseCmd(byte number) {
            client.CloseProcess(number);
            openedCmds[number] = false;
            GetCmd(number).IsEnabled = false;
            GetPagesListBox(number).Items.Clear();
            UpdateControls();
        }

        public MainPage() {
            InitializeComponent();
            InitClient();
            InitPagers();
            if(LoadAddressFromLocalSettings(out serverAddress)) {
                synchronized = true;
                EstabilishConnection();
            }
            InitTimer();
            UpdateControls();
        }

        private void OnConnected() {
            if(client.EstablishConnection()) {
                if(first) {
                    first = false;
                    Dispatcher.BeginInvoke(() => OpenCmd(0));
                    client.AddProcess(0);
                    client.GetLastStrings();
                }
            }
        }

        private void EstabilishConnection() {
            client.Init(serverAddress, OnConnected);
        }

        private void UpdateConnectionStatus() {
            if(!Connected) {
                if(synchronized) {
                    SystemTray.IsVisible = true;
                    SystemTray.ProgressIndicator = new ProgressIndicator() {
                        IsVisible = true,
                        IsIndeterminate = true,
                        Text = "Попытка установить соединение..."
                    };
                    connectionStatus.Foreground = NO_CONNECTION_BRUSH;
                    connectionStatus.Text = "нет соединения";
                }
                else {
                    connectionStatus.Foreground = SYNC_REQUIRED_BRUSH;
                    connectionStatus.Text = "требуется синхронизация";
                }
            }
            else {
                SystemTray.IsVisible = false;
                SystemTray.ProgressIndicator = null;
                connectionStatus.Foreground = CONNECTED_BRUSH;
                connectionStatus.Text = "соединено";
            }
        }

        private bool LoadAddressFromLocalSettings(out IPEndPoint address) {
            address = null;
            string text = (string)ApplicationData.Current.LocalSettings.Values[ADDRESS_VALUE_NAME];
            if(text != null) {
                try {
                    string[] ipAndPort = text.Split(':');
                    IPAddress ip;
                    int port;
                    if(IPAddress.TryParse(ipAndPort[0], out ip) &&
                       int.TryParse(ipAndPort[1], out port)) {
                        address = new IPEndPoint(ip, port);
                        return true;
                    }
                }
                catch {
                }                
            }
            return false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            if(SynchronizationPage.SynchronizationResult != null) {
                serverAddress = SynchronizationPage.SynchronizationResult;
                ApplicationData.Current.LocalSettings.Values[ADDRESS_VALUE_NAME] = serverAddress.ToString();
                synchronized = true;
            }
            base.OnNavigatedTo(e);
        }

        private void syncButton_Click(object sender, EventArgs e) {
            NavigationService.Navigate(new Uri("/SynchronizationPage.xaml", UriKind.Relative));
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e) {
            var margin = connectionStatus.Margin;
            if(e.Orientation == PageOrientation.PortraitUp) {
                connectionStatus.HorizontalAlignment = HorizontalAlignment.Left;
                margin.Top = 50;
            }
            else if(e.Orientation == PageOrientation.LandscapeLeft ||
                    e.Orientation == PageOrientation.LandscapeRight) {
                connectionStatus.HorizontalAlignment = HorizontalAlignment.Right;
                margin.Top = 20;
            }
            connectionStatus.Margin = margin;
        }        

        private void addCmdButton_Click(object sender, EventArgs e) {
            for(byte i = 0; i < COMMAND_LINES_PER_CLIENT; i++) {
                if(openedCmds[i] == false) {
                    client.AddProcess(i);
                    OpenCmd(i);
                    pivot.SelectedIndex = i;
                    return;
                }
            }
        }

        private void inputBox_TextChanged(object sender, TextChangedEventArgs e) {
            TextBox inputBox = sender as TextBox;
            byte number = byte.Parse(inputBox.Name.Last().ToString());     // last symbol of the name is number
            ListBox pagesListBox = GetPagesListBox(number);
            if(inputBox.Text.Contains("\r")) {
                client.SendText(inputBox.Text.Replace("\r", ""), --number);
                inputBox.Text = string.Empty;
                this.Focus();
                if(pagesListBox.Items.Count > 0)
                    pagesListBox.ScrollIntoView(pagesListBox.Items.Last());
            }
        }

        private void abortButton_Click(object sender, EventArgs e) {
            client.Abort(Convert.ToByte(pivot.SelectedIndex));
        }

        private void closeButton_Click(object sender, EventArgs e) {
            CloseCmd(Convert.ToByte(pivot.SelectedIndex));
        }
    }
}