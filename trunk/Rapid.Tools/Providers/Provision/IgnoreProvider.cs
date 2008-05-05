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
using Rapid.Tools.Domain;
using Rapid.Tools.Domain.Provision;

namespace Rapid.Wcm.Providers.Provision
{
	public class IgnoreProvider : RapidXmlToolProviderBase
	{

		public override void Execute(XmlElement contextElement, RapidXmlToolProviderContext context)
		{
			Manager.WriteMessage("Ignored.");
		}

	}
}
