// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.BuildNuspec;
using MultiProjPackTool.HelperExtensions;
using MultiProjPackTool.ParseProjects;
using MultiProjPackTool.ProcessHandler;
using MultiProjPackTool.SettingHandling;

namespace MultiProjPackTool
{
    //Commands to install update and uninstall locally
    //To install:   dotnet tool install JonPSmith.MultiProjPack -g --add-source C:\Users\JonPSmith\source\repos\MultiProgPackTool\MultiProjPackTool\nupkg  
    //To update:    dotnet tool update  JonPSmith.MultiProjPack -g --add-source C:\Users\JonPSmith\source\repos\MultiProgPackTool\MultiProjPackTool\nupkg
    //To uninstall: dotnet tool uninstall JonPSmith.MultiProjPack -g
    //
    //NOTE: if is a preview you must the following on the end: --version 1.0.0-preview001

    public class MainCode
    {
        private readonly IConfiguration _configuration;
        private readonly IWriteToConsole _consoleOut;

        public MainCode(IConfiguration configuration, IWriteToConsole consoleOut)
        {
            _configuration = configuration;
            _consoleOut = consoleOut;
        }

        public void BuildNuGet(string[] args, string currentDirectory)
        {
#if DEBUG
            //This allows me to run the console app using F5
            ProjectHelpers.FixDirWhenRunningInDebugMode(ref currentDirectory);
#endif

            CheckFolderHasProject(currentDirectory);

            var argsDecoded = new ArgsDecoded(args, currentDirectory, _consoleOut);

            if (argsDecoded.WhatAction != ToolActions.CreateNuGet)
                //Stop if not creating a Nuspec etc.
                return;

            var settingReader = new SetupSettings(_configuration, _consoleOut, currentDirectory);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //stop if any warnings
            _consoleOut.OutputErrorIfAnyWarnings();

            var upOneLevel = Path.GetFullPath(Path.Combine(currentDirectory + "\\..\\"));
            var appInfo = upOneLevel.ScanForProjects(settings, _consoleOut);

            //stop if any warnings
            _consoleOut.OutputErrorIfAnyWarnings();
            if (!appInfo.AllProjects.Any())
                _consoleOut.LogMessage($"No projects starting with '{settings.toolSettings.NamespacePrefix}' found in current directory.", 
                    LogLevel.Error);

            _consoleOut.LogMessage(appInfo.ToString(), LogLevel.Information);
            var nuspcBuilder = new NuspecBuilder(settings, argsDecoded, appInfo, _consoleOut);
            nuspcBuilder.BuildNuspecFile(currentDirectory);

            //stop if any warnings
            _consoleOut.OutputErrorIfAnyWarnings();

            if (!settings.toolSettings.NoAutoPack)
            {
                var processRunner = new RunProcess(settings, argsDecoded, _consoleOut);
                processRunner.RunPackAnyCopy(currentDirectory, appInfo);
            }
        }

        private void CheckFolderHasProject(string currentDirectory)
        {
            if (!Directory.GetFiles(currentDirectory).Any(x => x.EndsWith(".csproj")))
                _consoleOut.LogMessage("You need to run this tool in a folder containing a .csproj file", LogLevel.Error);
        }
    }
}