// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;

namespace MultiProjPackTool.HelperExtensions
{
    public static class ProjectHelpers
    {
        private static readonly string _binDir = $"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}";

        public static string GetCorrectAssemblyPath(this string projectPath, string debugOrRelease, string targetFramework)
        {
            var result = $"{projectPath}\\bin\\{debugOrRelease}\\";
            if (targetFramework != null)
                result += $"{targetFramework}\\";

            return result;
        }
    }
}