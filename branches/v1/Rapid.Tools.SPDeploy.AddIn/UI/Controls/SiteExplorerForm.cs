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


        ~SiteExplorerForm()
        {
            Connect.ExplorerForm = null;
        }

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

            refreshMenu.Visible = true;

            if (AppManager.Current.ActiveProject == null)
            {
                openMenu.Visible = true;
                deploymentMenu.Visible = false;

                openMenu.Click += new EventHandler(OpenMenuClick);

                //if (_openMenu == null)
                  //  _openMenu = new OpenMenu(openMenu, OpenMenuClick);
            }
            else
            {
                deploymentMenu.Visible = true;
                openMenu.Visible = false;

                try
                {
                    string url = SPEnvironmentInfo.Parse(AppManager.Current.ActiveProject).BaseUrl;
                    
                    _solutionMenu = new DeploymentMenu(deploymentMenu, url, RefreshSiteEvent);
                    _solutionMenu.RefreshAsync();
                    
                    LoadSiteFromUrl(new Uri(url));                   

                    //openMenu.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    //openMenu.ImageScaling = ToolStripItemImageScaling.None;                    
                    
                    //openMenu.Visible = true;

                    //openMenu.Enabled = true;
                    //FeatureMenu.Instance.Initialize(openMenu);
                }
                catch (Exception ex)
                {
                    ExceptionUtil.Handle(ex);
                }
            }

        }
        
        

        public void RefreshSiteEvent(object sender, EventArgs e)
        {
            SPEnvironmentInfo info = SPEnvironmentInfo.Parse(AppManager.Current.ActiveProject);
            OpenUrlForm o = new OpenUrlForm();
            o.Url = info.BaseUrl;
            o.ShowDialog();
            AppManager.Current.SetMachineInfo(o.Url);
            AppManager.Current.ResetActiveProject();
            RefreshSite();
        }

        private delegate void VoidDelegate();
        private void ExecAsync(VoidDelegate method)
        {
            method.BeginInvoke(null, null);
        }


        private TreeNode AddSiteNode(TreeNode inode)
        {
            preloadComplete = false;

            XmlElement xsite = (XmlElement)_structure.DocumentElement.SelectSingleNode("/Site");
            XmlElement xrootweb = (XmlElement)_structure.DocumentElement.SelectSingleNode("/Site/Web");

            inode.Text = string.Format("{0} ({1})", xrootweb.GetAttribute("Title"), xsite.GetAttribute("Url"));
            inode.EnsureVisible();

            SPSiteNodeTag tag = (SPSiteNodeTag)NodeTagFactory.Create(inode, NodeType.Site);
            tag.ServerRelativeUrl = xrootweb.Attributes["ServerRelativeUrl"].Value;

            foreach (XmlNode fn in xsite.SelectNodes("Feature"))
            {
                tag.ActivatedFeatures.Add(new Guid(fn.InnerText));
            }

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

            preloadComplete = true;

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
            AddChildWebNodes(parentNode, parentElement);
            AddChildFolderNodes(parentNode, parentElement);
            AddChildListNodes(parentNode, parentElement);
            AddChildViewNodes(parentNode, parentElement);
            AddChildFileNodes(parentNode, parentElement);
        }

        bool preloadComplete = false;

        private void AddWebNode(TreeNode node, XmlElement element)
        {
            if (_tree.InvokeRequired)
                _tree.Invoke(new VoidDelegate(delegate()
                {
                    node.Text = element.Attributes["Title"].Value;

                    NodeTag tag = NodeTagFactory.Create(node, NodeType.Web);
                    tag.ServerRelativeUrl = element.Attributes["ServerRelativeUrl"].Value;
                    tag.ID = new Guid(element.Attributes["ID"].Value);
                    node.Tag = tag;
                    foreach (XmlNode fn in element.SelectNodes("Feature"))
                    {
                        SPWebNodeTag t = tag as SPWebNodeTag;
                        t.ActivatedFeatures.Add(new Guid(fn.InnerText));
                    }

                    //  set the node image
                    if (!Convert.ToBoolean(element.Attributes["Publishing"].Value))
                        node.ImageKey = node.SelectedImageKey = "TeamSiteIcon";
                    else
                        node.ImageKey = node.SelectedImageKey = "PublishingSiteIcon";

                    if (preloadComplete || ((NodeTag)node.Tag).ID == _activeNodeTag.ID)
                    {
                        AddChildNodes(node, element);
                    }
                    else
                    {
                        node.Nodes.Add("Loading");
                    }
                }));
            else
            {
                node.Text = element.Attributes["Title"].Value;

                NodeTag tag = NodeTagFactory.Create(node, NodeType.Web);
                tag.ServerRelativeUrl = element.Attributes["ServerRelativeUrl"].Value;
                tag.ID = new Guid(element.Attributes["ID"].Value);
                node.Tag = tag;
                foreach (XmlNode fn in element.SelectNodes("Feature"))
                {
                    SPWebNodeTag t = tag as SPWebNodeTag;
                    t.ActivatedFeatures.Add(new Guid(fn.InnerText));
                }

                //  set the node image
                if (!Convert.ToBoolean(element.Attributes["Publishing"].Value))
                    node.ImageKey = node.SelectedImageKey = "TeamSiteIcon";
                else
                    node.ImageKey = node.SelectedImageKey = "PublishingSiteIcon";

                if (preloadComplete || (_activeNodeTag!=null && ((NodeTag)node.Tag).ID == _activeNodeTag.ID))
                {
                    AddChildNodes(node, element);
                }
                else
                {
                    node.Nodes.Add("Loading");
                }
            }
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

        string previousSite = string.Empty;

        public void RefreshMenu()
        {
            if (_solutionMenu != null)
                _solutionMenu.RefreshAsync();
//            FeatureMenu.Instance.Refresh();
        }

        private void RefreshMenuClick(object sender, EventArgs e)
        {
            try
            {
                AppManager.Current.ResetActiveProject();
                RefreshMenu();
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
               // if ((string)((ToolStripMenuItem)sender).Tag == "Open")
               // {
                    OpenUrlForm oform = new OpenUrlForm();
                    oform.ShowDialog();

                    Uri url = new Uri(oform.Url);

                    LoadSiteFromUrl(url);

                    AppManager.Current.CurrentUrls.Add(oform.Url);
                //}
                //else if ((string)((ToolStripMenuItem)sender).Tag == "Url")
                //{
                //    LoadSiteFromUrl(new Uri(((ToolStripMenuItem)sender).Text));
                //}
            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex);
            }
        }




        private void LoadSiteFromUrl(Uri url)
         {
            TreeNode rootNode = _tree.Nodes.Add("");

            StartLoading(rootNode);

            ExecAsync(delegate()
            {

                ProxyBridge bridge = null;
                if (AppManager.Current.ActiveProject != null)
                    bridge = AppManager.Current.ProxyBridge;
                else
                   bridge = new ProxyBridge(url.ToString());
                try
                {
                    _structure = new XmlDocument();
                    _structure.LoadXml(bridge.AddInService.GetSiteStructure().OuterXml);
                    if (_tree.InvokeRequired)
                        _tree.Invoke(new VoidDelegate(delegate()
                        {
                            rootNode = AddSiteNode(_loadingNode);
                            rootNode.Expand();
                            _tree.SelectedNode = rootNode;
                        }));
                    else
                    {
                        rootNode = AddSiteNode(_loadingNode);
                        rootNode.Expand();
                        _tree.SelectedNode = rootNode;
                    }

                   

                    RefreshMenu();
                }
                catch (Exception ex)
                {
                    ExceptionUtil.Handle(ex);
                }
                finally
                {
                    StopLoading();
                }
            });
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

            refreshMenu.Enabled = (tag.SiteTag != null);

            if (string.IsNullOrEmpty(previousUrl))
            {
                previousUrl = tag.SiteTag.Url;
                
            }

            if (_solutionMenu != null && tag.SiteTag.Url != previousUrl)
            {
                previousUrl = tag.SiteTag.Url;
                _solutionMenu.SetNodeTag(previousUrl);                

            }


        }

        string previousUrl = string.Empty;


        private void OpenSiteCompleted(object sender, GetSiteStructureCompletedEventArgs e)
        {
            try
            {
                _structure = new XmlDocument();
                _structure.LoadXml(e.Result.OuterXml);

                TreeNode rootNode = AddSiteNode(_loadingNode);
                rootNode.Expand();
                _tree.SelectedNode = rootNode;

                RefreshMenu();
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

                preloadComplete = true;
                AddWebNode(_loadingNode, _structure.DocumentElement);
                //preloadComplete = true;

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
            preloadComplete = false;
            _loadingNode = targetNode;

            if (_tree.InvokeRequired)
                _tree.Invoke(new VoidDelegate(delegate()
                {
                    _loadingNode.Text = (targetNode.Text == string.Empty) ? "Loading ..." : targetNode.Text + " (Loading)";

                    _loadingNode.Nodes.Clear();

                    if (targetNode.Text == "Loading ...")
                        _loadingNode.SelectedImageKey = _loadingNode.ImageKey = "LoadingIcon";

                    openMenu.Enabled = false;
                    refreshMenu.Enabled = false;
                    loading = true;
                }));
            else
            {
                _loadingNode.Text = (targetNode.Text == string.Empty) ? "Loading ..." : targetNode.Text + " (Loading)";

                _loadingNode.Nodes.Clear();

                if (targetNode.Text == "Loading ...")
                    _loadingNode.SelectedImageKey = _loadingNode.ImageKey = "LoadingIcon";

                openMenu.Enabled = false;
                refreshMenu.Enabled = false;
                loading = true;
            }
        }

        bool loading = false;

        private void StopLoading()
        {
            _loadingNode = null;
            if (openMenu.Owner.InvokeRequired)
                openMenu.Owner.Invoke(new VoidDelegate(delegate()
                {
                    openMenu.Enabled = true;
                    refreshMenu.Enabled = true;
                    deploymentMenu.Enabled = true;
                }));
            else
            {
                openMenu.Enabled = true;
                refreshMenu.Enabled = true;
                deploymentMenu.Enabled = true;
            }
            loading = false;
            preloadComplete = true;
        }

        private void NodeBeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (loading) return;
            UpdateActiveTag(e.Node.Tag as NodeTag);

            if (e.Node.Nodes[0].Text == "Loading")
            {
                e.Cancel = true;

                ExecAsync(delegate()
                {
                    ProxyBridge bridge = new ProxyBridge(_activeNodeTag.SiteTag.Url);

                    StartLoading(_activeNodeTag.WebTag.Node);

                    
                    _structure = new XmlDocument();
                    _structure.LoadXml(bridge.AddInService.GetWebStructure(_activeNodeTag.WebTag.ID).OuterXml);


                    AddWebNode(_loadingNode, _structure.DocumentElement);

                    StopLoading();
                    loading = true;
                    if (_tree.InvokeRequired)
                        _tree.Invoke(new VoidDelegate(delegate() { e.Node.Expand(); }));
                    else
                        e.Node.Expand();
                    loading = false;
                   
                });
            }

            e.Cancel = false;
            
        }


        internal void RefreshSite()
        {
            _tree.Nodes.Clear();

            refreshMenu.Visible = true;

            if (AppManager.Current.ActiveProject == null)
            {
                openMenu.Visible = true;
                deploymentMenu.Visible = false;

                if (_openMenu == null)
                    _openMenu = new OpenMenu(openMenu, OpenMenuClick);
            }
            else
            {

                deploymentMenu.Visible = true;
            //    openMenu.Visible = false;

                try
                {
                    string url = SPEnvironmentInfo.Parse(AppManager.Current.ActiveProject).WebApplicationUrl;

                    _solutionMenu.SetNodeTag(url);                   

                    LoadSiteFromUrl(new Uri(url));

                }
                catch (Exception ex)
                {
                    ExceptionUtil.Handle(ex);
                }
            }
        }

        
        
            
        
    }
}
