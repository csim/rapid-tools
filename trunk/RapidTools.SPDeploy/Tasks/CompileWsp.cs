using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text;
using System.Text.RegularExpressions;


namespace RapidTools.SPDeploy.Tasks
{

	public class CompileWsp : Task {

		private string _manifestPath;
		private string _workingDirectory;
		private string _projectDirectory;
		private string _outputPath;
		private string _tokens;

		private ITaskItem[] _rootFiles;
		private ITaskItem[] _templateFiles;
		private ITaskItem[] _featureManifestFiles;
		private ITaskItem[] _assemblyFiles;
		private ITaskItem[] _tokenReplacementFiles;

		private XmlDocument _manifest;
		private XmlNamespaceManager _nsmanager;
		private string _spnamespace = "http://schemas.microsoft.com/sharepoint/";
		private StringBuilder _ddf;

		[Required]
		public string ManifestPath {
			get { return _manifestPath; }
			set { _manifestPath = value; }
		}

		[Required]
		public string WorkingDirectory {
			get { return _workingDirectory; }
			set { _workingDirectory = value; }
		}

		[Required]
		public string ProjectDirectory {
			get { return _projectDirectory; }
			set { _projectDirectory = value; }
		}


		[Required]
		public string OutputPath {
			get { return _outputPath; }
			set { _outputPath = value; }
		}

		[Required]
		public ITaskItem[] RootFiles {
		  get { return _rootFiles; }
			set { _rootFiles = value; }
		}

		[Required]
		public ITaskItem[] TemplateFiles {
			get { return _templateFiles; }
			set { _templateFiles = value; }
		}

		[Required]
		public ITaskItem[] FeatureManifestFiles {
			get { return _featureManifestFiles; }
			set { _featureManifestFiles = value; }
		}

		[Required]
		public ITaskItem[] AssemblyFiles {
			get { return _assemblyFiles; }
			set { _assemblyFiles = value; }
		}


		public override bool Execute() {

			Log.LogMessage(string.Format("Compiling CAB to {0}", OutputPath));

			string outputFilename = Path.GetFileName(OutputPath);
			string outputDirectory = Path.GetDirectoryName(OutputPath);

			string tddfPath = string.Format(@"{0}{1}.ddf", WorkingDirectory, outputFilename);
			string tManifestPath = string.Format(@"{0}{1}.manifest.xml", WorkingDirectory, outputFilename);




			// Setup manifest file
			if (!File.Exists(ManifestPath)) {
				Log.LogError("Manifest file does not exist at {0}", ManifestPath);
				return false;
			}

			_manifest = new XmlDocument();
			_manifest.Load(ManifestPath);

			_nsmanager = new XmlNamespaceManager(_manifest.NameTable);
			_nsmanager.AddNamespace("sp", _spnamespace);


			// Setup ddf file
			_ddf = new StringBuilder();

			_ddf.AppendFormat(";\n.OPTION EXPLICIT\n.set DiskDirectoryTemplate=CDROM\n.set CompressionType=MSZIP\n.set UniqueFiles=\"ON\"\n.set Cabinet=\"ON\"\n.set DiskDirectory1=\n.set CabinetNameTemplate=\"{0}\"\n.set MaxDiskSize=0\n", OutputPath);
			_ddf.AppendFormat("\"{0}\" \"Manifest.xml\"\n", tManifestPath);


			if (FeatureManifestFiles.Length > 0) {
				XmlElement xFeatures = EnsureRootElement("FeatureManifests");
				BuildItems(FeatureManifestFiles, xFeatures, "FeatureManifest", false);
			}

			if (RootFiles.Length > 0) {
				XmlElement xRootFiles = EnsureRootElement("RootFiles");
				BuildItems(RootFiles, xRootFiles, "RootFile", false);
			}

			if (TemplateFiles.Length > 0) {
				XmlElement xTemplateFiles = EnsureRootElement("TemplateFiles");
				BuildItems(TemplateFiles, xTemplateFiles, "TemplateFile", false);
			}

			if (AssemblyFiles.Length > 0) {
				BuildItems(AssemblyFiles, true);
			}

			Log.LogMessage(string.Format("Writing makecab ddf to {0}", tddfPath));
			//Log.LogMessage(_ddf.ToString());

			if (File.Exists(tddfPath)) File.Delete(tddfPath);
			File.WriteAllText(tddfPath, _ddf.ToString(), Encoding.ASCII);


			Log.LogMessage(string.Format("Writing manifest to {0}", tManifestPath));
			if (File.Exists(tManifestPath)) File.Delete(tManifestPath);
			_manifest.Save(tManifestPath);


			if (File.Exists(OutputPath)) File.Delete(OutputPath);


			// Make the CAB file
			ProcessStartInfo pinfo = new ProcessStartInfo("makecab.exe");

			pinfo.WorkingDirectory = WorkingDirectory;
			pinfo.WindowStyle = ProcessWindowStyle.Hidden;
			pinfo.UseShellExecute = false;
			pinfo.CreateNoWindow = true;
			pinfo.Arguments = string.Format("/F \"{0}\"", tddfPath);
			pinfo.RedirectStandardOutput = true;
			pinfo.RedirectStandardError = true;
			
			Log.LogMessage(string.Format("Writing WSP to {0}", OutputPath));

			Process p = Process.Start(pinfo);
			string moutput = p.StandardOutput.ReadToEnd();
			p.WaitForExit();

			//Log.LogMessage(moutput);

			string eoutput = p.StandardError.ReadToEnd();
			bool error = (p.ExitCode != 0);
			if (error) {
				Log.LogMessage(moutput);
				Log.LogMessage(eoutput);
				Log.LogError("CompileWsp failed.");
				return false;
			}

			return true;
		}


