// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
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
            //Set defaults (some with tests)
            new SetCheckSetting(false)
            {
                PropertyName = "NamespacePrefix",
                GetDefaultValue = (settings, _) => settings.metadata.id
            },
            new SetCheckSetting(false)
            {
                PropertyName = "LogLevel", EnumNames = typeof(LogLevel),
                GetDefaultValue = (_,_) => LogLevel.Information.ToString()
            },
            new SetCheckSetting(false)
            {
                PropertyName = "AddSymbols", EnumNames = typeof(AddSymbolsTypes),
                GetDefaultValue = (_,_) => AddSymbolsTypes.None.ToString()
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
            var results = CheckDefaultSettings.Select(x => x.CheckUpdateSetting(settings, configuration))
                .Where(result => result != null).ToList();
            if (results.Any())
            {
                foreach (var error in results)
                {
                    consoleOut.LogMessage(error, LogLevel.Warning);
                }
                consoleOut.LogMessage($"There were {results.Count} errors on the settings, so cannot continue.", LogLevel.Error);
            }

            //special case: handling {USERPROFILE}
            var copyNuGetTo = settings.GetSetting(false, CopyNuGetToVariableName);
            if (copyNuGetTo?.StartsWith("{USERPROFILE}") == true)
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
        /// <returns></returns>
        private string CheckUpdateSetting(allsettings settings, IConfiguration configuration)
        {
            string SettingSection()
            {
                return (InNuGetSettings ? "<metadata>" : "<toolSettings>");
            }

            var existingValue = settings.GetSetting(InNuGetSettings, PropertyName);
            if (existingValue == null)
            {
                if (GetDefaultValue == null)
                    return $"The setting {PropertyName} in {SettingSection()} must be set to a value";

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