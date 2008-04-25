using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EnvDTE80;
using System.Net;
using System.IO;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Controls
{
    public partial class SolutionsControl : UserControl
    {

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
                    _serviceInstance.Credentials = CredentialCache.DefaultNetworkCredentials;
                }
                return _serviceInstance;
            }
            set { _serviceInstance = value; }
        }


        public delegate void VoidDelegate();


        public void FillTreeView()
        {
            VoidDelegate dele = new VoidDelegate(delegate()
            {
                Invoke(treeView1, delegate()
                {
                    treeView1.Enabled = false;
                });
                Rapid.Tools.SPDeploy.AddIn.SPToolsWebService.Solution[] sols = ServiceInstance.GetSols();
                Invoke(treeView1, delegate()
                {
                    treeView1.Nodes.Clear();
                });
                foreach (Rapid.Tools.SPDeploy.AddIn.SPToolsWebService.Solution sol in sols)
                {
                    TreeNode t = new TreeNode(sol.Name);
                    t.Tag = sol.Deployed;
                    if (sol.Deployed)
                        t.ImageKey = t.SelectedImageKey = "Deployed";
                    else
                        t.ImageKey = t.SelectedImageKey = "NotDeployed";

                    Invoke(treeView1, delegate()
                    {
                        treeView1.Nodes.Add(t);
                    });
                }
                Invoke(treeView1, delegate()
                {
                    treeView1.Enabled = true;
                });
            });
            dele.BeginInvoke(null, null);
        }

        public void Invoke(TreeView treeView, VoidDelegate del)
        {
            if (treeView1.InvokeRequired)
                treeView1.Invoke(del);
            else
                del.BeginInvoke(null, null);
        }

        private Domain.RapidOutputWindow _outputWindow;

        public Domain.RapidOutputWindow RapidOutputWindow
        {
            get
            {

                if (_outputWindow == null)
                    if (ApplicationObject == null)
                        return null;
                    else
                        _outputWindow = new Rapid.Tools.SPDeploy.AddIn.Domain.RapidOutputWindow(ApplicationObject);
                return _outputWindow;
            }
            set { _outputWindow = value; }
        }

        


        public SolutionsControl()
        {
            InitializeComponent();

            treeView1.ImageList = new ImageList();
            treeView1.ImageList.Images.Add("Deployed", Resources.Images.Files.ewr236l);
            treeView1.ImageList.Images.Add("NotDeployed", Resources.Images.Files.ewr237l);

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                if ((bool)e.Node.Tag)
                {
                    MenuItem m2 = new MenuItem("Retract", delegate(object se, EventArgs ev)
                    {
                        VoidDelegate del = new VoidDelegate(delegate()
                        {
                            Invoke(treeView1, delegate()
                {
                    treeView1.Enabled = false;
                });
                            RapidOutputWindow.Activate();
                            RapidOutputWindow.Clear();
                            RapidOutputWindow.Write("Retracting..." + Environment.NewLine);
                            RapidOutputWindow.Write(ServiceInstance.RetractSolution((string)((MenuItem)se).Tag));
                            FillTreeView();

                        });
                        del.BeginInvoke(null, null);
                    });
                    m2.Tag = e.Node.Text;
                    m.MenuItems.Add(m2);
                }
                else
                {
                    MenuItem m2 = new MenuItem("Deploy", delegate(object se, EventArgs ev)
                    {
                        VoidDelegate del = new VoidDelegate(delegate()
                        {
                            Invoke(treeView1, delegate()
                {
                    treeView1.Enabled = false;
                });
                            RapidOutputWindow.Activate();
                            RapidOutputWindow.Clear();
                            RapidOutputWindow.Write("Deploying..." + Environment.NewLine);
                            RapidOutputWindow.Write(ServiceInstance.DeploySolution((string)((MenuItem)se).Tag));
                            FillTreeView();
                        });
                        del.BeginInvoke(null, null);
                    });
                    m2.Tag = e.Node.Text;
                    m.MenuItems.Add(m2);
                    m2 = new MenuItem("Delete", delegate(object se, EventArgs ev)
                   {
                       VoidDelegate del = new VoidDelegate(delegate()
                        {
                            Invoke(treeView1, delegate()
                {
                    treeView1.Enabled = false;
                });
                            RapidOutputWindow.Activate();
                            RapidOutputWindow.Clear();
                            RapidOutputWindow.Write("Deleting..." + Environment.NewLine);
                            RapidOutputWindow.Write(ServiceInstance.DeleteSolution((string)((MenuItem)se).Tag));
                            FillTreeView();
                        });
                       del.BeginInvoke(null, null);
                   });
                    m2.Tag = e.Node.Text;
                    m.MenuItems.Add(m2);
                }
                e.Node.ContextMenu = m;
            }
            treeView1.SelectedNode = e.Node;
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FillTreeView();
        }
    }
}
