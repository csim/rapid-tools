using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;

namespace Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems
{
    public class CustomAction : ElementManifest.IManifestNode
    {
        private string _contentTypeId;

        /// <summary>
        /// Optional Text. Specifies the ID of a content type to associate with the custom action.
        /// </summary>
        public string ContentTypeId
        {
            get { return _contentTypeId; }
            set { _contentTypeId = value; }
        }
        private string _controlAssembly;

        /// <summary>
        /// Optional Text. Specifies the assembly of a control that supports the custom action.
        /// </summary>
        public string ControlAssembly
        {
            get { return _controlAssembly; }
            set { _controlAssembly = value; }
        }
        private string _controlClass;

        /// <summary>
        /// Optional Text. Specifies a control class that supports the custom action.
        /// </summary>
        public string ControlClass
        {
            get { return _controlClass; }
            set { _controlClass = value; }
        }
        private string _controlSrc;

        /// <summary>
        /// Optional Text.  
        /// </summary>
        public string ControlSrc
        {
            get { return _controlSrc; }
            set { _controlSrc = value; }
        }
        private string _description;

        /// <summary>
        /// Optional Text. Specifies a longer description for the action that is exposed as a tooltip or sub-description for the action.
        /// </summary>
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        private string _groupId;

        /// <summary>
        /// Optional Text. Identifies an action group that contains the action, for example, "SiteManagement". If contained within a custom action group, the value of the GroupId attribute must equal the group ID of the CustomActionGroup element.
        /// </summary>
        public string GroupId
        {
            get { return _groupId; }
            set { _groupId = value; }
        }
        private string _id;

        /// <summary>
        /// Optional Text. Specifies a unique identifier for the custom action. The ID may be a GUID, or it may be a unique term, for example, "HtmlViewer". See Default Custom Action Locations and IDs for a list of the default custom action IDs that are used in Windows SharePoint Services.
        /// </summary>
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private string _imageUrl;

        /// <summary>
        /// Optional Text. Specifies a virtual server relative link to an image that presents an icon for the item.
        /// </summary>
        public string ImageUrl
        {
            get { return _imageUrl; }
            set { _imageUrl = value; }
        }
        private string _location;

        /// <summary>
        /// Optional Text. Specifies the location of this custom action, for example, "Microsoft.SharePoint.SiteSettings". If the custom action is a menu item or toolbar button, then the possible options include EditControlBlock, NewFormToolbar, DisplayFormToolbar, and EditFormToolbar.
        /// </summary>
        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }
        private string _registrationId;

        /// <summary>
        /// Optional Text. Specifies the identifier of the list or item content type that this action is associated with, or the file type or programmatic identifier (ProgID).
        /// </summary>
        public string RegistrationId
        {
            get { return _registrationId; }
            set { _registrationId = value; }
        }
        private string _registrationType;

        /// <summary>
        /// Optional Text. Specifies the registration attachment for a per-item action. Possible values include: ContentType, FileType, List, ProgId 
        /// </summary>
        public string RegistrationType
        {
            get { return _registrationType; }
            set { _registrationType = value; }
        }
        private bool? _requireSiteAdministrator;

        /// <summary>
        /// Optional Boolean. TRUE to specify that the item be displayed only if the user is a site administrator; otherwise, FALSE. Using the RequireSiteAdministrator attribute for the drop-down menu of Windows SharePoint Services commands associated with list items is not supported.
        /// </summary>
        public bool? RequireSiteAdministrator
        {
            get { return _requireSiteAdministrator; }
            set { _requireSiteAdministrator = value; }
        }
        private string _rights;

        /// <summary>
        /// Optional Text. Specifies a set of rights that the user must have in order for the link to be visible, for example, "ViewListItems,ManageAlerts". If not specified, then the action always appears in the list of actions. To specify multiple rights, separate the values by using commas. The set of rights are grouped logically according to AND logic, which means that a user must have all the specified rights to see an action. For a list of possible values, see Microsoft.SharePoint.SPBasePermissions.
        /// </summary>
        public string Rights
        {
            get { return _rights; }
            set { _rights = value; }
        }
        private int? _sequence;

