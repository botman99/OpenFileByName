﻿//
// Copyright 2020 - Jeffrey "botman" Broome
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using EnvDTE;
using Microsoft.VisualStudio.Shell;

// NOTE: "processing" spin.gif is courtesy of http://www.chimply.com/

namespace OpenFileByName
{
	public partial class OpenFileDialog:Form
	{
		public const int WM_USER = 0x0400;  // Windows private user messages
		public const int WM_WORKER_DONE = WM_USER;

		private System.Threading.Thread WorkerThread = null;
		private System.Timers.Timer update_timer;

		private bool bWindowInitComplete;

		private System.Timers.Timer auto_save_timer;
		private int auto_save_width = -1;
		private int auto_save_height = -1;
		private int auto_save_left = -1;
		private int auto_save_top = -1;

		private ListViewColumnSorter columnSorter;
		private const int NumColumns = 2;
		private int[] ColumnWidthSize;  // store the ListView column width size so we can tell if they have actually changed

		public string input;

		public static List<ListViewItem> items = null;

		public OpenFileDialog(string previous_input)
		{
			bWindowInitComplete = false;  // we aren't done initializing the dialog yet

			InitializeComponent();

			TopMost = true;  // since we are not model, make Topmost so we are always visible while open

			pictureBox1.Image = null;

			input = previous_input;
			Globals.input = input;

			FileComboBox.Text = input;

			update_timer = new System.Timers.Timer();
			update_timer.Elapsed += new System.Timers.ElapsedEventHandler(OnUpdateTimerEvent);
			update_timer.AutoReset = false;
			update_timer.Enabled = false;

			auto_save_timer = new System.Timers.Timer();
			auto_save_timer.Elapsed += new System.Timers.ElapsedEventHandler(OnAutoSaveTimerEvent);
			auto_save_timer.AutoReset = false;
			auto_save_timer.Enabled = false;

			columnSorter = new ListViewColumnSorter();

			columnSorter.SetCaseSensitiveColumnSort(false);  // set the column case sensitive sort setting

			columnSorter.SortColumn = 0;  // by default, sort the first column (filename) in ascending alphabetical order
			columnSorter.Order = SortOrder.Ascending;
			columnSorter.SetSortIcon(FileListView);

			FileListView.ListViewItemSorter = columnSorter;  // enable the ListView column sorting

			ColumnWidthSize = new int[NumColumns];

			if ((Properties.Settings.Default.ListViewColumnWidth0 != 0) && (Properties.Settings.Default.ListViewColumnWidth1 != 0))
			{
				FileListView.Columns[0].Width = Properties.Settings.Default.ListViewColumnWidth0;
				FileListView.Columns[1].Width = Properties.Settings.Default.ListViewColumnWidth1;
			}

			for( int column_index = 0; column_index < NumColumns; column_index++ )
			{
				ColumnWidthSize[column_index] = FileListView.Columns[column_index].Width;
			}

			if (Properties.Settings.Default.FontFamilyName != "")
			{
				FontFamily font_family = new FontFamily(Properties.Settings.Default.FontFamilyName);
				float size = Properties.Settings.Default.FontSizeInPoints;
				FontStyle style = (FontStyle)Properties.Settings.Default.FontStyle;
				GraphicsUnit unit = (GraphicsUnit)Properties.Settings.Default.FontGraphicsUnit;

				FileListView.Font = new Font(font_family, size, style, unit);
				FileComboBox.Font = FileListView.Font;
				FileComboBox.Height = FileComboBox.ItemHeight + 8;
			}
		}

		protected override void WndProc(ref Message m)
		{
			switch( m.Msg )
			{
				case WM_WORKER_DONE:
				{
					try
					{
						pictureBox1.Image = null;

						if ((items != null) && (items.Count > 0))
						{
							FileListView.BeginUpdate();	
							FileListView.Items.AddRange(items.ToArray());
							FileListView.EndUpdate();

							if (input != "")
							{
								int select_index = -1;
								for (int index = 0; index < items.Count; ++index)
								{
									if (String.Equals(FileListView.Items[index].Text, input, StringComparison.OrdinalIgnoreCase))  // if there's an exact match, select it
									{
										select_index = index;
										break;
									}
								}

								if (select_index == -1)  // if we didn't find an exact match, find the first string that begins with the input text and select it
								{
									for (int index = 0; index < items.Count; ++index)
									{
										if (FileListView.Items[index].Text.IndexOf(input, StringComparison.OrdinalIgnoreCase) == 0)
										{
											select_index = index;
											break;
										}
									}
								}

								if (select_index >= 0)
								{
									FileListView.Items[select_index].Selected = true;
									FileListView.Items[select_index].Focused = true;
									FileListView.EnsureVisible(select_index);
								}
							}

							SetListViewLastColumnWidth();
						}
					}
					catch
					{
					}

					WorkerThread.Join();
					WorkerThread = null;
				}
				break;

			}

			base.WndProc(ref m);
		}

