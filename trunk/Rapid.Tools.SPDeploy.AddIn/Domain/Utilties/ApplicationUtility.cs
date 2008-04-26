using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using System.IO;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.Utilties
{
    public class ApplicationUtility
    {
        private DTE2 _applicationObject;

        public DTE2 ApplicationObject
        {
            get { return _applicationObject; }           
        }

        public ApplicationUtility(DTE2 applicationObject)
        {
            _applicationObject = applicationObject;
        }

        public void DeleteAndClose(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                if (ApplicationObject.ActiveDocument != null && ApplicationObject.ActiveDocument.FullName == filePath)
                    ApplicationObject.ActiveDocument.Close(EnvDTE.vsSaveChanges.vsSaveChangesNo);
            }
        }   

        public void OpenBrowser(string url)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = url;
            process.Start();
        }

        public void OpenFile(string filePath)
        {
            ApplicationObject.ItemOperations.OpenFile(filePath, EnvDTE.Constants.vsViewKindTextView);
        }

        public string GetProjectName()
        {
            return ApplicationObject.Solution.Projects.Item(1).Name;
        }      

    }
}
