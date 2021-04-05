// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;

namespace MultiProjPackTool.SettingHandling
{
    public class SetCheckSetting
    {
        private enum AddSymbolsTypes { None, Debug, Release, Always}

        private static readonly List<SetCheckSetting> CheckDefaultSettings = new List<SetCheckSetting>
        {
            //Check not null - <metadata> first
            new SetCheckSetting(true) {PropertyName = "id"},
            new SetCheckSetting(true) {PropertyName = "version"},
            //Set defaults (some with tests)
            new SetCheckSetting(false)
                {PropertyName = "NamespacePrefix", GetDefaultValue = settings => settings.metadata.id},
            new SetCheckSetting(false)
            {
                PropertyName = "LogLevel", EnumNames = typeof(LogLevel),
                GetDefaultValue = settings => LogLevel.Information.ToString()
            },
            new SetCheckSetting(false)
            {
                PropertyName = "AddSymbols", EnumNames = typeof(AddSymbolsTypes),
                GetDefaultValue = settings => AddSymbolsTypes.None.ToString()
            }
        };

        public SetCheckSetting(bool inNuGetSettings)
        {
            InNuGetSettings = inNuGetSettings;
        }

        public bool InNuGetSettings { get;  }
        public string PropertyName { get; set; }

        //NOTE: if null then it must not be null
        public Func<allsettings, string> GetDefaultValue { get; set; }

        public Type EnumNames { get; set; }

        public static void CheckUpdateAllSettings(allsettings settings, IWriteToConsole consoleOut)
        {
            var results = CheckDefaultSettings.Select(x => x.CheckUpdateSetting(settings))
                .Where(result => result != null).ToList();
            if (results.Any())
            {
                foreach (var error in results)
                {
                    consoleOut.LogMessage(error, LogLevel.Warning);
                }
                consoleOut.LogMessage($"There were {results.Count} errors on the settings, so cannot continue.", LogLevel.Warning);
            }
        }


        /// <summary>
        /// Checks settings and also sets a default values if a value is null
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        private string CheckUpdateSetting(allsettings settings)
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

                return settings.SetSetting(InNuGetSettings, PropertyName, GetDefaultValue(settings));
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