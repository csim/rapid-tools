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
using Rapid.Tools.Provision;

namespace Rapid.Tools.Provision.Providers
{
	public class IgnoreProvider : AssetProviderBase
	{

		public override void Import(XmlElement contextElement, RapidProvisionContext context)
		{
			Manager.WriteMessage("Ignored.");
		}

		public override XmlElement Export(XmlElement contextElement, RapidProvisionContext context)
		{
			Manager.WriteMessage("Ignored.");
			return null;
		}

	}
}
