using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Rapid.Tools.SPDeploy.AddIn.Domain
{
    public class DefaultColorTable : ProfessionalColorTable
    {
        public override Color ImageMarginGradientBegin
        {
            get
            {
                return SystemColors.ControlLight;
            }
        }

        public override Color ImageMarginGradientMiddle
        {
            get
            {
                return SystemColors.Control;
            }
        }

        public override Color ImageMarginGradientEnd
        {
            get
            {
                return SystemColors.ControlDark;
            }
        }



        public override Color MenuItemSelectedGradientBegin
        {
            get
            {
                return SystemColors.GradientInactiveCaption;
            }
        }



        public override Color MenuItemSelectedGradientEnd
        {
            get
            {
                return SystemColors.GradientInactiveCaption;
            }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get
            {
                return SystemColors.ControlLight;
            }
        }

        public override Color MenuItemPressedGradientMiddle
        {
            get
            {
                return SystemColors.ControlLight;
            }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get
            {
                return SystemColors.ControlLight;
            }
        }

        public override Color ButtonSelectedGradientBegin
        {
            get
            {
                return SystemColors.GradientInactiveCaption;
            }
        }

        public override Color ButtonSelectedGradientMiddle
        {
            get
            {
                return SystemColors.GradientInactiveCaption;
            }
        }

        public override Color ButtonSelectedGradientEnd
        {
            get
            {
                return SystemColors.GradientInactiveCaption;
            }
        }


        public override Color MenuItemSelected
        {
            get
            {
                return SystemColors.GradientInactiveCaption;
            }
        }
    }
}
