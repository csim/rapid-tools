using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.Utilties
{
    public class ServiceManager
    {
        private static readonly ServiceManager instance = new ServiceManager();

        private NetworkCredential defaultCredentials = CredentialCache.DefaultNetworkCredentials;
        private string url = string.Empty;

        private ServiceManager()
        {
            url = Functions.GetSiteUrlFromProject();
        }

        public static ServiceManager Instance
        {
            get { return instance; }
        }

        private SPToolsWebService.SPToolsWebService _serviceInstance;

        public SPToolsWebService.SPToolsWebService ServiceInstance
        {
            get
            {
                if (_serviceInstance == null)
                {
                    _serviceInstance = new Rapid.Tools.SPDeploy.AddIn.SPToolsWebService.SPToolsWebService();
                    _serviceInstance.Credentials = defaultCredentials;
                    _serviceInstance.Url = url + "/_layouts/SPTools/SPToolsWebService.asmx";
                }
                return _serviceInstance;
            }
            set { _serviceInstance = value; }
        }



    }
}
