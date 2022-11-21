// Copyright (c) 2022 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using MultiProjPackTool.ParseProjects;
using MultiProjPackTool.SettingHandling;
using System.IO;
using System.Linq;
using MultiProjPackTool.BuildNuspec;
using Test.Helpers;
using Test.Stubs;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests
{
    // see https://stackoverflow.com/questions/1408175/execute-unit-tests-serially-rather-than-in-parallel
    [Collection("Sequential")]
    public class TestMultipleTargetFrameworks
    {
        private readonly ITestOutputHelper _output;

        public TestMultipleTargetFrameworks(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void NuspecBuilder_MultiFrameworks_Works()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var settings = SettingHelpers.GetMinimalSettings();
            settings.toolSettings.NamespacePrefix = "MultiFrameworks";
            var dirToScan = "MultiFrameworks".GetPathToTestProjectGroups();
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
        public void ParseModularMonolithApp_MultiFrameworks()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var settings = SettingHelpers.GetMinimalSettings();
            settings.toolSettings.NamespacePrefix = "MultiFrameworks";

            //ATTEMPT
            var pathToProjects = Path.GetFullPath(Path.Combine(TestData.GetCallingAssemblyTopLevelDir() + "\\..\\"));
            var appInfo = pathToProjects.ScanForProjects(settings, stubWriter);

            //VERIFY
            foreach (var project in appInfo.AllProjects)
            {
                _output.WriteLine($"Project: {project.ProjectName}");
                foreach (var targetFramework in project.TargetFrameworks)
                {
                    _output.WriteLine($"  TargetFramework {targetFramework}");
                    foreach (var nuGet in project.NuGetPackagesByFramework[targetFramework])
                    {
                        _output.WriteLine($"       {nuGet.NuGetId}, {nuGet.Version}");
                    }
                }
            }
            appInfo.AllProjects.Select(x => x.ProjectName).ShouldEqual(new[] { "MultiFrameworks.Project1", "MultiFrameworks.Project2" });
            appInfo.NuGetInfosDistinctByFramework.Keys.ToArray().ShouldEqual(new[] { "net6.0", "net7.0", "netstandard2.1" });
        }
    }
}