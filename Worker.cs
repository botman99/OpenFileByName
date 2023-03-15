//
// Copyright 2020 - Jeffrey "botman" Broome
//

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
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
				foreach(string FilePath in OpenFileCustomCommand.SolutionFilenames)
				{
					string FileName = Path.GetFileName(FilePath);

					if ((Input == "") || FileName.IndexOf(Input, StringComparison.CurrentCultureIgnoreCase) >= 0)
					{
						ListViewItem item = new ListViewItem(FileName);

						item.SubItems.Add(FilePath);

						OpenFileDialog.items.Add(item);
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
