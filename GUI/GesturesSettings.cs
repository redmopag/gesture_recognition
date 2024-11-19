using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gesture_Recognition_App
{
    public partial class GesturesSettings : Form
    {
        private Dictionary<string, GestureAction> gestureActions = new Dictionary<string, GestureAction>();

        public class GestureAction 
        { 
            public int ActionCode {  get; set; }
            public string FilePath { get; set; }

            public GestureAction(int actionCode, string filePath = null)
            {
                ActionCode = actionCode;
                FilePath = filePath;
            }
        }

        private List<ActionItem> availableActions = new List<ActionItem>
        {
            new ActionItem(0, "Нет действия"),
            new ActionItem(1, "Сделать скриншот"),
            new ActionItem(2, "Уменьшить громкость"),
            new ActionItem(3, "Увеличить громкость"),
            new ActionItem(4, "Сделать снимок с камеры"),
            new ActionItem(5, "Открыть программу...")
        };

        public class ActionItem
        {
            public int ActionCode { get; }
            public string ActionName { get; }

            public ActionItem(int actionCode, string actionName)
            {
                ActionCode = actionCode;
                ActionName = actionName;
            }

            public override string ToString()
            {
                return ActionName;
            }
        }

        public GesturesSettings()
        {
            InitializeComponent();
            PopulateActionComboBox();
            LoadSettings();
            PopulateGesturesList();
        }

        private void PopulateActionComboBox()
        {
            comboBoxAction.Items.Clear();
            comboBoxAction.Items.AddRange(availableActions.ToArray());
            comboBoxAction.SelectedIndex = 0;
        }

        private void PopulateGesturesList()
        {
            listBoxGesture.Items.Clear();
            foreach (var gesture in gestureActions.Keys)
            {
                listBoxGesture.Items.Add(gesture);
            }
        }

        private void listBoxGestures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxGesture.SelectedItem != null)
            {
                string selectedGesture = listBoxGesture.SelectedItem.ToString();

                int actionCode = gestureActions[selectedGesture].ActionCode;
                comboBoxAction.SelectedItem = comboBoxAction.Items.Cast<ActionItem>()
                                          .FirstOrDefault(item => item.ActionCode == actionCode);

                if (actionCode == 5)
                {
                    textBoxFilePath.Visible = true;
                    btnSelectFile.Visible = true;

                    textBoxFilePath.Text = gestureActions[selectedGesture].FilePath ?? string.Empty;
                }
                else
                {
                    textBoxFilePath.Visible = false;
                    btnSelectFile.Visible = false;
                }
            }
        }


        private void comboBoxAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxGesture.SelectedItem != null)
            {
                string selectedGesture = listBoxGesture.SelectedItem.ToString();
                var selectedAction = (ActionItem)comboBoxAction.SelectedItem;

                gestureActions[selectedGesture].ActionCode = selectedAction.ActionCode;

                if (selectedAction.ActionCode == 5)
                {
                    textBoxFilePath.Visible = true;
                    btnSelectFile.Visible = true;

                    if (!string.IsNullOrEmpty(gestureActions[selectedGesture].FilePath))
                    {
                        textBoxFilePath.Text = gestureActions[selectedGesture].FilePath;
                    }
                    else
                    {
                        textBoxFilePath.Clear();
                    }
                }
                else
                {
                    textBoxFilePath.Visible = false;
                    btnSelectFile.Visible = false;
                }
            }
        }

        private void btnAddGesture_Click(object sender, EventArgs e)
        {
            string newGesture = textBoxGesture.Text.Trim();

            if (!string.IsNullOrEmpty(newGesture))
            {
                if (!gestureActions.ContainsKey(newGesture))
                {
                    gestureActions[newGesture] = new GestureAction(0);

                    listBoxGesture.Items.Add(newGesture);

                    SaveSettings();

                    textBoxGesture.Clear();
                }
                else
                {
                    MessageBox.Show("Жест уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Введите название жеста.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveSettings()
        {
            try
            {
                var json = JsonSerializer.Serialize(gestureActions);
                File.WriteAllText("settings.json", json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения настроек: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show($"Ошибка при загрузке настроек: {ex.Message}. Старый файл будет удален, и будет создан новый.");
                    DeleteSettingsFile();
                    gestureActions = new Dictionary<string, GestureAction>();
                }
            }
            else
            {
                gestureActions = new Dictionary<string, GestureAction>();
            }
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


        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            SaveSettings();
            MessageBox.Show("Настройки сохранены.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Исполняемые файлы (*.exe)|*.exe|Все файлы (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    textBoxFilePath.Text = filePath;

                    if (listBoxGesture.SelectedItem != null)
                    {
                        string selectedGesture = listBoxGesture.SelectedItem.ToString();
                        gestureActions[selectedGesture].FilePath = filePath;
                    }
                }
            }
        }
    }
}


