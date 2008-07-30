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
		
        private string _baseUrl;

        public string BaseUrl
        {
            get { return _baseUrl; }
            set { _baseUrl = value; }
        }

		public string ServerName
		{
			get { return _serverName; }
		}

		


		public string WebApplicationUrl
		{
			get
            {
                return _baseUrl;				
			}
		}



		public static SPEnvironmentInfo Parse(Project project)
		{
			SPEnvironmentInfo ret = new SPEnvironmentInfo();

			XmlElement xconfig = GetUserConfiguration(project);

			XmlNamespaceManager nm = new XmlNamespaceManager(xconfig.OwnerDocument.NameTable);
			nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

			XmlElement xhost = (XmlElement)xconfig.SelectSingleNode("n:WspServerName", nm);
			XmlElement xport = (XmlElement)xconfig.SelectSingleNode("n:WebApplicationPort", nm);
            XmlElement xbase = (XmlElement)xconfig.SelectSingleNode("n:WebApplicationUrl", nm);

			if (xhost == null)
				throw new Exception("Could not find server name in SPDeploy.user");

			if (xport == null)
				throw new Exception("Could not port name in SPDeploy.user");

			ret._serverName = xhost.InnerText;			
            ret._baseUrl = xbase.InnerText;

			return ret;

		}

		private static XmlDocument GetConfiguration(Project project)
		{
            DirectoryInfo ppath = new FileInfo(project.FullName).Directory;
			string configpath = string.Format(@"{0}\Properties\SPDeploy.user", ppath.FullName);

			if (!File.Exists(configpath))
				throw new FileNotFoundException("SPDeploy.user not found.");

			XmlDocument doc = new XmlDocument();
			doc.Load(configpath);

			return doc;
		}

		private static XmlElement GetUserConfiguration(Project project)
		{
			XmlDocument xdoc = GetConfiguration(project);

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
