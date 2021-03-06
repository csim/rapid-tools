using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Rapid.Tools.SPDeploy.AddIn.Domain;
using Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags;
using System.Threading;

namespace Rapid.Tools.SPDeploy.AddIn
{
    public class DeploymentMenu
    {

        private ToolStripMenuItem _solutionItem;

        public ToolStripMenuItem SolutionItem
        {
            get { return _solutionItem; }
            set { _solutionItem = value; }
        }

        public EventHandler MachineChanged;

        private ToolStripMenuItem _addSolutionItem;
        private ToolStripMenuItem _deploySolutionItem;
        private ToolStripMenuItem _retractSolutionItem;
        private ToolStripMenuItem _deleteSolutionItem;
        private ToolStripMenuItem _cycleSolutionItem;
        private ToolStripMenuItem _upgradeSolutionItem;
        private ToolStripMenuItem _serverUrl;

        private delegate void VoidDelegate();
        private void InvokeIfRequired(VoidDelegate method)
        {
            if (_solutionItem.Owner.InvokeRequired)
                _solutionItem.Owner.Invoke(method);
            else
                method();
        }

        EventHandler serverClick;
        
        public DeploymentMenu(ToolStripMenuItem menuItem, string siteUrl, EventHandler handler)
        {
            serverClick = handler;
            
            _solutionItem = menuItem;
            _solutionItem.Image = Resources.Images.Files.IMNUNK;
            _addSolutionItem = new ToolStripMenuItem("Add Solution");
            _deploySolutionItem = new ToolStripMenuItem("Deploy Solution");
            _retractSolutionItem = new ToolStripMenuItem("Retract Solution");
            _deleteSolutionItem = new ToolStripMenuItem("Delete Solution");
            _cycleSolutionItem = new ToolStripMenuItem("Cycle Solution");
            _upgradeSolutionItem = new ToolStripMenuItem("Upgrade Solution");
            _serverUrl = new ToolStripMenuItem(siteUrl);
            _serverUrl.Click += serverClick;

            InvokeIfRequired(delegate()
            {
                _solutionItem.DropDownItems.Add(_serverUrl);
            });
            InvokeIfRequired(delegate()
            {
                _solutionItem.DropDownItems.Add(new ToolStripSeparator());
            });
            InvokeIfRequired(delegate()
            {
                _solutionItem.DropDownItems.Add(_retractSolutionItem);
            });
            InvokeIfRequired(delegate()
            {
                _solutionItem.DropDownItems.Add(_deploySolutionItem);
            });
            InvokeIfRequired(delegate()
            {
                _solutionItem.DropDownItems.Add(_deleteSolutionItem);
            });
            InvokeIfRequired(delegate()
            {
                _solutionItem.DropDownItems.Add(_addSolutionItem);
            });
            InvokeIfRequired(delegate()
            {
                _solutionItem.DropDownItems.Add(_cycleSolutionItem);
            });
            InvokeIfRequired(delegate()
            {
                _solutionItem.DropDownItems.Add(_upgradeSolutionItem);
            });

            _serverUrl.Tag = "ServerButton";
            //_serverUrl.Click += ev;
            _retractSolutionItem.Click += new EventHandler(retractSolutionItem_Click);
            _deploySolutionItem.Click += new EventHandler(deploySolutionItem_Click);
            _deleteSolutionItem.Click += new EventHandler(deleteSolutionItem_Click);
            _addSolutionItem.Click += new EventHandler(addSolutionItem_Click);
            _cycleSolutionItem.Click += new EventHandler(cycleSolutionItem_Click);
            _upgradeSolutionItem.Click += new EventHandler(upgradeSolutionItem_Click);

            hideMenuItems();
        }

       




