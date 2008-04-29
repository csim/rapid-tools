using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using EnvDTE80;
using Rapid.Tools.SPDeploy.AddIn.Domain.Utilties;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class SPViewNodeTag : NodeTag
    {

        private Guid ListID;

        public SPViewNodeTag(TreeNode node, DTE2 applicationObject)
        {
            _node = node;
            ApplicationObject = applicationObject;
            ListID = ((NodeTag)Node.Parent.Parent.Tag).ID;
        }

        public override ContextMenu GetContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();

			DirectoryInfo wdir = AppManager.Current.ActiveWorkspaceDirectory;
			string filePath = string.Format(@"{0}\{1}\{2}\{3}.xml", wdir.FullName, Node.TreeView.Nodes[0].Text, Node.Parent.Parent.Text, Node.Text);

            if (ApplicationObject.ActiveDocument == null || ApplicationObject.ActiveDocument.FullName != filePath)
            {
                _contextMenu.MenuItems.Add("Open", delegate(object sender, EventArgs e)
                   {
					   AppManager.Current.EnsureDirectory(filePath);
					   File.WriteAllText(filePath, AppManager.Current.ActiveBridge.AddInService.GetViewSchema(SiteUrl, WebID, ListID, Node.Text));
                       AppManager.Current.OpenFile(filePath);
                   });
            }


            if (File.Exists(filePath))
            {
                _contextMenu.MenuItems.Add("Save To List", delegate(object sender, EventArgs e)
                {
					AppManager.Current.ActiveBridge.AddInService.UpdateViewSchema(SiteUrl, WebID, ListID, Node.Text, File.ReadAllText(filePath));
					AppManager.Current.CloseFile(filePath);
                });
                _contextMenu.MenuItems.Add("Remove From Workspace", delegate(object sender, EventArgs e)
                {
					AppManager.Current.CloseFile(filePath);
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
            
        }
    }
}
