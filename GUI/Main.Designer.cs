namespace Gesture_Recognition_App
{
    partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.comboBoxDevices = new System.Windows.Forms.ComboBox();
            this.DevicesLabel = new System.Windows.Forms.Label();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.InfoLabel = new System.Windows.Forms.Label();
            this.Menu = new System.Windows.Forms.MenuStrip();
            this.настройкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиЖестовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.настройкиПодключенияКСерверуToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.периодичностьОтправкиФреймовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.путьСохраненияСкриншотовToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cooldownДействийToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.информацияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.статистикаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.информацияToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.статистикаToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.PreviewImgBox = new System.Windows.Forms.PictureBox();
            this.StatusPicture = new System.Windows.Forms.PictureBox();
            this.Menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewImgBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatusPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxDevices
            // 
            this.comboBoxDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDevices.FormattingEnabled = true;
            this.comboBoxDevices.Location = new System.Drawing.Point(208, 29);
            this.comboBoxDevices.Name = "comboBoxDevices";
            this.comboBoxDevices.Size = new System.Drawing.Size(264, 21);
            this.comboBoxDevices.TabIndex = 4;
            this.comboBoxDevices.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevices_SelectedIndexChanged);
            // 
            // DevicesLabel
            // 
            this.DevicesLabel.AutoSize = true;
            this.DevicesLabel.Location = new System.Drawing.Point(12, 32);
            this.DevicesLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.DevicesLabel.Name = "DevicesLabel";
            this.DevicesLabel.Size = new System.Drawing.Size(190, 13);
            this.DevicesLabel.TabIndex = 5;
            this.DevicesLabel.Text = "Устройство входного видеопотока: ";
            // 
            // StatusLabel
            // 
            this.StatusLabel.AutoSize = true;
            this.StatusLabel.Location = new System.Drawing.Point(41, 548);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(179, 13);
            this.StatusLabel.TabIndex = 6;
            this.StatusLabel.Text = "Статус подключения к серверу: ...";
            // 
            // InfoLabel
            // 
            this.InfoLabel.AutoSize = true;
            this.InfoLabel.Location = new System.Drawing.Point(676, 587);
            this.InfoLabel.Name = "InfoLabel";
            this.InfoLabel.Size = new System.Drawing.Size(0, 13);
            this.InfoLabel.TabIndex = 7;
            // 
            // Menu
            // 
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настройкиToolStripMenuItem,
            this.информацияToolStripMenuItem,
            this.информацияToolStripMenuItem1});
            this.Menu.Location = new System.Drawing.Point(0, 0);
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(881, 24);
            this.Menu.TabIndex = 8;
            this.Menu.Text = "Menu";
            // 
            // настройкиToolStripMenuItem
            // 
            this.настройкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.настройкиЖестовToolStripMenuItem,
            this.настройкиПодключенияКСерверуToolStripMenuItem,
            this.периодичностьОтправкиФреймовToolStripMenuItem,
            this.путьСохраненияСкриншотовToolStripMenuItem,
            this.cooldownДействийToolStripMenuItem});
            this.настройкиToolStripMenuItem.Name = "настройкиToolStripMenuItem";
            this.настройкиToolStripMenuItem.Size = new System.Drawing.Size(79, 20);
            this.настройкиToolStripMenuItem.Text = "Настройки";
            // 
            // настройкиЖестовToolStripMenuItem
            // 
            this.настройкиЖестовToolStripMenuItem.Name = "настройкиЖестовToolStripMenuItem";
            this.настройкиЖестовToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.настройкиЖестовToolStripMenuItem.Text = "Настройки жестов...";
            this.настройкиЖестовToolStripMenuItem.Click += new System.EventHandler(this.настройкиЖестовToolStripMenuItem_Click);
            // 
            // настройкиПодключенияКСерверуToolStripMenuItem
            // 
            this.настройкиПодключенияКСерверуToolStripMenuItem.Name = "настройкиПодключенияКСерверуToolStripMenuItem";
            this.настройкиПодключенияКСерверуToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.настройкиПодключенияКСерверуToolStripMenuItem.Text = "Настройки подключения к серверу";
            this.настройкиПодключенияКСерверуToolStripMenuItem.Click += new System.EventHandler(this.настройкиПодключенияКСерверуToolStripMenuItem_Click);
            // 
            // периодичностьОтправкиФреймовToolStripMenuItem
            // 
            this.периодичностьОтправкиФреймовToolStripMenuItem.Name = "периодичностьОтправкиФреймовToolStripMenuItem";
            this.периодичностьОтправкиФреймовToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.периодичностьОтправкиФреймовToolStripMenuItem.Text = "Периодичность отправки фреймов";
            this.периодичностьОтправкиФреймовToolStripMenuItem.Click += new System.EventHandler(this.периодичностьОтправкиФреймовToolStripMenuItem_Click);
            // 
            // путьСохраненияСкриншотовToolStripMenuItem
            // 
            this.путьСохраненияСкриншотовToolStripMenuItem.Name = "путьСохраненияСкриншотовToolStripMenuItem";
            this.путьСохраненияСкриншотовToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.путьСохраненияСкриншотовToolStripMenuItem.Text = "Путь сохранения скриншотов...";
            this.путьСохраненияСкриншотовToolStripMenuItem.Click += new System.EventHandler(this.путьСохраненияСкриншотовToolStripMenuItem_Click);
            // 
            // cooldownДействийToolStripMenuItem
            // 
            this.cooldownДействийToolStripMenuItem.Name = "cooldownДействийToolStripMenuItem";
            this.cooldownДействийToolStripMenuItem.Size = new System.Drawing.Size(296, 22);
            this.cooldownДействийToolStripMenuItem.Text = "Промежуток повторных распознаваний";
            this.cooldownДействийToolStripMenuItem.Click += new System.EventHandler(this.cooldownДействийToolStripMenuItem_Click);
            // 
            // информацияToolStripMenuItem
            // 
            this.информацияToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.статистикаToolStripMenuItem});
            this.информацияToolStripMenuItem.Name = "информацияToolStripMenuItem";
            this.информацияToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.информацияToolStripMenuItem.Text = "Режимы";
            // 
            // статистикаToolStripMenuItem
            // 
            this.статистикаToolStripMenuItem.Name = "статистикаToolStripMenuItem";
            this.статистикаToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            this.статистикаToolStripMenuItem.Text = "Добавление жестов";
            this.статистикаToolStripMenuItem.Click += new System.EventHandler(this.режимДобавленияЖестовToolStripMenuItem_Click);
            // 
            // информацияToolStripMenuItem1
            // 
            this.информацияToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.статистикаToolStripMenuItem1});
            this.информацияToolStripMenuItem1.Name = "информацияToolStripMenuItem1";
            this.информацияToolStripMenuItem1.Size = new System.Drawing.Size(93, 20);
            this.информацияToolStripMenuItem1.Text = "Информация";
            // 
            // статистикаToolStripMenuItem1
            // 
            this.статистикаToolStripMenuItem1.Name = "статистикаToolStripMenuItem1";
            this.статистикаToolStripMenuItem1.Size = new System.Drawing.Size(135, 22);
            this.статистикаToolStripMenuItem1.Text = "Статистика";
            this.статистикаToolStripMenuItem1.Click += new System.EventHandler(this.статистикаToolStripMenuItem1_Click);
            // 
            // PreviewImgBox
            // 
            this.PreviewImgBox.Location = new System.Drawing.Point(12, 56);
            this.PreviewImgBox.Name = "PreviewImgBox";
            this.PreviewImgBox.Size = new System.Drawing.Size(854, 480);
            this.PreviewImgBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PreviewImgBox.TabIndex = 3;
            this.PreviewImgBox.TabStop = false;
            // 
            // StatusPicture
            // 
            this.StatusPicture.ErrorImage = global::Gesture_Recognition_App.Properties.Resources.error;
            this.StatusPicture.InitialImage = global::Gesture_Recognition_App.Properties.Resources.loading;
            this.StatusPicture.Location = new System.Drawing.Point(12, 542);
            this.StatusPicture.Name = "StatusPicture";
            this.StatusPicture.Size = new System.Drawing.Size(23, 23);
            this.StatusPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.StatusPicture.TabIndex = 9;
            this.StatusPicture.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(881, 576);
            this.Controls.Add(this.StatusPicture);
            this.Controls.Add(this.InfoLabel);
            this.Controls.Add(this.StatusLabel);
            this.Controls.Add(this.DevicesLabel);
            this.Controls.Add(this.comboBoxDevices);
            this.Controls.Add(this.PreviewImgBox);
            this.Controls.Add(this.Menu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.Menu;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "Распознавание жестов";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Menu.ResumeLayout(false);
            this.Menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewImgBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StatusPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox PreviewImgBox;
        private System.Windows.Forms.ComboBox comboBoxDevices;
        private System.Windows.Forms.Label DevicesLabel;
        private System.Windows.Forms.Label StatusLabel;
        private System.Windows.Forms.Label InfoLabel;
        private new System.Windows.Forms.MenuStrip Menu;
        private System.Windows.Forms.ToolStripMenuItem настройкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиЖестовToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem настройкиПодключенияКСерверуToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem информацияToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem статистикаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem периодичностьОтправкиФреймовToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem путьСохраненияСкриншотовToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cooldownДействийToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem информацияToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem статистикаToolStripMenuItem1;
        private System.Windows.Forms.PictureBox StatusPicture;
    }
}

