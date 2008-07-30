using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.Proxies.AddIn;
using Rapid.Tools.SPDeploy.AddIn.Proxies.Webs;

namespace Rapid.Tools.SPDeploy.AddIn.Domain
{
    public class ProxyBridge
    {
		public string _baseUrl;
		private AddInWebService _addInService = null;
		private WebsWebService _websService = null;

		public ProxyBridge(string baseUrl)
		{
			_baseUrl = baseUrl;
		}

		public AddInWebService AddInService
        {
            get
            {
				if (_addInService == null)
                {
					_addInService = new AddInWebService();
					_addInService.Credentials = CredentialCache.DefaultCredentials;
					_addInService.Url = string.Format("{0}/_layouts/RapidTools/Services/AddIn.asmx", _baseUrl);
                }
				return _addInService;
            }
        }

		public WebsWebService WebsService
		{
			get
			{
				if (_websService == null)
				{
					_websService = new WebsWebService();
					_websService.Credentials = CredentialCache.DefaultCredentials;
					_websService.Url = string.Format("{0}/_vti_bin/Webs.asmx", _baseUrl);
				}
				return _websService;
			}
		}


    }
}
