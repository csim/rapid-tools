using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using Microsoft.SharePoint;
using System.Security.Permissions;
using Microsoft.SharePoint.Administration;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Collections.ObjectModel;
using Rapid.Tools.Utilities;

namespace Rapid.Tools.Layouts.Services
{
    // An example callback Web Service API for the Hello2 Web Part
    [WebService(Namespace = "http://ascentium.com/RapidTools")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class AddInWebService : System.Web.Services.WebService
    {

        XmlTextWriter _textWriter;

        [WebMethod]
        public XmlDocument GetSiteStructure(string siteUrl)
        {
            string _documentString = string.Empty;
            using (SPSite site = new SPSite(siteUrl))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (_textWriter = new XmlTextWriter(ms, Encoding.UTF8))
                    {
                        _textWriter.WriteStartDocument();
                        _textWriter.WriteStartElement("Site");
                        _textWriter.WriteAttributeString("Url", site.Url);
                        _textWriter.WriteStartElement("Webs");
                        foreach (SPWeb web in site.AllWebs)
                        {
                            AddWebNode(web);
                        }
                        _textWriter.WriteEndElement();
                        _textWriter.WriteEndDocument();
                        _textWriter.Flush();
                        ms.Position = 0;
                        using (StreamReader sr = new StreamReader(ms))
                        {
                            _documentString = sr.ReadToEnd();
                        }
                        _textWriter.Close();
                    }
                    ms.Close();
                }
            }

            XmlDocument _document = new XmlDocument();
            _document.LoadXml(_documentString);
            return _document;
        }




        private void AddWebNode(SPWeb web)
        {
            _textWriter.WriteStartElement("Web");
            _textWriter.WriteAttributeString("Title", web.Title);
            _textWriter.WriteAttributeString("Url", web.Url);
            _textWriter.WriteAttributeString("Guid", web.ID.ToString());

			//PublishingWeb.IsPublishingWeb(web)
			// TODO: change to MOSS publishing web feature
			bool ispubweb = SPFeatureUtil.FeatureActivated(web, Guid.Empty);

            _textWriter.WriteAttributeString("Publishing", ispubweb.ToString());
            if (web.Webs.Count > 0)
            {
                _textWriter.WriteStartElement("Webs");
                foreach (SPWeb subweb in web.Webs)
                {
                    AddWebNode(subweb);
                }
                _textWriter.WriteEndElement();
            }
            if (web.Lists.Count > 0)
            {
                _textWriter.WriteStartElement("Lists");
                foreach (SPList list in web.Lists)
                {
                    AddListNode(list);
                }
                _textWriter.WriteEndElement();
            }
            _textWriter.WriteStartElement("Files");
            foreach (SPFile file in web.Files)
            {
                AddFileNodes(file);
            }
            _textWriter.WriteEndElement();
            _textWriter.WriteEndElement();
        }
        private void AddListNode(SPList list)
        {

            _textWriter.WriteStartElement("List");
            _textWriter.WriteAttributeString("Title", list.Title);
            _textWriter.WriteAttributeString("Type", list.BaseType.ToString());
            _textWriter.WriteAttributeString("Url", list.DefaultViewUrl);
            _textWriter.WriteAttributeString("ContentType", list.ContentTypes[0].ToString());
            _textWriter.WriteAttributeString("Hidden", list.Hidden.ToString());
            _textWriter.WriteAttributeString("Guid", list.ID.ToString());


            _textWriter.WriteStartElement("Views");
            foreach (SPView view in list.Views)
            {
                AddViewNode(view);
            }
            _textWriter.WriteEndElement();


            _textWriter.WriteStartElement("Folders");
            foreach (SPFolder folder in list.RootFolder.SubFolders)
            {
                AddFolderNodes(folder);
            }
            _textWriter.WriteEndElement();

            if (list.Items.Count > 0 && list.BaseType != SPBaseType.DocumentLibrary)
            {
                _textWriter.WriteStartElement("Items");
                foreach (SPListItem li in list.Items)
                {
                    AddItemNode(li);
                }
                _textWriter.WriteEndElement();
            }
            if (true)//(list.BaseType == SPBaseType.DocumentLibrary)
            {
                _textWriter.WriteStartElement("Files");
                foreach (SPFile file in list.RootFolder.Files)
                {
                    AddFileNodes(file);
                }
                _textWriter.WriteEndElement();
            }
            _textWriter.WriteEndElement();
        }

