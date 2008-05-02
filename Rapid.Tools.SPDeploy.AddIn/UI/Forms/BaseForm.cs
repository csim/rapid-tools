using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Rapid.Tools.SPDeploy.AddIn.Domain;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public partial class BaseForm : Form
    {
        public enum RapidFormType
        {
            ActivationDependency,
            CustomAction,
            Feature,
            WebPart,
            CustomActionGroup,
            FeatureReceiver
          
        }

        private Rapid.Tools.SPDeploy.AddIn.UI.Forms.FormsController.IRapidControl _mainControl = null;



        public BaseForm(RapidFormType type)
        {

            InitializeComponent();
            switch (type)
            {
                case RapidFormType.ActivationDependency:
                    _mainControl = new ActivationDependencyControl();
                    break;
                case RapidFormType.CustomAction:
                    _mainControl = new CustomActionControl();
                    break;
                case RapidFormType.CustomActionGroup:
                    _mainControl = new CustomActionGroupControl();
                    break;
                case RapidFormType.Feature:
                    _mainControl = new FeatureControl();
                    break;
                case RapidFormType.WebPart:
                    _mainControl = new WebPartControl();
                    break;
                case RapidFormType.FeatureReceiver:
                    _mainControl = new FeatureReceiverControl();
                    break;
                default:
                    break;
            }
            if (_mainControl != null)
            {
                panel1.Controls.Add(_mainControl as UserControl);
                if (_mainControl.GetPanels().Length > 1)
                {
                    pictureBox3.Visible = true;
                    _mainControl.RemoveControl(_mainControl.GetPanels()[1]);
                }               
            }
        }



        private void btnOk_Click(object sender, EventArgs e)
        {
            _mainControl.OkClicked();
            AppManager.Current.EnsureProjectFilesAdded(AppManager.Current.GetFeatureDirectory().FullName);
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _mainControl.CancelClicked();
            this.Close();
        }

        bool showMore = false;
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            showMore = !showMore;

            Panel[] panels = _mainControl.GetPanels();
            if (showMore)
            {
                pictureBox3.Image = Resources.Images.Files.buttonLess;
                _mainControl.AddControl(panels[1]);
            }
            else
            {
                pictureBox3.Image = Resources.Images.Files.buttonMore;
                _mainControl.RemoveControl(panels[1]);
            }
        }


    }
}