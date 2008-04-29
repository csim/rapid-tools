using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.Proxies.AddIn;
using Rapid.Tools.SPDeploy.AddIn.Proxies.Webs;
using Rapid.Tools.SPDeploy.AddIn.Domain.Utilties;

namespace Rapid.Tools.SPDeploy.AddIn.Domain
{
    public class ProxyBridge
    {
		private AddInProxy _addInService = null;
		private WebsProxy _websService = null;

		public AddInProxy AddInService
        {
            get
            {
				if (_addInService == null)
                {
					string host = AppManager.Instance.GetWebApplicationUrl();

					_addInService = new AddInProxy();
					_addInService.Credentials = CredentialCache.DefaultNetworkCredentials;
					_addInService.Url = string.Format("{0}/_layouts/RapidTools/Services/AddIn.asmx", host);
                }
				return _addInService;
            }
        }

		public WebsProxy WebsService
		{
			get
			{
				if (_websService == null)
				{
					string host = AppManager.Instance.GetWebApplicationUrl();

					_websService = new WebsProxy();
					_websService.Credentials = CredentialCache.DefaultNetworkCredentials;
					_websService.Url = string.Format("{0}/_vti_bin/Webs.asmx", host);
				}
				return _websService;
			}
		}


    }
}
