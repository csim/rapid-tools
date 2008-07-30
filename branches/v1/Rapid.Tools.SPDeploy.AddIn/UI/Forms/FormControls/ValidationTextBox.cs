using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms.FormControls
{
    public class ValidationTextBox : TextBox
    {
        private bool isValid;

        public bool IsValid
        {
            get { return isValid; }
            set { isValid = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Color c = (IsValid) ? Color.Black : Color.Red;

            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle, c, 1, ButtonBorderStyle.Solid, c, 1, ButtonBorderStyle.Solid, c, 1, ButtonBorderStyle.Solid, c, 1, ButtonBorderStyle.Solid);
        }
    }
}
