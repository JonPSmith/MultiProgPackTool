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

            package.files = _appInfo.AllProjects.SelectMany(x =>
            {
                var pathToDir = Path.GetDirectoryName(x.ProjectPath)
                    .GetCorrectAssemblyPath(_argsDecoded.DebugOrRelease, x.TargetFramework);

                var dllPath = pathToDir + $"{x.ProjectName}.dll";

                if (!File.Exists(dllPath))
                    _consoleOut.LogMessage($"The project {x.ProjectName} doesn't have a .dll file", LogLevel.Error);

                var result = new List<packageFile>
                {
                    new packageFile
                    {
                        src = dllPath,
                        target = $"lib\\{x.TargetFramework}"
                    }
                };
                _consoleOut.LogMessage($"Added {x.ProjectName}.dll file to NuGet files", LogLevel.Debug);
                var xmlPath = pathToDir + $"{x.ProjectName}.xml";
                if (File.Exists(xmlPath))
                {
                    result.Add(new packageFile
                    {
                        src = xmlPath,
                        target = $"lib\\{x.TargetFramework}"
                    });
                    _consoleOut.LogMessage($"Added {x.ProjectName}.xml file to NuGet files", LogLevel.Debug);
                }

                if (_argsDecoded.ShouldAddSymbols(_settings))
                {
                    var pdbPath = pathToDir + $"{x.ProjectName}.pdb";
                    if (!File.Exists(pdbPath))
                        _consoleOut.LogMessage($"You asked for symbols by {x.ProjectName} doesn't have a .pdb file", LogLevel.Warning);
                    else
                    {
                        result.Add(new packageFile
                        {
                            src = pathToDir + $"{x.ProjectName}.pdb",
                            target = $"lib\\{x.TargetFramework}"
                        });
                    }
                    _consoleOut.LogMessage($"The project {x.ProjectName} symbols (.pdb) file", LogLevel.Debug);
                }
                return result;

            }).ToArray();


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