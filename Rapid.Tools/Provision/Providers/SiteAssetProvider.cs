using System;
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
using Rapid.Tools.Provision;

namespace Rapid.Tools.Provision.Providers
{
	public class SiteAssetProvider : AssetProviderBase
	{

		public override void Import(XmlElement contextElement, ProvisionContext context)
		{

			ProvisionContext icontext = new ProvisionContext();

			string url = GetAttribute(contextElement, "Url");
			string title = GetAttribute(contextElement, "Title");
			string description = GetAttribute(contextElement, "Description");
			string templateID = GetAttribute(contextElement, "TemplateID");
			bool overwrite = GetAttributeBoolean(contextElement, "Overwrite");

			string anonymousstate = GetAttribute(contextElement, "AnonymousState");
			if (string.IsNullOrEmpty(anonymousstate)) anonymousstate = "Disabled";

			SPWeb.WebAnonymousState anonymousState = (SPWeb.WebAnonymousState)Enum.Parse(typeof(SPWeb.WebAnonymousState), anonymousstate);

			string ownerlogin = contextElement.GetAttribute("OwnerLogin");
			string ownername = contextElement.GetAttribute("OwnerName");
			string owneremail = contextElement.GetAttribute("OwnerEmail");

			WindowsIdentity ident = WindowsIdentity.GetCurrent();

			if (string.IsNullOrEmpty(ownerlogin)) ownerlogin = ident.Name;
			if (string.IsNullOrEmpty(ownername)) ownername = ident.Name;
			if (string.IsNullOrEmpty(owneremail)) owneremail = ident.Name;

			SPSite site = Manager.WebApplication.Sites[url];
			bool siteExists = (site != null);

			bool create = true;

			if (siteExists)
			{
				if (overwrite)
				{
					Manager.WriteMessage("Deleting existing site ({0})", url);
					DeleteSite(Manager.WebApplication, url);
				}
				else
				{
					create = false;
					Manager.WriteMessage("Site", string.Format("Site at \"{0}\" already exists.", url));
				}
			}

			if (create)
			{
				Manager.WriteMessage(url);
				SPSite isite = CreateSite(Manager.WebApplication, url, templateID, title, description, anonymousState, ownerlogin, ownername, owneremail);

				icontext.Web = isite.RootWeb;
			}

			Manager.ImportChildAssets(contextElement, icontext);

		}

		public override XmlElement Export(XmlElement contextElement, ProvisionContext context)
		{

			return null;

		}


		public virtual SPSite CreateSite(SPWebApplication webapp, string url, string templateID, string title, string description,
											SPWeb.WebAnonymousState anonymousState, string ownerlogin, string ownername, string owneremail)
		{
			SPSite newsite;


			newsite = webapp.Sites.Add(url, title, description, 1033, templateID, ownerlogin, ownername, owneremail);
			newsite.RootWeb.AnonymousState = anonymousState;

			return newsite;

		}


		public virtual void DeleteSite(SPWebApplication webapp, string url)
		{

			if (url.StartsWith("/"))
			{
				if (url.Length == 1)
					url = "";
				else
					url = url.Substring(1);
			}

			webapp.Sites.Delete(url);
		}

	}
}
