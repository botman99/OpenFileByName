//
// Copyright 2020 - Jeffrey "botman" Broome
//

using System;
using System.Windows.Forms;
using System.Drawing;

namespace OpenFileByName
{
	public partial class OpenFileOptionsDialog:Form
	{
		public Font NewFont = null;

		public OpenFileOptionsDialog(ListView FileListView)
		{
			InitializeComponent();

			UseSelectedText_Checkbox.Checked = Properties.Settings.Default.UseSelectedText;
			CloseOnDoubleclick_Checkbox.Checked = Properties.Settings.Default.CloseDialogOnDoubleclick;

			NewFont = FileListView.Font;

			textBox1.Text = string.Format(@"{0} {1} pt {2}", FileListView.Font.Name, FileListView.Font.Size, FileListView.Font.Style.ToString());

			DialogResult = DialogResult.Cancel;
		}

		private void OpenFileOptions_Shown(object sender, EventArgs e)
		{
			CenterToParent();
		}

		private void OKButton_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.UseSelectedText = UseSelectedText_Checkbox.Checked;
			Properties.Settings.Default.CloseDialogOnDoubleclick = CloseOnDoubleclick_Checkbox.Checked;

			Properties.Settings.Default.FontFamilyName = NewFont.FontFamily.Name;
			Properties.Settings.Default.FontStyle = (int)NewFont.Style;
			Properties.Settings.Default.FontSizeInPoints = NewFont.SizeInPoints;
			Properties.Settings.Default.FontGraphicsUnit = (int)NewFont.Unit;

			Properties.Settings.Default.Save();

			DialogResult = DialogResult.OK;
			Close();
		}

		private void CancelButton_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			FontDialog fontDialog = new FontDialog();
			fontDialog.Font = NewFont;

			try
			{
				if (fontDialog.ShowDialog() != DialogResult.Cancel)
				{
					NewFont = fontDialog.Font;
					textBox1.Text = string.Format(@"{0} {1} pt {2}",NewFont.Name, NewFont.Size, NewFont.Style.ToString());
				}
			}
			catch
			{
			}
		}
	}
}
