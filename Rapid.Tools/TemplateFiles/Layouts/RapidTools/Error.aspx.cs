using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.SharePoint;
using Rapid.Tools.Utilities;

namespace Rapid.Tools.Layouts
{
	public partial class ErrorPage : System.Web.UI.Page
	{

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Exception ex = (Exception)Context.Items[RapidToolsConfig.ExceptionDisplay.ContextKey];

			if (ex != null)
			{
				litShortException.Text = ex.Message;

				litFullException.Text = SPExceptionUtil.Format(ex, true);

				pnlShortException.Visible = !RapidToolsConfig.ExceptionDisplay.PrintStack;
				litFullException.Visible = RapidToolsConfig.ExceptionDisplay.PrintStack;

			}
		}

	}
}
