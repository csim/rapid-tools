using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.Utilities;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest;

namespace Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems
{
	public class Control : ElementManifest.IManifestNode
    {
        private string _id;

        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private int _sequence;

        public int Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }
      

        public Control() { }
        public Control(XmlNode node)
        {
            Id = node.Attributes["Id"].Value;
            Sequence = Convert.ToInt32(node.Attributes["Sequence"].Value);            
        }


        #region IManifestNode Members

        public void CreateNode(ref System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Control");
            ManifestUtility.AddAttribute(ref writer, "Id", Id);
            ManifestUtility.AddAttribute(ref writer, "Sequence", Sequence);
            ManifestUtility.AddAttribute(ref writer, "Url", string.Format("/templates/{0}.ascx", Id));
            writer.WriteEndElement();
        }

        #endregion
    }
}
