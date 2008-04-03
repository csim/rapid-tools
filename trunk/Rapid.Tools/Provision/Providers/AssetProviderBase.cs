using System;
using System.Xml;
using System.Data;
using System.Configuration;
using Microsoft.SharePoint;
using Rapid.Tools.Provision;

namespace Rapid.Tools.Provision.Providers
{
	public abstract class AssetProviderBase
	{

		private ProvisionManager _manager;

		public ProvisionManager Manager
		{
			get { return _manager; }
			set { _manager = value; }
		}

		public abstract void Import(XmlElement contextElement, ProvisionContext context);

		public abstract XmlElement Export(XmlElement contextElement, ProvisionContext context);


		/***** Utility Functions **********/
		protected string GetAttribute(XmlElement element, string attributeName)
		{
			return GetAttribute(element, attributeName, null);
		}

		protected string GetAttribute(XmlElement element, string attributeName, string defaultValue)
		{
			string val = element.GetAttribute(attributeName);
			if (string.IsNullOrEmpty(val)) return null;

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
