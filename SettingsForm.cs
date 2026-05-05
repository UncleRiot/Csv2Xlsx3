using System;
using System.Collections.Generic;
using System.IO;
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
            LoadSettings();

            string displayText = "Einstellungen werden gespeichert unter:\n" + _configFile;
            labelConfigPath.Text = displayText;

            _toolTip = new ToolTip();
            _toolTip.AutoPopDelay = 10000;
            _toolTip.InitialDelay = 300;
            _toolTip.ReshowDelay = 100;
            _toolTip.ShowAlways = true;

            _toolTip.SetToolTip(labelConfigPath, WrapText(_configFile, 80));
            ModernFormStyler.Apply(this);
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
            var lines = new[]
            {
                "AlwaysOnTop=" + checkBoxOnTop.Checked.ToString().ToLower(),
                "Systray=" + checkBoxSystray.Checked.ToString().ToLower(),
                "CsvDelimiter=" + NormalizeDelimiter(textBoxCsvDelimiter.Text)
            };

            File.WriteAllLines(_configFile, lines);
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

                key.SetValue(string.Empty, "Mit CSVtoXLSX konvertieren");

                using (var commandKey = key.CreateSubKey("command"))
                {
                    if (commandKey == null)
                        return;

                    string exePath = Application.ExecutablePath;
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

            buttonOk.Click += ButtonOk_Click;
            buttonCancel.Click += ButtonCancel_Click;
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