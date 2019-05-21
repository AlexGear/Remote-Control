using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Remote_Control {
    public partial class MainPage {
        private void InitClient() {
            client = new Client();
            client.ConnectedChanged += (s, e) => Dispatcher.BeginInvoke(UpdateControls);
            client.TextReceived += (s, e) => Dispatcher.BeginInvoke(() => TextReceived(e.Value, e.ProcessNumber));
            client.CommandReceived += (s, e) => Dispatcher.BeginInvoke(() => CommandReceived(e.Value, e.ProcessNumber));
        }

        private void InitTimer() {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3.0);
            timer.Tick += (s, e) => { if(!Connected) EstabilishConnection(); };
            timer.Start();
        }

        private void InitPagers() {
            for(byte i = 0; i < COMMAND_LINES_PER_CLIENT; i++) {
                pagers[i] = new Pager(FindName($"pagesListBox{i + 1}") as ListBox);
            }
        }
    }
}
