using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace Rapid.Tools.Provision
{
	public class RapidProvisionContext
	{

		private SPWeb _web;
		private SPList _list;
		private SPListItem _item;

		public RapidProvisionContext()
		{
		}

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

	}
}
