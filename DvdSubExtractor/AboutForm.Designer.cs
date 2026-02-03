namespace DvdSubExtractor
{
    partial class AboutForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            okButton = new Button();
            licenseButton = new Button();
            aboutTextBox = new RichTextBox();
            SuspendLayout();
            // 
            // okButton
            // 
            okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            okButton.DialogResult = DialogResult.OK;
            okButton.Location = new Point(101, 137);
            okButton.Margin = new Padding(3);
            okButton.Name = "okButton";
            okButton.Size = new Size(83, 23);
            okButton.TabIndex = 0;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            // 
            // licenseButton
            // 
            licenseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            licenseButton.Location = new Point(12, 137);
            licenseButton.Margin = new Padding(3);
            licenseButton.Name = "licenseButton";
            licenseButton.Size = new Size(83, 23);
            licenseButton.TabIndex = 2;
            licenseButton.Text = "License...";
            licenseButton.UseVisualStyleBackColor = true;
            licenseButton.Click += licenseButton_Click;
            // 
            // aboutTextBox
            // 
            aboutTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            aboutTextBox.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            aboutTextBox.Location = new Point(12, 12);
            aboutTextBox.Margin = new Padding(3);
            aboutTextBox.Name = "aboutTextBox";
            aboutTextBox.Size = new Size(354, 115);
            aboutTextBox.TabIndex = 1;
            aboutTextBox.Text = "";
            // 
            // AboutForm
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(380, 168);
            Controls.Add(aboutTextBox);
            Controls.Add(licenseButton);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3);
            Name = "AboutForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "About DVD Subtitle Extractor";
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button licenseButton;
        private System.Windows.Forms.RichTextBox aboutTextBox;
    }
}