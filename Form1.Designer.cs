namespace FileEncryptorApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

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
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnStartEncryption = new System.Windows.Forms.Button();
            this.btnStartDecryption = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(12, 12);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.ReadOnly = true;
            this.txtFilePath.Size = new System.Drawing.Size(379, 20);
            this.txtFilePath.TabIndex = 0;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(397, 10);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(75, 23);
            this.btnSelectFile.TabIndex = 1;
            this.btnSelectFile.Text = "Огляд...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtKey
            // 
            this.txtKey.Location = new System.Drawing.Point(12, 51);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(460, 20);
            this.txtKey.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Ключ шифрування:";
            // 
            // btnStartEncryption
            // 
            this.btnStartEncryption.Location = new System.Drawing.Point(12, 77);
            this.btnStartEncryption.Name = "btnStartEncryption";
            this.btnStartEncryption.Size = new System.Drawing.Size(227, 23);
            this.btnStartEncryption.TabIndex = 4;
            this.btnStartEncryption.Text = "Зашифрувати";
            this.btnStartEncryption.UseVisualStyleBackColor = true;
            this.btnStartEncryption.Click += new System.EventHandler(this.btnStartEncryption_Click);
            // 
            // btnStartDecryption
            // 
            this.btnStartDecryption.Location = new System.Drawing.Point(245, 77);
            this.btnStartDecryption.Name = "btnStartDecryption";
            this.btnStartDecryption.Size = new System.Drawing.Size(227, 23);
            this.btnStartDecryption.TabIndex = 5;
            this.btnStartDecryption.Text = "Розшифрувати";
            this.btnStartDecryption.UseVisualStyleBackColor = true;
            this.btnStartDecryption.Click += new System.EventHandler(this.btnStartDecryption_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 106);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(460, 23);
            this.progressBar.TabIndex = 6;
            // 
            // lblProgress
            // 
            this.lblProgress.AutoSize = true;
            this.lblProgress.Location = new System.Drawing.Point(12, 132);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(21, 13);
            this.lblProgress.TabIndex = 7;
            this.lblProgress.Text = "0%";
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(242, 132);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(84, 13);
            this.lblTime.TabIndex = 8;
            this.lblTime.Text = "Час: 0:00:00";
            // 
            // btnCancel
            // 
            this.btnCancel.Enabled = false;
            this.btnCancel.Location = new System.Drawing.Point(397, 132);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Відміна";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 166);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnStartDecryption);
            this.Controls.Add(this.btnStartEncryption);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtKey);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.txtFilePath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Шифрування файлів";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnStartEncryption;
        private System.Windows.Forms.Button btnStartDecryption;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Button btnCancel;
    }
}