using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using System.Xml;
using System.Security.Principal;
using System.Net;
using System.IO;
using Rapid.Tools.SPDeploy.AddIn.Proxies.SiteData;
using Rapid.Tools.SPDeploy.AddIn.Proxies.Webs;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using System.Diagnostics;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Controls
{
    public partial class SPToolsWindowControl : UserControl
    {
        TreeNode _currentNode;

        public Project _project;
        public string path;
        public string server;

        public XmlNode getUserConfigurationNode()
        {
            XmlDocument _spUserDoc = new XmlDocument();
            _spUserDoc.Load(string.Concat(path, "\\Properties\\SPDeploy.user"));

            XmlNamespaceManager _spUserNameSpaceManager = new XmlNamespaceManager(_spUserDoc.NameTable);
            _spUserNameSpaceManager.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

            XmlNode _node = null;
            foreach (XmlNode node in _spUserDoc.SelectNodes("/n:Project/n:PropertyGroup", _spUserNameSpaceManager))
            {
                if (node.Attributes["Condition"] != null && node.Attributes["Condition"].Value == string.Format("$(USERNAME) == '{0}'", WindowsIdentity.GetCurrent().Name.Split('\\')[1]))
                    _node = node;
            }
            return _node;
        }


        public SPToolsWindowControl()
        {
            InitializeComponent();
            Load += new EventHandler(CustomControl_Load);
        }
        
        string port;
        void CustomControl_Load(object sender, EventArgs e)
        {
            try
            {
                System.Threading.Thread t = new System.Threading.Thread(delegate()
                {
					WebsProxy w = new WebsProxy();
                    XmlNode node = getUserConfigurationNode();
                    if (node.ChildNodes[1].InnerText.Split(':').Length > 2)
                        port = node.ChildNodes[1].InnerText.Split(':')[2];
                    w.Url = "http://" + node.ChildNodes[0].InnerText + (string.IsNullOrEmpty(port) ? "" : ":" + port) + "/_vti_bin/webs.asmx";
                    server = node.ChildNodes[0].InnerText;
                    toolStripTextBox1.Text = server;
                    toolStripTextBox2.Text = port;


                    w.Credentials = CredentialCache.DefaultNetworkCredentials;
                    XmlNode n = w.GetAllSubWebCollection();


                    if (treeView1.InvokeRequired)
                    {
                        treeView1.Invoke(new mdel(delegate()
                        {
                            _currentNode = treeView1.Nodes.Add(n.ChildNodes[0].Attributes["Url"].Value, n.ChildNodes[0].Attributes["Title"].Value + " (" + n.ChildNodes[0].Attributes["Url"].Value + ")");
                            for (int i = 1; i < n.ChildNodes.Count; i++)
                            {
                                AddTreeNode(n.ChildNodes[i]);
                            }
                            treeView1.ExpandAll();
                        }));
                    }
                    else
                    {
                        _currentNode = treeView1.Nodes.Add(n.ChildNodes[0].Attributes["Url"].Value, n.ChildNodes[0].Attributes["Title"].Value + " (" + n.ChildNodes[0].Attributes["Url"].Value + ")");
                        for (int i = 1; i < n.ChildNodes.Count; i++)
                        {
                            AddTreeNode(n.ChildNodes[i]);
                        }
                        treeView1.ExpandAll();
                    }

                });
                t.Start();
            }
            catch (Exception)
            {
            }
        }

        delegate void mdel();

        private List<string> getActivatedFeatures(string web)
        {
			WebsProxy w = new WebsProxy();
            w.Url = string.Format("{0}/_vti_bin/Webs.asmx", web);
            w.Credentials = CredentialCache.DefaultNetworkCredentials;
            return new List<string>(w.GetActivatedFeatures().ToLower().Split(','));
        }


        private void AddTreeNode(XmlNode node)
        {
            while (!node.Attributes["Url"].Value.Contains(_currentNode.Name))
            {
                _currentNode = _currentNode.Parent;
            }
            _currentNode = _currentNode.Nodes.Add(node.Attributes["Url"].Value, node.Attributes["Title"].Value);
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                // Show menu only if the right mouse button is clicked.
                if (e.Button == MouseButtons.Right)
                {
                    try
                    {
                        // Point where the mouse is clicked.
                        Point p = new Point(e.X, e.Y);

                        // Get the node that the user has clicked.
                        TreeNode node = treeView1.GetNodeAt(p);
                        if (node != null)
                        {

                            // Select the node the user has clicked.
                            // The node appears selected until the menu is displayed on the screen.
                            TreeNode m_OldSelectNode = treeView1.SelectedNode;

                            treeView1.SelectedNode = node;

                            // Find the appropriate ContextMenu depending on the selected node.



                            DirectoryInfo di = new DirectoryInfo(path);
                            List<string> projectFeatures = new List<string>();
                            List<FeatureManifest> manis = new List<FeatureManifest>();
                            foreach (FileInfo fi in di.GetFiles("feature.xml", SearchOption.AllDirectories))
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.Load(fi.FullName);
                                FeatureManifest man = new FeatureManifest(doc);
                                projectFeatures.Add(man.Id);
                                manis.Add(man);
                            }


                            ContextMenu cm = new ContextMenu();

                            MenuItem m = new MenuItem("Add Feature");
                            MenuItem m2 = new MenuItem("Remove Feature");


                            List<string> activatedFeatures = getActivatedFeatures(node.Name);

                            foreach (FeatureManifest fm in manis)
                            {
                                if (activatedFeatures.Contains(fm.Id.ToLower()))
                                {
                                    if (node.Parent == null || fm.Scope == "Web")
                                    {
                                        MenuItem mii = new MenuItem(fm.Title);
                                        mii.Click += delegate(object s, EventArgs ea)
                                            {
                                                string _tpath = Path.GetTempPath();
                                                _tpath += "SPDeployBuild.bat";

                                                using (TextWriter _textWriter = new StreamWriter(_tpath, false))
                                                {
                                                    string mask = @"""C:\Program Files\MSBuild\SPDeploy\v1\psexec.exe"" -accepteula -s ";
                                                    mask += @"-w {1} \\{0} {2} {3}";
                                                    _textWriter.WriteLine(mask, "wa1devnint01", @"c:\_spdeploy", "", "stsadm -o deactivatefeature -id " + ((MenuItem)s).Tag + " -url " + node.Name);

                                                    _textWriter.WriteLine("pause");
                                                    _textWriter.Flush();
                                                    _textWriter.Close();
                                                }

                                                ProcessStartInfo _processInfo = new ProcessStartInfo(_tpath);

                                                _processInfo.UseShellExecute = false;
                                                _processInfo.CreateNoWindow = false;
                                                System.Diagnostics.Process _process = new System.Diagnostics.Process();

                                                _process.StartInfo = _processInfo;
                                                //_process.EnableRaisingEvents = true;

                                                _process.Start();
                                            };
                                        mii.Tag = fm.Id;


                                        m2.MenuItems.Add(mii);
                                    }
                                }
                                else
                                {
                                    if (node.Parent == null || fm.Scope == "Web")
                                    {
                                        MenuItem mii = new MenuItem(fm.Title);
                                        mii.Click += delegate(object s, EventArgs ea)
                                            {
                                                string _tpath = Path.GetTempPath();
                                                _tpath += "SPDeployBuild.bat";

                                                using (TextWriter _textWriter = new StreamWriter(_tpath, false))
                                                {
                                                    string mask = @"""C:\Program Files\MSBuild\SPDeploy\v1\psexec.exe"" -accepteula -s ";
                                                    mask += @"-w {1} \\{0} {2} {3}";
                                                    _textWriter.WriteLine(mask, "wa1devnint01", @"c:\_spdeploy", "", "stsadm -o activatefeature -id " + ((MenuItem)s).Tag + " -url " + node.Name);

                                                    _textWriter.WriteLine("pause");
                                                    _textWriter.Flush();
                                                    _textWriter.Close();
                                                }

                                                ProcessStartInfo _processInfo = new ProcessStartInfo(_tpath);

                                                _processInfo.UseShellExecute = false;
                                                _processInfo.CreateNoWindow = false;
                                                System.Diagnostics.Process _process = new System.Diagnostics.Process();

                                                _process.StartInfo = _processInfo;
                                                //_process.EnableRaisingEvents = true;

                                                _process.Start();
                                            };
                                        mii.Tag = fm.Id;


                                        m.MenuItems.Add(mii);
                                    }
                                }
                            }

                            if (m.MenuItems.Count > 0)
                                cm.MenuItems.Add(m);
                            if (m2.MenuItems.Count > 0)
                                cm.MenuItems.Add(m2);

                            cm.MenuItems.Add("Browse", delegate(object s, EventArgs ea)
                                       {
                                           System.Diagnostics.Process _process = new System.Diagnostics.Process();
                                           _process.StartInfo.FileName = node.Name;
                                           _process.Start();
                                       });




                            if (cm.MenuItems.Count == 0)
                            {
                                MenuItem mi = new MenuItem("no actions available");
                                mi.Enabled = false;
                                cm.MenuItems.Add(mi);
                            }

                            node.ContextMenu = cm;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }



            }
        }

        private void refToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void changeServerToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void refreshToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            CustomControl_Load(this, EventArgs.Empty);
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path + "\\Properties\\SPDeploy.user");

                XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
                nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

                XmlNode node = null;
                foreach (XmlNode no in doc.SelectNodes("/n:Project/n:PropertyGroup", nm))
                {
                    if (no.Attributes["Condition"] != null && no.Attributes["Condition"].Value == "$(USERNAME) == 'george.olson'")
                        node = no;
                }
                node.ChildNodes[0].InnerText = toolStripTextBox1.Text;
                doc.Save(path + "\\Properties\\SPDeploy.user");
                //refreshToolStripMenuItem1.PerformClick();

                ToolStripMenuItem tmi = toolStripTextBox1.OwnerItem as ToolStripMenuItem;
                tmi.DropDown.Close();
                toolsToolStripMenuItem.DropDown.Close();


                refreshToolStripMenuItem1_Click(this, EventArgs.Empty);

            }
        }

        private void toolStripTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path + "\\Properties\\SPDeploy.user");

                XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
                nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

                XmlNode node = null;
                foreach (XmlNode no in doc.SelectNodes("/n:Project/n:PropertyGroup", nm))
                {
                    if (no.Attributes["Condition"] != null && no.Attributes["Condition"].Value == "$(USERNAME) == 'george.olson'")
                        node = no;
                }

                if (node.ChildNodes[1].InnerText.Split(':').Length > 2)
                    port = node.ChildNodes[1].InnerText.Split(':')[2];
                node.ChildNodes[1].InnerText = node.ChildNodes[1].InnerText.Replace(":" + port, ":" + toolStripTextBox2.Text);
                doc.Save(path + "\\Properties\\SPDeploy.user");


                refreshToolStripMenuItem1_Click(this, EventArgs.Empty);
            }
        }

        //delegate void abb(object s, EventArgs e);

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!e.Node.IsExpanded)
                e.Node.Expand();

            System.Diagnostics.Process _process = new System.Diagnostics.Process();
            _process.StartInfo.FileName = e.Node.Name;
            _process.Start();
        }      

    }
}
