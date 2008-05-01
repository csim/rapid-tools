using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.IO.IsolatedStorage;
using EnvDTE;
using Microsoft.Win32;
using System.Xml;
using System.Security.Principal;
using Rapid.Tools.SPDeploy.AddIn.Domain;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Controls
{
    public partial class OpenUrlForm : Form
    {

		private string _url = "";

		public string Url
		{
			get
			{
				return _url;
			}
		}

        public OpenUrlForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
			_url = txtUrl.Text;
			Close();
		}
    }
}