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
using Rapid.Tools.Domain.Utilities;
using Rapid.Tools.Providers.Provision;

namespace Rapid.Tools.Domain.Provision
{

	public class RapidProvisionManager : RapidXmlToolManager
	{

		public RapidProvisionManager(SPWebApplication webApplication, string manifestPath) : base(webApplication, manifestPath)
		{
			ManifestRootNodeName = "ProvisionManifest";
			EnsureInitialize();
		}



		public void Import()
		{
			WriteMessage("Begin import from");
			WriteMessage(ManifestPath);

			ExecuteChildren(Manifest.DocumentElement);
		}

		//public void Export()
		//{
		//}

	}

}
