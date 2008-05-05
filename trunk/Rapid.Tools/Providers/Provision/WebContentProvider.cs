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
using Rapid.Tools.Domain;
using Rapid.Tools.Domain.Provision;

namespace Rapid.Wcm.Providers.Provision
{
	public class WebContentProvider : RapidXmlToolProviderBase
	{

		public override void Execute(XmlElement contextElement, RapidXmlToolProviderContext context)
		{

			if (context == null || context.Web == null) throw new Exception("Context is invalid.");

			RapidXmlToolProviderContext icontext = new RapidXmlToolProviderContext();

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

					Manager.ExecuteChildren(contextElement, icontext);
				}
			}

		}

	}
}
