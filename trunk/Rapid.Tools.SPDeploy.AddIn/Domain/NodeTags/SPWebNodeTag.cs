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

        public override void DoubleClick()
        {
            //ApplicationUtility.OpenBrowser(Url);
        }

        public override ContextMenu RightClick()
        {
            ContextMenu contextMenu = new ContextMenu();

            List<FeatureManifest> _features = new List<FeatureManifest>();
			
			foreach (FileInfo fileInfo in AppManager.Current.GetFeatureFiles())
                _features.Add(new FeatureManifest(File.ReadAllText(fileInfo.FullName)));

			List<string> _activatedFeatures = AppManager.Current.GetActivatedFeatures(ServerRelativeUrl);

            MenuItem _addFeatureMenu = new MenuItem("Add Feature");
            MenuItem _removeFeatureMenu = new MenuItem("Remove Feature");

            foreach (FeatureManifest featureManifest in _features)
            {
                if (_activatedFeatures.Contains(featureManifest.Id.ToLower()))
                {
                    if (Node.Parent == null || featureManifest.Scope == "Web")
                    {
                        MenuItem _featureMenuItem = new MenuItem(featureManifest.Title);
                        _featureMenuItem.Click += delegate(object s, EventArgs ea)
                            {
                                RapidOutputWindow.Instance.Activate();
                                RapidOutputWindow.Instance.Clear();
								RapidOutputWindow.Instance.Write(SiteTag.AddInService.RemoveFeature(WebID, (Guid)((MenuItem)s).Tag));
                            };
                        _featureMenuItem.Tag = new Guid(featureManifest.Id);
                        _removeFeatureMenu.MenuItems.Add(_featureMenuItem);
                    }
                }
                else
                {
					//if (Array.Find<Proxies.AddIn.Solution>(SiteTag.AddInService.GetSolutions(), delegate(Proxies.AddIn.Solution sol)
					//{
					//    return string.Compare(sol.Name, AppManager.Current.ActiveWspFileName, true) == 0 && sol.Deployed;
					//}) != null)
					//{
					//    if (Node.Parent == null || featureManifest.Scope == "Web")
					//    {
					//        MenuItem _featureMenuItem = new MenuItem(featureManifest.Title);
					//        _featureMenuItem.Click += delegate(object s, EventArgs ea)
					//            {
					//                RapidOutputWindow.Instance.Activate();
					//                RapidOutputWindow.Instance.Clear();
					//                RapidOutputWindow.Instance.Write(SiteTag.AddInService.AddFeature(WebID, (Guid)((MenuItem)s).Tag));

					//            };
					//        _featureMenuItem.Tag = new Guid(featureManifest.Id);
					//        _addFeatureMenu.MenuItems.Add(_featureMenuItem);
					//    }
					//}
                }
            }

            if (_addFeatureMenu.MenuItems.Count > 0)
                contextMenu.MenuItems.Add(_addFeatureMenu);
            if (_removeFeatureMenu.MenuItems.Count > 0)
                contextMenu.MenuItems.Add(_removeFeatureMenu);

            contextMenu.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
            {
				AppManager.Current.OpenBrowser(SiteTag.Url + ServerRelativeUrl);
            });

            return contextMenu;
        }
    }
}
