using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.SettingHandling;
using Test.Stubs;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test
{
    public class TestArgsAndSettings
    {
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
            var settingReader = new SetupSettings(stubWriter, pathToSettings);
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
            var settingReader = new SetupSettings(stubWriter, pathToSettings);
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
            var settingReader = new SetupSettings(stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            settings.metadata.id.ShouldEqual("BookApp.Books");
            settings.toolSettings.CopyNuGetTo.ShouldEqual("{USERPROFILE}\\LocalNuGet");
        }

        [Fact]
        public void TestReadSettingsWithOverridesAndChecks_AbsoluteMinimalDataProvided()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var pathToSettings = TestData.GetTestDataDir() + "\\MinimalSettings\\";

            var argsDecoded = new ArgsDecoded(new[] { "D" }, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            settings.metadata.id.ShouldEqual("BookApp.Books");
            settings.toolSettings.ShouldBeNull();
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
            var settingReader = new SetupSettings(stubWriter, pathToSettings);
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
            var settingReader = new SetupSettings(stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            var settingProp = typeof(allsettingsToolSettings).GetProperty(propertyName);
            settingProp.GetValue(settings.toolSettings).ShouldEqual(propertyValue);
        }

    }
}
