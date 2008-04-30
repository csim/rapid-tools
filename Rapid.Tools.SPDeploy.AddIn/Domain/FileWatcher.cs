using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Specialized;
using Rapid.Tools.SPDeploy.AddIn.Domain.NodeTags;

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

        FileSystemWatcher watcher;

        public Dictionary<string, NodeTag> WatchFiles;

        public FileWatcher()
        {
            WatchFiles = new Dictionary<string,NodeTag>();

			DirectoryInfo wdir = AppManager.Current.ActiveWorkspaceDirectory;
            
			watcher = new FileSystemWatcher(wdir.FullName);
            watcher.IncludeSubdirectories = true;
            watcher.Renamed += new RenamedEventHandler(FileRenamed);
            watcher.EnableRaisingEvents = true;

        }


		public void AddWatcher(NodeTag tag)
        {
			WatchFiles.Add(tag.WorkspacePath.FullName, tag);
        }

		public void RemoveWatcher(NodeTag tag)
        {
			string wpath = tag.WorkspacePath.FullName;

			if (WatchFiles.ContainsKey(wpath))
				WatchFiles.Remove(wpath);
        }
		

		private void FileRenamed(object sender, RenamedEventArgs e)
		{
			try
			{
				if (WatchFiles.ContainsKey(e.FullPath))
				{
					NodeTag tag = WatchFiles[e.FullPath];

					if (tag.TagType == NodeType.File)
					{
						// TODO: figure out the file locking here -- this is a temporary fix
						System.Threading.Thread.Sleep(1000);
						AppManager.Current.ActiveBridge.AddInService.SaveBinary(tag.WebID, tag.ID, File.ReadAllBytes(e.FullPath));
					}
					else if (tag.TagType == NodeType.View)
					{
						// TODO: figure out the file locking here -- this is a temporary fix
						System.Threading.Thread.Sleep(1000);
						AppManager.Current.ActiveBridge.AddInService.UpdateViewSchema(tag.WebID, tag.ListID, tag.Node.Text, File.ReadAllText(e.FullPath));
					}

				}
			}
			catch (Exception ex)
			{
				AppManager.Current.Write(ex);
			}
		}





        ~FileWatcher()
        {


        }


       
    }
}
