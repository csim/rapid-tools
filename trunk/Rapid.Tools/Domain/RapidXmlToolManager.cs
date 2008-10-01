using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Rapid.Tools.Providers.Provision;
using Rapid.Tools.Domain.Utilities;

namespace Rapid.Tools.Domain
{
	public class RapidXmlToolManager
	{

		private bool _initialized = false;
		private string _manifestDirectory;
		private string _manifestPath;

		private SPWebApplication _webApplication;
		private XmlDocument _manifest;

		private Dictionary<string, RapidXmlToolProviderBase> _providers;

		private List<TextWriter> _outputListeners;
		private string _messageLabel;
		private string _manifestRootNodeName;

		public Dictionary<string, RapidXmlToolProviderBase> Providers
		{
			get { return _providers; }
			set { _providers = value; }
		}

		public List<TextWriter> OutputListeners
		{
			get { return _outputListeners; }
			set { _outputListeners = value; }
		}

		public string MessageLabel
		{
			get { return _messageLabel; }
			set { _messageLabel = value; }
		}


		public virtual string ManifestRootNodeName
		{
			get { return _manifestRootNodeName; }
			set { _manifestRootNodeName = value; }
		}

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
		public XmlDocument Manifest
		{
			get { return _manifest; }
		}

		public RapidXmlToolManager(SPWebApplication webApplication, string manifestPath)
		{
			_webApplication = webApplication;
			_manifestPath = manifestPath;
			ManifestRootNodeName = "XmlTool";
		}


		protected void EnsureInitialize()
		{
			if (_initialized) return;
			
			MessageLabel = "System";

			Providers = new Dictionary<string, RapidXmlToolProviderBase>();

			OutputListeners = new List<TextWriter>();
			OutputListeners.Add(Console.Out);

			_manifest = new XmlDocument();
			_manifest.Load(_manifestPath);
			_manifestDirectory = Path.GetDirectoryName(_manifestPath);

			WriteMessage("Rapid Tools v{0} (c) Ascentium Corporation", RapidConfig.ApplicationVersion);

			XmlNodeList xproviders = Manifest.SelectNodes(string.Format("/{0}/Providers/Provider", ManifestRootNodeName));

			string name, type;
			object instance;
			Type ptype;
			RapidXmlToolProviderBase provider;

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

					if (instance is RapidXmlToolProviderBase)
					{
						provider = (RapidXmlToolProviderBase)instance;
						Providers.Add(name, provider);
					}
					else
					{
						WriteError(string.Format("Provider for \"{0}\" is not a valid Provider.", name, type));
					}
				}
			}
			WriteMessage("Providers Initialized.");
			_initialized = true;
		}

		protected virtual RapidXmlToolProviderBase GetProvider(string elementName)
		{
			EnsureInitialize();
			if (!Providers.ContainsKey(elementName))
			{
				WriteError(string.Format("No provider available.", elementName));
				return null;
			}

			RapidXmlToolProviderBase provider = Providers[elementName];
			if (provider == null || !(provider is RapidXmlToolProviderBase)) WriteError(string.Format("Provider invalid for element \"{0}\".", elementName));

			return provider;
		}


		public virtual void ExecuteChildren(XmlElement contextElement)
		{
			EnsureInitialize();
			ExecuteChildren(contextElement, null);
		}

		public virtual void ExecuteChildren(XmlElement contextElement, RapidXmlToolProviderContext context)
		{
			EnsureInitialize();
			XmlElement element;
			

			foreach (XmlNode node in contextElement.ChildNodes)
			{
				if (node is XmlElement && node.Name != "Providers")
				{
					element = (XmlElement)node;
					Execute(element, context);
				}
			}

			
		}


		public virtual void Execute(XmlElement element)
		{
			EnsureInitialize();
			Execute(element, null);
		}

		public virtual void Execute(XmlElement element, RapidXmlToolProviderContext context)
		{
			EnsureInitialize();
			
			string olabel = MessageLabel;
			MessageLabel = element.Name;

			if (element == null) throw new ArgumentException("element cannot be null.");

			RapidXmlToolProviderBase provider = GetProvider(element.Name);
			if (provider == null) return;

			provider.Manager = this;

			try
			{
				provider.Execute(element, context);
			}
			catch (Exception ex)
			{
				WriteError(ex);
			}

			MessageLabel = olabel;
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
				writer.WriteLine("{0,14} {1}", "[" + MessageLabel + "]", message);
			}

			Console.ResetColor();
		}


	}
}
