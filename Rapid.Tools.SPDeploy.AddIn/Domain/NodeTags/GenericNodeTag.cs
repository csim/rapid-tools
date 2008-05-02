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
			try
			{
				Browse();
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
        }

        public override System.Windows.Forms.ContextMenu RightClick()
        {
			ContextMenu contextMenu = new ContextMenu();
			try
			{
				//contextMenu.MenuItems.Add("No Actions");
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}

			return contextMenu;
		}
    }
}
