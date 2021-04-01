﻿// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace MultiProjPackTool.SettingHandling
{


    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class allsettings
    {

        private allsettingsNugetSettings nugetSettingsField;

        private allsettingsToolSettings toolSettingsField;

        /// <remarks/>
        public allsettingsNugetSettings nugetSettings
        {
            get
            {
                return this.nugetSettingsField;
            }
            set
            {
                this.nugetSettingsField = value;
            }
        }

        /// <remarks/>
        public allsettingsToolSettings toolSettings
        {
            get
            {
                return this.toolSettingsField;
            }
            set
            {
                this.toolSettingsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class allsettingsNugetSettings
    {

        private string idField;

        private string versionField;

        private string titleField;

        private string authorsField;

        private string ownersField;

        private string companyField;

        private string productField;

        private string copyrightField;

        private string descriptionField;

        private string releaseNotesField;

        private string licenseField;

        private string packageLicenseFileField;

        private string projectUrlField;

        private string iconUrlField;

        private allsettingsNugetSettingsRepository repositoryField;

        private string tagsField;

        /// <remarks/>
        public string id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        public string authors
        {
            get
            {
                return this.authorsField;
            }
            set
            {
                this.authorsField = value;
            }
        }

        /// <remarks/>
        public string owners
        {
            get
            {
                return this.ownersField;
            }
            set
            {
                this.ownersField = value;
            }
        }

        /// <remarks/>
        public string company
        {
            get
            {
                return this.companyField;
            }
            set
            {
                this.companyField = value;
            }
        }

        /// <remarks/>
        public string product
        {
            get
            {
                return this.productField;
            }
            set
            {
                this.productField = value;
            }
        }

        /// <remarks/>
        public string copyright
        {
            get
            {
                return this.copyrightField;
            }
            set
            {
                this.copyrightField = value;
            }
        }

        /// <remarks/>
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        public string releaseNotes
        {
            get
            {
                return this.releaseNotesField;
            }
            set
            {
                this.releaseNotesField = value;
            }
        }

        /// <remarks/>
        public string license
        {
            get
            {
                return this.licenseField;
            }
            set
            {
                this.licenseField = value;
            }
        }

        /// <remarks/>
        public string packageLicenseFile
        {
            get
            {
                return this.packageLicenseFileField;
            }
            set
            {
                this.packageLicenseFileField = value;
            }
        }

        /// <remarks/>
        public string projectUrl
        {
            get
            {
                return this.projectUrlField;
            }
            set
            {
                this.projectUrlField = value;
            }
        }

        /// <remarks/>
        public string iconUrl
        {
            get
            {
                return this.iconUrlField;
            }
            set
            {
                this.iconUrlField = value;
            }
        }

        /// <remarks/>
        public allsettingsNugetSettingsRepository repository
        {
            get
            {
                return this.repositoryField;
            }
            set
            {
                this.repositoryField = value;
            }
        }

        /// <remarks/>
        public string tags
        {
            get
            {
                return this.tagsField;
            }
            set
            {
                this.tagsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class allsettingsNugetSettingsRepository
    {

        private string typeField;

        private string urlField;

        private string branchField;

        private string commitField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string branch
        {
            get
            {
                return this.branchField;
            }
            set
            {
                this.branchField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string commit
        {
            get
            {
                return this.commitField;
            }
            set
            {
                this.commitField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class allsettingsToolSettings
    {

        private string namespacePrefixField;

        private string excludeProjectsField;

        private string copyNuGetToField;

        private string logLevelField;

        private string addSymbolsField;

        private string nuGetCachePathField;

        private bool noAutoPackField;

        /// <remarks/>
        public string NamespacePrefix
        {
            get
            {
                return this.namespacePrefixField;
            }
            set
            {
                this.namespacePrefixField = value;
            }
        }

        /// <remarks/>
        public string ExcludeProjects
        {
            get
            {
                return this.excludeProjectsField;
            }
            set
            {
                this.excludeProjectsField = value;
            }
        }

        /// <remarks/>
        public string CopyNuGetTo
        {
            get
            {
                return this.copyNuGetToField;
            }
            set
            {
                this.copyNuGetToField = value;
            }
        }

        /// <remarks/>
        public string LogLevel
        {
            get
            {
                return this.logLevelField;
            }
            set
            {
                this.logLevelField = value;
            }
        }

        /// <remarks/>
        public string AddSymbols
        {
            get
            {
                return this.addSymbolsField;
            }
            set
            {
                this.addSymbolsField = value;
            }
        }

        /// <remarks/>
        public string NuGetCachePath
        {
            get
            {
                return this.nuGetCachePathField;
            }
            set
            {
                this.nuGetCachePathField = value;
            }
        }

        /// <remarks/>
        public bool NoAutoPack
        {
            get
            {
                return this.noAutoPackField;
            }
            set
            {
                this.noAutoPackField = value;
            }
        }
    }




}