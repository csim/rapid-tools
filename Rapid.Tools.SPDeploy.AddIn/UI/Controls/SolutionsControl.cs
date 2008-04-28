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
using Rapid.Tools.SPDeploy.AddIn.Domain;
using Rapid.Tools.SPDeploy.AddIn.Proxies.AddIn;

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


        private AddInProxy _serviceInstance;

		public AddInProxy ServiceInstance
        {
            get
            {
                if (_serviceInstance == null)
                {
                    _serviceInstance = new AddInProxy();
                    _serviceInstance.Url = string.Concat(Domain.Utilties.Functions.GetSiteUrlFromProject(ApplicationObject), "/_layouts/RapidTools/Services/AddIn.asmx");
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
                bool added = false;
                Invoke(treeView1, delegate()
                {
                    treeView1.Enabled = false;
                });
                Proxies.AddIn.Solution[] sols = ServiceInstance.GetSols();
                Invoke(treeView1, delegate()
                {
                    treeView1.Nodes.Clear();
                });
                foreach (Proxies.AddIn.Solution sol in sols)
                {
                    if (string.Compare(sol.Name, ApplicationObject.Solution.Projects.Item(1).Name + ".wsp", true) == 0)
                        added = true;

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

                if (!added)
                {
                    TreeNode ndt = new TreeNode(ApplicationObject.Solution.Projects.Item(1).Name + ".wsp");
                    ndt.ImageKey = ndt.SelectedImageKey = "NotAdded";

                    Invoke(treeView1, delegate()
                    {
                        treeView1.Nodes.Add(ndt);
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
        


        public SolutionsControl()
        {
            InitializeComponent();

            treeView1.ImageList = new ImageList();
            treeView1.ImageList.Images.Add("Deployed", Resources.Images.Files.ewr236l);
            treeView1.ImageList.Images.Add("NotDeployed", Resources.Images.Files.ewr238l);
            treeView1.ImageList.Images.Add("NotAdded", Resources.Images.Files.ewr237l);

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                if (e.Node.Tag == null)
                {
                    m.MenuItems.Add("Add Solution", delegate(object se, EventArgs ev)
                    {

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

                        FillTreeView();

                    });
                }
                else if ((bool)e.Node.Tag)
                {
                    MenuItem m2 = new MenuItem("Retract", delegate(object se, EventArgs ev)
                    {
                        VoidDelegate del = new VoidDelegate(delegate()
                        {
                            Invoke(treeView1, delegate()
                {
                    treeView1.Enabled = false;
                });
                            RapidOutputWindow.Instance.Activate();
                            RapidOutputWindow.Instance.Clear();
                            RapidOutputWindow.Instance.Write("Retracting..." + Environment.NewLine);
                            RapidOutputWindow.Instance.Write(ServiceInstance.RetractSolution((string)((MenuItem)se).Tag));
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
                            RapidOutputWindow.Instance.Activate();
                            RapidOutputWindow.Instance.Clear();
                            RapidOutputWindow.Instance.Write("Deploying..." + Environment.NewLine);
                            RapidOutputWindow.Instance.Write(ServiceInstance.DeploySolution((string)((MenuItem)se).Tag));
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
                            RapidOutputWindow.Instance.Activate();
                            RapidOutputWindow.Instance.Clear();
                            RapidOutputWindow.Instance.Write("Deleting..." + Environment.NewLine);
                            RapidOutputWindow.Instance.Write(ServiceInstance.DeleteSolution((string)((MenuItem)se).Tag));
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
