// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace MultiProjPackTool.ParseProjects
{
    public class NuGetInfo
    {
        public string NuGetId { get; set; }

        public string Version { get; set; }

        public NuGetInfo(ProjectItemGroupPackageReference xml)
        {
            NuGetId = xml.Include;
            Version = xml.Version;
        }
    }
}