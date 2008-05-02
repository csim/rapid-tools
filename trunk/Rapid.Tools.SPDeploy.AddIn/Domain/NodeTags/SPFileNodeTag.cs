using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using EnvDTE80;
using System.Net;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class SPFileNodeTag : NodeTag
    {

        public SPFileNodeTag(TreeNode node)
        {
            _node = node;
			TagType = NodeType.File;
        }

        public override void DoubleClick()
        {
			try{
				if (!SiteTag.AddInService.IsCheckedOut(WebTag.ID, ID))
					SiteTag.AddInService.PerformFileAction(WebTag.ID, ID, Proxies.AddIn.FileActions.CheckOut);

				OpenWorkspaceFile();

				Resources.ResourceUtility.SetFileNodeIcon(Node, true);
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
		}

        public override ContextMenu RightClick()
        {
            ContextMenu contextMenu = new ContextMenu();

			try
			{

				if (!SiteTag.AddInService.IsCheckedOut(WebTag.ID, ID))
				{

					contextMenu.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
					{
						Browse();
					});

					contextMenu.MenuItems.Add("Check Out", delegate(object sender, EventArgs e)
					{

						SiteTag.AddInService.PerformFileAction(WebTag.ID, ID, Proxies.AddIn.FileActions.CheckOut);

						OpenWorkspaceFile();

						Resources.ResourceUtility.SetFileNodeIcon(Node, true);
					});
				}
				else
				{

					bool isopen = AppManager.Current.IsFileOpen(WorkspacePath.FullName);

					if (!isopen)
					{
						contextMenu.MenuItems.Add("Open", delegate(object sender, EventArgs e)
						{
						   OpenWorkspaceFile();
						});
					}

					contextMenu.MenuItems.Add("Check In", delegate(object sender, EventArgs e)
					{
						SiteTag.AddInService.PerformFileAction(WebTag.ID, ID, Proxies.AddIn.FileActions.CheckIn);
						Resources.ResourceUtility.SetFileNodeIcon(Node, false);
						CloseWorkspaceFile();
					});

					contextMenu.MenuItems.Add("Discard Check Out", delegate(object sender, EventArgs e)
					{
						SiteTag.AddInService.PerformFileAction(WebTag.ID, ID, Proxies.AddIn.FileActions.UndoCheckOut);
						Resources.ResourceUtility.SetFileNodeIcon(Node, false);
						CloseWorkspaceFile();
					});
				}

				contextMenu.MenuItems.Add("Delete", delegate(object sender, EventArgs e)
				{
					SiteTag.AddInService.PerformFileAction(WebTag.ID, ID, Proxies.AddIn.FileActions.Delete);
					CloseWorkspaceFile();
				});
			
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}

            return contextMenu;
        }

		public void OpenWorkspaceFile()
		{
			byte[] contents = SiteTag.AddInService.OpenBinary(WebTag.ID, ID);
			File.WriteAllBytes(WorkspacePath.FullName, contents);

			SiteTag.Watcher.AddWatcher(this);
			AppManager.Current.OpenFile(WorkspacePath.FullName);
		}

		public void CloseWorkspaceFile()
		{
			CloseWorkspaceFile(EnvDTE.vsSaveChanges.vsSaveChangesPrompt);
		}

		public void CloseWorkspaceFile(EnvDTE.vsSaveChanges promptType)
		{
			if (File.Exists(WorkspacePath.FullName))
			{
				AppManager.Current.CloseFile(WorkspacePath.FullName, promptType);
				File.Delete(WorkspacePath.FullName);
			}

			SiteTag.Watcher.RemoveWatcher(this);
		}

    }
}
