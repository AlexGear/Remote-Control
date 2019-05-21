namespace Remote_Control_PC {
    partial class QRCodeForm {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent() {
            this.qrPictureBox = new System.Windows.Forms.PictureBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.addressComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.qrPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // qrPictureBox
            // 
            this.qrPictureBox.BackColor = System.Drawing.Color.White;
            this.qrPictureBox.Location = new System.Drawing.Point(9, 62);
            this.qrPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.qrPictureBox.Name = "qrPictureBox";
            this.qrPictureBox.Size = new System.Drawing.Size(200, 200);
            this.qrPictureBox.TabIndex = 0;
            this.qrPictureBox.TabStop = false;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(229, 237);
            this.closeButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(100, 25);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Закрыть";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // addressComboBox
            // 
            this.addressComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.addressComboBox.FormattingEnabled = true;
            this.addressComboBox.Location = new System.Drawing.Point(9, 32);
            this.addressComboBox.Name = "addressComboBox";
            this.addressComboBox.Size = new System.Drawing.Size(320, 21);
            this.addressComboBox.TabIndex = 2;
            this.addressComboBox.SelectedIndexChanged += new System.EventHandler(this.AddressComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Адрес";
            // 
            // QRCodeForm
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(341, 272);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.addressComboBox);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.qrPictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "QRCodeForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Синхронизация";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Magenta;
            ((System.ComponentModel.ISupportInitialize)(this.qrPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox qrPictureBox;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ComboBox addressComboBox;
        private System.Windows.Forms.Label label1;
    }
}

