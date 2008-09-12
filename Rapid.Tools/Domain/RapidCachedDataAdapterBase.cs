using System;
using System.Xml.Serialization;
using System.Web.Caching;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;

namespace Rapid.Tools.Domain
{
	public abstract class RapidCachedDataAdapterBase<TData> where TData : class
	{
		private static readonly object _lock = new object();
		private TData _data;
		private Guid _siteID;

		protected virtual bool AutoRefreshCache {
			get
			{
				return true;
			}
		}
		
		protected virtual string CacheKey 
		{
			get
			{
				return string.Format("Rapid.{0}.Data", GetType().Name);
			}
		}

		protected virtual string CacheExpireKey 
		{
			get
			{
				return string.Format("{0}.Expire", CacheKey);
			}
		}

		protected virtual int CacheTimeoutMinutes {
			get
			{
				return 20;
			}
		}
		
		protected Guid SiteID
		{
			get
			{
				return _siteID;
			}
		}

		public virtual TData CachedData
		{
			get
			{
				if (_data == null)
					_data = GetData();

				return _data;
			}
		}


		public RapidCachedDataAdapterBase(Guid siteID)
		{
			_siteID = siteID;
		}

		public RapidCachedDataAdapterBase(SPSite site)
		{
			_siteID = site.ID;
		}


		protected abstract TData GetNativeData();

		protected virtual TData GetData()
		{
			TData ret = GetCache();

			if (ret == null)
			{
				ret = GetNativeData();
				SetCache(ret);
			}

			return ret;
		}

		protected virtual TData GetCache()
		{
			TData ret = null;
			string sdata;

			var ctx = HttpContext.Current;

			if (ctx != null)
			{
				var ctxitem = ctx.Items[CacheKey];
				if (ctxitem != null && ctxitem is TData)
				{
					ret = (TData)ctxitem;
				}
			}

			if (ret == null)
			{
				lock (_lock)
				{
					SPSecurity.RunWithElevatedPrivileges(() =>
					{
						using (SPSite esite = new SPSite(SiteID))
						{
							var expired = true;
							object oexpire = esite.RootWeb.AllProperties[CacheExpireKey];

							if (oexpire != null && oexpire is DateTime)
							{
								var dexpire = (DateTime)oexpire;
								expired = (DateTime.Now >= dexpire);
							}

							if (!expired)
							{
								sdata = (string)esite.RootWeb.AllProperties[CacheKey];
								if (!string.IsNullOrEmpty(sdata))
								{
									ret = Deserialize(sdata);
								}
							}
						}
					});
				}
			}

			if (ctx != null)
				ctx.Items[CacheKey] = ret;

			return ret;
		}
	

		protected virtual void SetCache(TData data)
		{
			string sdata;

			lock (_lock)
			{
				SPSecurity.RunWithElevatedPrivileges(() =>
				{
					using (SPSite esite = new SPSite(SiteID))
					{
						sdata = Serialize(data);
						esite.RootWeb.AllProperties[CacheKey] = sdata;
						esite.RootWeb.AllProperties[CacheExpireKey] = DateTime.Now.AddMinutes(CacheTimeoutMinutes);
						//esite.RootWeb.AllProperties[CacheExpireKey] = DateTime.Now.AddSeconds(30);

						esite.RootWeb.AllowUnsafeUpdates = true;
						esite.RootWeb.Update();
					}
				});
			}
		}


		protected virtual string Serialize(TData data)
		{
			return SerializeInternal(data);
		}

		protected virtual TData Deserialize(string data)
		{
			return DeserializeInternal<TData>(data);
		}


		protected string SerializeInternal(object data)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				XmlSerializer serializer = new XmlSerializer(data.GetType());
				serializer.Serialize(ms, data);

				ms.Flush();
				ms.Position = 0;

				using (StreamReader sr = new StreamReader(ms))
				{
					string ret = sr.ReadToEnd();

					// remove these lines if you need to strip the xml declaration adn the xsi and xmlns namespaces
					//ret = Regex.Replace(ret, @"<\?(.*?)\?>", "");
					//ret = Regex.Replace(ret, "xmlns:xsi=\"(.+?)\"", "");
					//ret = Regex.Replace(ret, "xmlns:xsd=\"(.+?)\"", "");

					return ret;
				}
			}
		}

		protected T DeserializeInternal<T>(string data)
		{
			using (XmlTextReader tr = new XmlTextReader(new StringReader(data)))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(T));
				object o = serializer.Deserialize(tr);
				return (T)o;
			}
		}


		public virtual void InvalidateCache()
		{
			var ctx = HttpContext.Current;

			if (ctx != null)
				ctx.Items[CacheKey] = null;

			SPSecurity.RunWithElevatedPrivileges(() =>
			{
				using (SPSite esite = new SPSite(SiteID))
				{
					esite.RootWeb.AllProperties[CacheKey] = null;
					esite.RootWeb.AllProperties[CacheExpireKey] = null;

					esite.RootWeb.AllowUnsafeUpdates = true;
					esite.RootWeb.Update();
				}
			});
		}

	}
}
