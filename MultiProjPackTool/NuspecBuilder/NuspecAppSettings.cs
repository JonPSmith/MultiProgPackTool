// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace MultiProjPackTool.NuspecBuilder
{
    public class NuspecAppSettings
    {
        public object RootName { get; set; }
        public object MainProject { get; set; }
        public object NuGetId { get; set; }
        public object Version { get; set; }
        public string Authors { get; set; }
        public string Description { get; set; }
        public string ProjectUrl { get; set; }


    }

}