using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using EnvDTE80;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.Domain.Utilties;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class SPFileNodeTag : WebNodeTag
    {

        public SPFileNodeTag(TreeNode node, DTE2 applicationObject)
        {
            _node = node;
            ApplicationObject = applicationObject;
        }

        public override void Action()
        {
            if (!ServiceInstance.IsCheckedOut(SiteUrl, WebGuid, Guid))
				ServiceInstance.PerformFileAction(SiteUrl, WebGuid, Guid, Proxies.AddIn.FileActions.CheckOut);

			string wguid = WebGuid.ToString().Replace("{", "").Replace("}", "");

			DirectoryInfo wdir = EnvironmentUtil.GetWorkingDirectory();
			string filePath = string.Format(@"{0}\{1}\{2}\{3}", wdir.FullName, Node.TreeView.Nodes[0].Text, wguid, Url.Replace("/", @"\"));

			EnvironmentUtil.EnsureDirectory(filePath);
            if (!File.Exists(filePath))
            {
                File.WriteAllBytes(filePath, ServiceInstance.OpenBinary(SiteUrl, WebGuid, Guid));
            }
            ApplicationUtility.OpenFile(filePath);

            Domain.Utilties.WatcherUtilitiy.Instance.AddWatcher(filePath, SiteUrl, WebGuid, Guid);

            Resources.ResourceUtility.SetFileNodeIcon(Node, true);
        }


        public override ContextMenu GetContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();

			string wguid = WebGuid.ToString().Replace("{", "").Replace("}", "");

			DirectoryInfo wdir = EnvironmentUtil.GetWorkingDirectory();
			string filePath = string.Format(@"{0}\{1}\{2}\{3}", wdir.FullName, Node.TreeView.Nodes[0].Text, wguid, Url.Replace("/", @"\"));

			EnvironmentUtil.EnsureDirectory(filePath);
            if (!File.Exists(filePath))
                File.WriteAllBytes(filePath, ServiceInstance.OpenBinary(SiteUrl, WebGuid, Guid));


            //_contextMenu.MenuItems.Add("GetfileInfo", delegate(object sender, EventArgs e)
            //{
            //    string tpath = Domain.Utilties.Functions.GetRandomTempPath();
            //    File.WriteAllText(tpath, ServiceInstance.GetFileInfo(SiteUrl, WebGuid, Guid));
            //    ApplicationUtility.OpenFile(tpath);
            //});


            if (!ServiceInstance.IsCheckedOut(SiteUrl, WebGuid, Guid))
            {

                _contextMenu.MenuItems.Add("Preview", delegate(object sender, EventArgs e)
               {
                   string path = Domain.Utilties.EnvironmentUtil.GetRandomTempPath();

                   File.WriteAllBytes(path, ServiceInstance.OpenBinary(SiteUrl, WebGuid, Guid));

                   ApplicationUtility.OpenFile(path);
               });

                _contextMenu.MenuItems.Add("Check Out", delegate(object sender, EventArgs e)
                {

					ServiceInstance.PerformFileAction(SiteUrl, WebGuid, Guid, Proxies.AddIn.FileActions.CheckOut);

                    File.WriteAllBytes(filePath, ServiceInstance.OpenBinary(SiteUrl, WebGuid, Guid));

                    ApplicationUtility.OpenFile(filePath);

                    Domain.Utilties.WatcherUtilitiy.Instance.AddWatcher(filePath, SiteUrl, WebGuid, Guid);

                    Resources.ResourceUtility.SetFileNodeIcon(Node, true);
                });
            }
            else
            {

                foreach (EnvDTE.Document doc in ApplicationObject.Documents)
                {
                    if (doc.FullName == filePath)
                        goto Label_001;
                }
                _contextMenu.MenuItems.Add("Open", delegate(object sender, EventArgs e)
               {
                   if (!File.Exists(filePath))
                   {
                       File.WriteAllBytes(filePath, ServiceInstance.OpenBinary(SiteUrl, WebGuid, Guid));
                   }
                   ApplicationUtility.OpenFile(filePath);
               });

            Label_001:
                               
                _contextMenu.MenuItems.Add("Check In", delegate(object sender, EventArgs e)
                {
                    ServiceInstance.SaveBinary(SiteUrl, WebGuid, Guid, File.ReadAllBytes(filePath));

					ServiceInstance.PerformFileAction(SiteUrl, WebGuid, Guid, Proxies.AddIn.FileActions.CheckIn);

                    Resources.ResourceUtility.SetFileNodeIcon(Node, false);

                    Domain.Utilties.WatcherUtilitiy.Instance.removeWatcher(filePath);

                    ApplicationUtility.DeleteAndClose(filePath);

                });
                _contextMenu.MenuItems.Add("Discard Check Out", delegate(object sender, EventArgs e)
                {
					ServiceInstance.PerformFileAction(SiteUrl, WebGuid, Guid, Proxies.AddIn.FileActions.UndoCheckOut);

                    Resources.ResourceUtility.SetFileNodeIcon(Node, false);

                    Domain.Utilties.WatcherUtilitiy.Instance.removeWatcher(filePath);

                    ApplicationUtility.DeleteAndClose(filePath);
                });
            }

            _contextMenu.MenuItems.Add("Delete", delegate(object sender, EventArgs e)
            {
				ServiceInstance.PerformFileAction(SiteUrl, WebGuid, Guid, Proxies.AddIn.FileActions.Delete);

                ApplicationUtility.DeleteAndClose(filePath);

                Domain.Utilties.WatcherUtilitiy.Instance.removeWatcher(filePath);
            });


            return _contextMenu;
        }
    }
}
