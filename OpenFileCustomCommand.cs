//
// Copyright 2020 - Jeffrey "botman" Broome
//

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

using Microsoft.VisualStudio;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using EnvDTE80;

// Some useful docs:
// https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.interop.ivssolution2.getprojectfilesinsolution?view=visualstudiosdk-2019
// https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.interop.ivssolution.getprojectenum?view=visualstudiosdk-2019
// https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.interop.ivshierarchy?view=visualstudiosdk-2019
// https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.ivshierarchyitem?view=visualstudiosdk-2019

namespace OpenFileByName
{
	static class Globals
	{
		public static bool bSingleton = false;  // singleton to prevent opening multiple dialog boxes at once (since dialog box is no longer modal)
		public static string input = "";
	}

	// NOTE: For IVsHierarchy node objects, the node (IVsHierarchyItem) 'itemid' has a limited lifetime and it is NOT safe to keep them and use then
	// without using IVsHierarchy.AdviseHierarchyEvents to listen for changes.  See the remarks here:
	// https://docs.microsoft.com/en-us/dotnet/api/microsoft.visualstudio.shell.interop.ivshierarchy?view=visualstudiosdk-2019

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
		private readonly AsyncPackage package;

		public static HashSet<string> SolutionFilenames = null;

		public static char[] InvalidChars;

		public static bool bForceUpdate = false;  // set when files or projects in the solution are added, removed, or moved.


		/// <summary>
		/// Initializes a new instance of the <see cref="OpenFileCustomCommand"/> class.
		/// Adds our command handlers for menu (commands must exist in the command table file)
		/// </summary>
		/// <param name="package">Owner package, not null.</param>
		/// <param name="commandService">Command service to add command to, not null.</param>
		private OpenFileCustomCommand(AsyncPackage package, OleMenuCommandService commandService)
		{
			this.package = package ?? throw new ArgumentNullException(nameof(package));
			commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

			var menuCommandID = new CommandID(CommandSet, CommandId);
			var menuItem = new MenuCommand(this.Execute, menuCommandID);
			commandService.AddCommand(menuItem);
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
		private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
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
		public static async Task InitializeAsync(AsyncPackage package)
		{
			// Switch to the main thread - the call to AddCommand in OpenFileCustomCommand's constructor requires
			// the UI thread.
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

			OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
			Instance = new OpenFileCustomCommand(package, commandService);
		}

		/// <summary>
		/// This function is the callback used to execute the command when the menu item is clicked.
		/// See the constructor to see how the menu item is associated with this function using
		/// OleMenuCommandService service and MenuCommand class.
		/// </summary>
		/// <param name="sender">Event sender.</param>
		/// <param name="e">Event args.</param>
		private void Execute(object sender, EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				if (Globals.bSingleton)
				{
					return;
				}

				InvalidChars = Path.GetInvalidPathChars();  // get characters not allowed in file paths

				if ((SolutionFilenames == null) || bForceUpdate)
				{
					try
					{
						SolutionFilenames = new HashSet<string>();

						IVsSolution2 solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution2;

						IVsHierarchy solutionHierarchy = (IVsHierarchy)solution;

						IVsProject Project = null;

						GetFilesInSolutionRecursive(solutionHierarchy, VSConstants.VSITEMID_ROOT, ref Project, ref SolutionFilenames);
					}
					catch
					{
					}
				}

				try
				{
					if (Properties.Settings.Default.UseSelectedText)
					{
						DTE2 dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
						if ((dte2 != null) && (dte2.ActiveDocument != null))
						{
							TextSelection selection = dte2.ActiveDocument.Selection as TextSelection;

							if ((selection != null) && (selection.Text != "") && (selection.Text.Length < 260))  // 260 is MAX_PATH
							{
								Globals.input = selection.Text;
							}
						}
					}
				}
				catch
				{
				}

				OpenFileDialog openFileDialog = new OpenFileByName.OpenFileDialog(Globals.input);

				Globals.bSingleton = true;

				openFileDialog.Show();  // don't use a modal dialog so we can set focus to opened documents
			}
			catch (Exception ex)
			{
				Console.WriteLine("Execute Exception: {0}", ex.Message);
			}
		}


