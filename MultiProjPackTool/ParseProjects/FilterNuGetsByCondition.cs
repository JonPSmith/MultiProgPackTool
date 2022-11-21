// Copyright (c) 2022 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using MultiProjPackTool.HelperExtensions;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MultiProjPackTool.ParseProjects;

public class FilterNuGetsByCondition
{
    private readonly ProjectItemGroup[] _itemsWithReferences;
    private readonly string _projectFilename;
    private readonly IWriteToConsole _consoleOut;

    public FilterNuGetsByCondition(Project projectDecoded, string projectFilename, IWriteToConsole consoleOut)
    {
        _itemsWithReferences = projectDecoded.ItemGroup
            ?.Where(x => x?.PackageReference?.Any() == true).ToArray() ?? new ProjectItemGroup[]{};
        _projectFilename = projectFilename;
        _consoleOut = consoleOut;
    }

    public List<NuGetInfo> IncludeTheseNuGetsNoConditions()
    {
        var result = new List<NuGetInfo>();
        foreach (var itemGroup in _itemsWithReferences.Where(x => x.Condition == null))
        {
            result.AddRange(itemGroup.PackageReference.Select(x => new NuGetInfo(x)));
        }
        return result;
    }

    public List<NuGetInfo> IncludeTheseNuGetsWithConditions(string targetFramework)
    {
        if (targetFramework == null) throw new ArgumentNullException(nameof(targetFramework));

        var result = new List<NuGetInfo>();
        foreach (var itemGroup in _itemsWithReferences.Where(x => x.Condition != null))
        {
            if (IncludeThisItemGroupWithConditions(itemGroup, targetFramework))
                result.AddRange(itemGroup.PackageReference.Select(x => new NuGetInfo(x)));
        }
        result.AddRange(IncludeTheseNuGetsNoConditions());
        return result;
    }

    //I couldn't find the definitive format of the .csproj condition, but see
    //https://learn.microsoft.com/en-us/nuget/consume-packages/package-references-in-project-files#adding-a-packagereference-condition
    //and https://markheath.net/post/csproj-conditional-references
    //<ItemGroup Condition=" '$(TargetFramework)' == 'net462' or '$(TargetFramework)' == 'net35'">

    private bool IncludeThisItemGroupWithConditions(ProjectItemGroup itemGroup, string targetFramework)
    {
        if (targetFramework == null)
            _consoleOut.LogMessage($"The {_projectFilename}.csproj has one target framework, " +
                                   $"but has the condition '{itemGroup.Condition}' on on your NuGets. This isn't supported.", LogLevel.Warning);

        var splitCondition = itemGroup.Condition.Split(' ').Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        //a condition should have three parts to the condition, e.g. '$(TargetFramework)' == 'net462', with an "or" between them
        int index = 0;
        while (index < splitCondition.Length)
        {
            if (ShouldInclude(splitCondition, index, targetFramework))
                return true;

            index += 4;
            if (index < splitCondition.Length && splitCondition[index] != "==")
            {
                LogBadCondition(splitCondition);
                return false;
            }
        }

        return false;
    }

    private bool ShouldInclude(string[] splitCondition, int index, string targetFramework)
    {
        if (splitCondition[index] != "'$(TargetFramework)'"
            || splitCondition[index+1] != "=="
            || splitCondition[index + 2][0] != '\'')
        {
            LogBadCondition(splitCondition);
        }

        return splitCondition[index + 2].Substring(1, splitCondition[index + 2].Length - 2) == targetFramework;
    }

    private void LogBadCondition(string[] splitCondition)
    {
        _consoleOut.LogMessage(
            $"The MultiProjPackTool can't understand the condition '<ItemGroup Condition=\" {string.Join(' ', splitCondition)}\">' " +
            $"in the {_projectFilename}.csproj file ", LogLevel.Warning);
    }
}