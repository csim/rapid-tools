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
            if (!AddInService.IsCheckedOut(SiteUrl, WebGuid, Guid))
				AddInService.PerformFileAction(SiteUrl, WebGuid, Guid, Proxies.AddIn.FileActions.CheckOut);

			string wguid = WebGuid.ToString().Replace("{", "").Replace("}", "");

			DirectoryInfo wdir = AppManager.Current.ActiveWorkspaceDirectory;
			string filePath = string.Format(@"{0}\{1}\{2}\{3}", wdir.FullName, Node.TreeView.Nodes[0].Text, wguid, Url.Replace("/", @"\"));

			AppManager.Current.EnsureDirectory(filePath);
            if (!File.Exists(filePath))
            {
                File.WriteAllBytes(filePath, AddInService.OpenBinary(SiteUrl, WebGuid, Guid));
            }
			AppManager.Current.OpenFile(filePath);

            Domain.Utilties.WatcherUtil.Instance.AddWatcher(filePath, SiteUrl, WebGuid, Guid);

            Resources.ResourceUtility.SetFileNodeIcon(Node, true);
        }


        public override ContextMenu GetContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();

			string wguid = WebGuid.ToString().Replace("{", "").Replace("}", "");

			DirectoryInfo wdir = AppManager.Current.ActiveWorkspaceDirectory;
			string filePath = string.Format(@"{0}\{1}\{2}\{3}", wdir.FullName, Node.TreeView.Nodes[0].Text, wguid, Url.Replace("/", @"\"));

			AppManager.Current.EnsureDirectory(filePath);
            if (!File.Exists(filePath))
                File.WriteAllBytes(filePath, AddInService.OpenBinary(SiteUrl, WebGuid, Guid));


            //_contextMenu.MenuItems.Add("GetfileInfo", delegate(object sender, EventArgs e)
            //{
            //    string tpath = Domain.Utilties.Functions.GetRandomTempPath();
            //    File.WriteAllText(tpath, ServiceInstance.GetFileInfo(SiteUrl, WebGuid, Guid));
            //    ApplicationUtility.OpenFile(tpath);
            //});


            if (!AddInService.IsCheckedOut(SiteUrl, WebGuid, Guid))
            {

                _contextMenu.MenuItems.Add("Preview", delegate(object sender, EventArgs e)
               {
				   string path = AppManager.Current.GetRandomTempPath();

                   File.WriteAllBytes(path, AddInService.OpenBinary(SiteUrl, WebGuid, Guid));

				   AppManager.Current.OpenFile(path);
               });

                _contextMenu.MenuItems.Add("Check Out", delegate(object sender, EventArgs e)
                {

					AddInService.PerformFileAction(SiteUrl, WebGuid, Guid, Proxies.AddIn.FileActions.CheckOut);

                    File.WriteAllBytes(filePath, AddInService.OpenBinary(SiteUrl, WebGuid, Guid));

					AppManager.Current.OpenFile(filePath);

                    Domain.Utilties.WatcherUtil.Instance.AddWatcher(filePath, SiteUrl, WebGuid, Guid);

                    Resources.ResourceUtility.SetFileNodeIcon(Node, true);
                });
            }
            else
            {

				bool isopen = AppManager.Current.IsFileOpen(filePath);

				if (!isopen)
				{
					_contextMenu.MenuItems.Add("Open", delegate(object sender, EventArgs e)
				   {
					   if (!File.Exists(filePath))
					   {
						   File.WriteAllBytes(filePath, AddInService.OpenBinary(SiteUrl, WebGuid, Guid));
					   }
					   AppManager.Current.OpenFile(filePath);
				   });
				}

				_contextMenu.MenuItems.Add("Check In", delegate(object sender, EventArgs e)
                {
                    AddInService.SaveBinary(SiteUrl, WebGuid, Guid, File.ReadAllBytes(filePath));

					AddInService.PerformFileAction(SiteUrl, WebGuid, Guid, Proxies.AddIn.FileActions.CheckIn);

                    Resources.ResourceUtility.SetFileNodeIcon(Node, false);

                    Domain.Utilties.WatcherUtil.Instance.RemoveWatcher(filePath);

					AppManager.Current.CloseWorkspaceFile(filePath);

                });
                _contextMenu.MenuItems.Add("Discard Check Out", delegate(object sender, EventArgs e)
                {
					AddInService.PerformFileAction(SiteUrl, WebGuid, Guid, Proxies.AddIn.FileActions.UndoCheckOut);

                    Resources.ResourceUtility.SetFileNodeIcon(Node, false);

                    Domain.Utilties.WatcherUtil.Instance.RemoveWatcher(filePath);

					AppManager.Current.CloseWorkspaceFile(filePath);
                });
            }

            _contextMenu.MenuItems.Add("Delete", delegate(object sender, EventArgs e)
            {
				AddInService.PerformFileAction(SiteUrl, WebGuid, Guid, Proxies.AddIn.FileActions.Delete);

				AppManager.Current.CloseWorkspaceFile(filePath);

                Domain.Utilties.WatcherUtil.Instance.RemoveWatcher(filePath);
            });


            return _contextMenu;
        }
    }
}
