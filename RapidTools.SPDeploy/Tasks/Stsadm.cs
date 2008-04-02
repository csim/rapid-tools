using System;
using System.Threading;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Management;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text.RegularExpressions;

namespace Rapid.Tools.SPDeploy.Tasks
{
	public class Stsadm : Task {

		private string _server;
		private string _command;
		private string _remoteWorkingDir;
		private string _PSExecExePath;
		private string _StsadmExePath;
        private string _PSExecUsername;
        private string _PSExecPassword;
		

		[Required]
		public string Server {
			get { return _server; }
			set { _server = value; }
		}

		[Required]
		public string PSExecExePath {
			get { return _PSExecExePath; }
			set { _PSExecExePath = value; }
		}

        public string PSExecUsername
        {
            get { return _PSExecUsername; }
            set { _PSExecUsername = value; }
        }

        public string PSExecPassword
        {
            get { return _PSExecPassword; }
            set { _PSExecPassword = value; }
        }

		[Required]
		public string StsadmExePath {
			get { return _StsadmExePath; }
			set { _StsadmExePath = value; }
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


		public override bool Execute() {

			//System.Diagnostics.Debugger.Launch();


            string mask = @"-accepteula -s ";

            if (!string.IsNullOrEmpty(PSExecUsername))
            {
                mask += string.Format(@" -u {0} -p {1} ", PSExecUsername, PSExecPassword);
            }

            if (string.IsNullOrEmpty(RemoteWorkingDir))
            {
                mask += @"\\{0} {3} {2}";
            }
            else
            {
                if (RemoteWorkingDir.IndexOf(" ") >= 0) RemoteWorkingDir = "\"" + RemoteWorkingDir + "\"";
                mask += @"-w {1} \\{0} {3} {2}";
            }

			if (StsadmExePath.IndexOf(" ") >= 0) StsadmExePath = "\"" + StsadmExePath + "\"";

			ProcessStartInfo pinfo = new ProcessStartInfo(PSExecExePath);
			pinfo.Arguments = string.Format(mask, Server, RemoteWorkingDir, Command, StsadmExePath);

			pinfo.UseShellExecute = false;
			pinfo.CreateNoWindow = true;
			pinfo.RedirectStandardOutput = true;
			pinfo.RedirectStandardError = true;

			pinfo.StandardOutputEncoding = Encoding.ASCII;
			pinfo.StandardErrorEncoding = Encoding.ASCII;
			pinfo.LoadUserProfile = false;

			//pinfo.UseShellExecute = true;
			//pinfo.CreateNoWindow = false;
			//pinfo.RedirectStandardOutput = false;
			//pinfo.RedirectStandardError = false;
			//PSExec -accepteula -s  -w c:\_temp \\dev-clints-wss "C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\BIN\stsadm.exe" -o UpgradeSolution -immediate -allowgacdeployment -allowcaspolicies -name "flex.wsp" -filename "flex.wsp"

            Log.LogMessage(pinfo.Arguments);

			Log.LogMessage(string.Format("Executing STSADM {0} on {1}", Command, Server));
            
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
	}


}


