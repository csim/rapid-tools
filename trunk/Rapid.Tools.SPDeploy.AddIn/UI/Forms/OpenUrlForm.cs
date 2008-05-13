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
using Rapid.Tools.SPDeploy.AddIn.UI.Forms;

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
            set
            {
                _url = value;
            }
		}

        public OpenUrlForm()
        {
            InitializeComponent();

            this.AcceptButton = button2;
            this.Load += new EventHandler(OpenUrlForm_Load);
        }

        void OpenUrlForm_Load(object sender, EventArgs e)
        {
            
            txtUrl.Text = Url;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Validator.isValidUrl(txtUrl.Text)) return;
            
                _url = txtUrl.Text;
                Close();
            
		}

        private void txtUrl_Validating(object sender, CancelEventArgs e)
        {
            if (!Validator.isValidUrl(txtUrl.Text))
            {
                errorProvider1.SetError(txtUrl, "Not a valid Url.");
                e.Cancel = true;
            }
        }
    }
}