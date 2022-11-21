// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;

namespace MultiProjPackTool.ParseProjects
{
    public static class ProjectsParser
    {
        public static AppStructureInfo ScanForProjects(this string directoryToScan, allsettings settings, IWriteToConsole consoleOut)
        {
            var projFilePaths = Directory.GetDirectories(directoryToScan)
                    .Where(dir => Path.GetFileName(dir).StartsWith(settings.toolSettings.NamespacePrefix))
                    .SelectMany(dir =>
                Directory.GetFiles(dir, "*.csproj")).ToList();

            projFilePaths.Where(x => !x.Contains(Path.GetFileNameWithoutExtension(x))).ToList()
                .ForEach(path => consoleOut.LogMessage(
                    $"the project {Path.GetFileName(path)} isn't in a folder of the same name", LogLevel.Warning));

            if (!string.IsNullOrEmpty(settings.toolSettings.ExcludeProjects))
            {
                var excludedProjectNames = settings.toolSettings.ExcludeProjects.Split(',')
                    .Select(x => $"{settings.toolSettings.NamespacePrefix}.{x.Trim()}").ToList() ?? new List<string>();
                foreach (var projectName in excludedProjectNames)
                {
                    var excludedPath =
                        projFilePaths.SingleOrDefault(x => Path.GetFileNameWithoutExtension(x) == projectName);
                    if (excludedPath != null)
                    {
                        projFilePaths.Remove(excludedPath);
                        consoleOut.LogMessage($"Excluded project '{projectName}'", LogLevel.Information);
                    }
                    else
                        consoleOut.LogMessage($"Could not find a project called '{projectName}' to exclude", LogLevel.Warning);
                }
            }

            var pInfo = projFilePaths
                .Select(path => new ProjectInfo(path))
                .ToDictionary(x => x.ProjectName);

            //Now we look at each csproj in turn and fill in
            //- What target framework (or frameworks) it has (used to build Nuspec)
            //- What NuGet packages it uses (used to build Nuspec)
            //- What it links to (used in displaying links)
            foreach (var projFilePath in projFilePaths)
            {
                var filename = Path.GetFileNameWithoutExtension(projFilePath);
                var projectDecoded = DeserializeToObject<Project>(projFilePath);

                var projectToUpdate = pInfo[filename];

                //This finds all the ItemGroups that contain PackageReferences
                var filterNuGets = new FilterNuGetsByCondition(projectDecoded, filename, consoleOut);

                //get target framework(s) and then the NuGets for each frameworks
                //NOTE: This will have duplicates if a dependent Project has the same NuGet

                projectToUpdate.TargetFrameworks = projectDecoded.PropertyGroup.TargetFrameworks?
                    .Split(";").Select(x => x.Trim()).ToList();
                if (projectToUpdate.TargetFrameworks != null)
                {
                    //we need to find the NuGet Packages in the ItemGroup that has a Condition that matches each targetFramework 
                    foreach (var targetFramework in projectToUpdate.TargetFrameworks)
                    {
                        projectToUpdate.NuGetPackagesByFramework[targetFramework] = filterNuGets.IncludeTheseNuGetsWithConditions(targetFramework);
                    }
                }
                else
                {
                    projectToUpdate.TargetFrameworks = new List<string>
                        { projectDecoded.PropertyGroup.TargetFramework };

                    //get the ItemGroup that contains the NuGet packages
                    projectToUpdate.NuGetPackagesByFramework[projectDecoded.PropertyGroup.TargetFramework] = 
                        filterNuGets.IncludeTheseNuGetsNoConditions();
                }

                // Fill in references to other packages
                projectToUpdate.ChildProjects = projectDecoded.ItemGroup
                    ?.SingleOrDefault(x => x?.ProjectReference?.Any() == true)
                    ?.ProjectReference
                    .Select(x => pInfo[Path.GetFileNameWithoutExtension(x.Include)])
                    .ToList() ?? new List<ProjectInfo>();
            }

            return new AppStructureInfo(settings.toolSettings.NamespacePrefix, pInfo, consoleOut);
        }

        private static T DeserializeToObject<T>(this string filepath) where T : class
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using StreamReader sr = new StreamReader(filepath);
            return (T)ser.Deserialize(sr);
        }
    }
}
