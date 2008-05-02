using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{    
    public partial class ActivationDependencyForm : Form
    {

        public ActivationDependencyForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            XmlDocument d = new XmlDocument();
            d.Load(folderPath + "\\feature.xml");
            FeatureManifest fm = new FeatureManifest(d);     



            if (comboBox1.SelectedItem != null)
            {
                DirectoryInfo di = new DirectoryInfo(folderPath.Remove(folderPath.LastIndexOf("\\")));
                foreach (FileInfo fi in di.GetFiles("feature.xml", SearchOption.AllDirectories))
                {
                    if (fi.FullName == folderPath + "\\feature.xml") continue;
                    XmlDocument doc = new XmlDocument();
                    doc.Load(fi.FullName);

                    XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
                    nm.AddNamespace("n", "http://schemas.microsoft.com/sharepoint/");

                    if (doc.SelectSingleNode("/n:Feature", nm).Attributes["Title"].Value == comboBox1.SelectedItem.ToString())
                    {
                        if (fm.ActivationDependencies == null) fm.ActivationDependencies = new List<string>();
                        fm.ActivationDependencies.Add(doc.SelectSingleNode("/n:Feature", nm).Attributes["Id"].Value);
                        break;
                    }
                }
            }
            if (txtTitle.Text != string.Empty)
                fm.ActivationDependencies.Add(txtTitle.Text);

            fm.CreateManifest(folderPath + "\\feature.xml");

            this.Close();
        }
        public string folderPath;

        public void GetFeatures()
        {
            DirectoryInfo di = new DirectoryInfo(folderPath.Remove(folderPath.LastIndexOf("\\")));
            foreach (FileInfo fi in di.GetFiles("feature.xml", SearchOption.AllDirectories))
            {
                if (fi.FullName == folderPath + "\\feature.xml") continue;
                XmlDocument doc = new XmlDocument();
                doc.Load(fi.FullName);

                XmlNamespaceManager nm = new XmlNamespaceManager(doc.NameTable);
                nm.AddNamespace("n", "http://schemas.microsoft.com/sharepoint/");

                comboBox1.Items.Add(doc.SelectSingleNode("/n:Feature", nm).Attributes["Title"].Value);
                comboBox1.SelectedIndex = 0;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
