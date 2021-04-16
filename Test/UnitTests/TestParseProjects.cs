// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using MultiProjPackTool.SettingHandling;
using Test.Stubs;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test
{
    public class TestParseProjects
    {
        private readonly ITestOutputHelper _output;

        readonly Dictionary<string, string> _myConfiguration = new Dictionary<string, string>
        {
            {"OS", "Windows"},
            {"USERPROFILE", @"C:\Users\Me"},
        };

        private readonly IConfiguration _configuration;

        public TestParseProjects(ITestOutputHelper output)
        {
            _output = output;
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(_myConfiguration)
                .Build();
        }

        [Fact]
        public void ParseModularMonolithApp_Project1()
        {
            //SETUP
            var stubWriter = new StubWriteToConsole();

            //ATTEMPT


            //VERIFY

        }
    }
}