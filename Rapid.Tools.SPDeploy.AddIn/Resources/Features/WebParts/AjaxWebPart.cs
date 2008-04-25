using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;

namespace [REPLACEPROJECTNAME].Web.UI.WebControls
{
    public class [REPLACEWEBPARTFILENAME] : WebPart
    {

        private ScriptManager scriptMan;
        public ScriptManager CurrentScriptManager
        {
            get
            {
                if (scriptMan == null)
                {
                    scriptMan = ScriptManager.GetCurrent(this.Page);
                    if (scriptMan == null)
                    {
                        scriptMan = new ScriptManager();
                        this.Controls.Add(scriptMan);
                    }
                }
                return scriptMan;
            }
        }

        protected override void CreateChildControls()
        {
            // add reference to client-side script
            ScriptReference Hello2Script = new ScriptReference(
                "[REPLACEPROJECTNAME].Web.UI.WebControls.WebPartResources.HelloScript.js", this.GetType().Assembly.FullName);
            this.CurrentScriptManager.Scripts.Add(Hello2Script);
            // parse together path to ASMX file
            string wsUrl = SPContext.Current.Web.Url + "/_vti_bin/TPG/HelloWebService.asmx";
            // add reference to ASMX Web Service
            ServiceReference serviceReference = new ServiceReference(wsUrl);
            this.CurrentScriptManager.Services.Add(serviceReference);
            // add client-side variable with ASMX path
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
                                                             "wsUrl",
                                                             string.Format("window.wsUrl='{0}';", wsUrl),
                                                             true);
        }

        private const string html =
        @"<div>
        <input id='btnCallWebService' type='button' value='Call Web Service' onclick='CallWebService()' />
        <hr/>
        <span id='DisplaySpan' style='font-size:14'></span>
      </div>";

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.Write(html);
        }
    }
}