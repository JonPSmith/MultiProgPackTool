// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using MultiProjPackTool.NuspecBuilder;

namespace Test.Helpers
{
    public static class NuspecFileHelpers
    {
        public const string DebugNuspecFile = "CreateNuGetDebug.nuspec";
        public const string ReleaseNuspecFile = "CreateNuGetRelease.nuspec";

        public static string PathToNuspecFile(this string dirToScan, bool debug = true)
        {
            return dirToScan + (debug ? DebugNuspecFile : ReleaseNuspecFile);
        }

        public static bool NuspecFileExists(this string dirToScan, bool debug = true)
        {
            return File.Exists(dirToScan.PathToNuspecFile(debug));
        }

        public static void EnsureNuspecFileDeleted(this string dirToScan, bool debug = true)
        {
            if (dirToScan.NuspecFileExists(debug))
                File.Delete(dirToScan.PathToNuspecFile(debug));
        }

        public static package DeserializeNuspecFile(this string dirToScan, bool debug = true) 
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(package));

            using StreamReader sr = new StreamReader(dirToScan.PathToNuspecFile(debug));
            var result = (package)ser.Deserialize(sr);
            sr.Close();
            return result;
        }
    }
}