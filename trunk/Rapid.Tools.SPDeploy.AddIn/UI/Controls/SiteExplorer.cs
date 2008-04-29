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
using Rapid.Tools.SPDeploy.AddIn.Domain.Utilties;
using Rapid.Tools.SPDeploy.AddIn.UI.Forms;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Controls
{
    public partial class SiteExplorer : UserControl
    {
        private TreeNode currentNode;

        private XmlDocument _siteStructureDocument;

        public XmlDocument SiteStructureDocument
        {
            get { return _siteStructureDocument; }
            set { _siteStructureDocument = value; }
        }

		private ProxyBridge _bridge = new ProxyBridge();

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

        }

        Domain.Utilties.WatcherUtil w;


        public class abc : ProfessionalColorTable
        {            
            public override Color ImageMarginGradientBegin
            {
                get
                {
                    return SystemColors.ControlLight;
                }
            }

            public override Color ImageMarginGradientMiddle
            {
                get
                {
                    return SystemColors.Control;
                }
            }

            public override Color ImageMarginGradientEnd
            {
                get
                {
                    return SystemColors.ControlDark;
                }
            }

            

            public override Color  MenuItemSelectedGradientBegin
            {
                get
                {
                    return SystemColors.GradientInactiveCaption;
                }
            }

           

            public override Color MenuItemSelectedGradientEnd
            {
                get
                {
                    return SystemColors.GradientInactiveCaption;
                }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get
                {
                    return SystemColors.ControlLight;
                }
            }

            public override Color MenuItemPressedGradientMiddle
            {
                get
                {
                    return SystemColors.ControlLight;
                }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get
                {
                    return SystemColors.ControlLight;
                }
            }            

            public override Color ButtonSelectedGradientBegin
            {
                get
                {
                    return SystemColors.GradientInactiveCaption;
                }
            }

            public override Color ButtonSelectedGradientMiddle
            {
                get
                {
                    return SystemColors.GradientInactiveCaption;
                }
            }

            public override Color ButtonSelectedGradientEnd
            {
                get
                {
                    return SystemColors.GradientInactiveCaption;
                }
            }
            
           
            public override Color MenuItemSelected
            {
                get
                {
                    return SystemColors.GradientInactiveCaption;
                }
            }
        }

      

        private WatcherUtil util;
        public delegate void VoidDelegate();
        public void FillTreeView()
        {

            abc b = new abc();
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(b);

			LoadingForm lf = new LoadingForm();
            lf.Show();

            SolutionMenu sm = new SolutionMenu(solutionToolStripMenuItem);
            sm.RefreshAsync();

            util = WatcherUtil.Instance;

            SiteStructureDocument = new XmlDocument();
            treeView1.Nodes.Add("Loading");
            treeView1.Nodes[0].SelectedImageKey = treeView1.Nodes[0].ImageKey = "LoadingIcon";
            treeView1.Enabled = false;

            _bridge.AddInService.GetSiteStructureCompleted += new GetSiteStructureCompletedEventHandler(ServiceInstance_GetSiteStructureCompleted);
			_bridge.AddInService.GetSiteStructureAsync(AppManager.Instance.GetWebApplicationUrl());

            lf.Close();


            return;
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

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Tag != null && e.Node.Tag is INodeTag)
            {
                INodeTag tag = e.Node.Tag as INodeTag;
                tag.Action();
            }
        }

        private void abbToolStripMenuItem_Click(object sender, EventArgs e)
        {
			foreach (FileInfo fi in AppManager.Instance.GetFeatureFiles())
            {
                MessageBox.Show(getState(fi).ToString());
            }
        }

        public enum State
        {
            NotInstalled,
            Installed,
            InstalledNotUpdated,
            Deployed,
            DeployedNotUpdated            
        }

        private State getState(FileInfo fi)
        {            
			DirectoryInfo dir = fi.Directory;

            foreach (FileInfo f in dir.GetFiles("*", SearchOption.AllDirectories))
            {
				string tpath = AppManager.Instance.GetRandomTempPath();

				byte[] rcontents = _bridge.AddInService.CompareFeatureFile(f.FullName.Substring(dir.FullName.Remove(dir.FullName.LastIndexOf("\\")).Length + 1));
				
				File.WriteAllBytes(tpath, rcontents);

                if (!FileCompare(tpath, f.FullName))
                    return State.InstalledNotUpdated;                    
			}

            return State.Installed;
        }

        // This method accepts two strings the represent two files to 
        // compare. A return value of 0 indicates that the contents of the files
        // are the same. A return value of any other value indicates that the 
        // files are not the same.
        private bool FileCompare(string file1, string file2)
        {
            int file1byte;
            int file2byte;
            FileStream fs1;
            FileStream fs2;

            // Determine if the same file was referenced two times.
            if (file1 == file2)
            {
                // Return true to indicate that the files are the same.
                return true;
            }

            // Open the two files.
            fs1 = new FileStream(file1, FileMode.Open);
            fs2 = new FileStream(file2, FileMode.Open);

            // Check the file sizes. If they are not the same, the files 
            // are not the same.
            if (fs1.Length != fs2.Length)
            {
                // Close the file
                fs1.Close();
                fs2.Close();

                // Return false to indicate files are different
                return false;
            }

            // Read and compare a byte from each file until either a
            // non-matching set of bytes is found or until the end of
            // file1 is reached.
            do
            {
                // Read one byte from each file.
                file1byte = fs1.ReadByte();
                file2byte = fs2.ReadByte();
            }
            while ((file1byte == file2byte) && (file1byte != -1));

            // Close the files.
            fs1.Close();
            fs2.Close();

            // Return the success of the comparison. "file1byte" is 
            // equal to "file2byte" at this point only if the files are 
            // the same.
            return ((file1byte - file2byte) == 0);
        }


    }


}
