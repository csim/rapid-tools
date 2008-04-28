using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class FeatureForm : Form
    {
        private bool _canceled = true;

        public bool Canceled
        {
            get { return _canceled; }
            set { _canceled = value; }
        }

        private string _fileLocation;

        public string FileLocation
        {
            get { return _fileLocation; }
            set { _fileLocation = value; }
        }

        bool showMore = false;
        public FeatureForm()
        {
            InitializeComponent();
            pictureBox2.Image = Resources.Images.Files.rapidheader;
            pictureBox3.Image = Resources.Images.Files.buttonMore;

            pictureBox3.Click += new EventHandler(pictureBox3_Click);
        }

        void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!showMore)
            {
                this.Height += panel1.Height;
                pictureBox3.Image = Resources.Images.Files.buttonLess;
            }
            else
            {
                this.Height -= panel1.Height;
                pictureBox3.Image = Resources.Images.Files.buttonMore;
            }
            showMore = !showMore;
        }

       
      

        public void GetFeatures(string projectPath)
        {
            FileLocation = projectPath.Remove(projectPath.LastIndexOf("\\"));
            FileLocation = FileLocation + "\\TemplateFiles\\Features";

            ddlScope.SelectedValue = "Web";
            txtFeatureTitle.Enabled = cbActivateOnDefault.Enabled = cbHidden.Enabled = txtImageUrl.Enabled = txtFeatureDescription.Enabled = ddlScope.Enabled = false;
            rbNewFeature.Checked = true;
            DirectoryInfo di = new DirectoryInfo(FileLocation);
            foreach (FileInfo fi in di.GetFiles("feature.xml", SearchOption.AllDirectories))
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(fi.FullName);

                XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
                nm.AddNamespace("n", "http://schemas.microsoft.com/sharepoint/");
                cbFeatureTitle.Items.Add(doc.SelectSingleNode("/n:Feature", nm).Attributes["Title"].Value);

                rbExistingFeature.Checked = true;
                cbFeatureTitle.SelectedIndex = 0;
            }
        }        


        private void btnOk_Click(object sender, EventArgs e)
        {
            if (rbExistingFeature.Checked)
            {
                DirectoryInfo di = new DirectoryInfo(FileLocation);
                foreach (FileInfo fi in di.GetFiles("feature.xml", SearchOption.AllDirectories))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(fi.FullName);

                    XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
                    nm.AddNamespace("n", "http://schemas.microsoft.com/sharepoint/");
                    if (doc.SelectSingleNode("/n:Feature", nm).Attributes["Title"].Value == cbFeatureTitle.SelectedItem.ToString())
                    {
                        FileLocation = fi.FullName;
                        break;
                    }
                }
            }
            else
            {
                ProjectFiles.FeatureManifest.FeatureManifest fm = new ProjectFiles.FeatureManifest.FeatureManifest();
                fm.Title = txtFeatureTitle.Text;
                fm.Description = txtFeatureDescription.Text;
                fm.Scope = ddlScope.SelectedItem.ToString();
                DirectoryInfo di = new DirectoryInfo(FileLocation);
                di.CreateSubdirectory(txtFeatureTitle.Text.Replace(" ", string.Empty));
                FileLocation = string.Format("{0}\\{1}\\feature.xml", FileLocation, txtFeatureTitle.Text.Replace(" ", string.Empty)); ;
                fm.Hidden = cbHidden.Checked;
                fm.ActivateOnDefault = cbActivateOnDefault.Checked;
                fm.ImageUrl = txtImageUrl.Text;
                fm.CreateManifest(FileLocation);
            }
            Canceled = false;
            this.Close();
        }

        private void rbNewFeature_CheckedChanged(object sender, EventArgs e)
        {
            txtFeatureTitle.Enabled = cbHidden.Enabled = cbActivateOnDefault.Enabled = txtImageUrl.Enabled = txtFeatureDescription.Enabled = ddlScope.Enabled = rbNewFeature.Checked;
            cbFeatureTitle.Enabled = rbExistingFeature.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Canceled = true;
            this.Close();
        }






    }
}