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
		private SPSiteNodeTag _siteNode;

        protected TreeNode _node;

        public TreeNode Node
        {
            get { return _node; }
        }

		public Guid ID;
		public Guid ListID; 
		public string ServerRelativeUrl;
        public string Text;

		public Guid _webID; 

		public Guid WebID
		{
			get
			{

				if (_webID == Guid.Empty)
				{
					TreeNode inode = Node;

					while (!(inode.Tag is SPSiteNodeTag) && !(inode.Tag is SPWebNodeTag))
						inode = inode.Parent;

					_webID = ((NodeTag)inode.Tag).ID;
				}

				return _webID;
			}
		}

		public SPSiteNodeTag SiteTag
		{
			get {

				if (_siteNode == null)
				{
					TreeNode inode = Node;
					
					while (!(inode.Tag is SPSiteNodeTag))
						inode = inode.Parent;

					_siteNode = (SPSiteNodeTag)inode.Tag;
				}

				return _siteNode;
			}
		}


		public FileInfo WorkspacePath
		{
			get
			{

				string wdir = string.Format(@"{0}\SPDeploy\Workspace", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
				string wpath = string.Format(@"{0}\{1}\{2}", wdir, SiteTag.ID.ToString().Replace("{", "").Replace("}", "").Replace("-", ""), ServerRelativeUrl.Replace("/", @"\"));

				if (TagType == NodeType.View)
					wpath += ".xml";

				FileInfo ifile = new FileInfo(wpath);

				if (!Directory.Exists(ifile.Directory.FullName))
					Directory.CreateDirectory(ifile.Directory.FullName);

				return ifile;
			}
		}


        public abstract ContextMenu RightClick();

        public abstract void DoubleClick();

	}
}
