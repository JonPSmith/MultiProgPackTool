// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;
using MultiProjPackTool.ParseProjects;
using MultiProjPackTool.SettingHandling;

namespace MultiProjPackTool.ProcessHandler
{
    public class RunProcess
    {
        private readonly allsettings _settings;
        private readonly ArgsDecoded _argsDecoded;
        private readonly IWriteToConsole _consoleOut;

        public RunProcess(allsettings settings, ArgsDecoded argsDecoded, IWriteToConsole consoleOut)
        {
            _settings = settings;
            _argsDecoded = argsDecoded;
            _consoleOut = consoleOut;
        }

        //Once you have build the NuSpecs you run then with the following commands 
        // DEBUG:   dotnet pack -p:NuspecFile=CreateNuGetDebug.nuspec
        // RELEASE: dotnet pack -c Release -p:NuspecFile=CreateNuGetRelease.nuspec
        //
        // With new symbols: dotnet pack -p:NuspecFile=CreateNuGetDebug.nuspec --include-symbols

        public void RunPackAnyCopy(string currentDirectory, AppStructureInfo appInfo)
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet.exe",
                RedirectStandardError = true,
                Arguments = FormPackCommand(currentDirectory),
                WorkingDirectory = currentDirectory
            };
            process.StartInfo = startInfo;
            _consoleOut.LogMessage($"Running \"dotnet {startInfo.Arguments}\"", LogLevel.Information);
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                _consoleOut.LogMessage("dotnet pack failed", LogLevel.Error);
            }
            _consoleOut.LogMessage("Finished dotnet pack...", LogLevel.Information);
            if (!string.IsNullOrEmpty(_settings.toolSettings.CopyNuGetTo))
            {
                var nuGetFromPath = Path.Combine(currentDirectory, _argsDecoded.NuspecFilename + ".nupkg");
                var nuGetToPath = Path.Combine(_settings.toolSettings.CopyNuGetTo, _settings.toolSettings.CopyNuGetTo + ".nupkg");
                var fileIsOverwritten = File.Exists(nuGetToPath);
                if (fileIsOverwritten && !_argsDecoded.UpdateNuGetCache)
                    _consoleOut.LogMessage("NuGet package NOT copied local NuGet server because package with that version already exists.", LogLevel.Warning);
                else
                {
                    File.Copy(nuGetFromPath, nuGetToPath, true);
                    _consoleOut.LogMessage($"Copied created NuGet package to {_settings.toolSettings.CopyNuGetTo}", LogLevel.Information);
                }
            }

            if (_argsDecoded.UpdateNuGetCache && _consoleOut.NumWarnings == 0)
            {
                //Replace the over all the dlls to the 

                var pathToNuGetFolderInCache = Path.Combine(_settings.toolSettings.NuGetCachePath, _settings.metadata.id.ToLower(), _settings.metadata.version);
                //Check that the NuGet is there 
                if (!Directory.Exists(pathToNuGetFolderInCache))
                    _consoleOut.LogMessage("Could not update NuGet as not in the cache. Have you added it yet?.", LogLevel.Error);
                else
                {
                    foreach (var projectInfo in appInfo.AllProjects)
                    {
                        var dllFilename = projectInfo.ProjectName + ".dll";
                        var pathFromDir = Path.GetDirectoryName(projectInfo.ProjectPath)
                            .GetCorrectAssemblyPath(_argsDecoded.DebugOrRelease, projectInfo.TargetFramework);
                        var pathToDir = Path.Combine(pathToNuGetFolderInCache, "lib", projectInfo.TargetFramework);
                        File.Copy(Path.Combine(pathFromDir, dllFilename),
                            Path.Combine(pathToDir, dllFilename), true);
                        _consoleOut.LogMessage($"Updated {dllFilename} in nuget cache.", LogLevel.Debug);

                        var xmlFilename = projectInfo.ProjectName + ".xml";
                        if (File.Exists(Path.Combine(pathFromDir, xmlFilename)))
                        {
                            File.Copy(Path.Combine(pathFromDir, xmlFilename),
                                Path.Combine(pathToDir, xmlFilename), true);
                            _consoleOut.LogMessage($"Updated {xmlFilename} in nuget cache.", LogLevel.Debug);
                        }
                    }
                    _consoleOut.LogMessage("Have updated .dll files in NugGet cache. Use Rebuild Solution to update.", LogLevel.Information);
                }
            }
        }

        private string FormPackCommand(string thisProjPath)
        {
            var command = _argsDecoded.DebugMode
                ? "pack -p:NuspecFile=CreateNuGetDebug.nuspec"
                : "pack -c Release -p:NuspecFile=CreateNuGetRelease.nuspec";

            //command += " --no-build";
            if (_argsDecoded.ShouldAddSymbols(_settings))
                command += " --include-symbols";

            return command;
        }
    }
}