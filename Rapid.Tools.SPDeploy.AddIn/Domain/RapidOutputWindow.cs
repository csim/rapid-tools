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

        private static readonly RapidOutputWindow instance = new RapidOutputWindow();

        public static RapidOutputWindow Instance
        {
            get { return instance; }
        }

        OutputWindowPane _rapidOutputWindow;

        private RapidOutputWindow()
        {
            OutputWindow _mainWindow = AppManager.Instance.ApplicationObject.ToolWindows.OutputWindow;
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

        public void Write(string message, bool addNewLine)
        {
            _rapidOutputWindow.OutputString(message);
            if (addNewLine)
                _rapidOutputWindow.OutputString(Environment.NewLine);
        }

        public void Clear()
        {
            _rapidOutputWindow.Clear();
        }
    }
}
