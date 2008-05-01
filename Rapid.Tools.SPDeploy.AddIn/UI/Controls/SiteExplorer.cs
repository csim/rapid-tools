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
using Rapid.Tools.SPDeploy.AddIn.Proxies.AddIn;
using System.Security.Principal;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using Rapid.Tools.SPDeploy.AddIn.Proxies.Webs;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;
using Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags;
using Rapid.Tools.SPDeploy.AddIn.Domain;
using Rapid.Tools.SPDeploy.AddIn.UI.Forms;
using Rapid.Tools.SPDeploy.AddIn.UI.Wizard;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Controls
{
    public partial class SiteExplorer : UserControl
    {
        private XmlDocument _siteStructureDocument;

        public XmlDocument SiteStructureDocument
        {
            get { return _siteStructureDocument; }
            set { _siteStructureDocument = value; }
        }

        public SiteExplorer()
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

            AppManager.Current.ActiveBridge.AddInService.GetSiteStructureCompleted += new GetSiteStructureCompletedEventHandler(ServiceInstance_GetSiteStructureCompleted);

        }



        DefaultColorTable _defaultColorTable;
        bool _preloaded = false;
      

        private FileWatcher util;
        public delegate void VoidDelegate();
        DeploymentMenu solutionMenu;

        public void FillTreeView()
        {

            _defaultColorTable = new DefaultColorTable();
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(_defaultColorTable);

            LoadingForm _loadingForm = null;


            if (!_preloaded)
            {
                _loadingForm = new LoadingForm();
                _loadingForm.Show();
            }

            try
            {
                ProxyBridge pb = new ProxyBridge();
				pb.AddInService.GetSolutions();
            }
            catch (InvalidOperationException ex)
            {

				AppManager.Current.Write(ex);

                if (!_preloaded)
                {
                    _loadingForm.Close();
                }
                MessageBox.Show(string.Format("The Rapid Tools server components are not installed on {0}", AppManager.Current.ActiveEnvironment.WebApplicationUrl));
                return;
            }

            if (solutionMenu == null)
                solutionMenu = new DeploymentMenu(solutionToolStripMenuItem, toolStripMenuItem1_Click);
            
            solutionMenu.RefreshAsync();

            //util = FileWatcher.Instance;

            SiteStructureDocument = new XmlDocument();
            treeView1.Nodes.Add("Loading");
            treeView1.Nodes[0].SelectedImageKey = treeView1.Nodes[0].ImageKey = "LoadingIcon";
            treeView1.Enabled = false;

			AppManager.Current.ActiveBridge.AddInService.GetSiteStructureAsync(AppManager.Current.ActiveEnvironment.WebApplicationUrl);

            if (!_preloaded)
                _loadingForm.Close();

            _preloaded = true;
        }



        void ServiceInstance_GetSiteStructureCompleted(object sender, GetSiteStructureCompletedEventArgs e)
        {
			try
			{
				SiteStructureDocument.LoadXml(e.Result.OuterXml);
				treeView1.Nodes.Clear();

				AddSiteNode();
				
				treeView1.Nodes[0].Expand();
				treeView1.Enabled = true;
			}
			catch (Exception ex)
			{
				AppManager.Current.Write(ex);
			}
        }

		private void AddSiteNode()
		{
			XmlElement isite = (XmlElement)SiteStructureDocument.DocumentElement.SelectSingleNode("/Site");
			XmlElement irootweb = (XmlElement)SiteStructureDocument.DocumentElement.SelectSingleNode("/Site/Web");

			TreeNode inode = treeView1.Nodes.Add(string.Format("{0} ({1})", irootweb.GetAttribute("Title"), isite.GetAttribute("Url")));

			NodeTag tag = NodeTagFactory.Create(inode, AppManager.Current.Application, NodeType.Web);
			tag.ServerRelativeUrl = irootweb.Attributes["ServerRelativeUrl"].Value;
			tag.ID = new Guid(irootweb.Attributes["ID"].Value);
			inode.Tag = tag;

			AddChildNodes(inode, irootweb);

		}

		private void AddChildNodes(TreeNode parentNode, XmlElement parentElement)
		{
			AddListNodes(parentNode, parentElement);
			AddFolderNodes(parentNode, parentElement);
			AddWebNodes(parentNode, parentElement);
			AddFileNodes(parentNode, parentElement);
		}

		private void AddWebNodes(TreeNode parentNode, XmlElement parentElement)
        {
			XmlNodeList nodes = parentElement.SelectNodes("Web");

			foreach (XmlElement ielement in nodes)
			{
				TreeNode inode = parentNode.Nodes.Add(ielement.GetAttribute("Title"));

				NodeTag tag = NodeTagFactory.Create(inode, AppManager.Current.Application, NodeType.Web);
				tag.ServerRelativeUrl = ielement.Attributes["ServerRelativeUrl"].Value;
				tag.ID = new Guid(ielement.Attributes["ID"].Value);
				inode.Tag = tag;

				//  set the node image
				if (!Convert.ToBoolean(ielement.Attributes["Publishing"].Value))
					inode.ImageKey = inode.SelectedImageKey = "TeamSiteIcon";
				else
					inode.ImageKey = inode.SelectedImageKey = "PublishingSiteIcon";

				AddChildNodes(inode, ielement);
			}

        }

        private void AddFileNodes(TreeNode parentNode, XmlElement parentElement)
        {
			XmlNodeList nodes = parentElement.SelectNodes("File");

			foreach (XmlElement ielement in nodes)
			{
				//  add the node
				TreeNode inode = parentNode.Nodes.Add(ielement.Attributes["Name"].Value);

				NodeTag tag = NodeTagFactory.Create(inode, AppManager.Current.Application, NodeType.File);
				tag.ID = new Guid(ielement.Attributes["ID"].Value);
				tag.ServerRelativeUrl = ielement.Attributes["ServerRelativeUrl"].Value;
				inode.Tag = tag;

				//  set the icon for the file node
				Resources.ResourceUtility.SetFileNodeIcon(inode, Convert.ToBoolean(ielement.Attributes["CheckedOut"].Value));
			}
        }

        private void AddListNodes(TreeNode parentNode, XmlElement parentElement)
        {
			XmlNodeList nodes = parentElement.SelectNodes("List");

			foreach (XmlElement ielement in nodes)
			{

				//  do not show empty or hidden lists
				//if (xmlElement.Attributes["Title"].Value == "" || Convert.ToBoolean(xmlElement.Attributes["Hidden"].Value)) return;

				//  add the node
				TreeNode inode = parentNode.Nodes.Add(ielement.Attributes["Title"].Value);

				NodeTag tag = NodeTagFactory.Create(inode, AppManager.Current.Application, NodeType.List);
				tag.ServerRelativeUrl = ielement.Attributes["ServerRelativeUrl"].Value;
				tag.ID = new Guid(ielement.Attributes["ID"].Value);
				inode.Tag = tag;

				inode.SelectedImageKey = inode.ImageKey = ielement.Attributes["Type"].Value == "DocumentLibrary" ? "DocumentLibraryIcon" : "GenericListIcon";

				AddChildNodes(inode, ielement);
			}

        }


		private void AddViewNodes(TreeNode parentNode, XmlElement parentElement)
		{
			XmlNodeList vnodes = parentElement.SelectNodes("View");

			if (vnodes.Count > 0) 
			{
				TreeNode vnode = parentNode.Nodes.Add("Views");
				vnode.SelectedImageKey = vnode.ImageKey = "FolderIcon";
				vnode.Tag = NodeTagFactory.Create(vnode, AppManager.Current.Application, NodeType.Null);

				foreach (XmlElement ielement in vnodes)
				{
					TreeNode vinode = vnode.Nodes.Add(ielement.Attributes["Title"].Value);
					vinode.SelectedImageKey = vinode.ImageKey = "ViewIcon";
					
					NodeTag t = NodeTagFactory.Create(vinode, AppManager.Current.Application, NodeType.View);
					t.ID = new Guid(ielement.Attributes["ID"].Value);
					t.ServerRelativeUrl = ielement.Attributes["ServerRelativeUrl"].Value;
					vinode.Tag = t;
				}
			}

		}
		private void AddFolderNodes(TreeNode parentNode, XmlElement parentElement)
        {
			XmlNodeList vnodes = parentElement.SelectNodes("Folder");

			foreach (XmlElement ielement in vnodes)
			{
				//  if the title is empty return
				if (ielement.Attributes["Title"].Value == "") return;

				//  add the node
				TreeNode inode = parentNode.Nodes.Add(ielement.Attributes["Title"].Value);
				inode.Tag = NodeTagFactory.Create(inode, AppManager.Current.Application, NodeType.Folder);

				//  set the node image
				inode.ImageKey = inode.SelectedImageKey = "FolderIcon";

				AddChildNodes(inode, ielement);
			}

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

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null && e.Node.Tag is INodeTag)
            {
                INodeTag tag = e.Node.Tag as INodeTag;
                tag.DoubleClick();
            }
        }     
       
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
			if ((string)((ToolStripMenuItem)sender).Tag == "ServerButton")
            {
				CreationForm cf = new CreationForm(AppManager.Current.ActiveEnvironment.ServerName, AppManager.Current.ActiveEnvironment.ServerPort);
                cf.ShowDialog();
            }
            treeView1.Nodes.Clear();
            FillTreeView();
        }


    }


}
