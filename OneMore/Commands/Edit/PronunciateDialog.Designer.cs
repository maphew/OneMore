﻿namespace River.OneMoreAddIn.Commands
{
	partial class PronunciateDialog
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
			if (disposing && (components != null))
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PronunciateDialog));
			this.wordLabel = new System.Windows.Forms.Label();
			this.wordBox = new System.Windows.Forms.TextBox();
			this.languageLabel = new System.Windows.Forms.Label();
			this.languagesBox = new System.Windows.Forms.ComboBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// wordLabel
			// 
			this.wordLabel.AutoSize = true;
			this.wordLabel.Location = new System.Drawing.Point(22, 35);
			this.wordLabel.Name = "wordLabel";
			this.wordLabel.Size = new System.Drawing.Size(47, 20);
			this.wordLabel.TabIndex = 0;
			this.wordLabel.Text = "Word";
			// 
			// wordBox
			// 
			this.wordBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.wordBox.Location = new System.Drawing.Point(110, 32);
			this.wordBox.Name = "wordBox";
			this.wordBox.Size = new System.Drawing.Size(282, 26);
			this.wordBox.TabIndex = 1;
			// 
			// languageLabel
			// 
			this.languageLabel.AutoSize = true;
			this.languageLabel.Location = new System.Drawing.Point(22, 75);
			this.languageLabel.Name = "languageLabel";
			this.languageLabel.Size = new System.Drawing.Size(81, 20);
			this.languageLabel.TabIndex = 2;
			this.languageLabel.Text = "Language";
			// 
			// languagesBox
			// 
			this.languagesBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.languagesBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.languagesBox.FormattingEnabled = true;
			this.languagesBox.Items.AddRange(new object[] {
            "English",
            "Hindi",
            "Spanish",
            "French",
            "Japanese",
            "Russian",
            "German",
            "Italian",
            "Korean",
            "Brazilian Portuguese",
            "Chinese (Simplified)",
            "Arabic",
            "Turkish"});
			this.languagesBox.Location = new System.Drawing.Point(110, 72);
			this.languagesBox.MaxDropDownItems = 13;
			this.languagesBox.Name = "languagesBox";
			this.languagesBox.Size = new System.Drawing.Size(282, 28);
			this.languagesBox.TabIndex = 3;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(292, 134);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(100, 38);
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(186, 134);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(100, 38);
			this.okButton.TabIndex = 5;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// PronunciateDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(416, 195);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.languagesBox);
			this.Controls.Add(this.languageLabel);
			this.Controls.Add(this.wordBox);
			this.Controls.Add(this.wordLabel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PronunciateDialog";
			this.Padding = new System.Windows.Forms.Padding(20, 29, 20, 20);
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Phonetics";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label wordLabel;
		private System.Windows.Forms.TextBox wordBox;
		private System.Windows.Forms.Label languageLabel;
		private System.Windows.Forms.ComboBox languagesBox;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
	}
}