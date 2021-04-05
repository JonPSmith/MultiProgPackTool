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

        private readonly string[] _args;
        private readonly IWriteToConsole _writeToConsole;

        public ToolActions WhatAction { get; }

        public bool DebugMode { get; }

        public string DebugOrRelease => DebugMode ? "Debug" : "Release";

        public bool UpdateNuGetCache { get; }

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
            for (int i = 1; i < _args.Length; i++)
            {
                if (_args[i].StartsWith("-m:") || _args[i].StartsWith("-t:"))
                    OverrideValue(_args[i], settings);
                else
                    _writeToConsole.LogMessage($"I did not understand the option {_args[i]}", LogLevel.Error);
            }
        }

        private void OverrideValue(string arg, allsettings settings)
        {
            var inNuGetSettings = arg.StartsWith("-m:");
            var trimmedArg = arg.Substring(3);
            var indexOfEqual = trimmedArg.IndexOf('=');
            if (indexOfEqual < 0)
                _writeToConsole.LogMessage($"The option '{arg}' wasn't in the format <variableName>=<value>", LogLevel.Error);
            var variableName = trimmedArg.Substring(0, indexOfEqual);
            var value = trimmedArg.Substring(indexOfEqual+1);

            if (inNuGetSettings)
            {
                var settingProp = typeof(allsettingsMetadata).GetProperty(variableName);
                if (settingProp == null)
                    _writeToConsole.LogMessage($"The variable name 'variableName' isn't a valid metadata setting", LogLevel.Error);
                settingProp.SetValue(settings.metadata, value);
            }
            else
            {
                var settingProp = typeof(allsettingsToolSettings).GetProperty(variableName);
                if (settingProp == null)
                    _writeToConsole.LogMessage($"The variable name 'variableName' isn't a valid tools setting", LogLevel.Error);
                settingProp.SetValue(settings.toolSettings ??= new allsettingsToolSettings(), value);
            }
        }

        private void OutputHelpText()
        {
            var messages = new string[]
            {
                "Usage: MultiProjPack <D/R/U> [[--] <additional arguments>...]]",
                "",
                "First parameter:  " +
                "  - D(ebug): This creates a NuGet package using the Debug version of the code",
                "  - R(elease): This creates a NuGet package using the Release version of the code",
                "  - U(pdate): This builds a NuGet package using the Debug code, but also updates the `.dll`s in the NuGet cache",
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