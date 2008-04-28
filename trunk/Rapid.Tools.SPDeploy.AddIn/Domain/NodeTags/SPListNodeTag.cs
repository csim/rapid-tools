using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using EnvDTE80;
using System.Net;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class SPListNodeTag : WebNodeTag
    {
        public SPListNodeTag(TreeNode node, DTE2 applicationObject)
        {
            _node = node;
            ApplicationObject = applicationObject;
        }

        public override void Action()
        {
            //ApplicationUtility.OpenBrowser(SiteUrl + Url);
        }

        public delegate void VoidDelegate();

        public override ContextMenu GetContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();

            Lists.Lists _listsWebService = new Rapid.Tools.SPDeploy.AddIn.Lists.Lists();
            _listsWebService.Url = SiteUrl + "/_vti_bin/lists.asmx";
            _listsWebService.Credentials = CredentialCache.DefaultNetworkCredentials;

            //if (((NodeTag)_node.Tag).IsDocumentLibrary)
            //{
            //    _contextMenu.MenuItems.Add("Upload", delegate(object sender, EventArgs e)
            //    {
            //        OpenFileDialog _fileDialog = new OpenFileDialog();

            //        if (_fileDialog.ShowDialog() == DialogResult.OK)
            //        {
            //            ServiceInstance.AddFile(((NodeTag)_node.TreeView.Nodes[0].Tag).Url, Guid, _fileDialog.FileName.Substring(_fileDialog.FileName.LastIndexOf("\\") + 1), File.ReadAllBytes(_fileDialog.FileName));
            //        }
            //    });
            //}

            _contextMenu.MenuItems.Add("View List Schema", delegate(object sender, EventArgs e)
            {
                string _schemaPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "\\SPDeploy\\Workspace\\Lists\\", Node.Text, ".xml");

                Domain.Utilties.Functions.EnsurePath(_schemaPath);

                File.WriteAllText(_schemaPath, _listsWebService.GetList(Node.Text).OuterXml);

                ApplicationObject.ItemOperations.OpenFile(_schemaPath, EnvDTE.Constants.vsViewKindTextView);
            });

            _contextMenu.MenuItems.Add("Create Feature", delegate(object sendr, EventArgs e)
            {

                VoidDelegate d = new VoidDelegate(delegate()
                {
                    try
                    {
                        string featureFile = string.Empty;
                        string schemaPath = string.Empty;
                        string _templateName = string.Empty;
                        string _templateDisplayName = string.Empty;
                        int _templateNumber = 6500;


                        UI.Forms.ListDefinitionDialog _listDialog = new Rapid.Tools.SPDeploy.AddIn.UI.Forms.ListDefinitionDialog();
                        _listDialog.ShowDialog();

                        _templateDisplayName = _listDialog.TemplateName;
                        _templateName = string.Join(string.Empty, _templateDisplayName.Split(' '));
                        _templateNumber = _listDialog.TemplateNumber;

                        foreach (string s in ServiceInstance.featureFiles(_listsWebService.GetList(Node.Text).OuterXml))
                        {
                            string projectPath = ApplicationObject.Solution.Projects.Item(1).FullName;
                            projectPath = projectPath.Remove(projectPath.LastIndexOf("\\"));

                            string fPath = s.Substring(s.IndexOf("\\TEMPLATE\\FEATURES\\"));
                            fPath = fPath.Replace("\\TEMPLATE\\FEATURES\\", "");
                            fPath = fPath.Substring(fPath.IndexOf("\\") + 1);
                            fPath = "\\TemplateFiles\\Features\\" + _templateName + "\\" + fPath;

                            Domain.Utilties.Functions.EnsurePath(projectPath + fPath);

                            string tempString = fPath.Substring(fPath.LastIndexOf("\\") + 1);


                            if (tempString.ToLower() == "schema.xml")
                            {
                                string schema = ServiceInstance.GetListSchema(SiteUrl, WebGuid, Guid);


                                XmlDocument oSchema = new XmlDocument();
                                Encoding enc = Encoding.UTF8;
                                oSchema.LoadXml(enc.GetString(ServiceInstance.OpenFile(s)));

                                XmlDocument nSchema = new XmlDocument();
                                nSchema.LoadXml(schema);

                                try
                                {

                                    oSchema.SelectSingleNode("/List/MetaData/Fields").InnerXml = nSchema.SelectSingleNode("/List/Fields").InnerXml;

                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(ServiceInstance.GetViewNodes(SiteUrl, WebGuid, Guid).OuterXml);


                                    XmlNode formNode = oSchema.SelectSingleNode("/List/MetaData/Forms");

                                    int ii = 1;
                                    foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                                    {
                                        XmlElement el = node as XmlElement;
                                        el.RemoveAttribute("Name");
                                        el.Attributes["Url"].Value = el.Attributes["Url"].Value.Substring(el.Attributes["Url"].Value.LastIndexOf("/") + 1);
                                        el.RemoveAttribute("Level");
                                        el.RemoveAttribute("ContentTypeID");
                                        el.SetAttribute("WebPartZoneID", "Main");
                                        el.SetAttribute("SetupPath", "pages\\viewpage.aspx");
                                        el.SetAttribute("BaseViewID", ii.ToString());
                                        ii++;

                                        bool found = false;
                                        foreach (XmlNode n in formNode.SelectNodes("Form"))
                                        {
                                            if (n.Attributes["Url"].Value == el.Attributes["Url"].Value)
                                                found = true;
                                        }

                                        if (!found)
                                            formNode.InnerXml += "<Form Type=\"DisplayForm\" Url=\"" + el.Attributes["Url"].Value + "\" SetupPath=\"pages\\form.aspx\" WebPartZoneID=\"Main\" />";
                                    }

                                    string str = oSchema.SelectSingleNode("/List/MetaData/Views").ChildNodes[0].OuterXml;
                                    str += doc.DocumentElement.InnerXml;

                                    oSchema.SelectSingleNode("/List/MetaData/Views").InnerXml = str;
                                }
                                catch (Exception ex)
                                {
                                }

                                oSchema.Save(projectPath + fPath);

                                schemaPath = projectPath + fPath;

                            }
                            else if (tempString.ToLower() == "feature.xml")
                            {
                                featureFile = projectPath + fPath;
                                XmlDocument fe = new XmlDocument();
                                File.WriteAllBytes(projectPath + fPath, ServiceInstance.OpenFile(s));
                                fe.Load(projectPath + fPath);

                                fe.DocumentElement.Attributes["Id"].Value = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");
                                fe.DocumentElement.Attributes["Title"].Value = _templateDisplayName + " Feature";
                                fe.DocumentElement.Attributes["Description"].Value = "...";

                                fe.Save(featureFile);
                            }
                            else
                                File.WriteAllBytes(projectPath + fPath, ServiceInstance.OpenFile(s));

                        }

                        XmlDocument feature = new XmlDocument();
                        feature.Load(featureFile);

                        string element = feature.DocumentElement.ChildNodes[0].ChildNodes[0].Attributes["Location"].Value;

                        string ns = featureFile.Remove(featureFile.LastIndexOf("\\")) + "\\" + element;

                        element = element.Remove(element.LastIndexOf("\\"));
                        element += "\\" + _templateName + ".xml";
                        feature.DocumentElement.ChildNodes[0].ChildNodes[0].Attributes["Location"].Value = element;
                        feature.Save(featureFile);

                        XmlDocument doc2 = new XmlDocument();
                        doc2.Load(ns);

                        XmlNode nod = doc2.DocumentElement.ChildNodes[0];
                        nod.Attributes["Name"].Value = _templateName;
                        nod.Attributes["Type"].Value = _templateNumber.ToString();
                        nod.Attributes["DisplayName"].Value = _templateDisplayName;
                        nod.Attributes["Description"].Value = "...";

                        File.Delete(ns);
                        ns = ns.Remove(ns.LastIndexOf("\\"));
                        ns = ns + "\\" + _templateName + ".xml";
                        doc2.Save(ns);

                        string newFold = schemaPath.Remove(schemaPath.LastIndexOf("\\"));
                        newFold = newFold.Remove(newFold.LastIndexOf("\\"));

                        DirectoryInfo dir = new DirectoryInfo(newFold);
                        dir = dir.CreateSubdirectory(_templateName);

                        string pp = schemaPath.Remove(schemaPath.LastIndexOf("\\"));

                        string newschemapath = string.Empty;

                        foreach (FileInfo ff in new DirectoryInfo(pp).GetFiles("*"))
                        {
                            File.WriteAllBytes(dir.FullName + "\\" + ff.Name, File.ReadAllBytes(ff.FullName));
                            if (string.Compare(ff.Name, "schema.xml", true) == 0)
                            {
                                newschemapath = dir.FullName + "\\" + ff.Name;
                            }
                        }


                        //File.WriteAllBytes(dir.FullName + "\\schema.xml", File.ReadAllBytes(schemaPath));
                        string tstring = dir.FullName;
                        dir = new DirectoryInfo(schemaPath.Remove(schemaPath.LastIndexOf("\\")));
                        if (dir.FullName != tstring)
                            dir.Delete(true);

                        featureFile = featureFile.Remove(featureFile.LastIndexOf("\\"));


                        dir = new DirectoryInfo(featureFile);
                        foreach (FileInfo fi in dir.GetFiles("*", SearchOption.AllDirectories))
                        {
                            ApplicationObject.Solution.Projects.Item(1).ProjectItems.AddFromFile(fi.FullName);
                        }


                        XmlDocument schemaDocument = new XmlDocument();
                        schemaDocument.Load(newschemapath);


                        schemaDocument.SelectSingleNode("/List/MetaData/ContentTypes").InnerXml = string.Empty;
                        foreach (string s in ServiceInstance.GetContentTypeNames(SiteUrl, WebGuid, Guid))
                        {
                            schemaDocument.SelectSingleNode("/List/MetaData/ContentTypes").InnerXml += s;
                        }


                        SPToolsWebService.ListOptions lo = ServiceInstance.GetOptions(SiteUrl, WebGuid, Guid);
                        if (lo.AllowContentTypes)
                            schemaDocument.DocumentElement.SetAttribute("AllowContentTypes", "true");
                        if (lo.ContentTypesEnabled)
                            schemaDocument.DocumentElement.SetAttribute("ContentTypesEnabled", "true");



                        schemaDocument.Save(newschemapath);



                    }
                    catch (Exception ee)
                    {

                    }
                });

                d.BeginInvoke(null, null);

            });

            _contextMenu.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
            {
                Domain.Utilties.Functions.OpenItem(SiteUrl + Url);
            });

            return _contextMenu;
        }
    }
}
