// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using MultiProjPackTool.ParseProjects;
using Test.Helpers;
using Test.Stubs;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests
{
    public class TestParseProjects
    {
        private readonly ITestOutputHelper _output;

        public TestParseProjects(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ParseModularMonolithApp_Group1()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var settings = SettingHelpers.GetMinimalSettings();
            settings.toolSettings.NamespacePrefix = "Group1";

            //ATTEMPT
            var pathToProjects = Path.GetFullPath(Path.Combine(TestData.GetCallingAssemblyTopLevelDir() + "\\..\\"));
            var appInfo = pathToProjects.ScanForProjects(settings, stubWriter);

            //VERIFY
            appInfo.AllProjects.Select(x => x.ProjectName).ShouldEqual(new []{ "Group1.Project1", "Group1.Project2", "Group1.Project3" });
            appInfo.NuGetInfosDistinctByFramework.Keys.ToArray().ShouldEqual(new []{ "net10.0" });
            appInfo.NuGetInfosDistinctByFramework.Values.Single().Select(x => x.NuGetId)
                .ShouldEqual(new []{ "Microsoft.Extensions.Logging", "Newtonsoft.Json" });
        }

        [Fact]
        public void ParseModularMonolithApp_Group1_Exclude()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var settings = SettingHelpers.GetMinimalSettings();
            settings.toolSettings.NamespacePrefix = "Group1"; 
            settings.toolSettings.ExcludeProjects = "Project1,Project3";

            //ATTEMPT
            var pathToProjects = Path.GetFullPath(Path.Combine(TestData.GetCallingAssemblyTopLevelDir() + $"\\..\\"));
            var appInfo = pathToProjects.ScanForProjects(settings, stubWriter);

            //VERIFY
            appInfo.AllProjects.Select(x => x.ProjectName).ShouldEqual(new[] { "Group1.Project2" });
            appInfo.NuGetInfosDistinctByFramework.Keys.ToArray().ShouldEqual(new[] { "net10.0" });
            appInfo.NuGetInfosDistinctByFramework.Values.Single().Select(x => x.NuGetId)
                .ShouldEqual(new[] { "Newtonsoft.Json" });
        }

        [Fact]
        public void ParseModularMonolithApp_Group1_Exclude_Bad()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var settings = SettingHelpers.GetMinimalSettings();
            settings.toolSettings.NamespacePrefix = "Group1";
            settings.toolSettings.ExcludeProjects = "Project1,BAD";

            //ATTEMPT
            var pathToProjects = Path.GetFullPath(Path.Combine(TestData.GetCallingAssemblyTopLevelDir() + $"\\..\\"));
            var appInfo = pathToProjects.ScanForProjects(settings, stubWriter);

            //VERIFY
            appInfo.AllProjects.Select(x => x.ProjectName).ShouldEqual(new[] { "Group1.Project2", "Group1.Project3" });
            stubWriter.NumWarnings.ShouldEqual(1);
        }

        [Fact]
        public void ParseModularMonolithApp_Group2()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();
            var settings = SettingHelpers.GetMinimalSettings();
            settings.toolSettings.NamespacePrefix = "Group2";

            //ATTEMPT
            var pathToProjects = "Group2".GetPathToTestProjectGroups();
            var appInfo = pathToProjects.ScanForProjects(settings, stubWriter);

            //VERIFY
            appInfo.AllProjects.Select(x => x.ProjectName).ShouldEqual(new[] { "Group2.Project1", "Group2.Project2" });
            appInfo.NuGetInfosDistinctByFramework.Keys.ToArray().ShouldEqual(new[] { "netstandard2.1", "net10.0" });
            appInfo.NuGetInfosDistinctByFramework["netstandard2.1"].Single().NuGetId.ShouldEqual("Newtonsoft.Json");
            appInfo.NuGetInfosDistinctByFramework["net10.0"].Single().NuGetId.ShouldEqual("Microsoft.Extensions.Logging");
        }
    }
}