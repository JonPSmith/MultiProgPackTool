using System;
using MultiProjPackTool.SettingHandling;
using Test.Helpers;
using Test.Stubs;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests
{
    public class TestArgsAndSettings
    {
        private readonly ITestOutputHelper _output;

        public TestArgsAndSettings(ITestOutputHelper output)
        {
            _output = output;
        }


        [Theory]
        [InlineData(null, true, false)]
        [InlineData("D", true, false)]
        [InlineData("R", false, false)]
        [InlineData("U", true, true)]
        public void TestArgsUpdateNuGetCache(string param, bool debugMode, bool updateCache)
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var args = param == null ? new string[0] : new[] { param };

            //ATTEMPT
            var argsDecoded = new ArgsDecoded(args, stubWriter);

            //VERIFY
            argsDecoded.WhatAction.ShouldEqual(ToolActions.CreateNuGet);
            argsDecoded.DebugMode.ShouldEqual(debugMode);
            argsDecoded.UpdateNuGetCache.ShouldEqual(updateCache);
        }

        [Theory]
        [InlineData("-h", ToolActions.ListHelp)]
        [InlineData("--help", ToolActions.ListHelp)]
        [InlineData("--CreateSettings", ToolActions.CreateSettingsFile)]
        public void TestArgsOtherActions(string param, ToolActions whatActions)
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();

            //ATTEMPT
            var argsDecoded = new ArgsDecoded(new[] { param }, stubWriter);

            //VERIFY
            argsDecoded.WhatAction.ShouldEqual(whatActions);
        }

        [Fact]
        public void TestReadSettingsWithOverridesAndChecks_NoFileFound()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            TestData.EnsureFileDeleted(SetupSettings.MultiProjPackFileName);
            var pathToSettings = TestData.GetTestDataDir();

            var argsDecoded = new ArgsDecoded(new[] { "D" }, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(SettingHelpers.GetTestConfiguration(), stubWriter, pathToSettings);
            try
            {
                var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);
            }
            catch (Exception e)
            {
                e.Message.ShouldEqual("ERROR: Could not find the MultiProjPack.xml in the current directory. Use --CreateSettings to create a empty file");
                return;
            }

            //VERIFY
            false.ShouldBeTrue("Didn't catch missing file");
        }

        [Fact]
        public void TestReadSettingsWithOverridesAndChecks_CreateNew()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            TestData.EnsureFileDeleted(SetupSettings.MultiProjPackFileName);
            var pathToSettings = TestData.GetTestDataDir();

            var argsDecoded = new ArgsDecoded(new[] { "--CreateSettings" }, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(SettingHelpers.GetTestConfiguration(), stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            var filePath = TestData.GetFilePath(SetupSettings.MultiProjPackFileName);
            filePath.ShouldNotBeNull();
            argsDecoded.WhatAction.ShouldEqual(ToolActions.CreateSettingsFile);
            settings.ShouldBeNull();
        }

        [Fact]
        public void TestReadSettingsWithOverridesAndChecks_AllDataProvided()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var pathToSettings = TestData.GetTestDataDir()+"\\FullSettings\\";

            var argsDecoded = new ArgsDecoded(new[] {"D"}, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(SettingHelpers.GetTestConfiguration(), stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            settings.metadata.id.ShouldEqual("BookApp.Books");
            settings.toolSettings.CopyNuGetTo.ShouldEqual(@"C:\Users\Me\LocalNuGet");
        }

        [Fact]
        public void TestReadSettingsWithOverridesAndChecks_CheckSettingDefaults()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var pathToSettings = TestData.GetTestDataDir() + "\\MinimalSettings\\";

            var argsDecoded = new ArgsDecoded(new[] { "D" }, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(SettingHelpers.GetTestConfiguration(), stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            //settings that are filled in if empty
            settings.toolSettings.NamespacePrefix.ShouldEqual("BookApp.Books");
            settings.toolSettings.LogLevel.ShouldEqual("Information");
            settings.toolSettings.AddSymbols.ShouldEqual("None");
            settings.toolSettings.NuGetCachePath.ShouldEqual(@"C:\Users\Me\.nuget\packages");
            //settings that aren't filled in if empty
            settings.toolSettings.ExcludeProjects.ShouldBeNull();
            settings.toolSettings.CopyNuGetTo.ShouldBeNull();
            settings.toolSettings.NoAutoPack.ShouldBeFalse();
        }

        [Fact]
        public void TestReadSettingsWithOverridesAndChecks_CopyNuGetToUserProfile()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var pathToSettings = TestData.GetTestDataDir() + "\\MinimalSettings\\";

            var argsDecoded = new ArgsDecoded(new[] { "D", "-t:CopyNuGetTo={USERPROFILE}\\MyNuGets" }, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(SettingHelpers.GetTestConfiguration(), stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            settings.toolSettings.CopyNuGetTo.ShouldEqual(@"C:\Users\Me\MyNuGets");
        }

        [Theory]
        [InlineData("-m:id=NewId", "id", "NewId")]
        [InlineData("-m:releaseNotes=\"The release notes\"", "releaseNotes", "\"The release notes\"")]
        public void TestReadSettingsWithOverridesAndChecks_AbsoluteMinimalDataProvided_OverrideNuGetSettings(string option, string propertyName, string propertyValue)
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var pathToSettings = TestData.GetTestDataDir() + "\\MinimalSettings\\";

            var argsDecoded = new ArgsDecoded(new[] { "D", option }, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(SettingHelpers.GetTestConfiguration(), stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            var settingProp = typeof(allsettingsMetadata).GetProperty(propertyName);
            settingProp.GetValue(settings.metadata).ShouldEqual(propertyValue);
        }

        [Theory]
        [InlineData("-t:LogLevel=Debug", "LogLevel", "Debug")]
        [InlineData(@"-t:CopyNuGetTo=C:\MyDir\MyNuGets", "CopyNuGetTo", @"C:\MyDir\MyNuGets")]
        public void TestReadSettingsWithOverridesAndChecks_AbsoluteMinimalDataProvided_OverrideToolSettings(string option, string propertyName, string propertyValue)
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var pathToSettings = TestData.GetTestDataDir() + "\\MinimalSettings\\";

            var argsDecoded = new ArgsDecoded(new[] { "D", option }, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(SettingHelpers.GetTestConfiguration(), stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            var settingProp = typeof(allsettingsToolSettings).GetProperty(propertyName);
            settingProp.GetValue(settings.toolSettings).ShouldEqual(propertyValue);
        }

        [Theory]
        [InlineData("-m:BadFormat")]
        [InlineData("-m:BadName=XXX")]
        [InlineData("-t:BadName=XXX")]
        public void TestReadSettingsWithOverridesAndChecks_AbsoluteMinimalDataProvided_OverrideToolSettings_Bad(string option)
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var pathToSettings = TestData.GetTestDataDir() + "\\MinimalSettings\\";

            var argsDecoded = new ArgsDecoded(new[] { "D", option }, stubWriter);


            //ATTEMPT
            var settingReader = new SetupSettings(SettingHelpers.GetTestConfiguration(), stubWriter, pathToSettings);
            try
            {
                var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);
            }
            catch (Exception e)
            {
                _output.WriteLine(e.Message);
                e.Message.ShouldStartWith("ERROR: ");

                return;
            }

            //VERIFY
            false.ShouldBeTrue("Didn't catch the incorrect options");
        }

    }
}
