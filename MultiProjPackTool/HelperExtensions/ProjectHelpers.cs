// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace MultiProjPackTool.HelperExtensions
{
    public static class ProjectHelpers
    {
        private static readonly string _binDir = $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}";

        //public static string GetExecutingAssemblyPath()
        //{
        //    var pathToManipulate = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //    var indexOfPart = pathToManipulate.IndexOf(_binDir, StringComparison.OrdinalIgnoreCase);
        //    if (indexOfPart <= 0)
        //        throw new Exception($"Did not find '{_binDir}' in the assembly.");

        //    var executingProjectPath = pathToManipulate.Substring(0, indexOfPart);
        //    return executingProjectPath;
        //}

        //public static string GetSolutionPathFromProjectPath(this string executingProjectPath)
        //{
        //    return Path.GetFullPath(Path.Combine(executingProjectPath, "..\\"));
        //}

        //public static string GetSolutionFilename(this string solutionDir, WriteToConsole writeToConsole)
        //{
        //    var files = Directory.GetFiles(solutionDir, "*.sln");
        //    if (files.Length > 1)
        //        throw new Exception( $"Found {files.Length} solution files. I can't handle that!");
        //    if (files.Length != 1)
        //        writeToConsole.LogMessage($"You didn't provide a setting for \"RootName\", and couldn't find a solution (.sln) file.", LogLevel.Error);
        //    return Path.GetFileNameWithoutExtension(files.Single());
        //}

        public static string GetCorrectAssemblyPath(this string projectPath, string debugOrRelease, string targetFramework)
        {
            var result = $"{projectPath}\\bin\\{debugOrRelease}\\";
            if (targetFramework != null)
                result += $"{targetFramework}\\";

            return result;
        }
    }
}