namespace Gesture_Recognition_App
{
    partial class AddingGestures
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddingGestures));
            this.DevicesLabel = new System.Windows.Forms.Label();
            this.comboBoxDevices = new System.Windows.Forms.ComboBox();
            this.PreviewImgBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.GestureName = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.PreviewImgBox)).BeginInit();
            this.SuspendLayout();
            // 
            // DevicesLabel
            // 
            this.DevicesLabel.AutoSize = true;
            this.DevicesLabel.Location = new System.Drawing.Point(12, 17);
            this.DevicesLabel.Margin = new System.Windows.Forms.Padding(3, 8, 3, 0);
            this.DevicesLabel.Name = "DevicesLabel";
            this.DevicesLabel.Size = new System.Drawing.Size(190, 13);
            this.DevicesLabel.TabIndex = 8;
            this.DevicesLabel.Text = "Устройство входного видеопотока: ";
            // 
            // comboBoxDevices
            // 
            this.comboBoxDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxDevices.FormattingEnabled = true;
            this.comboBoxDevices.Location = new System.Drawing.Point(208, 14);
            this.comboBoxDevices.Name = "comboBoxDevices";
            this.comboBoxDevices.Size = new System.Drawing.Size(264, 21);
            this.comboBoxDevices.TabIndex = 7;
            this.comboBoxDevices.SelectedIndexChanged += new System.EventHandler(this.comboBoxDevices_SelectedIndexChanged);
            // 
            // PreviewImgBox
            // 
            this.PreviewImgBox.Location = new System.Drawing.Point(12, 41);
            this.PreviewImgBox.Name = "PreviewImgBox";
            this.PreviewImgBox.Size = new System.Drawing.Size(854, 480);
            this.PreviewImgBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PreviewImgBox.TabIndex = 6;
            this.PreviewImgBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 524);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Название жеста:";
            // 
            // GestureName
            // 
            this.GestureName.Location = new System.Drawing.Point(15, 540);
            this.GestureName.Name = "GestureName";
            this.GestureName.Size = new System.Drawing.Size(399, 20);
            this.GestureName.TabIndex = 10;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(420, 528);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(446, 39);
            this.button1.TabIndex = 13;
            this.button1.Text = "Отправить фрейм";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(478, 13);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(388, 23);
            this.button2.TabIndex = 14;
            this.button2.Text = "Запустить переобучение модели";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // AddingGestures
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(878, 579);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.GestureName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DevicesLabel);
            this.Controls.Add(this.comboBoxDevices);
            this.Controls.Add(this.PreviewImgBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddingGestures";
            this.Text = "Обучение модели";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AddingGestures_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.PreviewImgBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label DevicesLabel;
        private System.Windows.Forms.ComboBox comboBoxDevices;
        private System.Windows.Forms.PictureBox PreviewImgBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox GestureName;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}