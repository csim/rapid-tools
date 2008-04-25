using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Ajaxification
{
    public class FeatureReceiver : SPFeatureReceiver
    {

        public override void FeatureInstalled(SPFeatureReceiverProperties properties) { }
        public override void FeatureUninstalling(SPFeatureReceiverProperties properties) { }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication WebApp = (SPWebApplication)properties.Feature.Parent;
            foreach (ModificationEntry entry in Entries)
            {
                WebApp.WebConfigModifications.Add(
                  CreateModification(entry.Name, entry.XPath, entry.Value)
                );
            }
            SPFarm.Local.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication WebApp = (SPWebApplication)properties.Feature.Parent;
            foreach (ModificationEntry entry in Entries)
            {
                WebApp.WebConfigModifications.Remove(
                  CreateModification(entry.Name, entry.XPath, entry.Value)
                );
            }
            SPFarm.Local.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
        }

        private struct ModificationEntry
        {
            public string Name;
            public string XPath;
            public string Value;
            public ModificationEntry(string Name, string XPath, string Value)
            {
                this.Name = Name;
                this.XPath = XPath;
                this.Value = Value;
            }
        }

        private ModificationEntry[] Entries = {
      new ModificationEntry("add[@path='*.asmx']",
                            "configuration/system.web/httpHandlers",
                            @"<add verb=""*"" path=""*.asmx"" validate=""false"" type=""System.Web.Script.Services.ScriptHandlerFactory, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" />"),
      new ModificationEntry("add[@path='ScriptResource.axd']",
                            "configuration/system.web/httpHandlers",
                            @"<add verb=""GET,HEAD"" path=""ScriptResource.axd"" type=""System.Web.Handlers.ScriptResourceHandler, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" validate=""false""/>"),
      new ModificationEntry("add[@path='*_AppService.axd']",
                            "configuration/system.web/httpHandlers",
                            @"<add verb=""*"" path=""*_AppService.axd"" type=""System.Web.Script.Services.ScriptHandlerFactory, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" validate=""false"" />"),
      new ModificationEntry("add[@name='ScriptModule']",
                            "configuration/system.web/httpModules",
                            @"<add name=""ScriptModule"" type=""System.Web.Handlers.ScriptModule, System.Web.Extensions, Version=1.0.61025.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"" />")
    };

        public SPWebConfigModification CreateModification(string Name, string XPath, string Value)
        {
            SPWebConfigModification modification = new SPWebConfigModification(Name, XPath);
            modification.Owner = "Ajaxification";
            modification.Sequence = 0;
            modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
            modification.Value = Value;
            return modification;
        }
    }
}
