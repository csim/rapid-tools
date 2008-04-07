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
using Rapid.Tools.Utilities;

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

				SPFeatureUtil.ActivateFeature(site, RapidToolsConstants.Features.ListsID);

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

			if (SPListUtil.ListExists(rweb, RapidToolsConstants.UrlMapList.Url))
			{
				return SPListUtil.GetList(rweb, RapidToolsConstants.UrlMapList.Url);
			}

			Guid listid = rweb.Lists.Add(RapidToolsConstants.UrlMapList.Title, RapidToolsConstants.UrlMapList.Description,
					RapidToolsConstants.UrlMapList.Url, RapidToolsConstants.UrlMapList.FeatureID.ToString(), RapidToolsConstants.UrlMapList.TemplateID, "", SPListTemplate.QuickLaunchOptions.Off);

			SPList lib = null;
			lib = rweb.Lists[listid];
			lib.ContentTypesEnabled = true;
			lib.EnableVersioning = false;
			lib.EnableModeration = false;
			lib.EnableMinorVersions = false;
			lib.EnableAttachments = false;
			lib.Update();

			SPListUtil.SetContentTypes(lib, new List<SPContentTypeId>(new SPContentTypeId[] { RapidToolsConstants.ContentTypes.UrlMappingID }));

			return lib;

		}



	}
}
