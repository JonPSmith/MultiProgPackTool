// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Extensions.Configuration;
using MultiProjPackTool.HelperExtensions;
using MultiProjPackTool.ParseProjects;

namespace MultiProjPackTool
{
    public class MainCode
    {
        private readonly IConfiguration _configuration;
        private readonly ConsoleOutput _consoleOut;

        //fields filled in by BuildNuGet method
        private Settings _settings;
        private AppStructureInfo _appInfo;

        public MainCode(IConfiguration configuration)
        {
            _configuration = configuration;
            _consoleOut = new ConsoleOutput();
        }

        public void BuildNuGet(string[] args)
        {

        }
    }
}