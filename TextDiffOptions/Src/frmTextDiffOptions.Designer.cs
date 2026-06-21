namespace TextDiffOptions
{
    partial class frmTextDiffOptions
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTextDiffOptions));
            statusStrip = new System.Windows.Forms.StatusStrip();
            toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            cmdCompare = new System.Windows.Forms.Button();
            cmdCancel = new System.Windows.Forms.Button();
            toolTip1 = new System.Windows.Forms.ToolTip(components);
            cmdAddShortcut = new System.Windows.Forms.Button();
            cmdRemoveShortcut = new System.Windows.Forms.Button();
            chkInfo = new System.Windows.Forms.CheckBox();
            chkSentences = new System.Windows.Forms.CheckBox();
            chkCase = new System.Windows.Forms.CheckBox();
            chkAccents = new System.Windows.Forms.CheckBox();
            chkNonBreakingSpaces = new System.Windows.Forms.CheckBox();
            chkPunctuation = new System.Windows.Forms.CheckBox();
            chkAll = new System.Windows.Forms.CheckBox();
            cmdPrevPage = new System.Windows.Forms.Button();
            cmdNextPage = new System.Windows.Forms.Button();
            chkPaginate = new System.Windows.Forms.CheckBox();
            chkRatio = new System.Windows.Forms.CheckBox();
            chkParagraphs = new System.Windows.Forms.CheckBox();
            chkQuotes = new System.Windows.Forms.CheckBox();
            chkSpaces = new System.Windows.Forms.CheckBox();
            chkNumbers = new System.Windows.Forms.CheckBox();
            lbAlgorithm = new System.Windows.Forms.ListBox();
            lblPath1 = new System.Windows.Forms.Label();
            lblPath2 = new System.Windows.Forms.Label();
            lblPageNum = new System.Windows.Forms.Label();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { toolStripStatusLabel });
            statusStrip.Location = new System.Drawing.Point(0, 222);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new System.Drawing.Size(695, 22);
            statusStrip.TabIndex = 0;
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // cmdCompare
            // 
            cmdCompare.Location = new System.Drawing.Point(321, 45);
            cmdCompare.Name = "cmdCompare";
            cmdCompare.Size = new System.Drawing.Size(103, 32);
            cmdCompare.TabIndex = 1;
            cmdCompare.Text = "Compare";
            toolTip1.SetToolTip(cmdCompare, "Compare the files using the selected tool with these options");
            cmdCompare.Click += cmdCompare_Click;
            // 
            // cmdCancel
            // 
            cmdCancel.Enabled = false;
            cmdCancel.Location = new System.Drawing.Point(459, 45);
            cmdCancel.Name = "cmdCancel";
            cmdCancel.Size = new System.Drawing.Size(103, 32);
            cmdCancel.TabIndex = 2;
            cmdCancel.Text = "Cancel";
            toolTip1.SetToolTip(cmdCancel, "Cancel the current operation");
            // 
            // cmdAddShortcut
            // 
            cmdAddShortcut.BackColor = System.Drawing.SystemColors.Control;
            cmdAddShortcut.Location = new System.Drawing.Point(425, 10);
            cmdAddShortcut.Name = "cmdAddShortcut";
            cmdAddShortcut.Size = new System.Drawing.Size(25, 25);
            cmdAddShortcut.TabIndex = 3;
            cmdAddShortcut.Text = "+";
            toolTip1.SetToolTip(cmdAddShortcut, "Add a \"Send To\" shortcut to TextDiffOptions in the Windows Explorer context menu");
            cmdAddShortcut.UseVisualStyleBackColor = false;
            cmdAddShortcut.Click += cmdAddShortcut_Click;
            // 
            // cmdRemoveShortcut
            // 
            cmdRemoveShortcut.BackColor = System.Drawing.SystemColors.Control;
            cmdRemoveShortcut.Location = new System.Drawing.Point(456, 10);
            cmdRemoveShortcut.Name = "cmdRemoveShortcut";
            cmdRemoveShortcut.Size = new System.Drawing.Size(25, 25);
            cmdRemoveShortcut.TabIndex = 4;
            cmdRemoveShortcut.Text = "-";
            toolTip1.SetToolTip(cmdRemoveShortcut, "Remove the \"Send To\" shortcut from Windows Explorer");
            cmdRemoveShortcut.UseVisualStyleBackColor = false;
            cmdRemoveShortcut.Click += cmdRemoveShortcut_Click;
            // 
            // chkInfo
            // 
            chkInfo.AutoSize = true;
            chkInfo.Checked = true;
            chkInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            chkInfo.Location = new System.Drawing.Point(321, 10);
            chkInfo.Name = "chkInfo";
            chkInfo.Size = new System.Drawing.Size(89, 19);
            chkInfo.TabIndex = 19;
            chkInfo.Text = "Information";
            toolTip1.SetToolTip(chkInfo, "Include file information in the text files to help identify the two files in the diff tool");
            chkInfo.UseVisualStyleBackColor = true;
            // 
            // chkSentences
            // 
            chkSentences.AutoSize = true;
            chkSentences.Checked = true;
            chkSentences.CheckState = System.Windows.Forms.CheckState.Checked;
            chkSentences.Location = new System.Drawing.Point(112, 113);
            chkSentences.Name = "chkSentences";
            chkSentences.Size = new System.Drawing.Size(79, 19);
            chkSentences.TabIndex = 11;
            chkSentences.Text = "Sentences";
            toolTip1.SetToolTip(chkSentences, "Check to compare full sentences; uncheck to compare individual words (without punctuation).");
            chkSentences.UseVisualStyleBackColor = true;
            chkSentences.CheckedChanged += chkSentences_CheckedChanged;
            chkSentences.Click += chkSentences_Click;
            // 
            // chkCase
            // 
            chkCase.AutoSize = true;
            chkCase.Checked = true;
            chkCase.CheckState = System.Windows.Forms.CheckState.Checked;
            chkCase.Location = new System.Drawing.Point(15, 73);
            chkCase.Name = "chkCase";
            chkCase.Size = new System.Drawing.Size(51, 19);
            chkCase.TabIndex = 8;
            chkCase.Text = "Case";
            toolTip1.SetToolTip(chkCase, "Check to take case (upper/lowercase) into account, otherwise ignore it.");
            chkCase.UseVisualStyleBackColor = true;
            chkCase.Click += chkCase_Click;
            // 
            // chkAccents
            // 
            chkAccents.AutoSize = true;
            chkAccents.Checked = true;
            chkAccents.CheckState = System.Windows.Forms.CheckState.Checked;
            chkAccents.Location = new System.Drawing.Point(15, 93);
            chkAccents.Name = "chkAccents";
            chkAccents.Size = new System.Drawing.Size(68, 19);
            chkAccents.TabIndex = 9;
            chkAccents.Text = "Accents";
            toolTip1.SetToolTip(chkAccents, "Check to take accents into account, otherwise ignore them.");
            chkAccents.UseVisualStyleBackColor = true;
            chkAccents.Click += chkAccents_Click;
            // 
            // chkNonBreakingSpaces
            // 
            chkNonBreakingSpaces.AutoSize = true;
            chkNonBreakingSpaces.Location = new System.Drawing.Point(15, 30);
            chkNonBreakingSpaces.Name = "chkNonBreakingSpaces";
            chkNonBreakingSpaces.Size = new System.Drawing.Size(138, 19);
            chkNonBreakingSpaces.TabIndex = 6;
            chkNonBreakingSpaces.Text = "Non-breaking spaces";
            toolTip1.SetToolTip(chkNonBreakingSpaces, "Check to keep non-breaking spaces (requires punctuation); uncheck to ignore them.");
            chkNonBreakingSpaces.UseVisualStyleBackColor = true;
            chkNonBreakingSpaces.Click += chkNonBreakingSpaces_Click;
            // 
            // chkPunctuation
            // 
            chkPunctuation.AutoSize = true;
            chkPunctuation.Checked = true;
            chkPunctuation.CheckState = System.Windows.Forms.CheckState.Checked;
            chkPunctuation.Location = new System.Drawing.Point(15, 113);
            chkPunctuation.Name = "chkPunctuation";
            chkPunctuation.Size = new System.Drawing.Size(91, 19);
            chkPunctuation.TabIndex = 10;
            chkPunctuation.Text = "Punctuation";
            toolTip1.SetToolTip(chkPunctuation, "Check to keep punctuation, otherwise ignore it.");
            chkPunctuation.UseVisualStyleBackColor = true;
            chkPunctuation.CheckedChanged += chkPunctuation_CheckedChanged;
            chkPunctuation.Click += chkPunctuation_Click;
            // 
            // chkAll
            // 
            chkAll.AutoSize = true;
            chkAll.Location = new System.Drawing.Point(15, 10);
            chkAll.Name = "chkAll";
            chkAll.Size = new System.Drawing.Size(40, 19);
            chkAll.TabIndex = 5;
            chkAll.Text = "All";
            toolTip1.SetToolTip(chkAll, "Check/uncheck all options");
            chkAll.UseVisualStyleBackColor = true;
            chkAll.Click += chkAll_Click;
            // 
            // cmdPrevPage
            // 
            cmdPrevPage.Enabled = false;
            cmdPrevPage.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 2);
            cmdPrevPage.Location = new System.Drawing.Point(175, 46);
            cmdPrevPage.Name = "cmdPrevPage";
            cmdPrevPage.Size = new System.Drawing.Size(24, 24);
            cmdPrevPage.TabIndex = 16;
            cmdPrevPage.Text = "ó";
            toolTip1.SetToolTip(cmdPrevPage, "Compare previous page");
            cmdPrevPage.UseVisualStyleBackColor = true;
            cmdPrevPage.Click += cmdPrevPage_Click;
            // 
            // cmdNextPage
            // 
            cmdNextPage.Enabled = false;
            cmdNextPage.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 2);
            cmdNextPage.Location = new System.Drawing.Point(256, 46);
            cmdNextPage.Name = "cmdNextPage";
            cmdNextPage.Size = new System.Drawing.Size(24, 24);
            cmdNextPage.TabIndex = 18;
            cmdNextPage.Text = "ô";
            toolTip1.SetToolTip(cmdNextPage, "Compare next page");
            cmdNextPage.UseVisualStyleBackColor = true;
            cmdNextPage.Click += cmdNextPage_Click;
            // 
            // chkPaginate
            // 
            chkPaginate.AutoSize = true;
            chkPaginate.Location = new System.Drawing.Point(175, 23);
            chkPaginate.Name = "chkPaginate";
            chkPaginate.Size = new System.Drawing.Size(72, 19);
            chkPaginate.TabIndex = 14;
            chkPaginate.Text = "Paginate";
            toolTip1.SetToolTip(chkPaginate, "Split into pages to speed up the diff tool (useful for word-by-word comparison)");
            chkPaginate.UseVisualStyleBackColor = true;
            chkPaginate.CheckedChanged += chkPaginate_CheckedChanged;
            // 
            // chkRatio
            // 
            chkRatio.AutoSize = true;
            chkRatio.Location = new System.Drawing.Point(253, 23);
            chkRatio.Name = "chkRatio";
            chkRatio.Size = new System.Drawing.Size(53, 19);
            chkRatio.TabIndex = 15;
            chkRatio.Text = "Ratio";
            toolTip1.SetToolTip(chkRatio, "Apply a ratio to compare two texts as if they had the same content length");
            chkRatio.UseVisualStyleBackColor = true;
            // 
            // chkParagraphs
            // 
            chkParagraphs.AutoSize = true;
            chkParagraphs.Checked = true;
            chkParagraphs.CheckState = System.Windows.Forms.CheckState.Checked;
            chkParagraphs.Location = new System.Drawing.Point(205, 113);
            chkParagraphs.Name = "chkParagraphs";
            chkParagraphs.Size = new System.Drawing.Size(85, 19);
            chkParagraphs.TabIndex = 12;
            chkParagraphs.Text = "Paragraphs";
            toolTip1.SetToolTip(chkParagraphs, "Check to compare paragraphs as-is; uncheck to compare sentences independently (if Punctuation is checked).");
            chkParagraphs.UseVisualStyleBackColor = true;
            chkParagraphs.Click += chkParagraphs_Click;
            // 
            // chkQuotes
            // 
            chkQuotes.AutoSize = true;
            chkQuotes.Location = new System.Drawing.Point(15, 136);
            chkQuotes.Name = "chkQuotes";
            chkQuotes.Size = new System.Drawing.Size(64, 19);
            chkQuotes.TabIndex = 13;
            chkQuotes.Text = "Quotes";
            toolTip1.SetToolTip(chkQuotes, "Uncheck to normalize quotes (apostrophes).");
            chkQuotes.UseVisualStyleBackColor = true;
            chkQuotes.Click += chkQuotes_Click;
            // 
            // chkSpaces
            // 
            chkSpaces.AutoSize = true;
            chkSpaces.Location = new System.Drawing.Point(15, 50);
            chkSpaces.Name = "chkSpaces";
            chkSpaces.Size = new System.Drawing.Size(62, 19);
            chkSpaces.TabIndex = 7;
            chkSpaces.Text = "Spaces";
            toolTip1.SetToolTip(chkSpaces, "Check to keep leading/trailing spaces and multiple line breaks; uncheck to ignore them.");
            chkSpaces.UseVisualStyleBackColor = true;
            chkSpaces.Click += chkSpaces_Click;
            // 
            // chkNumbers
            // 
            chkNumbers.AutoSize = true;
            chkNumbers.Checked = true;
            chkNumbers.CheckState = System.Windows.Forms.CheckState.Checked;
            chkNumbers.Location = new System.Drawing.Point(294, 113);
            chkNumbers.Name = "chkNumbers";
            chkNumbers.Size = new System.Drawing.Size(75, 19);
            chkNumbers.TabIndex = 23;
            chkNumbers.Text = "Numbers";
            toolTip1.SetToolTip(chkNumbers, "Check to keep numbers (page numbers, etc.); uncheck to ignore them.");
            chkNumbers.UseVisualStyleBackColor = true;
            chkNumbers.Click += chkNumbers_Click;
            // 
            // lbAlgorithm
            // 
            lbAlgorithm.FormattingEnabled = true;
            lbAlgorithm.Location = new System.Drawing.Point(584, 34);
            lbAlgorithm.Name = "lbAlgorithm";
            lbAlgorithm.Size = new System.Drawing.Size(100, 34);
            lbAlgorithm.TabIndex = 24;
            toolTip1.SetToolTip(lbAlgorithm, "Diff algorithm");
            // 
            // lblPath1
            // 
            lblPath1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblPath1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            lblPath1.Location = new System.Drawing.Point(12, 168);
            lblPath1.Name = "lblPath1";
            lblPath1.Size = new System.Drawing.Size(671, 15);
            lblPath1.TabIndex = 21;
            lblPath1.Text = "Path1";
            // 
            // lblPath2
            // 
            lblPath2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            lblPath2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            lblPath2.Location = new System.Drawing.Point(12, 192);
            lblPath2.Name = "lblPath2";
            lblPath2.Size = new System.Drawing.Size(671, 15);
            lblPath2.TabIndex = 22;
            lblPath2.Text = "Path2";
            // 
            // lblPageNum
            // 
            lblPageNum.BackColor = System.Drawing.SystemColors.ControlLightLight;
            lblPageNum.Enabled = false;
            lblPageNum.Location = new System.Drawing.Point(205, 48);
            lblPageNum.Name = "lblPageNum";
            lblPageNum.Size = new System.Drawing.Size(45, 19);
            lblPageNum.TabIndex = 17;
            lblPageNum.Text = "1/1";
            // 
            // frmTextDiffOptions
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(695, 244);
            Controls.Add(lbAlgorithm);
            Controls.Add(chkNumbers);
            Controls.Add(chkSpaces);
            Controls.Add(chkQuotes);
            Controls.Add(chkParagraphs);
            Controls.Add(chkRatio);
            Controls.Add(chkPaginate);
            Controls.Add(lblPageNum);
            Controls.Add(cmdPrevPage);
            Controls.Add(cmdNextPage);
            Controls.Add(chkAll);
            Controls.Add(chkSentences);
            Controls.Add(lblPath2);
            Controls.Add(lblPath1);
            Controls.Add(chkInfo);
            Controls.Add(chkPunctuation);
            Controls.Add(chkNonBreakingSpaces);
            Controls.Add(chkAccents);
            Controls.Add(chkCase);
            Controls.Add(cmdAddShortcut);
            Controls.Add(cmdRemoveShortcut);
            Controls.Add(cmdCancel);
            Controls.Add(cmdCompare);
            Controls.Add(statusStrip);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "frmTextDiffOptions";
            Text = "TextDiffOptions: Options interface for TextDiffToHtml";
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.Button cmdCompare;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button cmdAddShortcut;
        private System.Windows.Forms.Button cmdRemoveShortcut;
        private System.Windows.Forms.CheckBox chkInfo;
        private System.Windows.Forms.CheckBox chkSentences;
        private System.Windows.Forms.CheckBox chkCase;
        private System.Windows.Forms.CheckBox chkAccents;
        private System.Windows.Forms.CheckBox chkNonBreakingSpaces;
        private System.Windows.Forms.CheckBox chkPunctuation;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.Button cmdPrevPage;
        private System.Windows.Forms.Button cmdNextPage;
        private System.Windows.Forms.CheckBox chkPaginate;
        private System.Windows.Forms.CheckBox chkRatio;
        private System.Windows.Forms.CheckBox chkParagraphs;
        private System.Windows.Forms.CheckBox chkQuotes;
        private System.Windows.Forms.CheckBox chkSpaces;
        private System.Windows.Forms.CheckBox chkNumbers;
        private System.Windows.Forms.ListBox lbAlgorithm;
        private System.Windows.Forms.Label lblPath1;
        private System.Windows.Forms.Label lblPath2;
        private System.Windows.Forms.Label lblPageNum;
    }
}
