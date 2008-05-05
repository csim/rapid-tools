using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;
using Rapid.Tools.SPDeploy.AddIn.Domain;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class WebPartControl : UserControl, Rapid.Tools.SPDeploy.AddIn.UI.Forms.IRapidControl
    {


        public WebPartControl()
        {
            InitializeComponent();

            if (AppManager.Current.FeatureContext == null)
            {
                BaseForm bf = new BaseForm(BaseForm.RapidFormType.Feature);
                bf.ShowDialog();   
                if (AppManager.Current.FeatureContext == null)
                    FindForm().Close();
            }

            this.Paint += new PaintEventHandler(WebPartControl_Paint);           
            
        }

        void WebPartControl_Paint(object sender, PaintEventArgs e)
        {
            txtWebPartTitle.Focus();
        }

       

        #region IRapidControl Members

        public void OkClicked()
        {
            ElementManifest mf = new ElementManifest();
            string folderPath = AppManager.Current.FeatureContext.FilePath;
            
            folderPath = folderPath.Remove(folderPath.LastIndexOf("\\"));

            if (File.Exists(folderPath + "\\elements.xml"))
            {
                XmlDocument document = new XmlDocument();
                document.Load(folderPath + "\\elements.xml");
                mf = new ElementManifest(document);
            }

            Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems.WebPart wp = new Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems.WebPart();
            wp.FileName = txtWebPartFileName.Text + ".webpart";
            wp.Properties["Title"] = txtWebPartTitle.Text;
            wp.Properties["Group"] = txtGroup.Text;

            mf.Nodes.Add(wp);

            mf.WriteManifest(folderPath + "\\elements.xml");






            FeatureManifest man = AppManager.Current.FeatureContext;
            if (man.ElementManifests == null)
                man.ElementManifests = new List<string>();
            if (!man.ElementManifests.Contains("elements.xml"))
                man.ElementManifests.Add("elements.xml");
            man.WriteManifest();

            string webPartTitle = txtWebPartTitle.Text;


            File.WriteAllText(string.Format("{0}\\{1}.webpart", folderPath, txtWebPartFileName.Text), Resources.Features.WebParts.Files.webpartdotwebpart.Replace("[REPLACEPROJECTNAME]", AppManager.Current.ActiveProject.Name).Replace("[REPLACEWEBPARTFILENAME]", txtWebPartFileName.Text).Replace("[REPLACEWEBPARTTITLE]", txtWebPartTitle.Text).Replace("[REPLACEWEBPARTDESCRIPTION]", txtWebPartDescription.Text));
            
            string webPartLocation = folderPath.Remove(folderPath.LastIndexOf("\\"));
            webPartLocation = webPartLocation.Remove(webPartLocation.LastIndexOf("\\"));
            webPartLocation = webPartLocation.Remove(webPartLocation.LastIndexOf("\\"));
            DirectoryInfo di = new DirectoryInfo(webPartLocation);
            di.CreateSubdirectory("Web\\UI\\WebControls");
            webPartLocation = webPartLocation + "\\Web\\UI\\WebControls\\" + txtWebPartFileName.Text + ".cs";
            File.WriteAllText(webPartLocation, Resources.Features.WebParts.Files.webpartclass.Replace("[REPLACEPROJECTNAME]", AppManager.Current.ActiveProject.Name).Replace("[REPLACEWEBPARTFILENAME]", txtWebPartFileName.Text));
            AppManager.Current.OpenFile(webPartLocation);
            AppManager.Current.EnsureProjectFilesAdded(webPartLocation);

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
    }
}
