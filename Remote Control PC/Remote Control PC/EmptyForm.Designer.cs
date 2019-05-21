namespace Remote_Control_PC {
    partial class EmptyForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator separator;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmptyForm));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.syncMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            separator = new System.Windows.Forms.ToolStripSeparator();
            this.notifyIconContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // separator
            // 
            separator.Name = "separator";
            separator.Size = new System.Drawing.Size(175, 6);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.notifyIconContextMenu;
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Remote Control";
            this.notifyIcon.Visible = true;
            // 
            // notifyIconContextMenu
            // 
            this.notifyIconContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.syncMenuItem,
            separator,
            this.exitMenuItem});
            this.notifyIconContextMenu.Name = "notifyIconContextMenu";
            this.notifyIconContextMenu.Size = new System.Drawing.Size(179, 54);
            // 
            // syncMenuItem
            // 
            this.syncMenuItem.Name = "syncMenuItem";
            this.syncMenuItem.Size = new System.Drawing.Size(178, 22);
            this.syncMenuItem.Text = "Синхронизировать";
            this.syncMenuItem.Click += new System.EventHandler(this.OnSyncClick);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(178, 22);
            this.exitMenuItem.Text = "Выход";
            this.exitMenuItem.Click += new System.EventHandler(this.OnExitClick);
            // 
            // updateTimer
            // 
            this.updateTimer.Interval = 25;
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // EmptyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(0, 0);
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EmptyForm";
            this.Opacity = 0D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "EmptyForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.EmptyForm_FormClosed);
            this.notifyIconContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip notifyIconContextMenu;
        private System.Windows.Forms.ToolStripMenuItem syncMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.Timer updateTimer;
    }
}