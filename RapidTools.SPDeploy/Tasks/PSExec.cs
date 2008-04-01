using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Management;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace RapidTools.SPDeploy.Tasks
{
	public class PSExec : Task {

		private string _server;
		private string _command;
		private string _remoteWorkingDir;
		private string _PSExecExePath;
        private string _PSExecUsername;
        private string _PSExecPassword;

		[Required]
		public string Server {
			get { return _server; }
			set { _server = value; }
		}

		[Required]
		public string Command {
			get { return _command; }
			set { _command = value; }
		}

		public string RemoteWorkingDir {
			get { return _remoteWorkingDir; }
			set { _remoteWorkingDir = value; }
		}

		[Required]
		public string PSExecExePath {
			get { return _PSExecExePath; }
			set { _PSExecExePath = value; }
		}

        public string PSExecUsername {
            get { return _PSExecUsername; }
            set { _PSExecUsername = value; }
        }

        public string PSExecPassword
        {
            get { return _PSExecPassword; }
            set { _PSExecPassword = value; }
        }


		public override bool Execute() {

			string mask = @"-accepteula -s ";

            if (!string.IsNullOrEmpty(PSExecUsername))
            {
                mask += string.Format(@" -u {0} -p {1} ", PSExecUsername, PSExecPassword);
            }

			if (string.IsNullOrEmpty(RemoteWorkingDir)) {
				mask += @"\\{0} {2} {3}";
			} else {
				if (RemoteWorkingDir.IndexOf(" ") >= 0) RemoteWorkingDir = "\"" + RemoteWorkingDir + "\"";
				mask += @"-w {1} \\{0} {2} {3}";
			}

			ProcessStartInfo pinfo = new ProcessStartInfo(PSExecExePath);
			pinfo.Arguments = string.Format(mask, Server, RemoteWorkingDir, "", Command);

			pinfo.UseShellExecute = false;
			pinfo.CreateNoWindow = true;
			pinfo.RedirectStandardOutput = true;
			pinfo.RedirectStandardError = true;

			pinfo.StandardOutputEncoding = Encoding.ASCII;
			pinfo.StandardErrorEncoding = Encoding.ASCII;
			pinfo.LoadUserProfile = false;

			Log.LogMessage(string.Format("Executing {0} on {1}", Command, Server));

			//Log.LogMessage(pinfo.Arguments);

			Process p = new Process();

			//p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);

			p.StartInfo = pinfo;
			p.EnableRaisingEvents = true;

			p.Start();

			//Log.LogMessage(p.StandardOutput.CurrentEncoding.GetType().ToString());

			string line;
			do {
				//line = Encoding.UTF8.GetString(new byte[] { (byte)p.StandardOutput.Read() });
				line = p.StandardOutput.ReadLine();
				//line = Regex.Replace(line, @"[^\w]", "", RegexOptions.IgnoreCase);

				if (!string.IsNullOrEmpty(line)) {
					Log.LogMessage(line);
				}

			} while (!p.StandardOutput.EndOfStream);


			//Log.LogMessage(p.StandardOutput.ReadToEnd());
			p.WaitForExit();

			string eoutput = p.StandardError.ReadToEnd();
			bool error = (p.ExitCode != 0);
			if (error) {
				Log.LogMessage(eoutput);
				Log.LogError("STSADM failed.");
				return false;
			}


			return true;

		}

		void p_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			Log.LogMessage(e.Data);
		}

	}


}


