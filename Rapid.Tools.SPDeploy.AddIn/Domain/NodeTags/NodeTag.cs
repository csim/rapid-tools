using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using EnvDTE80;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.Proxies.AddIn;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
   
    public abstract class NodeTag : INodeTag
    {

        public NodeType TagType;

        protected TreeNode _node;

        public TreeNode Node
        {
            get { return _node; }
        }

		public Guid ID;
		public Guid ListID; 
		public string ServerRelativeUrl;
        public string Text;

        public Guid WebID
        {
            get
            {
				TreeNode inode = Node;

				while (inode.Tag == null || ((NodeTag)inode.Tag).TagType != NodeType.Web)
				{
					inode = inode.Parent;
				}

				return ((NodeTag)inode.Tag).ID;
            }
        }

        private string _siteUrl;

        public string SiteUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_siteUrl))
                    _siteUrl = ((NodeTag)Node.TreeView.Nodes[0].Tag).ServerRelativeUrl;
                return _siteUrl;
            }
        }

		public FileInfo WorkspacePath
		{
			get
			{
				DirectoryInfo wdir = AppManager.Current.ActiveWorkspaceDirectory;

				string wpath = string.Format(@"{0}{1}", wdir.FullName, ServerRelativeUrl.Replace("/", @"\"));
				

				if (TagType == NodeType.View)
					wpath += ".xml";

				FileInfo ifile = new FileInfo(wpath);

				if (!Directory.Exists(ifile.Directory.FullName))
					Directory.CreateDirectory(ifile.Directory.FullName);

				return ifile;
			}
		}

        #region INodeTag Members

        public abstract ContextMenu RightClick();

        public abstract void DoubleClick();

        #endregion
    }
}
