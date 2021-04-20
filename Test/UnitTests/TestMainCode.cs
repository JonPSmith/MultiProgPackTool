// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using MultiProjPackTool;
using Test.Helpers;
using Test.Stubs;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests
{
    public class TestMainCode
    {
        private readonly ITestOutputHelper _output;

        public TestMainCode(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestBasicNoCopy()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole(_output);
            var mainCode = new MainCode(SettingHelpers.GetTestConfiguration(), stubWriter);

            var pathToProjects = "Group1".GetPathToTestProjectGroups();
            var args = new[] {"D", "-t:NamespacePrefix=Group1." };

            //ATTEMPT
            mainCode.BuildNuGet(args, pathToProjects);

            //VERIFY
        }
    }
}