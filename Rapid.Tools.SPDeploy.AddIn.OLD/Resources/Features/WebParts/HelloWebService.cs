using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Services;
using System.Web.Script.Services;
using Microsoft.SharePoint;
using System.Security.Permissions;

namespace [REPLACEPROJECTNAME]
{
    // An example callback Web Service API for the Hello2 Web Part
    [WebService(Namespace = "http://[REPLACEMACHINENAME]/[REPLACEPROJECTNAME]")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class HelloWebService : System.Web.Services.WebService
    {

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod]
        public string HelloWorld()
        {
            string htmlFragment = "Hello world of AJAX callbacks" +
                                  "<br/>" +
                                  "The time is now " + DateTime.Now.ToLongTimeString();
            return htmlFragment;
        }

    }

}
