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

        private int _draggedColumnIndex = -1;
        private Rectangle _dragBoxFromMouseDown = Rectangle.Empty;
        private Panel _previewColumnDropIndicator;
        private int _previewColumnDragStartIndex = -1;

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
                dataGridViewPreview.AllowUserToOrderColumns = true;
                EnablePreviewColumnDropIndicator();
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

        private void EnableColumnDragDrop()
        {
            checkedListBoxColumns.AllowDrop = true;
            checkedListBoxColumns.MouseDown += CheckedListBoxColumns_MouseDown;
            checkedListBoxColumns.MouseMove += CheckedListBoxColumns_MouseMove;
            checkedListBoxColumns.DragOver += CheckedListBoxColumns_DragOver;
            checkedListBoxColumns.DragDrop += CheckedListBoxColumns_DragDrop;
        }
        private void ApplyPreviewColumnDropIndicatorRegion()
        {
            using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                Point[] points =
                {
            new Point(_previewColumnDropIndicator.Width / 2, _previewColumnDropIndicator.Height - 1),
            new Point(1, 1),
            new Point(_previewColumnDropIndicator.Width - 1, 1)
        };

                path.AddPolygon(points);

                Region oldRegion = _previewColumnDropIndicator.Region;
                _previewColumnDropIndicator.Region = new Region(path);

                if (oldRegion != null)
                {
                    oldRegion.Dispose();
                }
            }
        }
        private void EnablePreviewColumnDropIndicator()
        {
            _previewColumnDropIndicator = new TransparentPanel
            {
                Name = "panelPreviewColumnDropIndicator",
                Size = new Size(15, 15),
                Visible = false,
                BackColor = Color.Transparent
            };

            ApplyPreviewColumnDropIndicatorRegion();

            _previewColumnDropIndicator.Paint += PreviewColumnDropIndicator_Paint;

            dataGridViewPreview.Controls.Add(_previewColumnDropIndicator);
            _previewColumnDropIndicator.BringToFront();

            dataGridViewPreview.MouseDown += DataGridViewPreview_MouseDown;
            dataGridViewPreview.MouseMove += DataGridViewPreview_MouseMove;
            dataGridViewPreview.MouseUp += DataGridViewPreview_MouseUp;
            dataGridViewPreview.ColumnDisplayIndexChanged += DataGridViewPreview_ColumnDisplayIndexChanged;
        }

        private void PreviewColumnDropIndicator_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (Brush brush = new SolidBrush(Color.FromArgb(170, 70, 220)))
            {
                Point[] points =
                {
            new Point(_previewColumnDropIndicator.Width / 2, _previewColumnDropIndicator.Height - 1),
            new Point(1, 1),
            new Point(_previewColumnDropIndicator.Width - 1, 1)
        };

                e.Graphics.FillPolygon(brush, points);
            }
        }

        private void DataGridViewPreview_MouseDown(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo hitTestInfo = dataGridViewPreview.HitTest(e.X, e.Y);

            if (e.Button == MouseButtons.Left && hitTestInfo.Type == DataGridViewHitTestType.ColumnHeader)
            {
                _previewColumnDragStartIndex = hitTestInfo.ColumnIndex;
            }
            else
            {
                _previewColumnDragStartIndex = -1;
                HidePreviewColumnDropIndicator();
            }
        }

        private void DataGridViewPreview_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left)
            {
                HidePreviewColumnDropIndicator();
                return;
            }

            if (_previewColumnDragStartIndex < 0)
                return;

            if (e.Y < 0 || e.Y > dataGridViewPreview.ColumnHeadersHeight)
            {
                HidePreviewColumnDropIndicator();
                return;
            }

            ShowPreviewColumnDropIndicator(e.X);
        }

        private void DataGridViewPreview_MouseUp(object sender, MouseEventArgs e)
        {
            _previewColumnDragStartIndex = -1;
            HidePreviewColumnDropIndicator();
        }

        private void DataGridViewPreview_ColumnDisplayIndexChanged(object sender, DataGridViewColumnEventArgs e)
        {
            HidePreviewColumnDropIndicator();
        }

        private void ShowPreviewColumnDropIndicator(int mouseX)
        {
            if (dataGridViewPreview.Columns.Count == 0)
            {
                HidePreviewColumnDropIndicator();
                return;
            }

            int indicatorX = GetPreviewColumnInsertIndicatorX(mouseX);

            if (indicatorX < 0)
            {
                HidePreviewColumnDropIndicator();
                return;
            }

            _previewColumnDropIndicator.Location = new Point(
                indicatorX - (_previewColumnDropIndicator.Width / 2),
                1);

            _previewColumnDropIndicator.Visible = true;
            _previewColumnDropIndicator.BringToFront();
            _previewColumnDropIndicator.Invalidate();
        }

        private int GetPreviewColumnInsertIndicatorX(int mouseX)
        {
            var displayedColumns = dataGridViewPreview.Columns
                .Cast<DataGridViewColumn>()
                .Where(column => column.Visible)
                .OrderBy(column => column.DisplayIndex)
                .ToList();

            foreach (DataGridViewColumn column in displayedColumns)
            {
                Rectangle headerRectangle = dataGridViewPreview.GetCellDisplayRectangle(column.Index, -1, true);

                if (headerRectangle.Width <= 0)
                    continue;

                if (mouseX >= headerRectangle.Left && mouseX <= headerRectangle.Right)
                {
                    return mouseX < headerRectangle.Left + (headerRectangle.Width / 2)
                        ? headerRectangle.Left
                        : headerRectangle.Right;
                }
            }

            if (displayedColumns.Count == 0)
                return -1;

            Rectangle firstHeaderRectangle = dataGridViewPreview.GetCellDisplayRectangle(displayedColumns[0].Index, -1, true);
            Rectangle lastHeaderRectangle = dataGridViewPreview.GetCellDisplayRectangle(displayedColumns[displayedColumns.Count - 1].Index, -1, true);

            if (mouseX < firstHeaderRectangle.Left)
                return firstHeaderRectangle.Left;

            if (mouseX > lastHeaderRectangle.Right)
                return lastHeaderRectangle.Right;

            return -1;
        }

        private void HidePreviewColumnDropIndicator()
        {
            if (_previewColumnDropIndicator != null)
            {
                _previewColumnDropIndicator.Visible = false;
            }
        }

        private void CheckedListBoxColumns_MouseDown(object sender, MouseEventArgs e)
        {
            _draggedColumnIndex = checkedListBoxColumns.IndexFromPoint(e.Location);

            if (_draggedColumnIndex == ListBox.NoMatches)
            {
                _dragBoxFromMouseDown = Rectangle.Empty;
                return;
            }

            Size dragSize = SystemInformation.DragSize;
            _dragBoxFromMouseDown = new Rectangle(
                new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)),
                dragSize);
        }

        private void CheckedListBoxColumns_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != MouseButtons.Left)
                return;

            if (_draggedColumnIndex == ListBox.NoMatches)
                return;

            if (_dragBoxFromMouseDown == Rectangle.Empty)
                return;

            if (_dragBoxFromMouseDown.Contains(e.Location))
                return;

            checkedListBoxColumns.DoDragDrop(_draggedColumnIndex, DragDropEffects.Move);
        }

        private void CheckedListBoxColumns_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(typeof(int))
                ? DragDropEffects.Move
                : DragDropEffects.None;
        }

        private void CheckedListBoxColumns_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(int)))
                return;

            int sourceIndex = (int)e.Data.GetData(typeof(int));
            Point clientPoint = checkedListBoxColumns.PointToClient(new Point(e.X, e.Y));
            int targetIndex = checkedListBoxColumns.IndexFromPoint(clientPoint);

            if (targetIndex == ListBox.NoMatches)
                targetIndex = checkedListBoxColumns.Items.Count - 1;

            MoveColumnItem(sourceIndex, targetIndex);
            UpdatePreviewColumnOrder();
        }

        private void MoveColumnItem(int sourceIndex, int targetIndex)
        {
            if (sourceIndex < 0 || sourceIndex >= checkedListBoxColumns.Items.Count)
                return;

            if (targetIndex < 0 || targetIndex >= checkedListBoxColumns.Items.Count)
                return;

            if (sourceIndex == targetIndex)
                return;

            object item = checkedListBoxColumns.Items[sourceIndex];
            bool isChecked = checkedListBoxColumns.GetItemChecked(sourceIndex);

            checkedListBoxColumns.Items.RemoveAt(sourceIndex);

            if (targetIndex > sourceIndex)
                targetIndex--;

            checkedListBoxColumns.Items.Insert(targetIndex, item);
            checkedListBoxColumns.SetItemChecked(targetIndex, isChecked);
            checkedListBoxColumns.SelectedIndex = targetIndex;
        }

        private void UpdatePreviewColumnOrder()
        {
            if (dataGridViewPreview.Columns.Count == 0)
                return;

            for (int i = 0; i < checkedListBoxColumns.Items.Count; i++)
            {
                string columnName = checkedListBoxColumns.Items[i].ToString();

                if (!dataGridViewPreview.Columns.Contains(columnName))
                    continue;

                dataGridViewPreview.Columns[columnName].DisplayIndex = i;
            }
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
        private sealed class TransparentPanel : Panel
        {
            public TransparentPanel()
            {
                SetStyle(
                    ControlStyles.SupportsTransparentBackColor |
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer,
                    true);

                BackColor = Color.Transparent;
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            var checkedColumns = checkedListBoxColumns.CheckedItems.Cast<string>().ToHashSet(StringComparer.OrdinalIgnoreCase);

            SelectedColumns = dataGridViewPreview.Columns
                .Cast<DataGridViewColumn>()
                .OrderBy(column => column.DisplayIndex)
                .Select(column => column.Name)
                .Where(columnName => checkedColumns.Contains(columnName))
                .ToList();

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