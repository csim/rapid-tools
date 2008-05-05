using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace Rapid.Tools.Domain
{
	[Serializable]
	[XmlRoot("SiteColumnInfo")]
	public class RapidSiteColumnInfo
	{

		public RapidSiteColumnInfo()
		{
		}

		public RapidSiteColumnInfo(Guid id, string name)
		{
			this.ID = id;
			this.Name = name;
		}

		[XmlAttribute]
		public Guid ID;

		[XmlAttribute]
		public string Name;

	}

}
