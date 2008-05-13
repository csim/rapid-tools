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
   
    public abstract class NodeTag
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

		public SPWebNodeTag _webTag;

		public SPWebNodeTag WebTag
		{
			get
			{

				if (_webTag == null)
				{
					TreeNode inode = Node;

					while (!(inode.Tag is SPWebNodeTag))
						inode = inode.Parent;

					_webTag = (SPWebNodeTag)inode.Tag;
				}

				return _webTag;
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

				string wpath = string.Format(@"{0}\SPDeploy\Workspace", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
				wpath = string.Format(@"{0}\{1}\{2}", wpath, SiteTag.ID.ToString().Replace("{", "").Replace("}", "").Replace("-", ""), ServerRelativeUrl.Replace("/", @"\"));
				wpath = wpath.Replace(@"\\", @"\");

				if (TagType == NodeType.View)
					wpath += ".xml";

				FileInfo ifile = new FileInfo(wpath);

				if (!Directory.Exists(ifile.Directory.FullName))
					Directory.CreateDirectory(ifile.Directory.FullName);

				return ifile;
			}
		}

        protected void Browse()
        {
            Browse(string.Empty);
        }

		protected void Browse(string relativeUrl) 
		{
			AppManager.Current.OpenBrowser(string.Format("{0}{1}{2}", SiteTag.Url, ServerRelativeUrl, relativeUrl));
		}

		public abstract void Focus();

		public abstract void DoubleClick();
		
		public abstract ContextMenu RightClick();

	}
}
