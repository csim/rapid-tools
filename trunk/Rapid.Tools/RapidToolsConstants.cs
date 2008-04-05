using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Rapid.Tools
{
	public struct RapidToolsConstants
	{

		public static readonly string TemplateInstallPath = SPUtility.GetGenericSetupPath(@"TEMPLATE");
		public static readonly string FeatureInstallPath = TemplateInstallPath + @"\Features";
		public static readonly string LayoutsInstallPath = TemplateInstallPath + @"\Layouts";

		public struct ContentTypes
		{

			public static readonly SPContentTypeId ItemID = new SPContentTypeId("0x01");
			public static readonly SPContentTypeId DocumentID = new SPContentTypeId("0x0101");
			public static readonly SPContentTypeId FolderID = new SPContentTypeId("0x0120");

			public static readonly SPContentTypeId UrlMappingID = new SPContentTypeId("0x01002617A6C1BEB0439b9155104CFB953793");
		}

		public struct Features
		{
			public static readonly Guid UrlMapID = new Guid("{0D09DF91-B297-40da-9E63-088E3B6C2AF7}");
		}

		public struct UrlMapList
		{
			public static readonly int TemplateID = 752;
			public static readonly Guid FeatureID = RapidToolsConstants.Features.UrlMapID;
			public static readonly string Url = "_catalogs/RapidToolsUrlMap";
			public static readonly string Title = "Rapid Tools Url Map";
			public static readonly string Description = "";
		}


		public struct SiteColumns
		{
			public static readonly RapidToolsSiteColumnInfo ID = new RapidToolsSiteColumnInfo(new Guid("{1d22ea11-1e32-424e-89ab-9fedbadb6ce1}"), "ID");
			public static readonly RapidToolsSiteColumnInfo GUID = new RapidToolsSiteColumnInfo(new Guid("{ae069f25-3ac2-4256-b9c3-15dbc15da0e0}"), "GUID");

			public static readonly RapidToolsSiteColumnInfo Title = new RapidToolsSiteColumnInfo(new Guid("{fa564e0f-0c70-4ab9-b863-0177e6ddd247}"), "Title");
			public static readonly RapidToolsSiteColumnInfo Name = new RapidToolsSiteColumnInfo(new Guid("{8553196d-ec8d-4564-9861-3dbe931050c8}"), "Name");

			public static readonly RapidToolsSiteColumnInfo CreatedBy = new RapidToolsSiteColumnInfo(new Guid("{4dd7e525-8d6b-4cb4-9d3e-44ee25f973eb}"), "Created_x0020_By");
			public static readonly RapidToolsSiteColumnInfo ModifiedBy = new RapidToolsSiteColumnInfo(new Guid("{822c78e3-1ea9-4943-b449-57863ad33ca9}"), "Modified_x0020_By");

			public static readonly RapidToolsSiteColumnInfo Created = new RapidToolsSiteColumnInfo(new Guid("{8c06beca-0777-48f7-91c7-6da68bc07b69}"), "Created");
			public static readonly RapidToolsSiteColumnInfo Modified = new RapidToolsSiteColumnInfo(new Guid("{28cf69c5-fa48-462a-b5cd-27b6f9d2bd5f}"), "Modified");

			public static readonly RapidToolsSiteColumnInfo ContentTypeID = new RapidToolsSiteColumnInfo(new Guid("{03e45e84-1992-4d42-9116-26f756012634}"), "ContentTypeID");

			public static readonly RapidToolsSiteColumnInfo FileRef = new RapidToolsSiteColumnInfo(new Guid("{94f89715-e097-4e8b-ba79-ea02aa8b7adb}"), "FileRef");


			public static readonly RapidToolsSiteColumnInfo IncomingUrl = new RapidToolsSiteColumnInfo(new Guid("{285775BC-0D4A-4562-999C-B3AA435B53C1}"), "IncomingUrl");
			public static readonly RapidToolsSiteColumnInfo ResultUrl = new RapidToolsSiteColumnInfo(new Guid("{516C3624-1FA7-4ed7-9C30-CF2E6A5DC14A}"), "ResultUrl");
			public static readonly RapidToolsSiteColumnInfo Description = new RapidToolsSiteColumnInfo(new Guid("{F6D1D72F-51A7-4cc3-8246-FC21A561D884}"), "Description");
			public static readonly RapidToolsSiteColumnInfo RapidToolsActive = new RapidToolsSiteColumnInfo(new Guid("{E3EFF317-DCF8-4916-9ED9-D6BDC57E11F0}"), "RapidToolsActive");
		}
	}
}
