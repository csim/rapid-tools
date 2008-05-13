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
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class SPSiteNodeTag : SPWebNodeTag
    {
        private string _url;
        private Guid _siteID;


        private AddInWebService _addInService;
        private WebsWebService _websService;
        private ListsWebService _listsService;

        private FileWatcher _watcher;

        private List<Guid> _activatedFeatures = new List<Guid>();

        public List<Guid> ActivatedFeatures
        {
            get { return _activatedFeatures; }
            set { _activatedFeatures = value; }
        }
        
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public Guid SiteID
        {
            get { return _siteID; }
            set { _siteID = value; }
        }

        public FileWatcher Watcher
        {
            get
            {
                if (_watcher == null)
                    _watcher = new FileWatcher(WorkspacePath.Directory);

                return _watcher;
            }
        }

        public AddInWebService AddInService
        {
            get
            {
                if (_addInService == null)
                {
                    _addInService = new AddInWebService();
                    _addInService.Credentials = CredentialCache.DefaultCredentials;
                    _addInService.Url = string.Format("{0}/_layouts/RapidTools/Services/AddIn.asmx", Url);
                }

                return _addInService;
            }
        }


        public SPSiteNodeTag(TreeNode node)
            : base(node)
        {
            _node = node;
            TagType = NodeType.Site;
        }


        public WebsWebService WebsService
        {
            get
            {
                if (_websService == null)
                {
                    _websService = new WebsWebService();
                    _websService.Credentials = CredentialCache.DefaultCredentials;
                    _websService.Url = string.Format("{0}/_vti_bin/webs.asmx", Url);
                }
                return _websService;
            }
        }

        public ListsWebService ListsService
        {
            get
            {
                if (_listsService == null)
                {
                    _listsService = new ListsWebService();
                    _listsService.Credentials = CredentialCache.DefaultCredentials;
                    _listsService.Url = string.Format("{0}/_vti_bin/lists.asmx", Url);
                }
                return _listsService;
            }
        }



        public override void DoubleClick()
        {
            try
            {
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
                if (AppManager.Current.ActiveProject != null)
                {
                    List<FeatureManifest> projectFeatures = new List<FeatureManifest>(AppManager.Current.FeatureManifests);

                    MenuItem removeFeatureMenu = new MenuItem("Remove");
                    MenuItem addFeatureMenu = new MenuItem("Add");

                    foreach (FeatureManifest fm in projectFeatures)
                    {
                        if (fm.Scope != "Site" && fm.Scope != "Web") continue;

                        if (ActivatedFeatures.Contains(new Guid(fm.Id)))
                        {
                            MenuItem fMenuItem = new MenuItem(fm.Title + " (" + fm.Scope + ")", delegate(object sen, EventArgs ev)
                            {
                                try
                                {
                                    RapidOutputWindow.Instance.Activate();
                                    RapidOutputWindow.Instance.Clear();
                                    RapidOutputWindow.Instance.Write(SiteTag.AddInService.RemoveSiteFeature((Guid)((MenuItem)sen).Tag));
                                    RefreshFeatures();
                                }
                                catch (Exception ex)
                                {
                                    ExceptionUtil.Handle(ex);
                                }
                            });
                            fMenuItem.Tag = new Guid(fm.Id);
                            removeFeatureMenu.MenuItems.Add(fMenuItem);
                        }
                        else
                        {
                            if (Array.Find<Proxies.AddIn.Solution>(SiteTag.AddInService.GetSolutions(), delegate(Proxies.AddIn.Solution sol)
                            {
                                return string.Compare(sol.Name, AppManager.Current.ActiveProject.Name + ".wsp", true) == 0 && sol.Deployed;
                            }) != null)
                            {
                                MenuItem fMenuItem = new MenuItem(fm.Title + " (" + fm.Scope + ")", delegate(object sen, EventArgs ev)
                                 {
                                     try
                                     {
                                         RapidOutputWindow.Instance.Activate();
                                         RapidOutputWindow.Instance.Clear();

                                         RapidOutputWindow.Instance.Write(SiteTag.AddInService.AddSiteFeature((Guid)((MenuItem)sen).Tag));
                                         RefreshFeatures();
                                     }
                                     catch (Exception ex)
                                     {
                                         ExceptionUtil.Handle(ex);
                                     }
                                 });
                                fMenuItem.Tag = new Guid(fm.Id);
                                addFeatureMenu.MenuItems.Add(fMenuItem);
                            }
                        }
                    }
                    if (addFeatureMenu.MenuItems.Count > 0)
                        contextMenu.MenuItems.Add(addFeatureMenu);
                    if (removeFeatureMenu.MenuItems.Count > 0)
                        contextMenu.MenuItems.Add(removeFeatureMenu);
                }

                contextMenu.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
                {
                    Browse();
                });

                contextMenu.MenuItems.Add("Settings", delegate(object sender, EventArgs e)
                {
                    Browse("/_layouts/settings.aspx");                    
                });

                contextMenu.MenuItems.Add("Close", delegate(object sender, EventArgs e)
                {
                    Node.TreeView.Nodes.Remove(Node);
                });
            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex);
            }

            return contextMenu;
        }

        

        public new void RefreshFeatures()
        {
            ActivatedFeatures = new List<Guid>(AppManager.Current.GetActivatedSiteFeatures());            
        }
    }
}
