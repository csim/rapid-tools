using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;
using EnvDTE80;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.Proxies.AddIn;
using Rapid.Tools.SPDeploy.AddIn.Proxies.Lists;


namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class SPListNodeTag : NodeTag
    {
		public delegate void VoidDelegate();
		
		public SPListNodeTag(TreeNode node)
        {
            _node = node;
			TagType = NodeType.List;
        }


		public override void Focus()
		{
		}
		
		public override void DoubleClick()
        {
			try
			{
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
		}

        public override ContextMenu RightClick()
        {
            ContextMenu contextMenu = new ContextMenu();

			try
			{
				//if (((NodeTag)_node.Tag).IsDocumentLibrary)
				//{
				//    contextMenu.MenuItems.Add("Upload", delegate(object sender, EventArgs e)
				//    {
				//        OpenFileDialog _fileDialog = new OpenFileDialog();

				//        if (_fileDialog.ShowDialog() == DialogResult.OK)
				//        {
				//            ServiceInstance.AddFile(((NodeTag)_node.TreeView.Nodes[0].Tag).Url, Guid, _fileDialog.FileName.Substring(_fileDialog.FileName.LastIndexOf("\\") + 1), File.ReadAllBytes(_fileDialog.FileName));
				//        }
				//    });
				//}

				contextMenu.MenuItems.Add("View List Schema", delegate(object sender, EventArgs e)
				{
					string _schemaPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\SPDeploy\Workspace\Lists\", Node.Text, ".xml");

					AppManager.Current.EnsureDirectory(_schemaPath);

					File.WriteAllText(_schemaPath, SiteTag.ListsService.GetList(Node.Text).OuterXml);

					AppManager.Current.Application.ItemOperations.OpenFile(_schemaPath, EnvDTE.Constants.vsViewKindTextView);
				});

                if (AppManager.Current.ActiveProject != null)
                {
                    contextMenu.MenuItems.Add("Create Feature", delegate(object sendr, EventArgs e)
                    {

                        VoidDelegate d = new VoidDelegate(delegate()
                        {
                            try
                            {
                                string featureFile = string.Empty;
                                string schemaPath = string.Empty;
                                string _templateName = string.Empty;
                                string _templateDisplayName = string.Empty;
                                int _templateNumber = 6500;


                                UI.Forms.ListDefinitionDialog _listDialog = new Rapid.Tools.SPDeploy.AddIn.UI.Forms.ListDefinitionDialog();
                                _listDialog.ShowDialog();

                                _templateDisplayName = _listDialog.TemplateName;
                                _templateName = string.Join(string.Empty, _templateDisplayName.Split(' '));
                                _templateNumber = _listDialog.TemplateNumber;

                                foreach (string s in SiteTag.AddInService.featureFiles(SiteTag.ListsService.GetList(Node.Text).OuterXml))
                                {
                                    string projectPath = AppManager.Current.ActiveProject.FullName;
                                    projectPath = projectPath.Remove(projectPath.LastIndexOf("\\"));

                                    string fPath = s.Substring(s.IndexOf("\\TEMPLATE\\FEATURES\\"));
                                    fPath = fPath.Replace("\\TEMPLATE\\FEATURES\\", "");
                                    fPath = fPath.Substring(fPath.IndexOf("\\") + 1);
                                    fPath = "\\TemplateFiles\\Features\\" + _templateName + "\\" + fPath;

                                    AppManager.Current.EnsureDirectory(projectPath + fPath);

                                    string tempString = fPath.Substring(fPath.LastIndexOf("\\") + 1);


                                    if (tempString.ToLower() == "schema.xml")
                                    {
                                        string schema = SiteTag.AddInService.GetListSchema(WebTag.ID, ID);


                                        XmlDocument oSchema = new XmlDocument();
                                        Encoding enc = Encoding.UTF8;
                                        oSchema.LoadXml(enc.GetString(SiteTag.AddInService.OpenFile(s)));

                                        XmlDocument nSchema = new XmlDocument();
                                        nSchema.LoadXml(schema);

                                        try
                                        {

                                            oSchema.SelectSingleNode("/List/MetaData/Fields").InnerXml = nSchema.SelectSingleNode("/List/Fields").InnerXml;

                                            XmlDocument doc = new XmlDocument();
                                            doc.LoadXml(SiteTag.AddInService.GetViewNodes(WebTag.ID, ID).OuterXml);


                                            XmlNode formNode = oSchema.SelectSingleNode("/List/MetaData/Forms");

                                            int ii = 1;
                                            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                                            {
                                                XmlElement el = node as XmlElement;
                                                el.RemoveAttribute("Name");
                                                el.Attributes["Url"].Value = el.Attributes["Url"].Value.Substring(el.Attributes["Url"].Value.LastIndexOf("/") + 1);
                                                el.RemoveAttribute("Level");
                                                el.RemoveAttribute("ContentTypeID");
                                                el.SetAttribute("WebPartZoneID", "Main");
                                                el.SetAttribute("SetupPath", "pages\\viewpage.aspx");
                                                el.SetAttribute("BaseViewID", ii.ToString());
                                                ii++;

                                                bool found = false;
                                                foreach (XmlNode n in formNode.SelectNodes("Form"))
                                                {
                                                    if (n.Attributes["Url"].Value == el.Attributes["Url"].Value)
                                                        found = true;
                                                }

                                                if (!found)
                                                    formNode.InnerXml += "<Form Type=\"DisplayForm\" Url=\"" + el.Attributes["Url"].Value + "\" SetupPath=\"pages\\form.aspx\" WebPartZoneID=\"Main\" />";
                                            }

                                            string str = oSchema.SelectSingleNode("/List/MetaData/Views").ChildNodes[0].OuterXml;
                                            str += doc.DocumentElement.InnerXml;

                                            oSchema.SelectSingleNode("/List/MetaData/Views").InnerXml = str;
                                        }
                                        catch (Exception ex)
                                        {
                                        }

                                        oSchema.Save(projectPath + fPath);

                                        schemaPath = projectPath + fPath;

                                    }
                                    else if (tempString.ToLower() == "feature.xml")
                                    {
                                        featureFile = projectPath + fPath;
                                        XmlDocument fe = new XmlDocument();
                                        File.WriteAllBytes(projectPath + fPath, SiteTag.AddInService.OpenFile(s));
                                        fe.Load(projectPath + fPath);

                                        fe.DocumentElement.Attributes["Id"].Value = Guid.NewGuid().ToString().Replace("{", "").Replace("}", "");
                                        fe.DocumentElement.Attributes["Title"].Value = _templateDisplayName + " Feature";
                                        fe.DocumentElement.Attributes["Description"].Value = "...";

                                        fe.Save(featureFile);
                                    }
                                    else
                                        File.WriteAllBytes(projectPath + fPath, SiteTag.AddInService.OpenFile(s));

                                }

                                XmlDocument feature = new XmlDocument();
                                feature.Load(featureFile);

                                string element = feature.DocumentElement.ChildNodes[0].ChildNodes[0].Attributes["Location"].Value;

                                string ns = featureFile.Remove(featureFile.LastIndexOf("\\")) + "\\" + element;

                                element = element.Remove(element.LastIndexOf("\\"));
                                element += "\\" + _templateName + ".xml";
                                feature.DocumentElement.ChildNodes[0].ChildNodes[0].Attributes["Location"].Value = element;
                                feature.Save(featureFile);

                                XmlDocument doc2 = new XmlDocument();
                                doc2.Load(ns);

                                XmlNode nod = doc2.DocumentElement.ChildNodes[0];
                                nod.Attributes["Name"].Value = _templateName;
                                nod.Attributes["Type"].Value = _templateNumber.ToString();
                                nod.Attributes["DisplayName"].Value = _templateDisplayName;
                                nod.Attributes["Description"].Value = "...";

                                File.Delete(ns);
                                ns = ns.Remove(ns.LastIndexOf("\\"));
                                ns = ns + "\\" + _templateName + ".xml";
                                doc2.Save(ns);

                                string newFold = schemaPath.Remove(schemaPath.LastIndexOf("\\"));
                                newFold = newFold.Remove(newFold.LastIndexOf("\\"));

                                DirectoryInfo dir = new DirectoryInfo(newFold);
                                dir = dir.CreateSubdirectory(_templateName);

                                string pp = schemaPath.Remove(schemaPath.LastIndexOf("\\"));

                                string newschemapath = string.Empty;

                                foreach (FileInfo ff in new DirectoryInfo(pp).GetFiles("*"))
                                {
                                    File.WriteAllBytes(dir.FullName + "\\" + ff.Name, File.ReadAllBytes(ff.FullName));
                                    if (string.Compare(ff.Name, "schema.xml", true) == 0)
                                    {
                                        newschemapath = dir.FullName + "\\" + ff.Name;
                                    }
                                }


                                //File.WriteAllBytes(dir.FullName + "\\schema.xml", File.ReadAllBytes(schemaPath));
                                string tstring = dir.FullName;
                                dir = new DirectoryInfo(schemaPath.Remove(schemaPath.LastIndexOf("\\")));
                                if (dir.FullName != tstring)
                                    dir.Delete(true);

                                featureFile = featureFile.Remove(featureFile.LastIndexOf("\\"));


                                dir = new DirectoryInfo(featureFile);
                                foreach (FileInfo fi in dir.GetFiles("*", SearchOption.AllDirectories))
                                {
                                    AppManager.Current.ActiveProject.ProjectItems.AddFromFile(fi.FullName);
                                }


                                XmlDocument schemaDocument = new XmlDocument();
                                schemaDocument.Load(newschemapath);


                                schemaDocument.SelectSingleNode("/List/MetaData/ContentTypes").InnerXml = string.Empty;
                                foreach (string s in SiteTag.AddInService.GetContentTypeNames(WebTag.ID, ID))
                                {
                                    schemaDocument.SelectSingleNode("/List/MetaData/ContentTypes").InnerXml += s;
                                }


                                Proxies.AddIn.ListOptions lo = SiteTag.AddInService.GetOptions(WebTag.ID, ID);
                                if (lo.AllowContentTypes)
                                    schemaDocument.DocumentElement.SetAttribute("AllowContentTypes", "true");
                                if (lo.ContentTypesEnabled)
                                    schemaDocument.DocumentElement.SetAttribute("ContentTypesEnabled", "true");



                                schemaDocument.Save(newschemapath);



                            }
                            catch (Exception ee)
                            {

                            }
                        });

                        d.BeginInvoke(null, null);

                    });
                }
				contextMenu.MenuItems.Add("Browse", delegate(object sender, EventArgs e)
				{
					Browse();
				});

			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}

            return contextMenu;
        }
    }
}
