using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections.Specialized;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;

namespace Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems
{
	public class WebPart : ElementManifest.IManifestNode
    {
        private string _fileName;

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        private NameValueCollection _properties = new NameValueCollection();

        public NameValueCollection Properties
        {
            get { return _properties; }
            set { _properties = value; }
        }

        public WebPart() { }
        public WebPart(XmlNode node, XmlNamespaceManager manager)
        {
            XmlNodeList nodes = node.SelectNodes("n:File", manager);
            foreach (XmlNode n in nodes)
            {
                if (n.Attributes["Url"] != null)
                    FileName = n.Attributes["Url"].Value;

                XmlNodeList nl = n.SelectNodes("n:Property", manager);

                for (int i = 0; i < nl.Count; i++)
                {
                    Properties.Add(nl[i].Attributes["Name"].Value, nl[i].Attributes["Value"].Value);
                }
            }
        }



        #region IManifestNode Members

        public void CreateNode(ref XmlWriter writer)
        {
            writer.WriteStartElement("Module");
            writer.WriteAttributeString("Url","_catalogs/wp");
            writer.WriteAttributeString("RootWebOnly", "TRUE");
            writer.WriteStartElement("File");
            writer.WriteAttributeString("Url", FileName);
            writer.WriteAttributeString("Type", "GhostableInLibrary");
            foreach (string s in Properties.Keys)
            {
                writer.WriteStartElement("Property");
                writer.WriteAttributeString("Name", s);
                writer.WriteAttributeString("Value", Properties[s]);
                writer.WriteEndElement();
                
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        #endregion
    }
}
