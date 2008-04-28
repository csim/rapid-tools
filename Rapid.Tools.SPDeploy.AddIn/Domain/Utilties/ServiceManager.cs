using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.Proxies.AddIn;

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

        private AddInProxy _serviceInstance;

        public AddInProxy ServiceInstance
        {
            get
            {
                if (_serviceInstance == null)
                {
                    _serviceInstance = new AddInProxy();
                    _serviceInstance.Credentials = defaultCredentials;
                    _serviceInstance.Url = url + "/_layouts/RapidTools/Services/AddIn.asmx";
                }
                return _serviceInstance;
            }
            set { _serviceInstance = value; }
        }



    }
}
