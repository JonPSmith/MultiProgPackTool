using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;

namespace MultiProjPackTool
{
    class Program
    {
        private static IConfigurationRoot _configurationRoot;

        static void Main(string[] args)
        {
            //see https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration#configure-console-apps
            using var host = CreateHostBuilder(args).Build();

            var consoleOut = new WriteToConsole();

            var main = new MainCode(_configurationRoot, consoleOut);
            var currentDirectory = Environment.CurrentDirectory;//AppContext.BaseDirectory with Net5.0 doesn't work!
            main.BuildNuGet(args, currentDirectory);

        }
        //see https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#json-configuration-provider
        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //thanks to https://wakeupandcode.com/generic-host-builder-in-asp-net-core-3-1/
                .ConfigureHostConfiguration(configuration =>
                {
                    configuration
                        .AddEnvironmentVariables();

                    _configurationRoot = configuration.Build();
                });
    }
}