        private void AddViewNode(SPView view)
        {
            _textWriter.WriteStartElement("View");
            _textWriter.WriteAttributeString("Title", view.Title);
            _textWriter.WriteAttributeString("Guid", view.ID.ToString());
            _textWriter.WriteAttributeString("Url", view.Url);
            _textWriter.WriteEndElement();
        }
        private void AddItemNode(SPListItem li)
        {
            _textWriter.WriteStartElement("Item");
            _textWriter.WriteAttributeString("Url", li.Url);
            _textWriter.WriteAttributeString("Guid", li.UniqueId.ToString());
            _textWriter.WriteAttributeString("Name", li.Name);
            _textWriter.WriteEndElement();
        }
        private void AddFolderNodes(SPFolder folder)
        {
            _textWriter.WriteStartElement("Folder");
            _textWriter.WriteAttributeString("Title", folder.Name);
            _textWriter.WriteAttributeString("Url", folder.Url);
            if (folder.SubFolders.Count > 0)
            {
                _textWriter.WriteStartElement("Folders");
                foreach (SPFolder subFolder in folder.SubFolders)
                {
                    AddFolderNodes(subFolder);
                }
                _textWriter.WriteEndElement();
            }
            if (folder.Files.Count > 0)
            {
                _textWriter.WriteStartElement("Files");
                foreach (SPFile file in folder.Files)
                {
                    AddFileNodes(file);
                }
                _textWriter.WriteEndElement();
            }
            _textWriter.WriteEndElement();
        }
        private void AddFileNodes(SPFile file)
        {
            _textWriter.WriteStartElement("File");
            _textWriter.WriteAttributeString("Title", file.Title);
            _textWriter.WriteAttributeString("Name", file.Name);
            _textWriter.WriteAttributeString("Url", file.Url);
            _textWriter.WriteAttributeString("Customized", file.CustomizedPageStatus.ToString());
            _textWriter.WriteAttributeString("Guid", file.UniqueId.ToString());
            _textWriter.WriteAttributeString("CheckedOut", Convert.ToString(file.CheckOutStatus != SPFile.SPCheckOutStatus.None));
            _textWriter.WriteEndElement();
        }


        [WebMethod]
        public void DeleteFile(string siteUrl, string fileUrl)
        {
            using (SPSite site = new SPSite(siteUrl))
            {
                site.RootWeb.GetFile(fileUrl).Delete();
            }
        }

