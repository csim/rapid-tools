using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Security.Permissions;
using Microsoft.SharePoint.Administration;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Collections.ObjectModel;
using Rapid.Tools.Utilities;
using System.Globalization;
using System.Reflection;

namespace Rapid.Tools.Layouts.Services
{
    // An example callback Web Service API for the Hello2 Web Part
    [WebService(Namespace = "http://ascentium.com/RapidTools")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
	public class AddInWebService : System.Web.Services.WebService
    {

		XmlTextWriter _structureWriter;

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
				using (_structureWriter = new XmlTextWriter(ms, Encoding.UTF8))
				{

					AddSiteNode(site);

					_structureWriter.Flush();
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

			DirectoryInfo di = new DirectoryInfo(SPUtility.GetGenericSetupPath(""));

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
		public void UpdateViewSchema(Guid webUid, Guid listID, string viewNAme, XmlDocument document)
		{
			SPSite site = SPContext.Current.Site;
			using (SPWeb web = site.AllWebs[webUid])
			{
				SPView view = web.Lists[listID].Views[viewNAme];
				typeof(SPView).GetMethod("EnsureFullBlownXmlDocument", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(view, null);
				typeof(SPView).GetField("m_xdView", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, document);
				view.Update();
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

		private void AddSiteNode(SPSite site)
		{
			_structureWriter.WriteStartDocument();
			_structureWriter.WriteStartElement("Site");
			_structureWriter.WriteAttributeString("ID", site.ID.ToString());
			_structureWriter.WriteAttributeString("Url", site.Url);

			AddWebNode(site.RootWeb);

			_structureWriter.WriteEndDocument();

		}

		private void AddWebNode(SPWeb web)
		{
			_structureWriter.WriteStartElement("Web");
			_structureWriter.WriteAttributeString("ID", web.ID.ToString());
			_structureWriter.WriteAttributeString("Title", web.Title);
			_structureWriter.WriteAttributeString("ServerRelativeUrl", web.ServerRelativeUrl);

			//PublishingWeb.IsPublishingWeb(web)
			// TODO: change to MOSS publishing web feature
			bool ispubweb = SPFeatureUtil.FeatureActivated(web, Guid.Empty);

			_structureWriter.WriteAttributeString("Publishing", ispubweb.ToString());

			foreach (SPFolder folder in web.Folders) 
				AddFolderNode(folder);

			foreach (SPFile file in web.Files)			
				AddFileNode(file);

			_structureWriter.WriteEndElement();
		}

		private void AddListNode(SPList list)
		{

			_structureWriter.WriteStartElement("List");
			_structureWriter.WriteAttributeString("ID", list.ID.ToString());
			_structureWriter.WriteAttributeString("Title", list.Title);
			_structureWriter.WriteAttributeString("Type", list.BaseType.ToString());
			_structureWriter.WriteAttributeString("ServerRelativeUrl", list.DefaultViewUrl);
			_structureWriter.WriteAttributeString("ContentType", list.ContentTypes[0].ToString());
			_structureWriter.WriteAttributeString("Hidden", list.Hidden.ToString());

			foreach (SPView view in list.Views)							
				AddViewNode(view);
			
			foreach (SPFolder folder in list.RootFolder.SubFolders) 
				AddFolderNode(folder);

			if (list.Items.Count > 0)
			{
				if (list.BaseType != SPBaseType.DocumentLibrary)
				{
					foreach (SPListItem li in list.Items) 
						AddItemNode(li);
				}
				else
				{
					foreach (SPFile file in list.RootFolder.Files)	
						AddFileNode(file);
				}
			}

			_structureWriter.WriteEndElement();

		}

		private void AddViewNode(SPView view)
		{
			_structureWriter.WriteStartElement("View");
			_structureWriter.WriteAttributeString("ID", view.ID.ToString());
			_structureWriter.WriteAttributeString("Title", view.Title);
			_structureWriter.WriteAttributeString("ServerRelativeUrl", view.ServerRelativeUrl);
			_structureWriter.WriteEndElement();
		}
		
		private void AddItemNode(SPListItem li)
		{
			_structureWriter.WriteStartElement("Item");
			_structureWriter.WriteAttributeString("ID", li.UniqueId.ToString());
			_structureWriter.WriteAttributeString("Name", li.Name);
			_structureWriter.WriteAttributeString("Title", li.Title);
			_structureWriter.WriteAttributeString("ServerRelativeUrl", SPFileUtil.GetServerRelativeUrl(li.ParentList, li.Url));
			_structureWriter.WriteEndElement();
		}

		private void AddFolderNode(SPFolder folder)
		{
			bool islist = (folder.ParentListId != Guid.Empty);
			bool isweb = (folder.ServerRelativeUrl == folder.ParentWeb.ServerRelativeUrl);

			bool isListRoot = false;
			SPList list = null;

			if (islist)
			{
				list = folder.ParentWeb.Lists[folder.ParentListId];
				isListRoot = (list.RootFolder.ServerRelativeUrl == folder.ServerRelativeUrl);
			}

			if (isweb)
			{
				AddWebNode(folder.ParentWeb);
			}
			else if (islist && isListRoot)
			{				
				AddListNode(list);
			}
			else
			{
				AddStandardFolderNode(folder);
			}


		}

		private void AddStandardFolderNode(SPFolder folder)
		{
			_structureWriter.WriteStartElement("Folder");
			_structureWriter.WriteAttributeString("Title", folder.Name);
			_structureWriter.WriteAttributeString("ServerRelativeUrl", folder.ServerRelativeUrl);

			foreach (SPFolder subfolder in folder.SubFolders)
				AddFolderNode(subfolder);

			foreach (SPFile file in folder.Files)
				AddFileNode(file);

			_structureWriter.WriteEndElement();

		}


		//private void AddWebFolderNode1(SPFolder folder)
		//{
		//    bool islist = (folder.ParentListId != Guid.Empty);
		//    bool isweb = (folder.ParentWeb.ID != folder.ParentWeb.ID);

		//    if (islist)
		//    {
		//        SPList list = subFolder.ParentWeb.Lists[subFolder.ParentListId];

		//        bool isListRoot = (list.RootFolder.ServerRelativeUrl == subFolder.ServerRelativeUrl);
		//        bool isInWebRoot = (subFolder.ParentWeb.ServerRelativeUrl == subFolder.ServerRelativeUrl);

		//        bool renderFolderNode = (!isInWebRoot && !isListRoot);

		//        if (renderFolderNode)
		//        {
		//            _structureWriter.WriteStartElement("Folder");
		//            _structureWriter.WriteAttributeString("Title", folder.Name);
		//            _structureWriter.WriteAttributeString("ServerRelativeUrl", folder.ServerRelativeUrl);
		//        }

		//        AddListNode(list);

		//        if (renderFolderNode)
		//            _structureWriter.WriteEndElement();
		//    }

		//    if (!islist && !isweb)
		//    {
		//        _structureWriter.WriteStartElement("Folder");
		//        _structureWriter.WriteAttributeString("Title", folder.Name);
		//        _structureWriter.WriteAttributeString("ServerRelativeUrl", folder.ServerRelativeUrl);

		//        AddWebFolderNode(subFolder);

		//        foreach (SPFile file in folder.Files)
		//            AddFileNode(file);

		//        _structureWriter.WriteEndElement();
		//    }


		//    foreach (SPFolder subFolder in folder.SubFolders)
		//    {
		//        AddWebFolderNode(subFolder);
		//    }

		//}

		//private void AddListFolderNode1(SPFolder folder)
		//{
		//    foreach (SPFolder subFolder in folder.SubFolders)
		//    {
		//        _structureWriter.WriteStartElement("Folder");
		//        _structureWriter.WriteAttributeString("Title", folder.Name);
		//        _structureWriter.WriteAttributeString("ServerRelativeUrl", folder.ServerRelativeUrl);

		//        AddListFolderNode(subFolder);

		//        foreach (SPFile file in folder.Files) 
		//            AddFileNode(file);

		//        _structureWriter.WriteEndElement();
		//    }
		//}

		private void AddFileNode(SPFile file)
		{
			_structureWriter.WriteStartElement("File");
			_structureWriter.WriteAttributeString("Title", file.Title);
			_structureWriter.WriteAttributeString("Name", file.Name);
			_structureWriter.WriteAttributeString("ServerRelativeUrl", file.ServerRelativeUrl);
			_structureWriter.WriteAttributeString("Customized", file.CustomizedPageStatus.ToString());
			_structureWriter.WriteAttributeString("ID", file.UniqueId.ToString());
			_structureWriter.WriteAttributeString("CheckedOut", Convert.ToString(file.CheckOutStatus != SPFile.SPCheckOutStatus.None));
			_structureWriter.WriteEndElement();
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
