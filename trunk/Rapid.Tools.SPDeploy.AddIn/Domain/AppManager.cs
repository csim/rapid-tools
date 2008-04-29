using System;
using System.Security;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using EnvDTE;
using System.Security.Principal;
using Rapid.Tools.SPDeploy.AddIn.Proxies.Webs;
using System.Net;

namespace Rapid.Tools.SPDeploy.AddIn.Domain
{
    public class AppManager
    {
        private static AppManager _current = null;

        private DTE2 _application;

		public DTE2 Application
        {
			get { return _application; }
        }

        public AppManager(DTE2 application)
        {
			_application = application;
			ResetActiveProject();
        }


		public static AppManager Current
        {
            get { return _current; }
			set { _current = value; }
        }


		private Project _activeProject;
		private SPEnvironmentInfo _activeEnvironment = null;
		private DirectoryInfo _activeWorkspaceDirectory = null;
		private string _activeWspFileName = null;

		public Project ActiveProject
		{
			get {
				if (_activeProject == null)
					_activeProject = Application.ActiveDocument.ProjectItem.ContainingProject;

				return _activeProject; 
			}
		}

		public FileInfo ActiveProjectPath
		{
			get { return new FileInfo(ActiveProject.FullName); }
		}

		public SPEnvironmentInfo ActiveEnvironment
		{
			get {
				if (_activeEnvironment == null)
					_activeEnvironment = SPEnvironmentInfo.Parse(ActiveProject);

				return _activeEnvironment; 
			}
		}
		
		public DirectoryInfo ActiveWorkspaceDirectory
		{
			get
			{
				if (_activeWorkspaceDirectory == null)
				{
					string wdir = string.Format(@"{0}\SPDeploy\Workspace\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ActiveProject.Name);

					_activeWorkspaceDirectory = new DirectoryInfo(wdir);

					if (!Directory.Exists(_activeWorkspaceDirectory.FullName))
						Directory.CreateDirectory(_activeWorkspaceDirectory.FullName);
				}

				return _activeWorkspaceDirectory;
			}
		}

		public string ActiveWspFileName
		{
			get {
				if (string.IsNullOrEmpty(_activeWspFileName))
					_activeWspFileName = string.Format("{0}.wsp", ActiveProject.Name);

				return _activeWspFileName; 
			}
		}


		public void ResetActiveProject()
		{
			_activeProject = null;
			_activeEnvironment = null;
			_activeWorkspaceDirectory = null;
			_activeWspFileName = null;
		}


		public FileInfo[] GetFeatureFiles()
		{
			DirectoryInfo pdir = AppManager.Current.ActiveProjectPath.Directory;
			FileInfo[] files = pdir.GetFiles("feature.xml", SearchOption.AllDirectories);
			return files;
		}

		public void EnsureDirectory(string filepath)
		{
			FileInfo finfo = new FileInfo(filepath);
			EnsureDirectory(finfo);
		}

		public void EnsureDirectory(FileInfo finfo)
		{
			if (!Directory.Exists(finfo.Directory.FullName))
				Directory.CreateDirectory(finfo.Directory.FullName);
		}


		public void OpenVsItem(string item, DTE2 ApplicationObject)
		{
			ApplicationObject.ItemOperations.OpenFile(item, Constants.vsViewKindTextView);
		}

		public List<string> GetActivatedFeatures(string web)
		{
			ProxyBridge bridge = new ProxyBridge();

			string result = bridge.WebsService.GetActivatedFeatures();
			if (string.IsNullOrEmpty(result))
				return null;
			
			return new List<string>(result.ToLower().Split(','));
		}

		public void Execute(string filepath)
		{
			System.Diagnostics.Process _process = new System.Diagnostics.Process();
			_process.StartInfo.FileName = filepath;
			_process.Start();
		}

		public string GetRandomTempPath()
		{
			return Path.GetTempPath() + Path.GetRandomFileName();
		}


		public void CloseWorkspaceFile(string filePath)
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);

				if (Application.ActiveDocument != null && Application.ActiveDocument.FullName == filePath)
					Application.ActiveDocument.Close(EnvDTE.vsSaveChanges.vsSaveChangesNo);
			}
		}

		public void OpenBrowser(string url)
		{
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = url;
			process.Start();
		}

		public void OpenFile(string filePath)
		{
			Application.ItemOperations.OpenFile(filePath, EnvDTE.Constants.vsViewKindTextView);
		}


		public bool IsFileOpen(string filePath)
		{
			foreach (EnvDTE.Document doc in Application.Documents)
			{
				if (doc.FullName == filePath)
					return true;
			}

			return false;

		}

        internal void SetMachineInfo(string machineName, string port)
        {
            string path = AppManager.Current.Application.Solution.Projects.Item(1).FullName;
            path = path.Remove(path.LastIndexOf("\\"));

            XmlDocument doc = new XmlDocument();
            doc.Load(path + "\\Properties\\SPDeploy.user");

            XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
            nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

            XmlNode node = null;
            foreach (XmlNode no in doc.SelectNodes("/n:Project/n:PropertyGroup", nm))
            {
                if (no.Attributes["Condition"] != null && no.Attributes["Condition"].Value == string.Format("$(USERNAME) == '{0}'", WindowsIdentity.GetCurrent().Name.Split("\\".ToCharArray())[1]))
                    node = no;
            }
            node.ChildNodes[0].InnerText = machineName;
            node.ChildNodes[1].InnerText = "http://$(WspServerName):" + port;

            doc.Save(path + "\\Properties\\SPDeploy.user");
        }
    }
}
