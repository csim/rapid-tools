using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.Utilities;

namespace Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest
{
    public class FeatureManifest
    {

        public override bool Equals(object obj)
        {
            FeatureManifest fm = obj as FeatureManifest;
            bool b = true;
            b = b && fm.Id == this.Id;
            b = b && fm.Description == this.Description;
            b = b && fm.Scope == this.Scope;
            b = b && fm.Hidden == this.Hidden;
            return b;

        }
        bool? _activateOnDefault;

        /// <summary>
        /// Optional Boolean. TRUE if the Feature is activated by default during installation or when a Web application is created; FALSE if the Feature is not activated. This attribute equals TRUE by default. The ActivateOnDefault attribute does not apply to site collection (Site) or Web site (Web) scoped Features.
        /// </summary>
        public bool? ActivateOnDefault
        {
            get { return _activateOnDefault; }
            set { _activateOnDefault = value; }
        }
        bool? _alwaysForceInstall;

        //Optional Boolean. TRUE if the Feature is installed by force during installation even if the Feature is already installed.
        public bool? AlwaysForceInstall
        {
            get { return _alwaysForceInstall; }
            set { _alwaysForceInstall = value; }
        }
        bool? _autoActivateInCentralAdmin;

        /// <summary>
        /// Optional Boolean. TRUE if the Feature is activated by default in the Administrative Web site, site collection, or Web application. This attribute equals FALSE by default. The AutoActivateInCentralAdmin attribute does not apply to Farm-scoped Features.
        /// </summary>
        public bool? AutoActivateInCentralAdmin
        {
            get { return _autoActivateInCentralAdmin; }
            set { _autoActivateInCentralAdmin = value; }
        }
        string _creator;

        /// <summary>
        /// Optional Text.
        /// </summary>
        public string Creator
        {
            get { return _creator; }
            set { _creator = value; }
        }
        string _defaultResourceFile;

        /// <summary>
        /// Optional Text. Indicates a common resource file for retrieving Feature XML resources. If you specify a resource in the file, Windows SharePoint Services looks by default in \12\TEMPLATE\FEATURES\FeatureName\Resources\Resources.Culture.resx.
        /// </summary>
        public string DefaultResourceFile
        {
            get { return _defaultResourceFile; }
            set { _defaultResourceFile = value; }
        }
        string _description;

        /// <summary>
        /// Optional String. Returns a longer representation of what the Feature does.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        bool? _hidden;

        /// <summary>
        /// Optional Boolean. This attribute equals FALSE by default.
        /// </summary>
        public bool? Hidden
        {
            get { return _hidden; }
            set { _hidden = value; }
        }
        string _id;

        /// <summary>
        /// Required Text. Contains the globally unique identifier (GUID) for the Feature.
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        string _imageUrl;

        /// <summary>
        /// Optional Text. Contains the site-relative URL for an image to use to represent the feature in the user interface (UI).
        /// </summary>
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }
        string _imageUrlAltText;

        /// <summary>
        /// Optional Text. Contains the alternate text for the image that represents the feature. 
        /// </summary>
        public string ImageUrlAltText
        {
            get { return _imageUrlAltText; }
            set { _imageUrlAltText = value; }
        }
        string _receiverAssembly;

        /// <summary>
        /// Optional Text. If set along with ReceiverClass, specifies the strong name of the signed assembly located in the global assembly cache from which to load a receiver to handle Feature events.
        /// </summary>
        public string ReceiverAssembly
        {
            get { return _receiverAssembly; }
            set { _receiverAssembly = value; }
        }
        string _receiverClass;

        /// <summary>
        /// Optional Text. If set along with ReceiverAssembly, specifies the class that implements the Feature event processor.
        /// </summary>
        public string ReceiverClass
        {
            get { return _receiverClass; }
            set { _receiverClass = value; }
        }
        bool? _requireResources;

