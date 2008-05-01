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

        public override ContextMenu RightClick()
        {
            ContextMenu contextMenu = new ContextMenu();

			DirectoryInfo wdir = WorkspacePath.Directory;
			string wpath = WorkspacePath.FullName;

			contextMenu.MenuItems.Add("Open", delegate(object sender, EventArgs e)
				{
				   OpenWorkspaceFile();
				});

            contextMenu.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
				{
					AppManager.Current.OpenBrowser(SiteTag.Url + ServerRelativeUrl);
				});

            return contextMenu;
        }

        public override void DoubleClick()
        {
			OpenWorkspaceFile();
        }

		public void OpenWorkspaceFile()
		{
			string wpath = WorkspacePath.FullName;
			AppManager.Current.EnsureDirectory(wpath);

			File.WriteAllText(wpath, SiteTag.AddInService.GetViewSchema(WebID, ListID, Node.Text));
			AppManager.Current.OpenFile(wpath);

			SiteTag.Watcher.AddWatcher(this);
		}

		public void CloseWorkspaceFile()
		{
			if (File.Exists(WorkspacePath.FullName))
			{
				AppManager.Current.CloseFile(WorkspacePath.FullName);
				//File.Delete(WorkspacePath.FullName);
			}

			SiteTag.Watcher.RemoveWatcher(this);
		}
    }
}
