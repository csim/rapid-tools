using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Rapid.Tools.SPDeploy.AddIn.Domain.Utilties;
using System.IO;

namespace Rapid.Tools.SPDeploy.AddIn.Domain
{
    public class SolutionMenu
    {

        private ToolStripMenuItem _solutionItem;
		private ProxyBridge _bridge = new ProxyBridge();

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

        public SolutionMenu(ToolStripMenuItem solutionItem, EventHandler ev)
        {
            MachineChanged = ev;

            _solutionItem = solutionItem;
            _addSolutionItem = new ToolStripMenuItem("Add Solution");
            _deploySolutionItem = new ToolStripMenuItem("Deploy Solution");
            _retractSolutionItem = new ToolStripMenuItem("Retract Solution");
            _deleteSolutionItem = new ToolStripMenuItem("Delete Solution");
            _cycleSolutionItem = new ToolStripMenuItem("Cycle Solution");
            _upgradeSolutionItem = new ToolStripMenuItem("Upgrade Solution");
            _serverUrl = new ToolStripMenuItem(AppManager.Current.ActiveEnvironment.ServerName + ":" + AppManager.Current.ActiveEnvironment.ServerPort);

            _solutionItem.DropDownItems.Add(_serverUrl);
            _solutionItem.DropDownItems.Add(new ToolStripSeparator());
            _solutionItem.DropDownItems.Add(_retractSolutionItem);
            _solutionItem.DropDownItems.Add(_deploySolutionItem);
            _solutionItem.DropDownItems.Add(_deleteSolutionItem);
            _solutionItem.DropDownItems.Add(_addSolutionItem);
            _solutionItem.DropDownItems.Add(_cycleSolutionItem);
            _solutionItem.DropDownItems.Add(_upgradeSolutionItem);

            _serverUrl.Tag = "ServerButton";
            _serverUrl.Click += ev;
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

			string output = "";
			string projectname = AppManager.Current.ActiveProject.Name;
			string wspname = AppManager.Current.ActiveWspFileName;

            switch (action)
            {
                case Action.Deploy:
                    WriteLine("Deploying...");
					output = _bridge.AddInService.DeploySolution(wspname);
					WriteLine(output);
                    break;
                
				case Action.Retract:
					WriteLine("Retracting...");
					output = _bridge.AddInService.RetractSolution(wspname);
					WriteLine(output);
                    break;
                
				case Action.Delete:
					WriteLine("Deleting...");
					output = _bridge.AddInService.DeleteSolution(wspname);
					WriteLine(output);
                    break;
                
				case Action.Cycle:
					WriteLine("Retracting...");
					output = _bridge.AddInService.RetractSolution(wspname);
					WriteLine(output);

                    RefreshAsync();
                
					WriteLine("Deleting...");
					output = _bridge.AddInService.DeleteSolution(wspname);
					WriteLine(output);

					RefreshAsync();
                    
					CompileWsp();
                    CopyFiles();

					output = _bridge.AddInService.AddSolution(@"c:\_spdeploy\" + projectname + "\\" + wspname);
					WriteLine(output);

					RefreshAsync();
                    WriteLine("Deploying...");
					output = _bridge.AddInService.DeploySolution(wspname);
					WriteLine(output);
                    break;
                
				case Action.Add:
                    CompileWsp();
                    CopyFiles();
					output = _bridge.AddInService.AddSolution(@"c:\_spdeploy\" + projectname + "\\" + wspname);
					WriteLine(output);
                    break;
             
                case Action.Upgrade:
                    CompileWsp();
                    CopyFiles();
					_bridge.AddInService.UpgradeSolution(wspname, @"c:\_spdeploy\" + projectname + "\\" + wspname);
                    break;

                default:
                    break;
            }
            WriteLine("Completed: " + DateTime.Now);
            RefreshAsync();
        }

		private void WriteLine(string message)
		{
			RapidOutputWindow.Instance.Write(message, true);
		}

        private void CompileWsp()
        {
			WriteLine("Compiling WSP...");

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("C:\\Windows\\Microsoft.NET\\Framework\\v2.0.50727\\MSBuild.exe");
			psi.Arguments = string.Format("/target:CompileWsp \"{0}\"", AppManager.Current.ActiveProjectPath.FullName);
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = psi;

            p.Start();


            p.WaitForExit(10000);

            if (p.ExitCode != 0)
            {
				WriteLine("Compiling WSP Failed!");
                return;
            }

			WriteLine("Compiling WSP Successful");
        }

        private void CopyFiles()
        {
			WriteLine("Copying Files...");
			
			string projectname = AppManager.Current.ActiveProject.Name;
			string projectpath = AppManager.Current.ActiveProjectPath.FullName;
			string wspname = AppManager.Current.ActiveWspFileName;
			string wsppath = string.Format(@"{0}\bin\Debug\{1}", projectpath, wspname);

			byte[] wspcontents = File.ReadAllBytes(wsppath);

			_bridge.AddInService.SaveFile(string.Format(@"{0}\{1}", projectname, wsppath), wspcontents);
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

        public void RefreshAsync()
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


			Proxies.AddIn.Solution solution = null;
			Proxies.AddIn.Solution[] solutions = _bridge.AddInService.GetSols();

			solution = Array.Find<Proxies.AddIn.Solution>(solutions, delegate(Proxies.AddIn.Solution sol)
					{
						return string.Compare(sol.Name, AppManager.Current.ActiveWspFileName, true) == 0;
					});

			if (solution != null)
            {
				if (solution.Deployed)
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
