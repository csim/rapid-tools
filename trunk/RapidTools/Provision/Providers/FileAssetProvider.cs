using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Security.Principal;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RapidTools.Utilities;
using RapidTools.Provision;

namespace RapidTools.Provision.Providers
{
	public class FileAssetProvider : AssetProviderBase
	{

		public override void Import(XmlElement contextElement, RapidProvisionContext context)
		{

			RapidProvisionContext icontext = new RapidProvisionContext();

			string url = GetAttribute(contextElement, "Url");
			string title = GetAttribute(contextElement, "Title");
			string path = GetAttribute(contextElement, "Path");

			bool overwrite = GetAttributeBoolean(contextElement, "Overwrite");
			bool checkin = GetAttributeBoolean(contextElement, "Checkin", true);
			bool approve = GetAttributeBoolean(contextElement, "Approve", true);

			string apath = Manager.GetFilePath(path);
			string filename = Path.GetFileName(apath);

			if (!File.Exists(apath))
			{

				Manager.WriteError("File not found: {0}", apath);

			}
			else
			{

				if (context == null || context.Web == null) throw new Exception("Context is invalid, no context web specified.");
				SPWeb web = context.Web;

				SPFile file = SPFileUtil.Fetch(web, url);

				if (file.Exists)
				{
					if (overwrite)
					{
						file = AddFile(web, file.ServerRelativeUrl, filename, title, apath, true, false);
						Manager.WriteMessage("{0} (overwrite)", file.ServerRelativeUrl, path);
					}
					else
					{
						Manager.WriteMessage("{0} (skipped)", file.ServerRelativeUrl, path);
					}
				}
				else
				{
					file = AddFile(web, file.ServerRelativeUrl, filename, title, apath, true, false);
					Manager.WriteMessage("{0} (new)", file.ServerRelativeUrl, path);
				}

				if (file.InDocumentLibrary)
				{
					icontext.Web = web;
					icontext.Item = file.Item;
				}

				Manager.ImportChildAssets(contextElement, icontext);

				if (file != null)
				{
					if (checkin)
					{
						SPFileUtil.CheckIn(file);

						if (approve)
							SPFileUtil.Approve(file);
					}
				}

			}


		}

		private SPFile AddFile(SPWeb web, string fileServerRelativeUrl, string filename, string title, string pathOnDisk, bool overwrite, bool checkin)
		{
			SPFolder folder = SPFileUtil.EnsureParentFolder(web, fileServerRelativeUrl);
			SPFile file = SPFileUtil.AddFromDisk(folder, pathOnDisk, filename, overwrite, checkin);

			if (file.InDocumentLibrary)
			{
				SPFieldUtil.SetFieldValue(file.Item, RapidToolsConstants.SiteColumns.Title.ID, title);
				file.Item.Update();
			}

			return file;
		}


		public override XmlElement Export(XmlElement contextElement, RapidProvisionContext context)
		{

			return null;

		}

	}
}
