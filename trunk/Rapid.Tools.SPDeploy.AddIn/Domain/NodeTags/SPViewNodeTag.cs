using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using EnvDTE80;
using Rapid.Tools.SPDeploy.AddIn.Domain.Utilties;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class SPViewNodeTag : WebNodeTag
    {

        private Guid ListGuid;

        public SPViewNodeTag(TreeNode node, DTE2 applicationObject)
        {
            _node = node;
            ApplicationObject = applicationObject;
            ListGuid = ((WebNodeTag)Node.Parent.Parent.Tag).Guid;
        }

        public override ContextMenu GetContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();

			DirectoryInfo wdir = AppManager.Instance.GetWorkspaceDirectory();
			string filePath = string.Format(@"{0}\{1}\{2}\{3}.xml", wdir.FullName, Node.TreeView.Nodes[0].Text, Node.Parent.Parent.Text, Node.Text);

            if (ApplicationObject.ActiveDocument == null || ApplicationObject.ActiveDocument.FullName != filePath)
            {
                _contextMenu.MenuItems.Add("Open", delegate(object sender, EventArgs e)
                   {
					   AppManager.Instance.EnsureDirectory(filePath);
                       File.WriteAllText(filePath, AddInService.GetViewSchema(SiteUrl, WebGuid, ListGuid, Node.Text));
                       AppManager.Instance.OpenFile(filePath);
                   });
            }


            if (File.Exists(filePath))
            {
                _contextMenu.MenuItems.Add("Save To List", delegate(object sender, EventArgs e)
                {
                    AddInService.UpdateViewSchema(SiteUrl, WebGuid, ListGuid, Node.Text, File.ReadAllText(filePath));
					AppManager.Instance.CloseWorkspaceFile(filePath);
                });
                _contextMenu.MenuItems.Add("Remove From Workspace", delegate(object sender, EventArgs e)
                {
					AppManager.Instance.CloseWorkspaceFile(filePath);
                });
            }

            _contextMenu.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
            {
				AppManager.Instance.OpenBrowser(SiteUrl + "/" + Url);
            });

            return _contextMenu;
        }

        public override void Action()
        {
            
        }
    }
}
