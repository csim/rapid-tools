using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Specialized;

namespace Rapid.Tools.SPDeploy.AddIn.Domain
{
    public class FileWatcher
    {
		private static FileWatcher instance = null;

        public static FileWatcher Instance
        {
            get {
				if (instance == null) instance = new FileWatcher();
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

        public Hashtable WatchFiles;

        public FileWatcher()
        {
            WatchFiles = new Hashtable();

			DirectoryInfo wdir = AppManager.Current.ActiveWorkspaceDirectory;
            
			watcher = new FileSystemWatcher(wdir.FullName);
            watcher.IncludeSubdirectories = true;
            watcher.Renamed += new RenamedEventHandler(FileRenamed);
            watcher.EnableRaisingEvents = true;

        }



		public void AddWatcher(FileInfo file, string siteUrl, Guid webGuid, Guid fileGuid)
        {
            WatcherUtilInfo util = new WatcherUtilInfo();
            util.siteUrl = siteUrl;
            util.webGuid = webGuid;
            util.fileGuid = fileGuid;
            WatchFiles[file.FullName] = util;
        }

        public void RemoveWatcher(FileInfo file)
        {
			if (WatchFiles.Contains(file.FullName))
				WatchFiles.Remove(file.FullName);
        }



		private void FileRenamed(object sender, RenamedEventArgs e)
		{
			try
			{
				if (WatchFiles.Contains(e.FullPath))
				{
					WatcherUtilInfo util = WatchFiles[e.FullPath] as WatcherUtilInfo;

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





        ~FileWatcher()
        {


        }


       
    }
}
