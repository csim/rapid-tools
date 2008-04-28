using System.IO;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.Utilties
{
    public class WatcherUtilitiy
    {
        private static readonly WatcherUtilitiy instance = new WatcherUtilitiy();

        public static WatcherUtilitiy Instance
        {
            get { return instance; }
        }

        public class WatcherUtil
        {
            public string siteUrl;
            public Guid webGuid;
            public Guid fileGuid;
        }

        FileSystemWatcher watcher;

        public Hashtable VisualStudioItems;

        private WatcherUtilitiy()
        {
            VisualStudioItems = new Hashtable();
            watcher = new FileSystemWatcher(Domain.Utilties.Functions.GetWorkingDirectoryPath());
            watcher.IncludeSubdirectories = true;
            watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
            watcher.EnableRaisingEvents = true;
        }



        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (VisualStudioItems.Contains(e.FullPath))
            {
                WatcherUtil util = VisualStudioItems[e.FullPath] as WatcherUtil;
                try
                {
                    ServiceManager.Instance.ServiceInstance.SaveBinary(util.siteUrl, util.webGuid, util.fileGuid, File.ReadAllBytes(e.FullPath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void AddWatcher(string fileName, string siteUrl, Guid webGuid, Guid fileGuid)
        {
            WatcherUtil util = new WatcherUtil();
            util.siteUrl = siteUrl;
            util.webGuid = webGuid;
            util.fileGuid = fileGuid;
            VisualStudioItems[fileName] = util;
        }

        public void removeWatcher(string fileName)
        {
            if (VisualStudioItems.Contains(fileName))
                VisualStudioItems.Remove(fileName);
        }








    }
}
