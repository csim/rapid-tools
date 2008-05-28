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
using Rapid.Tools.Domain;

namespace Rapid.Tools.Domain.Utilities
{
	public static class SPXmlUtil
	{

		public static string Transform(string xmlPath, string xsltPath)
		{
			return Transform(xmlPath, xsltPath, null);
		}

		public static string Transform(string xmlPath, string xsltPath, string xsltParameters)
		{

			XPathDocument xml = ResolveFile(SPContext.Current.Site, xmlPath);
			XPathDocument xslt = ResolveFile(SPContext.Current.Site,  xsltPath);
			
			return Transform(xml, xslt, xsltParameters);
		}

		public static string Transform(XmlDocument xml, string xsltPath)
		{
			return Transform(xml, xsltPath, null);
		}

		public static string Transform(XmlDocument xml, string xsltPath, string xsltParameters)
		{
			using (StringReader reader = new StringReader(xml.OuterXml))
			{
				XPathDocument xxml = new XPathDocument(reader);
				XPathDocument xslt = ResolveFile(SPContext.Current.Site, xsltPath);
				return Transform(xxml, xslt, xsltParameters);
			}
		}

		public static string Transform(XPathDocument xml, string xsltPath)
		{
			return Transform(xml, xsltPath, null);
		}

		public static string Transform(XPathDocument xml, string xsltPath, string xsltParameters)
		{
			XPathDocument xslt = ResolveFile(SPContext.Current.Site, xsltPath);
			return Transform(xml, xslt, xsltParameters);
		}


		public static string Transform(XPathDocument xml, XPathDocument xslt, string xsltParameters)
		{

			SPSite site = SPContext.Current.Site;
			SPFile file = SPContext.Current.File;
			SPListItem item = SPContext.Current.ListItem;

			XslCompiledTransform xform = new XslCompiledTransform();
			xform.Load(xslt);

			XsltArgumentList args = ParseParameters(xsltParameters);

			if (item != null)
			{
				string title = Convert.ToString(item.Title);
				args.AddParam("PageTitle", "", title);
			}


			if (file != null && file.Exists)
			{
				args.AddParam("PageServerRelativeUrl", "", file.ServerRelativeUrl);

				if (file.ParentFolder != null)
					args.AddParam("PageFolderServerRelativeUrl", "", file.ParentFolder.ServerRelativeUrl);
			}
			else if (HttpContext.Current != null)
			{
				string spath = HttpContext.Current.Request.Url.AbsolutePath;

				args.AddParam("PageServerRelativeUrl", "", spath);

				int idx = spath.LastIndexOf("/");
				if (idx >= 0)
					args.AddParam("PageFolderServerRelativeUrl", "", spath.Substring(0, idx));
			}

			XmlWriterSettings writersettings = new XmlWriterSettings();
			writersettings.ConformanceLevel = ConformanceLevel.Fragment;
			writersettings.Indent = true;
			writersettings.OmitXmlDeclaration = true;

			StringBuilder output = new StringBuilder();

			using (XmlWriter outwriter = XmlWriter.Create(output, writersettings))
			{
				xform.Transform(xml.CreateNavigator(), args, outwriter);
			}

			return output.ToString();
		}


		public static XPathDocument ResolveFile(SPSite site, string path)
		{
			XPathDocument ret = null;

			if (path.ToUpper().StartsWith("HTTP://") || path.ToUpper().StartsWith("HTTPS://"))
			{
				ret = new XPathDocument(path);
			}
			else if (path.StartsWith("~") && HttpContext.Current != null)
			{
				path = HttpContext.Current.Server.MapPath(path);
				using (Stream fstream = File.OpenRead(path))
				{
					ret = new XPathDocument(fstream);
				}
			}
			else
			{
				// If the xml path is not an absolute url, assume that it in a file in the sharepoint content database
				SPFile xmlfile = SPFileUtil.Fetch(site, path);
				if (xmlfile == null || !xmlfile.Exists) return null;

				using (Stream fstream = xmlfile.OpenBinaryStream())
				{
					ret = new XPathDocument(fstream);
				}
			}

			return ret;
		}

		public static XsltArgumentList ParseParameters(string parameters)
		{

			XsltArgumentList ret = new XsltArgumentList();

			if (string.IsNullOrEmpty(parameters))
				return ret;

			string key, value;

			MatchCollection matches = Regex.Matches(parameters, "(.*?)=(.*?);", RegexOptions.Singleline);

			foreach (Match m in matches)
			{
				key = m.Result("$1");
				value = m.Result("$2");
				ret.AddParam(key, "", value);
			}

			return ret;

		}


	}
}
