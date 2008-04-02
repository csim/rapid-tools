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

namespace RapidTools.Utilities
{
	public static class SPListUtil
	{

		public static bool ListExists(SPWeb web, string webRelativeUrl)
		{
			try
			{
				SPList list = web.GetList(SPFileUtil.GetServerRelativeUrl(web, webRelativeUrl));
				return true;
			}
			catch (FileNotFoundException)
			{
				return false;
			}
		}

		public static SPList GetList(SPWeb web, string webRelativeUrl)
		{
			string surl = SPFileUtil.GetServerRelativeUrl(web, webRelativeUrl);
			try
			{
				return web.GetList(surl);
			}
			catch (FileNotFoundException)
			{
				return null;
			}
		}

		public static SPList GetList(SPWeb web, Guid id)
		{
			try
			{
				return web.Lists[id];
			}
			catch (ArgumentException)
			{
				return null;
			}
		}


		public static SPDocumentLibrary GetDocumentLibrary(SPWeb web, string webRelativeUrl)
		{
			SPList list = GetList(web, webRelativeUrl);

			if (list != null && list is SPDocumentLibrary)
			{
				return (SPDocumentLibrary)list;
			}
			else
			{
				return null;
			}
		}

		public static SPDocumentLibrary GetDocumentLibrary(SPWeb web, Guid id)
		{
			SPList list = GetList(web, id);

			if (list != null && list is SPDocumentLibrary)
			{
				return (SPDocumentLibrary)list;
			}
			else
			{
				return null;
			}
		}


		public static bool CheckPermissions(SPList list, SPBasePermissions permMask)
		{

			bool ret = list.DoesUserHavePermissions(permMask);
			return ret;

		}


		public static void SetContentTypes(SPList lib, List<SPContentTypeId> visibleContentTypes)
		{
			SetContentTypes(lib, visibleContentTypes, null);
		}

		public static void SetContentTypes(SPList lib, List<SPContentTypeId> visibleContentTypeIDs, List<SPContentTypeId> hiddenContentTypeIDs)
		{

			List<SPContentType> visibleContentTypes = new List<SPContentType>();

			SPContentType ct;
			SPContentTypeId? ctid;

			// *** Add all the content types that need to be added ***
			List<SPContentTypeId> newTypeIDs = new List<SPContentTypeId>();

			if (visibleContentTypeIDs != null) newTypeIDs.AddRange(visibleContentTypeIDs);
			if (hiddenContentTypeIDs != null) newTypeIDs.AddRange(hiddenContentTypeIDs);

			// Remove all existing content types
			foreach (SPContentType ctype in lib.ContentTypes)
				newTypeIDs.RemoveAll(delegate(SPContentTypeId tid) { return tid == ctype.Parent.Id; });

			// Add new content types to the list
			foreach (SPContentTypeId ttype in newTypeIDs)
			{
				ct = lib.ParentWeb.Site.RootWeb.ContentTypes[ttype];
				lib.ContentTypes.Add(ct);
			}

			// Get the leaf content types and construct the visible list
			foreach (SPContentTypeId ctype in visibleContentTypeIDs)
			{
				ctid = GetListSpecificContentType(lib, ctype);
				if (ctid.HasValue)
				{
					ct = lib.ContentTypes[ctid.Value];
					visibleContentTypes.Add(ct);
				}
			}

			lib.Update();

			lib.RootFolder.UniqueContentTypeOrder = visibleContentTypes;
			lib.RootFolder.Update();

		}



		public static SPContentTypeId? GetListSpecificContentType(SPList list, SPContentTypeId cid)
		{
			if (!list.ContentTypesEnabled) return null;

			foreach (SPContentType ctype in list.ContentTypes)
			{
				if (ctype.Id.Parent == cid)
					return ctype.Id;
			}

			return null;

		}

		public static bool SetListSpecificContentType(SPListItem item, SPContentTypeId contentTypeID)
		{

			SPList list = item.ParentList;

			SPContentTypeId? id = GetListSpecificContentType(list, contentTypeID);

			if (id.HasValue)
			{
				return SPFieldUtil.TrySetFieldValue(item, RapidToolsConstants.SiteColumns.ContentTypeID.ID, id.Value);
			}
			else
			{
				return false;
			}

		}

	}
}
