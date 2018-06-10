//
// Copyright (c) 2018 Jeffrey Broome.
//

using Microsoft.VisualStudio.Shell;
using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;

namespace OpenFileByName
{
	public class BuildProjectFilenames
	{
		public void Run()
		{
			lock (OpenFileCustomCommandPackage.ProjectFilenamesLock)
			{
				try
				{
					DTE2 dte2 = Package.GetGlobalService(typeof(DTE)) as DTE2;
					if (dte2 != null)
					{
						OpenFileCustomCommandPackage.ProjectFilenames = new List<OpenFileCustomCommandPackage.ProjectFileNameData>();

						Solution solution = dte2.Solution;

						foreach (Project project in solution.Projects)
						{
							// add each project to the project filenames list
							OpenFileCustomCommandPackage.ProjectFileNameData projectFilename = new OpenFileCustomCommandPackage.ProjectFileNameData();
							projectFilename.project = project;
							projectFilename.filenames = new List<OpenFileCustomCommandPackage.FilenameData>();
							projectFilename.filenames.AddRange(OpenFileCustomCommandPackage.GetProjectFilenames(project));

							OpenFileCustomCommandPackage.ProjectFilenames.Add(projectFilename);
						}
					}
				}
				catch
				{
				}

				// once the solution has been fully loaded, clear the flag to indicate that we are no longer processing the solution projects
				OpenFileCustomCommandPackage.bIsUpdatingSolutionFiles = false;
			}
		}
	}
}
