using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using System.Xml;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class FeatureReceiverForm : Form
    {
        public FeatureReceiverForm()
        {
            InitializeComponent();        
        }

        public delegate void createMethod(string featureFolder, string featureTitle, string featureDescription, string webPartTitle, string webPartDescription);

        public string featureFolderPath;
        public string projectName;
        private void btnOk_Click(object sender, EventArgs e)
        {
            string folder = featureFolderPath.Remove(featureFolderPath.LastIndexOf("\\"));
            folder = folder.Remove(folder.LastIndexOf("\\"));
            folder = folder.Remove(folder.LastIndexOf("\\")) + "\\Receivers";

            File.WriteAllText(string.Format("{0}\\{1}.cs", folder, txtWebPartFileName.Text), Resources.Features.FeatureReceivers.Files.FeatureReceiver.Replace("[REPLACEPROJECTNAME]", projectName).Replace("[REPLACECLASSNAME]", txtWebPartFileName.Text));
            XmlDocument doc = new XmlDocument();
            doc.Load(featureFolderPath + "\\feature.xml");

            FeatureManifest f = new FeatureManifest(doc);
            f.ReceiverAssembly = projectName + ", Version=1.0.0.0, Culture=neutral, PublicKeyToken=4623235946e3a5b5";
            f.ReceiverClass = string.Format("{0}.Receivers.{1}", projectName, txtWebPartFileName.Text);
            f.CreateManifest(featureFolderPath + "\\feature.xml");
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
      
    }
}