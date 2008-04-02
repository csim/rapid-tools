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

namespace Rapid.Tools
{
	public static class RapidToolsConfig
	{
		public static class Exceptions
		{

			public static bool Print
			{
				get
				{
					return Convert.ToBoolean(WebConfigurationManager.AppSettings["Rapid.Tools.exceptions.print"]);
				}
			}

			public static bool Log
			{
				get
				{
					return Convert.ToBoolean(WebConfigurationManager.AppSettings["Rapid.Tools.exceptions.log"]);
				}
			}

		}
	}
}
