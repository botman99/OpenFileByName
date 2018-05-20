using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using EnvDTE;
using Microsoft.VisualStudio.Shell;

// NOTE: "processing" spin.gif is courtesy of http://www.chimply.com/

namespace OpenFileByName
{
	public partial class OpenFileDialog : Form
	{
		public const int WM_USER = 0x0400;  // Windows private user messages
		public const int WM_WORKER_DONE = WM_USER;

		private System.Threading.Thread WorkerThread = null;

		public static List<ListViewItem> items = null;

		public OpenFileDialog()
		{
			InitializeComponent();

			pictureBox1.Image = null;

			CenterToParent();
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
							FileListView.Items.AddRange(items.ToArray());
						}
					}
					catch
					{
					}

					WorkerThread = null;
				}
				break;

			}

			base.WndProc(ref m);
		}

		private void SetListViewLastColumnWidth()
		{
			// auto size the last column to the width of the list view client area minus the width of the other two columns
			int width = FileListView.ClientSize.Width - (FileListView.Columns[0].Width + FileListView.Columns[1].Width);

			if (width > FileListView.Columns[2].Width)  // only adjust the width if the last column isn't already wide enough
			{
				FileListView.Columns[2].Width = width;
			}
		}

		private void StartWorkerThread(string input)
		{
			FileListView.Items.Clear();

			items = new List<ListViewItem>();

			if ((OpenFileCustomCommand.ProjectFilenames != null) && (OpenFileCustomCommand.ProjectFilenames.Count > 0))
			{
				if (WorkerThread != null)  // if there's already a worker thread running, kill it
				{
					WorkerThread.Abort();
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
			SetListViewLastColumnWidth();

			ActiveControl = FileComboBox;

			StartWorkerThread("");
		}

		private void OpenFile_SizeChanged(object sender, EventArgs e)
		{
			SetListViewLastColumnWidth();
		}

		private void OK_Button_Click(object sender, EventArgs e)
		{
			if (WorkerThread != null)  // if there's already a worker thread running, kill it
			{
				WorkerThread.Abort();
			}

			if (FileListView.SelectedItems.Count > 0)
			{
				DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;

				foreach (ListViewItem item in FileListView.SelectedItems)
				{
					if (item.SubItems[2].Text != "")
					{
						dte.ItemOperations.OpenFile(item.SubItems[2].Text);
					}
				}
			}

			DialogResult = DialogResult.OK;
			Close();
		}

		private void Cancel_Button_Click(object sender, EventArgs e)
		{
			if (WorkerThread != null)  // if there's already a worker thread running, kill it
			{
				WorkerThread.Abort();
			}

			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void FileListView_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			ListViewHitTestInfo info = FileListView.HitTest(e.X, e.Y);
			ListViewItem item = info.Item;

			if (item != null)
			{
				DTE dte = Package.GetGlobalService(typeof(DTE)) as DTE;

				if (item.SubItems[2].Text != "")
				{
					dte.ItemOperations.OpenFile(item.SubItems[2].Text);
				}
			}
		}

		private void FileComboBox_TextChanged(object sender, EventArgs e)
		{
			string input = FileComboBox.Text;

			StartWorkerThread(input);
		}
	}
}
