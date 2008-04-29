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

        public static DirectoryInfo GetProjectDirectory(DTE2 application)
        {
            string path = application.Solution.Projects.Item(1).FullName;
            path = path.Remove(path.LastIndexOf("\\"));
            return new DirectoryInfo(path);
        }

        public static FileInfo[] GetFeatureFiles(DTE2 ApplicationObject)
        {
            return Domain.Utilties.EnvironmentUtil.GetProjectDirectory(ApplicationObject).GetFiles("feature.xml", SearchOption.AllDirectories);
        }

        public static DirectoryInfo GetWorkingDirectory()
        {
            return new DirectoryInfo(GetWorkingDirectoryPath());
        }

        public static string GetWorkingDirectoryPath()
        {
           return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SPDeploy\\Workspace";
        }

        public static string GetSiteUrlFromProject()
        {
            DTE2 application = AppManager.Instance.ApplicationObject;

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
            return "http://" + node.ChildNodes[0].InnerText;
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
