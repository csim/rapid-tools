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
            if (!AppManager.Current.ActiveBridge.AddInService.IsCheckedOut(WebID, ID))
				AppManager.Current.ActiveBridge.AddInService.PerformFileAction(WebID, ID, Proxies.AddIn.FileActions.CheckOut);

			OpenWorkspaceFile();

            Resources.ResourceUtility.SetFileNodeIcon(Node, true);
        }

        public override ContextMenu RightClick()
        {
            ContextMenu _contextMenu = new ContextMenu();


			if (!AppManager.Current.ActiveBridge.AddInService.IsCheckedOut(WebID, ID))
            {

			   // _contextMenu.MenuItems.Add("Preview", delegate(object sender, EventArgs e)
			   //{
			   //    AppManager.Current.OpenFile(WebID, ServerRelativeUrl, FileID);
			   //});

                _contextMenu.MenuItems.Add("Check Out", delegate(object sender, EventArgs e)
                {

					AppManager.Current.ActiveBridge.AddInService.PerformFileAction(WebID, ID, Proxies.AddIn.FileActions.CheckOut);

					OpenWorkspaceFile();

                    Resources.ResourceUtility.SetFileNodeIcon(Node, true);
                });
            }
            else
            {

				bool isopen = AppManager.Current.IsFileOpen(WorkspacePath.FullName);

				if (!isopen)
				{
					_contextMenu.MenuItems.Add("Open", delegate(object sender, EventArgs e)
				   {
					   OpenWorkspaceFile();
				   });
				}

				_contextMenu.MenuItems.Add("Check In", delegate(object sender, EventArgs e)
                {
					AppManager.Current.ActiveBridge.AddInService.PerformFileAction(WebID, ID, Proxies.AddIn.FileActions.CheckIn);
                    Resources.ResourceUtility.SetFileNodeIcon(Node, false);
					CloseWorkspaceFile();

                });
                _contextMenu.MenuItems.Add("Discard Check Out", delegate(object sender, EventArgs e)
                {
					AppManager.Current.ActiveBridge.AddInService.PerformFileAction(WebID, ID, Proxies.AddIn.FileActions.UndoCheckOut);
                    Resources.ResourceUtility.SetFileNodeIcon(Node, false);
					CloseWorkspaceFile();
                });
            }

            _contextMenu.MenuItems.Add("Delete", delegate(object sender, EventArgs e)
            {
				AppManager.Current.ActiveBridge.AddInService.PerformFileAction(WebID, ID, Proxies.AddIn.FileActions.Delete);
				CloseWorkspaceFile();
            });


            return _contextMenu;
        }

		public void OpenWorkspaceFile()
		{
			byte[] contents = AppManager.Current.ActiveBridge.AddInService.OpenBinary(WebID, ID);
			File.WriteAllBytes(WorkspacePath.FullName, contents);

			AppManager.Current.ActiveFileWatcher.AddWatcher(this);
			AppManager.Current.OpenFile(WorkspacePath.FullName);
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

			AppManager.Current.ActiveFileWatcher.RemoveWatcher(this);
		}

    }
}
