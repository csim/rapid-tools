using System;
using System.Xml;
using System.Data;
using System.Configuration;
using Microsoft.SharePoint;
using RapidTools.Provision;

namespace RapidTools.Provision.Providers
{
	public abstract class AssetProviderBase
	{

		private RapidProvisionManager _manager;

		public RapidProvisionManager Manager
		{
			get { return _manager; }
			set { _manager = value; }
		}

		public abstract void Import(XmlElement contextElement, RapidProvisionContext context);

		public abstract XmlElement Export(XmlElement contextElement, RapidProvisionContext context);


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
