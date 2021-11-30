//
// Copyright 2020 - Jeffrey "botman" Broome
//

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

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
	[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
	[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
	[Guid(OpenFileCustomCommandPackage.PackageGuidString)]
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
	[ProvideMenuResource("Menus.ctmenu", 1)]
	public sealed class OpenFileCustomCommandPackage : AsyncPackage, IVsSolutionEvents, IVsSolutionLoadEvents, IVsTrackProjectDocumentsEvents2
	{
		/// <summary>
		/// OpenFileCustomCommandPackage GUID string.
		/// </summary>
		public const string PackageGuidString = "b7bf7f45-5406-429c-ac89-4971f3d8501e";

		private IVsSolution2 solution = null;
		private uint solutionEventsCookie = 0;

		private IVsTrackProjectDocuments2 ProjectDocuments = null;
		private uint projectDocumentsEventsCookie = 0;

		#region Package Members

		/// <summary>
		/// Initialization of the package; this method is called right after the package is sited, so this is the place
		/// where you can put all the initialization code that rely on services provided by VisualStudio.
		/// </summary>
		/// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
		/// <param name="progress">A provider for progress updates.</param>
		/// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
		protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
		{
			// When initialized asynchronously, the current thread may be a background thread at this point.
			// Do any initialization that requires the UI thread after switching to the UI thread.
			await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

			UnadviseSolutionEvents();
			AdviseSolutionEvents();

		    await OpenFileCustomCommand.InitializeAsync(this);
        }

		private void AdviseSolutionEvents()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			// Get the solution interface
			solution = ServiceProvider.GlobalProvider.GetService(typeof(SVsSolution)) as IVsSolution2;
			if (solution != null)
			{
				// Register for solution events
				solution.AdviseSolutionEvents(this, out solutionEventsCookie);
			}

			// Get the ProjectDocuments interface
			ProjectDocuments = Package.GetGlobalService(typeof(SVsTrackProjectDocuments)) as IVsTrackProjectDocuments2;
			if (ProjectDocuments != null)
			{
				// Register for project documents events
				ProjectDocuments.AdviseTrackProjectDocumentsEvents(this, out projectDocumentsEventsCookie);
			}
		}

		private void UnadviseSolutionEvents()
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			// Unadvise all events
			if (solution != null && solutionEventsCookie != 0)
			{
				solution.UnadviseSolutionEvents(solutionEventsCookie);
			}

			if (ProjectDocuments != null && projectDocumentsEventsCookie != 0)
			{
				ProjectDocuments.UnadviseTrackProjectDocumentsEvents(projectDocumentsEventsCookie);
			}
		}


		// IVsSolutionEvents interface begin
		public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution)
		{
			OpenFileCustomCommand.bForceUpdate = true;

			return VSConstants.S_OK;
		}

		public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel)
		{
			return VSConstants.S_OK;
		}

		public int OnBeforeCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterCloseSolution(object pUnkReserved)
		{
			return VSConstants.S_OK;
		}
		// IVsSolutionEvents interface end


		// IVsSolutionLoadEvents interface begin
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
			OpenFileCustomCommand.bForceUpdate = true;

			return VSConstants.S_OK;
		}
		// IVsSolutionLoadEvents interface end


		// IVsTrackProjectDocumentsEvents2 inteface begin
		public int OnQueryAddFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYADDFILEFLAGS[] rgFlags, VSQUERYADDFILERESULTS[] pSummaryResult, VSQUERYADDFILERESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterAddFilesEx(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDFILEFLAGS[] rgFlags)
		{
			OpenFileCustomCommand.bForceUpdate = true;

			return VSConstants.S_OK;
		}

		public int OnAfterAddDirectoriesEx(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSADDDIRECTORYFLAGS[] rgFlags)
		{
			OpenFileCustomCommand.bForceUpdate = true;

			return VSConstants.S_OK;
		}

		public int OnAfterRemoveFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEFILEFLAGS[] rgFlags)
		{
			OpenFileCustomCommand.bForceUpdate = true;

			return VSConstants.S_OK;
		}

		public int OnAfterRemoveDirectories(int cProjects, int cDirectories, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, VSREMOVEDIRECTORYFLAGS[] rgFlags)
		{
			OpenFileCustomCommand.bForceUpdate = true;

			return VSConstants.S_OK;
		}

		public int OnQueryRenameFiles(IVsProject pProject, int cFiles, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEFILEFLAGS[] rgFlags, VSQUERYRENAMEFILERESULTS[] pSummaryResult, VSQUERYRENAMEFILERESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterRenameFiles(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEFILEFLAGS[] rgFlags)
		{
			OpenFileCustomCommand.bForceUpdate = true;

			return VSConstants.S_OK;
		}

		public int OnQueryRenameDirectories(IVsProject pProject, int cDirs, string[] rgszMkOldNames, string[] rgszMkNewNames, VSQUERYRENAMEDIRECTORYFLAGS[] rgFlags, VSQUERYRENAMEDIRECTORYRESULTS[] pSummaryResult, VSQUERYRENAMEDIRECTORYRESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterRenameDirectories(int cProjects, int cDirs, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgszMkOldNames, string[] rgszMkNewNames, VSRENAMEDIRECTORYFLAGS[] rgFlags)
		{
			OpenFileCustomCommand.bForceUpdate = true;

			return VSConstants.S_OK;
		}

		public int OnQueryAddDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYADDDIRECTORYFLAGS[] rgFlags, VSQUERYADDDIRECTORYRESULTS[] pSummaryResult, VSQUERYADDDIRECTORYRESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryRemoveFiles(IVsProject pProject, int cFiles, string[] rgpszMkDocuments, VSQUERYREMOVEFILEFLAGS[] rgFlags, VSQUERYREMOVEFILERESULTS[] pSummaryResult, VSQUERYREMOVEFILERESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		public int OnQueryRemoveDirectories(IVsProject pProject, int cDirectories, string[] rgpszMkDocuments, VSQUERYREMOVEDIRECTORYFLAGS[] rgFlags, VSQUERYREMOVEDIRECTORYRESULTS[] pSummaryResult, VSQUERYREMOVEDIRECTORYRESULTS[] rgResults)
		{
			return VSConstants.S_OK;
		}

		public int OnAfterSccStatusChanged(int cProjects, int cFiles, IVsProject[] rgpProjects, int[] rgFirstIndices, string[] rgpszMkDocuments, uint[] rgdwSccStatus)
		{
			return VSConstants.S_OK;
		}
		// IVsTrackProjectDocumentsEvents2 inteface end

		#endregion
	}
}
