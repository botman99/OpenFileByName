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
			this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(764, 233);
			this.panel1.TabIndex = 4;
			// 
			// FileListView
			// 
			this.FileListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.FileListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.File,
            this.Path});
			this.FileListView.FullRowSelect = true;
			this.FileListView.GridLines = true;
			this.FileListView.HideSelection = false;
			this.FileListView.Location = new System.Drawing.Point(0, 0);
			this.FileListView.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.FileListView.Name = "FileListView";
			this.FileListView.Size = new System.Drawing.Size(764, 233);
			this.FileListView.TabIndex = 0;
			this.FileListView.UseCompatibleStateImageBehavior = false;
			this.FileListView.View = System.Windows.Forms.View.Details;
			this.FileListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.FileListView_ColumnClick);
			this.FileListView.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.FileListView_ColumnWidthChanged);
			this.FileListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.FileListView_MouseDoubleClick);
			// 
			// File
			// 
			this.File.Text = "File";
			this.File.Width = 200;
			// 
			// Path
			// 
			this.Path.Text = "Path";
			this.Path.Width = 400;
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.Options_Button);
			this.panel2.Controls.Add(this.pictureBox1);
			this.panel2.Controls.Add(this.FileComboBox);
			this.panel2.Controls.Add(this.OK_Button);
			this.panel2.Controls.Add(this.Cancel_Button);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel2.Location = new System.Drawing.Point(0, 233);
			this.panel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(764, 108);
			this.panel2.TabIndex = 5;
			this.panel2.Move += new System.EventHandler(this.OpenFileDialog_Move);
			this.panel2.Resize += new System.EventHandler(this.OpenFileDialog_Resize);
			// 
			// Options_Button
			// 
			this.Options_Button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Options_Button.Location = new System.Drawing.Point(673, 65);
			this.Options_Button.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(24, 24);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// FileComboBox
			// 
			this.FileComboBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.FileComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
			this.FileComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FileComboBox.ItemHeight = 13;
			this.FileComboBox.Location = new System.Drawing.Point(112, 7);
			this.FileComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.FileComboBox.MaxDropDownItems = 1;
			this.FileComboBox.Name = "FileComboBox";
			this.FileComboBox.Size = new System.Drawing.Size(540, 20);
			this.FileComboBox.TabIndex = 0;
			this.FileComboBox.TextChanged += new System.EventHandler(this.FileComboBox_TextChanged);
			// 
			// OK_Button
			// 
			this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.OK_Button.Location = new System.Drawing.Point(285, 65);
			this.OK_Button.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
			this.Cancel_Button.Location = new System.Drawing.Point(400, 65);
			this.Cancel_Button.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
			this.ClientSize = new System.Drawing.Size(764, 341);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel2);
			this.KeyPreview = true;
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.MinimumSize = new System.Drawing.Size(680, 249);
			this.Name = "OpenFileDialog";
			this.Text = "Open File By Name";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OpenFileDialog_FormClosing);
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