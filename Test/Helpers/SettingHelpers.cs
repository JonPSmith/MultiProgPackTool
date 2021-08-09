// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MultiProjPackTool.SettingHandling;
using Test.Stubs;
using TestSupport.Helpers;

namespace Test.Helpers
{
    public static class SettingHelpers
    {
        public static allsettings GetMinimalSettings(params string[] args)
        {
            return (TestData.GetTestDataDir() + "\\MinimalSettings\\").SetupSettings(args);
        }

        public static allsettings GetFullSettings(params string[] args)
        {
            return (TestData.GetTestDataDir() + "\\FullSettings\\").SetupSettings(args);
        }

        public static allsettings SetupSettings(this string pathToSettings, params string[] args)
        {
            var stubWriter = new StubWriteToConsole();
            if (args.Length == 0)
                args = new[] {"D"};
            var argsDecoded = new ArgsDecoded(args, pathToSettings, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(GetTestConfiguration(), stubWriter, pathToSettings);
            return settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);
        }

        private static readonly Dictionary<string, string> MyConfiguration = new Dictionary<string, string>
        {
            {"OS", "Windows"},
            {"USERPROFILE", @"C:\Users\Me"},
        };

        public static IConfiguration GetTestConfiguration(Dictionary<string, string> environments = null)
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(environments ?? MyConfiguration)
                .Build();
        }
    }
}