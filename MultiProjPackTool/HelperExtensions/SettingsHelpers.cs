// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using MultiProjPackTool.SettingHandling;

namespace MultiProjPackTool.HelperExtensions
{
    public static class SettingsHelpers
    {
        public static string GetSetting(this allsettings settings, bool inNuGetSettings, string variableName)
        {
            if (inNuGetSettings)
            {
                var settingProp = typeof(allsettingsMetadata).GetProperty(variableName);
                if (settingProp == null)
                    return $"The variable name '{variableName}' isn't a valid metadata setting";
                return (string)settingProp.GetValue(settings.metadata);
            }
            else
            {
                var settingProp = typeof(allsettingsToolSettings).GetProperty(variableName);
                if (settingProp == null)
                    return $"The variable name '{variableName}' isn't a valid tool setting";
                return (string)settingProp.GetValue(settings.toolSettings);
            }
        }

        public static string SetSetting(this allsettings settings, bool inNuGetSettings, string variableName, string value)
        {
            if (inNuGetSettings)
            {
                var settingProp = typeof(allsettingsMetadata).GetProperty(variableName);
                if (settingProp == null)
                    return $"The variable name '{variableName}' isn't a valid metadata setting";
                settingProp.SetValue(settings.metadata, value);
            }
            else
            {
                var settingProp = typeof(allsettingsToolSettings).GetProperty(variableName);
                if (settingProp == null)
                    return $"The variable name '{variableName}' isn't a valid tool setting";
                settingProp.SetValue(settings.toolSettings, value);
            }

            return null;  //no errors
        }
    }
}