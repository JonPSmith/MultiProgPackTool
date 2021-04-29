// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using MultiProjPackTool.SettingHandling;

namespace MultiProjPackTool.HelperExtensions
{
    public static class FileNameHelpers
    {
        public static string FormNupkgFilename(this allsettings settings)
        {
            return $"{settings.metadata.id}.{settings.metadata.version}.nupkg";
        }
    }
}