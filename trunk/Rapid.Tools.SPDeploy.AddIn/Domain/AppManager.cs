using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;

namespace Rapid.Tools.SPDeploy.AddIn.Domain
{
    public class AppManager
    {
        private static readonly AppManager instance = new AppManager();

        private DTE2 _applicationObject;

        public DTE2 ApplicationObject
        {
            get { return _applicationObject; }
            set { _applicationObject = value; }
        }

        private AppManager()
        {
        }

        public static AppManager Instance
        {
            get
            {
                return instance;
            }
        }

        private string _projectName;

        public string ProjectName
        {
            get
            {
                if (string.IsNullOrEmpty(_projectName))
                    _projectName = GetProjectName();
                return _projectName;
            }
        }

        private string _wspFileName;

        public string WspFileName
        {
            get
            {
                if (_wspFileName == null)
                    _wspFileName = GetWspFileName();
                return _wspFileName;
            }

        }

        private string _projectPath;

        public string ProjectPath
        {
            get
            {
                if (string.IsNullOrEmpty(_projectPath))
                    _projectPath = GetProjectPath();
                return _projectPath;
            }
            set { _projectPath = value; }
        }

        private string GetProjectPath()
        {
            return instance.ApplicationObject.Solution.Projects.Item(1).FullName;
        }

        private string GetProjectName()
        {
            return instance.ApplicationObject.Solution.Projects.Item(1).Name;
        }

        private string GetWspFileName()
        {
            return GetProjectName() + ".wsp";
        }

    }
}
