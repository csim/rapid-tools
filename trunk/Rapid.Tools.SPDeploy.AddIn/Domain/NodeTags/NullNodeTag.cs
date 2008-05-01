using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class NullNodeTag : NodeTag
    {

        public override void DoubleClick()
        {
            
        }

        public override System.Windows.Forms.ContextMenu RightClick()
        {
            ContextMenu _contextMenu = new ContextMenu();
            _contextMenu.MenuItems.Add("No Actions");
            return _contextMenu;
        }
    }
}
