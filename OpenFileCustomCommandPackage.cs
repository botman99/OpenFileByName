//------------------------------------------------------------------------------
// <copyright file="OpenFileCustomCommandPackage.cs" company="Jeffrey Broome">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

using EnvDTE;
using EnvDTE80;

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
	[ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string)]
	[ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string)]	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	public sealed class OpenFileCustomCommandPackage : Package, IVsSolutionEvents, IVsSolutionLoadEvents
	{
		/// <summary>
		/// OpenFileCustomCommandPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "b7bf7f45-5406-429c-ac89-4971f3d8501e";

		private IVsSolution2 solution = null;
		private uint solutionEventsCookie = 0;

		public static bool bIsUpdatingSolutionFiles = false;
		public static bool bHasSolutionChanged = false;

		private SolutionEvents solutionEvents;
		private ProjectItemsEvents projectItemsEvents;

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
			bHasSolutionChanged = true;

			return VSConstants.S_OK;
		}

		public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			bHasSolutionChanged = true;

			return VSConstants.S_OK;
		}

		public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			bHasSolutionChanged = true;

			return VSConstants.S_OK;
		}

		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			// when a new solution is opened, set the flag to indicate that we are still processing the solution projects
			bIsUpdatingSolutionFiles = true;
			bHasSolutionChanged = true;

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
			bHasSolutionChanged = true;

			return VSConstants.S_OK;
		}

		public int OnAfterBackgroundSolutionLoadComplete()
		{
			// once the solution has been fully loaded, clear the flag to indicate that we are no longer processing the solution projects
			bIsUpdatingSolutionFiles = false;
			bHasSolutionChanged = true;

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
			bHasSolutionChanged = true;
		}

		private void SolutionEvents_ProjectRemoved(Project project)
		{
			bHasSolutionChanged = true;
		}

		private void SolutionEvents_ProjectRenamed(Project project, string oldName)
		{
			// WARNING!!! - This does NOT get called when a VC project is renamed (but does get called when a C# project is renamed)
			bHasSolutionChanged = true;
		}

		private void ProjectItemsEvents_ItemAdded(ProjectItem projectItem)
		{
			bHasSolutionChanged = true;
		}

		private void ProjectItemsEvents_ItemRemoved(ProjectItem projectItem)
		{
			bHasSolutionChanged = true;
		}

		private void ProjectItemsEvents_ItemRenamed(ProjectItem projectItem, string oldName)
		{
			bHasSolutionChanged = true;
		}

		#endregion
	}
}
