using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;
using Microsoft.SharePoint.WebControls;

using Rapid.Tools.Domain;
using Rapid.Tools.Domain.Utilities;

namespace Rapid.Tools.Receivers
{
	public class UrlMapReceiver : SPFeatureReceiver
	{

		public override void FeatureInstalled(SPFeatureReceiverProperties properties)
		{
		}

		public override void FeatureActivated(SPFeatureReceiverProperties properties)
		{

			try
			{

				SPSite site = (SPSite)properties.Feature.Parent;
				SPWeb rweb = site.RootWeb;

				SPFeatureUtil.ActivateFeature(site, RapidConstants.Features.ListsID);

				SPList urlmappings = EnsureUrlMappingList(site);

			}
			catch (Exception ex)
			{
				SPExceptionUtil.Print(ex);
			}

		}


		public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
		{
		}

		public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
		{
		}



		private SPList EnsureUrlMappingList(SPSite site)
		{

			SPWeb rweb = site.RootWeb;

			if (SPListUtil.ListExists(rweb, RapidConstants.UrlMapList.Url))
			{
				return SPListUtil.GetList(rweb, RapidConstants.UrlMapList.Url);
			}

			Guid listid = rweb.Lists.Add(RapidConstants.UrlMapList.Title, RapidConstants.UrlMapList.Description,
					RapidConstants.UrlMapList.Url, RapidConstants.UrlMapList.FeatureID.ToString(), RapidConstants.UrlMapList.TemplateID, "", SPListTemplate.QuickLaunchOptions.Off);

			SPList lib = null;
			lib = rweb.Lists[listid];
			lib.ContentTypesEnabled = true;
			lib.EnableVersioning = false;
			lib.EnableModeration = false;
			lib.EnableMinorVersions = false;
			lib.EnableAttachments = false;
			lib.Update();

			SPListUtil.SetContentTypes(lib, new List<SPContentTypeId>(new SPContentTypeId[] { RapidConstants.ContentTypes.UrlMappingID }));

			return lib;

		}



	}
}
