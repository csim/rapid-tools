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
using System.Text.RegularExpressions;
using Rapid.Tools.SPDeploy.AddIn.UI.Forms.FormControls;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class ActivationDependencyControl : UserControl, Rapid.Tools.SPDeploy.AddIn.UI.Forms.IRapidControl
    {
        public ActivationDependencyControl()
        {
            InitializeComponent();


            if (AppManager.Current.FeatureContext == null)
            {
                BaseForm bf = new BaseForm(BaseForm.RapidFormType.Feature);
                bf.ShowDialog();

                if (AppManager.Current.FeatureContext == null)
                    FindForm().Close();

            }

            foreach (FeatureManifest fm in AppManager.Current.FeatureManifests)
            {
                if (AppManager.Current.FeatureContext == null || AppManager.Current.FeatureContext.Id != fm.Id)
                {
                    ddlFeatureId.Items.Add(fm);
                    ddlFeatureId.DisplayMember = "Title";
                }
            }

            rbInternal.Checked = (ddlFeatureId.Items.Count > 0);
            rbExternal.Checked = !rbInternal.Checked;
            rbInternal_CheckedChanged(this, EventArgs.Empty);

            rbInternal.CheckedChanged += new EventHandler(rbInternal_CheckedChanged);

            this.Paint += new PaintEventHandler(ActivationDependencyControl_Paint);
        }

        void rbInternal_CheckedChanged(object sender, EventArgs e)
        {
            txtFeatureId.Enabled = rbExternal.Checked;
            ddlFeatureId.Enabled = rbInternal.Checked;
            
            if (rbExternal.Checked)
                txtTitle_Validating(this, System.ComponentModel.CancelEventArgs.Empty as CancelEventArgs);
            else
                errorProvider1.Clear();
        }


        bool loaded = false;

        void ActivationDependencyControl_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded)
            {
                txtFeatureId.Focus();
            }
            loaded = true;
        }

        #region IRapidControl Members

        public void OkClicked()
        {
            string id = (txtFeatureId.Text == string.Empty) ? ((FeatureManifest)ddlFeatureId.SelectedItem).Id : txtFeatureId.Text;
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
            Validator v = new Validator();
            if (rbExternal.Checked)
            {
                if (!v.IsValidGuid(txtFeatureId.Text) || txtFeatureId.Text.Length < 1)
                    return false;
            }

            return true;
        }

        public Panel[] GetPanels()
        {
            return new Panel[] { panel1 };
        }

        public void AddControl(Control c)
        {
            Controls.Add(c);
        }

        public void RemoveControl(Control c)
        {
            Controls.Remove(c);
        }

        #endregion


        private void txtTitle_Validating(object sender, CancelEventArgs e)
        {
            if (rbExternal.Checked)
            {
                Validator v = new Validator();
                if (!v.IsValidGuid(txtFeatureId.Text) || txtFeatureId.Text.Length < 1)
                {
                    errorProvider1.SetError(txtFeatureId, "Please enter a valid guid.");
                    return;
                }
            }              
        }
    }    
}
