using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OpenFileByName
{
	class Worker
	{
		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		private static HandleRef FormHandle;
		private string Input;

		public Worker(IntPtr InFormHandle, string InInput)
		{
			FormHandle = new HandleRef(this, InFormHandle);
			Input = InInput;
		}

		public void Run()
		{
			try
			{
				for (int projectIndex = 0; projectIndex < OpenFileCustomCommand.ProjectFilenames.Count; ++projectIndex)
				{
					try
					{
						for( int filenameIndex = 0; filenameIndex < OpenFileCustomCommand.ProjectFilenames[projectIndex].filenames.Count; ++filenameIndex)
						{
							if ((Input == "") || OpenFileCustomCommand.ProjectFilenames[projectIndex].filenames[filenameIndex].name.IndexOf(Input, StringComparison.CurrentCultureIgnoreCase) >= 0)
							{
								ListViewItem item = new ListViewItem(OpenFileCustomCommand.ProjectFilenames[projectIndex].filenames[filenameIndex].name);

								item.SubItems.Add(OpenFileCustomCommand.ProjectFilenames[projectIndex].project.Name);
								item.SubItems.Add(OpenFileCustomCommand.ProjectFilenames[projectIndex].filenames[filenameIndex].filename);

								OpenFileDialog.items.Add(item);
							}
						}
					}
					catch
					{
					}
				}
			}
			catch
			{
			}

			PostMessage(FormHandle, OpenFileDialog.WM_WORKER_DONE, IntPtr.Zero, IntPtr.Zero);
		}
	}
}
