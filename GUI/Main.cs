using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV.Aruco;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Gesture_Recognition_App.GesturesSettings;

namespace Gesture_Recognition_App
{
    public partial class Main : Form
    {
        private Statistics statistics;

        private Process process;

        private static readonly HttpClient client = new HttpClient();

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap currentFrame;
        private System.Windows.Forms.Timer sendFrameTimer;

        private DateTime lastActionTime = DateTime.MinValue;
        private TimeSpan actionCooldown = TimeSpan.FromMilliseconds(Properties.Settings.Default.ActionInterval);

        private Dictionary<string, GestureAction> gestureActions;

        public Main()
        {
            InitializeComponent();
            StartServer();
            statistics = new Statistics();
            LoadSettings();
            InitializeTimer();
            LoadVideoDevices();
        }

        private void StartServer()
        {
            string pythonPath = Path.GetFullPath(@"..\..\..\.venv\Scripts\python.exe"); // VENV


            // УДАЛИТЬ!!!
            //string pythonPath = Path.GetFullPath(@"D:\Python38\python.exe"); // без виртуальной среды
            // УДАЛИТЬ!!!

            string scriptPath = Path.GetFullPath(@"..\..\..\gesture_recognition.py");

            Console.WriteLine($"Полный путь к скрипту: {scriptPath}");


            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonPath;
            start.WorkingDirectory = Path.GetDirectoryName(scriptPath);
            start.Arguments = '"' + scriptPath + '"';
            start.UseShellExecute = true;
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;

            process = Process.Start(start);

            /*using(
                process = Process.Start(start)
            )
            {
                using(var reader = process.StandardOutput)
                {
                    String result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }

                using(var reader = process.StandardError)
                {
                    String result = reader.ReadToEnd();
                    Console.WriteLine(result);
                }

                process.WaitForExit();
            }*/
        }

        private void LoadSettings()
        {
            if (File.Exists("settings.json"))
            {
                try
                {
                    var json = File.ReadAllText("settings.json");

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        MessageBox.Show("Файл настроек пуст. Создается новый файл.");
                        DeleteSettingsFile();
                        gestureActions = new Dictionary<string, GestureAction>();
                    }
                    else
                    {
                        gestureActions = JsonSerializer.Deserialize<Dictionary<string, GestureAction>>(json);

                        if (gestureActions == null)
                        {
                            throw new Exception("Не удалось десериализовать настройки. Данные повреждены.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке настроек:\n{ex.Message}.\nСтарый файл будет удален, и будет создан новый.");
                    DeleteSettingsFile();
                    gestureActions = new Dictionary<string, GestureAction>();
                }
            }
            else
            {
                gestureActions = new Dictionary<string, GestureAction>();
            }

            ApplySettings();
        }

        private void DeleteSettingsFile()
        {
            try
            {
                File.Delete("settings.json");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось удалить старый файл настроек: {ex.Message}");
            }

            CreateEmptySettingsFile();
        }

        private void CreateEmptySettingsFile()
        {
            try
            {
                var emptySettings = new Dictionary<string, GestureAction>();
                var json = JsonSerializer.Serialize(emptySettings);
                File.WriteAllText("settings.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось создать новый файл настроек: {ex.Message}");
            }
        }


        private void ApplySettings()
        {
            foreach (var gesture in gestureActions)
            {
                string gestureName = gesture.Key;
                int actionCode = gesture.Value.ActionCode;
            }
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
                await SendFrameToServer(base64Image);
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

        private async Task SendFrameToServer(string base64Image)
        {

            statistics.IncrementFrameCount();

            var json = $"{{\"frame\":\"{base64Image}\"}}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                string url = Properties.Settings.Default.ServerAddress;
                string port = Properties.Settings.Default.ServerPort.ToString();

                var stopwatch = Stopwatch.StartNew();

                HttpResponseMessage response = await client.PostAsync("http://" + url + ":" + port + "/classify_frame", content);

                stopwatch.Stop();
                double processingTime = stopwatch.Elapsed.TotalMilliseconds;
                statistics.UpdateProcessingTime(processingTime);


                string result = await response.Content.ReadAsStringAsync();

                string gesture = null;

                if (result != null)
                {
                    StatusLabel.Text = "Статус подключения к серверу: Подключен.";
                    gesture = ParseGestureFromJson(result);
                }

                if (gesture != null)
                {
                    HandleGesture(gesture);
                }
                else
                {
                    Console.WriteLine("Ошибка парсинга. Result == null?");
                }

            }
            catch (HttpRequestException)
            {
                StatusLabel.Text = "Статус подключения к серверу: ОШИБКА СОЕДИНЕНИЯ";
                InfoLabel.Text = "";
            }
            catch (Exception)
            {

            }
        }

        private string ParseGestureFromJson(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    if (doc.RootElement.TryGetProperty("gesture", out JsonElement gestureElement))
                    {
                        return gestureElement.GetString();
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при парсинге JSON: {ex.Message}");
                return null;
            }
        }

        private void HandleGesture(string gesture)
        {
            if (gesture.Equals("No gesture detected"))
            {
                return;
            }
            statistics.IncrementRecognitionCount();

            if (DateTime.Now - lastActionTime < actionCooldown)
            {
                Console.WriteLine("Слишком быстро. Повторное действие будет проигнорировано.");
                return;
            }

            lastActionTime = DateTime.Now;

            if (gestureActions.ContainsKey(gesture))
            {
                statistics.IncrementActionsCount();

                var actionCode = gestureActions[gesture].ActionCode;

                switch (actionCode)
                {
                    case 0:
                        Console.WriteLine($"Жест '{gesture}' не имеет действия.");
                        break;
                    case 1:
                        Console.WriteLine($"Жест '{gesture}' выполняет действие: Скриншот.");
                        TakeScreenshot();
                        break;
                    case 2:
                        Console.WriteLine($"Жест '{gesture}' выполняет действие: Громкость -.");
                        VolumeDown();
                        break;
                    case 3:
                        Console.WriteLine($"Жест '{gesture}' выполняет действие: Громкость +.");
                        VolumeUp();
                        break;
                    case 4:
                        Console.WriteLine($"Жест '{gesture}' выполняет действие: Снимок с камеры.");
                        TakePicture();
                        break;
                    case 5:
                        Console.WriteLine($"Жест '{gesture}' выполняет действие: Запуск программы.");

                        if (!string.IsNullOrEmpty(gestureActions[gesture].FilePath))
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(gestureActions[gesture].FilePath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Не удалось запустить программу для жеста '{gesture}': {ex.Message}");
                            }
                        }
                        break;
                    default:
                        Console.WriteLine($"Жест '{gesture}' имеет неизвестный код действия: {actionCode}");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"Настройки для жеста '{gesture}' не найдены.");
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

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            currentFrame = (Bitmap)eventArgs.Frame.Clone();
            PreviewImgBox.Image = currentFrame;
        }

        private void настройкиПодключенияКСерверуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerSettings serverSettings = new ServerSettings();
            serverSettings.Show();
        }

        private void периодичностьОтправкиФреймовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrameCooldownSettings frameCooldownSettings = new FrameCooldownSettings();
            frameCooldownSettings.FormClosed += UpdateFrameCooldown;
            frameCooldownSettings.Show();
        }

        private void UpdateFrameCooldown(object sender, EventArgs e)
        {
            sendFrameTimer.Interval = Properties.Settings.Default.FrameInterval;
        }

        private void настройкиЖестовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GesturesSettings gesturesSettings = new GesturesSettings();
            gesturesSettings.FormClosed += GestureSettingsClosed;
            gesturesSettings.ShowDialog();
        }

        private void GestureSettingsClosed(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void TakeScreenshot()
        {
            try
            {
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;

                using (Bitmap screenshot = new Bitmap(screenBounds.Width, screenBounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(screenshot))
                    {
                        g.CopyFromScreen(screenBounds.Location, Point.Empty, screenBounds.Size);
                    }

                    string savePath = Properties.Settings.Default.ScreenshotPath;
                    if (string.IsNullOrEmpty(savePath))
                    {
                        savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    }

                    string fileName = System.IO.Path.Combine(savePath, $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                    screenshot.Save(fileName, ImageFormat.Png);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void VolumeDown()
        {
            // Нужно подключать Windows API (CoreAudio)
        }

        private void VolumeUp()
        {
            // Нужно подключать Windows API (CoreAudio)
        }

        private void TakePicture()
        {
            try
            {
                if (currentFrame == null)
                {
                    MessageBox.Show("Нет доступного кадра для сохранения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string savePath = Properties.Settings.Default.ScreenshotPath;

                if (string.IsNullOrEmpty(savePath))
                {
                    savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                }

                Directory.CreateDirectory(savePath);

                string fileName = Path.Combine(savePath, $"Snapshot_{DateTime.Now:yyyyMMdd_HHmmss}.jpg");

                lock (currentFrame)
                {
                    currentFrame.Save(fileName, ImageFormat.Jpeg);
                }

                Console.WriteLine("Снимок сохранен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении снимка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void путьСохраненияСкриншотовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Выберите папку для сохранения скриншотов";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.ScreenshotPath = folderDialog.SelectedPath;
                    Properties.Settings.Default.Save();
                    MessageBox.Show("Путь сохранён: " + folderDialog.SelectedPath);
                }
            }
        }

        private void cooldownДействийToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActionCooldownSettings actionCooldownSettings = new ActionCooldownSettings();
            actionCooldownSettings.FormClosed += UpdateActionCooldown;
            actionCooldownSettings.Show();
        }

        private void UpdateActionCooldown(object sender, EventArgs e)
        {
            actionCooldown = TimeSpan.FromMilliseconds(Properties.Settings.Default.ActionInterval);
        }

        private void режимДобавленияЖестовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddingGestures addingGestures = new AddingGestures();
            addingGestures.Show();
        }

        private void статистикаToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            statistics.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
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

            if (sendFrameTimer != null)
            {
                sendFrameTimer.Stop();
                sendFrameTimer.Dispose();
                sendFrameTimer = null;
            }
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.FormClosed -= Main_FormClosed;
            this.Dispose();
        }
    }

}
