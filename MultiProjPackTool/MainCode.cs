// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

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
        private readonly WriteToConsole _writeToConsoleOut;


        public MainCode(IConfiguration configuration)
        {
            _configuration = configuration;
            _writeToConsoleOut = new WriteToConsole();
        }

        public void BuildNuGet(string[] args, string currentDirectory)
        {
            var argsDecoded = new ArgsDecoded(args, _writeToConsoleOut);
            var settingReader = new SetupSettings(_configuration, _writeToConsoleOut, currentDirectory);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            var appInfo = currentDirectory.ParseModularMonolithApp(settings, _writeToConsoleOut);

            _writeToConsoleOut.LogMessage(appInfo.ToString(), LogLevel.Information);
            var nuspcBuilder = new NuspecBuilder.NuspecBuilder(settings, argsDecoded, appInfo, _writeToConsoleOut);
            nuspcBuilder.BuildNuspecFile(currentDirectory);

            if (!settings.toolSettings.NoAutoPack)
            {
                var processRunner = new RunProcess(settings, argsDecoded, _writeToConsoleOut);
                processRunner.RunPackEct(currentDirectory, appInfo);
            }
        }
    }
}