        void upgradeSolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Upgrade, null, null);
        }

        void cycleSolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Cycle, null, null);
        }

        void addSolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Add, null, null);
        }

        void deleteSolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Delete, null, null);
        }

        void deploySolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Deploy, null, null);
        }

        public delegate void PerformActionDelegate(Action action);

        public void SetNodeTag(string url)
        {
            _serverUrl.Text = url;
            RefreshAsync();
        }

        public void PerformAction(Action action)
        {
            try
            {
                RapidOutputWindow.Instance.Activate();
                RapidOutputWindow.Instance.Clear();

                string output = "";
                string projectname = AppManager.Current.ActiveProject.Name;
                string wspname = AppManager.Current.ActiveProject.Name + ".wsp";

                ProxyBridge bridge = AppManager.Current.ProxyBridge;

                byte[] wspcontents;

                switch (action)
                {
                    case Action.Deploy:
                        AppManager.Current.WriteLine("Deploying...");
                        wspcontents = GetWspContents();
                        output = bridge.AddInService.DeploySolution(wspname);
                        AppManager.Current.WriteLine(output);
                        break;

                    case Action.Retract:
                        AppManager.Current.WriteLine("Retracting...");
                        output = bridge.AddInService.RetractSolution(wspname);
                        AppManager.Current.WriteLine(output);
                        break;

                    case Action.Delete:
                        AppManager.Current.WriteLine("Deleting...");
                        output = bridge.AddInService.DeleteSolution(wspname);
                        AppManager.Current.WriteLine(output);
                        break;

                    case Action.Cycle:
                        AppManager.Current.WriteLine("Retracting...");
                        output = bridge.AddInService.RetractSolution(wspname);
                        AppManager.Current.WriteLine(output);

                        RefreshAsync();

                        AppManager.Current.WriteLine("Deleting...");
                        output = bridge.AddInService.DeleteSolution(wspname);
                        AppManager.Current.WriteLine(output);

                        RefreshAsync();

                        wspcontents = GetWspContents();

                        output = bridge.AddInService.AddSolution(wspname, wspcontents);
                        AppManager.Current.WriteLine(output);

                        RefreshAsync();
                        AppManager.Current.WriteLine("Deploying...");
                        output = bridge.AddInService.DeploySolution(wspname);
                        AppManager.Current.WriteLine(output);
                        break;

                    case Action.Add:
                        wspcontents = GetWspContents();
                        output = bridge.AddInService.AddSolution(wspname, wspcontents);
                        AppManager.Current.WriteLine(output);
                        break;

                    case Action.Upgrade:
                        wspcontents = GetWspContents();
                        bridge.AddInService.UpgradeSolution(wspname, wspcontents);
                        break;

                    default:
                        break;
                }

                AppManager.Current.WriteLine("Completed: " + DateTime.Now);
                RefreshAsync();

            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex);
            }
        }

        private byte[] GetWspContents()
        {
            AppManager.Current.WriteLine("Compiling WSP...");
            AppManager.Current.ExecuteMSBuild("CompileWsp");

            string projectname = AppManager.Current.ActiveProject.Name;
            string projectpath = AppManager.Current.ActiveProjectPath.Directory.FullName;
            string wspname = AppManager.Current.ActiveProject.Name + ".wsp";

            // TODO: make this sensitive to the output directory based on configuration
            string wsppath = string.Format(@"{0}\bin\Debug\{1}", projectpath, wspname);

            byte[] wspcontents = File.ReadAllBytes(wsppath);

            return wspcontents;
        }

        public enum Action
        {
            Deploy,
            Retract,
            Delete,
            Add,
            Cycle,
            Upgrade
        }

        void retractSolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Retract, null, null);
        }

        private void hideMenuItems()
        {
            if (_solutionItem.Owner.InvokeRequired)
            {
                _solutionItem.Owner.Invoke(new VoidDelegate(hideMenuItemsAsync));
            }
            else
            {
                hideMenuItemsAsync();
            }
        }



        private void hideMenuItemsAsync()
        {
            _cycleSolutionItem.Visible =
            _upgradeSolutionItem.Visible =
            _addSolutionItem.Visible =
            _deploySolutionItem.Visible =
            _retractSolutionItem.Visible =
            _deleteSolutionItem.Visible =
            _deploySolutionItem.Visible = false;
        }

        public void RefreshAsync()
        {
            if (_solutionItem.Owner.InvokeRequired)
            {
                _solutionItem.Owner.Invoke(new VoidDelegate(RefreshMenuItems));
            }
            else
            {
                new VoidDelegate(RefreshMenuItems).BeginInvoke(null, null);
            }
        }



        public void RefreshMenuItems()
        {
            try
            {

                hideMenuItems();
                _solutionItem.Image = Resources.Images.Files.IMNUNK;

                Proxies.AddIn.Solution solution = null;

                ProxyBridge pb = AppManager.Current.ProxyBridge;

                Proxies.AddIn.Solution[] solutions = pb.AddInService.GetSolutions();

                string wp = AppManager.Current.ActiveProject.Name + ".wsp";

                solution = Array.Find<Proxies.AddIn.Solution>(solutions, delegate(Proxies.AddIn.Solution sol)
                        {
                            return string.Compare(sol.Name, wp, true) == 0;
                        });

                if (solution != null)
                {
                    if (solution.Deployed)
                    {
                        InvokeIfRequired(delegate()
                        {
                            _upgradeSolutionItem.Visible =
                            _cycleSolutionItem.Visible =
                            _retractSolutionItem.Visible = true;
                            _solutionItem.Image = Resources.Images.Files.IMNON;
                        });
                    }
                    else
                    {
                        InvokeIfRequired(delegate()
                        {
                            _deploySolutionItem.Visible =
                            _deleteSolutionItem.Visible = true;
                            _solutionItem.Image = Resources.Images.Files.IMNAWAY;
                        });
                    }
                }
                else
                {
                    _addSolutionItem.Visible = true;
                    _solutionItem.Image = Resources.Images.Files.IMNBUSY;
                }

            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex);
            }
        }
    }
}
