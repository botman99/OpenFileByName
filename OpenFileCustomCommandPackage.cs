//
// Copyright (c) 2018 Jeffrey Broome.
//

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;

namespace OpenFileByName
{
	/// <summary>
	/// This is the class that implements the package exposed by this assembly.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The minimum requirement for a class to be considered a valid package for Visual Studio
	/// is to implement the IVsPackage interface and register itself with the shell.
	/// This package uses the helper classes defined inside the Managed Package Framework (MPF)
	/// to do it: it derives from the Package class that provides the implementation of the
	/// IVsPackage interface and uses the registration attributes defined in the framework to
	/// register itself and its components with the shell. These attributes tell the pkgdef creation
	/// utility what data to put into .pkgdef file.
	/// </para>
	/// <para>
	/// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
	/// </para>
	/// </remarks>
	[PackageRegistration(UseManagedResourcesOnly = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[ProvideMenuResource("Menus.ctmenu", 1)]
	[Guid(OpenFileCustomCommandPackage.PackageGuidString)]
	[ProvideAutoLoad(UIContextGuids80.NoSolution)]
	[ProvideAutoLoad(UIContextGuids80.SolutionExists)]
	[ProvideAutoLoad(UIContextGuids80.SolutionHasMultipleProjects)]
	[ProvideAutoLoad(UIContextGuids80.SolutionHasSingleProject)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	public sealed class OpenFileCustomCommandPackage : Package, IVsSolutionEvents, IVsSolutionLoadEvents
	{
		/// <summary>
		/// OpenFileCustomCommandPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "b7bf7f45-5406-429c-ac89-4971f3d8501e";

		private IVsSolution2 solution = null;
		private uint solutionEventsCookie = 0;

		private System.Threading.Thread BuildProjectFilenamesThread = null;

		public static bool bIsUpdatingSolutionFiles = false;

		private SolutionEvents solutionEvents;
		private ProjectItemsEvents projectItemsEvents;

		public class FilenameData
		{
			public string name;
			public string filename;
		}

		public class ProjectFileNameData
		{
			// we need to use actual Project and not "Project.Name" string here, so that when we display the Project in the FindFile Dialog it will be
			// the correct project name even after the project is renamed (ProjectRenamed event doesn't get called for VC projects)
			public Project project;
			public List<FilenameData> filenames;
		}

		public static List<ProjectFileNameData> ProjectFilenames = null;

		public static Object ProjectFilenamesLock = new object();


		/// <summary>
		/// Initializes a new instance of the <see cref="OpenFileCustomCommand"/> class.
		/// </summary>
		public OpenFileCustomCommandPackage()
		{
			// Inside this method you can place any initialization code that does not require
			// any Visual Studio service because at this point the package object is created but
			// not sited yet inside Visual Studio environment. The place to do all the other
			// initialization is the Initialize method.
		}

		#region Package Members


		// IVsSolutionEvents interface
		public int OnAfterCloseSolution(object pUnkReserved)
		{
			if (BuildProjectFilenamesThread != null)
			{
				BuildProjectFilenamesThread.Abort();
				BuildProjectFilenamesThread.Join();

				BuildProjectFilenamesThread = null;
			}

			return VSConstants.S_OK;
		}

		public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			// when a new solution is opened, set the flag to indicate that we are still processing the solution projects
			bIsUpdatingSolutionFiles = true;

			return VSConstants.S_OK;
		}

		public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}


		// IVsSolutionLoadEvents interface
		public int OnBeforeOpenSolution(string pszSolutionFilename)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeBackgroundSolutionLoadBegins()
		{
			return VSConstants.S_OK;
		}

		public int OnQueryBackgroundLoadProjectBatch(out bool pfShouldDelayLoadToNextIdle)
		{
			pfShouldDelayLoadToNextIdle = false;
			return VSConstants.S_OK;
		}

		public int OnBeforeLoadProjectBatch(bool fIsBackgroundIdleBatch)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterLoadProjectBatch(bool fIsBackgroundIdleBatch)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterBackgroundSolutionLoadComplete()
		{
			if (BuildProjectFilenamesThread != null)
			{
				BuildProjectFilenamesThread.Abort();
				BuildProjectFilenamesThread.Join();

				BuildProjectFilenamesThread = null;
			}

			BuildProjectFilenamesThread = new System.Threading.Thread(new BuildProjectFilenames().Run);
			BuildProjectFilenamesThread.Priority = System.Threading.ThreadPriority.Normal;
			BuildProjectFilenamesThread.Start();  // start the thread running

			return VSConstants.S_OK;
		}

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			OpenFileCustomCommand.Initialize(this);
			base.Initialize();

			UnadviseSolutionEvents();
			AdviseSolutionEvents();

			DTE2 dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;

			// set up listeners for solution modified events
			solutionEvents = dte2.Events.SolutionEvents;
			solutionEvents.ProjectAdded += SolutionEvents_ProjectAdded;
			solutionEvents.ProjectRemoved += SolutionEvents_ProjectRemoved;
			solutionEvents.ProjectRenamed += SolutionEvents_ProjectRenamed;

			// set up listeners for project modified events
			projectItemsEvents = ((Events2)dte2.Events).ProjectItemsEvents;
			projectItemsEvents.ItemAdded += ProjectItemsEvents_ItemAdded;
			projectItemsEvents.ItemRemoved += ProjectItemsEvents_ItemRemoved;
			projectItemsEvents.ItemRenamed += ProjectItemsEvents_ItemRenamed;
		}

		/// <summary>
		/// Override of Package.Dispose() function from Microsoft.VisualStudio.Shell
		/// </summary>
		/// <param name="disposing">
		/// Bool Value: True if the object is being disposed, false if it is being finalized.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			UnadviseSolutionEvents();

			base.Dispose(disposing);
		}

