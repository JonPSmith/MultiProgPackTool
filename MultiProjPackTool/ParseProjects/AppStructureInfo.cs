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
        public AppStructureInfo(string namespacePrefix, Dictionary<string, ProjectInfo> allProjects, WriteToConsole writeToConsoleOut)
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

        public Dictionary<string, List<NuGetInfo>> NuGetInfosDistinctByFramework { get; private set; }

        private void SetupAllNuGetInfosDistinctWithChecks(WriteToConsole writeToConsoleOut)
        {
            var projectsByFramework = AllProjects.GroupBy(x => x.TargetFramework);
            NuGetInfosDistinctByFramework = new Dictionary<string, List<NuGetInfo>>();
            foreach (var projectsInFramework in projectsByFramework)
            {
                var groupedNuGets = projectsInFramework.SelectMany(x => x.NuGetPackages)
                    .GroupBy(x => x.NuGetId);

                var allNuGets = new List<NuGetInfo>();
                foreach (var groupedNuGet in groupedNuGets
                    .GroupBy(x => x.ToList().Select(z => z.Version)))
                {
                    var versionDistinct = groupedNuGet.Key.Distinct().ToList();
                    if (versionDistinct.Count > 1)
                        writeToConsoleOut.LogMessage(
                            $"{groupedNuGet.Key} NuGet has multiple versions: \n {string.Join("\n", versionDistinct)}",
                            LogLevel.Error);
                    allNuGets.Add(groupedNuGet.Single().First());
                }

                NuGetInfosDistinctByFramework[projectsInFramework.Key] = allNuGets;
            }
        }

        public override string ToString()
        {
            return
                $"Found {AllProjects.Count} projects starting with {NamespacePrefix}, with {NuGetInfosDistinctByFramework.Values.Sum(x => x.Count)} NuGet packages.";
        }
    }
}