// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
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
    // see https://stackoverflow.com/questions/1408175/execute-unit-tests-serially-rather-than-in-parallel
    [Collection("Sequential")]
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
        public void NuspecBuilder_Group1_Works_FullSettings()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var settings = SettingHelpers.GetFullSettings();
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
            //Check icon
            nuspecData.files.Single(x => x.target != "lib\\net5.0").src.ShouldEqual("..\\images\\icon.png");
            nuspecData.files.Single(x => x.target != "lib\\net5.0").target.ShouldEqual("images\\");
            //Check repository
            nuspecData.metadata.repository.type.ShouldEqual("git");
            nuspecData.metadata.repository.url.ShouldEqual("https://github.com/NuGet/NuGet.Client.git");
            nuspecData.metadata.repository.branch.ShouldEqual("dev");
            nuspecData.metadata.repository.commit.ShouldEqual("e1c65e4524cd70ee6e22abe33e6cb6ec73938cb3");
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
            nuspecData.files.Length.ShouldEqual(5);
            nuspecData.files.All(x => x.target == "lib\\net5.0").ShouldBeTrue();
            nuspecData.files.Select(x => x.src.Substring(x.src.Length - "projectx.dll".Length))
                .ShouldEqual(new[] { "Project1.dll", "Project2.dll", "Project2.pdb", "Project3.dll", "Project3.pdb" });
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

            //ensure .pdb file is there
            var pdbPath = Path.Combine(dirToScan, "Group1.Project1\\bin\\Debug\\net5.0\\Group1.Project1.pdb");
            File.WriteAllText(pdbPath, "dummy content");

            //ATTEMPT
            var builder = new NuspecBuilder(settings, argsDecoded, appInfo, stubWriter);
            builder.BuildNuspecFile(dirToScan);

            //VERIFY
            stubWriter.NumWarnings.ShouldEqual(0);
            dirToScan.NuspecFileExists().ShouldBeTrue();
        }

        [Fact]
        public void NuspecBuilder_Group1_Symbols_MissingSymbol()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output, false);
            var settings = SettingHelpers.GetMinimalSettings();
            var dirToScan = "Group1".GetPathToTestProjectGroups();
            settings.toolSettings.NamespacePrefix = "Group1";
            dirToScan.EnsureNuspecFileDeleted();

            var appInfo = dirToScan.ScanForProjects(settings, stubWriter);
            var argsDecoded = new ArgsDecoded(new[] { "D" }, dirToScan, stubWriter);

            settings.toolSettings.AddSymbols = "Debug";

            //delete a .pdb file
            var pdbPath = Path.Combine(dirToScan, "Group1.Project1\\bin\\Debug\\net5.0\\Group1.Project1.pdb");
            File.Delete(pdbPath);

            //ATTEMPT
            var builder = new NuspecBuilder(settings, argsDecoded, appInfo, stubWriter);
            builder.BuildNuspecFile(dirToScan);

            //VERIFY
            dirToScan.NuspecFileExists().ShouldBeTrue();
            stubWriter.NumWarnings.ShouldEqual(1);
            stubWriter.HighestLogLevel.ShouldEqual(LogLevel.Error);
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

        [Fact]
        public void NuspecBuilder_Group2_WithSymbols()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var settings = SettingHelpers.GetMinimalSettings();
            settings.toolSettings.NamespacePrefix = "Group2";
            var dirToScan = "Group2".GetPathToTestProjectGroups();
            dirToScan.EnsureNuspecFileDeleted();

            settings.toolSettings.AddSymbols = "Debug";

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