		private void GetFilesInSolutionRecursive(IVsHierarchy hierarchy, uint itemId, ref IVsProject Project, ref HashSet<string> SolutionFilenames)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			try
			{
				// NOTE: If itemId == VSConstants.VSITEMID_ROOT then this hierarchy is a solution, project, or folder in the Solution Explorer

				if (hierarchy == null)
				{
					return;
				}

				object ChildObject = null;

				// Get the first visible child node
				if (hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_FirstVisibleChild, out ChildObject) == VSConstants.S_OK)
				{
					while (ChildObject != null)
					{
						if ((ChildObject is int) && ((uint)(int)ChildObject == VSConstants.VSITEMID_NIL))
						{
							break;
						}

						uint visibleChildNodeId = Convert.ToUInt32(ChildObject);

						Guid nestedHierarchyGuid = typeof(IVsHierarchy).GUID;
						IntPtr nestedHiearchyValue = IntPtr.Zero;
						uint nestedItemIdValue = 0;

						// see if the child node has a nested hierarchy (i.e. is it a project?, is it a folder?, etc.)...
						if ((hierarchy.GetNestedHierarchy(visibleChildNodeId, ref nestedHierarchyGuid, out nestedHiearchyValue, out nestedItemIdValue) == VSConstants.S_OK) &&
							(nestedHiearchyValue != IntPtr.Zero && nestedItemIdValue == VSConstants.VSITEMID_ROOT))
						{
							// Get the new hierarchy
							IVsHierarchy nestedHierarchy = System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(nestedHiearchyValue) as IVsHierarchy;
							System.Runtime.InteropServices.Marshal.Release(nestedHiearchyValue);

							if (nestedHierarchy != null)
							{
								IVsProject NewProject = (IVsProject)nestedHierarchy;
								if (NewProject != null)
								{
									// recurse into the new nested hierarchy to handle children...
									GetFilesInSolutionRecursive(nestedHierarchy, VSConstants.VSITEMID_ROOT, ref NewProject, ref SolutionFilenames);
								}
							}
						}
						else
						{
							string projectFilename = "";

							try
							{
								if (Project.GetMkDocument(visibleChildNodeId, out projectFilename) == VSConstants.S_OK)
								{
									if ((projectFilename != null) && (projectFilename.Length > 0) &&
										(!projectFilename.EndsWith("\\")) &&  // some invalid "filenames" will end with '\\'
										(projectFilename.IndexOfAny(InvalidChars) == -1) &&
//										(File.Exists(projectFilename)))  // File.Exists is too slow for very large projects (thousands of files)
										(projectFilename.IndexOf(":", StringComparison.OrdinalIgnoreCase) == 1))  // make sure filename is of the form: drive letter followed by colon
									{
										SolutionFilenames.Add(projectFilename);
									}
								}
							}
							catch (Exception ex)
							{
								Console.WriteLine("Exception: {0}", ex.Message);
							}

							object NodeChildObject = null;

							// see if this regular node has children...
							if (hierarchy.GetProperty(visibleChildNodeId, (int)__VSHPROPID.VSHPROPID_FirstVisibleChild, out NodeChildObject) == VSConstants.S_OK)
							{
								if (NodeChildObject != null)
								{
									if ((NodeChildObject is int) && ((uint)(int)NodeChildObject != VSConstants.VSITEMID_NIL))
									{
										// recurse into the regular node to handle children...
										GetFilesInSolutionRecursive(hierarchy, visibleChildNodeId, ref Project, ref SolutionFilenames);
									}
								}
							}
						}

						ChildObject = null;

						// Get the next visible sibling node
						if (hierarchy.GetProperty(visibleChildNodeId, (int)__VSHPROPID.VSHPROPID_NextVisibleSibling, out ChildObject) != VSConstants.S_OK)
						{
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception: {0}", ex.Message);
			}
		}

	}
}
