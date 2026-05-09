namespace Csv2Xlsx3
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.CheckBox checkBoxSystray;
        private System.Windows.Forms.CheckBox checkBoxShell;
        private System.Windows.Forms.CheckBox checkBoxShellColumnSelection;
        private System.Windows.Forms.CheckBox checkBoxOnTop;
        private System.Windows.Forms.Label labelCsvDelimiter;
        private System.Windows.Forms.TextBox textBoxCsvDelimiter;
        private System.Windows.Forms.Label labelDelimiterHint;
        private System.Windows.Forms.Label labelLanguage;
        private System.Windows.Forms.ComboBox comboBoxLanguage;
        private System.Windows.Forms.Label labelConfigPath;
        private System.Windows.Forms.Label labelShellExeInfo;
        private System.Windows.Forms.Label labelShellExePath;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_toolTip != null)
                {
                    _toolTip.Dispose();
                    _toolTip = null;
                }

                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.checkBoxSystray = new System.Windows.Forms.CheckBox();
            this.checkBoxShell = new System.Windows.Forms.CheckBox();
            this.checkBoxShellColumnSelection = new System.Windows.Forms.CheckBox();
            this.checkBoxOnTop = new System.Windows.Forms.CheckBox();
            this.labelCsvDelimiter = new System.Windows.Forms.Label();
            this.textBoxCsvDelimiter = new System.Windows.Forms.TextBox();
            this.labelDelimiterHint = new System.Windows.Forms.Label();
            this.labelLanguage = new System.Windows.Forms.Label();
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.labelConfigPath = new System.Windows.Forms.Label();
            this.labelShellExeInfo = new System.Windows.Forms.Label();
            this.labelShellExePath = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // checkBoxSystray
            // 
            this.checkBoxSystray.AutoSize = true;
            this.checkBoxSystray.Location = new System.Drawing.Point(12, 12);
            this.checkBoxSystray.Name = "checkBoxSystray";
            this.checkBoxSystray.Size = new System.Drawing.Size(249, 19);
            this.checkBoxSystray.TabIndex = 0;
            this.checkBoxSystray.Text = "Minimize application to system tray";
            this.checkBoxSystray.UseVisualStyleBackColor = true;
            // 
            // checkBoxShell
            // 
            this.checkBoxShell.AutoSize = true;
            this.checkBoxShell.Location = new System.Drawing.Point(12, 37);
            this.checkBoxShell.Name = "checkBoxShell";
            this.checkBoxShell.Size = new System.Drawing.Size(307, 19);
            this.checkBoxShell.TabIndex = 1;
            this.checkBoxShell.Text = "Enable shell context menu for CSV files";
            this.checkBoxShell.UseVisualStyleBackColor = true;
            // 
            // checkBoxShellColumnSelection
            // 
            this.checkBoxShellColumnSelection.AutoSize = true;
            this.checkBoxShellColumnSelection.Location = new System.Drawing.Point(35, 62);
            this.checkBoxShellColumnSelection.Name = "checkBoxShellColumnSelection";
            this.checkBoxShellColumnSelection.Size = new System.Drawing.Size(248, 19);
            this.checkBoxShellColumnSelection.TabIndex = 2;
            this.checkBoxShellColumnSelection.Text = "Column selection for context menu usage";
            this.checkBoxShellColumnSelection.UseVisualStyleBackColor = true;
            // 
            // checkBoxOnTop
            // 
            this.checkBoxOnTop.AutoSize = true;
            this.checkBoxOnTop.Location = new System.Drawing.Point(12, 92);
            this.checkBoxOnTop.Name = "checkBoxOnTop";
            this.checkBoxOnTop.Size = new System.Drawing.Size(214, 19);
            this.checkBoxOnTop.TabIndex = 3;
            this.checkBoxOnTop.Text = "Always on top";
            this.checkBoxOnTop.UseVisualStyleBackColor = true;
            // 
            // labelCsvDelimiter
            // 
            this.labelCsvDelimiter.AutoSize = true;
            this.labelCsvDelimiter.Location = new System.Drawing.Point(12, 132);
            this.labelCsvDelimiter.Name = "labelCsvDelimiter";
            this.labelCsvDelimiter.Size = new System.Drawing.Size(96, 15);
            this.labelCsvDelimiter.TabIndex = 4;
            this.labelCsvDelimiter.Text = "CSV delimiter";
            // 
            // textBoxCsvDelimiter
            // 
            this.textBoxCsvDelimiter.Location = new System.Drawing.Point(134, 129);
            this.textBoxCsvDelimiter.Name = "textBoxCsvDelimiter";
            this.textBoxCsvDelimiter.Size = new System.Drawing.Size(48, 23);
            this.textBoxCsvDelimiter.TabIndex = 5;
            // 
            // labelDelimiterHint
            // 
            this.labelDelimiterHint.AutoSize = true;
            this.labelDelimiterHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelDelimiterHint.Location = new System.Drawing.Point(190, 132);
            this.labelDelimiterHint.Name = "labelDelimiterHint";
            this.labelDelimiterHint.Size = new System.Drawing.Size(196, 15);
            this.labelDelimiterHint.TabIndex = 6;
            this.labelDelimiterHint.Text = "\",\" and \";\" are detected automatically";
            // 
            // labelLanguage
            // 
            this.labelLanguage.AutoSize = true;
            this.labelLanguage.Location = new System.Drawing.Point(12, 164);
            this.labelLanguage.Name = "labelLanguage";
            this.labelLanguage.Size = new System.Drawing.Size(59, 15);
            this.labelLanguage.TabIndex = 7;
            this.labelLanguage.Text = "Language";
            // 
            // comboBoxLanguage
            // 
            this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLanguage.FormattingEnabled = true;
            this.comboBoxLanguage.Location = new System.Drawing.Point(134, 161);
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.Size = new System.Drawing.Size(180, 23);
            this.comboBoxLanguage.TabIndex = 8;
            // 
            // labelConfigPath
            // 
            this.labelConfigPath.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelConfigPath.Location = new System.Drawing.Point(12, 198);
            this.labelConfigPath.Name = "labelConfigPath";
            this.labelConfigPath.Size = new System.Drawing.Size(536, 38);
            this.labelConfigPath.TabIndex = 9;
            this.labelConfigPath.Text = "Path is set at runtime";
            // 
            // labelShellExeInfo
            // 
            this.labelShellExeInfo.AutoSize = true;
            this.labelShellExeInfo.Location = new System.Drawing.Point(12, 250);
            this.labelShellExeInfo.Name = "labelShellExeInfo";
            this.labelShellExeInfo.Size = new System.Drawing.Size(211, 15);
            this.labelShellExeInfo.TabIndex = 10;
            this.labelShellExeInfo.Text = "Shell context menu points to:";
            // 
            // labelShellExePath
            // 
            this.labelShellExePath.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelShellExePath.Location = new System.Drawing.Point(12, 270);
            this.labelShellExePath.Name = "labelShellExePath";
            this.labelShellExePath.Size = new System.Drawing.Size(536, 34);
            this.labelShellExePath.TabIndex = 11;
            this.labelShellExePath.Text = "Path is set at runtime";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(377, 320);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 12;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(458, 320);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(90, 23);
            this.buttonCancel.TabIndex = 13;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(560, 355);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.labelShellExePath);
            this.Controls.Add(this.labelShellExeInfo);
            this.Controls.Add(this.labelConfigPath);
            this.Controls.Add(this.comboBoxLanguage);
            this.Controls.Add(this.labelLanguage);
            this.Controls.Add(this.labelDelimiterHint);
            this.Controls.Add(this.textBoxCsvDelimiter);
            this.Controls.Add(this.labelCsvDelimiter);
            this.Controls.Add(this.checkBoxOnTop);
            this.Controls.Add(this.checkBoxShellColumnSelection);
            this.Controls.Add(this.checkBoxShell);
            this.Controls.Add(this.checkBoxSystray);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}