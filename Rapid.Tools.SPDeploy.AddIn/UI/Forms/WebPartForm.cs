using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using Rapid.Tools.SPDeploy.AddIn.ProjectFiles;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class WebPartForm : Form
    {
        public WebPartForm()
        {
            InitializeComponent();
        }

        public string folderPath;
        public string webPartTitle;
        public WebPartType webPartType;

        public enum WebPartType
        {
            Blank,
            Ajax
        }



        private void btnOk_Click(object sender, EventArgs e)
        {
            ElementManifest mf = new ElementManifest();
            if (File.Exists(folderPath + "\\elements.xml"))
            {
                XmlDocument document = new XmlDocument();
                document.Load(folderPath + "\\elements.xml");
                mf = new ElementManifest(document);
            }

            WebPart wp = new WebPart();
            wp.FileName = txtWebPartFileName.Text + ".webpart";
            wp.Properties["Title"] = txtWebPartTitle.Text;
            wp.Properties["Group"] = txtGroup.Text;

            mf.Nodes.Add(wp);

            mf.WriteManifest(folderPath + "\\elements.xml");
            XmlDocument d = new XmlDocument();
            d.Load(folderPath + "\\feature.xml");

            FeatureManifest man = new FeatureManifest(d);
            if (man.ElementManifests == null)
                man.ElementManifests = new List<string>();
            if (!man.ElementManifests.Contains("elements.xml"))
                man.ElementManifests.Add("elements.xml");
            man.CreateManifest(folderPath + "\\feature.xml");

            webPartTitle = txtWebPartTitle.Text;

            File.WriteAllText(string.Format("{0}\\{1}.webpart", folderPath, txtWebPartFileName.Text), Resources.Features.WebParts.Files.webpartdotwebpart.Replace("[REPLACEPROJECTNAME]", projectName).Replace("[REPLACEWEBPARTFILENAME]", txtWebPartFileName.Text).Replace("[REPLACEWEBPARTTITLE]", txtWebPartTitle.Text).Replace("[REPLACEWEBPARTDESCRIPTION]", txtWebPartDescription.Text));




            string webPartLocation = folderPath.Remove(folderPath.LastIndexOf("\\"));
            webPartLocation = webPartLocation.Remove(webPartLocation.LastIndexOf("\\"));
            webPartLocation = webPartLocation.Remove(webPartLocation.LastIndexOf("\\"));
            DirectoryInfo di = new DirectoryInfo(webPartLocation);
            di.CreateSubdirectory("Web\\UI\\WebControls");
            webPartLocation = webPartLocation + "\\Web\\UI\\WebControls\\" + txtWebPartFileName.Text + ".cs";

            switch (webPartType)
            {
                case WebPartType.Blank:                    
                    File.WriteAllText(webPartLocation, Resources.Features.WebParts.Files.webpartclass.Replace("[REPLACEPROJECTNAME]", projectName).Replace("[REPLACEWEBPARTFILENAME]", txtWebPartFileName.Text));
                    break;
                case WebPartType.Ajax:
                    man.ReceiverAssembly = projectName + ", Version=1.0.0.0, Culture=neutral, PublicKeyToken=4623235946e3a5b5";
                    man.ReceiverClass = "Ajaxification.FeatureReceiver";
                    man.CreateManifest(folderPath + "\\feature.xml");
                    File.WriteAllText(webPartLocation, Resources.Features.WebParts.Files.AjaxWebPart.Replace("[REPLACEPROJECTNAME]", projectName).Replace("[REPLACEWEBPARTFILENAME]", txtWebPartFileName.Text));
                    di = new DirectoryInfo(webPartLocation.Remove(webPartLocation.LastIndexOf("\\")));
                    di.CreateSubdirectory("WebPartResources"); webPartLocation = webPartLocation.Remove(webPartLocation.LastIndexOf("\\")) + "\\WebPartResources";

                    File.WriteAllText(webPartLocation + "\\ResourceDeclarations.cs", Resources.Features.WebParts.Files.ResourceDeclarations.Replace("[REPLACEPROJECTNAME]", projectName));
                    File.WriteAllText(webPartLocation + "\\HelloScript.js", Resources.Features.WebParts.Files.HelloScript.Replace("[REPLACEPROJECTNAME]", projectName));
                    folderPath = folderPath.Remove(folderPath.LastIndexOf("\\"));
                    folderPath = folderPath.Remove(folderPath.LastIndexOf("\\"));
                    di = new DirectoryInfo(folderPath.Remove(folderPath.LastIndexOf("\\")));
                    di.CreateSubdirectory("ISAPI\\" + projectName);
                    folderPath = folderPath.Remove(folderPath.LastIndexOf("\\")) + "\\ISAPI\\" + projectName;
                    File.WriteAllText(folderPath + "\\HelloWebService.asmx", Resources.Features.WebParts.Files.HelloWebService.Replace("[REPLACEPROJECTNAME]", projectName));
                    File.WriteAllText(folderPath + "\\HelloWebService.cs", Resources.Features.WebParts.Files.HelloWebService1.Replace("[REPLACEPROJECTNAME]", projectName).Replace("[REPLACEMACHINENAME]", machineName));

                    string otherpath = folderPath.Remove(folderPath.LastIndexOf("\\"));

                    di = new DirectoryInfo(di.FullName + "\\Receivers");
                    File.WriteAllText(di.FullName + "\\Ajaxification.cs", Resources.Features.WebParts.Files.Ajaxification);    
        
                    break;
                default:
                    break;
            }





            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public string projectName;
        public string machineName;

        private void btnCancel_Click_1(object sender, EventArgs e)
        {            
            this.Close();
        }
    }
}