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
using System.Globalization;

namespace Rapid.Tools.Layouts.Services
{
    // An example callback Web Service API for the Hello2 Web Part
    [WebService(Namespace = "http://ascentium.com/RapidTools")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class AddInWebService : System.Web.Services.WebService
    {

		XmlTextWriter _textWriter;

		[Serializable]
		public class Solution
		{
			public string Name;
			public bool Deployed;
		}

		public enum FileActions
		{
			CheckIn,
			CheckOut,
			Delete,
			UndoCheckOut
		}


		[WebMethod]
		public List<Solution> GetSolutions()
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
		public string AddSolution(string wspName, byte[] wspcontents)
		{
			try
			{
				if (Context.User.Identity.IsAuthenticated)
					AddSolutionWorker(wspName, wspcontents);
				else
					SPSecurity.RunWithElevatedPrivileges(delegate() { AddSolutionWorker(wspName, wspcontents); });

				return "Solution added successfully";
			}
			catch (Exception ex)
			{
				return SPExceptionUtil.Format(ex);
			}
		}

		[WebMethod]
		public string DeploySolution(string wspName)
		{
			try
			{
				if (Context.User.Identity.IsAuthenticated)
					DeploySolutionWorker(wspName);
				else
					SPSecurity.RunWithElevatedPrivileges(delegate() { DeploySolutionWorker(wspName); });

				return "Solution successfully deployed.";
			}
			catch (Exception ex)
			{
				return SPExceptionUtil.Format(ex);
			}
		}

		[WebMethod]
		public string UpgradeSolution(string wspName, byte[] wspcontents)
		{
			try
			{
				if (Context.User.Identity.IsAuthenticated)
					UpgradeSolutionWorker(wspName, wspcontents);
				else
					SPSecurity.RunWithElevatedPrivileges(delegate() { UpgradeSolutionWorker(wspName, wspcontents); });

				return "Solution upgrade successful.";
			}
			catch (Exception ex)
			{
				return SPExceptionUtil.Format(ex);
			}
		}

		[WebMethod]
		public string RetractSolution(string wspName)
		{
			try
			{
				if (Context.User.Identity.IsAuthenticated)
					RetractSolutionWorker(wspName);
				else
					SPSecurity.RunWithElevatedPrivileges(delegate() { RetractSolutionWorker(wspName); });

				return "Solution successfully retracted.";
			}
			catch (Exception ex)
			{
				return SPExceptionUtil.Format(ex);
			}
		}


		[WebMethod]
		public string DeleteSolution(string wspName)
		{
			try
			{
				if (Context.User.Identity.IsAuthenticated)
					DeleteSolutionWorker(wspName);
				else
					SPSecurity.RunWithElevatedPrivileges(delegate() { DeleteSolutionWorker(wspName); });

				return "Solution successfully deleted.";
			}
			catch (Exception ex)
			{
				return SPExceptionUtil.Format(ex);
			}
		}


		[WebMethod]
		public XmlDocument GetSiteStructure()
		{

			string _documentString = string.Empty;

			SPSite site = SPContext.Current.Site;

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

				}
			}

			XmlDocument _document = new XmlDocument();
			_document.LoadXml(_documentString);
			return _document;
		}



        [WebMethod]
        public void DeleteFile(string fileUrl)
        {
			SPSite site = SPContext.Current.Site;
            site.RootWeb.GetFile(fileUrl).Delete();
        }

        [WebMethod]
        public void AddFile(Guid listID, string fileUrl, byte[] bytes)
        {
			SPSite site = SPContext.Current.Site;

            using (SPWeb web = site.RootWeb)
            {
                SPFile file = web.Lists[listID].RootFolder.Files.Add(fileUrl, bytes, true);
                file.Update();
            }
        }

        [WebMethod]
        public bool IsCheckedOut(Guid webID, Guid fileID)
        {
			SPSite site = SPContext.Current.Site;
            return GetFile(site, webID, fileID).CheckOutStatus != SPFile.SPCheckOutStatus.None;
        }

        [WebMethod]
        public string GetFileInfo(Guid webID, Guid fileID)
        {
			SPSite site = SPContext.Current.Site;

            string st = string.Empty;
            SPFile file = GetFile(site, webID, fileID);
            foreach (string s in file.Properties.Keys)
            {
                st += s + ": " + file.Properties[s] + "\r\n";
            }
            return st;
        }



