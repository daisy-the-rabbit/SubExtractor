namespace DvdSubExtractor
{
    partial class OcrBlocksStep
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            unknownCharacterButton = new Button();
            label3 = new Label();
            doneSplitButton = new Button();
            cancelSplitButton = new Button();
            beginSplitButton = new Button();
            splitHelpLabel = new Label();
            label4 = new Label();
            label2 = new Label();
            label5 = new Label();
            label6 = new Label();
            undoButton = new Button();
            tryDifferentPaletteButton = new Button();
            subtitleIndexLabel = new Label();
            startOverSubButton = new Button();
            startOverMovieButton = new Button();
            reviewButton = new Button();
            ignoreBaselineCheckBox = new CheckBox();
            progressBar1 = new ProgressBar();
            retryButton = new Button();
            paletteIndexLabel = new Label();
            commentButton = new Button();
            dvdLabel = new Label();
            manualEntryTextBox = new TextBox();
            manualCharLabel = new Label();
            toolTip1 = new ToolTip(components);
            manualItalicCheckBox = new CheckBox();
            manualWaitEnterCheckBox = new CheckBox();
            triangleFill4 = new TriangleFill();
            triangleFill3 = new TriangleFill();
            triangleFill2 = new TriangleFill();
            triangleFill1 = new TriangleFill();
            label1 = new Label();
            label7 = new Label();
            label8 = new Label();
            matchSoFarView = new DvdSubOcr.MatchSoFarView();
            blockViewer = new DvdSubOcr.BlockViewer();
            characterSelector = new DvdSubOcr.CharacterSelector();
            SuspendLayout();
            // 
            // unknownCharacterButton
            // 
            unknownCharacterButton.Location = new Point(135, 631);
            unknownCharacterButton.Margin = new Padding(3);
            unknownCharacterButton.Name = "unknownCharacterButton";
            unknownCharacterButton.Size = new Size(112, 23);
            unknownCharacterButton.TabIndex = 14;
            unknownCharacterButton.Text = "Ignore";
            toolTip1.SetToolTip(unknownCharacterButton, "The selected pattern is not part of any character");
            unknownCharacterButton.Click += unknownCharacterButton_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.Location = new Point(34, 429);
            label3.Margin = new Padding(3, 0, 3, 0);
            label3.Name = "label3";
            label3.Size = new Size(191, 16);
            label3.TabIndex = 6;
            label3.Text = "Press Ctrl key for Italic Characters";
            // 
            // doneSplitButton
            // 
            doneSplitButton.Enabled = false;
            doneSplitButton.Location = new Point(674, 422);
            doneSplitButton.Margin = new Padding(3);
            doneSplitButton.Name = "doneSplitButton";
            doneSplitButton.Size = new Size(107, 23);
            doneSplitButton.TabIndex = 20;
            doneSplitButton.Text = "Save Split";
            toolTip1.SetToolTip(doneSplitButton, "When one part of the block to split is correctly selected (green), save it");
            doneSplitButton.UseVisualStyleBackColor = true;
            doneSplitButton.Click += doneSplitButton_Click;
            // 
            // cancelSplitButton
            // 
            cancelSplitButton.Enabled = false;
            cancelSplitButton.Location = new Point(674, 451);
            cancelSplitButton.Margin = new Padding(3);
            cancelSplitButton.Name = "cancelSplitButton";
            cancelSplitButton.Size = new Size(107, 23);
            cancelSplitButton.TabIndex = 21;
            cancelSplitButton.Text = "Cancel Split";
            toolTip1.SetToolTip(cancelSplitButton, "Cancel the split operation");
            cancelSplitButton.UseVisualStyleBackColor = true;
            cancelSplitButton.Click += cancelSplitButton_Click;
            // 
            // beginSplitButton
            // 
            beginSplitButton.Enabled = false;
            beginSplitButton.Location = new Point(549, 436);
            beginSplitButton.Margin = new Padding(3);
            beginSplitButton.Name = "beginSplitButton";
            beginSplitButton.Size = new Size(107, 23);
            beginSplitButton.TabIndex = 19;
            beginSplitButton.Text = "Split in 2";
            toolTip1.SetToolTip(beginSplitButton, "If the highlighted block contains all or part of 2 different characters, begin the split operation");
            beginSplitButton.UseVisualStyleBackColor = true;
            beginSplitButton.Click += beginSplitButton_Click;
            // 
            // splitHelpLabel
            // 
            splitHelpLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            splitHelpLabel.Location = new Point(546, 388);
            splitHelpLabel.Margin = new Padding(3, 0, 3, 0);
            splitHelpLabel.Name = "splitHelpLabel";
            splitHelpLabel.Size = new Size(235, 31);
            splitHelpLabel.TabIndex = 18;
            splitHelpLabel.Text = "If the highlighted block(s) contains parts of 2 different characters, Split it up:";
            splitHelpLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.Location = new Point(12, 53);
            label4.Margin = new Padding(3, 0, 3, 0);
            label4.Name = "label4";
            label4.Size = new Size(596, 18);
            label4.TabIndex = 2;
            label4.Text = "If a character is in 2 or more pieces, click or use Arrow Keys to highlight them all (up to 10) before selecting";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            label4.Click += label4_Click;
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(533, 477);
            label2.Margin = new Padding(3, 0, 3, 0);
            label2.Name = "label2";
            label2.Size = new Size(264, 61);
            label2.TabIndex = 22;
            label2.Text = "If the pieces look like just the outlines of characters, or if each character is split in many parts, try a different Palette of colors:";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(12, 34);
            label5.Margin = new Padding(3, 0, 3, 0);
            label5.Name = "label5";
            label5.Size = new Size(622, 17);
            label5.TabIndex = 1;
            label5.Text = "Type or Click on the Character below which matches the darkened pattern, or use the Split or Palette Features";
            // 
            // label6
            // 
            label6.Location = new Point(603, 57);
            label6.Margin = new Padding(3, 0, 3, 0);
            label6.Name = "label6";
            label6.Size = new Size(116, 14);
            label6.TabIndex = 4;
            label6.Text = "Text OCR'd So Far";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // undoButton
            // 
            undoButton.Enabled = false;
            undoButton.Location = new Point(262, 631);
            undoButton.Margin = new Padding(3);
            undoButton.Name = "undoButton";
            undoButton.Size = new Size(112, 23);
            undoButton.TabIndex = 15;
            undoButton.Text = "&Undo";
            toolTip1.SetToolTip(undoButton, "Undo the last OCR match or split");
            undoButton.UseVisualStyleBackColor = true;
            undoButton.Click += undoButton_Click;
            // 
            // tryDifferentPaletteButton
            // 
            tryDifferentPaletteButton.Location = new Point(612, 540);
            tryDifferentPaletteButton.Margin = new Padding(3);
            tryDifferentPaletteButton.Name = "tryDifferentPaletteButton";
            tryDifferentPaletteButton.Size = new Size(107, 24);
            tryDifferentPaletteButton.TabIndex = 23;
            tryDifferentPaletteButton.Text = "Different Palette";
            toolTip1.SetToolTip(tryDifferentPaletteButton, "Try a different set of colors from the subtitle which show the characters best");
            tryDifferentPaletteButton.UseVisualStyleBackColor = true;
            tryDifferentPaletteButton.Click += tryDifferentPaletteButton_Click;
            // 
            // subtitleIndexLabel
            // 
            subtitleIndexLabel.Location = new Point(499, 576);
            subtitleIndexLabel.Margin = new Padding(3, 0, 3, 0);
            subtitleIndexLabel.Name = "subtitleIndexLabel";
            subtitleIndexLabel.Size = new Size(122, 23);
            subtitleIndexLabel.TabIndex = 27;
            subtitleIndexLabel.Text = "label7";
            subtitleIndexLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // startOverSubButton
            // 
            startOverSubButton.Location = new Point(484, 622);
            startOverSubButton.Margin = new Padding(3);
            startOverSubButton.Name = "startOverSubButton";
            startOverSubButton.Size = new Size(111, 37);
            startOverSubButton.TabIndex = 16;
            startOverSubButton.Text = "Start Over on this Subtitle";
            startOverSubButton.UseVisualStyleBackColor = true;
            startOverSubButton.Visible = false;
            startOverSubButton.Click += startOverSubButton_Click;
            // 
            // startOverMovieButton
            // 
            startOverMovieButton.Location = new Point(602, 622);
            startOverMovieButton.Margin = new Padding(3);
            startOverMovieButton.Name = "startOverMovieButton";
            startOverMovieButton.Size = new Size(111, 37);
            startOverMovieButton.TabIndex = 17;
            startOverMovieButton.Text = "Start Over for the whole Movie";
            startOverMovieButton.UseVisualStyleBackColor = true;
            startOverMovieButton.Visible = false;
            startOverMovieButton.Click += startOverMovieButton_Click;
            // 
            // reviewButton
            // 
            reviewButton.Location = new Point(719, 611);
            reviewButton.Margin = new Padding(3);
            reviewButton.Name = "reviewButton";
            reviewButton.Size = new Size(111, 42);
            reviewButton.TabIndex = 28;
            reviewButton.Text = "Review and Correct OCR Matches";
            toolTip1.SetToolTip(reviewButton, "View and fix any mistaken OCR matches or Splits made for this movie");
            reviewButton.UseVisualStyleBackColor = true;
            reviewButton.Click += reviewButton_Click;
            // 
            // ignoreBaselineCheckBox
            // 
            ignoreBaselineCheckBox.AutoSize = true;
            ignoreBaselineCheckBox.Location = new Point(273, 429);
            ignoreBaselineCheckBox.Margin = new Padding(3);
            ignoreBaselineCheckBox.Name = "ignoreBaselineCheckBox";
            ignoreBaselineCheckBox.Size = new Size(132, 19);
            ignoreBaselineCheckBox.TabIndex = 7;
            ignoreBaselineCheckBox.Text = "Ignore Baseline Here";
            toolTip1.SetToolTip(ignoreBaselineCheckBox, "Retry the OCR without checking that the highlighted character lines up vertically with its neighbors to left and right");
            ignoreBaselineCheckBox.UseVisualStyleBackColor = true;
            ignoreBaselineCheckBox.Visible = false;
            ignoreBaselineCheckBox.CheckedChanged += ignoreBaselineCheckBox_CheckedChanged;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(627, 576);
            progressBar1.Margin = new Padding(3);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(195, 23);
            progressBar1.TabIndex = 26;
            // 
            // retryButton
            // 
            retryButton.Location = new Point(488, 366);
            retryButton.Margin = new Padding(3);
            retryButton.Name = "retryButton";
            retryButton.Size = new Size(55, 55);
            retryButton.TabIndex = 25;
            retryButton.Text = "Retry (Debug)";
            retryButton.UseVisualStyleBackColor = true;
            retryButton.Visible = false;
            retryButton.Click += retryButton_Click;
            // 
            // paletteIndexLabel
            // 
            paletteIndexLabel.AutoSize = true;
            paletteIndexLabel.Location = new Point(725, 546);
            paletteIndexLabel.Margin = new Padding(3, 0, 3, 0);
            paletteIndexLabel.Name = "paletteIndexLabel";
            paletteIndexLabel.Size = new Size(0, 17);
            paletteIndexLabel.TabIndex = 24;
            paletteIndexLabel.Click += paletteIndexLabel_Click;
            // 
            // commentButton
            // 
            commentButton.Location = new Point(9, 631);
            commentButton.Margin = new Padding(3);
            commentButton.Name = "commentButton";
            commentButton.Size = new Size(112, 23);
            commentButton.TabIndex = 13;
            commentButton.Text = "Comment";
            toolTip1.SetToolTip(commentButton, "Add a comment at this point in the final subtitle file output");
            commentButton.UseVisualStyleBackColor = true;
            commentButton.Click += commentButton_Click;
            // 
            // dvdLabel
            // 
            dvdLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dvdLabel.Location = new Point(65, 6);
            dvdLabel.Margin = new Padding(3, 0, 3, 0);
            dvdLabel.Name = "dvdLabel";
            dvdLabel.Size = new Size(531, 23);
            dvdLabel.TabIndex = 0;
            dvdLabel.Text = "label2";
            dvdLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // manualEntryTextBox
            // 
            manualEntryTextBox.Location = new Point(153, 606);
            manualEntryTextBox.Margin = new Padding(3);
            manualEntryTextBox.MaxLength = 1;
            manualEntryTextBox.Name = "manualEntryTextBox";
            manualEntryTextBox.Size = new Size(32, 20);
            manualEntryTextBox.TabIndex = 10;
            manualEntryTextBox.TextChanged += manualEntryTextBox_TextChanged;
            manualEntryTextBox.KeyDown += manualEntryTextBox_KeyDown;
            // 
            // manualCharLabel
            // 
            manualCharLabel.AutoSize = true;
            manualCharLabel.Location = new Point(9, 607);
            manualCharLabel.Margin = new Padding(3, 0, 3, 0);
            manualCharLabel.Name = "manualCharLabel";
            manualCharLabel.Size = new Size(144, 17);
            manualCharLabel.TabIndex = 9;
            manualCharLabel.Text = "Manually Enter Character:";
            // 
            // manualItalicCheckBox
            // 
            manualItalicCheckBox.AutoSize = true;
            manualItalicCheckBox.Location = new Point(190, 607);
            manualItalicCheckBox.Margin = new Padding(3);
            manualItalicCheckBox.Name = "manualItalicCheckBox";
            manualItalicCheckBox.Size = new Size(47, 19);
            manualItalicCheckBox.TabIndex = 30;
            manualItalicCheckBox.Text = "Italic";
            toolTip1.SetToolTip(manualItalicCheckBox, "Determines whether a manually entered character is Italic or not");
            manualItalicCheckBox.UseVisualStyleBackColor = true;
            manualItalicCheckBox.CheckedChanged += manualItalicCheckBox_CheckedChanged;
            // 
            // manualWaitEnterCheckBox
            // 
            manualWaitEnterCheckBox.AutoSize = true;
            manualWaitEnterCheckBox.Location = new Point(371, 607);
            manualWaitEnterCheckBox.Margin = new Padding(3);
            manualWaitEnterCheckBox.Name = "manualWaitEnterCheckBox";
            manualWaitEnterCheckBox.Size = new Size(118, 19);
            manualWaitEnterCheckBox.TabIndex = 37;
            manualWaitEnterCheckBox.Text = "Wait for Enter Key";
            toolTip1.SetToolTip(manualWaitEnterCheckBox, "Determines whether a manually entered character is Italic or not");
            manualWaitEnterCheckBox.UseVisualStyleBackColor = true;
            manualWaitEnterCheckBox.CheckedChanged += manualWaitEnterCheckBox_CheckedChanged;
            // 
            // triangleFill4
            // 
            triangleFill4.FillColor = Color.WhiteSmoke;
            triangleFill4.ForeColor = Color.DodgerBlue;
            triangleFill4.Location = new Point(9, 69);
            triangleFill4.Margin = new Padding(5);
            triangleFill4.Name = "triangleFill4";
            triangleFill4.Origin = Corner.BottomLeft;
            triangleFill4.Size = new Size(473, 7);
            triangleFill4.TabIndex = 35;
            toolTip1.SetToolTip(triangleFill4, "Indicates manual entry is Italic");
            triangleFill4.Visible = false;
            // 
            // triangleFill3
            // 
            triangleFill3.FillColor = Color.WhiteSmoke;
            triangleFill3.ForeColor = Color.DodgerBlue;
            triangleFill3.Location = new Point(9, 422);
            triangleFill3.Margin = new Padding(5);
            triangleFill3.Name = "triangleFill3";
            triangleFill3.Origin = Corner.TopRight;
            triangleFill3.Size = new Size(473, 7);
            triangleFill3.TabIndex = 34;
            toolTip1.SetToolTip(triangleFill3, "Indicates manual entry is Italic");
            triangleFill3.Visible = false;
            // 
            // triangleFill2
            // 
            triangleFill2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            triangleFill2.FillColor = Color.WhiteSmoke;
            triangleFill2.ForeColor = Color.DodgerBlue;
            triangleFill2.Location = new Point(2, 76);
            triangleFill2.Margin = new Padding(5);
            triangleFill2.Name = "triangleFill2";
            triangleFill2.Origin = Corner.BottomRight;
            triangleFill2.Size = new Size(7, 346);
            triangleFill2.TabIndex = 32;
            toolTip1.SetToolTip(triangleFill2, "Indicates manual entry is Italic");
            triangleFill2.Visible = false;
            // 
            // triangleFill1
            // 
            triangleFill1.FillColor = Color.WhiteSmoke;
            triangleFill1.ForeColor = Color.DodgerBlue;
            triangleFill1.Location = new Point(482, 76);
            triangleFill1.Margin = new Padding(5);
            triangleFill1.Name = "triangleFill1";
            triangleFill1.Size = new Size(7, 346);
            triangleFill1.TabIndex = 31;
            toolTip1.SetToolTip(triangleFill1, "Indicates manual entry is Italic");
            triangleFill1.Visible = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label1.Location = new Point(234, 607);
            label1.Margin = new Padding(3, 0, 3, 0);
            label1.Name = "label1";
            label1.Size = new Size(135, 16);
            label1.TabIndex = 29;
            label1.Text = "(Toggle with Space key)";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label7.Location = new Point(380, 636);
            label7.Margin = new Padding(3, 0, 3, 0);
            label7.Name = "label7";
            label7.Size = new Size(70, 16);
            label7.TabIndex = 33;
            label7.Text = "(Backspace)";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
            label8.Location = new Point(488, 607);
            label8.Margin = new Padding(3, 0, 3, 0);
            label8.Name = "label8";
            label8.Size = new Size(95, 16);
            label8.TabIndex = 36;
            label8.Text = "(Toggle with F2)";
            // 
            // matchSoFarView
            // 
            matchSoFarView.BackColor = Color.DimGray;
            matchSoFarView.ForeColor = Color.Aquamarine;
            matchSoFarView.Location = new Point(490, 76);
            matchSoFarView.Margin = new Padding(5);
            matchSoFarView.Name = "matchSoFarView";
            matchSoFarView.Size = new Size(342, 284);
            matchSoFarView.TabIndex = 5;
            matchSoFarView.TabStop = false;
            // 
            // blockViewer
            // 
            blockViewer.Location = new Point(9, 76);
            blockViewer.Margin = new Padding(3);
            blockViewer.Name = "blockViewer";
            blockViewer.Size = new Size(473, 346);
            blockViewer.TabIndex = 3;
            blockViewer.EncodeClicked += blockViewer_EncodeClicked;
            // 
            // characterSelector
            // 
            characterSelector.Location = new Point(9, 450);
            characterSelector.Margin = new Padding(5);
            characterSelector.Name = "characterSelector";
            characterSelector.Size = new Size(473, 153);
            characterSelector.TabIndex = 8;
            characterSelector.SelectedCharacterChanged += characterSelector_SelectedCharacterChanged;
            // 
            // OcrBlocksStep
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Controls.Add(manualWaitEnterCheckBox);
            Controls.Add(label8);
            Controls.Add(triangleFill4);
            Controls.Add(triangleFill3);
            Controls.Add(label7);
            Controls.Add(triangleFill2);
            Controls.Add(triangleFill1);
            Controls.Add(manualItalicCheckBox);
            Controls.Add(label1);
            Controls.Add(manualCharLabel);
            Controls.Add(manualEntryTextBox);
            Controls.Add(dvdLabel);
            Controls.Add(commentButton);
            Controls.Add(paletteIndexLabel);
            Controls.Add(retryButton);
            Controls.Add(progressBar1);
            Controls.Add(ignoreBaselineCheckBox);
            Controls.Add(reviewButton);
            Controls.Add(splitHelpLabel);
            Controls.Add(tryDifferentPaletteButton);
            Controls.Add(startOverMovieButton);
            Controls.Add(beginSplitButton);
            Controls.Add(startOverSubButton);
            Controls.Add(cancelSplitButton);
            Controls.Add(matchSoFarView);
            Controls.Add(doneSplitButton);
            Controls.Add(subtitleIndexLabel);
            Controls.Add(label2);
            Controls.Add(undoButton);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(unknownCharacterButton);
            Controls.Add(blockViewer);
            Controls.Add(characterSelector);
            Margin = new Padding(3);
            Name = "OcrBlocksStep";
            Size = new Size(842, 662);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private DvdSubOcr.CharacterSelector characterSelector;
        private DvdSubOcr.BlockViewer blockViewer;
        private System.Windows.Forms.Button unknownCharacterButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button doneSplitButton;
        private System.Windows.Forms.Button cancelSplitButton;
        private System.Windows.Forms.Button beginSplitButton;
        private System.Windows.Forms.Label splitHelpLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button undoButton;
        private System.Windows.Forms.Button tryDifferentPaletteButton;
        private System.Windows.Forms.Label subtitleIndexLabel;
        private DvdSubOcr.MatchSoFarView matchSoFarView;
        private System.Windows.Forms.Button startOverSubButton;
        private System.Windows.Forms.Button startOverMovieButton;
        private System.Windows.Forms.Button reviewButton;
        private System.Windows.Forms.CheckBox ignoreBaselineCheckBox;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button retryButton;
        private System.Windows.Forms.Label paletteIndexLabel;
        private System.Windows.Forms.Button commentButton;
        private System.Windows.Forms.Label dvdLabel;
        private System.Windows.Forms.TextBox manualEntryTextBox;
        private System.Windows.Forms.Label manualCharLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox manualItalicCheckBox;
        private TriangleFill triangleFill1;
        private TriangleFill triangleFill2;
        private System.Windows.Forms.Label label7;
        private TriangleFill triangleFill3;
        private TriangleFill triangleFill4;
        private System.Windows.Forms.CheckBox manualWaitEnterCheckBox;
        private System.Windows.Forms.Label label8;
    }
}
