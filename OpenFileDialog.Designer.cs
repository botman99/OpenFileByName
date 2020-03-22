namespace OpenFileByName
{
	partial class OpenFileDialog
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.FileListView = new System.Windows.Forms.ListView();
			this.File = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Project = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.Path = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.panel2 = new System.Windows.Forms.Panel();
			this.Options_Button = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.FileComboBox = new System.Windows.Forms.ComboBox();
			this.OK_Button = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.FileListView);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(764, 223);
			this.panel1.TabIndex = 4;
			// 
			// FileListView
			// 
			this.FileListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.FileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.File,
            this.Project,
            this.Path});
			this.FileListView.FullRowSelect = true;
			this.FileListView.GridLines = true;
			this.FileListView.Location = new System.Drawing.Point(0, 0);
			this.FileListView.Name = "FileListView";
			this.FileListView.Size = new System.Drawing.Size(764, 223);
			this.FileListView.TabIndex = 0;
			this.FileListView.UseCompatibleStateImageBehavior = false;
			this.FileListView.View = System.Windows.Forms.View.Details;
			this.FileListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.FileListView_ColumnClick);
			this.FileListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.FileListView_MouseDoubleClick);
			// 
			// File
			// 
			this.File.Text = "File";
			this.File.Width = 200;
			// 
			// Project
			// 
			this.Project.Text = "Project";
			this.Project.Width = 100;
			// 
			// Path
			// 
			this.Path.Text = "Path";
			this.Path.Width = 460;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.Options_Button);
			this.panel2.Controls.Add(this.pictureBox1);
			this.panel2.Controls.Add(this.FileComboBox);
			this.panel2.Controls.Add(this.OK_Button);
			this.panel2.Controls.Add(this.Cancel_Button);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 223);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(764, 98);
			this.panel2.TabIndex = 5;
			this.panel2.Move += new System.EventHandler(this.OpenFileDialog_Move);
			this.panel2.Resize += new System.EventHandler(this.OpenFileDialog_Resize);
			// 
			// Options_Button
			// 
			this.Options_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Options_Button.Location = new System.Drawing.Point(673, 55);
			this.Options_Button.Name = "Options_Button";
			this.Options_Button.Size = new System.Drawing.Size(79, 31);
			this.Options_Button.TabIndex = 4;
			this.Options_Button.Text = "Options";
			this.Options_Button.UseVisualStyleBackColor = true;
			this.Options_Button.Click += new System.EventHandler(this.OptionsButton_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.pictureBox1.Location = new System.Drawing.Point(668, 7);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(24, 24);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// FileComboBox
			// 
			this.FileComboBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.FileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.FileComboBox.FormattingEnabled = true;
			this.FileComboBox.Location = new System.Drawing.Point(112, 7);
			this.FileComboBox.MaxDropDownItems = 10;
			this.FileComboBox.Name = "FileComboBox";
			this.FileComboBox.Size = new System.Drawing.Size(540, 21);
			this.FileComboBox.TabIndex = 0;
			this.FileComboBox.TextChanged += new System.EventHandler(this.FileComboBox_TextChanged);
			// 
			// OK_Button
			// 
			this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.OK_Button.Location = new System.Drawing.Point(285, 55);
			this.OK_Button.Name = "OK_Button";
			this.OK_Button.Size = new System.Drawing.Size(79, 31);
			this.OK_Button.TabIndex = 1;
			this.OK_Button.Text = "OK";
			this.OK_Button.UseVisualStyleBackColor = true;
			this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
			// 
			// Cancel_Button
			// 
			this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.Cancel_Button.Location = new System.Drawing.Point(400, 55);
			this.Cancel_Button.Name = "Cancel_Button";
			this.Cancel_Button.Size = new System.Drawing.Size(79, 31);
			this.Cancel_Button.TabIndex = 2;
			this.Cancel_Button.Text = "Cancel";
			this.Cancel_Button.UseVisualStyleBackColor = true;
			this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
			// 
			// OpenFileDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(764, 321);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(680, 250);
			this.Name = "OpenFileDialog";
			this.Text = "Open File By Name";
			this.Shown += new System.EventHandler(this.OpenFile_Shown);
			this.SizeChanged += new System.EventHandler(this.OpenFile_SizeChanged);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OpenFileDialog_KeyDown);
			this.Move += new System.EventHandler(this.OpenFileDialog_Move);
			this.Resize += new System.EventHandler(this.OpenFileDialog_Resize);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView FileListView;
		private System.Windows.Forms.ColumnHeader File;
		private System.Windows.Forms.ColumnHeader Project;
		private System.Windows.Forms.ColumnHeader Path;
		private System.Windows.Forms.Button OK_Button;
		private System.Windows.Forms.Button Cancel_Button;
		private System.Windows.Forms.ComboBox FileComboBox;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button Options_Button;
	}
}