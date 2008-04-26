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
            treeView1.ImageList.Images.Add("ViewIcon", Resources.Images.Files.ICXML);
            treeView1.ImageList.Images.Add("ViewsIcon", Resources.Images.Files.ICZIP);

           
        }

        Domain.Utilties.WatcherUtilitiy w;


        private Domain.Utilties.ApplicationUtility _applicationUtility;

        public Domain.Utilties.ApplicationUtility ApplicationUtility
        {
            get {
                if (_applicationUtility == null)
                    _applicationUtility = new Rapid.Tools.SPDeploy.AddIn.Domain.Utilties.ApplicationUtility(ApplicationObject);
                return _applicationUtility; }
            set { _applicationUtility = value; }
        }


       

        public delegate void VoidDelegate();

        public void InvokeTreeView(VoidDelegate method)
        {
            if (treeView1.InvokeRequired)
                treeView1.Invoke(method);
            else
                method.BeginInvoke(null, null);
        }


        public void InvokeMenu(VoidDelegate dele)
        {
            if (menuStrip1.InvokeRequired)
                menuStrip1.Invoke(dele);
            else
                dele.BeginInvoke(null, null);
        }

        public void FillSolutionMenu()
        {
            foreach (SPToolsWebService.Solution sol in ServiceInstance.GetSols())
            {
                solutionToolStripMenuItem.DropDownItems.Clear();
                solutionToolStripMenuItem.Image = Resources.Images.Files.IMNBUSY;

                if (string.Compare(sol.Name, ApplicationObject.Solution.Projects.Item(1).Name + ".wsp", true) == 0)
                {
                    if (sol.Deployed)
                    {                      
                        
                        solutionToolStripMenuItem.Image = Resources.Images.Files.IMNON;
                        ToolStripItem item = solutionToolStripMenuItem.DropDownItems.Add("Retract");                        
                        item.Click += delegate(object sender, EventArgs e) {
                            RapidOutputWindow.Instance.Activate();
                            RapidOutputWindow.Instance.Clear();
                            RapidOutputWindow.Instance.Write("Retracting...");
                            RapidOutputWindow.Instance.Write(ServiceInstance.RetractSolution(ApplicationUtility.GetProjectName() + ".wsp"));
                            FillSolutionMenu();
                        };
                        break;
                    }
                    else
                    {
                        
                        ToolStripItem item = solutionToolStripMenuItem.DropDownItems.Add("Deploy");
                        item.Click += delegate(object sender, EventArgs e) {
                            RapidOutputWindow.Instance.Activate();
                            RapidOutputWindow.Instance.Clear();
                            RapidOutputWindow.Instance.Write("Deploying...");
                            RapidOutputWindow.Instance.Write(ServiceInstance.DeploySolution(ApplicationUtility.GetProjectName() + ".wsp"));
                            FillSolutionMenu();
                        
                        };

                        ToolStripItem item2 = solutionToolStripMenuItem.DropDownItems.Add("Delete");
                        
                        
                        item2.Click += delegate(object sender, EventArgs e) {
                            RapidOutputWindow.Instance.Activate();
                            RapidOutputWindow.Instance.Clear();
                            RapidOutputWindow.Instance.Write("Deleting...");
                            RapidOutputWindow.Instance.Write(ServiceInstance.DeleteSolution(ApplicationUtility.GetProjectName() + ".wsp"));
                            FillSolutionMenu();
                        };
                        break;
                    }
                }
            }

            if (solutionToolStripMenuItem.DropDownItems.Count == 0)
            {
                ToolStripItem item = solutionToolStripMenuItem.DropDownItems.Add("Add");
                item.Click += delegate(object sender, EventArgs e) {

                    EnvDTE.SolutionBuild b = ApplicationObject.Solution.SolutionBuild;
                    b.Build(true);

                    string name = ApplicationObject.Solution.Projects.Item(1).Name;

                    RapidOutputWindow.Instance.Activate();
                    RapidOutputWindow.Instance.Clear();
                    RapidOutputWindow.Instance.Write("Compiling WSP..." + Environment.NewLine);
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("C:\\Windows\\Microsoft.NET\\Framework\\v2.0.50727\\MSBuild.exe");
                    psi.Arguments = "/target:CompileWsp " + ApplicationObject.Solution.Projects.Item(1).FullName;
                    psi.CreateNoWindow = true;
                    psi.UseShellExecute = false;

                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo = psi;

                    p.Start();                  


                    p.WaitForExit(10000);

                    if (p.ExitCode != 0)
                    {
                        RapidOutputWindow.Instance.Write("Compiling WSP Failed!" + Environment.NewLine);
                        return;
                    }

                    RapidOutputWindow.Instance.Write("Compiling WSP Successful" + Environment.NewLine + Environment.NewLine);
                    RapidOutputWindow.Instance.Write("Adding Solution..." + Environment.NewLine);

                    string wspPath = ApplicationObject.Solution.Projects.Item(1).FullName;
                    wspPath = wspPath.Remove(wspPath.LastIndexOf("\\"));
                    wspPath = wspPath + "\\obj\\Debug\\" + ApplicationObject.Solution.Projects.Item(1).Name + ".wsp";

                    ServiceInstance.SaveFile(name + "\\" + name + ".wsp", File.ReadAllBytes(wspPath));

                    RapidOutputWindow.Instance.Write(ServiceInstance.AddSolution(@"c:\_spdeploy\" + name + "\\" + name + ".wsp"));

                    FillSolutionMenu();
                };
            }
        }

        public void FillTreeView()
        {

            //Domain.Menus.SolutionMenu sm = new Rapid.Tools.SPDeploy.AddIn.Domain.Menus.SolutionMenu(menuStrip1);
           // sm.AddItems();


            Domain.Menus.SolutionMenu sm = new Rapid.Tools.SPDeploy.AddIn.Domain.Menus.SolutionMenu(solutionToolStripMenuItem);
            sm.RefreshMenuItemsAsync();

            


            return;


            VoidDelegate del = new VoidDelegate(delegate()
            {
                w = Domain.Utilties.WatcherUtilitiy.Instance;
                w.serviceInstance = ServiceInstance;
            });

            del.BeginInvoke(null, null);

            VoidDelegate del2 = new VoidDelegate(FillSolutionMenu);
            del2.BeginInvoke(null, null);
           


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

            XmlNodeList _nodeList = xmlElement.SelectNodes("Views/View");
            currentNode = currentNode.Nodes.Add("Views");
            currentNode.SelectedImageKey = currentNode.ImageKey = "FolderIcon";
            currentNode.Tag = NodeTagFactory.Create(currentNode, ApplicationObject, NodeType.Null);            
            foreach (XmlNode no in _nodeList)
            {
                           
                TreeNode tNode = currentNode.Nodes.Add(no.Attributes["Title"].Value);
                tNode.SelectedImageKey = tNode.ImageKey = "ViewIcon";   
                WebNodeTag t = NodeTagFactory.Create(tNode, ApplicationObject, NodeType.View);
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
            currentNode.Tag = NodeTagFactory.Create(currentNode, ApplicationObject, NodeType.Null);

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
