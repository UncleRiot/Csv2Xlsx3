using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Csv2Xlsx3
{
    public partial class AboutForm : Form
    {
        private const string GithubRepositoryUrl = "https://github.com/UncleRiot/Csv2Xlsx3";
        private const string GithubLatestReleaseApiUrl = "https://api.github.com/repos/UncleRiot/Csv2Xlsx3/releases/latest";
        private const string KoFiUrl = "https://ko-fi.com/uncleriot";

        public AboutForm()
        {
            InitializeComponent();

            ModernFormStyler.Apply(this);
            ReplaceKoFiButtonWithImageOnlyButton();
            ApplyAboutLayout();
            ApplyAboutTheme();
            LoadEmbeddedApplicationImage("molotov.png");
            LoadEmbeddedKoFiImage("ko-fi.png");

            labelVersion.Text = "Version: " + GetCurrentVersion();
            _ = CheckForUpdatesAsync();
        }
        private void ReplaceKoFiButtonWithImageOnlyButton()
        {
            Control parent = btnKoFi.Parent;
            int childIndex = parent.Controls.GetChildIndex(btnKoFi);

            Button originalButton = btnKoFi;

            ImageOnlyButton imageOnlyButton = new ImageOnlyButton
            {
                Name = originalButton.Name,
                Location = originalButton.Location,
                Size = originalButton.Size,
                Anchor = originalButton.Anchor,
                TabIndex = originalButton.TabIndex,
                Cursor = Cursors.Hand
            };

            imageOnlyButton.Click += btnKoFi_Click;

            parent.Controls.Remove(originalButton);
            originalButton.Dispose();

            btnKoFi = imageOnlyButton;

            parent.Controls.Add(btnKoFi);
            parent.Controls.SetChildIndex(btnKoFi, childIndex);
        }
        private sealed class ImageOnlyButton : Button
        {
            public ImageOnlyButton()
            {
                SetStyle(
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.SupportsTransparentBackColor,
                    true);

                FlatStyle = FlatStyle.Flat;
                FlatAppearance.BorderSize = 0;
                TabStop = false;
                Text = string.Empty;
                UseVisualStyleBackColor = false;
                BackColor = Color.Transparent;
                ForeColor = Color.Transparent;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.Clear(Parent != null ? Parent.BackColor : BackColor);

                if (BackgroundImage == null)
                    return;

                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                float scale = Math.Min((float)Width / BackgroundImage.Width, (float)Height / BackgroundImage.Height);
                int imageWidth = (int)(BackgroundImage.Width * scale);
                int imageHeight = (int)(BackgroundImage.Height * scale);
                int x = (Width - imageWidth) / 2;
                int y = (Height - imageHeight) / 2;

                e.Graphics.DrawImage(BackgroundImage, x, y, imageWidth, imageHeight);
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                base.OnMouseLeave(e);
                Invalidate();
            }

            protected override bool ShowFocusCues
            {
                get { return false; }
            }
        }
        private void ApplyAboutLayout()
        {
            ClientSize = ModernTheme.AboutFormSize;
            MinimumSize = Size;
            MaximumSize = Size;

            int contentTop = ModernTheme.TitleBarHeight + ModernTheme.AboutContentTop;
            int textTop = ModernTheme.TitleBarHeight + ModernTheme.AboutTextTop;
            int textStep = ModernTheme.AboutTextLineHeight + ModernTheme.AboutTextLineGap;

            pictureBox1.Location = new Point(ModernTheme.AboutOuterMargin, contentTop);
            pictureBox1.Size = new Size(ModernTheme.AboutImageSize, ModernTheme.AboutImageSize);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            labelTitle.Location = new Point(ModernTheme.AboutTextLeft, textTop);
            labelTitle.Size = new Size(ModernTheme.AboutTextWidth, ModernTheme.AboutTextLineHeight);
            labelTitle.TextAlign = ContentAlignment.MiddleLeft;

            labelCopyright.Location = new Point(ModernTheme.AboutTextLeft, textTop + textStep);
            labelCopyright.Size = new Size(ModernTheme.AboutTextWidth, ModernTheme.AboutTextLineHeight);
            labelCopyright.TextAlign = ContentAlignment.MiddleLeft;

            labelVersion.Location = new Point(ModernTheme.AboutTextLeft, textTop + textStep * 2);
            labelVersion.Size = new Size(ModernTheme.AboutTextWidth, ModernTheme.AboutTextLineHeight);
            labelVersion.TextAlign = ContentAlignment.MiddleLeft;

            labelUpdateStatus.Location = new Point(ModernTheme.AboutTextLeft, textTop + textStep * 3);
            labelUpdateStatus.Size = new Size(ModernTheme.AboutTextWidth, ModernTheme.AboutTextLineHeight);
            labelUpdateStatus.TextAlign = ContentAlignment.MiddleLeft;

            linkLabelGithub.Location = new Point(ModernTheme.AboutTextLeft, textTop + textStep * 4);
            linkLabelGithub.Size = new Size(ModernTheme.AboutTextWidth, ModernTheme.AboutTextLineHeight);
            linkLabelGithub.TextAlign = ContentAlignment.MiddleLeft;

            labelInfo.Location = new Point(
                ModernTheme.AboutOuterMargin,
                ModernTheme.TitleBarHeight + ModernTheme.AboutInfoTop);
            labelInfo.Size = new Size(
                ClientSize.Width - ModernTheme.AboutOuterMargin * 2,
                ModernTheme.AboutInfoHeight);
            labelInfo.TextAlign = ContentAlignment.MiddleLeft;

            btnKoFi.Location = new Point(
                ModernTheme.AboutOuterMargin,
                ModernTheme.TitleBarHeight + ModernTheme.AboutKoFiTop);
            btnKoFi.Size = ModernTheme.AboutKoFiButtonSize;
            btnKoFi.Padding = Padding.Empty;
            btnKoFi.ImageAlign = ContentAlignment.MiddleCenter;
            btnKoFi.TextAlign = ContentAlignment.MiddleCenter;
            btnKoFi.Text = string.Empty;

            btnOk.Size = ModernTheme.AboutOkButtonSize;
            btnOk.Location = new Point(
                ClientSize.Width - ModernTheme.AboutOuterMargin - btnOk.Width,
                ClientSize.Height - ModernTheme.AboutOuterMargin - btnOk.Height);
        }

        private void ApplyAboutTheme()
        {
            BackColor = ModernTheme.WindowBackColor;
            ForeColor = ModernTheme.TextColor;

            labelTitle.ForeColor = ModernTheme.TextColor;
            labelCopyright.ForeColor = ModernTheme.TextColor;
            labelVersion.ForeColor = ModernTheme.TextColor;
            labelUpdateStatus.ForeColor = ModernTheme.TextColor;
            labelInfo.ForeColor = ModernTheme.TextColor;

            linkLabelGithub.LinkColor = ModernTheme.AccentColor;
            linkLabelGithub.ActiveLinkColor = ModernTheme.AccentHoverColor;
            linkLabelGithub.VisitedLinkColor = ModernTheme.AccentColor;

            btnKoFi.BackColor = Color.Transparent;
            btnKoFi.ForeColor = Color.Transparent;
            btnKoFi.FlatStyle = FlatStyle.Flat;
            btnKoFi.FlatAppearance.BorderSize = 0;
            btnKoFi.FlatAppearance.MouseOverBackColor = Color.Transparent;
            btnKoFi.FlatAppearance.MouseDownBackColor = Color.Transparent;
            btnKoFi.Font = ModernTheme.KoFiButtonFont;
            btnKoFi.Cursor = Cursors.Hand;
            btnKoFi.UseVisualStyleBackColor = false;
            btnKoFi.Text = string.Empty;
            btnKoFi.Image = null;
            btnKoFi.Region = null;

            ModernTheme.ApplyButtonStyle(btnOk);
        }

        private void LoadEmbeddedApplicationImage(string fileName)
        {
            using (Image image = LoadEmbeddedImage(fileName))
            {
                if (image != null)
                {
                    pictureBox1.Image = CreateCircularImage(image, pictureBox1.Width, pictureBox1.Height);
                }
            }
        }

        private void LoadEmbeddedKoFiImage(string fileName)
        {
            Image image = LoadEmbeddedImage(fileName);

            if (image != null)
            {
                if (btnKoFi.BackgroundImage != null)
                {
                    btnKoFi.BackgroundImage.Dispose();
                }

                btnKoFi.BackgroundImage = image;
                btnKoFi.BackgroundImageLayout = ImageLayout.Zoom;
                btnKoFi.Text = string.Empty;
                btnKoFi.Image = null;
            }
        }

        private Image LoadEmbeddedImage(string fileName)
        {
            Assembly assembly = typeof(AboutForm).Assembly;
            string resourceName = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(name => name.EndsWith("." + fileName, StringComparison.OrdinalIgnoreCase));

            if (resourceName == null)
                return null;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    return null;

                return Image.FromStream(stream);
            }
        }

        private Bitmap CreateCircularImage(Image sourceImage, int width, int height)
        {
            Bitmap output = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(output))
            {
                graphics.Clear(Color.Transparent);
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddEllipse(2, 2, width - 4, height - 4);
                    graphics.SetClip(path);

                    float scale = Math.Max((float)(width - 4) / sourceImage.Width, (float)(height - 4) / sourceImage.Height);
                    int scaledWidth = (int)(sourceImage.Width * scale);
                    int scaledHeight = (int)(sourceImage.Height * scale);
                    int x = (width - scaledWidth) / 2;
                    int y = (height - scaledHeight) / 2;

                    graphics.DrawImage(sourceImage, x, y, scaledWidth, scaledHeight);
                    graphics.ResetClip();
                }

                using (Pen borderPen = new Pen(ModernTheme.AccentColor, 2))
                {
                    graphics.DrawEllipse(borderPen, 2, 2, width - 4, height - 4);
                }
            }

            return output;
        }

        private string GetCurrentVersion()
        {
            string informationalVersion = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            if (!string.IsNullOrWhiteSpace(informationalVersion))
            {
                int metadataIndex = informationalVersion.IndexOf('+');
                if (metadataIndex >= 0)
                {
                    informationalVersion = informationalVersion.Substring(0, metadataIndex);
                }

                return informationalVersion.TrimStart('v', 'V');
            }

            Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return assemblyVersion != null ? assemblyVersion.ToString(3) : "0.0.0";
        }

        private bool TryParseVersion(string versionText, out Version version)
        {
            version = null;

            if (string.IsNullOrWhiteSpace(versionText))
                return false;

            string normalizedVersionText = versionText.Trim().TrimStart('v', 'V');

            int metadataIndex = normalizedVersionText.IndexOf('+');
            if (metadataIndex >= 0)
            {
                normalizedVersionText = normalizedVersionText.Substring(0, metadataIndex);
            }

            int prereleaseIndex = normalizedVersionText.IndexOf('-');
            if (prereleaseIndex >= 0)
            {
                normalizedVersionText = normalizedVersionText.Substring(0, prereleaseIndex);
            }

            return Version.TryParse(normalizedVersionText, out version);
        }

        private async Task CheckForUpdatesAsync()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Csv2Xlsx3/" + GetCurrentVersion());

                    string json = await httpClient.GetStringAsync(GithubLatestReleaseApiUrl);

                    using (JsonDocument document = JsonDocument.Parse(json))
                    {
                        if (!document.RootElement.TryGetProperty("tag_name", out JsonElement tagNameElement))
                        {
                            labelUpdateStatus.Text = "No new updates";
                            labelUpdateStatus.ForeColor = ModernTheme.TextColor;
                            labelUpdateStatus.Font = ModernTheme.DefaultFont;
                            labelUpdateStatus.Cursor = Cursors.Default;
                            labelUpdateStatus.Click -= labelUpdateStatus_Click;
                            return;
                        }

                        string latestVersionText = tagNameElement.GetString();

                        if (!TryParseVersion(GetCurrentVersion(), out Version currentVersion) ||
                            !TryParseVersion(latestVersionText, out Version latestVersion))
                        {
                            labelUpdateStatus.Text = "No new updates";
                            labelUpdateStatus.ForeColor = ModernTheme.TextColor;
                            labelUpdateStatus.Font = ModernTheme.DefaultFont;
                            labelUpdateStatus.Cursor = Cursors.Default;
                            labelUpdateStatus.Click -= labelUpdateStatus_Click;
                            return;
                        }

                        if (latestVersion > currentVersion)
                        {
                            labelUpdateStatus.Text = "Update available: " + latestVersionText;
                            labelUpdateStatus.ForeColor = ModernTheme.AccentColor;
                            labelUpdateStatus.Font = new Font(labelUpdateStatus.Font, FontStyle.Underline);
                            labelUpdateStatus.Cursor = Cursors.Hand;
                            labelUpdateStatus.Click -= labelUpdateStatus_Click;
                            labelUpdateStatus.Click += labelUpdateStatus_Click;
                        }
                        else
                        {
                            labelUpdateStatus.Text = "No new updates";
                            labelUpdateStatus.ForeColor = ModernTheme.TextColor;
                            labelUpdateStatus.Font = ModernTheme.DefaultFont;
                            labelUpdateStatus.Cursor = Cursors.Default;
                            labelUpdateStatus.Click -= labelUpdateStatus_Click;
                        }
                    }
                }
            }
            catch
            {
                labelUpdateStatus.Text = "No new updates";
                labelUpdateStatus.ForeColor = ModernTheme.TextColor;
                labelUpdateStatus.Font = ModernTheme.DefaultFont;
                labelUpdateStatus.Cursor = Cursors.Default;
                labelUpdateStatus.Click -= labelUpdateStatus_Click;
            }
        }
        private void labelUpdateStatus_Click(object sender, EventArgs e)
        {
            OpenUrl(GithubRepositoryUrl + "/releases/latest");
        }
        private void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void linkLabelGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenUrl(GithubRepositoryUrl);
        }

        private void btnKoFi_Click(object sender, EventArgs e)
        {
            OpenUrl(KoFiUrl);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnKoFi_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}