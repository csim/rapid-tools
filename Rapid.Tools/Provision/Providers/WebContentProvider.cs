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
using Microsoft.SharePoint;
using Rapid.Tools.Provision;

namespace Rapid.Tools.Provision.Providers
{
	public class WebContentProvider : AssetProviderBase
	{

		public override void Import(XmlElement contextElement, RapidProvisionContext context)
		{

			if (context == null || context.Web == null) throw new Exception("Context is invalid.");

			RapidProvisionContext icontext = new RapidProvisionContext();

			string url = GetAttribute(contextElement, "Url");

			SPWeb web = null;
			bool exists;

			try
			{
				web = context.Web.Site.OpenWeb(url);
				string surl = web.ServerRelativeUrl;
				exists = true;
			}
			catch (FileNotFoundException)
			{
				exists = false;
				Manager.WriteWarning("Invalid web at {0}", url);
			}

			if (exists)
			{
				using (web)
				{
					Manager.WriteMessage("{0} Importing...", web.ServerRelativeUrl);
					icontext.Web = web;

					Manager.ImportChildAssets(contextElement, icontext);
				}
			}

		}

		public override System.Xml.XmlElement Export(XmlElement contextElement, RapidProvisionContext context)
		{

			Manager.WriteMessage("Exporting...");

			return null;
		}

	}
}
