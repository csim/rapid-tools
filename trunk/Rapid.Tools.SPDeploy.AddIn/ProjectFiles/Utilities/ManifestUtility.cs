using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Rapid.Tools.SPDeploy.AddIn.ProjectFiles.Utilities
{
    public class ManifestUtility
    {
        public static void AddAttribute(ref XmlWriter writer, string propertyName, string propertyValue)
        {
            if (!string.IsNullOrEmpty(propertyValue))
                writer.WriteAttributeString(propertyName, propertyValue);
        }
        public static void AddAttribute(ref XmlWriter writer, string propertyName, int? propertyValue)
        {
            if (propertyValue.HasValue)
                writer.WriteAttributeString(propertyName, propertyValue.Value.ToString());
        }
        public static void AddAttribute(ref XmlWriter writer, string propertyName, bool? propertyValue)
        {
            if (propertyValue.HasValue)
                writer.WriteAttributeString(propertyName, propertyValue.Value.ToString());
        }
    }
}
