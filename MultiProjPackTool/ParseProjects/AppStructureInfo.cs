// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
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
        /// for each TargetFramework
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
                    {
                        NuGetInfosDistinctByFramework[targetFramework] = projectInfo.NuGetPackages;
                    }
                    else
                    {
                        var currentPackages = NuGetInfosDistinctByFramework[targetFramework];
                        currentPackages.AddRange(projectInfo.NuGetPackages);
                        NuGetInfosDistinctByFramework[targetFramework] = currentPackages;
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