namespace OpenFileByName
{
	partial class OpenFileOptionsDialog
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
			this.UseSelectedText_Checkbox = new System.Windows.Forms.CheckBox();
			this.OKButton = new System.Windows.Forms.Button();
			this.CloseOnDoubleclick_Checkbox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// UseSelectedText_Checkbox
			// 
			this.UseSelectedText_Checkbox.AutoSize = true;
			this.UseSelectedText_Checkbox.Location = new System.Drawing.Point(49, 35);
			this.UseSelectedText_Checkbox.Name = "UseSelectedText_Checkbox";
			this.UseSelectedText_Checkbox.Size = new System.Drawing.Size(289, 17);
			this.UseSelectedText_Checkbox.TabIndex = 0;
			this.UseSelectedText_Checkbox.Text = "Use selected text as input to \'Open File By Name\' dialog";
			this.UseSelectedText_Checkbox.UseVisualStyleBackColor = true;
			this.UseSelectedText_Checkbox.CheckedChanged += new System.EventHandler(this.UseSelectedText_Checkbox_CheckedChanged);
			// 
			// OKButton
			// 
			this.OKButton.Location = new System.Drawing.Point(159, 114);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(79, 31);
			this.OKButton.TabIndex = 1;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CloseOnDoubleclick_Checkbox
			// 
			this.CloseOnDoubleclick_Checkbox.AutoSize = true;
			this.CloseOnDoubleclick_Checkbox.Location = new System.Drawing.Point(49, 68);
			this.CloseOnDoubleclick_Checkbox.Name = "CloseOnDoubleclick_Checkbox";
			this.CloseOnDoubleclick_Checkbox.Size = new System.Drawing.Size(298, 17);
			this.CloseOnDoubleclick_Checkbox.TabIndex = 2;
			this.CloseOnDoubleclick_Checkbox.Text = "Close \'Open File By Name\' dialog on filename double-click";
			this.CloseOnDoubleclick_Checkbox.UseVisualStyleBackColor = true;
			this.CloseOnDoubleclick_Checkbox.CheckedChanged += new System.EventHandler(this.CloseOnDoubleclick_Checkbox_CheckedChanged);
			// 
			// OpenFileOptionsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(396, 157);
			this.Controls.Add(this.CloseOnDoubleclick_Checkbox);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.UseSelectedText_Checkbox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OpenFileOptionsDialog";
			this.Text = "Open File By Name - Options";
			this.Shown += new System.EventHandler(this.OpenFileOptions_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox UseSelectedText_Checkbox;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.CheckBox CloseOnDoubleclick_Checkbox;
	}
}