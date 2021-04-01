// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using MultiProjPackTool.HelperExtensions;
using MultiProjPackTool.ParseProjects;
using MultiProjPackTool.SettingHandling;

namespace MultiProjPackTool
{
    public class MainCode
    {
        private readonly IConfiguration _configuration;
        private readonly WriteToConsole _writeToConsoleOut;

        //fields filled in by BuildNuGet method
        private allsettings _settings;
        private AppStructureInfo _appInfo;

        public MainCode(IConfiguration configuration)
        {
            _configuration = configuration;
            _writeToConsoleOut = new WriteToConsole();
        }

        public void BuildNuGet(string[] args, string currentDirectory)
        {
        }
    }
}