﻿#pragma checksum "D:\Programs\Remote Control\Remote Control\Remote Control\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "36E80A545D678433061C39E13163C615"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Remote_Control {
    
    
    public partial class MainPage : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal Microsoft.Phone.Shell.ApplicationBarMenuItem syncButton;
        
        internal Microsoft.Phone.Shell.ApplicationBarIconButton addCmdButton;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBlock title;
        
        internal System.Windows.Controls.TextBlock connectionStatus;
        
        internal Microsoft.Phone.Controls.Pivot pivot;
        
        internal Microsoft.Phone.Controls.PivotItem cmd1;
        
        internal System.Windows.Controls.TextBlock outputBox;
        
        internal System.Windows.Controls.TextBox inputBox;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Remote%20Control;component/MainPage.xaml", System.UriKind.Relative));
            this.syncButton = ((Microsoft.Phone.Shell.ApplicationBarMenuItem)(this.FindName("syncButton")));
            this.addCmdButton = ((Microsoft.Phone.Shell.ApplicationBarIconButton)(this.FindName("addCmdButton")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.title = ((System.Windows.Controls.TextBlock)(this.FindName("title")));
            this.connectionStatus = ((System.Windows.Controls.TextBlock)(this.FindName("connectionStatus")));
            this.pivot = ((Microsoft.Phone.Controls.Pivot)(this.FindName("pivot")));
            this.cmd1 = ((Microsoft.Phone.Controls.PivotItem)(this.FindName("cmd1")));
            this.outputBox = ((System.Windows.Controls.TextBlock)(this.FindName("outputBox")));
            this.inputBox = ((System.Windows.Controls.TextBox)(this.FindName("inputBox")));
        }
    }
}

