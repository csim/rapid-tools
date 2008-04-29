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

			DirectoryInfo wdir = EnvironmentUtil.GetWorkingDirectory();
			string filePath = string.Format(@"{0}\{1}\{2}\{3}.xml", wdir.FullName, Node.TreeView.Nodes[0].Text, Node.Parent.Parent.Text, Node.Text);

            if (ApplicationObject.ActiveDocument == null || ApplicationObject.ActiveDocument.FullName != filePath)
            {
                _contextMenu.MenuItems.Add("Open", delegate(object sender, EventArgs e)
                   {
					   EnvironmentUtil.EnsureDirectory(filePath);
                       File.WriteAllText(filePath, ServiceInstance.GetViewSchema(SiteUrl, WebGuid, ListGuid, Node.Text));
                       ApplicationUtility.OpenFile(filePath);
                   });
            }


            if (File.Exists(filePath))
            {
                _contextMenu.MenuItems.Add("Save To List", delegate(object sender, EventArgs e)
                {
                    ServiceInstance.UpdateViewSchema(SiteUrl, WebGuid, ListGuid, Node.Text, File.ReadAllText(filePath));
                    ApplicationUtility.DeleteAndClose(filePath);
                });
                _contextMenu.MenuItems.Add("Remove From Workspace", delegate(object sender, EventArgs e)
                {
                    ApplicationUtility.DeleteAndClose(filePath);
                });
            }

            _contextMenu.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
            {
                ApplicationUtility.OpenBrowser(SiteUrl + "/" + Url);
            });

            return _contextMenu;
        }

        public override void Action()
        {
            
        }
    }
}
