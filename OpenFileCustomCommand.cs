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

		public class FilenameData
		{
			public string Name = "";
			public string Pathname = "";
		}

		public class ProjectFileNameData
		{
			public string ProjectPathName = "";  // "Project/folder/folder/folder"
			public string WindowsPathName = "";  // ProjectPathName with backslashes instead of forward slashes
			public List<FilenameData> Filenames = new List<FilenameData>();
		}

		public static List<ProjectFileNameData> ProjectFilenames = null;
		public static Object ProjectFilenamesLock = new object();

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

				if ((ProjectFilenames == null) || bForceUpdate)
				{
					lock (ProjectFilenamesLock)
					{
						try
						{
							ProjectFilenames = new List<ProjectFileNameData>();

							IVsSolution2 solution = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution2;

							IVsHierarchy solutionHierarchy = (IVsHierarchy)solution;

							IVsProject Project = null;
							ProjectFileNameData ProjectFileNameData = null;

							GetFilesInSolutionRecursive(solutionHierarchy, VSConstants.VSITEMID_ROOT, "", ref Project, ref ProjectFileNameData);
						}
						catch
						{
						}

						/* For Debugging Purposes
						StreamWriter projectsFile = new StreamWriter("D:\\Projects.txt");
						foreach (ProjectFileNameData projectFileNameData in ProjectFilenames)
						{
							projectsFile.WriteLine(projectFileNameData.ProjectPathName);
						}
						projectsFile.Close();

						StreamWriter projectsFilenamesFile = new StreamWriter("D:\\ProjectsFiles.txt");
						foreach (ProjectFileNameData projectFileNameData in ProjectFilenames)
						{
							foreach(FilenameData filenameData in projectFileNameData.Filenames)
							{
								projectsFilenamesFile.WriteLine("{0}, {1}", projectFileNameData.ProjectPathName, filenameData.Pathname);
							}
						}
						projectsFilenamesFile.Close();
						*/
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


		private void GetFilesInSolutionRecursive(IVsHierarchy hierarchy, uint itemId, string ProjectName, ref IVsProject Project, ref ProjectFileNameData ProjectFileNameData)
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
								IVsProject NewProject = null;
								string NewProjectName = "";

								NewProject = (IVsProject)nestedHierarchy;
								if (NewProject != null)
								{
									object nameObject = null;

									if ((nestedHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_Name, out nameObject) == VSConstants.S_OK) && (nameObject != null))
									{
										NewProjectName = (string)nameObject;

										if ((NewProjectName != null) && (NewProjectName.Length > 0))
										{
											if ((ProjectName == null) || (ProjectName.Length == 0))
											{
												ProjectName = NewProjectName;
											}
											else
											{
												ProjectName = String.Format("{0}\\{1}", ProjectName, NewProjectName);  // add the new project to the project path string
											}
										}
									}

									// create the new ProjectFileNameData record...
									ProjectFileNameData NewProjectFileNameData = new ProjectFileNameData();

									NewProjectFileNameData.WindowsPathName = ProjectName;
									NewProjectFileNameData.ProjectPathName = ProjectName.Replace("\\", "/");

									// recurse into the new nested hierarchy to handle children...
									GetFilesInSolutionRecursive(nestedHierarchy, VSConstants.VSITEMID_ROOT, ProjectName, ref NewProject, ref NewProjectFileNameData);

									if ((NewProjectName != null) && (NewProjectName.Length > 0) &&
										(ProjectName.IndexOfAny(InvalidChars) == -1))
									{
										ProjectName = Path.GetDirectoryName(ProjectName);  // strip off the trailing "directory" from the project path string
									}

									if (NewProjectFileNameData.Filenames.Count > 0)
									{
										ProjectFilenames.Add(NewProjectFileNameData);
									}
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
										FilenameData CurrentFilenameData = new FilenameData();
										CurrentFilenameData.Pathname = projectFilename;
										CurrentFilenameData.Name = Path.GetFileName(projectFilename);

										ProjectFileNameData.Filenames.Add(CurrentFilenameData);
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
										GetFilesInSolutionRecursive(hierarchy, visibleChildNodeId, ProjectName, ref Project, ref ProjectFileNameData);
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
