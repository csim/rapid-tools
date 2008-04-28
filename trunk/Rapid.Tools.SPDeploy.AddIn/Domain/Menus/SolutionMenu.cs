using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Rapid.Tools.SPDeploy.AddIn.Domain.Utilties;
using System.IO;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.Menus
{
    public class SolutionMenu
    {

        private ToolStripMenuItem _solutionItem;

        public ToolStripMenuItem SolutionItem
        {
            get { return _solutionItem; }
            set { _solutionItem = value; }
        }

        private ToolStripMenuItem _addSolutionItem;
        private ToolStripMenuItem _deploySolutionItem;
        private ToolStripMenuItem _retractSolutionItem;
        private ToolStripMenuItem _deleteSolutionItem;
        private ToolStripMenuItem _cycleSolutionItem;
        private ToolStripMenuItem _upgradeSolutionItem;

        public SolutionMenu(ToolStripMenuItem solutionItem)
        {
            _solutionItem = solutionItem;
            _addSolutionItem = new ToolStripMenuItem("Add Solution");
            _deploySolutionItem = new ToolStripMenuItem("Deploy Solution");
            _retractSolutionItem = new ToolStripMenuItem("Retract Solution");
            _deleteSolutionItem = new ToolStripMenuItem("Delete Solution");
            _cycleSolutionItem = new ToolStripMenuItem("Cycle Solution");
            _upgradeSolutionItem = new ToolStripMenuItem("Upgrade Solution");

            _solutionItem.DropDownItems.Add(_retractSolutionItem);
            _solutionItem.DropDownItems.Add(_deploySolutionItem);
            _solutionItem.DropDownItems.Add(_deleteSolutionItem);
            _solutionItem.DropDownItems.Add(_addSolutionItem);
            _solutionItem.DropDownItems.Add(_cycleSolutionItem);
            _solutionItem.DropDownItems.Add(_upgradeSolutionItem);



            _retractSolutionItem.Click += new EventHandler(_retractSolutionItem_Click);
            _deploySolutionItem.Click += new EventHandler(_deploySolutionItem_Click);
            _deleteSolutionItem.Click += new EventHandler(_deleteSolutionItem_Click);
            _addSolutionItem.Click += new EventHandler(_addSolutionItem_Click);
            _cycleSolutionItem.Click += new EventHandler(_cycleSolutionItem_Click);
            _upgradeSolutionItem.Click += new EventHandler(_upgradeSolutionItem_Click);

            hideMenuItems();
        }

        void _upgradeSolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Upgrade, null, null);
        }

        void _cycleSolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Cycle, null, null);
        }

        void _addSolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Add, null, null);
        }

        void _deleteSolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Delete, null, null);
        }

        void _deploySolutionItem_Click(object sender, EventArgs e)
        {
            PerformActionDelegate del = new PerformActionDelegate(PerformAction);
            del.BeginInvoke(Action.Deploy, null, null);
        }

        public delegate void PerformActionDelegate(Action action);

        public void PerformAction(Action action)
        {
            RapidOutputWindow.Instance.Activate();
            RapidOutputWindow.Instance.Clear();
            switch (action)
            {
                case Action.Deploy:
                    RapidOutputWindow.Instance.Write("Deploying...", true);
                    RapidOutputWindow.Instance.Write(ServiceManager.Instance.ServiceInstance.DeploySolution(AppManager.Instance.WspFileName), true);
                    break;
                case Action.Retract:
                    RapidOutputWindow.Instance.Write("Retracting...", true);
                    RapidOutputWindow.Instance.Write(ServiceManager.Instance.ServiceInstance.RetractSolution(AppManager.Instance.WspFileName), true);
                    break;
                case Action.Delete:
                    RapidOutputWindow.Instance.Write("Deleting...", true);
                    RapidOutputWindow.Instance.Write(ServiceManager.Instance.ServiceInstance.DeleteSolution(AppManager.Instance.WspFileName), true);
                    break;
                case Action.Cycle:
                    RapidOutputWindow.Instance.Write("Retracting...", true);
                    RapidOutputWindow.Instance.Write(ServiceManager.Instance.ServiceInstance.RetractSolution(AppManager.Instance.WspFileName), true);
                    RefreshMenuItemsAsync();
                    RapidOutputWindow.Instance.Write("Deleting...", true);
                    RapidOutputWindow.Instance.Write(ServiceManager.Instance.ServiceInstance.DeleteSolution(AppManager.Instance.WspFileName), true);
                    RefreshMenuItemsAsync();
                    CompileWsp();
                    CopyFiles();
                    RapidOutputWindow.Instance.Write(ServiceManager.Instance.ServiceInstance.AddSolution(@"c:\_spdeploy\" + AppManager.Instance.ProjectName + "\\" + AppManager.Instance.WspFileName), true);
                    RefreshMenuItemsAsync();
                    RapidOutputWindow.Instance.Write("Deploying...", true);
                    RapidOutputWindow.Instance.Write(ServiceManager.Instance.ServiceInstance.DeploySolution(AppManager.Instance.WspFileName), true);                    
                    break;
                case Action.Add:
                    CompileWsp();
                    CopyFiles();
                    RapidOutputWindow.Instance.Write(ServiceManager.Instance.ServiceInstance.AddSolution(@"c:\_spdeploy\" + AppManager.Instance.ProjectName + "\\" + AppManager.Instance.WspFileName), true);
                    break;                
                case Action.Upgrade:
                    CompileWsp();
                    CopyFiles();
                    RapidOutputWindow.Instance.Write(ServiceManager.Instance.ServiceInstance.UpgradeSolution(AppManager.Instance.WspFileName, @"c:\_spdeploy\" + AppManager.Instance.ProjectName + "\\" + AppManager.Instance.WspFileName), true);
                    break;
                default:
                    break;
            }
            RapidOutputWindow.Instance.Write("Completed: " + DateTime.Now);
            RefreshMenuItemsAsync();
        }

        private void CompileWsp()
        {
            RapidOutputWindow.Instance.Write("Compiling WSP...", true);

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("C:\\Windows\\Microsoft.NET\\Framework\\v2.0.50727\\MSBuild.exe");
            psi.Arguments = "/target:CompileWsp " + AppManager.Instance.ProjectPath;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;

            p.Start();


            p.WaitForExit(10000);

            if (p.ExitCode != 0)
            {
                RapidOutputWindow.Instance.Write("Compiling WSP Failed!", true);
                return;
            }

            RapidOutputWindow.Instance.Write("Compiling WSP Successful", true);
        }

        private void CopyFiles()
        {                        
            RapidOutputWindow.Instance.Write("Copying Files...", true);
            string wspPath = AppManager.Instance.ProjectPath;
            wspPath = wspPath.Remove(wspPath.LastIndexOf("\\"));
            wspPath = wspPath + "\\obj\\Debug\\" + AppManager.Instance.WspFileName;
            ServiceManager.Instance.ServiceInstance.SaveFile(AppManager.Instance.ProjectName + "\\" + AppManager.Instance.WspFileName, File.ReadAllBytes(wspPath));            
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

        void _retractSolutionItem_Click(object sender, EventArgs e)
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

        private delegate void VoidDelegate();

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

        public void RefreshMenuItemsAsync()
        {
            if (_solutionItem.Owner.InvokeRequired)
            {
                _solutionItem.Owner.Invoke(new VoidDelegate(RefreshMenuItems));
            }
            else
            {
                RefreshMenuItems();
            }
        }

        public void RefreshMenuItems()
        {
            hideMenuItems();


            SPToolsWebService.Solution solItem = null;
            if ((solItem = Array.Find<SPToolsWebService.Solution>(ServiceManager.Instance.ServiceInstance.GetSols(), delegate(SPToolsWebService.Solution sol)
            {
                return string.Compare(sol.Name, AppManager.Instance.WspFileName, true) == 0;
            })) != null)
            {
                if (solItem.Deployed)
                {
                    _upgradeSolutionItem.Visible =
                    _cycleSolutionItem.Visible =
                    _retractSolutionItem.Visible = true;                    
                    _solutionItem.Image = Resources.Images.Files.IMNON;
                }
                else
                {
                    _deploySolutionItem.Visible =
                    _deleteSolutionItem.Visible = true;
                    _solutionItem.Image = Resources.Images.Files.IMNAWAY;
                }
            }
            else
            {
                _addSolutionItem.Visible = true;
                _solutionItem.Image = Resources.Images.Files.IMNBUSY;
            }
        }

    }
}
