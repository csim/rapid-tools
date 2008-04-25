using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags
{
    public class NullNodeTag : NodeTag
    {

        public override void Action()
        {
            
        }

        public override System.Windows.Forms.ContextMenu GetContextMenu()
        {
            ContextMenu _contextMenu = new ContextMenu();
            _contextMenu.MenuItems.Add("No Actions");
            return _contextMenu;
        }
    }
}