		private void AdviseSolutionEvents()
		{
			// Get solution
			solution = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution)) as IVsSolution2;
			if (solution != null)
			{
				// Register for solution events
				solution.AdviseSolutionEvents(this, out solutionEventsCookie);
			}
		}

		private void UnadviseSolutionEvents()
		{
			// Unadvise all events
			if (solution != null && solutionEventsCookie != 0)
			{
				solution.UnadviseSolutionEvents(solutionEventsCookie);
			}
		}

		private void SolutionEvents_ProjectAdded(Project project)
		{
			lock (ProjectFilenamesLock)
			{
				try
				{
					// add the new project to the project filenames list
					OpenFileCustomCommandPackage.ProjectFileNameData projectFilename = new OpenFileCustomCommandPackage.ProjectFileNameData();
					projectFilename.project = project;
					projectFilename.filenames = new List<OpenFileCustomCommandPackage.FilenameData>();
					projectFilename.filenames.AddRange(OpenFileCustomCommandPackage.GetProjectFilenames(project));

					OpenFileCustomCommandPackage.ProjectFilenames.Add(projectFilename);
				}
				catch
				{
				}
			}
		}

		private void SolutionEvents_ProjectRemoved(Project project)
		{
			lock (ProjectFilenamesLock)
			{
				try
				{
					// remove the project from the project filenames list
					for (int index = 0; index < ProjectFilenames.Count; ++index)
					{
						if (ProjectFilenames[index].project.Name == project.Name)
						{
							ProjectFilenames.RemoveAt(index);
						}
					}
				}
				catch
				{
				}
			}
		}

		private void SolutionEvents_ProjectRenamed(Project project, string oldName)
		{
			// WARNING!!! - This does NOT get called when a VC project is renamed (but does get called when a C# project is renamed)

			lock (ProjectFilenamesLock)
			{
				try
				{
					// rename the project in the profect filenames list
					for (int index = 0; index < ProjectFilenames.Count; ++index)
					{
						if (ProjectFilenames[index].project.Name == oldName)
						{
							ProjectFilenames[index].project = project;
						}
					}
				}
				catch
				{
				}
			}
		}

		private void ProjectItemsEvents_ItemAdded(ProjectItem projectItem)
		{
			lock (ProjectFilenamesLock)
			{
				try
				{
					string projectName = projectItem.ContainingProject.Name;

					if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
					{
						for (int projectIndex = 0; projectIndex < ProjectFilenames.Count; ++projectIndex)
						{
							if (ProjectFilenames[projectIndex].project.Name == projectName)
							{
								FilenameData filenameData = new FilenameData();
								filenameData.name = projectItem.Name;
								filenameData.filename = projectItem.get_FileNames(0);

								ProjectFilenames[projectIndex].filenames.Add(filenameData);

								break;
							}
						}
					}
				}
				catch
				{
				}
			}
		}

		private void ProjectItemsEvents_ItemRemoved(ProjectItem projectItem)
		{
			lock (ProjectFilenamesLock)
			{
				try
				{
					string projectName = projectItem.ContainingProject.Name;

					if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
					{
						string projectItemFilename = projectItem.get_FileNames(0);
						bool bDone = false;

						for (int projectIndex = 0; !bDone && (projectIndex < ProjectFilenames.Count); ++projectIndex)
						{
							if (ProjectFilenames[projectIndex].project.Name == projectName)
							{
								for (int itemIndex = 0; !bDone && (itemIndex < ProjectFilenames[projectIndex].filenames.Count); ++itemIndex)
								{
									if (ProjectFilenames[projectIndex].filenames[itemIndex].filename == projectItemFilename)
									{
										ProjectFilenames[projectIndex].filenames.RemoveAt(itemIndex);
										bDone = true;  // break out of both loops
									}
								}
							}
						}
					}
				}
				catch
				{
				}
			}
		}

		private void ProjectItemsEvents_ItemRenamed(ProjectItem projectItem, string oldName)
		{
			lock (ProjectFilenamesLock)
			{
				try
				{
					string projectName = projectItem.ContainingProject.Name;

					if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
					{
						string projectItemFilename = projectItem.get_FileNames(0);
						bool bDone = false;

						for (int projectIndex = 0; !bDone && (projectIndex < ProjectFilenames.Count); ++projectIndex)
						{
							if (ProjectFilenames[projectIndex].project.Name == projectName)
							{
								for (int itemIndex = 0; !bDone && (itemIndex < ProjectFilenames[projectIndex].filenames.Count); ++itemIndex)
								{
									if (ProjectFilenames[projectIndex].filenames[itemIndex].name == oldName)
									{
										ProjectFilenames[projectIndex].filenames[itemIndex].name = projectItem.Name;
										ProjectFilenames[projectIndex].filenames[itemIndex].filename = projectItem.get_FileNames(0);

										bDone = true;  // break out of both loops
									}
								}
							}
						}
					}
				}
				catch
				{
				}
			}
		}


		public static IEnumerable<FilenameData> GetProjectItemFilenames(ProjectItem projectItem)
		{
			List<FilenameData> list = new List<FilenameData>();

			if (projectItem == null)
			{
				return list;
			}

			try
			{
				if (projectItem.SubProject != null)  // i.e. Kind == vsProjectItemKindSubProject
				{
					list.AddRange(GetProjectFilenames(projectItem.SubProject));
				}
				else if ((projectItem.ProjectItems != null) && (projectItem.ProjectItems.Count != 0))  // i.e. Kind == vsProjectItemKindVirtualFolder
				{
					foreach (ProjectItem childProjectItem in projectItem.ProjectItems)
					{
						list.AddRange(GetProjectItemFilenames(childProjectItem));
					}
				}
				else if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
				{
					if ((!projectItem.Name.EndsWith(".vcxproj.filters")) &&
						(!projectItem.Name.EndsWith(".vcxproj.user")) &&
						(!projectItem.Name.EndsWith(".csproj.user")))
					{
						FilenameData filenameData = new FilenameData();
						filenameData.name = projectItem.Name;
						filenameData.filename = projectItem.get_FileNames(0);

						list.Add(filenameData);
					}
				}
			}
			catch
			{
			}

			return list;
		}

		public static IEnumerable<FilenameData> GetProjectFilenames(Project project)
		{
			List<FilenameData> list = new List<FilenameData>();

			if (project == null)
			{
				return list;
			}

			try
			{
				foreach (ProjectItem projectItem in project.ProjectItems)
				{
					if (projectItem != null)
					{
						if (projectItem.Kind == EnvDTE.Constants.vsProjectItemKindPhysicalFile)
						{
							if ((!projectItem.Name.EndsWith(".vcxproj.filters")) &&
								(!projectItem.Name.EndsWith(".vcxproj.user")) &&
								(!projectItem.Name.EndsWith(".csproj.user")))
							{
								FilenameData filenameData = new FilenameData();
								filenameData.name = projectItem.Name;
								filenameData.filename = projectItem.get_FileNames(0);

								list.Add(filenameData);
							}
						}
						else
						{
							list.AddRange(GetProjectItemFilenames(projectItem));
						}
					}
				}
			}
			catch
			{
			}

			return list;
		}

		#endregion
	}
}
