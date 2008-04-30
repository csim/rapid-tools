using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using EnvDTE80;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class SPViewNodeTag : NodeTag
    {
        public SPViewNodeTag(TreeNode node)
        {
            _node = node;
			TagType = NodeType.View;
            ListID = ((NodeTag)Node.Parent.Parent.Tag).ID;
        }

        public override ContextMenu GetContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();

			DirectoryInfo wdir = AppManager.Current.ActiveWorkspaceDirectory;
			string wpath = WorkspacePath.FullName;

			if (AppManager.Current.Application.ActiveDocument == null || AppManager.Current.Application.ActiveDocument.FullName != wpath)
            {
                _contextMenu.MenuItems.Add("Open", delegate(object sender, EventArgs e)
                   {
					   OpenWorkspaceFile();
                   });
            }

            _contextMenu.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
            {
				AppManager.Current.OpenBrowser(SiteUrl + "/" + ServerRelativeUrl);
            });

            return _contextMenu;
        }

        public override void DoubleClick()
        {
			OpenWorkspaceFile();
        }

		public void OpenWorkspaceFile()
		{
			string wpath = WorkspacePath.FullName;
			AppManager.Current.EnsureDirectory(wpath);
			
			File.WriteAllText(wpath, AppManager.Current.ActiveBridge.AddInService.GetViewSchema(WebID, ListID, Node.Text));
			AppManager.Current.OpenFile(wpath);

			AppManager.Current.ActiveFileWatcher.AddWatcher(this);
		}

		public void CloseWorkspaceFile()
		{
			if (File.Exists(WorkspacePath.FullName))
			{
				AppManager.Current.CloseFile(WorkspacePath.FullName);
				//File.Delete(WorkspacePath.FullName);
			}

			AppManager.Current.ActiveFileWatcher.RemoveWatcher(this);
		}
    }
}
