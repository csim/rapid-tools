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

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class ActivationDependencyControl : UserControl, Rapid.Tools.SPDeploy.AddIn.UI.Forms.FormsController.IRapidControl
    {


        bool isValid = true;
        public ActivationDependencyControl()
        {
            InitializeComponent();

            if (AppManager.Current.FeatureContext == null)
            {
               BaseForm bf = new BaseForm( BaseForm.RapidFormType.Feature);
                bf.ShowDialog();

                if (AppManager.Current.FeatureContext == null)
                    isValid = false;
                    
            }

            foreach (FeatureManifest fm in AppManager.Current.FeatureManifests)
            {
                comboBox1.Items.Add(fm);
                comboBox1.DisplayMember = "Title";
            }
        }

        #region IRapidControl Members        

        public void OkClicked()
        {
            string id = (txtTitle.Text == string.Empty) ? ((FeatureManifest)comboBox1.SelectedItem).Id : txtTitle.Text;
            FeatureManifest fm = AppManager.Current.FeatureContext;
            fm.ActivationDependencies.Add(id);
            fm.WriteManifest();

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
