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
            StatusPicture.Image = Properties.Resources.loading;
            StartServer();
            statistics = new Statistics();
            LoadSettings();
            InitializeTimer();
            LoadVideoDevices();
        }

        private void StartServer()
        {
            string pythonPath = Path.GetFullPath(@"..\..\..\.venv\Scripts\python.exe"); // VENV

            string scriptPath = Path.GetFullPath(@"..\..\..\gesture_recognition.py");

            Console.WriteLine($"������ ���� � �������: {scriptPath}");

            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = pythonPath;
            start.WorkingDirectory = Path.GetDirectoryName(scriptPath);
            start.Arguments = '"' + scriptPath + '"';
            start.UseShellExecute = true;
            start.RedirectStandardOutput = false;
            start.RedirectStandardError = false;

            try
            {
                process = Process.Start(start);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"�� ������� ��������� Python-������.\n{ex.Message}");
            }
        }
        private void StopServer()
        {
            try
            {
                if (process != null && !process.HasExited)
                {
                    process.Kill();
                    process.WaitForExit();
                    Console.WriteLine("Python-������ ��� ����������.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ ��� ���������� Python-�������: {ex.Message}");
            }
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
                        MessageBox.Show("���� �������� ����. ��������� ����� ����.");
                        DeleteSettingsFile();
                        gestureActions = new Dictionary<string, GestureAction>();
                    }
                    else
                    {
                        gestureActions = JsonSerializer.Deserialize<Dictionary<string, GestureAction>>(json);

                        if (gestureActions == null)
                        {
                            throw new Exception("�� ������� ��������������� ���������. ������ ����������.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������ ��� �������� ��������:\n{ex.Message}.\n������ ���� ����� ������, � ����� ������ �����.");
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
                MessageBox.Show($"�� ������� ������� ������ ���� ��������: {ex.Message}");
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
                MessageBox.Show($"�� ������� ������� ����� ���� ��������: {ex.Message}");
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
            Bitmap frameCopy = null;

            lock (this)
            {
                if (currentFrame != null)
                {
                    frameCopy = (Bitmap)currentFrame.Clone(); // ������� ����� ��� ����������� �������
                }
            }

            if (frameCopy != null)
            {
                try
                {
                    string base64Image = ConvertFrameToBase64(frameCopy);
                    await SendFrameToServer(base64Image);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"������ �������� ������: {ex.Message}");
                }
                finally
                {
                    frameCopy.Dispose(); // ����������� ����� ����� �������������
                }
            }
        }


        private string ConvertFrameToBase64(Bitmap frame)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                frame.Save(ms, ImageFormat.Jpeg); // ����������� � JPEG
                return Convert.ToBase64String(ms.ToArray());
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
                    StatusLabel.Text = "������ ����������� � �������: ���������.";
                    StatusPicture.Image = Properties.Resources.OK;
                    gesture = ParseGestureFromJson(result);
                }

                if (gesture != null)
                {
                    HandleGesture(gesture);
                }
                else
                {
                    Console.WriteLine("������ ��������. Result == null?");
                }

            }
            catch (HttpRequestException)
            {
                StatusLabel.Text = "������ ����������� � �������: ������ ����������";
                StatusPicture.Image = Properties.Resources.error;
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
                Console.WriteLine($"������ ��� �������� JSON: {ex.Message}");
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
                Console.WriteLine("������� ������. ��������� �������� ����� ���������������.");
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
                        Console.WriteLine($"���� '{gesture}' �� ����� ��������.");
                        break;
                    case 1:
                        Console.WriteLine($"���� '{gesture}' ��������� ��������: ��������.");
                        TakeScreenshot();
                        break;
                    case 2:
                        Console.WriteLine($"���� '{gesture}' ��������� ��������: ��������� -.");
                        VolumeDown();
                        break;
                    case 3:
                        Console.WriteLine($"���� '{gesture}' ��������� ��������: ��������� +.");
                        VolumeUp();
                        break;
                    case 4:
                        Console.WriteLine($"���� '{gesture}' ��������� ��������: ������ � ������.");
                        TakePicture();
                        break;
                    case 5:
                        Console.WriteLine($"���� '{gesture}' ��������� ��������: ������ ���������.");

                        if (!string.IsNullOrEmpty(gestureActions[gesture].FilePath))
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(gestureActions[gesture].FilePath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"�� ������� ��������� ��������� ��� ����� '{gesture}': {ex.Message}");
                            }
                        }
                        break;
                    default:
                        Console.WriteLine($"���� '{gesture}' ����� ����������� ��� ��������: {actionCode}");
                        break;
                }
            }
            else
            {
                Console.WriteLine($"��������� ��� ����� '{gesture}' �� �������.");
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
                MessageBox.Show("��������������� �� �������.");
        }

        private void comboBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine($"������� ������ ���������� {comboBoxDevices.SelectedIndex}");
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
            try
            {
                Bitmap newFrame = (Bitmap)eventArgs.Frame.Clone();

                lock (this)
                {
                    currentFrame?.Dispose();
                    currentFrame = newFrame;
                }

                if (PreviewImgBox.Image != null)
                {
                    PreviewImgBox.Image.Dispose();
                }
                PreviewImgBox.Image = (Bitmap)newFrame.Clone();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"������ ��������� ������ �����: {ex.Message}");
            }
        }



        private void ����������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ServerSettings serverSettings = new ServerSettings();
            serverSettings.Show();
        }

        private void ����������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrameCooldownSettings frameCooldownSettings = new FrameCooldownSettings();
            frameCooldownSettings.FormClosed += UpdateFrameCooldown;
            frameCooldownSettings.Show();
        }

        private void UpdateFrameCooldown(object sender, EventArgs e)
        {
            sendFrameTimer.Interval = Properties.Settings.Default.FrameInterval;
        }

        private void ���������������ToolStripMenuItem_Click(object sender, EventArgs e)
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
                MessageBox.Show($"������: {ex.Message}");
            }
        }

        private void VolumeDown()
        {
            // ����� ���������� Windows API (CoreAudio)
        }

        private void VolumeUp()
        {
            // ����� ���������� Windows API (CoreAudio)
        }

        private void TakePicture()
        {
            try
            {
                if (currentFrame == null)
                {
                    MessageBox.Show("��� ���������� ����� ��� ����������.", "������", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                Console.WriteLine("������ ��������!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"������ ��� ���������� ������: {ex.Message}", "������", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ������������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "�������� ����� ��� ���������� ����������";
                folderDialog.ShowNewFolderButton = true;

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    Properties.Settings.Default.ScreenshotPath = folderDialog.SelectedPath;
                    Properties.Settings.Default.Save();
                    MessageBox.Show("���� �������: " + folderDialog.SelectedPath);
                }
            }
        }

        private void cooldown��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ActionCooldownSettings actionCooldownSettings = new ActionCooldownSettings();
            actionCooldownSettings.FormClosed += UpdateActionCooldown;
            actionCooldownSettings.Show();
        }

        private void UpdateActionCooldown(object sender, EventArgs e)
        {
            actionCooldown = TimeSpan.FromMilliseconds(Properties.Settings.Default.ActionInterval);
        }

        private void ���������������������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopServer();
            StopVideoCapture();

            AddingGestures addingGestures = new AddingGestures();

            addingGestures.FormClosed += (s, args) =>
            {
                StartServer();
                StartVideoCapture();
            };

            addingGestures.Show();
        }

        private void StopVideoCapture()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                sendFrameTimer?.Stop();
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource = null;
            }

            currentFrame = null;
            if (PreviewImgBox.Image != null)
            {
                PreviewImgBox.Image.Dispose();
                PreviewImgBox.Image = null;
            }
        }

        private void StartVideoCapture()
        {
            if (comboBoxDevices.SelectedIndex >= 0)
            {
                videoSource = new VideoCaptureDevice(videoDevices[comboBoxDevices.SelectedIndex].MonikerString);
                videoSource.NewFrame += Video_NewFrame;
                videoSource.Start();
            }

            sendFrameTimer?.Start();
        }




        private void ����������ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            statistics.Show();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (process != null && !process.HasExited)
            {
                process.Kill();
                process.Dispose();
            }

            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource = null;
            }

            sendFrameTimer?.Stop();
            sendFrameTimer?.Dispose();
            currentFrame?.Dispose();
            PreviewImgBox.Image?.Dispose();
            client.Dispose();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.FormClosed -= Main_FormClosed;
            this.Dispose();
        }
    }

}
