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
		private static readonly WatcherUtil instance = new WatcherUtil();

        public static WatcherUtil Instance
        {
            get { return instance; }
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

			DirectoryInfo wdir = EnvironmentUtil.GetWorkingDirectory();
            
			watcher = new FileSystemWatcher(wdir.FullName);
            watcher.IncludeSubdirectories = true;
            watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
            watcher.EnableRaisingEvents = true;

            if (File.Exists(@"C:\theinfo.dat"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<WatcherUtilInfo>));
                FileStream fs = new FileStream(@"C:\theinfo.dat", FileMode.Open);
                List<WatcherUtilInfo> wu = ser.Deserialize(fs) as List<WatcherUtilInfo>;

                foreach (WatcherUtilInfo u in wu)
                {
                    VisualStudioItems.Add(u.filePath, u);
                }
            }
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

            XmlSerializer ser = new XmlSerializer(typeof(List<WatcherUtilInfo>));
            FileStream fs = new FileStream(@"C:\theinfo.dat", FileMode.Create);
            ser.Serialize(fs, abc);                        
        }


       
    }
}
