using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using EnvDTE80;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.SPToolsWebService;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class FileNodeTag : NodeTag
    {

        public FileNodeTag(TreeNode node, DTE2 applicationObject)
        {
            _node = node;
            ApplicationObject = applicationObject;
        }

        public override void Action()
        {
            if (!ServiceInstance.IsCheckedOut(SiteUrl, WebGuid, Guid))
                ServiceInstance.PerformFileAction(SiteUrl, WebGuid, Guid, FileActions.CheckOut);

            string filePath = Domain.Utilties.Functions.GetWorkingDirectoryPath() + "\\" + Url.Replace("/", "\\");
            if (!File.Exists(filePath))
            {
                File.WriteAllBytes(filePath, ServiceInstance.OpenBinary(SiteUrl, WebGuid, Guid));
            }
            ApplicationUtility.OpenFile(filePath);
            Resources.ResourceUtility.SetFileNodeIcon(Node, true);
        }


        public override ContextMenu GetContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();

            string filePath = Domain.Utilties.Functions.GetWorkingDirectoryPath() + "\\" + Url.Replace("/", "\\");


            string fpath = Url.Replace("/", "\\");

            if (fpath.Contains("\\"))
            {
                fpath = fpath.Remove(fpath.LastIndexOf("\\"));
                Domain.Utilties.Functions.GetWorkingDirectory().CreateSubdirectory(fpath);
            }
            if (!ServiceInstance.IsCheckedOut(SiteUrl, WebGuid, Guid))
            {

                _contextMenu.MenuItems.Add("Preview", delegate(object sender, EventArgs e)
               {
                   string path = Domain.Utilties.Functions.GetRandomTempPath();

                   File.WriteAllBytes(path, ServiceInstance.OpenBinary(SiteUrl, WebGuid, Guid));

                   ApplicationUtility.OpenFile(path);
               });

                _contextMenu.MenuItems.Add("Check Out", delegate(object sender, EventArgs e)
                {

                    ServiceInstance.PerformFileAction(SiteUrl, WebGuid, Guid, FileActions.CheckOut);

                    File.WriteAllBytes(filePath, ServiceInstance.OpenBinary(SiteUrl, WebGuid, Guid));

                    ApplicationUtility.OpenFile(filePath);

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

                _contextMenu.MenuItems.Add("Save Back (Temp)", delegate(object sender, EventArgs e)
                {
                    ServiceInstance.SaveBinary(SiteUrl, WebGuid, Guid, File.ReadAllBytes(filePath));
                });

                _contextMenu.MenuItems.Add("Check In", delegate(object sender, EventArgs e)
                {
                    ServiceInstance.SaveBinary(SiteUrl, WebGuid, Guid, File.ReadAllBytes(filePath));

                    ServiceInstance.PerformFileAction(SiteUrl, WebGuid, Guid, FileActions.CheckIn);

                    Resources.ResourceUtility.SetFileNodeIcon(Node, false);

                    ApplicationUtility.DeleteAndClose(filePath);

                });
                _contextMenu.MenuItems.Add("Discard Check Out", delegate(object sender, EventArgs e)
                {
                    ServiceInstance.PerformFileAction(SiteUrl, WebGuid, Guid, FileActions.UndoCheckOut);

                    Resources.ResourceUtility.SetFileNodeIcon(Node, false);

                    ApplicationUtility.DeleteAndClose(filePath);
                });
            }

            _contextMenu.MenuItems.Add("Delete", delegate(object sender, EventArgs e)
            {
                ServiceInstance.PerformFileAction(SiteUrl, WebGuid, Guid, FileActions.Delete);

                ApplicationUtility.DeleteAndClose(filePath);
            });


            return _contextMenu;
        }
    }
}
