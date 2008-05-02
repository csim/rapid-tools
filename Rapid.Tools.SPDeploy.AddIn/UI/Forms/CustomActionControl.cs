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
    public partial class CustomActionControl : UserControl, Rapid.Tools.SPDeploy.AddIn.UI.Forms.FormsController.IRapidControl
    {


        bool isValid = true;
        public CustomActionControl()
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

            Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems.CustomAction c = new Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems.CustomAction();

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
            
            if (man.ElementManifests == null)
                man.ElementManifests = new List<string>();
            if (!man.ElementManifests.Contains("elements.xml"))
                man.ElementManifests.Add("elements.xml");
            man.WriteManifest();


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