		private void OnUpdateTimerEvent(object source, System.Timers.ElapsedEventArgs e)
		{
			update_timer.Enabled = false;

			Invoke((MethodInvoker)delegate { StartWorkerThread(input); });
		}

		private void OnAutoSaveTimerEvent(object source, System.Timers.ElapsedEventArgs e)
		{
			auto_save_timer.Enabled = false;

			bool bSettingsChanged = false;

			if ((Properties.Settings.Default.OpenFileDialog_Left != auto_save_left) ||
				(Properties.Settings.Default.OpenFileDialog_Top != auto_save_top))
			{
				Properties.Settings.Default.OpenFileDialog_Left = auto_save_left;
				Properties.Settings.Default.OpenFileDialog_Top = auto_save_top;

				bSettingsChanged = true;
			}

			if ((Properties.Settings.Default.OpenFileDialog_Width != auto_save_width) ||
				(Properties.Settings.Default.OpenFileDialog_Height != auto_save_height))
			{
				Properties.Settings.Default.OpenFileDialog_Width = auto_save_width;
				Properties.Settings.Default.OpenFileDialog_Height = auto_save_height;

				bSettingsChanged = true;
			}

			if (bSettingsChanged)
			{
				Properties.Settings.Default.Save();
			}
		}

		private void StartWorkerThread(string input)
		{
			FileListView.Items.Clear();

			items = new List<ListViewItem>();

			if ((OpenFileCustomCommand.SolutionFilenames != null) && (OpenFileCustomCommand.SolutionFilenames.Count > 0))
			{
				if (WorkerThread != null)  // if there's already a worker thread running, kill it
				{
					WorkerThread.Abort();
					WorkerThread.Join();
				}

				pictureBox1.Image = Properties.Resources.spin;  // turn on the "processing" spin gif

				WorkerThread = new System.Threading.Thread(new Worker(Handle, input).Run);
				WorkerThread.Priority = System.Threading.ThreadPriority.Normal;
				WorkerThread.Start();  // start the thread running
			}
			else
			{
				pictureBox1.Image = null;
			}
		}

		private void OpenFile_Shown(object sender, EventArgs e)
		{
			CenterToParent();

			if ((Properties.Settings.Default.OpenFileDialog_Width != -1) && (Properties.Settings.Default.OpenFileDialog_Height != -1))
			{
				Size = new System.Drawing.Size(Properties.Settings.Default.OpenFileDialog_Width, Properties.Settings.Default.OpenFileDialog_Height);
			}

			if ((Properties.Settings.Default.OpenFileDialog_Left != -1) && (Properties.Settings.Default.OpenFileDialog_Top != -1))
			{
				Location = new Point(Properties.Settings.Default.OpenFileDialog_Left, Properties.Settings.Default.OpenFileDialog_Top);
			}

			ActiveControl = FileComboBox;

			StartWorkerThread(input);

			bWindowInitComplete = true;
		}

		private void OpenFileDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			Globals.bSingleton = false;
		}

		private void OpenFileDialog_Move(object sender, EventArgs e)
		{
			if (bWindowInitComplete)
			{
				auto_save_left = Location.X;
				auto_save_top = Location.Y;

				auto_save_timer.Enabled = true;
				auto_save_timer.Interval = 500;  // wait 500ms before auto saving
			}
		}

		private void OpenFileDialog_Resize(object sender, EventArgs e)
		{
			if (bWindowInitComplete)
			{
				auto_save_width = Size.Width;
				auto_save_height = Size.Height;

				auto_save_timer.Enabled = true;
				auto_save_timer.Interval = 500;  // wait 500ms before auto saving
			}
		}

		private void OpenFile_SizeChanged(object sender, EventArgs e)
		{
			SetListViewLastColumnWidth();
		}

		private void OK_Button_Click(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			if (WorkerThread != null)  // if there's already a worker thread running, kill it
			{
				WorkerThread.Abort();
			}

			if (FileListView.SelectedItems.Count > 0)
			{
				try
				{
					DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;

					if (dte != null)
					{
						foreach (ListViewItem item in FileListView.SelectedItems)
						{
							if (item.SubItems[1].Text != "")
							{
								if (System.IO.File.Exists(item.SubItems[1].Text))
								{
									dte.ItemOperations.OpenFile(item.SubItems[1].Text);
									dte.ActiveDocument.Activate();  // set focus on the document
									dte.ActiveDocument.ActiveWindow.Activate();  // set focus on the document's window

									this.Activate();
								}
							}
						}
					}
				}
				catch
				{
				}
			}

			Close();
		}