		private SPFile GetFile(SPSite site, Guid webID, Guid fileID)
        {
            SPFile file = null;

			using (SPWeb web = site.AllWebs[webID])
            {
                file = web.GetFile(fileID);
            }
            return file;
        }


        [WebMethod]
        public List<string> Views(Guid webID, Guid listID)
        {
			SPSite site = SPContext.Current.Site;

            List<string> views = new List<string>();
            foreach (SPView view in site.AllWebs[webID].Lists[listID].Views)
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
        public byte[] OpenBinary(Guid webID, Guid fileID)
        {
			SPSite site = SPContext.Current.Site;
            return GetFile(site, webID, fileID).OpenBinary();
        }

        [WebMethod]
        public bool SaveBinary(Guid webID, Guid fileID, byte[] bytes)
        {
            try
            {
				SPSite site = SPContext.Current.Site;
                GetFile(site, webID, fileID).SaveBinary(bytes);
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

		public void ExecuteAdminJobs(string wspName)
		{
			//SPRunningJobCollection jobs = SPWebService.AdministrationService.RunningJobs;

			//uint lcid = 1033;
			//string jobname = "solution-deployment-" + ((wspName.Length >= 50) ? wspName.Substring(0, 50) : wspName) + "-" + lcid.ToString(NumberFormatInfo.InvariantInfo);

			//SPAdministrationServiceJobDefinition job = SPFarm.Local.TimerService.JobDefinitions.GetValue<SPAdministrationServiceJobDefinition>(jobname);

			//if (job != null)
			//{
			//    job.Execute(job.Service.Id);

			//}

		}


        [WebMethod]
        public string AddFeature( Guid webID, Guid featureId)
        {
            try
            {
				SPSite site = SPContext.Current.Site;
                if (SPFarm.Local.FeatureDefinitions[featureId].Scope == SPFeatureScope.Site)
                {
                    site.Features.Add(featureId);
                }
                else
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {
                        web.Features.Add(featureId, true);
                    }
                }

				return "Feature added successfully.";
            }
            catch (Exception ex)
            {
				return SPExceptionUtil.Format(ex);
            }
        }

        [WebMethod]
        public string RemoveFeature(Guid webID, Guid featureId)
        {
            try
            {
				SPSite site = SPContext.Current.Site;

                if (SPFarm.Local.FeatureDefinitions[featureId].Scope == SPFeatureScope.Site)
                {
                    site.Features.Remove(featureId);
                }
                else
                {
                    using (SPWeb web = site.AllWebs[webID])
                    {
                        web.Features.Remove(featureId, true);
                    }
                }

				return "Feature removed successfully.";
            }
            catch (Exception ex)
            {
				return SPExceptionUtil.Format(ex);
            }

        }


        [WebMethod]
        public bool PerformFileAction(Guid webID, Guid fileID, FileActions action)
        {
			SPSite site = SPContext.Current.Site;

            SPFile file = GetFile(site, webID, fileID);
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
        public string getInfo(Guid webUid, Guid listID)
        {
			SPSite site = SPContext.Current.Site;

            using (SPWeb web = site.AllWebs[webUid])
            {
                SPList list = web.Lists[listID];
                return list.Forms.SchemaXml;
            }
        }

        [WebMethod]
        public byte[] OpenBinaryFile(Guid webID, Guid fileID)
        {
			SPSite site = SPContext.Current.Site;

            using (SPWeb web = site.AllWebs[webID])
            {
                return web.GetFile(fileID).OpenBinary();
            }
        }

        [Serializable]
        public class ListOptions
        {
            public bool AllowContentTypes;
            public bool ContentTypesEnabled;
        }



        [WebMethod]
        public ListOptions GetOptions(Guid webUid, Guid listID)
        {
            ListOptions o = new ListOptions();
			
			SPSite site = SPContext.Current.Site;

			using (SPWeb web = site.AllWebs[webUid])
            {
                SPList list = web.Lists[listID];
                if (list.AllowContentTypes)
                    o.AllowContentTypes = true;
                if (list.ContentTypesEnabled)
                    o.ContentTypesEnabled = true;
            }

			return o;
        }

        [WebMethod]
        public string GetViewSchema(Guid webUid, Guid listID, string viewNAme)
        {
			SPSite site = SPContext.Current.Site;

            using (SPWeb web = site.AllWebs[webUid])
            {
                SPList list = web.Lists[listID];
                return list.Views[viewNAme].HtmlSchemaXml;
            }
        }

        [WebMethod]
        public XmlDocument GetViewNodes(Guid webID, Guid listID)
        {
            XmlDocument document = new XmlDocument();
			SPSite site = SPContext.Current.Site;

            using (SPWeb web = site.AllWebs[webID])
            {
                SPList list = web.Lists[listID];

                string content = "<Views>";


                foreach (SPView view in list.Views)
                {
                    content += view.HtmlSchemaXml;
                }

                content += "</Views>";

                document.LoadXml(content);
            }
            return document;
        }

        [WebMethod]
        public string GetContentTypeName(string contentTypeId)
        {
            string name = string.Empty;
			SPSite site = SPContext.Current.Site;

            SPContentTypeId id = new SPContentTypeId(contentTypeId);
            SPContentType t = site.RootWeb.ContentTypes[id];
            name = t.Name + t.FieldLinks[0];

			return name;
        }

        [WebMethod]
        public List<string> GetContentTypeNames(Guid webID, Guid listID)
        {
            List<string> s = new List<string>();
			SPSite site = SPContext.Current.Site;

            using (SPWeb web = site.AllWebs[webID])
            {
                SPList list = web.Lists[listID];
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

			return s;
        }


        [WebMethod]
        public string GetListSchema(Guid webUid, Guid listID)
        {
			SPSite site = SPContext.Current.Site; 
			
			using (SPWeb web = site.AllWebs[webUid])
            {
                SPList list = web.Lists[listID];
                return list.SchemaXml;
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
        public void UpdateViewSchema(Guid webUid, Guid listID, string viewNAme, string schema)
        {
			SPSite site = SPContext.Current.Site;

            using (SPWeb web = site.AllWebs[webUid])
            {
                SPList list = web.Lists[listID];
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


        [WebMethod]
        public byte[] OpenFile(string filePath)
        {
            return System.IO.File.ReadAllBytes(filePath);
        }



		private void AddSolutionWorker(string wspName, byte[] wspcontents)
		{
			string tdir = Path.GetDirectoryName(Path.GetTempFileName());
			string wsppath = string.Format(@"{0}\{1}", tdir, wspName);

			if (File.Exists(wsppath))
				File.Delete(wsppath);

			File.WriteAllBytes(wsppath, wspcontents);

			SPFarm.Local.Solutions.Add(wsppath);

			File.Delete(wsppath);
		}

		private void DeploySolutionWorker(string wspName)
		{
			SPSite site = SPContext.Current.Site;

			SPSolution solution = SPFarm.Local.Solutions[wspName];
			Collection<SPWebApplication> webapps = new Collection<SPWebApplication>();
			webapps.Add(site.WebApplication);

			if (solution.ContainsWebApplicationResource)
				solution.DeployLocal(true, webapps, true);
			else
				solution.DeployLocal(true, true);

			ExecuteAdminJobs(wspName);
		}


		private void UpgradeSolutionWorker(string wspName, byte[] wspcontents)
		{
			SPSolution solution = SPFarm.Local.Solutions[wspName];

			string tdir = Path.GetDirectoryName(Path.GetTempFileName());
			string wsppath = string.Format(@"{0}\{1}", tdir, wspName);

			if (File.Exists(wsppath))
				File.Delete(wsppath);

			File.WriteAllBytes(wsppath, wspcontents);

			solution.Upgrade(wsppath);
			ExecuteAdminJobs(wspName);

			File.Delete(wsppath);
		}


		private void RetractSolutionWorker(string wspName)
		{
			SPSite site = SPContext.Current.Site;

			SPSolution solution = SPFarm.Local.Solutions[wspName];
			Collection<SPWebApplication> webapps = new Collection<SPWebApplication>();
			webapps.Add(site.WebApplication);

			if (solution.ContainsWebApplicationResource)
				solution.RetractLocal(webapps);
			else
				solution.RetractLocal();

			ExecuteAdminJobs(wspName);
		}

		private void DeleteSolutionWorker(string wspName)
		{
			SPSolution solution = SPFarm.Local.Solutions[wspName];
			solution.Delete();
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



    }

	internal class SPAdministrationServiceJobDefinitionCollection : SPPersistedChildCollection<SPAdministrationServiceJobDefinition>
	{
		
		internal SPAdministrationServiceJobDefinitionCollection(SPService service) : base(service)
		{
		}

		internal SPAdministrationServiceJobDefinitionCollection(SPWebApplication webApplication) : base(webApplication)
		{
		}
	}


 

}
