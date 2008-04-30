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
        public static NodeTag Create(TreeNode node, DTE2 applicationObject, NodeType type)
        {
            switch (type)
            {
                case NodeType.Site:
                    return new NullNodeTag();
                    break;
                case NodeType.Web:
                    return new SPWebNodeTag(node);
                    break;
                case NodeType.List:
                    return new SPListNodeTag(node);
                    break;
                case NodeType.ListItem:
                    return new NullNodeTag();
                    break;
                case NodeType.Folder:
                    return new NullNodeTag();
                    break;
                case NodeType.File:
                    return new SPFileNodeTag(node);
                    break;
                case NodeType.View:
                    return new SPViewNodeTag(node);
                    break;
                default:
                    break;
            }
            return new NullNodeTag();
        }
    }    
}
