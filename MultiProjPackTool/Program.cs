using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace MultiProjPackTool
{
    class Program
    {
        private static IConfigurationRoot _configurationRoot;

        static void Main(string[] args)
        {
            //see https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration#configure-console-apps
            using var host = CreateHostBuilder(args).Build();

            var main = new MainCode(_configurationRoot);
            main.BuildNuGet(args, Environment.CurrentDirectory);

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
