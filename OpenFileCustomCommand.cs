//
// Copyright (c) 2018 Jeffrey Broome.
//

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using EnvDTE;
using EnvDTE80;
using System.Windows.Forms;

namespace OpenFileByName
{
	/// <summary>
	/// Command handler
	/// </summary>
	internal sealed class OpenFileCustomCommand
	{
		/// <summary>
		/// Command ID.
		/// </summary>
		public const int CommandId = 0x0100;

		/// <summary>
		/// Command menu group (command set GUID).
		/// </summary>
		public static readonly Guid CommandSet = new Guid("963cca7e-1714-4ba5-8252-dbb5857e5929");

		/// <summary>
		/// VS Package that provides this command, not null.
		/// </summary>
		private readonly Package package;

		private string input = "";

		/// <summary>
		/// Initializes a new instance of the <see cref="OpenFileCustomCommand"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		private OpenFileCustomCommand(Package package)
		{
			if (package == null)
			{
				throw new ArgumentNullException("package");
			}

			this.package = package;

			OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			if (commandService != null)
			{
				var menuCommandID = new CommandID(CommandSet, CommandId);
				var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
				commandService.AddCommand(menuItem);
			}
		}

		/// <summary>
		/// Gets the instance of the command.
		/// </summary>
		public static OpenFileCustomCommand Instance
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the service provider from the owner package.
		/// </summary>
		private IServiceProvider ServiceProvider
		{
			get
			{
				return this.package;
			}
		}

		/// <summary>
		/// Initializes the singleton instance of the command.
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		public static void Initialize(Package package)
		{
			Instance = new OpenFileCustomCommand(package);
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked.
		/// See the constructor to see how the menu item is associated with this function using
		/// OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void MenuItemCallback(object sender, EventArgs e)
		{
			if (OpenFileCustomCommandPackage.bIsUpdatingSolutionFiles)
			{
				string message = string.Format(CultureInfo.CurrentCulture, "Solution files are being processed, please wait.", this.GetType().FullName);
				string title = "Open File By Name";

				VsShellUtilities.ShowMessageBox(this.ServiceProvider, message, title,
					OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

				return;
			}

			if (Properties.Settings.Default.UseSelectedText)
			{
				DTE2 dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
				if (dte2.ActiveDocument != null)
				{
					TextSelection selection = dte2.ActiveDocument.Selection as TextSelection;

					if ((selection != null) && (selection.Text != "") && (selection.Text.Length < 260))  // 260 is MAX_PATH
					{
						input = selection.Text;
					}
				}
			}

			OpenFileDialog openFileDialog = new OpenFileByName.OpenFileDialog(input);

			openFileDialog.ShowDialog();

			if (openFileDialog.DialogResult == DialogResult.OK)
			{
				input = openFileDialog.input;
			}
		}
	}
}
