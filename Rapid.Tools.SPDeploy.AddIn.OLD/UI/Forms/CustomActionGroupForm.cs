using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;
using System.Xml;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems;
using System.IO;
using System.Diagnostics;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class CustomActionGroupForm : Form
    {
        public CustomActionGroupForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            Process p = new Process();
            p.StartInfo.FileName = "http://msdn2.microsoft.com/en-us/library/bb802730.aspx";
            p.Start();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public string folderPath;
        private void btnOk_Click(object sender, EventArgs e)
        {
            ElementManifest mf = new ElementManifest(); ;
            if (File.Exists(folderPath + "\\elements.xml"))
            {
                XmlDocument document = new XmlDocument();
                document.Load(folderPath + "\\elements.xml");
                mf = new ElementManifest(document);
            }

            mf.WriteManifest(folderPath + "\\elements.xml");
            XmlDocument d = new XmlDocument();
            d.Load(folderPath + "\\feature.xml");

            FeatureManifest man = new FeatureManifest(d);
            if (man.ElementManifests == null)
                man.ElementManifests = new List<string>();
            if (!man.ElementManifests.Contains("elements.xml"))
                man.ElementManifests.Add("elements.xml");
            man.CreateManifest(folderPath + "\\feature.xml");


            CustomActionGroup g = new CustomActionGroup();
            g.Id = txtId.Text;
            g.Description = txtDescription.Text;
            g.Location = txtLocation.Text;
            g.Sequence = Convert.ToInt32(txtSequence.Text);
            g.Title = txtTitle.Text;
            mf.Nodes.Add(g);

            mf.WriteManifest(folderPath + "\\elements.xml");

            this.Close();
        }
    }
}