// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;
using MultiProjPackTool.ParseProjects;
using MultiProjPackTool.SettingHandling;

namespace MultiProjPackTool.NuspecBuilder
{
    public class NuspecBuilder
    {
        private readonly allsettings _settings;
        private readonly ArgsDecoded _argsDecoded;
        private readonly AppStructureInfo _appInfo;
        private readonly IWriteToConsole _consoleOut;

        public NuspecBuilder(allsettings settings, ArgsDecoded argsDecoded, AppStructureInfo appInfo, IWriteToConsole consoleOut)
        {
            _settings = settings;
            _argsDecoded = argsDecoded;
            _appInfo = appInfo;
            _consoleOut = consoleOut;
        }


        public void BuildNuspecFile(string currentDirectory)
        {
            var package = new package
            {
                metadata = new packageMetadata()
            };

            _settings.CopySettingMetadataIntoNuspec(package);

            package.metadata.dependencies = _appInfo.NuGetInfosDistinctByFramework.Keys.Select(key =>
                new packageMetadataGroup
                {
                    targetFramework = key,
                    dependency = _appInfo.NuGetInfosDistinctByFramework[key].Select(nuGetInfo =>
                        new packageMetadataGroupDependency
                        {
                            id = nuGetInfo.NuGetId,
                            version = nuGetInfo.Version
                        }).ToArray()
                }).ToArray();

            var diffFrameworks = _appInfo.AllProjects.Select(x => x.TargetFramework).Distinct().ToList();
            if (diffFrameworks.Count() > 1)
                _consoleOut.LogMessage($"The projects use multiple frameworks ({(string.Join(", ", diffFrameworks))}).\n" +
                                      $"That usually has problems unless the project that uses this uses multiple frameworks too.",
                    LogLevel.Warning, true); //it can continue with this warning

            var files = _appInfo.AllProjects.SelectMany(x =>
            {
                var pathToDir = Path.GetDirectoryName(x.ProjectPath)
                    .GetCorrectAssemblyPath(_argsDecoded.DebugOrRelease, x.TargetFramework);

                var dllPath = pathToDir + $"{x.ProjectName}.dll";

                if (!File.Exists(dllPath))
                    _consoleOut.LogMessage($"The project {x.ProjectName} doesn't have a .dll file", LogLevel.Warning);

                var result = new List<packageFile>
                {
                    new packageFile
                    {
                        src = dllPath.GoUpOneLevelUsingRelativePath(currentDirectory),
                        target = $"lib\\{x.TargetFramework}"
                    }
                };
                _consoleOut.LogMessage($"Added {x.ProjectName}.dll file to NuGet files", LogLevel.Debug);


                foreach (var fileType in new[] { ".xml", ".pdb" })
                {
                    var filePath = pathToDir + x.ProjectName + fileType;
                    if (File.Exists(filePath))
                    {
                        result.Add(new packageFile
                        {
                            src = filePath.GoUpOneLevelUsingRelativePath(currentDirectory),
                            target = $"lib\\{x.TargetFramework}"
                        });
                        _consoleOut.LogMessage($"Added {x.ProjectName}{fileType} file to NuGet files", LogLevel.Debug);
                    }
                    else if (_argsDecoded.ShouldAddSymbols(_settings) && fileType == ".pdb")
                    {
                        _consoleOut.LogMessage($"You asked for symbols but project {x.ProjectName} doesn't have a .pdb file", LogLevel.Warning);
                    }
                }

                return result;

            }).ToList();

            //Now we handle the icon (if there)
            if (_settings.metadata.icon != null)
            {
                var iconPath = _settings.toolSettings.IconPath == null
                    ? currentDirectory
                    : Path.Combine(currentDirectory, _settings.toolSettings.IconPath);

                files.Add(new packageFile
                {
                    src = Path.Combine(iconPath, _settings.metadata.icon).GoUpOneLevelUsingRelativePath(currentDirectory),
                    target = "images\\"
                });
                _consoleOut.LogMessage($"Added icon file to NuGet files", LogLevel.Debug);
            }

            package.files = files.ToArray();

            //only continue if there are no warnings
            _consoleOut.OutputErrorIfAnyWarnings();

            //Create/update Nuspec file
            var filename = $"CreateNuGet{_argsDecoded.DebugOrRelease}.nuspec";

            //see https://www.jonasjohn.de/snippets/csharp/xmlserializer-example.htm
            XmlSerializer serializerObj = new XmlSerializer(typeof(package));

            // Create a new file stream to write the serialized object to a file
            TextWriter writeFileStream = new StreamWriter(Path.Combine(currentDirectory, filename));
            serializerObj.Serialize(writeFileStream, package);
            writeFileStream.Close();

            _consoleOut.LogMessage($"Updated {_argsDecoded.DebugOrRelease} Nuspec file: NuGetId: '{_settings.metadata.id}', Version: {_settings.metadata.version} containing {_appInfo.AllProjects.Count} projects.", LogLevel.Information);
        }


    }
}