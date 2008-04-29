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
			DTE2 application = AppManager.Instance.ApplicationObject;

			// TODO: change to the active project
			return application.Solution.Projects.Item(1);
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
			XmlElement xhost = (XmlElement)xconfig.SelectSingleNode("WspServerName");
			XmlElement xport = (XmlElement)xconfig.SelectSingleNode("WebApplicationPort");

			if (xhost == null)
				throw new Exception("Could not find server name in SPDeploy.user");

			if (xport == null)
				throw new Exception("Could not port name in SPDeploy.user");

			string host = xhost.Value;
			string port = xport.Value;

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
			string wdir = string.Format(@"{0}\SPDeploy\Workspace", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            return new DirectoryInfo(wdir);
        }


        public static string GetSiteUrlFromProject(DTE2 application)
        {
            string path = application.Solution.Projects.Item(1).FullName;
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
            return string.Concat("http://" + node.ChildNodes[0].InnerText);
        }

        public static void EnsurePath(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path.Remove(3));
            path = path.Substring(3);

            if (path.Contains("."))
            {
                path = path.Remove(path.LastIndexOf("\\"));
                di.CreateSubdirectory(path);
            }
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

        public static void Execute(string itemPath)
        {
            System.Diagnostics.Process _process = new System.Diagnostics.Process();
            _process.StartInfo.FileName = itemPath;
            _process.Start();
        }

        public static string GetRandomTempPath()
        {
            return Path.GetTempPath() + Path.GetRandomFileName();
        }
    }
}
