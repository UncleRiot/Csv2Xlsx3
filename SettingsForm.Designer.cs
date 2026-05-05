namespace Csv2Xlsx3
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.CheckBox checkBoxSystray;
        private System.Windows.Forms.CheckBox checkBoxShell;
        private System.Windows.Forms.CheckBox checkBoxOnTop;
        private System.Windows.Forms.Label labelShellInfo;
        private System.Windows.Forms.Label labelCsvDelimiter;
        private System.Windows.Forms.TextBox textBoxCsvDelimiter;
        private System.Windows.Forms.Label labelDelimiterHint;
        private System.Windows.Forms.Label labelConfigPath;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.checkBoxSystray = new System.Windows.Forms.CheckBox();
            this.checkBoxShell = new System.Windows.Forms.CheckBox();
            this.checkBoxOnTop = new System.Windows.Forms.CheckBox();
            this.labelShellInfo = new System.Windows.Forms.Label();
            this.labelCsvDelimiter = new System.Windows.Forms.Label();
            this.textBoxCsvDelimiter = new System.Windows.Forms.TextBox();
            this.labelDelimiterHint = new System.Windows.Forms.Label();
            this.labelConfigPath = new System.Windows.Forms.Label();
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
            this.checkBoxSystray.Text = "Programm bei Minimierung in den Systray";
            this.checkBoxSystray.UseVisualStyleBackColor = true;
            // 
            // checkBoxShell
            // 
            this.checkBoxShell.AutoSize = true;
            this.checkBoxShell.Location = new System.Drawing.Point(12, 37);
            this.checkBoxShell.Name = "checkBoxShell";
            this.checkBoxShell.Size = new System.Drawing.Size(307, 19);
            this.checkBoxShell.TabIndex = 1;
            this.checkBoxShell.Text = "Shell-Kontextmenü für CSV-Dateien aktivieren";
            this.checkBoxShell.UseVisualStyleBackColor = true;
            // 
            // checkBoxOnTop
            // 
            this.checkBoxOnTop.AutoSize = true;
            this.checkBoxOnTop.Location = new System.Drawing.Point(12, 62);
            this.checkBoxOnTop.Name = "checkBoxOnTop";
            this.checkBoxOnTop.Size = new System.Drawing.Size(214, 19);
            this.checkBoxOnTop.TabIndex = 2;
            this.checkBoxOnTop.Text = "Programm immer im Vordergrund";
            this.checkBoxOnTop.UseVisualStyleBackColor = true;
            // 
            // labelShellInfo
            // 
            this.labelShellInfo.Location = new System.Drawing.Point(12, 92);
            this.labelShellInfo.Name = "labelShellInfo";
            this.labelShellInfo.Size = new System.Drawing.Size(470, 46);
            this.labelShellInfo.TabIndex = 3;
            this.labelShellInfo.Text = "Das Shell-Kontextmenü ermöglicht die Konvertierung direkt per Rechtsklick auf CSV-Dateien im Windows-Explorer.";
            // 
            // labelCsvDelimiter
            // 
            this.labelCsvDelimiter.AutoSize = true;
            this.labelCsvDelimiter.Location = new System.Drawing.Point(12, 152);
            this.labelCsvDelimiter.Name = "labelCsvDelimiter";
            this.labelCsvDelimiter.Size = new System.Drawing.Size(96, 15);
            this.labelCsvDelimiter.TabIndex = 4;
            this.labelCsvDelimiter.Text = "CSV-Trennzeichen";
            // 
            // textBoxCsvDelimiter
            // 
            this.textBoxCsvDelimiter.Location = new System.Drawing.Point(134, 149);
            this.textBoxCsvDelimiter.Name = "textBoxCsvDelimiter";
            this.textBoxCsvDelimiter.Size = new System.Drawing.Size(48, 23);
            this.textBoxCsvDelimiter.TabIndex = 5;
            // 
            // labelDelimiterHint
            // 
            this.labelDelimiterHint.AutoSize = true;
            this.labelDelimiterHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelDelimiterHint.Location = new System.Drawing.Point(190, 152);
            this.labelDelimiterHint.Name = "labelDelimiterHint";
            this.labelDelimiterHint.Size = new System.Drawing.Size(196, 15);
            this.labelDelimiterHint.TabIndex = 6;
            this.labelDelimiterHint.Text = "\",\" und \";\" werden automatisch erkannt";
            // 
            // labelConfigPath
            // 
            this.labelConfigPath.ForeColor = System.Drawing.SystemColors.GrayText;
            this.labelConfigPath.Location = new System.Drawing.Point(12, 182);
            this.labelConfigPath.Name = "labelConfigPath";
            this.labelConfigPath.Size = new System.Drawing.Size(470, 38);
            this.labelConfigPath.TabIndex = 7;
            this.labelConfigPath.Text = "Pfad wird zur Laufzeit gesetzt";
            // 
            // buttonOk
            // 
            this.buttonOk.Location = new System.Drawing.Point(326, 230);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 8;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(407, 230);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(90, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Abbrechen";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.buttonOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(509, 265);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.labelConfigPath);
            this.Controls.Add(this.labelDelimiterHint);
            this.Controls.Add(this.textBoxCsvDelimiter);
            this.Controls.Add(this.labelCsvDelimiter);
            this.Controls.Add(this.labelShellInfo);
            this.Controls.Add(this.checkBoxOnTop);
            this.Controls.Add(this.checkBoxShell);
            this.Controls.Add(this.checkBoxSystray);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Einstellungen";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}