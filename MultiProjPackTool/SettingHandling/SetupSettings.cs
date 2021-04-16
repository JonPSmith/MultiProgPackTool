// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Xml.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MultiProjPackTool.HelperExtensions;

namespace MultiProjPackTool.SettingHandling
{
    public class SetupSettings
    {
        public const string MultiProjPackFileName = "MultiProjPack.xml";
        private readonly IConfiguration _configuration;
        private readonly IWriteToConsole _writeToConsoleOut;
        private readonly string _currentDirectory;

        public SetupSettings(IConfiguration configuration, IWriteToConsole writeToConsoleOut, string currentDirectory)
        {
            _configuration = configuration;
            _writeToConsoleOut = writeToConsoleOut;
            _currentDirectory = currentDirectory;
        }


        //NOTE: should check that it can access some .csproj file before we enter this
        public allsettings ReadSettingsWithOverridesAndChecks(ArgsDecoded argsDecoded)
        {
           var filepath = Path.Combine(_currentDirectory, MultiProjPackFileName);

            if (argsDecoded.WhatAction == ToolActions.CreateSettingsFile)
            {
                if (File.Exists(filepath))
                    _writeToConsoleOut.LogMessage($"A {MultiProjPackFileName} already exists in this folder. I won't overwrite it",
                        LogLevel.Error);

                const string fromFilepath = "SettingHandling\\TypicalMultiProjPack.xml";
                File.Copy(fromFilepath, filepath, true);
                _writeToConsoleOut.LogMessage($"Now fill in the {MultiProjPackFileName} with your information.",
                    LogLevel.Information);
                return null;
            }

            if (!File.Exists(filepath))
                _writeToConsoleOut.LogMessage($"Could not find the {MultiProjPackFileName} in the current directory. Use --CreateSettings to create a empty file",
                    LogLevel.Error);

            var settings = ReadAllSettingsFromXmlFile(filepath);

            //Apply args updates
            argsDecoded.OverrideSettings(settings);
            //Check/SetDefault settings
            SetCheckSetting.CheckUpdateAllSettings(settings, _configuration, _writeToConsoleOut);

            if (argsDecoded.WhatAction == ToolActions.CreateNuGet)
            {
                if (CheckSetDefaultIfPropertyIsNull(settings))
                    _writeToConsoleOut.LogMessage("I have stopped because there were errors in your settings.", LogLevel.Error);

                return settings;
            }

            return null;
        }


        private bool CheckSetDefaultIfPropertyIsNull(allsettings settings)
        {
            bool hasErrors = false;

            return hasErrors;
        }

        private allsettings ReadAllSettingsFromXmlFile(string filepath)
        {
            //see https://docs.microsoft.com/en-us/dotnet/standard/serialization/how-to-deserialize-an-object#to-deserialize-an-object
            XmlSerializer serializerObj = new XmlSerializer(typeof(allsettings));
            FileStream readFileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            // Load the object saved above by using the Deserialize function
            var settings = (allsettings) serializerObj.Deserialize(readFileStream);
            if (settings.metadata == null)
                _writeToConsoleOut.LogMessage("The MultiProjPack settings must have a <metadata> part in it.", LogLevel.Error);
            if (settings.toolSettings == null)
                settings.toolSettings = new allsettingsToolSettings();
            readFileStream.Close();
            return settings;
        }
    }
}