using System;
using System.IO;
using System.Xml;
using System.Web;
using System.Web.Configuration;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Web.Caching;

namespace Rapid.Tools.Domain
{
	public static class RapidConfig
	{
		public static string ApplicationVersion
		{
			get
			{
				return Convert.ToString(WebConfigurationManager.AppSettings["Application.Version"]);
			}
		}

		public static class ExceptionDisplay
		{
			public static bool PrintStack
			{
				get
				{
					return Convert.ToBoolean(WebConfigurationManager.AppSettings["Rapid.Tools.ExceptionDisplay.PrintStack"]);
				}
			}

			public static string DisplayUrl
			{
				get
				{
					return Convert.ToString(WebConfigurationManager.AppSettings["Rapid.Tools.ExceptionDisplay.DisplayUrl"]);
				}
			}

			public static bool EndResponse
			{
				get
				{
					return Convert.ToBoolean(WebConfigurationManager.AppSettings["Rapid.Tools.ExceptionDisplay.EndResponse"]);
				}
			}

			//public static bool Log
			//{
			//    get
			//    {
			//        return Convert.ToBoolean(WebConfigurationManager.AppSettings["Rapid.Tools.ExceptionDisplay.Log"]);
			//    }
			//}

			public static string ContextKey
			{
				get
				{
					return "Rapid.Tools.Exception";
				}
			}

		}
	}
}
