using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.Utilities;

namespace Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems
{
	public class HideCustomAction : ElementManifest.IManifestNode
    {
        private string _groupId;

        /// <summary>
        /// Optional Text. Identifies an action group that contains the action, for example, "SiteAdministration".
        /// </summary>
        public string GroupId
        {
            get { return _groupId; }
            set { _groupId = value; }
        }

        private string _hideActionId;

        /// <summary>
        /// Optional Text. Specifies the ID of the custom action to hide, for example, "DeleteWeb". See Default Custom Action Locations and IDs for a list of the default custom action IDs that are used in Windows SharePoint Services.
        /// </summary>
        public string HideActionId
        {
            get { return _hideActionId; }
            set { _hideActionId = value; }
        }

        private string _id;

        /// <summary>
        /// Optional Text.Specifies the ID of this hide custom action element, for example, "HideDeleteWeb".
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _location;

        /// <summary>
        /// Optional Text. Specifies the location of the custom action to hide, for example, "Microsoft.SharePoint.SiteSettings". See Default Custom Action Locations and IDs for a list of the default custom action locations that are used in Windows SharePoint Services.
        /// </summary>
        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public HideCustomAction() { }
        public HideCustomAction(XmlNode node)
        {
            if (node.Attributes["GroupId"] != null)
                GroupId = node.Attributes["GroupId"].Value;
            if (node.Attributes["HideActionId"] != null)
                HideActionId = node.Attributes["HideActionId"].Value;
            if (node.Attributes["Id"] != null)
                Id = node.Attributes["Id"].Value;
            if (node.Attributes["Location"] != null)
                Location = node.Attributes["Location"].Value;
        }        

        #region IManifestNode Members

        public void CreateNode(ref System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("HideCustomAction");

            ManifestUtility.AddAttribute(ref writer, "GroupId", GroupId);
            ManifestUtility.AddAttribute(ref writer, "HideActionId", HideActionId);
            ManifestUtility.AddAttribute(ref writer, "Id", Id);
            ManifestUtility.AddAttribute(ref writer, "Location", Location);

            writer.WriteEndElement();
        }

        #endregion
    }
}
