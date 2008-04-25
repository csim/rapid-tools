using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;
using System.IO;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems;
using System.Xml;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class CustomActionForm : Form
    {
        public CustomActionForm()
        {
            InitializeComponent();
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

            CustomAction c = new CustomAction();

            c.Title = txtTitle.Text;
            c.Id = txtId.Text;
            c.Description = txtDescription.Text;
            c.GroupId = txtGroup.Text;
            c.UrlAction = txtUrl.Text;
            c.Location = txtLocation.Text;
            c.Sequence = Convert.ToInt32(txtSequence.Text);
            c.ImageUrl = txtImageUrl.Text;

            mf.Nodes.Add(c);
            mf.WriteManifest(folderPath + "\\elements.xml");
            XmlDocument d = new XmlDocument();
            d.Load(folderPath + "\\feature.xml");

            FeatureManifest man = new FeatureManifest(d);
            if (man.ElementManifests == null)
                man.ElementManifests = new List<string>();
            if (!man.ElementManifests.Contains("elements.xml"))
                man.ElementManifests.Add("elements.xml");
            man.CreateManifest(folderPath + "\\feature.xml");

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtSequence_Validating(object sender, CancelEventArgs e)
        {
            int i;
            if (int.TryParse(txtSequence.Text, out i)) e.Cancel = false;
            else
            {
                MessageBox.Show("Sequence must be an integer.");
                e.Cancel = true;
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "http://msdn2.microsoft.com/en-us/library/bb802730.aspx";
            p.Start();
        }


    }
}