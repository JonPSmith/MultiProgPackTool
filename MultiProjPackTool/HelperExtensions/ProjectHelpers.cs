// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.IO;

namespace MultiProjPackTool.HelperExtensions
{
    public static class ProjectHelpers
    {
        private static readonly string BinDir = $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}";

        public static string GetCorrectAssemblyPath(this string projectPath, string debugOrRelease, string targetFramework)
        {
            var result = $"{projectPath}{BinDir}{debugOrRelease}\\";
            if (targetFramework != null)
                result += $"{targetFramework}\\";

            return result;
        }

        //Thanks to https://stackoverflow.com/questions/13266756/absolute-to-relative-path
        public static string TurnAbsolutePathToRelativePath(this string absolutePath, string currentDirectory)
        {
            var fileUri = new Uri(absolutePath);
            var referenceUri = new Uri(currentDirectory);
            return Uri.UnescapeDataString(referenceUri.MakeRelativeUri(fileUri).ToString())
                .Replace('/', Path.DirectorySeparatorChar);
        }

        public static string GoUpOneLevelUsingRelativePath(this string absolutePath, string currentDirectory)
        {
            return "..\\" + absolutePath.TurnAbsolutePathToRelativePath(currentDirectory);
        }

        public static void FixDirWhenRunningInDebugMode(ref string currentDirectory)
        {
            var indexOfPart = currentDirectory.IndexOf(BinDir, StringComparison.OrdinalIgnoreCase);
            if (indexOfPart <= 0)
                return;

            var executingProjectPath = currentDirectory.Substring(0, indexOfPart);
            currentDirectory = executingProjectPath;
        }
    }
}