// ---------------------------------------------------------------------------
// Datei:      MainForm.cs
// Autor:      Daniel Capilla
// Projekt:    CSVtoXLSX_v3
// Version:    1.0.1 (Stand: 2026-05-06)
// ---------------------------------------------------------------------------
// Beschreibung:
// Hauptfenster für die Auswahl und Konvertierung von CSV-Dateien zu XLSX.
// Features:
//   - Drag & Drop von CSV
//   - Batchkonvertierung
//   - Spaltenauswahl mit Vorschau
//   - Minimierung ins Systray (Tray-Icon)
//   - Shell-Kontextmenü (Rechtsklick auf .csv in Windows-Explorer)
//   - Einstellungsdatei: CSVtoXLSX.cfg im Programmverzeichnis
//   - Registry: Shell-Kontextmenü wird (de)aktiviert unter HKCU\Software\Classes\SystemFileAssociations\.csv\shell\CSVtoXLSX
// ---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Configuration;

namespace Csv2Xlsx3
{
    public partial class MainForm : Form
    {
        private readonly string[] _args;

        public static void ProcessCsvFileSilent(string csvFilePath)
        {
            var dt = LoadCsvIntoDataTableSilent(csvFilePath);
            string xlsxPath = Path.ChangeExtension(csvFilePath, ".xlsx");

            using (var wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt, "Daten");
                wb.SaveAs(xlsxPath);
            }
        }
        private static string DetectCsvDelimiterSilent(string path)
        {
            var configuredDelimiter = GetConfiguredCsvDelimiter();

            if (TryDetectCsvDelimiter(path, configuredDelimiter, out var detectedDelimiter))
                return detectedDelimiter;

            return configuredDelimiter;
        }
        private static CsvConfiguration CreateCsvConfigurationSilent(string path)
        {
            return new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = DetectCsvDelimiterSilent(path),
                BadDataFound = null,
                MissingFieldFound = null,
                HeaderValidated = null,
                IgnoreBlankLines = true
            };
        }
        private static DataTable LoadCsvIntoDataTableSilent(string path)
        {
            var dt = new DataTable();

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CreateCsvConfigurationSilent(path)))
            using (var dr = new CsvDataReader(csv))
            {
                dt.Load(dr);
            }

            return dt;
        }
        private void CreateMainWindowResizeGrip()
        {
            if (Controls.ContainsKey("panelMainWindowResizeGrip"))
                return;

            Panel panelMainWindowResizeGrip = new Panel
            {
                Name = "panelMainWindowResizeGrip",
                Size = new Size(ModernTheme.MainWindowResizeGripSize, ModernTheme.MainWindowResizeGripSize),
                Location = new Point(
                    ClientSize.Width - ModernTheme.MainWindowResizeGripSize - ModernTheme.WindowBorderWidth,
                    ClientSize.Height - ModernTheme.MainWindowResizeGripSize - ModernTheme.WindowBorderWidth),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Cursor = Cursors.SizeNWSE,
                BackColor = ModernTheme.ControlBackColor
            };

            panelMainWindowResizeGrip.Paint += (sender, e) =>
            {
                ModernTheme.DrawMainWindowResizeGrip(e.Graphics, panelMainWindowResizeGrip.ClientSize);
            };

            panelMainWindowResizeGrip.MouseDown += (sender, e) =>
            {
                if (e.Button != MouseButtons.Left)
                    return;

                const int WM_NCLBUTTONDOWN = 0xA1;
                const int HTBOTTOMRIGHT = 17;

                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTBOTTOMRIGHT, 0);
            };

            Controls.Add(panelMainWindowResizeGrip);
            panelMainWindowResizeGrip.BringToFront();
        }
        public MainForm() : this(Array.Empty<string>())
        {
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        public MainForm(string[] args)
        {
            _args = args;

            DateTime expiry = new DateTime(2027, 05, 31);
            if (DateTime.Now.Date > expiry)
            {
                ModernTheme.ShowMessage(
                    "Die Testversion dieser Anwendung ist abgelaufen.\n\nBitte wenden Sie sich an den Autor.",
                    "Testzeitraum abgelaufen",
                    MessageBoxIcon.Stop);
                Environment.Exit(0);
                return;
            }

            InitializeComponent();
            Text = "CSV2XLSX";
            MinimumSize = ModernTheme.MainWindowMinimumSize;
            StartPosition = FormStartPosition.CenterScreen;
            LoadMainWindowSize();
            ApplyWindowIcon();
            ApplySettings();
            ModernFormStyler.Apply(this);
            CreateMainWindowResizeGrip();

            notifyIcon.Text = "CSV2XLSX";

            if (_args != null && _args.Length > 0 && File.Exists(_args[0]))
            {
                HandleCsvFile(_args[0]);
            }
        }
        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            SaveMainWindowSize();
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveMainWindowSize();
            base.OnFormClosing(e);
        }
        private void SaveMainWindowSize()
        {
            if (WindowState != FormWindowState.Normal)
                return;

            var settings = LoadCsvConfiguration();

            settings["MainWindowWidth"] = Math.Max(Width, ModernTheme.MainWindowMinimumSize.Width).ToString(CultureInfo.InvariantCulture);
            settings["MainWindowHeight"] = Math.Max(Height, ModernTheme.MainWindowMinimumSize.Height).ToString(CultureInfo.InvariantCulture);

            SaveCsvConfiguration(settings);
        }
        private void LoadMainWindowSize()
        {
            var settings = LoadCsvConfiguration();

            int width = Width;
            int height = Height;

            if (settings.TryGetValue("MainWindowWidth", out var widthValue))
            {
                int.TryParse(widthValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out width);
            }

            if (settings.TryGetValue("MainWindowHeight", out var heightValue))
            {
                int.TryParse(heightValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out height);
            }

            width = Math.Max(width, ModernTheme.MainWindowMinimumSize.Width);
            height = Math.Max(height, ModernTheme.MainWindowMinimumSize.Height);

            Size = new Size(width, height);
        }

        private void ApplyWindowIcon()
        {
            ModernTheme.ApplyApplicationIcon(this);

            if (notifyIcon != null)
            {
                ModernTheme.ApplyApplicationIcon(notifyIcon);
            }
        }

        private void ApplySettings()
        {
            var settings = LoadCsvConfiguration();

            TopMost =
                settings.TryGetValue("AlwaysOnTop", out var alwaysOnTopValue) &&
                alwaysOnTopValue.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var settings = LoadCsvConfiguration();

            bool minimizeToTray =
                settings.TryGetValue("Systray", out var systrayValue) &&
                systrayValue.Trim().Equals("true", StringComparison.OrdinalIgnoreCase);

            if (minimizeToTray && WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.Visible = true;
            }

            Control panelMainWindowResizeGrip = Controls["panelMainWindowResizeGrip"];
            if (panelMainWindowResizeGrip != null)
            {
                panelMainWindowResizeGrip.Location = new Point(
                    ClientSize.Width - panelMainWindowResizeGrip.Width - ModernTheme.WindowBorderWidth,
                    ClientSize.Height - panelMainWindowResizeGrip.Height - ModernTheme.WindowBorderWidth);
                panelMainWindowResizeGrip.BringToFront();
                panelMainWindowResizeGrip.Invalidate();
            }

            Invalidate();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTCLIENT = 1;
            const int HTBOTTOMRIGHT = 17;

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m);

                if ((int)m.Result == HTCLIENT)
                {
                    Point clientPoint = PointToClient(new Point(
                        unchecked((short)(long)m.LParam),
                        unchecked((short)((long)m.LParam >> 16))));

                    if (ModernTheme.IsInMainWindowResizeGripArea(ClientSize, clientPoint))
                    {
                        m.Result = (IntPtr)HTBOTTOMRIGHT;
                        return;
                    }
                }

                return;
            }

            base.WndProc(ref m);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
        private void einstellungenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm())
            {
                settingsForm.StartPosition = FormStartPosition.CenterParent;
                settingsForm.TopMost = TopMost;

                if (settingsForm.ShowDialog(this) == DialogResult.OK)
                {
                    ApplySettings();
                }
            }
        }

        private void programmBeendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void überToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var about = new AboutForm())
            {
                about.StartPosition = FormStartPosition.CenterParent;
                about.TopMost = TopMost;
                about.ShowDialog(this);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void dropPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1 &&
                    Path.GetExtension(files[0]).Equals(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void dropPanel_DragDrop(object sender, DragEventArgs e)
        {
            var file = ((string[])e.Data.GetData(DataFormats.FileDrop)).First();

            try
            {
                var dt = LoadCsvIntoDataTable(file);
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

                using (var colForm = new ColumnSelectionForm(columnNames, dt))
                {
                    colForm.StartPosition = FormStartPosition.CenterParent;
                    colForm.TopMost = TopMost;

                    if (colForm.ShowDialog(this) != DialogResult.OK || colForm.SelectedColumns.Count == 0)
                        return;

                    var dtFiltered = dt.DefaultView.ToTable(false, colForm.SelectedColumns.ToArray());

                    using (var saveDlg = new SaveFileDialog())
                    {
                        saveDlg.Filter = "Excel Workbook|*.xlsx";
                        saveDlg.Title = "Speichern unter";
                        saveDlg.FileName = Path.GetFileNameWithoutExtension(file) + ".xlsx";

                        if (saveDlg.ShowDialog(this) == DialogResult.OK)
                        {
                            using (var wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(dtFiltered, "Daten");
                                wb.SaveAs(saveDlg.FileName);
                            }

                            ModernTheme.ShowMessage(
                                this,
                                "Erfolgreich gespeichert!",
                                "Fertig",
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModernTheme.ShowMessage(
                    this,
                    "Fehler: " + ex.Message,
                    "Fehler",
                    MessageBoxIcon.Error);
            }
        }

        private void batchjobToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var openDlg = new OpenFileDialog())
            {
                openDlg.Title = "Mehrere CSV-Dateien wählen";
                openDlg.Filter = "CSV-Dateien (*.csv)|*.csv";
                openDlg.Multiselect = true;

                if (openDlg.ShowDialog(this) != DialogResult.OK || openDlg.FileNames.Length == 0)
                    return;

                var fileGroups = new Dictionary<string, List<string>>();
                var headerDict = new Dictionary<string, List<string>>();

                foreach (var file in openDlg.FileNames)
                {
                    var columns = ReadCsvHeaders(file);
                    if (columns == null)
                        continue;

                    string key = string.Join("|", columns);
                    if (!fileGroups.ContainsKey(key))
                    {
                        fileGroups[key] = new List<string>();
                        headerDict[key] = columns;
                    }

                    fileGroups[key].Add(file);
                }

                bool batchAborted = false;

                foreach (var group in fileGroups)
                {
                    var files = group.Value;
                    var header = headerDict[group.Key];
                    var previewTable = LoadCsvPreview(files.First(), 5);

                    using (var colForm = new ColumnSelectionForm(header, previewTable))
                    {
                        colForm.StartPosition = FormStartPosition.CenterParent;
                        colForm.TopMost = TopMost;

                        var dialogResult = colForm.ShowDialog(this);
                        if (dialogResult != DialogResult.OK || colForm.SelectedColumns.Count == 0)
                        {
                            batchAborted = true;
                            break;
                        }

                        foreach (var file in files)
                        {
                            var dt = LoadCsvIntoDataTable(file);

                            var selectedCols = colForm.SelectedColumns
                                .Where(col => !string.IsNullOrWhiteSpace(col) && dt.Columns.Contains(col))
                                .ToArray();

                            if (selectedCols.Length == 0)
                            {
                                ModernTheme.ShowMessage(
                                    this,
                                    "Keine gültigen Spalten zur Auswahl!",
                                    "Fehler",
                                    MessageBoxIcon.Error);
                                continue;
                            }

                            var dtFiltered = dt.DefaultView.ToTable(false, selectedCols);
                            string xlsxPath = Path.ChangeExtension(file, ".xlsx");

                            using (var wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(dtFiltered, "Daten");
                                wb.SaveAs(xlsxPath);
                            }
                        }
                    }

                    if (batchAborted)
                        break;
                }

                if (!batchAborted)
                {
                    ModernTheme.ShowMessage(
                        this,
                        "Alle Dateien wurden verarbeitet.",
                        "Batchjob beendet",
                        MessageBoxIcon.Information);
                }
            }
        }

        private DataTable LoadCsvIntoDataTable(string path)
        {
            return LoadCsvIntoDataTable(path, true);
        }

        private List<string> ReadCsvHeaders(string path)
        {
            return ReadCsvHeaders(path, true);
        }

        private DataTable LoadCsvPreview(string path, int maxRows = 5)
        {
            return LoadCsvPreview(path, maxRows, true);
        }

        private DataTable LoadCsvIntoDataTable(string path, bool allowUserPrompt)
        {
            var dt = new DataTable();

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CreateCsvConfiguration(path, allowUserPrompt)))
            using (var dr = new CsvDataReader(csv))
            {
                dt.Load(dr);
            }

            return dt;
        }

        private List<string> ReadCsvHeaders(string path, bool allowUserPrompt)
        {
            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CreateCsvConfiguration(path, allowUserPrompt)))
            {
                csv.Read();
                csv.ReadHeader();
                return csv.HeaderRecord?.ToList();
            }
        }

        private DataTable LoadCsvPreview(string path, int maxRows, bool allowUserPrompt)
        {
            var dt = new DataTable();

            using (var reader = new StreamReader(path))
            using (var csv = new CsvReader(reader, CreateCsvConfiguration(path, allowUserPrompt)))
            using (var dr = new CsvDataReader(csv))
            {
                dt.Load(dr);
            }

            var preview = dt.Clone();
            for (int i = 0; i < Math.Min(maxRows, dt.Rows.Count); i++)
            {
                preview.ImportRow(dt.Rows[i]);
            }

            return preview;
        }

        private CsvConfiguration CreateCsvConfiguration(string path, bool allowUserPrompt)
        {
            return new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = DetectCsvDelimiter(path, allowUserPrompt),
                BadDataFound = null,
                MissingFieldFound = null,
                HeaderValidated = null,
                IgnoreBlankLines = true
            };
        }

        private string DetectCsvDelimiter(string path, bool allowUserPrompt)
        {
            var configuredDelimiter = GetConfiguredCsvDelimiter();

            if (TryDetectCsvDelimiter(path, configuredDelimiter, out var detectedDelimiter))
                return detectedDelimiter;

            if (allowUserPrompt)
                return PromptForCsvDelimiter(configuredDelimiter);

            return configuredDelimiter;
        }

        private static bool TryDetectCsvDelimiter(string path, string configuredDelimiter, out string detectedDelimiter)
        {
            detectedDelimiter = configuredDelimiter;

            var lines = File.ReadLines(path)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Take(5)
                .ToList();

            if (lines.Count == 0)
                return true;

            var candidates = new List<string>();
            if (!string.IsNullOrEmpty(configuredDelimiter))
                candidates.Add(configuredDelimiter);

            candidates.Add(",");
            candidates.Add(";");
            candidates.Add("\t");
            candidates.Add("|");

            candidates = candidates.Distinct().ToList();

            var evaluations = candidates
                .Select(delimiter => new
                {
                    Delimiter = delimiter,
                    Counts = lines.Select(line => CountDelimiterOccurrences(line, delimiter)).ToList()
                })
                .Select(item => new
                {
                    item.Delimiter,
                    item.Counts,
                    PositiveLineCount = item.Counts.Count(count => count > 0),
                    DistinctPositiveCounts = item.Counts.Where(count => count > 0).Distinct().Count(),
                    TotalCount = item.Counts.Sum()
                })
                .ToList();

            var consistentCandidates = evaluations
                .Where(item => item.PositiveLineCount > 0 && item.DistinctPositiveCounts <= 1)
                .OrderByDescending(item => item.PositiveLineCount)
                .ThenByDescending(item => item.TotalCount)
                .ToList();

            if (consistentCandidates.Count == 1)
            {
                detectedDelimiter = consistentCandidates[0].Delimiter;
                return true;
            }

            if (consistentCandidates.Count > 1)
            {
                var bestCandidate = consistentCandidates[0];
                var secondCandidate = consistentCandidates[1];

                if (bestCandidate.PositiveLineCount > secondCandidate.PositiveLineCount ||
                    bestCandidate.TotalCount > secondCandidate.TotalCount)
                {
                    detectedDelimiter = bestCandidate.Delimiter;
                    return true;
                }

                return false;
            }

            var candidatesWithMatches = evaluations
                .Where(item => item.TotalCount > 0)
                .OrderByDescending(item => item.TotalCount)
                .ToList();

            if (candidatesWithMatches.Count == 1)
            {
                detectedDelimiter = candidatesWithMatches[0].Delimiter;
                return true;
            }

            if (candidatesWithMatches.Count > 1 &&
                candidatesWithMatches[0].TotalCount > candidatesWithMatches[1].TotalCount)
            {
                detectedDelimiter = candidatesWithMatches[0].Delimiter;
                return true;
            }

            return false;
        }

        private static int CountDelimiterOccurrences(string line, string delimiter)
        {
            if (string.IsNullOrEmpty(line) || string.IsNullOrEmpty(delimiter))
                return 0;

            int count = 0;
            bool insideQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        i++;
                        continue;
                    }

                    insideQuotes = !insideQuotes;
                    continue;
                }

                if (!insideQuotes &&
                    i + delimiter.Length <= line.Length &&
                    string.Compare(line, i, delimiter, 0, delimiter.Length, StringComparison.Ordinal) == 0)
                {
                    count++;
                    i += delimiter.Length - 1;
                }
            }

            return count;
        }

        private static string GetConfiguredCsvDelimiter()
        {
            var settings = LoadCsvConfiguration();

            if (settings.TryGetValue("CsvDelimiter", out var value))
            {
                value = value.Trim();

                if (value == @"\t")
                    return "\t";

                if (!string.IsNullOrEmpty(value))
                    return value;
            }

            return ",";
        }
        private static void SaveCsvConfiguration(Dictionary<string, string> settings)
        {
            string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CSVtoXLSX.cfg");

            File.WriteAllLines(
                configFile,
                settings.Select(setting => setting.Key + "=" + setting.Value));
        }
        private static Dictionary<string, string> LoadCsvConfiguration()
        {
            var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CSVtoXLSX.cfg");

            if (!File.Exists(configFile))
                return settings;

            foreach (var line in File.ReadAllLines(configFile))
            {
                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2)
                    continue;

                settings[parts[0].Trim()] = parts[1].Trim();
            }

            return settings;
        }

        private string PromptForCsvDelimiter(string configuredDelimiter)
        {
            string selectedDelimiter = configuredDelimiter;

            using (var form = new Form())
            using (var label = new Label())
            using (var comboBox = new ComboBox())
            using (var buttonOk = new Button())
            using (var buttonCancel = new Button())
            {
                form.Text = "CSV-Trennzeichen";
                form.StartPosition = FormStartPosition.CenterParent;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                form.ShowInTaskbar = false;
                form.ClientSize = new Size(390, 185);

                label.AutoSize = false;
                label.Left = 20;
                label.Top = 20;
                label.Width = 350;
                label.Height = 52;
                label.Text = "Das Trennzeichen konnte nicht eindeutig erkannt werden. Bitte wählen Sie das verwendete CSV-Trennzeichen.";
                label.TextAlign = ContentAlignment.MiddleLeft;

                comboBox.Left = 20;
                comboBox.Top = 82;
                comboBox.Width = 350;
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox.DisplayMember = "Value";
                comboBox.ValueMember = "Key";
                comboBox.Items.Add(new KeyValuePair<string, string>(",", "Komma (,)"));
                comboBox.Items.Add(new KeyValuePair<string, string>(";", "Semikolon (;)"));
                comboBox.Items.Add(new KeyValuePair<string, string>("\t", @"Tabulator (\t)"));
                comboBox.Items.Add(new KeyValuePair<string, string>("|", "Pipe (|)"));

                for (int i = 0; i < comboBox.Items.Count; i++)
                {
                    var item = (KeyValuePair<string, string>)comboBox.Items[i];
                    if (item.Key == configuredDelimiter)
                    {
                        comboBox.SelectedIndex = i;
                        break;
                    }
                }

                if (comboBox.SelectedIndex < 0)
                    comboBox.SelectedIndex = 0;

                buttonOk.Text = "OK";
                buttonOk.Left = 195;
                buttonOk.Top = 122;
                buttonOk.Width = 75;
                buttonOk.Height = 30;
                buttonOk.DialogResult = DialogResult.OK;

                buttonCancel.Text = "Abbrechen";
                buttonCancel.Left = 280;
                buttonCancel.Top = 122;
                buttonCancel.Width = 90;
                buttonCancel.Height = 30;
                buttonCancel.DialogResult = DialogResult.Cancel;

                form.Controls.Add(label);
                form.Controls.Add(comboBox);
                form.Controls.Add(buttonOk);
                form.Controls.Add(buttonCancel);
                form.AcceptButton = buttonOk;
                form.CancelButton = buttonCancel;

                ModernTheme.ApplyLabelStyle(label);
                ModernTheme.ApplyComboBoxStyle(comboBox);
                ModernTheme.ApplyButtonStyle(buttonOk);
                ModernTheme.ApplyButtonStyle(buttonCancel);
                ModernFormStyler.Apply(form);

                if (form.ShowDialog(this) == DialogResult.OK)
                    selectedDelimiter = ((KeyValuePair<string, string>)comboBox.SelectedItem).Key;
            }

            return selectedDelimiter;
        }

        private void HandleCsvFile(string csvFilePath)
        {
            try
            {
                var dt = LoadCsvIntoDataTable(csvFilePath);
                var columnNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToList();

                using (var colForm = new ColumnSelectionForm(columnNames, dt))
                {
                    colForm.StartPosition = FormStartPosition.CenterParent;
                    colForm.TopMost = TopMost;

                    if (colForm.ShowDialog(this) != DialogResult.OK || colForm.SelectedColumns.Count == 0)
                        return;

                    var dtFiltered = dt.DefaultView.ToTable(false, colForm.SelectedColumns.ToArray());
                    string xlsxPath = Path.ChangeExtension(csvFilePath, ".xlsx");

                    using (var wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(dtFiltered, "Daten");
                        wb.SaveAs(xlsxPath);
                    }

                    ModernTheme.ShowMessage(
                        this,
                        "Datei wurde erfolgreich konvertiert:\n" + xlsxPath,
                        "Fertig",
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ModernTheme.ShowMessage(
                    this,
                    "Fehler beim Konvertieren:\n" + ex.Message,
                    "Fehler",
                    MessageBoxIcon.Error);
            }
        }
    }
}