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

using Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;

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

        public Project ActiveProject
        {
            get
            {
                if (_activeProject == null)
                    _activeProject = Application.ActiveDocument.ProjectItem.ContainingProject;

                return _activeProject;
            }
        }

        public FileInfo ActiveProjectPath
        {
            get { return new FileInfo(ActiveProject.FullName); }
        }


        public void ResetActiveProject()
        {
            _activeProject = null;
            _activeEnvironment = null;
        }




        public FileInfo[] GetFeatureFiles()
        {
            DirectoryInfo pdir = ActiveProjectPath.Directory;
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
            //string result = ActiveBridge.WebsService.GetActivatedFeatures();
            //if (string.IsNullOrEmpty(result))
            //    return null;

            //return new List<string>(result.ToLower().Split(','));
            return null;
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


        public void OpenBrowser(string url)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = url;
            process.Start();
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
            string path = ActiveProject.FullName;
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



        public void EnsureProjectFilesAdded(string folderPath)
        {
            if (folderPath.Contains("."))
                folderPath = folderPath.Remove(folderPath.LastIndexOf("\\"));
            foreach (FileInfo fi in new DirectoryInfo(folderPath).GetFiles("*", SearchOption.AllDirectories))
            {
                ActiveProject.ProjectItems.AddFromFile(fi.FullName);
            }
        }


        public void ExecuteMSBuild(string target)
        {
            try
            {
                RapidOutputWindow.Instance.Activate();
                RapidOutputWindow.Instance.Clear();

                string command = @"/target:" + target + " \"" + AppManager.Current.ActiveProject.FullName;

                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(@"C:\Windows\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe");
                psi.Arguments = command;
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;

                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo = psi;
                p.Start();

                string line = string.Empty;

				do
				{
					line = p.StandardOutput.ReadLine();
					if (!string.IsNullOrEmpty(line))
					{
						RapidOutputWindow.Instance.Write(line + "\r\n");
					}
				} while (!p.StandardOutput.EndOfStream);
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
		}


        public string GetRootNamespace()
        {
            XmlDocument d = new XmlDocument();
            d.Load(ActiveProject.FullName);

            XmlNamespaceManager nm = new XmlNamespaceManager(d.NameTable);
            nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

            return d.SelectSingleNode("/n:Project/n:PropertyGroup/n:RootNamespace", nm).InnerText;
        }


        //public string GetFeatureFolder()
        //{
        //    UI.Forms.FeatureForm featureForm = new UI.Forms.FeatureForm();
        //    featureForm.GetFeatures(ActiveProject.FullName);
        //    featureForm.ShowDialog();

        //    if (featureForm.Canceled)
        //        return string.Empty;

        //    return featureForm.FileLocation.Remove(featureForm.FileLocation.LastIndexOf("\\"));
        //}

        public void OpenFile(string filePath)
        {
            Application.ItemOperations.OpenFile(filePath, EnvDTE.Constants.vsViewKindTextView);
        }

        public void CloseFile(string filePath)
        {
            CloseFile(filePath, EnvDTE.vsSaveChanges.vsSaveChangesPrompt);
        }

        public void CloseFile(string filePath, EnvDTE.vsSaveChanges promptType)
        {
            if (File.Exists(filePath))
            {
                Documents docs = AppManager.Current.Application.Documents;

                for (int i = 1; i <= docs.Count; i++)
                {
                    if (docs.Item(i).FullName.ToLower() == filePath.ToLower())
                        docs.Item(i).Close(promptType);
                }

                File.Delete(filePath);
            }
        }


        public void Write(Exception ex)
        {
            string message = ExceptionUtil.Format(ex);
            Write(message, true);
        }

        public void WriteLine(string message)
        {
            Write(message, true);
        }

        public void Write(string message)
        {
            Write(message, false);
        }

        public void Write(string message, bool newLine)
        {
            RapidOutputWindow.Instance.Write(message, newLine);
        }

        public IEnumerable<FeatureManifest> FeatureManifests
        {
            get
            {

                foreach (FileInfo fi in GetFeatureDirectory().GetFiles("feature.xml", SearchOption.AllDirectories))
                {
                    FeatureManifest fm = new FeatureManifest(File.ReadAllText(fi.FullName));
                    fm.FilePath = fi.FullName;
                    yield return fm;
                }
            }
        }

        public DirectoryInfo GetFeatureDirectory()
        {
            string path = this.ActiveProjectPath.FullName;
            path = path.Remove(path.LastIndexOf("\\"));
            path += "\\TemplateFiles\\Features";

            return new DirectoryInfo(path);
        }

        private FeatureManifest _featureContext;

        public FeatureManifest FeatureContext
        {
            get { return _featureContext; }
            set { _featureContext = value; }
        }







    }


}
