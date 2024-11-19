using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Gesture_Recognition_App
{
    public partial class AddingGestures : Form
    {
        private Process process;

        private string filePath = Path.GetFullPath(@"..\..\..\model\points_classifier\classifier_labels.csv");
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap currentFrame;

        public AddingGestures()
        {
            InitializeComponent();
            LoadVideoDevices();
        }

        private void LoadVideoDevices()
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in videoDevices)
            {
                comboBoxDevices.Items.Add(device.Name);
            }

            if (comboBoxDevices.Items.Count > 0)
                comboBoxDevices.SelectedIndex = 0;
            else
                MessageBox.Show("Видеоустройства не найдены.");
        }

        private void comboBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine($"Выбрано другое устройство {comboBoxDevices.SelectedIndex}");

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }

            videoSource = new VideoCaptureDevice(videoDevices[comboBoxDevices.SelectedIndex].MonikerString);
            videoSource.NewFrame += Video_NewFrame;
            videoSource.Start();
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs e)
        {
            currentFrame = (Bitmap)e.Frame.Clone();
            PreviewImgBox.Image = currentFrame;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await SendFrame();
        }

        private async Task SendFrame()
        {
            int index = 0;

            string name = GestureName.Text;

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Введите название жеста.");
                return;
            }
            if (currentFrame == null)
            {
                MessageBox.Show("Пустой фрейм.");
                return;
            }
            else
            {
                var existingLines = File.ReadAllLines(filePath);

                int lineIndex = Array.FindIndex(existingLines, line => line == name);

                if (lineIndex >= 0)
                {
                    Console.WriteLine($"Имя найдено в индексе: {lineIndex}");
                    index = lineIndex;
                }
                else
                {
                    index = File.ReadAllLines(filePath).Length;
                    Console.WriteLine($"Новый индекс: {index}");
                }


                string base64Frame = ConvertFrameToBase64(currentFrame);
                await SendFrameToServer(name, index, base64Frame);
            }
        }

        private async Task SendFrameToServer(string name, int index, string frame)
        {
            using (HttpClient client = new HttpClient())
            {
                var json = $"{{\"frame\":\"{frame}\",\"number\":{index}}}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    string url = Properties.Settings.Default.ServerAddress;
                    string port = Properties.Settings.Default.ServerPort.ToString();
                    HttpResponseMessage response = await client.PostAsync("http://" + url + ":" + port + "/log_dataset", content);
                    string result = await response.Content.ReadAsStringAsync();

                    string status = null;

                    if (result != null)
                    {
                        status = ParseResponseFromJson(result);
                    }

                    if (status.Equals("saved"))
                    {
                        MessageBox.Show($"Жест {name} распознан и добавлен в модель с индексом {index}.");
                        var existingLines = File.ReadAllLines(filePath);

                        int lineIndex = Array.FindIndex(existingLines, line => line == name);
                        if (lineIndex >= 0)
                        {
                            Console.WriteLine($"Жест уже существует в модели");
                        }
                        else
                        {
                            File.AppendAllText(filePath, name + Environment.NewLine);
                        }
                    }
                    else if (status.Equals("Frame or number not provided"))
                    {
                        MessageBox.Show("Не передано название или индекс жеста");
                    }
                    else if (status.Equals("Invalid number provided"))
                    {
                        MessageBox.Show("Передан некорректный индекс");
                    }
                    else if (status.Equals("not_saved"))
                    {
                        MessageBox.Show($"Жест на фрейме не распознан.");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка парсинга. Result == null?");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка отправки запроса:\n{ex.Message}");
                }
            }
        }

        private string ParseResponseFromJson(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("status", out JsonElement jsonElement))
                    {
                        return jsonElement.GetString();
                    }
                    if (doc.RootElement.TryGetProperty("error", out JsonElement errorElement))
                    {
                        return errorElement.GetString();
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка парсинга JSON:\n{ex.Message}");
                return null;
            }
        }

        private string ConvertFrameToBase64(Bitmap frame)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                frame.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] data = ms.ToArray();
                return Convert.ToBase64String(data);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StartScript();
        }

        private void StartScript()
        {
            int num_classes = File.ReadAllText(filePath).Length;

            string argument = $"--num_classes {num_classes}";

            string pythonPath = Path.GetFullPath(@"..\..\..\.venv\Scripts\python.exe");

            // УДАЛИТЬ!!!
            //string pythonPath = Path.GetFullPath(@"C:\Users\rusla\AppData\Local\Programs\Python\Python38\python.exe");
            // УДАЛИТЬ!!!

            string scriptPath = Path.GetFullPath(@"..\..\..\points_classifier_training.py");

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonPath;
            start.WorkingDirectory = Path.GetDirectoryName(scriptPath);
            start.Arguments = $"\"{scriptPath}\" {argument}";
            start.UseShellExecute = true;
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;

            process = Process.Start(start);
            process.WaitForExit();
        }

        private void AddingGestures_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (process != null)
            {
                process.Kill();
                process = null;
            }

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource = null;
            }
        }
    }
}
