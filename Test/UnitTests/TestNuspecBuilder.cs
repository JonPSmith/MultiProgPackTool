// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using MultiProjPackTool.NuspecBuilder;
using MultiProjPackTool.ParseProjects;
using MultiProjPackTool.SettingHandling;
using Test.Helpers;
using Test.Stubs;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests
{
    public class TestNuspecBuilder
    {
        private readonly ITestOutputHelper _output;

        public TestNuspecBuilder(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void NuspecBuilder_Group1_Works()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var settings = SettingHelpers.GetMinimalSettings();
            settings.toolSettings.NamespacePrefix = "Group1";
            var dirToScan = "Group1".GetPathToTestProjectGroups();
            dirToScan.EnsureNuspecFileDeleted();

            var appInfo = dirToScan.ScanForProjects(settings, stubWriter);
            var argsDecoded = new ArgsDecoded(new[] { "D" }, dirToScan, stubWriter);

            //ATTEMPT
            var builder = new NuspecBuilder(settings, argsDecoded, appInfo, stubWriter);
            builder.BuildNuspecFile(dirToScan);

            //VERIFY
            dirToScan.NuspecFileExists().ShouldBeTrue();
        }

        [Fact]
        public void NuspecBuilder_Group1_CheckNuspec()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var settings = SettingHelpers.GetMinimalSettings();
            settings.toolSettings.NamespacePrefix = "Group1";
            var dirToScan = "Group1".GetPathToTestProjectGroups();
            dirToScan.EnsureNuspecFileDeleted();

            var appInfo = dirToScan.ScanForProjects(settings, stubWriter);
            var argsDecoded = new ArgsDecoded(new[] { "D" }, dirToScan, stubWriter);

            //ATTEMPT
            var builder = new NuspecBuilder(settings, argsDecoded, appInfo, stubWriter);
            builder.BuildNuspecFile(dirToScan);

            //VERIFY
            dirToScan.NuspecFileExists().ShouldBeTrue();
            var nuspecData = dirToScan.DeserializeNuspecFile();
            nuspecData.files.Length.ShouldEqual(3);
            nuspecData.files.All(x => x.target == "lib\\net5.0").ShouldBeTrue();
            nuspecData.files.Select(x => x.src.Substring(x.src.Length - "projectx.dll".Length))
                .ShouldEqual(new[] { "Project1.dll", "Project2.dll", "Project3.dll" });
        }

        [Fact]
        public void NuspecBuilder_Group1_Symbols()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var settings = SettingHelpers.GetMinimalSettings();
            var dirToScan = "Group1".GetPathToTestProjectGroups();
            settings.toolSettings.NamespacePrefix = "Group1";
            dirToScan.EnsureNuspecFileDeleted();

            var appInfo = dirToScan.ScanForProjects(settings, stubWriter);
            var argsDecoded = new ArgsDecoded(new[] { "D" }, dirToScan, stubWriter);

            settings.toolSettings.AddSymbols = "Debug";

            //ATTEMPT
            var builder = new NuspecBuilder(settings, argsDecoded, appInfo, stubWriter);
            builder.BuildNuspecFile(dirToScan);

            //VERIFY
            stubWriter.NumWarnings.ShouldEqual(0);
            dirToScan.NuspecFileExists().ShouldBeTrue();
        }

        [Fact]
        public void NuspecBuilder_Group2()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var settings = SettingHelpers.GetMinimalSettings();
            settings.toolSettings.NamespacePrefix = "Group2";
            var dirToScan = "Group2".GetPathToTestProjectGroups();
            dirToScan.EnsureNuspecFileDeleted();

            var appInfo = dirToScan.ScanForProjects(settings, stubWriter);
            var argsDecoded = new ArgsDecoded(new[] { "D" }, dirToScan, stubWriter);

            //ATTEMPT
            var builder = new NuspecBuilder(settings, argsDecoded, appInfo, stubWriter);
            builder.BuildNuspecFile(dirToScan);

            //VERIFY
            dirToScan.NuspecFileExists().ShouldBeTrue();
        }
    }
}