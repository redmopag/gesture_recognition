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
    public partial class FrameCooldownSettings : Form
    {
        public int FrameInterval
        {
            get;
            private set;
        }

        public FrameCooldownSettings()
        {
            InitializeComponent();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar1.Value;
            FrameInterval = trackBar1.Value;
            Properties.Settings.Default.FrameInterval = FrameInterval;
            Properties.Settings.Default.Save();
        }

        private void numericUpDown1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int value = Convert.ToInt32(numericUpDown1.Value);
                trackBar1.Value = value;
                FrameInterval = value;
                Properties.Settings.Default.FrameInterval = FrameInterval;
                Properties.Settings.Default.Save();
            }
            catch
            {
                MessageBox.Show("Введите целочисленное значение (от 100 до 1000)");
            }
        }

        private void FrameCooldownSettings_Load(object sender, EventArgs e)
        {
            FrameInterval = Properties.Settings.Default.FrameInterval;
            trackBar1.Value = FrameInterval;
            numericUpDown1.Value = FrameInterval;
        }
    }
}
