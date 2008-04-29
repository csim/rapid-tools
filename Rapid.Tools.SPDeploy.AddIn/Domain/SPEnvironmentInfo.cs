using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using EnvDTE80;
using EnvDTE;


namespace Rapid.Tools.SPDeploy.AddIn.Domain
{
	public class SPEnvironmentInfo
	{

		private string _serverName;
		private string _serverPort;

		public string ServerName
		{
			get { return _serverName; }
		}

		public string ServerPort
		{
			get { return _serverPort; }
		}


		public string WebApplicationUrl
		{
			get {
				string port = ServerPort;

				if (string.IsNullOrEmpty(port) || port == "80")
					port = "";
				else
					port = ":" + port;

				return string.Format("http://{0}{1}", ServerName, port);
			}
		}



		public static SPEnvironmentInfo Parse(Project project)
		{
			SPEnvironmentInfo ret = new SPEnvironmentInfo();

			XmlElement xconfig = GetUserConfiguration();

			XmlNamespaceManager nm = new XmlNamespaceManager(xconfig.OwnerDocument.NameTable);
			nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

			XmlElement xhost = (XmlElement)xconfig.SelectSingleNode("n:WspServerName", nm);
			XmlElement xport = (XmlElement)xconfig.SelectSingleNode("n:WebApplicationPort", nm);

			if (xhost == null)
				throw new Exception("Could not find server name in SPDeploy.user");

			if (xport == null)
				throw new Exception("Could not port name in SPDeploy.user");

			ret._serverName = xhost.InnerText;
			ret._serverPort = xport.InnerText;

			return ret;

		}

		private static XmlDocument GetConfiguration()
		{
			DirectoryInfo ppath = AppManager.Current.ActiveProjectPath.Directory;
			string configpath = string.Format(@"{0}\Properties\SPDeploy.user", ppath.FullName);

			if (!File.Exists(configpath))
				throw new FileNotFoundException("SPDeploy.user not found.");

			XmlDocument doc = new XmlDocument();
			doc.Load(configpath);

			return doc;
		}

		private static XmlElement GetUserConfiguration()
		{
			XmlDocument xdoc = GetConfiguration();

			XmlNamespaceManager nm = new XmlNamespaceManager(xdoc.NameTable);
			nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

			string username = WindowsIdentity.GetCurrent().Name.Split("\\".ToCharArray())[1];

			XmlElement xconfig = (XmlElement)xdoc.SelectSingleNode(string.Format("/n:Project/n:PropertyGroup[contains(@Condition,'{0}')]", username), nm);

			if (xconfig == null)
				throw new Exception("Could not find user configuration in SPDeploy.user");

			return xconfig;
		}

	}
}
