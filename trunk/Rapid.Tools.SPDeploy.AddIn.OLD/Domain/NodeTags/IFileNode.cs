using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using EnvDTE80;
using System.IO;
using System.Xml;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags;
using Rapid.Tools.SPDeploy.AddIn.SPToolsWebService;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public interface INodeTag
    {
        ContextMenu GetContextMenu();
        void Action();
    }

    public enum NodeType
    {
        Site,
        Web,
        List,
        ListItem,
        Folder,
        File
    }

    public abstract class NodeTag : INodeTag
    {

        private Domain.Utilties.ApplicationUtility _applicationUtility;

        public Domain.Utilties.ApplicationUtility ApplicationUtility
        {
            get {
                if (_applicationUtility == null)
                    _applicationUtility = new Rapid.Tools.SPDeploy.AddIn.Domain.Utilties.ApplicationUtility(ApplicationObject);
                return _applicationUtility; }
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
            while (((NodeTag)_node.Tag).TagType != NodeType.Web)
            {
                _node = _node.Parent;
            }
            return ((NodeTag)_node.Tag).Guid;
        }

        private string _siteUrl;

        public string SiteUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_siteUrl))
                    _siteUrl = ((NodeTag)Node.TreeView.Nodes[0].Tag).Url;
                return _siteUrl;
            }
        }     

        private SPToolsWebService.SPToolsWebService _serviceInstance;

        public SPToolsWebService.SPToolsWebService ServiceInstance
        {
            get
            {
                if (_serviceInstance == null)
                {
                    _serviceInstance = new Rapid.Tools.SPDeploy.AddIn.SPToolsWebService.SPToolsWebService();
                    _serviceInstance.Url = string.Concat(Domain.Utilties.Functions.GetSiteUrlFromProject(ApplicationObject), "/_layouts/SPTools/SPToolsWebService.asmx");
                    _serviceInstance.Credentials = CredentialCache.DefaultCredentials;
                }
                return _serviceInstance;
            }
        }

        private Domain.RapidOutputWindow _outputWindow;

        public Domain.RapidOutputWindow RapidOutputWindow
        {
            get
            {
                if (_outputWindow == null)
                {
                    if (ApplicationObject == null)
                        return null;
                    _outputWindow = new Domain.RapidOutputWindow(ApplicationObject);
                }
                return _outputWindow;
            }
            set { _outputWindow = value; }
        }

       

        #region INodeTag Members

        public abstract ContextMenu GetContextMenu(); 

        public abstract void Action();      

        #endregion
    }

   

    public class NodeTagFactory
    {
        public static NodeTag Create(TreeNode node, DTE2 applicationObject, NodeType type)
        {
            switch (type)
            {
                case NodeType.Site:
                    break;
                case NodeType.Web:
                    return new WebNodeTag(node, applicationObject);
                    break;
                case NodeType.List:
                    return new ListNodeTag(node, applicationObject);
                    break;
                case NodeType.ListItem:
                    break;
                case NodeType.Folder:
                    break;
                case NodeType.File:
                    return new FileNodeTag(node, applicationObject);
                    break;
                default:
                    break;
            }
            return null;
        }
    }

    





}
