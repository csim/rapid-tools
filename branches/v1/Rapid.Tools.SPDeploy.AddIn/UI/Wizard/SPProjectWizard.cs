using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualStudio.TemplateWizard;
using EnvDTE;
using Microsoft.Win32;
using EnvDTE80;
using System.Xml;
using System.Security.Principal;
using Rapid.Tools.SPDeploy.AddIn.UI.Controls;


namespace Rapid.Tools.SPDeploy.AddIn.UI.Wizard
{
    public class SPProjectWizard : IWizard
    {

        #region IWizard Members

        void IWizard.BeforeOpeningFile(ProjectItem projectItem)
        {           
        }

        void IWizard.ProjectFinishedGenerating(Project project)
        {
            OpenUrlForm form = new OpenUrlForm();
            form.ShowDialog();
        }

        void IWizard.ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        void IWizard.RunFinished()
        {
        }

        void IWizard.RunStarted(object automationObject, System.Collections.Generic.Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {           
        }

        bool IWizard.ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        #endregion
    }
}