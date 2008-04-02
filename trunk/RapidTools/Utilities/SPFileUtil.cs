using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace RapidTools.Utilities
{
	public static class SPFileUtil
	{


		public static string GetServerRelativeUrl(SPList list, string listRelativeUrl)
		{
			return GetServerRelativeUrl(list.RootFolder, listRelativeUrl);
		}

		public static string GetServerRelativeUrl(SPWeb web, string webRelativeUrl)
		{
			return GetServerRelativeUrl(web.RootFolder, webRelativeUrl);
		}

		public static string GetServerRelativeUrl(SPFolder folder, string folderRelativeUrl)
		{
			string furl = folder.ServerRelativeUrl;
			return string.Format("{0}/{1}", furl == "/" ? "" : furl, folderRelativeUrl).Replace("//", "/");
		}

		public static string GetServerRelativeUrl(SPFolder folder, params string[] parts)
		{
			if (parts == null) return folder.ServerRelativeUrl;

			string surl = folder.ServerRelativeUrl;

			foreach (string part in parts)
			{
				surl = string.Format("{0}/{1}", surl, part);
			}

			return surl.Replace("//", "/");
		}


		/// <summary>
		/// Fetch a file from the SharePoint data store. With this method it is not necessary to know the web that the file belongs to.
		/// </summary>
		/// <param name="site">The site that the file belongs to.</param>
		/// <param name="path">Path to the file relative to the server. i.e. "/SharedDocuments/Document1.doc"</param>
		public static SPFile Fetch(SPSite site, string serverRelativeUrl)
		{

			if (string.IsNullOrEmpty(serverRelativeUrl)) return null;

			using (SPSite fsite = new SPSite(string.Format("{0}{1}", site.Url, serverRelativeUrl)))
			{
				using (SPWeb fweb = fsite.OpenWeb())
				{
					SPFile file = fweb.GetFile(serverRelativeUrl);
					return file;
				}
			}

		}

		/// <summary>
		/// Fetch a file from the SharePoint data store.
		/// </summary>
		/// <param name="web">The web that the file belongs to.</param>
		/// <param name="path">Path to the file relative to the web. i.e. "SharedDocuments/Document1.doc"</param>
		public static SPFile Fetch(SPWeb web, string webRelativePath)
		{
			if (string.IsNullOrEmpty(webRelativePath)) return null;
			string srel = SPFileUtil.GetServerRelativeUrl(web, webRelativePath);
			return web.GetFile(srel);
		}

		public static SPFile Fetch(SPDocumentLibrary library, string libraryRelativeUrl)
		{
			if (string.IsNullOrEmpty(libraryRelativeUrl)) return null;
			string srel = SPFileUtil.GetServerRelativeUrl(library, libraryRelativeUrl);
			return library.ParentWeb.GetFile(srel);
		}

		public static SPFile FetchWithServerRelativeUrl(SPWeb web, string serverRelativePath)
		{
			if (string.IsNullOrEmpty(serverRelativePath)) return null;
			return web.GetFile(serverRelativePath);
		}


		public static void Delete(SPSite site, string serverRelativeUrl)
		{
			SPFile file = Fetch(site, serverRelativeUrl);
			if (file.Exists) file.Delete();
		}

		public static void Delete(SPWeb web, string webRelativePath)
		{
			SPFile file = Fetch(web, webRelativePath);
			if (file.Exists) file.Delete();
		}

		public static byte[] FetchContents(SPWeb web, string webRelativePath)
		{
			string srel = SPFileUtil.GetServerRelativeUrl(web, webRelativePath);
			SPFile file = web.GetFile(srel);
			if (!file.Exists) return null;
			return file.OpenBinary();
		}

		public static byte[] FetchContents(SPDocumentLibrary lib, string libraryRelativePath)
		{
			SPWeb web = lib.ParentWeb;
			string srel = SPFileUtil.GetServerRelativeUrl(lib.RootFolder, libraryRelativePath);

			SPFile file = web.GetFile(srel);

			if (!file.Exists) return null;
			return file.OpenBinary();
		}


		public static SPFile AddFromDisk(SPFolder folder, string localPhysicalPath)
		{
			return AddFromDisk(folder, localPhysicalPath, false);
		}

		public static SPFile AddFromDisk(SPFolder folder, string localPhysicalPath, bool overwrite)
		{
			string filename = Path.GetFileName(localPhysicalPath);
			return AddFromDisk(folder, localPhysicalPath, filename, overwrite, false);
		}

		public static SPFile AddFromDisk(SPFolder folder, string localPhysicalPath, string filename, bool overwrite, bool checkin)
		{
			if (!File.Exists(localPhysicalPath)) return null;

			byte[] contents;
			contents = File.ReadAllBytes(localPhysicalPath);
			return Add(folder, filename, contents, overwrite, checkin);
		}


		public static SPFile Add(SPFolder folder, string filename, string contents, bool overwrite, bool checkin)
		{
			return Add(folder, filename, Encoding.Default.GetBytes(contents), overwrite, checkin);
		}

		public static SPFile Add(SPFolder folder, string filename, byte[] contents, bool overwrite, bool checkin)
		{
			string npath = SPFileUtil.GetServerRelativeUrl(folder, filename);
			SPFile nfile = FetchWithServerRelativeUrl(folder.ParentWeb, npath);

			if (!nfile.Exists)
			{
				nfile = folder.Files.Add(filename, contents, overwrite);
				if (checkin)
				{
					CheckIn(nfile);
					Approve(nfile);
				}
			}
			else
			{
				if (overwrite)
				{
					CheckOut(nfile);
					nfile.SaveBinary(contents);
					if (checkin)
					{
						CheckIn(nfile);
						Approve(nfile);
					}
				}
			}

			return nfile;
		}

		public static SPFile Add(SPFolder folder, string filename, Stream stream, bool overwrite, bool checkin)
		{
			string npath = SPFileUtil.GetServerRelativeUrl(folder, filename);
			SPFile nfile = FetchWithServerRelativeUrl(folder.ParentWeb, npath);

			if (!nfile.Exists)
			{
				nfile = folder.Files.Add(filename, stream, overwrite);
				if (checkin)
				{
					CheckIn(nfile);
					Approve(nfile);
				}
			}
			else
			{
				if (overwrite)
				{
					CheckOut(nfile);
					nfile.SaveBinary(stream);
					if (checkin)
					{
						CheckIn(nfile);
						Approve(nfile);
					}
				}
			}

			return nfile;
		}


		public static void MirrorDirectory(SPFolder folder, string rootPathOnDisk)
		{
			MirrorDirectory(folder, rootPathOnDisk, false);
		}

		public static void MirrorDirectory(SPFolder folder, string rootPathOnDisk, bool overwrite)
		{
			MirrorDirectory(folder, rootPathOnDisk, RapidToolsConstants.ContentTypes.DocumentID, false);
		}

		public static void MirrorDirectory(SPFolder folder, string rootPathOnDisk, SPContentTypeId contentTypeID, bool overwrite)
		{

			if (rootPathOnDisk.EndsWith(@"\")) rootPathOnDisk = rootPathOnDisk.Substring(0, rootPathOnDisk.Length - 1);

			SPFile nfile;
			SPListItem item;

			string[] files = Directory.GetFiles(rootPathOnDisk);
			string npath, nfilename;


			byte[] contents;
			foreach (string file in files)
			{
				contents = File.ReadAllBytes(file);

				nfilename = Path.GetFileName(file);
				npath = SPFileUtil.GetServerRelativeUrl(folder, nfilename);

				nfile = SPFileUtil.AddFromDisk(folder, file, nfilename, true, false);

				item = nfile.Item;
				SPFieldUtil.SetFieldValue(item, RapidToolsConstants.SiteColumns.ContentTypeID.ID, contentTypeID);
				item.Update();

				CheckIn(nfile);
				Approve(nfile);

				Console.WriteLine("{0,14} {1}", "[mirror]", nfile.ServerRelativeUrl);

			}

			string[] dirs = Directory.GetDirectories(rootPathOnDisk);
			SPFolder subfolder;
			string leafDirName;

			SPContentTypeId? folderContentTypeID = RapidToolsConstants.ContentTypes.FolderID;

			if (folder.Item != null && folder.Item.ParentList != null)
			{
				folderContentTypeID = SPListUtil.GetListSpecificContentType(folder.Item.ParentList, RapidToolsConstants.ContentTypes.FolderID) ?? RapidToolsConstants.ContentTypes.FolderID;
			}

			foreach (string dir in dirs)
			{
				leafDirName = dir.Substring(dir.LastIndexOf(@"\") + 1);
				subfolder = folder.SubFolders.Add(leafDirName);

				if (subfolder.Item != null)
				{
					SPFieldUtil.SetFieldValue(subfolder.Item, RapidToolsConstants.SiteColumns.ContentTypeID.ID, folderContentTypeID);
					subfolder.Item.Update();
				}

				Console.WriteLine("{0,14} {1}", "[mirror]", subfolder.ServerRelativeUrl);


				MirrorDirectory(subfolder, dir, contentTypeID, overwrite);
			}

		}


		/// <summary>
		/// Check Out a set of files.
		/// </summary>
		/// <param name="files">Files to be checked out.</param>
		public static bool CheckOut(SPFile[] files)
		{
			if (files == null) return false;

			bool ret = true;
			foreach (SPFile file in files)
			{
				ret = CheckOut(file) && ret;
			}

			return ret;
		}

		/// <summary>
		/// Check Out a set of files.
		/// </summary>
		/// <param name="files">Files to be checked out.</param>
		public static bool CheckOut(List<SPFile> files)
		{
			if (files == null) return false;
			return CheckOut(files.ToArray());
		}

		/// <summary>
		/// Check Out a file.
		/// </summary>
		/// <param name="file">File to be checked out.</param>
		public static bool CheckOut(SPFile file)
		{
			if (file == null) return false;
			if (!file.Exists) return false;
			if (!file.InDocumentLibrary) return false;

			bool versioned = file.Item.ParentList.EnableVersioning;

			if (versioned && file.CheckOutStatus == SPFile.SPCheckOutStatus.None)
			{
				file.CheckOut();
				return true;
			}

			return false;
		}


		/// <summary>
		/// Check In a file.
		/// </summary>
		/// <param name="file">File to be checked in.</param>
		public static bool CheckIn(SPFile file)
		{
			return CheckIn(file, SPCheckinType.MajorCheckIn);
		}

		/// <summary>
		/// Check In a file.
		/// </summary>
		/// <param name="file">File to be checked in.</param>
		/// <param name="type">Check In type</param>
		public static bool CheckIn(SPFile file, SPCheckinType type)
		{
			return CheckIn(file, type, "");
		}

		/// <summary>
		/// Check In a file.
		/// </summary>
		/// <param name="file">File to be checked in.</param>
		/// <param name="comment">Comment associated with this check in.</param>
		public static bool CheckIn(SPFile file, string comment)
		{
			return CheckIn(file, SPCheckinType.MajorCheckIn, comment);
		}

		/// <summary>
		/// Check In a file.
		/// </summary>
		/// <param name="file">File to be checked in.</param>
		/// <param name="type">Check In type</param>
		/// <param name="comment">Comment associated with this check in.</param>
		public static bool CheckIn(SPFile file, SPCheckinType type, string comment)
		{
			if (file == null) return false;
			if (!file.InDocumentLibrary) return false;

			bool versioned = file.Item.ParentList.EnableVersioning;

			if (versioned && file.CheckOutStatus != SPFile.SPCheckOutStatus.None)
			{
				file.CheckIn(comment, type);
				return true;
			}
			return false;
		}


		/// <summary>
		/// Approve a file.
		/// </summary>
		/// <param name="file">File to be approved.</param>
		public static bool Approve(SPFile file)
		{
			return Approve(file, "");
		}

		/// <summary>
		/// Approve a file.
		/// </summary>
		/// <param name="file">File to be approved.</param>
		/// <param name="comment">Comment associated with this approval.</param>
		public static bool Approve(SPFile file, string comment)
		{
			if (file == null) return false;
			if (!file.InDocumentLibrary) return false;

			bool moderated = file.Item.ParentList.EnableModeration;

			if (moderated)
			{
				file.Approve(comment);
				return true;
			}

			return false;

		}



		//public static SPFolder EnsureParentFolder(SPWeb web, string fileServerRelativeUrl) {
		//    fileServerRelativeUrl = web.GetFile(fileServerRelativeUrl).Url;

		//    int index = fileServerRelativeUrl.LastIndexOf("/");
		//    if (index < 0) return null;

		//    string folderUrl = fileServerRelativeUrl.Substring(0, index);
		//    return EnsureFolder(web, folderUrl);

		//}

		//public static SPFolder EnsureFolder(SPWeb web, string folderWebRelativeUrl) {

		//    SPFolder cfolder = web.GetFolder(SPUtil.GetServerRelativeUrl(web, folderWebRelativeUrl));
		//    if (cfolder.Exists) return cfolder;

		//    SPFolder currentFolder = web.RootFolder;
		//    foreach (string folder in folderWebRelativeUrl.Split('/')) {
		//        currentFolder = currentFolder.SubFolders.Add(folder);
		//    }
		//    return currentFolder;
		//}

		public static SPFolder EnsureParentFolder(SPWeb web, string fileServerRelativeUrl)
		{
			fileServerRelativeUrl = web.GetFile(fileServerRelativeUrl).Url;

			int index = fileServerRelativeUrl.LastIndexOf("/");
			if (index < 0) return null;

			string folderUrl = fileServerRelativeUrl.Substring(0, index);
			return EnsureFolder(web, folderUrl);

		}

		public static SPFolder EnsureFolder(SPWeb web, string folderServerRelativeUrl)
		{

			SPFolder targetFolder = web.GetFolder(folderServerRelativeUrl);
			if (targetFolder.Exists) return targetFolder;

			SPFolder currentFolder = web.RootFolder;
			SPFolder nextFolder;
			string nextFolderWebRelativeUrl;

			foreach (string folder in targetFolder.Url.Split('/'))
			{
				//nextFolderServerRelativeUrl = SPUtil.GetServerRelativeUrl(currentFolder, folder);
				//nextFolderWebRelativeUrl = string.Format("{0}{1}{2}", currentFolder.Url, currentFolder.Url == "" ? "" : "/", folder);
				//SPUtil.GetServerRelativeUrl(web, nextFolderWebRelativeUrl);
				//nextFolderWebRelativeUrl = SPUtil.GetServerRelativeUrl(rweb, string.Format("{0}/{1}", currentFolder.Url, folder));

				nextFolderWebRelativeUrl = string.Format("{0}{1}{2}", currentFolder.Url, currentFolder.Url == "" ? "" : "/", folder);

				nextFolder = web.GetFolder(nextFolderWebRelativeUrl);
				if (nextFolder.Exists)
				{
					currentFolder = nextFolder;
				}
				else
				{
					currentFolder = currentFolder.SubFolders.Add(folder);

					//if (currentFolder.Item != null && currentFolder.Item.ParentList != null) {
					//    SPFieldUtil.SetFieldValue(currentFolder.Item, RapidToolsConstants.SiteColumns.ContentTypeID.ID, RapidToolsConstants.ContentTypes.FolderID);
					//    currentFolder.Item.Update();
					//}


				}
			}

			return currentFolder;
		}


		public static string FetchAsString(SPFile file)
		{
			byte[] bcontents = file.OpenBinary();
			Encoding enc = GetEncoding(bcontents);
			return enc.GetString(bcontents);
		}

		public static Encoding GetEncoding(SPFile file)
		{
			byte[] bcontents = file.OpenBinary();
			return GetEncoding(bcontents);
		}

		public static Encoding GetEncoding(byte[] fileContents)
		{
			// By default use UTF-8
			Encoding enc = Encoding.UTF8;

			// Determine the encoding of the file contents
			// This is not pretty but there is no other way.
			using (MemoryStream ms = new MemoryStream())
			{
				ms.Write(fileContents, 0, fileContents.Length);
				using (StreamReader sr = new StreamReader(ms, true))
				{
					sr.ReadToEnd();
					enc = sr.CurrentEncoding;
				}
			}

			return enc;
		}

	}
}
