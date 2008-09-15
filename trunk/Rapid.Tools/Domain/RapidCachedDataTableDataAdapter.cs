using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.XPath;
using System.IO;
using Microsoft.SharePoint;

namespace Rapid.Tools.Domain
{
	public abstract class RapidCachedDataTableDataAdapter : RapidCachedDataAdapterBase<DataTable>
	{

		public RapidCachedDataTableDataAdapter(Guid siteID)
			: base(siteID)
		{
		}

		public RapidCachedDataTableDataAdapter(SPSite site)
			: base(site)
		{
		}

		protected override string Serialize(DataTable data)
		{
			if (data == null)
				return null;

			var ds = new DataSet();
			ds.Tables.Add(data);
			return SerializeInternal(ds);
		}

		protected override DataTable Deserialize(string data)
		{
			if (string.IsNullOrEmpty(data))
				return null;

			var ds = DeserializeInternal<DataSet>(data);
			return ds.Tables[0];
		}

	}
}
