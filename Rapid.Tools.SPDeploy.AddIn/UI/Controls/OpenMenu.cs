using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using Rapid.Tools.SPDeploy.AddIn.Domain;
using Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags;

namespace Rapid.Tools.SPDeploy.AddIn
{
    public class OpenMenu
    {

        private ToolStripMenuItem _menu;

        public ToolStripMenuItem SolutionItem
        {
            get { return _menu; }
            set { _menu = value; }
        }

        public EventHandler MachineChanged;

        private ToolStripMenuItem _openMenuItem;

		public OpenMenu(ToolStripMenuItem menuItem, EventHandler parentClick)
        {
			_menu = menuItem;
			_openMenuItem = new ToolStripMenuItem("Open Other ...");

			_menu.DropDownItems.Add(new ToolStripSeparator());
			_menu.DropDownItems.Add(_openMenuItem);

			_openMenuItem.Tag = "Open";
			_openMenuItem.Click += parentClick;

        }

    }
}
