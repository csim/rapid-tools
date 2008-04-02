using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Web;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using System.Security;
using System.Web.UI.WebControls.WebParts;

namespace RapidTools.Utilities
{
	public static class SPXmlUtil
	{

		public static string Transform(string xmlPath, string xsltPath)
		{
			return Transform(xmlPath, xsltPath, null);
		}

		public static string Transform(string xmlPath, string xsltPath, string xsltParameters)
		{

			XmlDocument xml = new XmlDocument();

			if (xmlPath.ToUpper().StartsWith("HTTP://")
				|| Regex.IsMatch(xmlPath, @"(\w)\:\\", RegexOptions.IgnoreCase | RegexOptions.Singleline))
			{

				xml.Load(xmlPath);

			}
			else
			{

				// If the xml path is not an absolute url, assume that it in a file in the sharepoint content database

				SPFile xmlfile = SPFileUtil.Fetch(SPContext.Current.Site, xmlPath);
				if (xmlfile == null || !xmlfile.Exists) return null;

				using (Stream fstream = xmlfile.OpenBinaryStream())
				{
					using (XmlReader reader = XmlReader.Create(fstream))
					{
						xml.Load(reader);
					}
				}
			}

			return Transform(xml, xsltPath, xsltParameters);
		}

		public static string Transform(XmlDocument xml, string xsltPath)
		{
			return Transform(xml, xsltPath, null);
		}

		public static string Transform(XmlDocument xml, string xsltPath, string xsltParameters)
		{

			SPSite site = SPContext.Current.Site;
			SPFile file = SPContext.Current.File;
			SPListItem item = SPContext.Current.ListItem;


			XslCompiledTransform xslt = new XslCompiledTransform();

			if (xsltPath.ToUpper().StartsWith("HTTP://")
				|| Regex.IsMatch(xsltPath, @"(\w)\:\\", RegexOptions.IgnoreCase | RegexOptions.Singleline))
			{

				xslt.Load(xsltPath);

			}
			else
			{

				// If the transform path is not an absolute url, assume that it in a file in the sharepoint content database

				SPFile xsltfile = SPFileUtil.Fetch(site, xsltPath);
				if (xsltfile == null || !xsltfile.Exists) return null;

				string contents = SPFileUtil.FetchAsString(xsltfile);

				using (Stream fstream = xsltfile.OpenBinaryStream())
				{
					using (XmlReader reader = XmlReader.Create(fstream))
					{
						xslt.Load(reader);
					}
				}

			}

			StringBuilder output = new StringBuilder();

			XmlWriterSettings writersettings = new XmlWriterSettings();
			writersettings.ConformanceLevel = ConformanceLevel.Fragment;
			writersettings.Indent = true;
			writersettings.OmitXmlDeclaration = true;


			XsltArgumentList args = new XsltArgumentList();

			Dictionary<string, string> param = new Dictionary<string, string>();


			if (item != null)
			{
				string title = Convert.ToString(item[RapidToolsConstants.SiteColumns.Title.ID]);
				param["PageTitle"] = title;
			}


			if (file != null && file.Exists)
			{

				param["PageServerRelativeUrl"] = file.ServerRelativeUrl;

				if (file.ParentFolder != null)
					param["PageFolderServerRelativeUrl"] = file.ParentFolder.ServerRelativeUrl;

			}
			else if (HttpContext.Current != null)
			{

				string spath = HttpContext.Current.Request.Url.AbsolutePath;

				param["PageServerRelativeUrl"] = spath;

				int idx = spath.LastIndexOf("/");
				if (idx >= 0)
					param["PageFolderServerRelativeUrl"] = spath.Substring(0, idx);

			}

			if (!string.IsNullOrEmpty(xsltParameters))
			{
				string key, value;

				MatchCollection matches = Regex.Matches(xsltParameters, "(.*?)=(.*?);", RegexOptions.Singleline);

				foreach (Match m in matches)
				{
					key = m.Result("$1");
					value = m.Result("$2");
					param[key] = value;
				}
			}

			foreach (string key in param.Keys)
			{
				args.AddParam(key, "", param[key]);
			}



			using (XmlWriter outwriter = XmlWriter.Create(output, writersettings))
			{
				xslt.Transform(xml.CreateNavigator(), args, outwriter);
			}

			return output.ToString();

		}


	}
}
