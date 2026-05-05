namespace Csv2Xlsx3
{
    partial class AboutForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelUpdateStatus;
        private System.Windows.Forms.LinkLabel linkLabelGithub;
        private System.Windows.Forms.Label labelInfo;
        private System.Windows.Forms.Button btnKoFi;
        private System.Windows.Forms.Button btnOk;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            if (disposing && pictureBox1 != null && pictureBox1.Image != null)
                pictureBox1.Image.Dispose();

            if (disposing && btnKoFi != null && btnKoFi.Image != null)
                btnKoFi.Image.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelUpdateStatus = new System.Windows.Forms.Label();
            this.linkLabelGithub = new System.Windows.Forms.LinkLabel();
            this.labelInfo = new System.Windows.Forms.Label();
            this.btnKoFi = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(20, 26);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(86, 86);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // labelTitle
            // 
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(131, 27);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(320, 21);
            this.labelTitle.TabIndex = 1;
            this.labelTitle.Text = "CSV2XLSX";
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelCopyright
            // 
            this.labelCopyright.Location = new System.Drawing.Point(131, 57);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(320, 20);
            this.labelCopyright.TabIndex = 2;
            this.labelCopyright.Text = "(c) Daniel Capilla";
            this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVersion
            // 
            this.labelVersion.Location = new System.Drawing.Point(131, 81);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(320, 20);
            this.labelVersion.TabIndex = 3;
            this.labelVersion.Text = "Version: 0.0.1";
            this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelUpdateStatus
            // 
            this.labelUpdateStatus.Location = new System.Drawing.Point(131, 105);
            this.labelUpdateStatus.Name = "labelUpdateStatus";
            this.labelUpdateStatus.Size = new System.Drawing.Size(320, 20);
            this.labelUpdateStatus.TabIndex = 4;
            this.labelUpdateStatus.Text = "Checking for updates...";
            this.labelUpdateStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // linkLabelGithub
            // 
            this.linkLabelGithub.Location = new System.Drawing.Point(131, 129);
            this.linkLabelGithub.Name = "linkLabelGithub";
            this.linkLabelGithub.Size = new System.Drawing.Size(320, 20);
            this.linkLabelGithub.TabIndex = 5;
            this.linkLabelGithub.TabStop = true;
            this.linkLabelGithub.Text = "https://github.com/UncleRiot/CSV2XLSX_v3";
            this.linkLabelGithub.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.linkLabelGithub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGithub_LinkClicked);
            // 
            // labelInfo
            // 
            this.labelInfo.Location = new System.Drawing.Point(20, 175);
            this.labelInfo.Name = "labelInfo";
            this.labelInfo.Size = new System.Drawing.Size(430, 39);
            this.labelInfo.TabIndex = 6;
            this.labelInfo.Text = "CSV2XLSX is free to use.\r\nIf this tool saves you time, you can support development here:";
            this.labelInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnKoFi
            // 
            this.btnKoFi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnKoFi.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnKoFi.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnKoFi.Location = new System.Drawing.Point(20, 221);
            this.btnKoFi.Name = "btnKoFi";
            this.btnKoFi.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.btnKoFi.Size = new System.Drawing.Size(176, 42);
            this.btnKoFi.TabIndex = 7;
            this.btnKoFi.Text = "   Buy me a coffee";
            this.btnKoFi.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnKoFi.UseVisualStyleBackColor = false;
            this.btnKoFi.Click += new System.EventHandler(this.btnKoFi_Click);
            this.btnKoFi.Paint += new System.Windows.Forms.PaintEventHandler(this.btnKoFi_Paint);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(385, 232);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 30);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // AboutForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 282);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.labelUpdateStatus);
            this.Controls.Add(this.linkLabelGithub);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.btnKoFi);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
        }
    }
}