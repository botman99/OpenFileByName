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
			this.UseSelectedText_Checkbox = new System.Windows.Forms.CheckBox();
			this.OKButton = new System.Windows.Forms.Button();
			this.CloseOnDoubleclick_Checkbox = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// UseSelectedText_Checkbox
			// 
			this.UseSelectedText_Checkbox.AutoSize = true;
			this.UseSelectedText_Checkbox.Location = new System.Drawing.Point(65, 43);
			this.UseSelectedText_Checkbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.UseSelectedText_Checkbox.Name = "UseSelectedText_Checkbox";
			this.UseSelectedText_Checkbox.Size = new System.Drawing.Size(422, 21);
			this.UseSelectedText_Checkbox.TabIndex = 0;
			this.UseSelectedText_Checkbox.Text = "Use selected editor text as input to \'Open File By Name\' dialog";
			this.UseSelectedText_Checkbox.UseVisualStyleBackColor = true;
			this.UseSelectedText_Checkbox.CheckedChanged += new System.EventHandler(this.UseSelectedText_Checkbox_CheckedChanged);
			// 
			// OKButton
			// 
			this.OKButton.Location = new System.Drawing.Point(212, 150);
			this.OKButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.OKButton.Name = "OKButton";
			this.OKButton.Size = new System.Drawing.Size(105, 38);
			this.OKButton.TabIndex = 1;
			this.OKButton.Text = "OK";
			this.OKButton.UseVisualStyleBackColor = true;
			this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
			// 
			// CloseOnDoubleclick_Checkbox
			// 
			this.CloseOnDoubleclick_Checkbox.AutoSize = true;
			this.CloseOnDoubleclick_Checkbox.Location = new System.Drawing.Point(65, 84);
			this.CloseOnDoubleclick_Checkbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.CloseOnDoubleclick_Checkbox.Name = "CloseOnDoubleclick_Checkbox";
			this.CloseOnDoubleclick_Checkbox.Size = new System.Drawing.Size(405, 21);
			this.CloseOnDoubleclick_Checkbox.TabIndex = 2;
			this.CloseOnDoubleclick_Checkbox.Text = "Close \'Open File By Name\' dialog automatically after double";
			this.CloseOnDoubleclick_Checkbox.UseVisualStyleBackColor = true;
			this.CloseOnDoubleclick_Checkbox.CheckedChanged += new System.EventHandler(this.CloseOnDoubleclick_Checkbox_CheckedChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(83, 109);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(143, 17);
			this.label1.TabIndex = 3;
			this.label1.Text = "clicking on a filename";
			// 
			// OpenFileOptionsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(528, 213);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CloseOnDoubleclick_Checkbox);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.UseSelectedText_Checkbox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OpenFileOptionsDialog";
			this.Text = "Open File By Name - Options";
			this.Shown += new System.EventHandler(this.OpenFileOptions_Shown);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OpenFileOptionsDialog_KeyDown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox UseSelectedText_Checkbox;
		private System.Windows.Forms.Button OKButton;
		private System.Windows.Forms.CheckBox CloseOnDoubleclick_Checkbox;
		private System.Windows.Forms.Label label1;
	}
}