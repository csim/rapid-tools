using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using System.IO;
using System.Xml;
using EnvDTE80;
using Rapid.Tools.SPDeploy.AddIn.UI.Controls;
using System.Net;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.Utilties.ContextMenus
{

    public class WebContextMenu : ContextMenu
    {
        private SPToolsWebService.SPToolsWebService _serviceInstance;

        public SPToolsWebService.SPToolsWebService ServiceInstance
        {
            get
            {
                if (_serviceInstance == null)
                {
                    _serviceInstance = new Rapid.Tools.SPDeploy.AddIn.SPToolsWebService.SPToolsWebService();
                    _serviceInstance.Url = string.Concat(Domain.Utilties.Functions.GetSiteUrlFromProject(ApplicationObject), "/_layouts/SPTools/SPToolsWebService.asmx");
                    _serviceInstance.Credentials = CredentialCache.DefaultCredentials;
                }
                return _serviceInstance;
            }
        }

        private DTE2 ApplicationObject;

        public WebContextMenu(TreeNode node, DTE2 applicationObject)
        {
            ApplicationObject = applicationObject;
            string Url = ((NodeTag)node.Tag).Url;
            List<FeatureManifest> _manifests = new List<FeatureManifest>();

            foreach (FileInfo fileInfo in Domain.Utilties.Functions.GetProjectDirectory(ApplicationObject).GetFiles("feature.xml", SearchOption.AllDirectories))
            {
                XmlDocument _tempDoc = new XmlDocument();
                _tempDoc.Load(fileInfo.FullName);
                FeatureManifest man = new FeatureManifest(_tempDoc);
                _manifests.Add(man);
            }

            MenuItem _addFeatureMenu = new MenuItem("Add Feature");
            MenuItem _removeFeatureMenu = new MenuItem("Remove Feature");

            List<string> _activatedFeatures = Domain.Utilties.Functions.GetActivatedFeatures(Url);

            foreach (FeatureManifest featureManifest in _manifests)
            {
                if (_activatedFeatures.Contains(featureManifest.Id.ToLower()))
                {
                    if (node.Parent == null || featureManifest.Scope == "Web")
                    {
                        MenuItem _featureMenuItem = new MenuItem(featureManifest.Title);
                        _featureMenuItem.Click += delegate(object s, EventArgs ea)
                            {                                
                                //RapidOutputWindow.Activate();
                                //RapidOutputWindow.Clear();
                                //RapidOutputWindow.Write(ServiceInstance.RemoveFeature(siteUrl, _guid, (Guid)((MenuItem)s).Tag));
                            };
                        _featureMenuItem.Tag = new Guid(featureManifest.Id);
                        _removeFeatureMenu.MenuItems.Add(_featureMenuItem);
                    }
                }
                else
                {
                    if (Array.Find<SPToolsWebService.Solution>(ServiceInstance.GetSols(), delegate(SPToolsWebService.Solution sol)
                    {
                        return string.Compare(sol.Name, ApplicationObject.Solution.Projects.Item(1).Name + ".wsp", true) == 0 && sol.Deployed;
                    }) != null)
                    {
                        if (node.Parent == null || featureManifest.Scope == "Web")
                        {
                            MenuItem _featureMenuItem = new MenuItem(featureManifest.Title);
                            _featureMenuItem.Click += delegate(object s, EventArgs ea)
                                {
                                    //RapidOutputWindow.Activate();
                                    //RapidOutputWindow.Clear();
                                    //RapidOutputWindow.Write(ServiceInstance.AddFeature(siteUrl, _guid, (Guid)((MenuItem)s).Tag));

                                };
                            _featureMenuItem.Tag = new Guid(featureManifest.Id);
                            _addFeatureMenu.MenuItems.Add(_featureMenuItem);
                        }
                    }
                }
            }

            if (_addFeatureMenu.MenuItems.Count > 0)
                this.MenuItems.Add(_addFeatureMenu);
            if (_removeFeatureMenu.MenuItems.Count > 0)
                this.MenuItems.Add(_removeFeatureMenu);

            this.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
            {
                Domain.Utilties.Functions.OpenItem(Url);
            });         
        }
    }
}
