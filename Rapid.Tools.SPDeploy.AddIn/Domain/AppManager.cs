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
        private static readonly AppManager instance = new AppManager();

        private DTE2 _applicationObject;

        public DTE2 ApplicationObject
        {
            get { return _applicationObject; }
            set { _applicationObject = value; }
        }

        private AppManager()
        {
        }

        public static AppManager Instance
        {
            get
            {
                return instance;
            }
        }



		public Project GetProject()
		{
			return ApplicationObject.ActiveDocument.ProjectItem.ContainingProject;
		}

		public string GetProjectPath()
        {
			Project proj = GetProject();
			return proj.FullName;
        }

		public string GetProjectName()
        {
			Project proj = GetProject();
            return proj.Name;
        }

        public string GetWspFileName()
        {
            return GetProjectName() + ".wsp";
        }


		public DirectoryInfo GetProjectDirectory()
		{
			string path = GetProjectPath();
			return new DirectoryInfo(Path.GetDirectoryName(path));
		}

		public DirectoryInfo GetWorkspaceDirectory()
		{
			Project proj = GetProject();
			string wdir = string.Format(@"{0}\SPDeploy\Workspace\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), proj.Name);

			DirectoryInfo ret = new DirectoryInfo(wdir);

			if (!Directory.Exists(ret.FullName))
				Directory.CreateDirectory(ret.FullName);

			return ret;
		}


		private string GetUserName()
		{
			string username = WindowsIdentity.GetCurrent().Name.Split("\\".ToCharArray())[1];
			return username;
		}

		private XmlDocument GetConfiguration()
		{
			DirectoryInfo ppath = AppManager.Instance.GetProjectDirectory();
			string configpath = string.Format(@"{0}\Properties\SPDeploy.user", ppath.FullName);

			if (!File.Exists(configpath))
				throw new FileNotFoundException("SPDeploy.user not found.");

			XmlDocument doc = new XmlDocument();
			doc.Load(configpath);

			return doc;
		}

		public XmlElement GetUserConfiguration()
		{
			XmlDocument xdoc = GetConfiguration();

			XmlNamespaceManager nm = new XmlNamespaceManager(xdoc.NameTable);
			nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

			string username = GetUserName();

			XmlElement xconfig = (XmlElement)xdoc.SelectSingleNode(string.Format("/n:Project/n:PropertyGroup[contains(@Condition,'{0}')]", username), nm);

			if (xconfig == null)
				throw new Exception("Could not find user configuration in SPDeploy.user");

			return xconfig;
		}

        public string GetMachineName()
        {
            return GetUserConfiguration().ChildNodes[0].InnerText;
        }

        public string GetPort()
        {
            string port = GetUserConfiguration().ChildNodes[1].InnerText;
            return port.Substring(port.LastIndexOf(":") + 1);
        }

		public string GetWebApplicationUrl()
		{

			XmlElement xconfig = GetUserConfiguration();

			XmlNamespaceManager nm = new XmlNamespaceManager(xconfig.OwnerDocument.NameTable);
			nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

			XmlElement xhost = (XmlElement)xconfig.SelectSingleNode("n:WspServerName", nm);
			XmlElement xport = (XmlElement)xconfig.SelectSingleNode("n:WebApplicationPort", nm);

			if (xhost == null)
				throw new Exception("Could not find server name in SPDeploy.user");

			if (xport == null)
				throw new Exception("Could not port name in SPDeploy.user");

			string host = xhost.InnerText;
			string port = xport.InnerText;

			if (string.IsNullOrEmpty(port) || port == "80")
				port = "";
			else
				port = ":" + port;

			return string.Format("http://{0}{1}", host, port);

		}

		public FileInfo[] GetFeatureFiles()
		{
			DirectoryInfo pdir = AppManager.Instance.GetProjectDirectory();
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

				if (ApplicationObject.ActiveDocument != null && ApplicationObject.ActiveDocument.FullName == filePath)
					ApplicationObject.ActiveDocument.Close(EnvDTE.vsSaveChanges.vsSaveChangesNo);
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
			ApplicationObject.ItemOperations.OpenFile(filePath, EnvDTE.Constants.vsViewKindTextView);
		}


		public bool IsFileOpen(string filePath)
		{
			foreach (EnvDTE.Document doc in ApplicationObject.Documents)
			{
				if (doc.FullName == filePath)
					return true;
			}

			return false;

		}

        internal void SetMachineInfo(string machineName, string port)
        {
            string path = AppManager.Instance.ApplicationObject.Solution.Projects.Item(1).FullName;
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
