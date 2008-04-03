using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;
using System.IO;
using System.Xml;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class HideCustomActionForm : Form
    {
        public HideCustomActionForm()
        {
            InitializeComponent();
        }

        public string folderPath;

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WebBrowser wb = new WebBrowser();
            wb.Url = new Uri("http://msdn2.microsoft.com/en-us/library/bb802730.aspx");
            Controls.Add(wb);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

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


            HideCustomAction h = new HideCustomAction();
            h.GroupId = txtGroup.Text;
            h.HideActionId = txtHideActionId.Text;
            h.Id = txtId.Text;
            h.Location = txtLocation.Text;
            mf.Nodes.Add(h);

            mf.WriteManifest(folderPath + "\\elements.xml");


            this.Close();
        }
    }
}