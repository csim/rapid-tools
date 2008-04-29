using System.IO;
using System.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace Rapid.Tools.SPDeploy.AddIn.Domain.Utilties
{
    public class WatcherUtilitiy
    {
        private static readonly WatcherUtilitiy instance = new WatcherUtilitiy();

        public static WatcherUtilitiy Instance
        {
            get { return instance; }
        }       

        [Serializable]
        public class WatcherUtil
        {
            public string siteUrl;
            public Guid webGuid;
            public Guid fileGuid;
            public string filePath;
        }

        FileSystemWatcher watcher;

        public Hashtable VisualStudioItems;

        private WatcherUtilitiy()
        {
            VisualStudioItems = new Hashtable();

			DirectoryInfo wdir = EnvironmentUtil.GetWorkingDirectory();
            
			watcher = new FileSystemWatcher(wdir.FullName);
            watcher.IncludeSubdirectories = true;
            watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
            watcher.EnableRaisingEvents = true;

            if (File.Exists(@"C:\theinfo.dat"))
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<WatcherUtil>));
                FileStream fs = new FileStream(@"C:\theinfo.dat", FileMode.Open);
                List<WatcherUtil> wu = ser.Deserialize(fs) as List<WatcherUtil>;

                foreach (WatcherUtil u in wu)
                {
                    VisualStudioItems.Add(u.filePath, u);
                }
            }
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






        ~WatcherUtilitiy()
        {

            List<WatcherUtil> abc = new List<WatcherUtil>();

            foreach (string k in VisualStudioItems.Keys)
            {
                WatcherUtil u = new WatcherUtil();
                u = VisualStudioItems[k] as WatcherUtil;
                u.filePath = k;
                abc.Add(u);
            }

            XmlSerializer ser = new XmlSerializer(typeof(List<WatcherUtil>));
            FileStream fs = new FileStream(@"C:\theinfo.dat", FileMode.Create);
            ser.Serialize(fs, abc);                        
        }


       
    }
}
