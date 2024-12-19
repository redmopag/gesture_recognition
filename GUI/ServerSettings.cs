using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Gesture_Recognition_App
{
    public partial class ServerSettings : Form
    {
        public ServerSettings()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string urlText = textBox1.Text;
            string portText = textBox2.Text;

            if (string.IsNullOrWhiteSpace(urlText))
            {
                MessageBox.Show("Укажите правильный URL");
                return;
            }
            if (!string.IsNullOrEmpty(portText))
            {
                if (!int.TryParse(portText, out int port) || port < 1 || port > 65535)
                {
                    MessageBox.Show("Укажите правильный порт (1-65535)");
                    return;
                }
            }

            pictureBox1.Image = Properties.Resources.loading;
            label1.Text = "Запрос отправлен...";

            try
            {
                using (Ping ping = new Ping())
                {
                    PingReply reply = await ping.SendPingAsync(urlText, 3000);
                    if (reply.Status == IPStatus.Success)
                    {
                        pictureBox1.Image = Properties.Resources.OK;
                        label1.Text = $"Ping: {reply.RoundtripTime} мс.";
                    }
                    else
                    {
                        pictureBox1.Image = Properties.Resources.error;
                        label1.Text = $"Ping: {reply.Status}";
                    }
                }
            }
            catch (Exception ex)
            {
                label1.Text = $"Ошибка при выполнении ping: {ex.Message}";
                pictureBox1.Image = Properties.Resources.error;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string urlText = textBox1.Text;
            string portText = textBox2.Text;

            if (string.IsNullOrWhiteSpace(urlText))
            {
                MessageBox.Show("Укажите правильный URL");
                return;
            }
            if (!string.IsNullOrEmpty(portText))
            {
                if (!int.TryParse(portText, out int port) || port < 1 || port > 65535)
                {
                    MessageBox.Show("Укажите правильный порт (1-65535)");
                    return;
                }
            }

            Properties.Settings.Default.ServerAddress = urlText;
            Properties.Settings.Default.ServerPort = int.Parse(portText);
            Properties.Settings.Default.Save();
            this.Close();
        }

        private void ServerSettings_Load(object sender, EventArgs e)
        {
            textBox1.Text = Properties.Settings.Default.ServerAddress;
            textBox2.Text = Properties.Settings.Default.ServerPort.ToString();
        }
    }
}
