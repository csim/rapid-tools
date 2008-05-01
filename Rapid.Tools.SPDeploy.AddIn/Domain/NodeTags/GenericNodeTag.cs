using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class GenericNodeTag : NodeTag
    {

        public override void DoubleClick()
        {
            
        }

        public override System.Windows.Forms.ContextMenu RightClick()
        {
            ContextMenu contextMenu = new ContextMenu();
            //contextMenu.MenuItems.Add("No Actions");
            return contextMenu;
        }
    }
}
