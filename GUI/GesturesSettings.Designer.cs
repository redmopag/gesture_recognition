namespace Gesture_Recognition_App
{
    partial class GesturesSettings
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
            this.listBoxGesture = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxAction = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.textBoxGesture = new System.Windows.Forms.TextBox();
            this.AddGestureButton = new System.Windows.Forms.Button();
            this.textBoxFilePath = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // listBoxGesture
            // 
            this.listBoxGesture.FormattingEnabled = true;
            this.listBoxGesture.Location = new System.Drawing.Point(12, 29);
            this.listBoxGesture.Name = "listBoxGesture";
            this.listBoxGesture.Size = new System.Drawing.Size(267, 368);
            this.listBoxGesture.TabIndex = 0;
            this.listBoxGesture.SelectedIndexChanged += new System.EventHandler(this.listBoxGestures_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Жесты:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(314, 13);
            this.label2.Margin = new System.Windows.Forms.Padding(32, 0, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Доступные действия:";
            // 
            // comboBoxAction
            // 
            this.comboBoxAction.FormattingEnabled = true;
            this.comboBoxAction.Location = new System.Drawing.Point(317, 30);
            this.comboBoxAction.Name = "comboBoxAction";
            this.comboBoxAction.Size = new System.Drawing.Size(334, 21);
            this.comboBoxAction.TabIndex = 3;
            this.comboBoxAction.SelectedIndexChanged += new System.EventHandler(this.comboBoxAction_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(317, 412);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(334, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Сохранить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(317, 58);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(334, 23);
            this.btnSelectFile.TabIndex = 5;
            this.btnSelectFile.Text = "Выбрать программу для запуска...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Visible = false;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // textBoxGesture
            // 
            this.textBoxGesture.Location = new System.Drawing.Point(12, 412);
            this.textBoxGesture.Name = "textBoxGesture";
            this.textBoxGesture.Size = new System.Drawing.Size(143, 20);
            this.textBoxGesture.TabIndex = 6;
            // 
            // AddGestureButton
            // 
            this.AddGestureButton.Location = new System.Drawing.Point(162, 410);
            this.AddGestureButton.Name = "AddGestureButton";
            this.AddGestureButton.Size = new System.Drawing.Size(117, 23);
            this.AddGestureButton.TabIndex = 7;
            this.AddGestureButton.Text = "Добавить";
            this.AddGestureButton.UseVisualStyleBackColor = true;
            this.AddGestureButton.Click += new System.EventHandler(this.btnAddGesture_Click);
            // 
            // textBoxFilePath
            // 
            this.textBoxFilePath.Enabled = false;
            this.textBoxFilePath.Location = new System.Drawing.Point(317, 88);
            this.textBoxFilePath.Name = "textBoxFilePath";
            this.textBoxFilePath.Size = new System.Drawing.Size(334, 20);
            this.textBoxFilePath.TabIndex = 8;
            this.textBoxFilePath.Visible = false;
            // 
            // GesturesSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 450);
            this.Controls.Add(this.textBoxFilePath);
            this.Controls.Add(this.AddGestureButton);
            this.Controls.Add(this.textBoxGesture);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBoxAction);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBoxGesture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GesturesSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Настройки действий жестов";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxGesture;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBoxAction;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox textBoxGesture;
        private System.Windows.Forms.Button AddGestureButton;
        private System.Windows.Forms.TextBox textBoxFilePath;
    }
}