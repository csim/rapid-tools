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

			public static readonly RapidToolsSiteColumnInfo CommentCount = new RapidToolsSiteColumnInfo(new Guid("{AE2703EB-BC13-4d49-923A-F999C3D05F70}"), "CommentCount");

			public static readonly RapidToolsSiteColumnInfo SkinPath = new RapidToolsSiteColumnInfo(new Guid("{8D27E145-C531-424e-8EF8-38C92C72356E}"), "SkinPath");


			public static readonly RapidToolsSiteColumnInfo ContentTypeID = new RapidToolsSiteColumnInfo(new Guid("{03e45e84-1992-4d42-9116-26f756012634}"), "ContentTypeID");

			public static readonly RapidToolsSiteColumnInfo FileRef = new RapidToolsSiteColumnInfo(new Guid("{94f89715-e097-4e8b-ba79-ea02aa8b7adb}"), "FileRef");

		}

	}


}