        /// <summary>
        /// Optional Boolean. TRUE to specify that Windows SharePoint Services check whether resources exist for the Feature by verifying that the standard "sentinel" resource for the Feature is present for a particular culture.
        /// </summary>
        public bool? RequireResources
        {
            get { return _requireResources; }
            set { _requireResources = value; }
        }
        string _scope;

        /// <summary>
        /// Required Text. Specifies the scope in which the Feature can be activated and contains one of the following values: Farm (farm), WebApplication (Web application), Site (site collection), Web (Web site). For information about scope, see Element Scope.
        /// </summary>
        public string Scope
        {
            get { return _scope; }
            set { _scope = value; }
        }
        string _solutionId;

        /// <summary>
        /// Optional Text. Specifies the solution to which the Feature belongs.
        /// </summary>
        public string SolutionId
        {
            get { return _solutionId; }
            set { _solutionId = value; }
        }
        string _title;

        /// <summary>
        /// Optional Text. Returns the title of the Feature. Limited to 255 characters.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        string _version;

        /// <summary>
        /// Optional Text. Specifies a System.Version-compliant representation of the version of a Feature. This can be up to four numbers delimited by decimals that represent a version.
        /// </summary>
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private List<string> _elements;

        /// <summary>
        /// A list containing the element manifest files.
        /// </summary>
        public List<string> ElementManifests
        {
            get { return _elements; }
            set { _elements = value; }
        }

        private List<string> _activationDependencies = new List<string>();

        /// <summary>
        /// A list containing the activation dependencies.
        /// </summary>
        public List<string> ActivationDependencies
        {
            get { return _activationDependencies; }
            set { _activationDependencies = value; }
        }

        public FeatureManifest() { Id = Guid.NewGuid().ToString(); }        
        
        public FeatureManifest(XmlDocument document)
        {
            SetProperties(document);
        }

