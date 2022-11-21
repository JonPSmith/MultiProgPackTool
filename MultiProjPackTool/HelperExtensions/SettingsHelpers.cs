// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using MultiProjPackTool.BuildNuspec;

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
                    return null;
                return (string)settingProp.GetValue(settings.metadata);
            }
            else
            {
                var settingProp = typeof(allsettingsToolSettings).GetProperty(variableName);
                if (settingProp == null)
                    return null;
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

        public static void CopySettingMetadataIntoNuspec(this allsettings settings, package nuspec)
        {
            foreach (var settingsProp in typeof(allsettingsMetadata).GetProperties())
            {
                var nuspecProp = typeof(packageMetadata).GetProperty(settingsProp.Name);
                if (nuspecProp == null)
                    throw new InvalidOperationException(
                        $"Could not find the property {settingsProp.Name} in the nuspec package");

                //Some properties have multiple parts
                switch (settingsProp.Name)
                {
                    case "license":
                        var getLicense = (allsettingsMetadataLicense)settingsProp.GetValue(settings.metadata);
                        if (getLicense == null)
                            continue;
                        nuspec.metadata.license = new packageMetadataLicense
                        {
                            Value = getLicense.Value,
                            type = getLicense.type
                        };
                        break;
                    case "repository":
                        var repository = (allsettingsMetadataRepository)settingsProp.GetValue(settings.metadata);
                        if (repository == null)
                            continue;
                        nuspec.metadata.repository = new packageMetadataRepository
                        {
                            type = repository.type,
                            url = repository.url,
                            branch = repository.branch,
                            commit = repository.commit,
                        };
                        break;
                    default:
                        nuspecProp.SetValue(nuspec.metadata, settingsProp.GetValue(settings.metadata));
                        break;
                }

               
            }
        }
    }
}