		private string TranslateToWspPath(string path, string basePath) {

			if (!basePath.EndsWith(@"\"))
				basePath = basePath + @"\";

			if (path.ToLower().StartsWith(basePath.ToLower())) {
				return path.Substring(basePath.Length);
			}

			return path;

		}

		
		private XmlElement EnsureRootElement(string name) {


			XmlElement root = (XmlElement)_manifest.SelectSingleNode(string.Format("//sp:{0}", name), _nsmanager);
			if (root == null) {
				root = _manifest.CreateElement(name, _spnamespace);
				_manifest.DocumentElement.AppendChild(root);
			}

			return root;

		}

		private void BuildItems(ITaskItem[] items, bool flatten) {
			BuildItems(items, null, null, flatten);
		}

		private void BuildItems(ITaskItem[] items, XmlElement xManifestNode, string itemNodeName, bool flatten) {

			string itemFullPath, itemRelativPath, itemWspPath, itemFilename, itemExtension, itemBaseDir;
			bool includeInManifest;
			XmlElement xItem;

			foreach (ITaskItem item in items) {
				itemBaseDir = item.GetMetadata("BaseDir");
				itemFullPath = item.GetMetadata("FullPath");
				itemWspPath = TranslateToWspPath(itemFullPath, itemBaseDir);
				itemFilename = item.GetMetadata("Filename");
				itemExtension = item.GetMetadata("Extension");

				itemRelativPath = itemFullPath.Replace(ProjectDirectory, @"..\..");

				includeInManifest = string.IsNullOrEmpty(item.GetMetadata("IncludeInManifest")) ? true : Convert.ToBoolean(item.GetMetadata("IncludeInManifest"));

				if (flatten) itemWspPath = Path.GetFileName(itemWspPath);

				_ddf.AppendFormat("\"{0}\" \"{1}\"\n", itemRelativPath, itemWspPath);

				if (includeInManifest && (xManifestNode != null || !string.IsNullOrEmpty(itemNodeName))) {
					xItem = xManifestNode.OwnerDocument.CreateElement(itemNodeName, _spnamespace);
					xItem.SetAttribute("Location", itemWspPath);
					xManifestNode.AppendChild(xItem);
				}

				//Log.LogMessage(itemBaseDir);
			}


		}


	}


}
