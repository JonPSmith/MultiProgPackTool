// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;
using MultiProjPackTool.NuspecBuilder;
using MultiProjPackTool.ParseProjects;
using MultiProjPackTool.ProcessHandler;
using MultiProjPackTool.SettingHandling;

namespace MultiProjPackTool
{
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
            var argsDecoded = new ArgsDecoded(args, currentDirectory, _consoleOut);

            if (argsDecoded.WhatAction != ToolActions.CreateNuGet)
                //Stop if not creating a Nuspec etc.
                return;

            var settingReader = new SetupSettings(_configuration, _consoleOut, currentDirectory);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //stop if any warnings
            _consoleOut.OutputErrorIfAnyWarnings();

            var appInfo = currentDirectory.ScanForProjects(settings, _consoleOut);

            //stop if any warnings
            _consoleOut.OutputErrorIfAnyWarnings();
            if (!appInfo.AllProjects.Any())
                _consoleOut.LogMessage($"No projects starting with '{settings.toolSettings.NamespacePrefix}' found in current directory.", 
                    LogLevel.Error);

            _consoleOut.LogMessage(appInfo.ToString(), LogLevel.Information);
            var nuspcBuilder = new NuspecBuilder.NuspecBuilder(settings, argsDecoded, appInfo, _consoleOut);
            nuspcBuilder.BuildNuspecFile(currentDirectory);

            //stop if any warnings
            _consoleOut.OutputErrorIfAnyWarnings();

            if (!settings.toolSettings.NoAutoPack)
            {
                var processRunner = new RunProcess(settings, argsDecoded, _consoleOut);
                processRunner.RunPackAnyCopy(currentDirectory, appInfo);
            }
        }
    }
}