using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Web;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using RapidTools.Utilities;
using RapidTools.Provision.Providers;

namespace RapidTools.Provision
{

	public class RapidProvisionManager
	{

		public List<TextWriter> OutputListeners;
		private string _messageLabel = "System";


		private SPWebApplication _webApplication;
		private XmlDocument _manifest;
		private Dictionary<string, AssetProviderBase> _assetProviders;

		private string _manifestDirectory;
		private string _manifestPath;

		public SPWebApplication WebApplication
		{
			get { return _webApplication; }
		}

		public string ManifestDirectory
		{
			get { return _manifestDirectory; }
		}
		public string ManifestPath
		{
			get { return _manifestPath; }
		}

		public Dictionary<string, AssetProviderBase> AssetProviders
		{
			get { return _assetProviders; }
		}

		public XmlDocument Manifest
		{
			get { return _manifest; }
		}


		public RapidProvisionManager(SPWebApplication webApplication, string manifestPath)
		{
			Initialize(webApplication, manifestPath);
		}

		protected void Initialize(SPWebApplication webApplication, string manifestPath)
		{

			OutputListeners = new List<TextWriter>();

			OutputListeners.Add(Console.Out);

			_assetProviders = new Dictionary<string, AssetProviderBase>();

			_webApplication = webApplication;

			_manifest = new XmlDocument();
			_manifest.Load(manifestPath);

			_manifestPath = manifestPath;
			_manifestDirectory = Path.GetDirectoryName(manifestPath);

			WriteMessage("RapidProvision v1.0 (c) Ascentium Corporation");
			WriteMessage("For more information contact Clint Simon (clints@ascentium.com)");

			XmlNodeList xproviders = Manifest.SelectNodes("/ProvisionManifest/AssetProviders/AssetProvider");

			string name, type;
			object instance;
			Type ptype;
			AssetProviderBase provider;

			foreach (XmlElement xprovider in xproviders)
			{
				name = xprovider.GetAttribute("Name");
				type = xprovider.GetAttribute("Type");

				ptype = Type.GetType(type);

				if (ptype == null)
				{

					WriteError(string.Format("Unable to load provider type for \"{0}\" element.", name, type));

				}
				else
				{

					instance = Activator.CreateInstance(ptype);

					if (instance is AssetProviderBase)
					{
						provider = (AssetProviderBase)instance;
						provider.Manager = this;
						AssetProviders.Add(name, provider);
					}
					else
					{
						WriteError(string.Format("Provider for \"{0}\" is not a valid Asset Provider.", name, type));
					}
				}
			}

		}


		public void Import()
		{

			WriteMessage("Begin import from");
			WriteMessage(ManifestPath);

			ImportChildAssets(Manifest.DocumentElement);

		}


		public void ImportChildAssets(XmlElement contextElement)
		{
			ImportChildAssets(contextElement, null);
		}

		public void ImportChildAssets(XmlElement contextElement, RapidProvisionContext context)
		{
			XmlElement element;

			foreach (XmlNode node in contextElement.ChildNodes)
			{
				if (node is XmlElement && node.Name != "AssetProviders")
				{
					element = (XmlElement)node;
					_messageLabel = element.Name;
					ImportAsset(element, context);
				}
			}

		}


		public void ImportAsset(XmlElement assetElement)
		{
			ImportAsset(assetElement, null);
		}

		public void ImportAsset(XmlElement assetElement, RapidProvisionContext context)
		{

			if (assetElement == null) throw new ArgumentException("assetElement cannot be null.");

			string name = assetElement.Name;

			AssetProviderBase provider;

			if (AssetProviders.ContainsKey(name))
			{

				provider = AssetProviders[name];
				if (provider == null || !(provider is AssetProviderBase)) WriteError(string.Format("Asset provider invalid for element \"{0}\".", name));

				try
				{
					provider.Import(assetElement, context);
				}
				catch (Exception ex)
				{
					WriteError(ex);
				}

			}
			else
			{
				WriteError(string.Format("No provider available.", name));
			}

		}




		public void Export()
		{
		}

		public void ExportAssset()
		{
		}



		/*** Utility Functions ****/

		public string GetFilePath(string manifestRelativeFilePath)
		{
			return Path.Combine(ManifestDirectory, manifestRelativeFilePath);
		}


		public void WriteMessage(string message)
		{
			WriteMessage(message, null);
		}
		public void WriteMessage(string message, params object[] args)
		{
			WriteLine(message, Console.ForegroundColor, args);
		}

		public void WriteWarning(string message)
		{
			WriteWarning(message, null);
		}
		public void WriteWarning(string message, params object[] args)
		{
			WriteLine(message, ConsoleColor.Yellow, args);
		}

		public void WriteError(Exception ex)
		{
			WriteError(ex, null);
		}
		public void WriteError(Exception ex, string additionalMessage)
		{
			string fex = SPExceptionUtil.Format(ex);
			WriteError("{0}\n\n{1}\n", ex.Message, fex);
		}
		public void WriteError(string message)
		{
			WriteError(message, null);
		}
		public void WriteError(string message, params object[] args)
		{
			WriteLine(message, ConsoleColor.Red, args);
		}

		private void WriteLine(string message, ConsoleColor color, params object[] args)
		{
			if (args != null) message = string.Format(message, args);

			Console.ForegroundColor = color;

			foreach (TextWriter writer in OutputListeners)
			{
				writer.WriteLine("{0,14} {1}", "[" + _messageLabel + "]", message);
			}

			Console.ResetColor();

		}

	}

}
