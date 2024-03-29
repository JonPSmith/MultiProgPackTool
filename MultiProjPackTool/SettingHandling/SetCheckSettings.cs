﻿// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;

namespace MultiProjPackTool.SettingHandling
{
    public class SetCheckSetting
    {
        private const string CopyNuGetToVariableName = "CopyNuGetTo";
        public enum AddSymbolsTypes { None, Debug, Release, Always}

        private static readonly List<SetCheckSetting> CheckDefaultSettings = new List<SetCheckSetting>
        {
            //Check not null - <metadata> first
            new SetCheckSetting(true) {PropertyName = "id"},
            new SetCheckSetting(true) {PropertyName = "version"},
            new SetCheckSetting(true) {PropertyName = "authors"},
            new SetCheckSetting(true) {PropertyName = "description"},
            //Set defaults (some with tests)
            new SetCheckSetting(false)
            {
                PropertyName = "NamespacePrefix",
                GetDefaultValue = (settings, _) => settings.metadata.id
            },
            new SetCheckSetting(false)
            {
                PropertyName = "LogLevel", EnumNames = typeof(LogLevel),
                GetDefaultValue = (x,y) => LogLevel.Information.ToString()
            },
            new SetCheckSetting(false)
            {
                PropertyName = "AddSymbols", EnumNames = typeof(AddSymbolsTypes),
                GetDefaultValue = (x,y) => AddSymbolsTypes.None.ToString()
            },
            new SetCheckSetting(false)
            {
                PropertyName = "NuGetCachePath",
                GetDefaultValue = (_,configuration) => configuration["OS"].Contains("Windows")
                    ? $"{configuration["USERPROFILE"]}\\.nuget\\packages"
                    : "~/.nuget/packages"
            },

        };

        public SetCheckSetting(bool inNuGetSettings)
        {
            InNuGetSettings = inNuGetSettings;
        }

        public bool InNuGetSettings { get;  }
        public string PropertyName { get; set; }

        //NOTE: if null then it must not be null
        public Func<allsettings, IConfiguration, string> GetDefaultValue { get; set; }

        public Type EnumNames { get; set; }

        public static void CheckUpdateAllSettings(allsettings settings, IConfiguration configuration, IWriteToConsole consoleOut)
        {
            CheckDefaultSettings.Select(x => x.CheckUpdateSetting(settings, configuration))
                .Where(result => result != null).ToList()
                .ForEach(error => consoleOut.LogMessage(error, LogLevel.Warning));

            //special case: handling {USERPROFILE}
            var copyNuGetTo = settings.GetSetting(false, CopyNuGetToVariableName);
            if (!string.IsNullOrEmpty(copyNuGetTo) && copyNuGetTo.StartsWith("{USERPROFILE}"))
            {
                var updatedValue = copyNuGetTo.Replace("{USERPROFILE}", configuration["USERPROFILE"]);
                settings.SetSetting(false, "CopyNuGetTo", updatedValue);
            }
        }


        /// <summary>
        /// Checks settings and also sets a default values if a value is null
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="configuration"></param>
        /// <returns>null if OK, otherwise error message</returns>
        private string CheckUpdateSetting(allsettings settings, IConfiguration configuration)
        {
            string SettingSection()
            {
                return (InNuGetSettings ? "<metadata>" : "<toolSettings>");
            }

            var existingValue = settings.GetSetting(InNuGetSettings, PropertyName);
            if (string.IsNullOrEmpty(existingValue))
            { 
                if (GetDefaultValue == null)
                    return $"The setting <{PropertyName}> in {SettingSection()} must be set to a value";

                return settings.SetSetting(InNuGetSettings, PropertyName, GetDefaultValue(settings, configuration));
            }

            if (EnumNames != null)
            {
                if (Enum.GetValues(EnumNames).OfType<object>()
                    .FirstOrDefault(v => v.ToString() == existingValue) == null)
                    return $"The value in the setting {PropertyName} of {SettingSection()} must be one of the following:\n" +
                           string.Join(", ", Enum.GetNames(EnumNames));
            }

            return null;
        }
    }
}