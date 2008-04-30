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

        public string ServerRelativeUrl;
        public Guid ID;
        public string Text;
        public DTE2 ApplicationObject;

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
				FileInfo ifile = new FileInfo(string.Format(@"{0}\{1}", wdir.FullName, ServerRelativeUrl.Replace("/", @"\")));

				if (!Directory.Exists(ifile.Directory.FullName))
					Directory.CreateDirectory(ifile.Directory.FullName);

				return ifile;
			}
		}


		//string siteUrl, Guid webID, string fileUrl, Guid fileGuid
		public void OpenWorkspaceFile()
		{
			byte[] contents = AppManager.Current.ActiveBridge.AddInService.OpenBinary(SiteUrl, WebID, ID);
			File.WriteAllBytes(WorkspacePath.FullName, contents);

			AppManager.Current.ActiveFileWatcher.AddWatcher(WorkspacePath, SiteUrl, WebID, ID);

			AppManager.Current.Application.ItemOperations.OpenFile(WorkspacePath.FullName, EnvDTE.Constants.vsViewKindTextView);
		}

		public void CloseWorkspaceFile()
		{
			CloseWorkspaceFile(EnvDTE.vsSaveChanges.vsSaveChangesPrompt);
		}

		public void CloseWorkspaceFile(EnvDTE.vsSaveChanges saveChanges)
		{
			if (File.Exists(WorkspacePath.FullName))
			{
				AppManager.Current.CloseFile(WorkspacePath.FullName);
				//File.Delete(WorkspacePath.FullName);
			}

			AppManager.Current.ActiveFileWatcher.RemoveWatcher(WorkspacePath);
		}


        #region INodeTag Members

        public abstract ContextMenu GetContextMenu();

        public abstract void DoubleClick();

        #endregion
    }
}
