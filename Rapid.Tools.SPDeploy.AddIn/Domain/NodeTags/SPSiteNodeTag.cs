using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using EnvDTE80;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.Proxies.AddIn;
using Rapid.Tools.SPDeploy.AddIn.Proxies.Webs;
using Rapid.Tools.SPDeploy.AddIn.Proxies.Lists;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class SPSiteNodeTag : NodeTag
    {
		private string _url;

		private AddInProxy _addInService;
		private WebsProxy _websService;
		private ListsProxy _listsService;
		
		private FileWatcher _watcher;

		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		public FileWatcher Watcher
		{
			get {
				if (_watcher == null)
					_watcher = new FileWatcher(WorkspacePath.Directory);

				return _watcher; 
			}
		}

		public AddInProxy AddInService
		{
			get
			{
				if (_addInService == null)
				{
					_addInService = new AddInProxy();
					_addInService.Credentials = CredentialCache.DefaultCredentials;
					_addInService.Url = string.Format("{0}/_layouts/RapidTools/Services/AddIn.asmx", Url);
				}

				return _addInService;
			}
		}


		public SPSiteNodeTag(TreeNode node)
        {
            _node = node;
			TagType = NodeType.Site;
        }


		public WebsProxy WebsService
		{
			get
			{
				if (_websService == null)
				{
					_websService = new WebsProxy();
					_websService.Credentials = CredentialCache.DefaultCredentials;
					_websService.Url = string.Format("{0}/_vti_bin/webs.asmx", Url);
				}
				return _websService;
			}
		}

		public ListsProxy ListsService
		{
			get
			{
				if (_listsService == null)
				{
					_listsService = new ListsProxy();
					_listsService.Credentials = CredentialCache.DefaultCredentials;
					_listsService.Url = string.Format("{0}/_vti_bin/lists.asmx", Url);
				}
				return _listsService;
			}
		}

        public override void DoubleClick()
        {
            
        }

        public override ContextMenu RightClick()
        {
            ContextMenu contextMenu = new ContextMenu();
            //contextMenu.MenuItems.Add("No Actions");

			contextMenu.MenuItems.Add("Close", delegate(object sender, EventArgs e)
			{
				Node.TreeView.Nodes.Remove(Node);
			});
			
			return contextMenu;
        }
    }
}
