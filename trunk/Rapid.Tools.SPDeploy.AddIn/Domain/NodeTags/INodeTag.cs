using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using EnvDTE80;
using System.IO;
using System.Xml;
using System.Net;
using Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public interface INodeTag
    {
        ContextMenu RightClick();
        void DoubleClick();
    }   
}
