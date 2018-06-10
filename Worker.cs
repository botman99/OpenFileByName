//
// Copyright (c) 2018 Jeffrey Broome.
//

using System;

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
				for (int projectIndex = 0; projectIndex < OpenFileCustomCommandPackage.ProjectFilenames.Count; ++projectIndex)
				{
					try
					{
						for( int filenameIndex = 0; filenameIndex < OpenFileCustomCommandPackage.ProjectFilenames[projectIndex].filenames.Count; ++filenameIndex)
						{
							if ((Input == "") || OpenFileCustomCommandPackage.ProjectFilenames[projectIndex].filenames[filenameIndex].name.IndexOf(Input, StringComparison.CurrentCultureIgnoreCase) >= 0)
							{
								ListViewItem item = new ListViewItem(OpenFileCustomCommandPackage.ProjectFilenames[projectIndex].filenames[filenameIndex].name);

								item.SubItems.Add(OpenFileCustomCommandPackage.ProjectFilenames[projectIndex].project.Name);
								item.SubItems.Add(OpenFileCustomCommandPackage.ProjectFilenames[projectIndex].filenames[filenameIndex].filename);

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
