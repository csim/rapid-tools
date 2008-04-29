using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using System.Security.Principal;
using System.Xml;
using System.IO;
using Rapid.Tools.SPDeploy.AddIn.Proxies.Webs;
using System.Net;
using EnvDTE;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.Utilties
{
    public class EnvironmentUtil
    {


		public static Project GetProject()
		{
			return AppManager.Instance.ApplicationObject.ActiveDocument.ProjectItem.ContainingProject;
		}

		private static string GetUsername()
		{
			string username = WindowsIdentity.GetCurrent().Name.Split("\\".ToCharArray())[1];
			return username;
		}

		private static XmlDocument GetConfiguration()
		{
			DirectoryInfo ppath = GetProjectDirectory();
			string configpath = string.Format(@"{0}\Properties\SPDeploy.user", ppath.FullName);

			if (!File.Exists(configpath))
				throw new FileNotFoundException("SPDeploy.user not found.");

			XmlDocument doc = new XmlDocument();
			doc.Load(configpath);

			return doc;
		}

		public static XmlElement GetUserConfiguration()
		{
			XmlDocument xdoc = GetConfiguration();

			XmlNamespaceManager nm = new XmlNamespaceManager(xdoc.NameTable);
			nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");
			
			string username = GetUsername();

			XmlElement xconfig = (XmlElement)xdoc.SelectSingleNode(string.Format("/n:Project/n:PropertyGroup[contains(@Condition,'{0}')]", username), nm);

			if (xconfig == null)
				throw new Exception("Could not find user configuration in SPDeploy.user");

			return xconfig;
		}



		public static string GetWebApplicationUrl()
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
		
		public static DirectoryInfo GetProjectDirectory()
        {
			Project proj = GetProject();
			string path = proj.FullName;
			path = path.Remove(path.LastIndexOf("\\"));

            return new DirectoryInfo(path);
        }

        public static FileInfo[] GetFeatureFiles(DTE2 ApplicationObject)
        {
			DirectoryInfo pdir = GetProjectDirectory();
            FileInfo[] files = pdir.GetFiles("feature.xml", SearchOption.AllDirectories);
			return files;
        }

        public static DirectoryInfo GetWorkingDirectory()
        {
			Project proj = GetProject();
			string wdir = string.Format(@"{0}\SPDeploy\Workspace\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), proj.Name);

			DirectoryInfo ret = new DirectoryInfo(wdir);

			if (!Directory.Exists(ret.FullName))
				Directory.CreateDirectory(ret.FullName);

            return ret;
        }


        public static void EnsureDirectory(string filepath)
        {
			FileInfo finfo = new FileInfo(filepath);

			if (!Directory.Exists(finfo.Directory.FullName))
				Directory.CreateDirectory(finfo.Directory.FullName);
        }


        public static void OpenVsItem(string item, DTE2 ApplicationObject)
        {
            ApplicationObject.ItemOperations.OpenFile(item, Constants.vsViewKindTextView);
        }

        public static List<string> GetActivatedFeatures(string web)
        {
            WebsProxy _websProxy = new WebsProxy();
            _websProxy.Url = string.Format("{0}/_vti_bin/Webs.asmx", web);
            _websProxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            return new List<string>(_websProxy.GetActivatedFeatures().ToLower().Split(','));
        }

        public static void Execute(string filepath)
        {
            System.Diagnostics.Process _process = new System.Diagnostics.Process();
			_process.StartInfo.FileName = filepath;
            _process.Start();
        }

        public static string GetRandomTempPath()
        {
            return Path.GetTempPath() + Path.GetRandomFileName();
        }
    }
}
