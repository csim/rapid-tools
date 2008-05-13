using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using System.IO;
using EnvDTE80;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class SPWebNodeTag : NodeTag
    {
        public SPWebNodeTag(TreeNode node)
        {
            TagType = NodeType.Web;
            _node = node;
        }

        public override void Focus()
        {
        }

        private List<Guid> _activatedFeatures = new List<Guid>();

        public List<Guid> ActivatedFeatures
        {
            get { return _activatedFeatures; }
            set { _activatedFeatures = value; }
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
                    MenuItem _addFeatureMenu = new MenuItem("Add Feature");
                    MenuItem _removeFeatureMenu = new MenuItem("Remove Feature");

                    foreach (FeatureManifest featureManifest in AppManager.Current.FeatureManifests)
                    {
                        if (featureManifest.Scope != "Web") continue;

                        if (ActivatedFeatures.Contains(new Guid(featureManifest.Id)))
                        {

                            MenuItem _featureMenuItem = new MenuItem(featureManifest.Title);
                            _featureMenuItem.Click += delegate(object s, EventArgs ea)
                                {
                                    RapidOutputWindow.Instance.Activate();
                                    RapidOutputWindow.Instance.Clear();
                                    RapidOutputWindow.Instance.Write(SiteTag.AddInService.RemoveFeature(WebTag.ID, (Guid)((MenuItem)s).Tag));
                                    RefreshFeatures();
                                };
                            _featureMenuItem.Tag = new Guid(featureManifest.Id);
                            _removeFeatureMenu.MenuItems.Add(_featureMenuItem);

                        }
                        else
                        {
                            if (Array.Find<Proxies.AddIn.Solution>(SiteTag.AddInService.GetSolutions(), delegate(Proxies.AddIn.Solution sol)
                            {
                                return string.Compare(sol.Name, AppManager.Current.ActiveProject.Name + ".wsp", true) == 0 && sol.Deployed;
                            }) != null)
                            {
                                MenuItem _featureMenuItem = new MenuItem(featureManifest.Title);
                                _featureMenuItem.Click += delegate(object s, EventArgs ea)
                                    {
                                        RapidOutputWindow.Instance.Activate();
                                        RapidOutputWindow.Instance.Clear();
                                        RapidOutputWindow.Instance.Write(SiteTag.AddInService.AddFeature(((NodeTag)Node.Tag).WebTag.ID, (Guid)((MenuItem)s).Tag));
                                        RefreshFeatures();
                                    };
                                _featureMenuItem.Tag = new Guid(featureManifest.Id);
                                _addFeatureMenu.MenuItems.Add(_featureMenuItem);
                            }
                        }
                    }

                    if (_addFeatureMenu.MenuItems.Count > 0)
                        contextMenu.MenuItems.Add(_addFeatureMenu);
                    if (_removeFeatureMenu.MenuItems.Count > 0)
                        contextMenu.MenuItems.Add(_removeFeatureMenu);
                }
                contextMenu.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
                {
                    Browse();
                });

                contextMenu.MenuItems.Add("Settings", delegate(object sender, EventArgs e)
               {
                   Browse("/_layouts/settings.aspx"); 
               });


                
            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex);
            }



            return contextMenu;
        }
               

        public void RefreshFeatures()
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
            {
                ActivatedFeatures = new List<Guid>(Array.ConvertAll<string, Guid>(AppManager.Current.GetActivatedFeatures(this.ID).ToArray(), delegate(string g)
                {
                    return new Guid(g);
                }));
            }));
            t.Start();
        }
    }
}
