using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.IsolatedStorage;
using EnvDTE;
using Microsoft.Win32;
using System.Xml;
using System.Security.Principal;
using Rapid.Tools.SPDeploy.AddIn.Domain.Utilties;
using Rapid.Tools.SPDeploy.AddIn.Domain;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Wizard
{
    public partial class CreationForm : Form
    {

        public CreationForm()
        {
            InitializeComponent();
        }

        string _machineName;
        string _portName;

        public CreationForm(string machineName, string port) : this()
        {
            _machineName = machineName;
            _portName = port;
            txtMachineName.Text = machineName;
            txtPort.Text = port;
        }

        public Project CurrentProject;


        public void writeMessage(string message)
        {
            using (TextWriter tw = new StreamWriter("c:\\log.txt"))
            {
                tw.WriteLine(DateTime.Now + "  " + message);
                tw.Flush();
                tw.Close();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_machineName))
            {
                AppManager.Current.SetMachineInfo(txtMachineName.Text, (txtPort.Text == string.Empty) ? "80" : txtPort.Text);               
            }
            else
            {
                try
                {
                    //  add a registry entry if it does not exist
                    RegistryKey _regKeyAppRoot = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\VisualStudio\8.0\MSBuild\SafeImports");
                    if (_regKeyAppRoot.GetValue("SPDeploy") == null)
                        _regKeyAppRoot.SetValue("SPDeploy", "C:\\Program Files\\MSBuild\\SPDeploy\\v1\\spdeploy.targets");


                    DirectoryInfo _projectDirectory = new DirectoryInfo(CurrentProject.FullName.Remove(CurrentProject.FullName.LastIndexOf("\\")));

                    XmlDocument _xmlDocument = new XmlDocument();
                    _xmlDocument.Load(_projectDirectory.FullName + "\\manifest.xml");

                    XmlNamespaceManager _namespaceManager = new XmlNamespaceManager(_xmlDocument.NameTable);
                    _namespaceManager.AddNamespace("n", "http://schemas.microsoft.com/sharepoint/");

                    XmlNode n = _xmlDocument.SelectSingleNode("/n:Solution", _namespaceManager);
                    n.Attributes["SolutionId"].Value = Guid.NewGuid().ToString();

                    n.InnerXml = string.Format(@"<Assemblies>
        <Assembly DeploymentTarget=""GlobalAssemblyCache"" Location=""{0}.dll"">
            <SafeControls>
                <SafeControl Namespace=""{0}.Web.UI.WebControls"" Safe=""True"" TypeName=""*"" />
            </SafeControls>
        </Assembly>
    </Assemblies>", CurrentProject.Name);

                    _xmlDocument.Save(_projectDirectory.FullName + "\\manifest.xml");

                    //  create a key for to sign the assembly
                    File.WriteAllBytes(string.Format("{0}\\{1}.snk", _projectDirectory.FullName, CurrentProject.Name), Resources.Project.Files.key);
                    CurrentProject.ProjectItems.AddFromFile(string.Format("{0}\\{1}.snk", _projectDirectory.FullName, CurrentProject.Name));

                    _xmlDocument.Load(_projectDirectory.FullName + "\\Properties\\SPDeploy.user");

                    _namespaceManager = new XmlNamespaceManager(_xmlDocument.NameTable);
                    _namespaceManager.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

                    XmlElement ele = _xmlDocument.CreateElement("PropertyGroup", _xmlDocument.NamespaceURI);
                    ele.SetAttribute("Condition", "$(USERNAME) == '" + WindowsIdentity.GetCurrent().Name.Split('\\')[1] + "'");
                    XmlElement ele2 = _xmlDocument.CreateElement("WspServerName", _xmlDocument.NamespaceURI);
                    ele2.InnerText = txtMachineName.Text;
                    ele.AppendChild(ele2);
                    ele2 = _xmlDocument.CreateElement("WebApplicationUrl", _xmlDocument.NamespaceURI);
                    ele2.InnerText = "http://$(WspServerName):" + ((txtPort.Text == "") ? "80" : txtPort.Text);
                    ele.AppendChild(ele2);
                    ele2 = _xmlDocument.CreateElement("WebApplicationOwnerEmail");
                    ele.AppendChild(ele2);


                    _xmlDocument.SelectSingleNode("/n:Project", _namespaceManager).InsertBefore(ele, _xmlDocument.SelectSingleNode("/n:Project/n:PropertyGroup[@Condition]", _namespaceManager));
                    _xmlDocument.InnerXml = _xmlDocument.InnerXml.Replace(" xmlns=\"\"", "");

                    //_xmlDocument.SelectSingleNode("/n:Project/n:PropertyGroup[@Condition]", _namespaceManager).Attributes["Condition"].Value = "$(USERNAME) == '" + WindowsIdentity.GetCurrent().Name.Split('\\')[1] + "'";
                    //_xmlDocument.SelectSingleNode("/n:Project/n:PropertyGroup[@Condition]/n:WspServerName", _namespaceManager).InnerText = txtMachineName.Text;
                    //_xmlDocument.SelectSingleNode("/n:Project/n:PropertyGroup/n:LocalDeployment", _namespaceManager).InnerText = cbLocal.Checked.ToString();
                    _xmlDocument.Save(_projectDirectory.FullName + "\\Properties\\SPDeploy.user");


                    CurrentProject.Save(_projectDirectory.FullName + "\\" + CurrentProject.Name + ".csproj");

                    _xmlDocument = new XmlDocument();
                    _xmlDocument.Load(_projectDirectory.FullName + "\\" + CurrentProject.Name + ".csproj");
                    XmlNode projectNode = _xmlDocument.SelectSingleNode("/n:Project/n:PropertyGroup", _namespaceManager);
                    XmlNode node = _xmlDocument.CreateElement("SignAssembly", "http://schemas.microsoft.com/developer/msbuild/2003");
                    node.InnerText = "true";
                    projectNode.AppendChild(node);
                    node = _xmlDocument.CreateElement("AssemblyOriginatorKeyFile", "http://schemas.microsoft.com/developer/msbuild/2003");
                    node.InnerText = CurrentProject.Name + ".snk";
                    projectNode.AppendChild(node);
                    _xmlDocument.Save(_projectDirectory.FullName + "\\" + CurrentProject.Name + ".csproj");


                }
                catch (Exception ex)
                {
                    writeMessage(ex.Message);
                }
            }
            this.Close();
        }
    }
}