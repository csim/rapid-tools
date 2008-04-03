using System;
using System.Web.Configuration;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Rapid.Tools.Utilities;
using Rapid.Tools.Provision;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.StsAdmin;
using Microsoft.SharePoint.Administration;


namespace Rapid.Tools.Commands {
	public class Command : ISPStsadmCommand {

		public struct CommandNames {
			public const string Import = "rapidtools-import";

			public const string EnableExceptionHandling = "rapidtools-enableexceptionhandling";
			public const string DisableExceptionHandling = "rapidtools-disableexceptionhandling";
		}

		public const string SuccessMessage = "\nOperation completed successfully.";


		public void PrintHeader() {
			Console.WriteLine("Rapid Tools v1.0 (c) Ascentium Corporation");
			Console.WriteLine("For more information contact Clint Simon (clints@ascentium.com)");
		}

		public string PrintHelpMessage(string command) {
			string help = GetHelpMessage(command);
			return string.Format("stsadm -o {0}\n\t{1}", command, help);
		}

		public string GetHelpMessage(string command) {
			StringBuilder help = new StringBuilder();

			switch (command.ToLower()) {

			case CommandNames.Import:
				help.AppendLine(" -url <SiteUrl>");
				help.AppendLine("\t -manifest <ProvisionManifestPath> Default: provision.xml");
				break;

			case CommandNames.EnableExceptionHandling:
				help.AppendLine("[-print]");
				help.AppendLine("\t[-log] (Application pool identity needs write access to the event log)");
				break;

			case CommandNames.DisableExceptionHandling:
				break;

			}

			return help.ToString();
		}


		public int Run(string command, StringDictionary args, out string output) {
			if (args.ContainsKey("debug")) {
				System.Diagnostics.Debugger.Launch();
				//Console.WriteLine("Pausing to attach debugger, press any key to continue.");
				//Console.ReadKey(true);
			}


			int ret = 0;
			output = "";

			PrintHeader();

			switch (command.ToLower()) {
			
			case CommandNames.Import:
				ret = Import(command, args, out output);
				break;

			case CommandNames.EnableExceptionHandling:
				ret = EnableExceptionHandling(command, args, out output);
				break;

			case CommandNames.DisableExceptionHandling:
				ret = DisableExceptionHandling(command, args, out output);
				break;

			}

			Console.WriteLine("");

			return ret;
		}


		public int Import(string command, StringDictionary args, out string output) {
			try {

				output = "";

				bool valid = true;
				valid = valid && args.ContainsKey("url");

				if (!valid) {
					output = PrintHelpMessage(command);
					return 0;
				}

				string url = args["url"];
				string manifest = args.ContainsKey("manifest") ? args["manifest"] : "provision.xml";

				Uri uurl = new Uri(url);
				SPWebApplication webapp = SPWebApplication.Lookup(uurl);

				ProvisionManager manager = new ProvisionManager(webapp, manifest);
				manager.Import();

			} catch (Exception ex) {
				output = PrintHelpMessage(command);
				output += "\n" + SPExceptionUtil.Format(ex);
			}

			return 0;
		}

