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
    public partial class FeatureControl : UserControl, Rapid.Tools.SPDeploy.AddIn.UI.Forms.FormsController.IRapidControl
    {


        public FeatureControl()
        {
            InitializeComponent();                  
            
            rbNewFeature.Checked = true;

            if (AppManager.Current.FeatureContext != null)
            {
                panel1.Location = panel3.Location;
                panel2.Location = new Point(panel2.Location.X, panel2.Location.Y - (panel1.Height + 20));
                Controls.Remove(panel3);

                FeatureManifest fm = AppManager.Current.FeatureContext;
                txtFeatureTitle.Text = fm.Title;
                txtFeatureDescription.Text = fm.Description;
                txtImageUrl.Text = fm.ImageUrl;
                if (fm.ActivateOnDefault.HasValue)
                    cbActivateOnDefault.Checked = fm.ActivateOnDefault.Value;
                if (fm.Hidden.HasValue)
                    cbHidden.Checked = fm.Hidden.Value;
                ddlScope.SelectedIndex = ddlScope.FindStringExact(fm.Scope);
            }

            foreach (FeatureManifest fm in AppManager.Current.FeatureManifests)
            {
                cbFeatureTitle.Items.Add(fm);
                cbFeatureTitle.DisplayMember = "Title";
                rbNewFeature.Checked = true;
            }

           
        }      


        #region IRapidControl Members

        public void OkClicked()
        {            
            if (rbExistingFeature.Checked)
            {                
                AppManager.Current.FeatureContext = cbFeatureTitle.SelectedItem as FeatureManifest;
            }
            else
            {
                DirectoryInfo di = AppManager.Current.GetFeatureDirectory();
                di = di.CreateSubdirectory(txtFeatureTitle.Text.Replace(" ", string.Empty));


                FeatureManifest fm = AppManager.Current.FeatureContext;
                if (fm == null)
                    fm = new FeatureManifest();
                fm.Title = txtFeatureTitle.Text;
                fm.Description = txtFeatureDescription.Text;
                fm.Scope = ddlScope.SelectedItem.ToString();



                fm.Hidden = cbHidden.Checked;
                fm.ActivateOnDefault = cbActivateOnDefault.Checked;
                fm.ImageUrl = txtImageUrl.Text;
                if (AppManager.Current.FeatureContext != null)
                {
                    fm.WriteManifest();
                    AppManager.Current.FeatureContext = null;
                }
                else
                {
                    fm.CreateManifest(di.FullName + "\\feature.xml");
                    AppManager.Current.FeatureContext = fm;
                }
            }
        }

        public void CancelClicked()
        {
            
        }

        

        #endregion      


        #region IRapidControl Members


        public bool FormIsValid()
        {
            return true;
        }     

        public Panel[] GetPanels()
        {
            return new Panel[] { panel1, panel2 };
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
    }
}
