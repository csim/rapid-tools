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

namespace Rapid.Tools.Domain.Utilities
{
	public static class SPFeatureUtil
	{

		public static bool FeatureActivated(SPSite site, Guid featureID)
		{
			SPFeature feature = site.Features[featureID];
			return (feature != null);
		}

		public static bool FeatureActivated(SPWeb web, Guid featureID)
		{
			SPFeature feature = web.Features[featureID];
			return (feature != null);
		}

		public static SPFeature ActivateFeature(SPSite site, Guid featureID)
		{
			SPFeature feature = site.Features[featureID];
			if (feature != null) return feature;
			return site.Features.Add(featureID);
		}

		public static SPFeature ActivateFeature(SPWeb web, Guid featureID)
		{
			SPFeature feature = web.Features[featureID];
			if (feature != null) return feature;
			return web.Features.Add(featureID);
		}

		public static bool DeactivateFeature(SPSite site, Guid featureID)
		{
			SPFeature feature = site.Features[featureID];
			if (feature == null) return false;
			site.Features.Remove(featureID);
			return true;
		}

		public static bool DeactivateFeature(SPWeb web, Guid featureID)
		{
			SPFeature feature = web.Features[featureID];
			if (feature == null) return false;
			web.Features.Remove(featureID);
			return true;
		}

	}
}
