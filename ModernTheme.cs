using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Csv2Xlsx3
{
    public static class ModernTheme
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public static readonly Color WindowBackColor = Color.FromArgb(27, 40, 56);
        public static readonly Color TitleBarBackColor = Color.FromArgb(23, 26, 33);
        public static readonly Color ControlBackColor = Color.FromArgb(42, 71, 94);
        public static readonly Color ControlHoverBackColor = Color.FromArgb(55, 90, 120);
        public static readonly Color AccentColor = Color.FromArgb(102, 192, 244);
        public static readonly Color AccentHoverColor = Color.FromArgb(143, 212, 255);
        public static readonly Color TextColor = Color.FromArgb(199, 213, 224);
        public static readonly Color DarkTextColor = Color.FromArgb(23, 26, 33);
        public static readonly Color DisabledTextColor = Color.FromArgb(120, 130, 138);
        public static readonly Color WindowBorderColor = Color.FromArgb(102, 192, 244);
        public static readonly Color CloseButtonHoverColor = Color.FromArgb(196, 43, 28);

        public static readonly Size MainWindowMinimumSize = new Size(220, 220);

        public const string ApplicationIconResourceFileName = "CSVtoXLSX.png";

        public const int WindowBorderWidth = 1;
        public const int TitleBarHeight = 34;
        public const int TitleBarIconLeft = 10;
        public const int TitleBarIconTop = 7;
        public const int TitleBarIconSize = 20;
        public const int TitleBarTextLeft = 38;
        public const float TitleFontSize = 9F;
        public const string FontFamilyName = "Segoe UI";

        public const int AboutOuterMargin = 28;
        public const int AboutContentTop = 26;
        public const int AboutImageSize = 96;
        public const int AboutTextLeft = 150;
        public const int AboutTextTop = 28;
        public const int AboutTextWidth = 320;
        public const int AboutTextLineHeight = 20;
        public const int AboutTextLineGap = 10;
        public const int AboutInfoTop = 176;
        public const int AboutInfoHeight = 42;
        public const int AboutKoFiTop = 222;

        public const int MainWindowResizeGripSize = 22;

        public static readonly Size TitleBarButtonSize = new Size(46, 34);
        public static readonly Size AboutFormSize = new Size(500, 340);
        public static readonly Size AboutKoFiButtonSize = new Size(180, 42);
        public static readonly Size AboutOkButtonSize = new Size(75, 30);

        public static readonly Font DefaultFont = new Font(FontFamilyName, 9F, FontStyle.Regular);
        public static readonly Font HeaderFont = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        public static readonly Font KoFiButtonFont = new Font(FontFamilyName, 9F, FontStyle.Bold);

        public static void ApplyApplicationIcon(Form form)
        {
            Icon icon = CreateApplicationIcon();

            if (icon != null)
            {
                form.Icon = icon;
            }
        }

        public static void ApplyApplicationIcon(NotifyIcon notifyIcon)
        {
            Icon icon = CreateApplicationIcon();

            if (icon != null)
            {
                notifyIcon.Icon = icon;
            }
        }

        public static Icon CreateApplicationIcon()
        {
            using (Image image = LoadEmbeddedImage(ApplicationIconResourceFileName))
            {
                if (image != null)
                {
                    return CreateIconFromImage(image);
                }
            }

            Icon executableIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            return executableIcon != null ? (Icon)executableIcon.Clone() : null;
        }

        private static Image LoadEmbeddedImage(string fileName)
        {
            System.Reflection.Assembly assembly = typeof(ModernTheme).Assembly;
            string resourceName = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(name => name.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                return null;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    return null;

                using (Image image = Image.FromStream(stream))
                {
                    return new Bitmap(image);
                }
            }
        }

        private static Icon CreateIconFromImage(Image image)
        {
            using (Bitmap bitmap = new Bitmap(image, new Size(256, 256)))
            {
                IntPtr hIcon = bitmap.GetHicon();

                try
                {
                    using (Icon icon = Icon.FromHandle(hIcon))
                    {
                        return (Icon)icon.Clone();
                    }
                }
                finally
                {
                    DestroyIcon(hIcon);
                }
            }
        }

        public static void ApplyButtonStyle(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = AccentColor;
            button.BackColor = ControlBackColor;
            button.ForeColor = TextColor;
            button.Font = DefaultFont;
            button.Cursor = Cursors.Hand;
            button.UseVisualStyleBackColor = false;

            button.MouseEnter += (sender, e) => button.BackColor = ControlHoverBackColor;
            button.MouseLeave += (sender, e) => button.BackColor = ControlBackColor;
        }

        public static void ApplyTextBoxStyle(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = ControlBackColor;
            textBox.ForeColor = TextColor;
            textBox.Font = DefaultFont;
        }

        public static void ApplyCheckBoxStyle(CheckBox checkBox)
        {
            checkBox.BackColor = WindowBackColor;
            checkBox.ForeColor = checkBox.Enabled ? TextColor : DisabledTextColor;
            checkBox.Font = DefaultFont;
            checkBox.UseVisualStyleBackColor = false;

            checkBox.EnabledChanged += (sender, e) =>
            {
                checkBox.ForeColor = checkBox.Enabled ? TextColor : DisabledTextColor;
            };
        }

        public static void ApplyLabelStyle(Label label)
        {
            label.BackColor = Color.Transparent;
            label.ForeColor = TextColor;
            label.Font = DefaultFont;
        }

        public static void ApplyComboBoxStyle(ComboBox comboBox)
        {
            comboBox.BackColor = ControlBackColor;
            comboBox.ForeColor = TextColor;
            comboBox.Font = DefaultFont;
            comboBox.FlatStyle = FlatStyle.Flat;
        }

        public static void ApplyMenuStyle(MenuStrip menuStrip)
        {
            menuStrip.BackColor = TitleBarBackColor;
            menuStrip.ForeColor = TextColor;
            menuStrip.Font = DefaultFont;
            menuStrip.RenderMode = ToolStripRenderMode.Professional;
            menuStrip.Renderer = new ToolStripProfessionalRenderer(new ModernColorTable());

            foreach (ToolStripMenuItem item in menuStrip.Items)
            {
                ApplyMenuItemStyle(item);
            }
        }

        private static void ApplyMenuItemStyle(ToolStripMenuItem item)
        {
            item.BackColor = TitleBarBackColor;
            item.ForeColor = TextColor;
            item.Padding = new Padding(4, 0, 4, 0);

            if (item.DropDown is ToolStripDropDownMenu dropDownMenu)
            {
                dropDownMenu.ShowImageMargin = false;
                dropDownMenu.ShowCheckMargin = false;
            }

            foreach (ToolStripItem child in item.DropDownItems)
            {
                child.BackColor = TitleBarBackColor;
                child.ForeColor = TextColor;
                child.Padding = new Padding(4, 0, 4, 0);

                if (child is ToolStripMenuItem childMenuItem)
                {
                    ApplyMenuItemStyle(childMenuItem);
                }
            }
        }

        public static bool IsInMainWindowResizeGripArea(Size clientSize, Point clientPoint)
        {
            return clientPoint.X >= clientSize.Width - MainWindowResizeGripSize &&
                   clientPoint.Y >= clientSize.Height - MainWindowResizeGripSize;
        }

        public static void DrawMainWindowResizeGrip(Graphics graphics, Size clientSize)
        {
            using (Pen pen = new Pen(AccentColor, 2F))
            {
                int right = clientSize.Width - 7;
                int bottom = clientSize.Height - 7;

                graphics.DrawLine(pen, right - 14, bottom, right, bottom - 14);
                graphics.DrawLine(pen, right - 9, bottom, right, bottom - 9);
                graphics.DrawLine(pen, right - 4, bottom, right, bottom - 4);
            }
        }

        public static DialogResult ShowMessage(string text, string caption, MessageBoxIcon icon)
        {
            return ShowMessage(null, text, caption, icon);
        }

        public static DialogResult ShowMessage(IWin32Window owner, string text, string caption, MessageBoxIcon icon)
        {
            using (Form dialog = new Form())
            {
                dialog.Text = caption;
                dialog.StartPosition = owner == null ? FormStartPosition.CenterScreen : FormStartPosition.CenterParent;
                dialog.ClientSize = new Size(420, 170);
                dialog.MinimumSize = dialog.Size;
                dialog.MaximumSize = dialog.Size;
                dialog.MaximizeBox = false;
                dialog.MinimizeBox = false;
                dialog.ShowInTaskbar = false;

                Label labelMessage = new Label
                {
                    AutoSize = false,
                    Location = new Point(24, 24),
                    Size = new Size(372, 70),
                    Text = text,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                Button buttonOk = new Button
                {
                    Text = LanguageManager.T("Button.OK"),
                    Size = new Size(75, 30),
                    Location = new Point(321, 116),
                    DialogResult = DialogResult.OK
                };

                dialog.Controls.Add(labelMessage);
                dialog.Controls.Add(buttonOk);
                dialog.AcceptButton = buttonOk;

                ApplyLabelStyle(labelMessage);
                ApplyButtonStyle(buttonOk);
                ModernFormStyler.Apply(dialog);

                return owner == null ? dialog.ShowDialog() : dialog.ShowDialog(owner);
            }
        }

        private sealed class ModernColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => ControlHoverBackColor;
            public override Color MenuItemBorder => AccentColor;
            public override Color MenuItemSelectedGradientBegin => ControlHoverBackColor;
            public override Color MenuItemSelectedGradientEnd => ControlHoverBackColor;
            public override Color MenuItemPressedGradientBegin => ControlBackColor;
            public override Color MenuItemPressedGradientEnd => ControlBackColor;
            public override Color ToolStripDropDownBackground => TitleBarBackColor;
            public override Color ImageMarginGradientBegin => TitleBarBackColor;
            public override Color ImageMarginGradientMiddle => TitleBarBackColor;
            public override Color ImageMarginGradientEnd => TitleBarBackColor;
        }
    }
}