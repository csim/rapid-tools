using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.Utilities;

namespace Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems
{
    public class CustomActionGroup : ElementManifest.IManifestNode
    {
        private string _description;

        /// <summary>
        /// Optional Text. Specifies a longer description that is exposed as a sub-description for the action group.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private string _id;

        /// <summary>
        /// Optional Text. Specifies a unique identifier for the element. The ID may be a GUID, or it may be a unique term,
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _location;

        /// <summary>
        /// Required Text. Specifies a value for where the action lives. This string is a name that is declared on the LinkSectionTable control within a page. See Default Custom Action Locations and IDs for a list of the default custom action group locations that are used in Windows SharePoint Services.
        /// </summary>
        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        private int? _sequence;

        /// <summary>
        /// Optional Integer. Specifies the ordering priority for the action group.
        /// </summary>
        public int? Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }

        private string _title;

        /// <summary>
        /// Required Text. Specifies the end user description for the action group.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public CustomActionGroup() { }
        public CustomActionGroup(XmlNode node)
        {
            if (node.Attributes["Description"] != null)
                Description = node.Attributes["Description"].Value;
            if (node.Attributes["Id"] != null)
                Id = node.Attributes["Id"].Value;
            if (node.Attributes["Location"] != null)
                Location = node.Attributes["Location"].Value;
            if (node.Attributes["Sequence"] != null)
                Sequence = Convert.ToInt32(node.Attributes["Sequence"].Value);
            if (node.Attributes["Title"] != null)
                Title = node.Attributes["Title"].Value;
        }


        #region IManifestNode Members

        public void CreateNode(ref System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("CustomActionGroup");

            ManifestUtility.AddAttribute(ref writer, "Description", Description);
            ManifestUtility.AddAttribute(ref writer, "Id", Id);
            ManifestUtility.AddAttribute(ref writer, "Location", Location);
            ManifestUtility.AddAttribute(ref writer, "Sequence", Sequence);
            ManifestUtility.AddAttribute(ref writer, "Title", Title);

            writer.WriteEndElement();
        }

        #endregion
    }
}
