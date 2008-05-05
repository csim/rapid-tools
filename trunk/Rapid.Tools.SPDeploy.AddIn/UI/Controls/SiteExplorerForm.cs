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
    public partial class SiteExplorerForm : UserControl
    {
		private DeploymentMenu _solutionMenu;
		private OpenMenu _openMenu;
		private DefaultColorTable _defaultColorTable;

		private XmlDocument _structure;
		private TreeNode _loadingNode;

		private NodeTag _activeNodeTag;


        public SiteExplorerForm()
        {
            InitializeComponent();

            _tree.ImageList = new ImageList();
            _tree.ImageList.Images.Add("GenericIcon", Resources.Images.Files.ICGEN);
            _tree.ImageList.Images.Add("BlankIcon", Resources.Images.Files.BLANK);
            _tree.ImageList.Images.Add("PublishingSiteIcon", Resources.Images.Files.CAT);
            _tree.ImageList.Images.Add("TeamSiteIcon", Resources.Images.Files.STSICON);
            _tree.ImageList.Images.Add("GenericListIcon", Resources.Images.Files.ITGEN);
            _tree.ImageList.Images.Add("DocumentLibraryIcon", Resources.Images.Files.ITDL);
            _tree.ImageList.Images.Add("FolderIcon", Resources.Images.Files.FOLDER);
            _tree.ImageList.Images.Add("LoadingIcon", Resources.Images.Files.actionssettings);
            _tree.ImageList.Images.Add("ViewIcon", Resources.Images.Files.ICXML);
            _tree.ImageList.Images.Add("ViewsIcon", Resources.Images.Files.ICZIP);

			_defaultColorTable = new DefaultColorTable();
			menuStrip1.Renderer = new ToolStripProfessionalRenderer(_defaultColorTable);
			
			deploymentMenu.Visible = false;
			openMenu.Visible = true;

			if (_openMenu == null)
				_openMenu = new OpenMenu(openMenu, OpenMenuClick);

		}


		private TreeNode AddSiteNode(TreeNode inode)
		{
			XmlElement xsite = (XmlElement)_structure.DocumentElement.SelectSingleNode("/Site");
			XmlElement xrootweb = (XmlElement)_structure.DocumentElement.SelectSingleNode("/Site/Web");

			inode.Text = string.Format("{0} ({1})", xrootweb.GetAttribute("Title"), xsite.GetAttribute("Url"));
			inode.EnsureVisible();

			SPSiteNodeTag tag = (SPSiteNodeTag)NodeTagFactory.Create(inode, NodeType.Site);
			tag.ServerRelativeUrl = xrootweb.Attributes["ServerRelativeUrl"].Value;

			tag.SiteID = new Guid(xsite.Attributes["ID"].Value);
			tag.ID = new Guid(xrootweb.Attributes["ID"].Value);

			tag.Url = xsite.Attributes["Url"].Value;
			inode.Tag = tag;

			//  set the node image
			if (!Convert.ToBoolean(xrootweb.Attributes["Publishing"].Value))
				inode.ImageKey = inode.SelectedImageKey = "TeamSiteIcon";
			else
				inode.ImageKey = inode.SelectedImageKey = "PublishingSiteIcon";

			AddChildNodes(inode, xrootweb);

			return inode;

		}

		
		private void AddChildWebNodes(TreeNode parentNode, XmlElement parentElement)
        {
			XmlNodeList nodes = parentElement.SelectNodes("Web");

			foreach (XmlElement ielement in nodes)
			{
				TreeNode inode = parentNode.Nodes.Add("");
				AddWebNode(inode, ielement);
			}

        }

        private void AddChildFileNodes(TreeNode parentNode, XmlElement parentElement)
        {
			XmlNodeList nodes = parentElement.SelectNodes("File");

			foreach (XmlElement ielement in nodes)
			{
				//  add the node
				TreeNode inode = parentNode.Nodes.Add(ielement.Attributes["Name"].Value);

				NodeTag tag = NodeTagFactory.Create(inode, NodeType.File);
				tag.ID = new Guid(ielement.Attributes["ID"].Value);
				tag.ServerRelativeUrl = ielement.Attributes["ServerRelativeUrl"].Value;
				inode.Tag = tag;

				//  set the icon for the file node
				Resources.ResourceUtility.SetFileNodeIcon(inode, Convert.ToBoolean(ielement.Attributes["CheckedOut"].Value));
			}
        }

        private void AddChildListNodes(TreeNode parentNode, XmlElement parentElement)
        {
			XmlNodeList nodes = parentElement.SelectNodes("List");

			foreach (XmlElement ielement in nodes)
			{

				//  do not show empty or hidden lists
				//if (xmlElement.Attributes["Title"].Value == "" || Convert.ToBoolean(xmlElement.Attributes["Hidden"].Value)) return;

				//  add the node
				TreeNode inode = parentNode.Nodes.Add(ielement.Attributes["Title"].Value);

				NodeTag tag = NodeTagFactory.Create(inode, NodeType.List);
				tag.ServerRelativeUrl = ielement.Attributes["ServerRelativeUrl"].Value;
				tag.ID = new Guid(ielement.Attributes["ID"].Value);
				inode.Tag = tag;

				inode.SelectedImageKey = inode.ImageKey = ielement.Attributes["Type"].Value == "DocumentLibrary" ? "DocumentLibraryIcon" : "GenericListIcon";

				AddChildNodes(inode, ielement);
			}

        }

		private void AddChildViewNodes(TreeNode parentNode, XmlElement parentElement)
		{
			XmlNodeList vnodes = parentElement.SelectNodes("View");

			if (vnodes.Count > 0) 
			{
				TreeNode vnode = parentNode.Nodes.Add("Views");
				vnode.SelectedImageKey = vnode.ImageKey = "FolderIcon";
				vnode.Tag = NodeTagFactory.Create(vnode, NodeType.Null);

				foreach (XmlElement ielement in vnodes)
				{
					TreeNode vinode = vnode.Nodes.Add(ielement.Attributes["Title"].Value);
					vinode.SelectedImageKey = vinode.ImageKey = "ViewIcon";
					
					NodeTag t = NodeTagFactory.Create(vinode, NodeType.View);
					t.ID = new Guid(ielement.Attributes["ID"].Value);
					t.ServerRelativeUrl = ielement.Attributes["ServerRelativeUrl"].Value;
					vinode.Tag = t;
				}
			}

		}
		
		private void AddChildFolderNodes(TreeNode parentNode, XmlElement parentElement)
        {
			XmlNodeList vnodes = parentElement.SelectNodes("Folder");

			foreach (XmlElement ielement in vnodes)
			{
				//  if the title is empty return
				if (ielement.Attributes["Title"].Value == "") return;

				//  add the node
				TreeNode inode = parentNode.Nodes.Add(ielement.Attributes["Title"].Value);
				inode.Tag = NodeTagFactory.Create(inode, NodeType.Folder);

				//  set the node image
				inode.ImageKey = inode.SelectedImageKey = "FolderIcon";

				AddChildNodes(inode, ielement);
			}

        }

		private void AddChildNodes(TreeNode parentNode, XmlElement parentElement)
		{
			AddChildListNodes(parentNode, parentElement);
			AddChildViewNodes(parentNode, parentElement);
			AddChildFolderNodes(parentNode, parentElement);
			AddChildWebNodes(parentNode, parentElement);
			AddChildFileNodes(parentNode, parentElement);
		}


		
		private void AddWebNode(TreeNode node, XmlElement element)
		{
			node.Text = element.Attributes["Title"].Value;

			NodeTag tag = NodeTagFactory.Create(node, NodeType.Web);
			tag.ServerRelativeUrl = element.Attributes["ServerRelativeUrl"].Value;
			tag.ID = new Guid(element.Attributes["ID"].Value);
			node.Tag = tag;

			//  set the node image
			if (!Convert.ToBoolean(element.Attributes["Publishing"].Value))
				node.ImageKey = node.SelectedImageKey = "TeamSiteIcon";
			else
				node.ImageKey = node.SelectedImageKey = "PublishingSiteIcon";

			AddChildNodes(node, element);
		}




        private void NodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
			try
			{
				NodeTag tag = null;


				if (e.Node.Tag != null && e.Node.Tag is NodeTag)
					tag = (NodeTag)e.Node.Tag;

				UpdateActiveTag(tag);

				if (tag == null) return;

				if (e.Button == MouseButtons.Right)
				{
					e.Node.ContextMenu = tag.RightClick();
				}
				else if (e.Button == MouseButtons.Left)
				{
					tag.Focus();
				}

				_tree.SelectedNode = e.Node;
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
		}

        private void NodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
			try
			{
				NodeTag tag = null;

				if (e.Node.Tag != null && e.Node.Tag is NodeTag)
					tag = (NodeTag)e.Node.Tag;

				if (tag == null) return;

				UpdateActiveTag(tag);
				tag.DoubleClick();
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
		}

		private void RefreshMenuClick(object sender, EventArgs e)
        {
			try
			{
				ToolStripMenuItem menu = (ToolStripMenuItem)sender;

				if ((string)menu.Tag == "Refresh")
				{
					RefreshCurrentWeb();
				}
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
        }

		private void OpenMenuClick(object sender, EventArgs e)
		{
			try
			{
				if ((string)((ToolStripMenuItem)sender).Tag == "Open")
				{
					OpenUrlForm oform = new OpenUrlForm();
					oform.ShowDialog();

					Uri url = new Uri(oform.Url);

					TreeNode rootNode = _tree.Nodes.Add("");

					StartLoading(rootNode);

					ProxyBridge bridge = new ProxyBridge(url.ToString());
					bridge.AddInService.GetSiteStructureCompleted += new GetSiteStructureCompletedEventHandler(OpenSiteCompleted);
					bridge.AddInService.GetSiteStructureAsync();
				}
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
		}


		private void RefreshCurrentSite()
		{
			if (_activeNodeTag == null) return;

			ProxyBridge bridge = new ProxyBridge(_activeNodeTag.SiteTag.Url);

			StartLoading(_activeNodeTag.SiteTag.Node);

			bridge.AddInService.GetSiteStructureCompleted += new GetSiteStructureCompletedEventHandler(RefreshSiteCompleted);
			bridge.AddInService.GetSiteStructureAsync();
		}

		private void RefreshCurrentWeb()
		{
			if (_activeNodeTag == null) return;

			if (_activeNodeTag.WebTag is SPSiteNodeTag)
			{
				RefreshCurrentSite();
				return;
			}

			ProxyBridge bridge = new ProxyBridge(_activeNodeTag.SiteTag.Url);

			StartLoading(_activeNodeTag.WebTag.Node);

			bridge.AddInService.GetWebStructureCompleted += new GetWebStructureCompletedEventHandler(RefreshWebCompleted);
			bridge.AddInService.GetWebStructureAsync(_activeNodeTag.WebTag.ID);
		}

		
		private void UpdateActiveTag(NodeTag tag)
		{
			_activeNodeTag = tag;

			refreshMenu.Visible = (tag != null);
			refreshMenu.Enabled = (tag.SiteTag != null);

			deploymentMenu.Visible = (tag != null);

		}


		private void OpenSiteCompleted(object sender, GetSiteStructureCompletedEventArgs e)
		{
			try
			{
				_structure = new XmlDocument();
				_structure.LoadXml(e.Result.OuterXml);

				TreeNode rootNode = AddSiteNode(_loadingNode);
				rootNode.Expand();
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
			finally
			{
				StopLoading();
			}
		}


		private void RefreshSiteCompleted(object sender, GetSiteStructureCompletedEventArgs e)
		{
			try
			{
				_structure = new XmlDocument();
				_structure.LoadXml(e.Result.OuterXml);

				TreeNode rootNode = AddSiteNode(_loadingNode);
				UpdateActiveTag((NodeTag)rootNode.Tag);
				rootNode.Expand();

			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
			finally
			{
				StopLoading();
			}
		}

		private void RefreshWebCompleted(object sender, GetWebStructureCompletedEventArgs e)
		{
			try
			{
				_structure = new XmlDocument();
				_structure.LoadXml(e.Result.OuterXml);

				AddWebNode(_loadingNode, _structure.DocumentElement);

				//AddChildWebNodes(parent, _siteStructureDocument.DocumentElement);
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
			finally
			{
				StopLoading();
			}
		}


		private void StartLoading(TreeNode targetNode)
		{
			_loadingNode = targetNode;
			_loadingNode.Text = "Loading ...";

			_loadingNode.Nodes.Clear();
			
			_loadingNode.SelectedImageKey = _loadingNode.ImageKey = "LoadingIcon";

			openMenu.Enabled = false;
			refreshMenu.Enabled = false;
			deploymentMenu.Visible = false;
		}

		private void StopLoading()
		{
			_loadingNode = null;

			openMenu.Enabled = true;
			refreshMenu.Enabled = true;
		}

    }
}
