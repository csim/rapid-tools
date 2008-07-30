using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using EnvDTE80;
using System.IO;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public enum NodeType
    {
        Null,
        Site,
        Web,
        List,
        ListItem,
        Folder,
        File,
        View
    }

    public class NodeTagFactory
    {
        public static NodeTag Create(TreeNode node, NodeType type)
        {
            switch (type)
            {
				case NodeType.Site:
					return new SPSiteNodeTag(node);
				case NodeType.Web:
                    return new SPWebNodeTag(node);
                case NodeType.List:
                    return new SPListNodeTag(node);
                case NodeType.File:
                    return new SPFileNodeTag(node);
                case NodeType.View:
                    return new SPViewNodeTag(node);
            }

			return new GenericNodeTag(node);
        }
    }    
}
