using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class ListDefinitionDialog : Form
    {
        private string templateName;

        public string TemplateName
        {
            get { return templateName; }
            set { templateName = value; }
        }

        private int templateNumber;

        public int TemplateNumber
        {
            get { return templateNumber; }
            set { templateNumber = value; }
        }



        public ListDefinitionDialog()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            TemplateName = txtTemplateName.Text;
            TemplateNumber = Convert.ToInt32(txtTemplateNumber.Text);
            Cancelled = false;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        bool _cancelled = true;

        public bool Cancelled
        {
            get { return _cancelled; }
            set { _cancelled = value; }
        }
    }
}