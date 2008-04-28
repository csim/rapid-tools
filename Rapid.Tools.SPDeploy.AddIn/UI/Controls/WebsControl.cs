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
using Rapid.Tools.SPDeploy.AddIn.Domain;
using Rapid.Tools.SPDeploy.AddIn.Domain.Utilties;
using Rapid.Tools.SPDeploy.AddIn.UI.Forms;

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
            treeView1.ImageList.Images.Add("ViewIcon", Resources.Images.Files.ICXML);
            treeView1.ImageList.Images.Add("ViewsIcon", Resources.Images.Files.ICZIP);

        }

        Domain.Utilties.WatcherUtilitiy w;


        private Domain.Utilties.ApplicationUtility _applicationUtility;

        public Domain.Utilties.ApplicationUtility ApplicationUtility
        {
            get
            {
                if (_applicationUtility == null)
                    _applicationUtility = new Rapid.Tools.SPDeploy.AddIn.Domain.Utilties.ApplicationUtility(AppManager.Instance.ApplicationObject);
                return _applicationUtility;
            }
            set { _applicationUtility = value; }
        }

        public delegate void VoidDelegate();
        public void FillTreeView()
        {
           



            Domain.Menus.SolutionMenu sm = new Rapid.Tools.SPDeploy.AddIn.Domain.Menus.SolutionMenu(solutionToolStripMenuItem);
            sm.RefreshMenuItemsAsync();








            SiteStructureDocument = new XmlDocument();
            treeView1.Nodes.Add("Loading");
            treeView1.Nodes[0].SelectedImageKey = treeView1.Nodes[0].ImageKey = "LoadingIcon";
            treeView1.Enabled = false;
            ServiceManager.Instance.ServiceInstance.GetSiteStructureCompleted += new GetSiteStructureCompletedEventHandler(ServiceInstance_GetSiteStructureCompleted);
            ServiceManager.Instance.ServiceInstance.GetSiteStructureAsync(Domain.Utilties.Functions.GetSiteUrlFromProject(AppManager.Instance.ApplicationObject));

           

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

            WebNodeTag tag = NodeTagFactory.Create(currentNode, AppManager.Instance.ApplicationObject, NodeType.Web);
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

            WebNodeTag tag = NodeTagFactory.Create(_fileNode, AppManager.Instance.ApplicationObject, NodeType.File);
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

            WebNodeTag tag = NodeTagFactory.Create(currentNode, AppManager.Instance.ApplicationObject, NodeType.List);
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

            XmlNodeList _nodeList = xmlElement.SelectNodes("Views/View");
            currentNode = currentNode.Nodes.Add("Views");
            currentNode.SelectedImageKey = currentNode.ImageKey = "FolderIcon";
            currentNode.Tag = NodeTagFactory.Create(currentNode, AppManager.Instance.ApplicationObject, NodeType.Null);
            foreach (XmlNode no in _nodeList)
            {

                TreeNode tNode = currentNode.Nodes.Add(no.Attributes["Title"].Value);
                tNode.SelectedImageKey = tNode.ImageKey = "ViewIcon";
                WebNodeTag t = NodeTagFactory.Create(tNode, AppManager.Instance.ApplicationObject, NodeType.View);
                t.Guid = new Guid(no.Attributes["Guid"].Value);
                t.Url = no.Attributes["Url"].Value;
                tNode.Tag = t;

            }

            currentNode = currentNode.Parent;

            //  add the folders
            _nodeList = xmlElement.SelectNodes("Folders/Folder");
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
            currentNode.Tag = NodeTagFactory.Create(currentNode, AppManager.Instance.ApplicationObject, NodeType.Null);

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
                if (e.Node.Tag != null && e.Node.Tag is INodeTag)
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
            if (e.Node.Tag != null && e.Node.Tag is INodeTag)
            {
                INodeTag tag = e.Node.Tag as INodeTag;
                tag.Action();
            }
        }
    }


}
