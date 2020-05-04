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
			this.label2 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.ButtonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// UseSelectedText_Checkbox
			// 
			this.UseSelectedText_Checkbox.AutoSize = true;
			this.UseSelectedText_Checkbox.Location = new System.Drawing.Point(65, 43);
			this.UseSelectedText_Checkbox.Name = "UseSelectedText_Checkbox";
			this.UseSelectedText_Checkbox.Size = new System.Drawing.Size(422, 21);
			this.UseSelectedText_Checkbox.TabIndex = 0;
			this.UseSelectedText_Checkbox.Text = "Use selected editor text as input to \'Open File By Name\' dialog";
			this.UseSelectedText_Checkbox.UseVisualStyleBackColor = true;
			// 
			// OKButton
			// 
			this.OKButton.Location = new System.Drawing.Point(149, 193);
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
			this.CloseOnDoubleclick_Checkbox.Location = new System.Drawing.Point(65, 80);
			this.CloseOnDoubleclick_Checkbox.Name = "CloseOnDoubleclick_Checkbox";
			this.CloseOnDoubleclick_Checkbox.Size = new System.Drawing.Size(405, 21);
			this.CloseOnDoubleclick_Checkbox.TabIndex = 2;
			this.CloseOnDoubleclick_Checkbox.Text = "Close \'Open File By Name\' dialog automatically after double";
			this.CloseOnDoubleclick_Checkbox.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(87, 105);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(143, 17);
			this.label1.TabIndex = 3;
			this.label1.Text = "clicking on a filename";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(61, 142);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Font:";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(104, 142);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(321, 22);
			this.textBox1.TabIndex = 5;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(431, 140);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(40, 28);
			this.button1.TabIndex = 6;
			this.button1.Text = "...";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// ButtonCancel
			// 
			this.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.ButtonCancel.Location = new System.Drawing.Point(275, 193);
			this.ButtonCancel.Name = "ButtonCancel";
			this.ButtonCancel.Size = new System.Drawing.Size(105, 38);
			this.ButtonCancel.TabIndex = 7;
			this.ButtonCancel.Text = "Cancel";
			this.ButtonCancel.UseVisualStyleBackColor = true;
			this.ButtonCancel.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// OpenFileOptionsDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(528, 247);
			this.Controls.Add(this.ButtonCancel);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.CloseOnDoubleclick_Checkbox);
			this.Controls.Add(this.OKButton);
			this.Controls.Add(this.UseSelectedText_Checkbox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.KeyPreview = true;
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
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button ButtonCancel;
	}
}