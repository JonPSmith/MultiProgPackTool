// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using TestSupport.Helpers;

namespace Test.Helpers
{
    public static class GroupHelpers
    {
        public static string GetPathToTestProjectGroups(this string groupDirName)
        {
            return Path.GetFullPath(Path.Combine(TestData.GetCallingAssemblyTopLevelDir() + $"\\..\\"));
        }
    }
}