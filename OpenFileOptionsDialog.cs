//
// Copyright 2020 - Jeffrey "botman" Broome
//

using System;
using System.Windows.Forms;

namespace OpenFileByName
{
	public partial class OpenFileOptionsDialog:Form
	{
		public OpenFileOptionsDialog()
		{
			InitializeComponent();

			UseSelectedText_Checkbox.Checked = Properties.Settings.Default.UseSelectedText;
			CloseOnDoubleclick_Checkbox.Checked = Properties.Settings.Default.CloseDialogOnDoubleclick;
		}

		private void OpenFileOptions_Shown(object sender, EventArgs e)
		{
			CenterToParent();
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void UseSelectedText_Checkbox_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.UseSelectedText = UseSelectedText_Checkbox.Checked;

			Properties.Settings.Default.Save();
		}

		private void CloseOnDoubleclick_Checkbox_CheckedChanged(object sender, EventArgs e)
		{
			Properties.Settings.Default.CloseDialogOnDoubleclick = CloseOnDoubleclick_Checkbox.Checked;

			Properties.Settings.Default.Save();
		}

		private void OpenFileOptionsDialog_KeyDown(object sender, KeyEventArgs e)
		{
			if( e.KeyCode == Keys.Escape )
			{
				DialogResult = DialogResult.OK;
				Close();
			}
		}
	}
}
