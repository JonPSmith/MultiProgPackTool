// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace MultiProjPackTool.NuspecBuilder
{




    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd", IsNullable = false)]
    public partial class package
    {

        private packageMetadata metadataField;

        private packageFile[] filesField;

        /// <remarks/>
        public packageMetadata metadata
        {
            get
            {
                return this.metadataField;
            }
            set
            {
                this.metadataField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("file", IsNullable = false)]
        public packageFile[] files
        {
            get
            {
                return this.filesField;
            }
            set
            {
                this.filesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")]
    public partial class packageMetadata
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

        private packageMetadataRepository repositoryField;

        private string tagsField;

        private packageMetadataGroup[] dependenciesField;

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
        public packageMetadataRepository repository
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

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("group", IsNullable = false)]
        public packageMetadataGroup[] dependencies
        {
            get
            {
                return this.dependenciesField;
            }
            set
            {
                this.dependenciesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")]
    public partial class packageMetadataRepository
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")]
    public partial class packageMetadataGroup
    {

        private packageMetadataGroupDependency[] dependencyField;

        private string targetFrameworkField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("dependency")]
        public packageMetadataGroupDependency[] dependency
        {
            get
            {
                return this.dependencyField;
            }
            set
            {
                this.dependencyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string targetFramework
        {
            get
            {
                return this.targetFrameworkField;
            }
            set
            {
                this.targetFrameworkField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")]
    public partial class packageMetadataGroupDependency
    {

        private string idField;

        private string versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
        [System.Xml.Serialization.XmlAttributeAttribute()]
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
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd")]
    public partial class packageFile
    {

        private string srcField;

        private string targetField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string src
        {
            get
            {
                return this.srcField;
            }
            set
            {
                this.srcField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string target
        {
            get
            {
                return this.targetField;
            }
            set
            {
                this.targetField = value;
            }
        }
    }





}