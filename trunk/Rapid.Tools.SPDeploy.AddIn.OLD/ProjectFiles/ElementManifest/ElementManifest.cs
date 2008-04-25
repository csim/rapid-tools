using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest.ManifestItems;
using System.IO;

namespace Rapid.Tools.SPDeploy.AddIn.ProjectFiles.ElementManifest
{
    public class ElementManifest
    {
        public ElementManifest() { }
        public ElementManifest(XmlDocument document)
        {
            XmlNamespaceManager nm = new XmlNamespaceManager(document.NameTable);
            nm.AddNamespace("n", "http://schemas.microsoft.com/sharepoint/");
            foreach (XmlNode node in document.SelectNodes("/n:Elements/*", nm))
            {
                switch (node.Name)
                {
                    case "CustomAction":
                        Nodes.Add(new CustomAction(node));
                        break;
                    case "CustomActionGroup":
                        Nodes.Add(new CustomActionGroup(node));
                        break;
                    case "HideCustomAction":
                        Nodes.Add(new HideCustomAction(node));
                        break;
                    case "Module":
                        if (node.Attributes["Url"].Value == "_catalogs/wp")
                            Nodes.Add(new ProjectFiles.ElementManifest.ManifestItems.WebPart(node, nm));
                        break;
                    case "Control":
                        Nodes.Add(new Control(node));
                        break;
                }
            }

        }



        public List<IManifestNode> Nodes = new List<IManifestNode>();

        public void WriteManifest(string fileLocation)
        {
            using (StreamWriter manifestStream = new StreamWriter(fileLocation))
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = Encoding.UTF8;
                settings.Indent = true;
                settings.NewLineOnAttributes = false;
                settings.NewLineHandling = NewLineHandling.None;
                XmlWriter writer = XmlWriter.Create(manifestStream, settings);

                writer.WriteStartDocument();

                writer.WriteStartElement("Elements", "http://schemas.microsoft.com/sharepoint/");
                foreach (IManifestNode node in Nodes)
                    node.CreateNode(ref writer);

                writer.WriteEndElement();

                writer.WriteEndDocument();

                writer.Flush();
                writer.Close();
            }


        }

        public interface IManifestNode
        {
            void CreateNode(ref XmlWriter writer);
        }
    }
}
