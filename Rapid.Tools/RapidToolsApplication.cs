using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using Rapid.Tools.Utilities;

namespace Rapid.Tools
{

	public class RapidToolsApplication : Microsoft.SharePoint.ApplicationRuntime.SPHttpApplication
	{

		public RapidToolsApplication()
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

				if (RapidToolsConfig.Exceptions.Print)
					SPExceptionUtil.Print(ex);

				if (RapidToolsConfig.Exceptions.Print)
					Response.End();

			}
			catch (System.Threading.ThreadAbortException)
			{
			}

		}

	}
}


