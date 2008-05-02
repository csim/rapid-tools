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
    public partial class FeatureReceiverControl : UserControl, Rapid.Tools.SPDeploy.AddIn.UI.Forms.FormsController.IRapidControl
    {


        bool isValid = true;
        public FeatureReceiverControl()
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

            string folder = folderPath.Remove(folderPath.LastIndexOf("\\"));
            folder = folder.Remove(folder.LastIndexOf("\\"));
            folder = folder.Remove(folder.LastIndexOf("\\")) + "\\Receivers";

            File.WriteAllText(string.Format("{0}\\{1}.cs", folder, txtWebPartFileName.Text), Resources.Features.FeatureReceivers.Files.FeatureReceiver.Replace("[REPLACEPROJECTNAME]", AppManager.Current.ActiveProject.Name).Replace("[REPLACECLASSNAME]", txtWebPartFileName.Text));
            AppManager.Current.EnsureProjectFilesAdded(folder);

            man.ReceiverAssembly = AppManager.Current.ActiveProject.Name + ", Version=1.0.0.0, Culture=neutral, PublicKeyToken=4623235946e3a5b5";
            man.ReceiverClass = string.Format("{0}.Receivers.{1}", AppManager.Current.ActiveProject.Name, txtWebPartFileName.Text);
            
            man.WriteManifest();

            AppManager.Current.EnsureProjectFilesAdded(AppManager.Current.GetFeatureDirectory().FullName);


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
