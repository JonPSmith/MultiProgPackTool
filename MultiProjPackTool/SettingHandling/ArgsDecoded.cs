// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;

namespace MultiProjPackTool.SettingHandling
{
    public enum ToolActions
    {
        CreateNuGet,
        [Display(Name = "--help", ShortName = "-h", Description = "Lists the features in this tool")]
        ListHelp,
        [Display(Name = "--CreateSettings", Description = "Creates an empty MultiProjPack.xml file for you to fill in")]
        CreateSettingsFile
    }

    public class ArgsDecoded
    {
        private readonly SetCheckSetting[] _overrideViaOptions = new SetCheckSetting[]
        {
            new SetCheckSetting(true) {Name = "-i:", PropertyName = "id", Description = "Sets the NuGet id"},
            new SetCheckSetting(true) {Name = "-v:", PropertyName = "version", Description = "Sets the NuGet version"},
            new SetCheckSetting(true) {Name = "-n:", PropertyName = "releaseNotes", Description = "Set the NuGet releaseNotes"},
            new SetCheckSetting(false) {Name = "--verbosity:", PropertyName = "LogLevel", Description = "Sets level for output. Uses LogLevel names, e.g. Debug, Information, Warning, Error."},
        };

        private readonly string[] _args;
        private readonly IWriteToConsole _writeToConsole;

        public ToolActions WhatAction { get; }

        public bool DebugMode { get; }

        public string DebugOrRelease => DebugMode ? "Debug" : "Release";

        public bool UpdateNuGetCache { get; }

        public string Message { get; }

        public ArgsDecoded(string[] args, IWriteToConsole writeToConsole)
        {
            _args = args;
            _writeToConsole = writeToConsole;
            WhatAction = DecideWhatToDo(args);
            if (WhatAction == ToolActions.CreateNuGet)
            {
                //Now we set the two 
                if (args.Length > 0 && args[0][0] != '-')
                {
                    switch (args[0])
                    {
                        case "U": case "u":
                            DebugMode = true;
                            UpdateNuGetCache = true;
                            _writeToConsole.LogMessage("Building a NuGet package using Debug code and updating existing cached version", LogLevel.Information);
                            break;
                        case "D": case "d":
                            DebugMode = true;
                            _writeToConsole.LogMessage("Building a NuGet package using Debug code", LogLevel.Information);
                            break;
                        case "R": case "r":
                            DebugMode = false;
                            _writeToConsole.LogMessage("Building a NuGet package using Release code", LogLevel.Information);
                            break;
                        default:
                            _writeToConsole.LogMessage("Argument must be build D(ebug) version, R(elease) version, or U(pdate) which is debug + direct update of NuGet cache", 
                                LogLevel.Error);
                            break;
                    }
                }
                else
                {
                    //Defaults 
                    DebugMode = true;
                    _writeToConsole.LogMessage("Building a NuGet package using Debug code", LogLevel.Information);
                }
            }
            else if (WhatAction == ToolActions.ListHelp)
                OutputHelpText();

            //else other section has to handle the creating of an empty xml settings file
        }

        public void OverrideSettings(allsettings settings)
        {
            foreach (var possibleOverride in _overrideViaOptions)
            {
                foreach (var arg in _args)
                {
                    if (arg.StartsWith(possibleOverride.Name ?? "impossible arg"))
                    {
                        var value = arg.Substring(possibleOverride.Name.Length);
                        possibleOverride.OverrideValue(value, settings);
                    }

                }
            }
        }

        private void OutputHelpText()
        {
            var messages = new string[]
            {
                "Usage: MultiProjPack <D/R/U> [[--] <additional arguments>...]]",
                "",
                "First parameter:  D (for Debug), R (for Release) or U (for direct update)",
                "Options:",


            };

            foreach (var message in messages)
            {
                _writeToConsole.LogMessage(message, LogLevel.Information);
            }
        }

        private ToolActions DecideWhatToDo(string[] args)
        {
            foreach (var toolEnum in Enum.GetValues<ToolActions>())
            {
                var displayAtt = typeof(ToolActions).GetMember(toolEnum.ToString())[0]
                    .GetCustomAttribute<DisplayAttribute>();
                if (displayAtt != null && 
                    (args.Contains(displayAtt.Name ?? "impossible arg") || args.Contains(displayAtt.ShortName ?? "impossible arg"))) 
                    return toolEnum;
            }

            return ToolActions.CreateNuGet;
        }


    }
}