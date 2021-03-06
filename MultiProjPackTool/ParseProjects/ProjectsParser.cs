// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;
using MultiProjPackTool.SettingHandling;

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
            //- What target framework it has (used to build Nuspec)
            //- What NuGet packages it uses (used to build Nuspec)
            //- What it links to (used in displaying links)
            foreach (var projFilePath in projFilePaths)
            {
                var filename = Path.GetFileNameWithoutExtension(projFilePath);
                var projectDecoded = DeserializeToObject<Project>(projFilePath);

                var projectToUpdate = pInfo[filename];

                //get target framework
                projectToUpdate.TargetFramework = projectDecoded.PropertyGroup.TargetFramework;

                //get NuGet packages

                projectToUpdate.NuGetPackages = projectDecoded.ItemGroup
                    ?.SingleOrDefault(x => x?.PackageReference?.Any() == true)
                    ?.PackageReference
                    .Select(x => new NuGetInfo(x))
                    .ToList() ?? new List<NuGetInfo>();

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
