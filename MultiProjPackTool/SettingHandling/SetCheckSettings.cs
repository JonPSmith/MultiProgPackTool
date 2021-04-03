// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace MultiProjPackTool.SettingHandling
{
    public class SetCheckSetting
    {
        public SetCheckSetting(bool inNuGetSettings)
        {
            InNuGetSettings = inNuGetSettings;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool InNuGetSettings { get;  }
        public string PropertyName { get; set; }

        //NOTE: if null then it must not be null
        public string DefaultValue { get; set; }

        public void OverrideValue(string value, allsettings settings)
        {
            if (InNuGetSettings)
            {
                var settingProp = typeof(allsettingsMetadata).GetProperty(PropertyName);
                settingProp.SetValue(settings.metadata, value);
            }
            else
            {
                var settingProp = typeof(allsettingsToolSettings).GetProperty(PropertyName);
                settingProp.SetValue(settings.toolSettings ??= new allsettingsToolSettings(), value);
            }
        }
    }
}