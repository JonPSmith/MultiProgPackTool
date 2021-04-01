// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;

namespace MultiProjPackTool.ParseProjects
{
    public class ProjectInfo
    {
        public ProjectInfo(string projectPath)
        {
            ProjectPath = projectPath;

            ChildProjects = new List<ProjectInfo>();
            ParentProjects = new List<ProjectInfo>();
            NuGetPackages = new List<NuGetInfo>();
        }

        public string ProjectPath { get;  }

        public string ProjectName => Path.GetFileNameWithoutExtension(ProjectPath);

        public string TargetFramework { get; set; }

        public List<ProjectInfo> ChildProjects { get; set; }

        public List<ProjectInfo> ParentProjects { get; set; }

        public List<NuGetInfo> NuGetPackages { get; set; }
    }
}