using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV.Flann;
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

        private System.Windows.Forms.Timer sendFrameTimer;
        private static readonly HttpClient client = new HttpClient();

        private string filePath = Path.GetFullPath(@"..\..\..\model\points_classifier\classifier_labels.csv");
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap currentFrame;

        public AddingGestures()
        {
            InitializeComponent();
            InitializeTimer();
            LoadVideoDevices();
        }

        private void InitializeTimer()
        {
            if (sendFrameTimer == null)
            {
                sendFrameTimer = new System.Windows.Forms.Timer();
            }

            sendFrameTimer.Interval = Properties.Settings.Default.FrameInterval;
            sendFrameTimer.Tick += async (s, e) => await SendCurrentFrame();
        }

        private async Task SendCurrentFrame()
        {
            if (sendFrameTimer != null && currentFrame != null)
            {
                string base64Image = ConvertFrameToBase64(currentFrame);
                await SendUnsavingFrameToServer(base64Image);
            }
        }

        private async Task SendUnsavingFrameToServer(string base64Image)
        {
            int index = 99;

            bool save = false;

            var json = JsonSerializer.Serialize(new
            {
                frame = base64Image,
                number = index,
                save = save
            });
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                string url = Properties.Settings.Default.ServerAddress;
                string port = Properties.Settings.Default.ServerPort.ToString();

                var stopwatch = Stopwatch.StartNew();

                HttpResponseMessage response = await client.PostAsync("http://" + url + ":" + port + "/log_dataset", content);

                stopwatch.Stop();

                double processingTime = stopwatch.Elapsed.TotalMilliseconds;

                string result = await response.Content.ReadAsStringAsync();

                string status = null;

                if (result != null)
                {
                    status = ParseResponseFromJson(result);
                }

                if (status != null)
                {
                    if (status.Equals("not_saved"))
                    {
                        currentStatus.Text = $"Жест не распознан.";
                    }
                    else if (status.Equals("processed"))
                    {
                        currentStatus.Text = "Жест распознан и готов к сохранению";
                    }
                    else
                    {
                        currentStatus.Text = "Неизвестный статус. Проверьте подключение к серверу.";
                    }
                }
            }
            catch (HttpRequestException)
            {
                
            }
            catch (Exception)
            {

            }
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
            sendFrameTimer.Start();
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs e)
        {
            currentFrame = (Bitmap)e.Frame.Clone();
            PreviewImgBox.Image = currentFrame;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            await SendSavingFrame();
        }

        private async Task SendSavingFrame()
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
                await SendSavingFrameToServer(name, index, base64Frame);
            }
        }

        private async Task SendSavingFrameToServer(string name, int index, string frame)
        {
            bool save = true;
                var json = JsonSerializer.Serialize(new
                {
                    frame = frame,
                    number = index,
                    save = save
                });
                Console.WriteLine (json);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                try
                {
                    string url = Properties.Settings.Default.ServerAddress;
                    string port = Properties.Settings.Default.ServerPort.ToString();
                    HttpResponseMessage response = await client.PostAsync("http://" + url + ":" + port + "/log_dataset", content);
                    string result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(result);
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
                if (!process.HasExited)
                {
                    process.Kill();
                }
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
