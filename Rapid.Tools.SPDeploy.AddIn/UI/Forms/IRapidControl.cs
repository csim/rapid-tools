using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Rapid.Tools.SPDeploy.AddIn.UI.Forms
{
    public interface IRapidControl
    {
        void OkClicked();
        void CancelClicked();
        bool FormIsValid();
        Panel[] GetPanels();
        void AddControl(Control c);
        void RemoveControl(Control c);
    }
}
