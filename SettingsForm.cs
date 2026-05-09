using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace Csv2Xlsx3
{
    public partial class SettingsForm : Form
    {
        private readonly string _configFile;
        private ToolTip _toolTip;

        public SettingsForm()
        {
            InitializeComponent();

            _configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CSVtoXLSX.cfg");

            ApplyLanguage();
            LoadLanguageOptions();
            LoadSettings();

            labelConfigPath.Text = LanguageManager.T("Settings.ConfigPathText", _configFile);
            labelShellExePath.Text = GetShellContextMenuExecutablePath();

            _toolTip = new ToolTip();
            _toolTip.AutoPopDelay = 10000;
            _toolTip.InitialDelay = 300;
            _toolTip.ReshowDelay = 100;
            _toolTip.ShowAlways = true;

            _toolTip.SetToolTip(labelConfigPath, WrapText(_configFile, 80));
            _toolTip.SetToolTip(labelShellExePath, WrapText(labelShellExePath.Text, 80));

            ModernFormStyler.Apply(this);

            UpdateShellContextSettingsState();
        }

        private void ApplyLanguage()
        {
            Text = LanguageManager.T("Settings.Title");
            checkBoxSystray.Text = LanguageManager.T("Settings.Systray");
            checkBoxShell.Text = LanguageManager.T("Settings.Shell");
            checkBoxShellColumnSelection.Text = LanguageManager.T("Settings.ShellContextColumnSelection");
            checkBoxOnTop.Text = LanguageManager.T("Settings.AlwaysOnTop");
            labelCsvDelimiter.Text = LanguageManager.T("Settings.CsvDelimiter");
            labelDelimiterHint.Text = LanguageManager.T("Settings.DelimiterHint");
            labelLanguage.Text = LanguageManager.T("Settings.Language");
            labelConfigPath.Text = LanguageManager.T("Settings.ConfigPathRuntime");
            labelShellExeInfo.Text = LanguageManager.T("Settings.ShellExeInfo");
            labelShellExePath.Text = LanguageManager.T("Settings.ConfigPathRuntime");
            buttonOk.Text = LanguageManager.T("Button.OK");
            buttonCancel.Text = LanguageManager.T("Button.Cancel");
        }

        private void LoadLanguageOptions()
        {
            comboBoxLanguage.Items.Clear();

            foreach (var languageOption in LanguageManager.GetAvailableLanguages())
            {
                comboBoxLanguage.Items.Add(languageOption);

                if (string.Equals(languageOption.Code, LanguageManager.CurrentLanguageCode, StringComparison.OrdinalIgnoreCase))
                {
                    comboBoxLanguage.SelectedItem = languageOption;
                }
            }

            if (comboBoxLanguage.SelectedIndex < 0 && comboBoxLanguage.Items.Count > 0)
            {
                comboBoxLanguage.SelectedIndex = 0;
            }
        }

        private string WrapText(string text, int maxLineLength)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var sb = new StringBuilder();
            int current = 0;

            while (current < text.Length)
            {
                int length = Math.Min(maxLineLength, text.Length - current);
                sb.AppendLine(text.Substring(current, length));
                current += length;
            }

            return sb.ToString();
        }

        private void LoadSettings()
        {
            var settings = ReadSettings();

            checkBoxOnTop.Checked = GetBool(settings, "AlwaysOnTop");
            checkBoxSystray.Checked = GetBool(settings, "Systray");
            checkBoxShell.Checked = IsContextMenuRegistered();
            checkBoxShellColumnSelection.Checked = GetBool(settings, "ShellContextColumnSelection");

            if (settings.TryGetValue("CsvDelimiter", out var delimiter))
            {
                textBoxCsvDelimiter.Text = delimiter == @"\t" ? @"\t" : delimiter;
            }
            else
            {
                textBoxCsvDelimiter.Text = ",";
            }
        }

        private Dictionary<string, string> ReadSettings()
        {
            var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!File.Exists(_configFile))
                return settings;

            foreach (var line in File.ReadAllLines(_configFile))
            {
                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2)
                    continue;

                settings[parts[0].Trim()] = parts[1].Trim();
            }

            return settings;
        }

        private static bool GetBool(Dictionary<string, string> settings, string key)
        {
            return settings.TryGetValue(key, out var value) &&
                   value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        private void SaveSettings()
        {
            var settings = ReadSettings();

            settings["AlwaysOnTop"] = checkBoxOnTop.Checked.ToString().ToLower();
            settings["Systray"] = checkBoxSystray.Checked.ToString().ToLower();
            settings["CsvDelimiter"] = NormalizeDelimiter(textBoxCsvDelimiter.Text);
            settings["ShellContextColumnSelection"] = (checkBoxShell.Checked && checkBoxShellColumnSelection.Checked).ToString().ToLower();

            if (comboBoxLanguage.SelectedItem is LanguageOption selectedLanguage)
            {
                settings["Language"] = selectedLanguage.Code;
                LanguageManager.SetCurrentLanguage(selectedLanguage.Code);
            }

            File.WriteAllLines(
                _configFile,
                settings.Select(setting => setting.Key + "=" + setting.Value));
        }

        private static string NormalizeDelimiter(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return ",";

            value = value.Trim();

            if (value == @"\t")
                return @"\t";

            return value;
        }

        private static bool IsContextMenuRegistered()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\SystemFileAssociations\.csv\shell\CSVtoXLSX"))
            {
                return key != null;
            }
        }

        private static void RegisterContextMenu()
        {
            using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\SystemFileAssociations\.csv\shell\CSVtoXLSX"))
            {
                if (key == null)
                    return;

                string exePath = Application.ExecutablePath;

                key.SetValue(string.Empty, "Convert with CSV2XLSX");
                key.SetValue("Icon", exePath);

                using (var commandKey = key.CreateSubKey("command"))
                {
                    if (commandKey == null)
                        return;

                    commandKey.SetValue(string.Empty, $"\"{exePath}\" \"%1\"");
                }
            }
        }

        private static void UnregisterContextMenu()
        {
            Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\SystemFileAssociations\.csv\shell\CSVtoXLSX", false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            checkBoxShell.CheckedChanged += CheckBoxShell_CheckedChanged;
            buttonOk.Click += ButtonOk_Click;
            buttonCancel.Click += ButtonCancel_Click;
        }
        private void CheckBoxShell_CheckedChanged(object sender, EventArgs e)
        {
            UpdateShellContextSettingsState();
        }
        private void UpdateShellContextSettingsState()
        {
            bool shellEnabled = checkBoxShell.Checked;

            checkBoxShellColumnSelection.Enabled = true;
            checkBoxShellColumnSelection.AutoCheck = shellEnabled;
            checkBoxShellColumnSelection.TabStop = shellEnabled;
            checkBoxShellColumnSelection.ForeColor = shellEnabled
                ? ModernTheme.TextColor
                : System.Drawing.SystemColors.GrayText;

            labelShellExeInfo.Visible = shellEnabled;
            labelShellExePath.Visible = shellEnabled;

            if (!shellEnabled)
            {
                checkBoxShellColumnSelection.Checked = false;
            }
        }
        private static string GetShellContextMenuExecutablePath()
        {
            return Application.ExecutablePath;
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            SaveSettings();

            if (checkBoxShell.Checked)
                RegisterContextMenu();
            else
                UnregisterContextMenu();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}