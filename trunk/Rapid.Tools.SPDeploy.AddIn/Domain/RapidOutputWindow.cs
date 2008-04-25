using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE80;
using EnvDTE;
using System.Collections;

namespace Rapid.Tools.SPDeploy.AddIn.Domain
{
    public class RapidOutputWindow
    {
        const string STR_Window_Name = "Rapid";

        OutputWindowPane _rapidOutputWindow;

        public RapidOutputWindow(DTE2 applicationObject)
        {
            OutputWindow _mainWindow = applicationObject.ToolWindows.OutputWindow;
            IEnumerator _enum = _mainWindow.OutputWindowPanes.GetEnumerator();
            while (_enum.MoveNext())
            {
                if (((OutputWindowPane)_enum.Current).Name == STR_Window_Name)
                {
                    _rapidOutputWindow = _enum.Current as OutputWindowPane;
                    break;
                }
            }
            if (_rapidOutputWindow == null)
                _rapidOutputWindow = _mainWindow.OutputWindowPanes.Add(STR_Window_Name);
        }

        public void Activate()
        {
            _rapidOutputWindow.Activate();            
        }

        public void Write(string message)
        {
            _rapidOutputWindow.OutputString(message);
        }

        public void Clear()
        {
            _rapidOutputWindow.Clear();
            }
    }
}
