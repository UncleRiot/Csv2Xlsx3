using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Csv2Xlsx3
{
    public partial class ColumnSelectionForm : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        public List<string> SelectedColumns { get; private set; } = new List<string>();

        public ColumnSelectionForm(List<string> columns, DataTable previewTable)
        {
            InitializeComponent();

            ApplyLanguage();

            foreach (var column in columns)
            {
                checkedListBoxColumns.Items.Add(column, true);
            }

            if (previewTable != null)
            {
                dataGridViewPreview.DataSource = previewTable;
            }

            ModernFormStyler.Apply(this);

            MinimumSize = Size;
            LoadColumnSelectionWindowSize();
            CreateColumnSelectionResizeGrip();
        }

        private void ApplyLanguage()
        {
            Text = LanguageManager.T("ColumnSelection.Title");
            buttonOk.Text = LanguageManager.T("Button.OK");
            buttonCancel.Text = LanguageManager.T("Button.Cancel");
        }

        private void CreateColumnSelectionResizeGrip()
        {
            if (Controls.ContainsKey("panelColumnSelectionResizeGrip"))
                return;

            Panel panelColumnSelectionResizeGrip = new Panel
            {
                Name = "panelColumnSelectionResizeGrip",
                Size = new Size(ModernTheme.MainWindowResizeGripSize, ModernTheme.MainWindowResizeGripSize),
                Location = new Point(
                    ClientSize.Width - ModernTheme.MainWindowResizeGripSize - ModernTheme.WindowBorderWidth,
                    ClientSize.Height - ModernTheme.MainWindowResizeGripSize - ModernTheme.WindowBorderWidth),
                Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
                Cursor = Cursors.SizeNWSE,
                BackColor = ModernTheme.WindowBackColor
            };

            panelColumnSelectionResizeGrip.Paint += (sender, e) =>
            {
                ModernTheme.DrawMainWindowResizeGrip(e.Graphics, panelColumnSelectionResizeGrip.ClientSize);
            };

            panelColumnSelectionResizeGrip.MouseDown += (sender, e) =>
            {
                if (e.Button != MouseButtons.Left)
                    return;

                const int WM_NCLBUTTONDOWN = 0xA1;
                const int HTBOTTOMRIGHT = 17;

                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HTBOTTOMRIGHT, 0);
            };

            Controls.Add(panelColumnSelectionResizeGrip);
            panelColumnSelectionResizeGrip.BringToFront();
        }

        private void LoadColumnSelectionWindowSize()
        {
            string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CSVtoXLSX.cfg");

            if (!File.Exists(configFile))
                return;

            int width = Width;
            int height = Height;

            foreach (var line in File.ReadAllLines(configFile))
            {
                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2)
                    continue;

                if (parts[0].Trim() == "ColumnSelectionWindowWidth")
                    int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out width);

                if (parts[0].Trim() == "ColumnSelectionWindowHeight")
                    int.TryParse(parts[1].Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out height);
            }

            width = Math.Max(width, MinimumSize.Width);
            height = Math.Max(height, MinimumSize.Height);

            Size = new Size(width, height);
        }

        private void SaveColumnSelectionWindowSize()
        {
            if (WindowState != FormWindowState.Normal)
                return;

            string configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CSVtoXLSX.cfg");
            var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (File.Exists(configFile))
            {
                foreach (var line in File.ReadAllLines(configFile))
                {
                    var parts = line.Split(new[] { '=' }, 2);
                    if (parts.Length != 2)
                        continue;

                    settings[parts[0].Trim()] = parts[1].Trim();
                }
            }

            settings["ColumnSelectionWindowWidth"] = Math.Max(Width, MinimumSize.Width).ToString(CultureInfo.InvariantCulture);
            settings["ColumnSelectionWindowHeight"] = Math.Max(Height, MinimumSize.Height).ToString(CultureInfo.InvariantCulture);

            File.WriteAllLines(
                configFile,
                settings.Select(setting => setting.Key + "=" + setting.Value));
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            Control panelColumnSelectionResizeGrip = Controls["panelColumnSelectionResizeGrip"];
            if (panelColumnSelectionResizeGrip != null)
            {
                panelColumnSelectionResizeGrip.Location = new Point(
                    ClientSize.Width - panelColumnSelectionResizeGrip.Width - ModernTheme.WindowBorderWidth,
                    ClientSize.Height - panelColumnSelectionResizeGrip.Height - ModernTheme.WindowBorderWidth);
                panelColumnSelectionResizeGrip.BringToFront();
                panelColumnSelectionResizeGrip.Invalidate();
            }
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            SaveColumnSelectionWindowSize();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveColumnSelectionWindowSize();
            base.OnFormClosing(e);
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            SelectedColumns = checkedListBoxColumns.CheckedItems.Cast<string>().ToList();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}