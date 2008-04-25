using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Collections;
using EnvDTE80;
using System.IO;
using System.Net;
using EnvDTE;
using Rapid.Tools.SPDeploy.AddIn.SPToolsWebService;
using System.Security.Principal;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using Rapid.Tools.SPDeploy.AddIn.Proxies.Webs;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;
using Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Controls
{
    public partial class WebsControl : UserControl
    {
        private TreeNode currentNode;

        private XmlDocument _siteStructureDocument;

        public XmlDocument SiteStructureDocument
        {
            get { return _siteStructureDocument; }
            set { _siteStructureDocument = value; }
        }

        private DTE2 _applicationObject;

        public DTE2 ApplicationObject
        {
            get { return _applicationObject; }
            set { _applicationObject = value; }
        }

        private SPToolsWebService.SPToolsWebService _serviceInstance;

        public SPToolsWebService.SPToolsWebService ServiceInstance
        {
            get
            {
                if (_serviceInstance == null)
                {
                    _serviceInstance = new Rapid.Tools.SPDeploy.AddIn.SPToolsWebService.SPToolsWebService();
                    _serviceInstance.Url = string.Concat(Domain.Utilties.Functions.GetSiteUrlFromProject(ApplicationObject), "/_layouts/SPTools/SPToolsWebService.asmx");
                    _serviceInstance.Credentials = CredentialCache.DefaultCredentials;
                }
                return _serviceInstance;
            }
        }

        public WebsControl()
        {
            InitializeComponent();

            treeView1.ImageList = new ImageList();
            treeView1.ImageList.Images.Add("GenericIcon", Resources.Images.Files.ICGEN);
            treeView1.ImageList.Images.Add("BlankIcon", Resources.Images.Files.BLANK);
            treeView1.ImageList.Images.Add("PublishingSiteIcon", Resources.Images.Files.CAT);
            treeView1.ImageList.Images.Add("TeamSiteIcon", Resources.Images.Files.STSICON);
            treeView1.ImageList.Images.Add("GenericListIcon", Resources.Images.Files.ITGEN);
            treeView1.ImageList.Images.Add("DocumentLibraryIcon", Resources.Images.Files.ITDL);
            treeView1.ImageList.Images.Add("FolderIcon", Resources.Images.Files.FOLDER);
            treeView1.ImageList.Images.Add("LoadingIcon", Resources.Images.Files.actionssettings);

        }




       

        public delegate void VoidDelegate();

        public void InvokeTreeView(VoidDelegate method)
        {
            if (treeView1.InvokeRequired)
                treeView1.Invoke(method);
            else
                method.BeginInvoke(null, null);
        }

        public void FillTreeView()
        {
            SiteStructureDocument = new XmlDocument();
            treeView1.Nodes.Add("Loading");
            treeView1.Nodes[0].SelectedImageKey = treeView1.Nodes[0].ImageKey = "LoadingIcon";
            treeView1.Enabled = false;
            ServiceInstance.GetSiteStructureCompleted += new GetSiteStructureCompletedEventHandler(ServiceInstance_GetSiteStructureCompleted);
            ServiceInstance.GetSiteStructureAsync(Domain.Utilties.Functions.GetSiteUrlFromProject(ApplicationObject));
        }

        void ServiceInstance_GetSiteStructureCompleted(object sender, GetSiteStructureCompletedEventArgs e)
        {
            SiteStructureDocument.LoadXml(e.Result.OuterXml);
            treeView1.Nodes.Clear();
            AddWebNodes(SiteStructureDocument.DocumentElement.SelectSingleNode("/Site/Webs/Web") as XmlElement);
            treeView1.Nodes[0].Expand();
            treeView1.Enabled = true;
        }

        private void AddWebNodes(XmlElement xmlElement)
        {
            if (currentNode == null)
                currentNode = treeView1.Nodes.Add(xmlElement.Attributes[0].Value);
            else
                currentNode = currentNode.Nodes.Add(xmlElement.Attributes[0].Value);

            WebNodeTag tag = NodeTagFactory.Create(currentNode, ApplicationObject, NodeType.Web);
            tag.Url = xmlElement.Attributes["Url"].Value;
            tag.Guid = new Guid(xmlElement.Attributes["Guid"].Value);
            currentNode.Tag = tag;


            //  set the node image
            if (!Convert.ToBoolean(xmlElement.Attributes["Publishing"].Value))
                currentNode.ImageKey = currentNode.SelectedImageKey = "TeamSiteIcon";
            else
                currentNode.ImageKey = currentNode.SelectedImageKey = "PublishingSiteIcon";


            //  add the web nodes
            XmlNodeList _nodeList = xmlElement.SelectNodes("Webs/Web");
            if (_nodeList.Count > 0)
            {
                foreach (XmlNode no in _nodeList)
                {
                    AddWebNodes(no as XmlElement);
                }
            }

            //  add the list nodes
            _nodeList = xmlElement.SelectNodes("Lists/List");
            if (_nodeList.Count > 0)
            {
                foreach (XmlNode no in _nodeList)
                {
                    AddListNodes(no as XmlElement);
                }
            }

            //  add the file nodes
            _nodeList = xmlElement.SelectNodes("Files/File");
            if (_nodeList.Count > 0)
            {
                foreach (XmlNode no in _nodeList)
                    AddFileNodes(no as XmlElement);
            }

            //  move up to the parent node if it exists to continue
            if (currentNode.Parent != null)
                currentNode = currentNode.Parent;

        }

        private void AddFileNodes(XmlElement xmlElement)
        {
            //  add the node
            TreeNode _fileNode = currentNode.Nodes.Add(xmlElement.Attributes["Name"].Value);

            WebNodeTag tag = NodeTagFactory.Create(_fileNode, ApplicationObject, NodeType.File);
            tag.Url = xmlElement.Attributes["Url"].Value;
            tag.Guid = new Guid(xmlElement.Attributes["Guid"].Value);
            _fileNode.Tag = tag;

            //  set the icon for the file node
            Resources.ResourceUtility.SetFileNodeIcon(_fileNode, Convert.ToBoolean(xmlElement.Attributes["CheckedOut"].Value));
        }

        private void AddListNodes(XmlElement xmlElement)
        {
            //  do not show empty or hidden lists
            if (xmlElement.Attributes["Title"].Value == "" || Convert.ToBoolean(xmlElement.Attributes["Hidden"].Value)) return;

            //  add the node
            currentNode = currentNode.Nodes.Add(xmlElement.Attributes["Title"].Value);

            WebNodeTag tag = NodeTagFactory.Create(currentNode, ApplicationObject, NodeType.List);
            tag.Url = xmlElement.Attributes["Url"].Value;
            tag.Guid = new Guid(xmlElement.Attributes["Guid"].Value);
            currentNode.Tag = tag;

            //  set the icon to generic unless it is a document library            
            if (xmlElement.Attributes["Type"].Value != "DocumentLibrary")
            {
                currentNode.SelectedImageKey = currentNode.ImageKey = "GenericListIcon";
            }
            else
            {
                currentNode.SelectedImageKey = currentNode.ImageKey = "DocumentLibraryIcon";                
            }

            //  add the folders
            XmlNodeList _nodeList = xmlElement.SelectNodes("Folders/Folder");
            foreach (XmlNode no in _nodeList)
                AddFolderNodes(no as XmlElement);

            //  add the files
            _nodeList = xmlElement.SelectNodes("Files/File");
            foreach (XmlNode no in _nodeList)
                AddFileNodes(no as XmlElement);           

            //  move to the parent
            if (currentNode.Parent != null)
                currentNode = currentNode.Parent;
        }
       

        private void AddFolderNodes(XmlElement xmlElement)
        {
            //  if the title is empty return
            if (xmlElement.Attributes["Title"].Value == "") return;

            //  add the node
            currentNode = currentNode.Nodes.Add(xmlElement.Attributes["Title"].Value);

            //  set the node image
            currentNode.ImageKey = currentNode.SelectedImageKey = "FolderIcon";

            //  add the folders
            XmlNodeList _nodeList = xmlElement.SelectNodes("Folders/Folder");
            foreach (XmlNode no in _nodeList)
                AddFolderNodes(no as XmlElement);

            //  add the files
            _nodeList = xmlElement.SelectNodes("Files/File");
            foreach (XmlNode no in _nodeList)
                AddFileNodes(no as XmlElement);

            //  move to the parent node
            if (currentNode.Parent != null)
                currentNode = currentNode.Parent;
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                e.Node.ContextMenu = ((INodeTag)e.Node.Tag).GetContextMenu();               
            }
            treeView1.SelectedNode = e.Node;
        }

        private void showListItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
         
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            currentNode = null;
            FillTreeView();
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            INodeTag tag = e.Node.Tag as INodeTag;
            tag.Action();
        }
    }

    
}
