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
    public partial class Statistics : Form
    {
        public int FrameCount { get; set; }
        public int RecognitionCount { get; set; }
        public int ActionsCount { get; set; }
        public double AverageProcessingTime { get; set; }

        private Timer updateTimer;

        public Statistics()
        {
            InitializeComponent();
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            updateTimer = new Timer();
            updateTimer.Interval = 100;
            updateTimer.Tick += UpdateStatistics;
            updateTimer.Start();
        }

        private void UpdateStatistics(object sender, EventArgs e)
        {
            label1.Text = $"Отправлено фреймов: {FrameCount}";
            label2.Text = $"Распознано жестов: {RecognitionCount}";
            label4.Text = $"Выполнено действий: {ActionsCount}";
            label3.Text = $"Среднее время обработки: {AverageProcessingTime:F2} мс.";
        }

        public void IncrementFrameCount()
        {
            FrameCount++;
        }

        public void IncrementRecognitionCount()
        {
            RecognitionCount++;
        }

        public void IncrementActionsCount()
        {
            ActionsCount++;
        }

        public void UpdateProcessingTime(double timeMs)
        {
            AverageProcessingTime = (AverageProcessingTime * (FrameCount - 1) + timeMs) / FrameCount;
        }

        private void Statistics_Load(object sender, EventArgs e)
        {

        }

        private void Statistics_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