		private void Cancel_Button_Click(object sender, EventArgs e)
		{
			if (WorkerThread != null)  // if there's already a worker thread running, kill it
			{
				WorkerThread.Abort();
			}

			Close();
		}

		private void FileListView_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			ListViewHitTestInfo info = FileListView.HitTest(e.X, e.Y);
			ListViewItem item = info.Item;

			if (item != null)
			{
				try
				{
					DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;

					if (dte != null)
					{
						if (item.SubItems[1].Text != "")
						{
							if (System.IO.File.Exists(item.SubItems[1].Text))
							{
								dte.ItemOperations.OpenFile(item.SubItems[1].Text);
								dte.ActiveDocument.Activate();  // set focus on the document
								dte.ActiveDocument.ActiveWindow.Activate();  // set focus on the document's window

								this.Activate();
							}
						}
					}
				}
				catch
				{
				}
			}

			if (Properties.Settings.Default.CloseDialogOnDoubleclick)
			{
				if (WorkerThread != null)  // if there's already a worker thread running, kill it
				{
					WorkerThread.Abort();
				}

				Close();
			}
		}

		private void FileComboBox_TextChanged(object sender, EventArgs e)
		{
			input = FileComboBox.Text;
			Globals.input = input;

			if (update_timer != null)
			{
				update_timer.Enabled = true;
				update_timer.Interval = 500;  // wait 500ms before starting worker thread
			}
		}

		private void FileListView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
		{
			if (bWindowInitComplete)
			{
				bool bHasColumnWidthChanged = false;

				for( int column_index = 0; column_index < NumColumns; column_index++ )
				{
					if( ColumnWidthSize[column_index] != FileListView.Columns[column_index].Width )
					{
						bHasColumnWidthChanged = true;
					}
				}

				if( bHasColumnWidthChanged )
				{
					Properties.Settings.Default.ListViewColumnWidth0 = FileListView.Columns[0].Width;
					Properties.Settings.Default.ListViewColumnWidth1 = FileListView.Columns[1].Width;
					Properties.Settings.Default.Save();
				}

				SetListViewLastColumnWidth();
			}
		}

		private void SetListViewLastColumnWidth()
		{
			if (bWindowInitComplete)
			{
				// auto size the last column to the width of the list view client area minus the width of the other two columns
				int width = FileListView.ClientSize.Width - FileListView.Columns[0].Width;

				if (width > FileListView.Columns[NumColumns-1].Width)  // only adjust the width if the last column isn't already wide enough
				{
					FileListView.Columns[NumColumns-1].Width = width;
				}
			}
		}

		private void FileListView_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			// Reverse the current sort direction for this column.
			if (e.Column == columnSorter.SortColumn)
			{
				// Reverse the current sort direction for this column.
				if (columnSorter.Order == SortOrder.Ascending)
				{
					columnSorter.Order = SortOrder.Descending;
				}
				else
				{
					columnSorter.Order = SortOrder.Ascending;
				}
			}
			else
			{
				// Set the column number that is to be sorted; default to ascending.
				columnSorter.SortColumn = e.Column;
				columnSorter.Order = SortOrder.Ascending;
			}

			if (columnSorter.Order != SortOrder.None)
			{
				columnSorter.SetSortIcon(FileListView);
			}

			// Perform the sort with these new sort options.
			FileListView.Sort();
		}

		private void OptionsButton_Click(object sender, EventArgs e)
		{
			OpenFileOptionsDialog openFileOptionsDialog = new OpenFileByName.OpenFileOptionsDialog(FileListView);

			if (openFileOptionsDialog.ShowDialog(this) != DialogResult.Cancel)
			{
				FileListView.Font = openFileOptionsDialog.NewFont;

				FileComboBox.Font = FileListView.Font;
				FileComboBox.Height = FileComboBox.ItemHeight + 8;

				FileListView.Invalidate();
			}
		}

		private void OpenFileDialog_KeyDown(object sender, KeyEventArgs e)
		{
			if( e.KeyCode == Keys.Escape )
			{
				if (WorkerThread != null)  // if there's already a worker thread running, kill it
				{
					WorkerThread.Abort();
				}

				Close();
			}
		}
	}
}
