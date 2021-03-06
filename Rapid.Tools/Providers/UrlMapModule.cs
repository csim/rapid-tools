using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

using Rapid.Tools.Domain;
using Rapid.Tools.Domain.Utilities;

namespace Rapid.Tools.Providers
{
	public class UrlMapModule : IHttpModule
	{

		public void Init(HttpApplication context)
		{

			context.BeginRequest += new EventHandler(BeginRequest);

		}


		public void BeginRequest(object sender, EventArgs e)
		{
			try
			{

				HttpContext context = HttpContext.Current;
				HttpRequest request = context.Request;
				HttpResponse response = context.Response;
				Uri suri = request.Url;

				string fext = VirtualPathUtility.GetExtension(request.FilePath);
				List<string> ext = new List<string>(new string[] { ".aspx", ".html", ".html" });

				// Only process the request for thease file extensions
				if (ext.Contains(fext))
				{

					string port = (suri.Port != 80) ? ":" + suri.Port.ToString() : "";

					string surl = string.Format("{0}://{1}{2}{3}", suri.Scheme, suri.Host, port, suri.AbsolutePath);


					string rurl = null;
					SPSecurity.RunWithElevatedPrivileges(delegate()
					{
						rurl = GetMapping(surl);
					});

					if (!string.IsNullOrEmpty(rurl))
						context.Response.Redirect(rurl);
				}

			}
			catch (Exception ex)
			{
				// Ignore execptions so that the http stream is not polluted
				//SPExceptionUtil.Print(ex);
			}
		}

		public void Dispose()
		{
		}


		private string GetMapping(string sourceUrl)
		{

			using (SPSite site = new SPSite(sourceUrl))
			{

				bool lexists = SPListUtil.ListExists(site.RootWeb, RapidConstants.UrlMapList.Url);
				if (!lexists) return null;

				SPList list = SPListUtil.GetList(site.RootWeb, RapidConstants.UrlMapList.Url);

				string query = "";

				string contentTypeClause = SPCamlUtil.GetComparison("BeginsWith", RapidConstants.SiteColumns.ContentTypeID.ID, "Text", RapidConstants.ContentTypes.UrlMappingID.ToString());
				query = contentTypeClause;

				string activeClause = SPCamlUtil.GetComparison("Eq", RapidConstants.SiteColumns.RapidToolsActive.ID, "Boolean", "1");
				query = SPCamlUtil.AppendCondition(query, "And", activeClause);

				string sourceClause = SPCamlUtil.GetComparison("Eq", RapidConstants.SiteColumns.IncomingUrl.ID, "Text", sourceUrl);
				query = SPCamlUtil.AppendCondition(query, "And", sourceClause);

				string orderByClause = string.Format("<OrderBy><FieldRef ID=\"{0}\" Name=\"{1}\" Ascending=\"TRUE\" /></OrderBy>", RapidConstants.SiteColumns.ID.ID, RapidConstants.SiteColumns.ID.Name);

				query = string.Format("<Where>{0}</Where>{1}", query, orderByClause);

				SPQuery q = new SPQuery();

				q.Folder = list.RootFolder;
				q.ViewAttributes = "Scope=\"Recursive\""; // Recursively look in all folders
				q.Query = query;

				//System.Web.HttpContext.Current.Response.Write(string.Format("{0}<hr/>", System.Web.HttpUtility.HtmlEncode(q.Query)));

				SPListItemCollection items = list.GetItems(q);
				if (items == null || items.Count == 0) return null;

				string ret = Convert.ToString(items[0][RapidConstants.SiteColumns.ResultUrl.ID]);
				return ret;

			}
		}



	}
}
