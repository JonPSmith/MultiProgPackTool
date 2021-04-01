// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MultProjPackTool.HelperExtensions;

namespace MultProjPackTool.ParseProjects
{
    public class AppStructureInfo
    {
        public AppStructureInfo(string rootName, Dictionary<string, ProjectInfo> allProjects, ConsoleOutput consoleOut)
        {
            RootName = rootName;
            AllProjects = allProjects.Values.ToList();
            SetupAllNuGetInfosDistinctWithChecks(consoleOut);

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

        public string RootName { get; private set; }

        public List<ProjectInfo> RootProjects { get; private set; }

        public List<ProjectInfo> AllProjects { get; private set; }

        public Dictionary<string, List<NuGetInfo>> NuGetInfosDistinctByFramework { get; private set; }

        private void SetupAllNuGetInfosDistinctWithChecks(ConsoleOutput consoleOut)
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
                        consoleOut.LogMessage(
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
                $"Found {AllProjects.Count} projects starting with {RootName}, with {NuGetInfosDistinctByFramework.Values.Sum(x => x.Count)} NuGet packages.";
        }
    }
}