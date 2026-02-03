namespace DvdSubExtractor
{
    partial class ChooseSubtitlesStep
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
            if(disposing && (this.pictureBitmap != null))
            {
                this.pictureBitmap.Dispose();
                this.pictureBitmap = null;
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            subtitleListBox = new ListBox();
            previousButton = new Button();
            nextButton = new Button();
            binFileComboBox = new ComboBox();
            label1 = new Label();
            browseButton = new Button();
            openFileDialog1 = new OpenFileDialog();
            indexLabel = new Label();
            label2 = new Label();
            indexUpDown = new NumericUpDown();
            label3 = new Label();
            forcedCheckBox = new CheckBox();
            subtitlePictureBox = new PictureBox();
            scaleBitmapCheckBox = new CheckBox();
            toolTip1 = new ToolTip(components);
            ((System.ComponentModel.ISupportInitialize)indexUpDown).BeginInit();
            ((System.ComponentModel.ISupportInitialize)subtitlePictureBox).BeginInit();
            SuspendLayout();
            // 
            // subtitleListBox
            // 
            subtitleListBox.FormattingEnabled = true;
            subtitleListBox.Location = new Point(53, 65);
            subtitleListBox.Margin = new Padding(3);
            subtitleListBox.Name = "subtitleListBox";
            subtitleListBox.Size = new Size(310, 105);
            subtitleListBox.TabIndex = 1;
            toolTip1.SetToolTip(subtitleListBox, "Subtitle tracks in data file");
            subtitleListBox.SelectedIndexChanged += subtitleListBox_SelectedIndexChanged;
            // 
            // previousButton
            // 
            previousButton.Location = new Point(407, 97);
            previousButton.Margin = new Padding(3);
            previousButton.Name = "previousButton";
            previousButton.Size = new Size(105, 42);
            previousButton.TabIndex = 2;
            previousButton.Text = "Previous Subtitle";
            toolTip1.SetToolTip(previousButton, "Display previous subtitle in the current data file and track");
            previousButton.UseVisualStyleBackColor = true;
            previousButton.Click += previousButton_Click;
            // 
            // nextButton
            // 
            nextButton.Location = new Point(524, 97);
            nextButton.Margin = new Padding(3);
            nextButton.Name = "nextButton";
            nextButton.Size = new Size(105, 42);
            nextButton.TabIndex = 3;
            nextButton.Text = "Next Subtitle";
            toolTip1.SetToolTip(nextButton, "Display next subtitle in the current data file and track");
            nextButton.UseVisualStyleBackColor = true;
            nextButton.Click += nextButton_Click;
            // 
            // binFileComboBox
            // 
            binFileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            binFileComboBox.FormattingEnabled = true;
            binFileComboBox.Location = new Point(53, 31);
            binFileComboBox.Margin = new Padding(3);
            binFileComboBox.Name = "binFileComboBox";
            binFileComboBox.Size = new Size(433, 22);
            binFileComboBox.TabIndex = 4;
            toolTip1.SetToolTip(binFileComboBox, "Current subtitle data file");
            binFileComboBox.SelectedIndexChanged += binFileComboBox_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(53, 11);
            label1.Margin = new Padding(3, 0, 3, 0);
            label1.Name = "label1";
            label1.Size = new Size(97, 17);
            label1.TabIndex = 5;
            label1.Text = "Subtitle Data File";
            // 
            // browseButton
            // 
            browseButton.Location = new Point(524, 31);
            browseButton.Margin = new Padding(3);
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(105, 23);
            browseButton.TabIndex = 6;
            browseButton.Text = "Browse...";
            toolTip1.SetToolTip(browseButton, "Select new subtitle data file(s)");
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += browseButton_Click;
            // 
            // indexLabel
            // 
            indexLabel.AutoSize = true;
            indexLabel.Location = new Point(517, 151);
            indexLabel.Margin = new Padding(3, 0, 3, 0);
            indexLabel.Name = "indexLabel";
            indexLabel.Size = new Size(0, 17);
            indexLabel.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(404, 69);
            label2.Margin = new Padding(3, 0, 3, 0);
            label2.Name = "label2";
            label2.Size = new Size(175, 17);
            label2.TabIndex = 8;
            label2.Text = "Select the Subtitle Track to OCR";
            // 
            // indexUpDown
            // 
            indexUpDown.Location = new Point(450, 148);
            indexUpDown.Margin = new Padding(3);
            indexUpDown.Name = "indexUpDown";
            indexUpDown.Size = new Size(60, 20);
            indexUpDown.TabIndex = 9;
            toolTip1.SetToolTip(indexUpDown, "Current subtitle index within the track");
            indexUpDown.ValueChanged += indexUpDown_ValueChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(407, 148);
            label3.Margin = new Padding(3, 0, 3, 0);
            label3.Name = "label3";
            label3.Size = new Size(37, 17);
            label3.TabIndex = 10;
            label3.Text = "Index";
            // 
            // forcedCheckBox
            // 
            forcedCheckBox.AutoSize = true;
            forcedCheckBox.Enabled = false;
            forcedCheckBox.Location = new Point(665, 98);
            forcedCheckBox.Margin = new Padding(3);
            forcedCheckBox.Name = "forcedCheckBox";
            forcedCheckBox.Size = new Size(86, 19);
            forcedCheckBox.TabIndex = 11;
            forcedCheckBox.Text = "Forced Only";
            toolTip1.SetToolTip(forcedCheckBox, "Show only those subtitles with the Forced attribute set");
            forcedCheckBox.UseVisualStyleBackColor = true;
            forcedCheckBox.CheckedChanged += forcedCheckBox_CheckedChanged;
            // 
            // subtitlePictureBox
            // 
            subtitlePictureBox.BackColor = Color.DimGray;
            subtitlePictureBox.Location = new Point(0, 182);
            subtitlePictureBox.Margin = new Padding(3);
            subtitlePictureBox.Name = "subtitlePictureBox";
            subtitlePictureBox.Size = new Size(839, 480);
            subtitlePictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            subtitlePictureBox.TabIndex = 0;
            subtitlePictureBox.TabStop = false;
            // 
            // scaleBitmapCheckBox
            // 
            scaleBitmapCheckBox.AutoSize = true;
            scaleBitmapCheckBox.Location = new Point(665, 121);
            scaleBitmapCheckBox.Margin = new Padding(3);
            scaleBitmapCheckBox.Name = "scaleBitmapCheckBox";
            scaleBitmapCheckBox.Size = new Size(98, 19);
            scaleBitmapCheckBox.TabIndex = 12;
            scaleBitmapCheckBox.Text = "Scale to Video";
            toolTip1.SetToolTip(scaleBitmapCheckBox, "Show the subtitle as it would appear on the entire video frame instead of just the bounding subtitle rectangle");
            scaleBitmapCheckBox.UseVisualStyleBackColor = true;
            scaleBitmapCheckBox.CheckedChanged += scaleBitmapCheckBox_CheckedChanged;
            // 
            // ChooseSubtitlesStep
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(scaleBitmapCheckBox);
            Controls.Add(forcedCheckBox);
            Controls.Add(label3);
            Controls.Add(indexUpDown);
            Controls.Add(label2);
            Controls.Add(indexLabel);
            Controls.Add(browseButton);
            Controls.Add(label1);
            Controls.Add(binFileComboBox);
            Controls.Add(nextButton);
            Controls.Add(previousButton);
            Controls.Add(subtitleListBox);
            Controls.Add(subtitlePictureBox);
            Margin = new Padding(3);
            Name = "ChooseSubtitlesStep";
            Size = new Size(842, 662);
            Load += ChooseSubtitlesStep_Load;
            ((System.ComponentModel.ISupportInitialize)indexUpDown).EndInit();
            ((System.ComponentModel.ISupportInitialize)subtitlePictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox subtitlePictureBox;
        private System.Windows.Forms.ListBox subtitleListBox;
        private System.Windows.Forms.Button previousButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.ComboBox binFileComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label indexLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown indexUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox forcedCheckBox;
        private System.Windows.Forms.CheckBox scaleBitmapCheckBox;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
