using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using System.IO;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class FeatureModificationForm : Form
    {
        bool showMore = false;
        FeatureManifest featureManifest = null;
        string _filePath;

        public FeatureModificationForm(string filePath)
        {
            InitializeComponent();

            pictureBox2.Image = Resources.Images.Files.rapidheader;
            pictureBox3.Image = Resources.Images.Files.buttonMore;

            pictureBox3.Click += new EventHandler(pictureBox3_Click);

            _filePath = filePath;
            this.featureManifest = new FeatureManifest(File.ReadAllText(filePath));

            txtTitle.Text = featureManifest.Title;
            txtDescription.Text = featureManifest.Description;
            ddlScope.SelectedIndex = ddlScope.FindStringExact(featureManifest.Scope);
            if (featureManifest.Hidden.HasValue)
                cbHidden.Checked = featureManifest.Hidden.Value;
            

        }

        void pictureBox3_Click(object sender, EventArgs e)
        {
            if (!showMore)
            {
                this.Height = this.Height + panel2.Height;
                pictureBox3.Image = Resources.Images.Files.buttonLess;
            }
            else
            {
                this.Height = this.Height - panel2.Height;
                pictureBox3.Image = Resources.Images.Files.buttonMore;
            }

            showMore = !showMore;

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            featureManifest.Title = txtTitle.Text;
            featureManifest.Description = txtDescription.Text;
            featureManifest.Scope = ddlScope.SelectedItem.ToString();
            featureManifest.Hidden = cbHidden.Checked;
            featureManifest.ActivateOnDefault = cbActivateOnDefault.Checked;
            if (txtImageUrl.Text != string.Empty)
                featureManifest.ImageUrl = txtImageUrl.Text;
            featureManifest.CreateManifest(_filePath);
            this.Close();
        }
    }
}