		public int EnableExceptionHandling(string command, StringDictionary args, out string output) {

			output = "";

			bool valid = true;
			valid = valid && args.ContainsKey("url");

			if (!valid) {
				output = PrintHelpMessage(command);
				return 0;
			}

			string url = args["url"];

			string sourcename = args.ContainsKey("sourcename") ? args["sourcename"] : "Rapid";
			string logname = args.ContainsKey("logname") ? args["logname"] : "Application";

			bool print = args.ContainsKey("print");
			bool log = args.ContainsKey("log");

			string globalasax = "global.asax";
			string webconfig = "web.config";


			try {

				using (SPSite site = new SPSite(url)) {

					SPUrlZone zone = SPWebApplicationUtil.GetZone(site.Url);
					SPWebApplication webapp = site.WebApplication;
					SPIisSettings settings = webapp.IisSettings[zone];

					string root = settings.Path.ToString();

					globalasax = string.Format(@"{0}\global.asax", root);
					webconfig = string.Format(@"{0}\web.config", root);

				}
			} catch (Exception ex) {
				SPExceptionUtil.Print(ex);
			}

			if (!File.Exists(webconfig)) {
				throw new Exception(string.Format("Unable to find web.config at {0}", webconfig));
			}


			try {

				Type tglobal = typeof(RapidToolsApplication);
				string contents = string.Format("<%@ Application Inherits=\"{0}, {1}\" %>", tglobal.FullName, tglobal.Assembly.FullName);
				File.WriteAllText(globalasax, contents);

			} catch (UnauthorizedAccessException) {
				Console.WriteLine("Unable to write to {0}. To correct this problem delete the file at {0}.\n", globalasax);
			}


			XmlDocument xconfig = new XmlDocument();
			xconfig.Load(webconfig);

			XmlElement xconfiguration = (XmlElement)xconfig.SelectSingleNode("/configuration");
			XmlElement xsystemweb = EnsureElement(xconfiguration, "system.web");

			XmlElement xappsettings = EnsureElement(xconfiguration, "appSettings");

			XmlElement xsetting = EnsureAddElement(xappsettings, "Rapid.Tools.exceptions.sourceName", "key");
			xsetting.SetAttribute("value", sourcename);

			xsetting = EnsureAddElement(xappsettings, "Rapid.Tools.exceptions.logName", "key");
			xsetting.SetAttribute("value", logname);

			xsetting = EnsureAddElement(xappsettings, "Rapid.Tools.exceptions.print", "key");
			xsetting.SetAttribute("value", print.ToString());

			xsetting = EnsureAddElement(xappsettings, "Rapid.Tools.exceptions.log", "key");
			xsetting.SetAttribute("value", log.ToString());

			xconfig.Save(webconfig);

			return 0;
		}

		public int DisableExceptionHandling(string command, StringDictionary args, out string output) {

			output = "";

			bool valid = true;
			valid = valid && args.ContainsKey("url");

			if (!valid) {
				output = PrintHelpMessage(command);
				return 0;
			}

			string url = args["url"];
			string globalasax = "global.asax";

			try {
				using (SPSite site = new SPSite(url)) {

					SPUrlZone zone = SPWebApplicationUtil.GetZone(site.Url);
					SPWebApplication webapp = site.WebApplication;
					SPIisSettings settings = webapp.IisSettings[zone];

					string root = settings.Path.ToString();

					globalasax = string.Format(@"{0}\global.asax", root);

				}
			} catch (Exception ex) {
				SPExceptionUtil.Print(ex);
			}

			try {

				Type tglobal = typeof(Microsoft.SharePoint.ApplicationRuntime.SPHttpApplication);
				string contents = string.Format("<%@ Application Inherits=\"{0}, {1}\" %>", tglobal.FullName, tglobal.Assembly.FullName);
				File.WriteAllText(globalasax, contents);

			} catch (UnauthorizedAccessException) {
				Console.WriteLine("Unable to write to {0}. To correct this problem delete the file at {0}.\n", globalasax);
			}

			return 0;
		}

		private XmlElement EnsureElement(XmlElement parentElement, string newElementName) {

			XmlElement xnew = (XmlElement)parentElement.SelectSingleNode(newElementName);

			if (xnew == null) {
				xnew = parentElement.OwnerDocument.CreateElement(newElementName);
				parentElement.AppendChild(xnew);
			}

			return xnew;

		}

		private XmlElement EnsureAddElement(XmlElement parentElement, string newElementName) {
			return EnsureAddElement(parentElement, newElementName, "name");
		}

		private XmlElement EnsureAddElement(XmlElement parentElement, string newKey, string keyAttributeName) {

			XmlElement xadd = (XmlElement)parentElement.SelectSingleNode(string.Format("add[@{0}='{1}']", keyAttributeName, newKey));

			if (xadd == null) {
				xadd = parentElement.OwnerDocument.CreateElement("add");
				parentElement.AppendChild(xadd);
			}

			xadd.SetAttribute(keyAttributeName, newKey);


			return xadd;
		}


	}
}
