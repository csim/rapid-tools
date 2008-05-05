using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace Rapid.Tools.Domain
{
	public class RapidXmlToolProviderContext
	{

		public RapidXmlToolProviderContext()
		{
		}

		private SPWeb _web;
		private SPList _list;
		private SPListItem _item;
		private object _data;
		private NameValueCollection _arguments;

		public SPWeb Web
		{
			get { return _web; }
			set { _web = value; }
		}

		public SPList List
		{
			get { return _list; }
			set { _list = value; }
		}

		public SPListItem Item
		{
			get { return _item; }
			set { _item = value; }
		}

		public object Data
		{
			get { return _data; }
			set { _data = value; }
		}

		public NameValueCollection Arguments
		{
			get { return _arguments; }
			set { _arguments = value; }
		}

	}
}