        public FeatureManifest(string xmlString)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlString);
            SetProperties(document);            
        }

        public void SetProperties(XmlDocument document)
        {
            XmlNode node = document.DocumentElement;
            if (node.Attributes["ActivateOnDefault"] != null)
                ActivateOnDefault = Convert.ToBoolean(node.Attributes["ActivateOnDefault"].Value);
            if (node.Attributes["AlwaysForceInstall"] != null)
                AlwaysForceInstall = Convert.ToBoolean(node.Attributes["AlwaysForceInstall"].Value);
            if (node.Attributes["AutoActivateInCentralAdmin"] != null)
                AutoActivateInCentralAdmin = Convert.ToBoolean(node.Attributes["AutoActivateInCentralAdmin"].Value);
            if (node.Attributes["Creator"] != null)
                Creator = node.Attributes["Creator"].Value;
            if (node.Attributes["DefaultResourceFile"] != null)
                DefaultResourceFile = node.Attributes["DefaultResourceFile"].Value;
            if (node.Attributes["Description"] != null)
                Description = node.Attributes["Description"].Value;
            if (node.Attributes["Hidden"] != null)
                Hidden = Convert.ToBoolean(node.Attributes["Hidden"].Value);
            if (node.Attributes["Id"] != null)
                Id = node.Attributes["Id"].Value;
            if (node.Attributes["ImageUrl"] != null)
                ImageUrl = node.Attributes["ImageUrl"].Value;
            if (node.Attributes["ImageUrlAltText"] != null)
                ImageUrlAltText = node.Attributes["ImageUrlAltText"].Value;
            if (node.Attributes["ReceiverAssembly"] != null)
                ReceiverAssembly = node.Attributes["ReceiverAssembly"].Value;
            if (node.Attributes["ReceiverClass"] != null)
                ReceiverClass = node.Attributes["ReceiverClass"].Value;
            if (node.Attributes["RequireResources"] != null)
                RequireResources = Convert.ToBoolean(node.Attributes["RequireResources"].Value);
            if (node.Attributes["Scope"] != null)
                Scope = node.Attributes["Scope"].Value;
            if (node.Attributes["SolutionId"] != null)
                SolutionId = node.Attributes["SolutionId"].Value;
            if (node.Attributes["Title"] != null)
                Title = node.Attributes["Title"].Value;
            if (node.Attributes["Version"] != null)
                Version = node.Attributes["Version"].Value;

            XmlNamespaceManager nm = new XmlNamespaceManager(document.NameTable);
            nm.AddNamespace("n", "http://schemas.microsoft.com/sharepoint/");

            if (document.SelectNodes("/n:Feature/n:ActivationDependencies/n:ActivationDependency", nm).Count > 0)
            {
                foreach (XmlNode n in document.SelectNodes("/n:Feature/n:ActivationDependencies/n:ActivationDependency", nm))
                {
                    if (ActivationDependencies == null)
                        ActivationDependencies = new List<string>();
                    ActivationDependencies.Add(n.Attributes["FeatureId"].Value);
                }
            }

            if (document.SelectNodes("/n:Feature/n:ElementManifests/n:ElementManifest", nm).Count > 0)
            {
                foreach (XmlNode n in document.SelectNodes("/n:Feature/n:ElementManifests/n:ElementManifest", nm))
                {
                    if (ElementManifests == null)
                        ElementManifests = new List<string>();
                    ElementManifests.Add(n.Attributes["Location"].Value);
                }
            }
        }

        public void CreateManifest(string fileLocation)
        {
            using (StreamWriter manifestStream = new StreamWriter(fileLocation))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                settings.NewLineOnAttributes = false;
                settings.NewLineHandling = NewLineHandling.None;
                XmlWriter writer = XmlWriter.Create(manifestStream, settings);

                writer.WriteStartDocument();

                writer.WriteStartElement("Feature", "http://schemas.microsoft.com/sharepoint/");

                ManifestUtility.AddAttribute(ref writer, "ActivateOnDefault", ActivateOnDefault);
                ManifestUtility.AddAttribute(ref writer, "AlwaysForceInstall", AlwaysForceInstall);
                ManifestUtility.AddAttribute(ref writer, "AutoActivateInCentralAdmin", AutoActivateInCentralAdmin);
                ManifestUtility.AddAttribute(ref writer, "Creator", Creator);
                ManifestUtility.AddAttribute(ref writer, "DefaultResourceFile", DefaultResourceFile);
                ManifestUtility.AddAttribute(ref writer, "Description", Description);
                ManifestUtility.AddAttribute(ref writer, "Hidden", Hidden);
                ManifestUtility.AddAttribute(ref writer, "Id", Id);
                ManifestUtility.AddAttribute(ref writer, "ImageUrl", ImageUrl);
                ManifestUtility.AddAttribute(ref writer, "ImageUrlAltText", ImageUrlAltText);
                ManifestUtility.AddAttribute(ref writer, "ReceiverAssembly", ReceiverAssembly);
                ManifestUtility.AddAttribute(ref writer, "ReceiverClass", ReceiverClass);
                ManifestUtility.AddAttribute(ref writer, "RequireResources", RequireResources);
                ManifestUtility.AddAttribute(ref writer, "Scope", Scope);
                ManifestUtility.AddAttribute(ref writer, "SolutionId", SolutionId);
                ManifestUtility.AddAttribute(ref writer, "Title", Title);
                ManifestUtility.AddAttribute(ref writer, "Version", Version);

                if (ElementManifests != null && ElementManifests.Count > 0)
                {
                    writer.WriteStartElement("ElementManifests");
                    ElementManifests.ForEach(delegate(string s)
                    {
                        writer.WriteStartElement("ElementManifest");
                        writer.WriteAttributeString("Location", s);
                        writer.WriteEndElement();
                    });
                    writer.WriteEndElement();
                }

                if (ActivationDependencies != null && ActivationDependencies.Count > 0)
                {
                    writer.WriteStartElement("ActivationDependencies");
                    ActivationDependencies.ForEach(delegate(string s)
                    {
                        writer.WriteStartElement("ActivationDependency");
                        writer.WriteAttributeString("FeatureId", s);
                        writer.WriteEndElement();
                    });
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();

                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }
        }
    }
}
