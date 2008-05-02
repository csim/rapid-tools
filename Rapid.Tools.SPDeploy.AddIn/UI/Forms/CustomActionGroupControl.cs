using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using Rapid.Tools.SPDeploy.AddIn.Domain;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class CustomActionGroupControl : UserControl, Rapid.Tools.SPDeploy.AddIn.UI.Forms.FormsController.IRapidControl
    {


        bool isValid = true;
        public CustomActionGroupControl()
        {
            InitializeComponent();

            if (AppManager.Current.FeatureContext == null)
            {
               BaseForm bf = new BaseForm( BaseForm.RapidFormType.Feature);
                bf.ShowDialog();

                if (AppManager.Current.FeatureContext == null)
                    isValid = false;
                    
            }           
        }

        #region IRapidControl Members        

        public void OkClicked()
        {
            FeatureManifest man = AppManager.Current.FeatureContext;

            string folderPath = man.FilePath;
            folderPath = folderPath.Remove(folderPath.LastIndexOf("\\"));


            ElementManifest mf = new ElementManifest(); ;
            if (File.Exists(folderPath + "\\elements.xml"))
            {
                XmlDocument document = new XmlDocument();
                document.Load(folderPath + "\\elements.xml");
                mf = new ElementManifest(document);
            }

            mf.WriteManifest(folderPath + "\\elements.xml");

            if (man.ElementManifests == null)
                man.ElementManifests = new List<string>();
            if (!man.ElementManifests.Contains("elements.xml"))
                man.ElementManifests.Add("elements.xml");

            man.WriteManifest();


            Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems.CustomActionGroup g = new Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems.CustomActionGroup();
            g.Id = txtId.Text;
            g.Description = txtDescription.Text;
            g.Location = txtLocation.Text;
            g.Sequence = Convert.ToInt32(txtSequence.Text);
            g.Title = txtTitle.Text;
            mf.Nodes.Add(g);

            mf.WriteManifest(folderPath + "\\elements.xml");

            AppManager.Current.FeatureContext = null;
        }

        public void CancelClicked()
        {
            

            AppManager.Current.FeatureContext = null;
        }

        #endregion


        #region IRapidControl Members


        public bool FormIsValid()
        {
            return isValid;
        }

        #endregion

        #region IRapidControl Members


        public Panel[] GetPanels()
        {
            return new Panel[] { panel1 };
        }

        #endregion

        #region IRapidControl Members


        public void AddControl(Control c)
        {
            Controls.Add(c);
        }

        public void RemoveControl(Control c)
        {
            Controls.Remove(c);
        }

        #endregion
    }
}