        [WebMethod]
        public void AddFile(string siteUrl, Guid listGuid, string fileUrl, byte[] bytes)
        {
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.RootWeb)
                {
                    SPFile file = web.Lists[listGuid].RootFolder.Files.Add(fileUrl, bytes, true);
                    file.Update();
                }
            }
        }

        [WebMethod]
        public bool IsCheckedOut(string siteUrl, Guid webGuid, Guid fileGuid)
        {
            return GetFile(siteUrl, webGuid, fileGuid).CheckOutStatus != SPFile.SPCheckOutStatus.None;
        }

        [WebMethod]
        public string GetFileInfo(string siteUrl, Guid webGuid, Guid fileGuid)
        {
            string st = string.Empty;
            SPFile file = GetFile(siteUrl, webGuid, fileGuid);
            foreach (string s in file.Properties.Keys)
            {
                st += s + ": " + file.Properties[s] + "\r\n";
            }
            return st;
        }



        private SPFile GetFile(string siteUrl, Guid webGuid, Guid fileGuid)
        {
            SPFile file = null;
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.AllWebs[webGuid])
                {
                    file = web.GetFile(fileGuid);
                }
            }
            return file;
        }


        [WebMethod]
        public List<string> Views(string siteUrl, Guid webGuid, Guid listGuid)
        {
            SPSite site = new SPSite(siteUrl);
            List<string> views = new List<string>();
            foreach (SPView view in site.AllWebs[webGuid].Lists[listGuid].Views)
            {
                views.Add(view.Title);
            }
            return views;
        }

        [WebMethod]
        public List<string> featureFiles(string listSchema)
        {
            List<string> files = new List<string>();
            XmlDocument document = new XmlDocument();
            document.LoadXml(listSchema);

            string featureId = document.DocumentElement.Attributes["FeatureId"].Value;

            // SPFarm f = SPFarm.Local;
            //SPFeatureDefinition fd = f.FeatureDefinitions[new Guid(featureId)];
            //fd.RootDirectory


            DirectoryInfo di = new DirectoryInfo(@"c:\program files\common files\microsoft shared\web server extensions\12");

            foreach (FileInfo fi in di.GetFiles("feature.xml", SearchOption.AllDirectories))
            {
                document.Load(fi.FullName);
                if (document.DocumentElement.Attributes["Id"].Value.ToLower() == featureId.ToLower())
                {
                    di = new DirectoryInfo(fi.FullName.Remove(fi.FullName.LastIndexOf("\\")));
                    foreach (FileInfo f in di.GetFiles("*", SearchOption.AllDirectories))
                    {
                        files.Add(f.FullName);
                    }
                    break;
                }
            }
            return files;
        }

        [WebMethod]
        public byte[] OpenBinary(string siteUrl, Guid webGuid, Guid fileGuid)
        {
            return GetFile(siteUrl, webGuid, fileGuid).OpenBinary();
        }

        [WebMethod]
        public bool SaveBinary(string siteUrl, Guid webGuid, Guid fileGuid, byte[] bytes)
        {
            try
            {
                GetFile(siteUrl, webGuid, fileGuid).SaveBinary(bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod]
        public List<string> GetInstalledFeatues(List<Guid> featureIds)
        {
            List<string> features = new List<string>();
            foreach (SPFeatureDefinition def in SPFarm.Local.FeatureDefinitions)
            {
                if (featureIds.Contains(def.Id))
                    features.Add(def.GetXmlDefinition(System.Globalization.CultureInfo.InvariantCulture).OuterXml);
            }
            return features;
        }

        [WebMethod]
        public string UpgradeSolution(string solution, string filePath)
        {
            try
            {
                SPFarm.Local.Solutions[solution].Upgrade(filePath);
                ExecuteJobDefinitions();
                return "Solution upgrade successful.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        [WebMethod]
        public string SaveFile(string filePath, byte[] contents)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo("C:\\_spdeploy");

                string path = filePath.Remove(filePath.LastIndexOf("\\"));

                di.CreateSubdirectory(path);

                File.WriteAllBytes("c:\\_spdeploy\\" + filePath, contents);
                return "File saved successfully";
            }
            catch (Exception ex) { return ex.Message; }
        }

        [WebMethod]
        public string AddSolution(string filePath)
        {
            try
            {
                SPFarm.Local.Solutions.Add(filePath);
                return "Solution added successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Serializable]
        public class Solution
        {
            public string Name;
            public bool Deployed;
        }

        [WebMethod]
        public List<string> GetSolutions()
        {
            List<string> sols = new List<string>();
            foreach (SPSolution sol in SPFarm.Local.Solutions)
            {
                sols.Add(sol.Name);
            }
            return sols;
        }

        [WebMethod]
        public List<Solution> GetSols()
        {
            List<Solution> sols = new List<Solution>();
            foreach (SPSolution sol in SPFarm.Local.Solutions)
            {
                Solution s = new Solution();
                s.Name = sol.Name;
                s.Deployed = sol.Deployed;
                sols.Add(s);
            }
            return sols;
        }

        [WebMethod]
        public string DeploySolution(string solutionName)
        {
            try
            {
                SPSolution solution = SPFarm.Local.Solutions[solutionName];
                Collection<SPWebApplication> collection = new Collection<SPWebApplication>();
                foreach (SPWebApplication app in SPWebService.ContentService.WebApplications)
                    collection.Add(app);
                if (solution.ContainsWebApplicationResource)
                    solution.DeployLocal(true, collection, true);
                else
                    solution.DeployLocal(true, true);

                ExecuteJobDefinitions();
                return "Solution successfully deployed.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string RetractSolution(string solutionName)
        {
            try
            {
                SPSolution solution = SPFarm.Local.Solutions[solutionName];
                Collection<SPWebApplication> collection = new Collection<SPWebApplication>();
                foreach (SPWebApplication app in SPWebService.ContentService.WebApplications)
                    collection.Add(app);
                if (solution.ContainsWebApplicationResource)
                    solution.RetractLocal(collection);
                else
                    solution.RetractLocal();
                ExecuteJobDefinitions();
                return "Solution successfully retracted.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void ExecuteJobDefinitions()
        {
            try
            {
                if (SPFarm.Local.TimerService.JobDefinitions != null)
                {
                    foreach (SPJobDefinition job in SPFarm.Local.TimerService.JobDefinitions)
                    {
                        try
                        {
                            job.Execute(SPServer.Local.Id);
                        }
                        catch { }
                    }
                }
            }
            catch { }
        }

        [WebMethod]
        public string DeleteSolution(string solutionName)
        {
            try
            {
                SPSolution solution = SPFarm.Local.Solutions[solutionName];
                solution.Delete();
                return "Solution successfully deleted.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        [WebMethod]
        public string AddFeature(string siteUrl, Guid webGuid, Guid featureId)
        {
            try
            {
                using (SPSite site = new SPSite(siteUrl))
                {
                    if (SPFarm.Local.FeatureDefinitions[featureId].Scope == SPFeatureScope.Site)
                    {
                        site.Features.Add(featureId);
                    }
                    else
                    {
                        using (SPWeb web = site.AllWebs[webGuid])
                        {
                            web.Features.Add(featureId, true);
                        }
                    }
                }
                return "Feature added successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [WebMethod]
        public string RemoveFeature(string siteUrl, Guid webGuid, Guid featureId)
        {
            try
            {
                using (SPSite site = new SPSite(siteUrl))
                {
                    if (SPFarm.Local.FeatureDefinitions[featureId].Scope == SPFeatureScope.Site)
                    {
                        site.Features.Remove(featureId);
                    }
                    else
                    {
                        using (SPWeb web = site.AllWebs[webGuid])
                        {
                            web.Features.Remove(featureId, true);
                        }
                    }
                }
                return "Feature removed successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


        [WebMethod]
        public bool PerformFileAction(string siteUrl, Guid webGuid, Guid fileGuid, FileActions action)
        {
            SPFile file = GetFile(siteUrl, webGuid, fileGuid);
            try
            {
                switch (action)
                {
                    case FileActions.CheckIn:
                        file.CheckIn("SPDeploy Check In");
                        break;
                    case FileActions.CheckOut:
                        file.CheckOut();
                        break;
                    case FileActions.Delete:
                        file.Delete();
                        break;
                    case FileActions.UndoCheckOut:
                        file.UndoCheckOut();
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        [WebMethod]
        public string getInfo(string siteUrl, Guid webUid, Guid listGuid)
        {
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.AllWebs[webUid])
                {
                    SPList list = web.Lists[listGuid];
                    return list.Forms.SchemaXml;
                }
            }
        }

        [WebMethod]
        public byte[] OpenBinaryFile(string siteUrl, Guid webGuid, Guid fileGuid)
        {
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.AllWebs[webGuid])
                {
                    return web.GetFile(fileGuid).OpenBinary();
                }
            }
        }

        [Serializable]
        public class ListOptions
        {
            public bool AllowContentTypes;
            public bool ContentTypesEnabled;
        }



        [WebMethod]
        public ListOptions GetOptions(string siteUrl, Guid webUid, Guid listGuid)
        {
            ListOptions o = new ListOptions();
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.AllWebs[webUid])
                {
                    SPList list = web.Lists[listGuid];
                    if (list.AllowContentTypes)
                        o.AllowContentTypes = true;
                    if (list.ContentTypesEnabled)
                        o.ContentTypesEnabled = true;
                }
            }
            return o;
        }

        [WebMethod]
        public string GetViewSchema(string siteUrl, Guid webUid, Guid listGuid, string viewNAme)
        {
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.AllWebs[webUid])
                {
                    SPList list = web.Lists[listGuid];
                    return list.Views[viewNAme].HtmlSchemaXml;
                }
            }
        }

        [WebMethod]
        public XmlDocument GetViewNodes(string siteUrl, Guid webGuid, Guid listGuid)
        {
            XmlDocument document = new XmlDocument();
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.AllWebs[webGuid])
                {
                    SPList list = web.Lists[listGuid];

                    string content = "<Views>";


                    foreach (SPView view in list.Views)
                    {
                        content += view.HtmlSchemaXml;
                    }

                    content += "</Views>";

                    document.LoadXml(content);
                }
            }
            return document;
        }

        [WebMethod]
        public string GetContentTypeName(string siteUrl, string contentTypeId)
        {
            string name = string.Empty;
            using (SPSite site = new SPSite(siteUrl))
            {
                SPContentTypeId id = new SPContentTypeId(contentTypeId);
                SPContentType t = site.RootWeb.ContentTypes[id];
                name = t.Name + t.FieldLinks[0];
            }
            return name;
        }

        [WebMethod]
        public List<string> GetContentTypeNames(string siteUrl, Guid webGuid, Guid listGuid)
        {
            List<string> s = new List<string>();
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.AllWebs[webGuid])
                {
                    SPList list = web.Lists[listGuid];
                    foreach (SPContentType ct in list.ContentTypes)
                    {
                        try
                        {
                            XmlDocument schemaDoc = new XmlDocument();
                            schemaDoc.LoadXml(ct.SchemaXml);

                            schemaDoc.DocumentElement.RemoveChild(schemaDoc.SelectSingleNode("/ContentType/Fields"));

                            XmlElement el = schemaDoc.CreateElement("FieldRefs");
                            foreach (SPFieldLink l in ct.FieldLinks)
                            {
                                el.InnerXml += l.SchemaXml;
                            }
                            schemaDoc.DocumentElement.PrependChild(el);
                            s.Add(schemaDoc.OuterXml);

                        }
                        catch (Exception ex)
                        {
                            s.Add(ex.Message);
                        }
                    }
                }
            }
            return s;
        }


        [WebMethod]
        public string GetListSchema(string siteUrl, Guid webUid, Guid listGuid)
        {
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.AllWebs[webUid])
                {
                    SPList list = web.Lists[listGuid];
                    return list.SchemaXml;
                }
            }
        }


        [WebMethod]
        public byte[] CompareFeatureFile(string relativeFilePath)
        {
            string path = @"c:\program files\common files\microsoft shared\web server extensions\12\TEMPLATE\FEATURES\" + relativeFilePath;
            if (!File.Exists(path))
                return null;
            else
                return File.ReadAllBytes(path);            
        }


        [WebMethod]
        public void UpdateViewSchema(string siteUrl, Guid webUid, Guid listGuid, string viewNAme, string schema)
        {
            using (SPSite site = new SPSite(siteUrl))
            {
                using (SPWeb web = site.AllWebs[webUid])
                {
                    SPList list = web.Lists[listGuid];
                    SPView view = list.Views[viewNAme];

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(schema);

                    view.ViewFields.DeleteAll();

                    foreach (XmlNode node in doc.DocumentElement.SelectNodes("ViewFields/FieldRef"))
                    {
                        view.ViewFields.Add(node.Attributes["Name"].Value);
                    }

                    try
                    {
                        view.Toolbar = doc.SelectSingleNode("/View/Toolbar").InnerXml;
                        view.GroupByHeader = doc.SelectSingleNode("/View/GroupByHeader").InnerXml;
                        view.GroupByFooter = doc.SelectSingleNode("/View/GroupByFooter").InnerXml;
                        view.ViewHeader = doc.SelectSingleNode("/View/ViewHeader").InnerXml;
                        view.ViewBody = doc.SelectSingleNode("/View/ViewBody").InnerXml;
                        view.ViewFooter = doc.SelectSingleNode("/View/ViewFooter").InnerXml;
                        view.Paged = Convert.ToBoolean(doc.SelectSingleNode("/View/RowLimit").Attributes["Paged"].Value);
                        view.RowLimit = Convert.ToUInt32(doc.SelectSingleNode("/View/RowLimit").InnerText);
                        view.ViewEmpty = doc.SelectSingleNode("/View/ViewEmpty").InnerXml;
                        view.Query = doc.SelectSingleNode("/View/Query").InnerXml;


                        view.Update();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }


        [WebMethod]
        public byte[] OpenFile(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath);
        }

        public enum FileActions
        {
            CheckIn,
            CheckOut,
            Delete,
            UndoCheckOut
        }


    }
}
