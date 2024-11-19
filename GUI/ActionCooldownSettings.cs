using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gesture_Recognition_App
{
    public partial class ActionCooldownSettings : Form
    {
        public int ActionInterval
        {
            get;
            private set;
        }

        public ActionCooldownSettings()
        {
            InitializeComponent();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar1.Value;
            ActionInterval = trackBar1.Value;
            Properties.Settings.Default.ActionInterval = ActionInterval;
            Properties.Settings.Default.Save();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int value = Convert.ToInt32(numericUpDown1.Value);
                trackBar1.Value = value;
                ActionInterval = value;
                Properties.Settings.Default.ActionInterval = ActionInterval;
                Properties.Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show("Введите целочисленное значение от 100 до 3000");
            }
        }

        private void ActionCooldownSettings_Load(object sender, EventArgs e)
        {
            ActionInterval = Properties.Settings.Default.ActionInterval;
            trackBar1.Value = ActionInterval;
            numericUpDown1.Value = ActionInterval;
        }
    }
}
