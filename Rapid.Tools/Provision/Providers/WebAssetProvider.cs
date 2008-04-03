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

using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;
using Rapid.Tools.Provision;

namespace Rapid.Tools.Provision.Providers
{
	public class WebAssetProvider : AssetProviderBase
	{

		public override void Import(XmlElement contextElement, ProvisionContext context)
		{

			ProvisionContext icontext = new ProvisionContext();

			string url = GetAttribute(contextElement, "Url");
			string templateID = GetAttribute(contextElement, "TemplateID");
			string title = GetAttribute(contextElement, "Title");

			if (context == null || context.Web == null) throw new Exception("Context is invalid, no context web specified.");

			SPWeb pweb = context.Web;

			SPWeb iweb = pweb.Webs.Add(url, title, "", 1033, templateID, false, false);

			if (!iweb.IsRootWeb) iweb.Navigation.UseShared = true;
			//pweb.Navigation.TopNavigationBar.AddAsLast(new SPNavigationNode(iweb.Title, iweb.ServerRelativeUrl));

			Manager.WriteMessage(iweb.ServerRelativeUrl);

			icontext.Web = iweb;

			Manager.ImportChildAssets(contextElement, icontext);


		}

		public override XmlElement Export(XmlElement contextElement, ProvisionContext context)
		{
			return null;
		}


	}
}
