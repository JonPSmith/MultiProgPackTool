// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;

namespace MultiProjPackTool.ParseProjects
{
    public class AppStructureInfo
    {
        public AppStructureInfo(string namespacePrefix, Dictionary<string, ProjectInfo> allProjects, IWriteToConsole writeToConsoleOut)
        {
            NamespacePrefix = namespacePrefix;
            AllProjects = allProjects.Values.ToList();
            SetupAllNuGetInfosDistinctWithChecks(writeToConsoleOut);

            foreach (var project in allProjects.Values)
            {
                foreach (var lookForLinks in allProjects.Values
                    .Where(lookForLinks => lookForLinks.ChildProjects.Select(x => x.ProjectName).Contains(project.ProjectName)))
                {
                    project.ParentProjects.Add(lookForLinks);
                }
            }

            RootProjects = allProjects.Values.Where(x => !x.ParentProjects.Any())
                .OrderBy(x => x.ProjectName.Length).ToList();
        }

        public string NamespacePrefix { get; private set; }

        public List<ProjectInfo> RootProjects { get; private set; }

        public List<ProjectInfo> AllProjects { get; private set; }

        /// <summary>
        /// This dictionary key is the name of the TargetFramework, and the value is all the projects for that TargetFramework
        /// </summary>
        public Dictionary<string, List<NuGetInfo>> NuGetInfosDistinctByFramework { get; private set; }

        /// <summary>
        /// This fills in the <see cref="NuGetInfosDistinctByFramework"/> with the <see cref="NuGetInfo"/>
        /// for each TargetFramework. It warns if a existing NuGet package has an different to the same NuGet package
        /// being added from a different project
        /// </summary>
        /// <param name="writeToConsoleOut"></param>
        private void SetupAllNuGetInfosDistinctWithChecks(IWriteToConsole writeToConsoleOut)
        {
            NuGetInfosDistinctByFramework = new Dictionary<string, List<NuGetInfo>>();
            foreach (var projectInfo in AllProjects)
            {
                foreach (var targetFramework in projectInfo.TargetFrameworks)
                {
                    if (!NuGetInfosDistinctByFramework.ContainsKey(targetFramework))
                        NuGetInfosDistinctByFramework[targetFramework] = new List<NuGetInfo>();
                  
                    foreach(var nuGetInfo in projectInfo.NuGetPackagesByFramework[targetFramework])
                    {
                        var existingNuget = NuGetInfosDistinctByFramework[targetFramework]
                            .SingleOrDefault(x => x.NuGetId == nuGetInfo.NuGetId);
                        if (existingNuget?.NuGetId == nuGetInfo.NuGetId)
                        {
                            //already in - check if same version

                            if (existingNuget.Version != nuGetInfo.Version)
                            {
                                writeToConsoleOut.LogMessage(
                                    $"The NuGet '{nuGetInfo.NuGetId}' in framework '{targetFramework}' " +
                                    $"has an existing version of {existingNuget.Version}, which is different " +
                                    $"from the same NuGet in the {projectInfo.ProjectName} which has version {nuGetInfo.Version}.",
                                    LogLevel.Warning, true);
                            }
                        }
                        else
                        {
                            NuGetInfosDistinctByFramework[targetFramework].Add(nuGetInfo);
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return
                $"Found {AllProjects.Count} projects starting with {NamespacePrefix}, with {NuGetInfosDistinctByFramework.Values.Sum(x => x.Count)} NuGet packages.";
        }
    }
}