        /// <summary>
        /// Optional Integer. Specifies the ordering priority for actions.
        /// </summary>
        public int? Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }
        private bool? _showInLists;

        /// <summary>
        /// Optional Boolean. Show the Action in Lists
        /// </summary>
        public bool? ShowInLists
        {
            get { return _showInLists; }
            set { _showInLists = value; }
        }
        private bool? _showInReadOnlyContentTypes;

        /// <summary>
        /// Optional Boolean. TRUE if the custom action is only displayed for read-only content types on the page for managing content types. The default value is FALSE.
        /// </summary>
        public bool? ShowInReadOnlyContentTypes
        {
            get { return _showInReadOnlyContentTypes; }
            set { _showInReadOnlyContentTypes = value; }
        }
        private bool? _showInSealedContentTypes;

        /// <summary>
        /// Optional Boolean. TRUE if the custom action is only displayed for sealed content types on the page for managing content types. The default value is FALSE.
        /// </summary>
        public bool? ShowInSealedContentTypes
        {
            get { return _showInSealedContentTypes; }
            set { _showInSealedContentTypes = value; }
        }
        private string _title;

        /// <summary>
        /// Required Text. Specifies the end user description for this action.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private string _urlAction;

        public string UrlAction
        {
            get { return _urlAction; }
            set { _urlAction = value; }
        }

       


        public CustomAction() { }

        public CustomAction(XmlNode node)
        {
            if (node.Attributes["ContentTypeID"] != null)            
                ContentTypeId = node.Attributes["ContentTypeID"].Value;
            if (node.Attributes["ControlAssembly"] != null)
                ControlAssembly = node.Attributes["ControlAssembly"].Value;
            if (node.Attributes["ControlClass"] != null)
                ControlClass = node.Attributes["ControlClass"].Value;
            if (node.Attributes["Description"] != null)
                Description = node.Attributes["Description"].Value;
            if (node.Attributes["ControlSrc"] != null)
                ControlSrc = node.Attributes["ControlSrc"].Value;
            if (node.Attributes["GroupId"] != null)
                GroupId = node.Attributes["GroupId"].Value;
            if (node.Attributes["Id"] != null)
                Id = node.Attributes["Id"].Value;
            if (node.Attributes["ImageUrl"] != null)
                ImageUrl = node.Attributes["ImageUrl"].Value;
            if (node.Attributes["Location"] != null)
                Location = node.Attributes["Location"].Value;
            if (node.Attributes["RegistrationId"] != null)
                RegistrationId = node.Attributes["RegistrationId"].Value;
            if (node.Attributes["RequireSiteAdministrator"] != null)
                RequireSiteAdministrator = Convert.ToBoolean(node.Attributes["RequireSiteAdministrator"].Value);
            if (node.Attributes["Rights"] != null)
                Rights = node.Attributes["Rights"].Value;
            if (node.Attributes["Sequence"] != null)
                Sequence = Convert.ToInt32(node.Attributes["Sequence"].Value);
            if (node.Attributes["ShowInLists"] != null)
                ShowInLists = Convert.ToBoolean(node.Attributes["ShowInLists"].Value);
            if (node.Attributes["ShowInReadOnlyContentTypes"] != null)
                ShowInReadOnlyContentTypes = Convert.ToBoolean(node.Attributes["ShowInReadOnlyContentTypes"].Value);
            if (node.Attributes["Title"] != null)
                Title = node.Attributes["Title"].Value;
            foreach (XmlNode cNode in node.ChildNodes)
            {
                switch (cNode.Name)
                {
                    case "UrlAction":
                        UrlAction = cNode.Attributes["Url"].Value;
                        break;
                }
            }
        }

        #region IManifestNode Members

        /// <summary>
        /// Writes the properties to the specified writer.
        /// </summary>
        /// <param name="writer">Writer to use</param>
        public void CreateNode(ref XmlWriter writer)
        {
            writer.WriteStartElement("CustomAction");

            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "ContentTypeId", ContentTypeId);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "ControlAssembly", ControlAssembly);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "ControlClass", ControlClass);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "Description", Description);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "ControlSrc", ControlSrc);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "GroupId", GroupId);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "Id", Id);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "ImageUrl", ImageUrl);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "Location", Location);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "RegistrationId", RegistrationId);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "RegistrationType", RegistrationType);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "RequireSiteAdministrator", RequireSiteAdministrator);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "Rights", Rights);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "Sequence", Sequence);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "ShowInLists", ShowInLists);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "ShowInReadOnlyContentTypes", ShowInReadOnlyContentTypes);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "ShowInSealedContentTypes", ShowInSealedContentTypes);
            ProjectFiles.Utilities.ManifestUtility.AddAttribute(ref writer, "Title", Title);

           
            writer.WriteStartElement("UrlAction");
            writer.WriteAttributeString("Url", UrlAction);
            writer.WriteEndElement();
            
            writer.WriteEndElement();
        }

        #endregion      
         
    }
}
