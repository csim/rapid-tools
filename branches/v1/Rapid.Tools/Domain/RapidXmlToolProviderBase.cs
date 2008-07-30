using System;
using System.Xml;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Rapid.Tools.Domain
{
	public abstract class RapidXmlToolProviderBase
	{

		private RapidXmlToolManager _manager;

		public RapidXmlToolManager Manager
		{
			get { return _manager; }
			set { _manager = value; }
		}

		public abstract void Execute(XmlElement contextElement, RapidXmlToolProviderContext context);


		/***** Utility Functions **********/
		protected string GetAttribute(XmlElement element, string attributeName)
		{
			return GetAttribute(element, attributeName, null);
		}

		protected string GetAttribute(XmlElement element, string attributeName, string defaultValue)
		{
			string val = element.GetAttribute(attributeName);
			if (string.IsNullOrEmpty(val)) return defaultValue;

			return val;
		}

		protected bool GetAttributeBoolean(XmlElement element, string attributeName)
		{
			return GetAttributeBoolean(element, attributeName, false);
		}

		protected bool GetAttributeBoolean(XmlElement element, string attributeName, bool defaultValue)
		{
			string val = element.GetAttribute(attributeName);
			if (string.IsNullOrEmpty(val)) return defaultValue;

			return Convert.ToBoolean(val);
		}

		protected DateTime GetAttributeDateTime(XmlElement element, string attributeName)
		{
			return GetAttributeDateTime(element, attributeName, DateTime.Now);
		}

		protected DateTime GetAttributeDateTime(XmlElement element, string attributeName, DateTime defaultValue)
		{
			string val = element.GetAttribute(attributeName);
			if (string.IsNullOrEmpty(val)) return defaultValue;

			return Convert.ToDateTime(val);
		}


	}
}
