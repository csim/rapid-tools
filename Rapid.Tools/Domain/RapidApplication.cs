using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using Rapid.Tools.Domain.Utilities;

namespace Rapid.Tools.Domain
{
	public class RapidApplication : Microsoft.SharePoint.ApplicationRuntime.SPHttpApplication
	{

		public RapidApplication()
			: base()
		{
			Error += new EventHandler(Global_Error);
		}

		private void Global_Error(object sender, EventArgs e)
		{

			Exception ex = Server.GetLastError();
			if (ex == null) return;

			if (ex is HttpUnhandledException) ex = ex.InnerException;

			try
			{
				if (RapidConfig.ExceptionDisplay.EndResponse)
				{
					SPExceptionUtil.Print(ex);
					Response.End();
				}
				else
				{
					Context.Items[RapidConfig.ExceptionDisplay.ContextKey] = ex;
					Server.Transfer(RapidConfig.ExceptionDisplay.DisplayUrl);
				}
			}
			catch (System.Threading.ThreadAbortException)
			{
			}

		}

	}
}


