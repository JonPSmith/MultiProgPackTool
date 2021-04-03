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
                e.Message.ShouldEqual("StubWriteToConsole: There was an error");
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
            settings.toolSettings.CopyNuGetTo.ShouldEqual("{UserProfile}\\LocalNuGet ");
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
        [InlineData("-i:ThisId", "id")]
        [InlineData("-v:1.0.0-preview1", "version")]
        [InlineData("-n:\"Great update\"", "releaseNotes")]
        public void TestReadSettingsWithOverridesAndChecks_AbsoluteMinimalDataProvided_OverrideNuGet(string option, string propertyName)
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var pathToSettings = TestData.GetTestDataDir() + "\\MinimalSettings\\";

            var argsDecoded = new ArgsDecoded(new[] { option }, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            var settingProp = typeof(allsettingsMetadata).GetProperty(propertyName);
            settingProp.GetValue(settings.metadata).ShouldEqual(option.Substring(3));
        }

        [Theory]
        [InlineData("--verbosity:Debug", "LogLevel")]
        public void TestReadSettingsWithOverridesAndChecks_AbsoluteMinimalDataProvided_OverrideTools(string option, string propertyName)
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var pathToSettings = TestData.GetTestDataDir() + "\\MinimalSettings\\";

            var argsDecoded = new ArgsDecoded(new[] { option }, stubWriter);

            //ATTEMPT
            var settingReader = new SetupSettings(stubWriter, pathToSettings);
            var settings = settingReader.ReadSettingsWithOverridesAndChecks(argsDecoded);

            //VERIFY
            var settingProp = typeof(allsettingsToolSettings).GetProperty(propertyName);
            settingProp.GetValue(settings.toolSettings).ShouldEqual(option.Substring("--verbosity:".Length));
        }
    }
}
