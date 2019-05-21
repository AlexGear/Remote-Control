using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Remote_Control {
    public class Pager {
        private ListBox pagesListBox;
        private bool IsDarkTheme => (Application.Current.Resources["PhoneBackgroundBrush"] as SolidColorBrush).Color == Colors.Black;

        private TextBlock CreateTextBlock() {
            return new TextBlock() {
                Foreground = IsDarkTheme ? new SolidColorBrush(Color.FromArgb(255, 172, 172, 172)) :
                                           new SolidColorBrush(Color.FromArgb(255, 50, 50, 50)),
                Margin = new Thickness(10, 0, 10, 0),
                FontSize = 16,
                FontFamily = new FontFamily("Lucida Console"),
                TextWrapping = TextWrapping.Wrap
            };
        }

        public Pager(ListBox pagesListBox) {
            this.pagesListBox = pagesListBox;
            pagesListBox.Items.Add(CreateTextBlock());   // first
        }

        public void AddText(string text) {
            string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            int i = 0;
            if(pagesListBox.Items.Count > 0)
                (pagesListBox.Items.Last() as TextBlock).Text += lines[i++];

            for(; i < lines.Length; i++) {
                TextBlock page = CreateTextBlock();
                page.Text = lines[i];
                pagesListBox.Items.Add(page);
            }
        }
    }
}
