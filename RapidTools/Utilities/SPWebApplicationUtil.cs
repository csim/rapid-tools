using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Rapid.Tools.Utilities
{
	public static class SPWebApplicationUtil
	{

		public static SPUrlZone GetZone(string url)
		{
			SPUrlZone zone = SPFarm.Local.AlternateUrlCollections.LookupAlternateUrl(new Uri(url)).UrlZone;
			return zone;
		}

		public static SPIisSettings GetIisSettings(SPSite site)
		{
			SPUrlZone zone = GetZone(site.Url);
			SPIisSettings settings = site.WebApplication.IisSettings[zone];
			return settings;
		}


	}
}
