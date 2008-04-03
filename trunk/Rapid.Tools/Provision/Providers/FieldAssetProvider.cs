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
using Rapid.Tools.Utilities;
using Rapid.Tools.Provision;

namespace Rapid.Tools.Provision.Providers
{
	public class FieldAssetProvider : AssetProviderBase
	{

		public override void Import(XmlElement contextElement, ProvisionContext context)
		{
			string url = GetAttribute(contextElement, "Url");
			string name = GetAttribute(contextElement, "Name");
			string type = GetAttribute(contextElement, "Type");

			if (context == null || context.Web == null) throw new Exception("Context is invalid, no context web specified.");
			if (context == null || context.Item == null)
			{
				Manager.WriteWarning("Skipping, item context not available.");
				return;
			}

			SPListItem item = context.Item;
			bool exists = SPFieldUtil.FieldExists(item.ParentList, name);

			if (!exists)
			{
				Manager.WriteWarning("\"{0}\" is not valid field, skipping.", name);
				return;
			}


			bool update = false;

			if (string.IsNullOrEmpty(type) || type.ToLower() == "text")
			{
				string val = GetAttribute(contextElement, "Value");

				if (string.IsNullOrEmpty(val))
					val = contextElement.InnerText;

				SPFieldUtil.SetFieldValue(item, name, val);
				update = true;

				Manager.WriteMessage("Set {0}", name);
			}

			if (update) item.Update();

		}

		public override System.Xml.XmlElement Export(XmlElement contextElement, ProvisionContext context)
		{

			Manager.WriteMessage("Exporting...");

			return null;
		}

	}
}
