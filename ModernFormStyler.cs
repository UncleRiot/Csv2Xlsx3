using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Csv2Xlsx3
{
    public static class ModernFormStyler
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public static void Apply(Form form)
        {
            bool allowMaximize = form.MaximizeBox;
            bool allowMinimize = form.MinimizeBox;

            form.SuspendLayout();

            ModernTheme.ApplyApplicationIcon(form);

            form.FormBorderStyle = FormBorderStyle.None;
            form.BackColor = ModernTheme.WindowBackColor;
            form.ForeColor = ModernTheme.TextColor;
            form.Font = ModernTheme.DefaultFont;
            form.SizeGripStyle = SizeGripStyle.Hide;

            MoveExistingControlsBelowTitleBar(form);
            AddModernTitleBar(form, allowMinimize, allowMaximize);

            ModernWindowFrame.Apply(form);

            foreach (Control control in form.Controls)
            {
                ApplyControl(control);
            }

            form.ResumeLayout(false);
            form.PerformLayout();
        }

        private static void MoveExistingControlsBelowTitleBar(Form form)
        {
            if (form.Controls.ContainsKey("panelModernTitleBar"))
                return;

            foreach (Control control in form.Controls)
            {
                if (control.Dock == DockStyle.None)
                {
                    control.Top += ModernTheme.TitleBarHeight;
                }
            }

            form.Padding = new Padding(
                form.Padding.Left,
                form.Padding.Top + ModernTheme.TitleBarHeight,
                form.Padding.Right,
                form.Padding.Bottom);

            form.Height += ModernTheme.TitleBarHeight;
        }

        private static void AddModernTitleBar(Form form, bool allowMinimize, bool allowMaximize)
        {
            if (form.Controls.ContainsKey("panelModernTitleBar"))
                return;

            Panel panelModernTitleBar = new Panel
            {
                Name = "panelModernTitleBar",
                Location = new Point(0, 0),
                Size = new Size(form.ClientSize.Width, ModernTheme.TitleBarHeight),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = ModernTheme.TitleBarBackColor
            };

            PictureBox pictureBoxModernTitleIcon = new PictureBox
            {
                Name = "pictureBoxModernTitleIcon",
                Location = new Point(ModernTheme.TitleBarIconLeft, ModernTheme.TitleBarIconTop),
                Size = new Size(ModernTheme.TitleBarIconSize, ModernTheme.TitleBarIconSize),
                SizeMode = PictureBoxSizeMode.StretchImage,
                BackColor = Color.Transparent,
                Image = form.Icon != null ? form.Icon.ToBitmap() : null
            };

            Label labelModernTitle = new Label
            {
                Name = "labelModernTitle",
                Text = " " + form.Text,
                AutoSize = false,
                Location = new Point(ModernTheme.TitleBarTextLeft, 0),
                Size = new Size(form.ClientSize.Width - 140, ModernTheme.TitleBarHeight),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = ModernTheme.TextColor,
                BackColor = Color.Transparent,
                Font = new Font(ModernTheme.FontFamilyName, ModernTheme.TitleFontSize, FontStyle.Regular)
            };

            Button buttonModernClose = CreateModernTitleBarButton(
                "buttonModernClose",
                "×",
                new Point(form.ClientSize.Width - ModernTheme.TitleBarButtonSize.Width, 0));

            buttonModernClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonModernClose.MouseEnter += (sender, e) => buttonModernClose.BackColor = ModernTheme.CloseButtonHoverColor;
            buttonModernClose.MouseLeave += (sender, e) => buttonModernClose.BackColor = ModernTheme.TitleBarBackColor;
            buttonModernClose.Click += (sender, e) => form.Close();

            Button buttonModernMinimize = CreateModernTitleBarButton(
                "buttonModernMinimize",
                "−",
                new Point(form.ClientSize.Width - ModernTheme.TitleBarButtonSize.Width * 2, 0));

            buttonModernMinimize.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonModernMinimize.Visible = allowMinimize;
            buttonModernMinimize.Click += (sender, e) => form.WindowState = FormWindowState.Minimized;

            panelModernTitleBar.MouseDown += (sender, e) => BeginWindowDrag(form, e);
            labelModernTitle.MouseDown += (sender, e) => BeginWindowDrag(form, e);
            pictureBoxModernTitleIcon.MouseDown += (sender, e) => BeginWindowDrag(form, e);

            panelModernTitleBar.Controls.Add(pictureBoxModernTitleIcon);
            panelModernTitleBar.Controls.Add(labelModernTitle);
            panelModernTitleBar.Controls.Add(buttonModernMinimize);
            panelModernTitleBar.Controls.Add(buttonModernClose);

            form.Controls.Add(panelModernTitleBar);
            panelModernTitleBar.BringToFront();
        }

        private static Button CreateModernTitleBarButton(string name, string text, Point location)
        {
            Button button = new Button
            {
                Name = name,
                Text = text,
                Location = location,
                Size = ModernTheme.TitleBarButtonSize,
                FlatStyle = FlatStyle.Flat,
                BackColor = ModernTheme.TitleBarBackColor,
                ForeColor = ModernTheme.TextColor,
                Font = new Font(ModernTheme.FontFamilyName, 10F, FontStyle.Regular),
                Cursor = Cursors.Hand,
                TabStop = false,
                TextAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = ModernTheme.ControlHoverBackColor;
            button.FlatAppearance.MouseDownBackColor = ModernTheme.ControlBackColor;

            return button;
        }

        private static void BeginWindowDrag(Form form, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            ReleaseCapture();
            SendMessage(form.Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
        }

        private static void ApplyControl(Control control)
        {
            if (control is Button button)
            {
                if (!button.Name.StartsWith("buttonModern"))
                {
                    ModernTheme.ApplyButtonStyle(button);
                }
            }
            else if (control is Label label)
            {
                ModernTheme.ApplyLabelStyle(label);
            }
            else if (control is TextBox textBox)
            {
                ModernTheme.ApplyTextBoxStyle(textBox);
            }
            else if (control is ComboBox comboBox)
            {
                ModernTheme.ApplyComboBoxStyle(comboBox);
            }
            else if (control is CheckBox checkBox)
            {
                ModernTheme.ApplyCheckBoxStyle(checkBox);
            }
            else if (control is MenuStrip menuStrip)
            {
                ModernTheme.ApplyMenuStyle(menuStrip);
            }
            else if (control is Panel panel)
            {
                if (panel.Name != "panelModernTitleBar")
                {
                    panel.BackColor = ModernTheme.ControlBackColor;
                    panel.ForeColor = ModernTheme.TextColor;
                }
            }
            else if (control is CheckedListBox checkedListBox)
            {
                checkedListBox.BackColor = ModernTheme.ControlBackColor;
                checkedListBox.ForeColor = ModernTheme.TextColor;
                checkedListBox.BorderStyle = BorderStyle.FixedSingle;
                checkedListBox.Font = ModernTheme.DefaultFont;
            }
            else if (control is DataGridView dataGridView)
            {
                ApplyDataGridViewStyle(dataGridView);
            }

            foreach (Control child in control.Controls)
            {
                ApplyControl(child);
            }
        }

        private static void ApplyDataGridViewStyle(DataGridView dataGridView)
        {
            dataGridView.BackgroundColor = ModernTheme.WindowBackColor;
            dataGridView.GridColor = ModernTheme.ControlHoverBackColor;
            dataGridView.BorderStyle = BorderStyle.FixedSingle;
            dataGridView.EnableHeadersVisualStyles = false;
            dataGridView.DefaultCellStyle.BackColor = ModernTheme.ControlBackColor;
            dataGridView.DefaultCellStyle.ForeColor = ModernTheme.TextColor;
            dataGridView.DefaultCellStyle.SelectionBackColor = ModernTheme.AccentColor;
            dataGridView.DefaultCellStyle.SelectionForeColor = ModernTheme.DarkTextColor;
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = ModernTheme.TitleBarBackColor;
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = ModernTheme.TextColor;
            dataGridView.RowHeadersDefaultCellStyle.BackColor = ModernTheme.TitleBarBackColor;
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = ModernTheme.TextColor;
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(35, 55, 75);
        }
    }
}