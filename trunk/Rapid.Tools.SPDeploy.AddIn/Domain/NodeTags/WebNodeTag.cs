using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using EnvDTE80;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.Proxies.AddIn;
using Rapid.Tools.SPDeploy.AddIn.Domain.Utilties;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
   
    public abstract class WebNodeTag : INodeTag
    {
        private Domain.Utilties.ApplicationUtility _applicationUtility;

        public Domain.Utilties.ApplicationUtility ApplicationUtility
        {
            get
            {
                if (_applicationUtility == null)
                    _applicationUtility = new Rapid.Tools.SPDeploy.AddIn.Domain.Utilties.ApplicationUtility(ApplicationObject);
                return _applicationUtility;
            }
            set { _applicationUtility = value; }
        }

        public NodeType TagType;

        protected TreeNode _node;

        public TreeNode Node
        {
            get { return _node; }
        }

        public string Url;
        public Guid Guid;
        public string Text;
        public DTE2 ApplicationObject;

        public Guid WebGuid
        {
            get
            {
                return getWebGuid(Node);
            }
        }

        private Guid getWebGuid(TreeNode _node)
        {
            while (_node.Tag == null || ((WebNodeTag)_node.Tag).TagType != NodeType.Web)
            {
                _node = _node.Parent;
            }
            return ((WebNodeTag)_node.Tag).Guid;
        }

        private string _siteUrl;

        public string SiteUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_siteUrl))
                    _siteUrl = ((WebNodeTag)Node.TreeView.Nodes[0].Tag).Url;
                return _siteUrl;
            }
        }

        private AddInProxy _serviceInstance;

        public AddInProxy AddInService
        {
            get
            {
                if (_serviceInstance == null)
                {
					_serviceInstance = new AddInProxy();
                    _serviceInstance.Url = string.Format("{0}/_layouts/RapidTools/Services/AddIn.asmx", EnvironmentUtil.GetWebApplicationUrl());
                    _serviceInstance.Credentials = CredentialCache.DefaultCredentials;
                }
                return _serviceInstance;
            }
        }    

        #region INodeTag Members

        public abstract ContextMenu GetContextMenu();

        public abstract void Action();

        #endregion
    }
}
