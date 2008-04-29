using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.Utilties
{
    public class WatcherUtil
    {
		private static WatcherUtil instance = null;

        public static WatcherUtil Instance
        {
            get {
				if (instance == null) instance = new WatcherUtil();
				return instance; 
			}
        }       

        [Serializable]
        public class WatcherUtilInfo
        {
            public string siteUrl;
            public Guid webGuid;
            public Guid fileGuid;
            public string filePath;
        }

        FileSystemWatcher watcher;

        public Hashtable VisualStudioItems;

        private WatcherUtil()
        {
            VisualStudioItems = new Hashtable();

			DirectoryInfo wdir = AppManager.Current.ActiveWorkspaceDirectory;
            
			watcher = new FileSystemWatcher(wdir.FullName);
            watcher.IncludeSubdirectories = true;
            watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
            watcher.EnableRaisingEvents = true;

        }



        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (VisualStudioItems.Contains(e.FullPath))
            {
                WatcherUtilInfo util = VisualStudioItems[e.FullPath] as WatcherUtilInfo;
                try
                {
					ProxyBridge bridge = new ProxyBridge();
					bridge.AddInService.SaveBinary(util.siteUrl, util.webGuid, util.fileGuid, File.ReadAllBytes(e.FullPath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);                    
                }
            }
        }

        public void AddWatcher(string fileName, string siteUrl, Guid webGuid, Guid fileGuid)
        {
            WatcherUtilInfo util = new WatcherUtilInfo();
            util.siteUrl = siteUrl;
            util.webGuid = webGuid;
            util.fileGuid = fileGuid;
            VisualStudioItems[fileName] = util;
        }

        public void RemoveWatcher(string fileName)
        {
            if (VisualStudioItems.Contains(fileName))
                VisualStudioItems.Remove(fileName);
        }






        ~WatcherUtil()
        {

            List<WatcherUtilInfo> abc = new List<WatcherUtilInfo>();

            foreach (string k in VisualStudioItems.Keys)
            {
                WatcherUtilInfo u = new WatcherUtilInfo();
                u = VisualStudioItems[k] as WatcherUtilInfo;
                u.filePath = k;
                abc.Add(u);
            }

        }


       
    }
}
