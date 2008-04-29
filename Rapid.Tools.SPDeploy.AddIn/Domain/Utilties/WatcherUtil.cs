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

        public Hashtable WatchedItems;

        private WatcherUtil()
        {
            WatchedItems = new Hashtable();

			DirectoryInfo wdir = AppManager.Current.ActiveWorkspaceDirectory;
            
			watcher = new FileSystemWatcher(wdir.FullName);
            watcher.IncludeSubdirectories = true;
            watcher.Renamed += new RenamedEventHandler(WatchedFileRenamed);
            watcher.EnableRaisingEvents = true;

        }



        private void WatchedFileRenamed(object sender, RenamedEventArgs e)
        {
			try
			{
				if (WatchedItems.Contains(e.FullPath))
				{
					WatcherUtilInfo util = WatchedItems[e.FullPath] as WatcherUtilInfo;
					
					// TODO: figure out the file locking here -- this is a temporary fix
					System.Threading.Thread.Sleep(1000);
					AppManager.Current.ActiveBridge.AddInService.SaveBinary(util.siteUrl, util.webGuid, util.fileGuid, File.ReadAllBytes(e.FullPath));
				}
			}
			catch (Exception ex)
			{
				ExceptionUtil.Handle(ex);
			}
		}

		public void AddWatcher(FileInfo file, string siteUrl, Guid webGuid, Guid fileGuid)
        {
            WatcherUtilInfo util = new WatcherUtilInfo();
            util.siteUrl = siteUrl;
            util.webGuid = webGuid;
            util.fileGuid = fileGuid;
            WatchedItems[file.FullName] = util;
        }

        public void RemoveWatcher(FileInfo file)
        {
			if (WatchedItems.Contains(file.FullName))
				WatchedItems.Remove(file.FullName);
        }






        ~WatcherUtil()
        {

            List<WatcherUtilInfo> abc = new List<WatcherUtilInfo>();

            foreach (string k in WatchedItems.Keys)
            {
                WatcherUtilInfo u = new WatcherUtilInfo();
                u = WatchedItems[k] as WatcherUtilInfo;
                u.filePath = k;
                abc.Add(u);
            }

        }


       
    }
}
