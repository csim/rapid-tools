using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Text;
using System.Text.RegularExpressions;

namespace RapidTools.SPDeploy.Tasks
{
	public class TranslateTokens : Task {

		private string _workingDirectory;
		private string _tokens;
		private NameValueCollection _ntokens;

		private ITaskItem[] _rootFiles;
		private ITaskItem[] _templateFiles;
		private ITaskItem[] _tokenReplacementFiles;
		

		[Required]
		public string WorkingDirectory {
			get { return _workingDirectory; }
			set { _workingDirectory = value; }
		}

		[Required]
		public string Tokens {
			get { return _tokens; }
			set { _tokens = value; }
		}

		[Required]
		public ITaskItem[] RootFiles {
			get { return _rootFiles; }
			set { _rootFiles = value; }
		}

		[Required]
		public ITaskItem[] TemplateFiles {
			get { return _templateFiles; }
			set { _templateFiles = value; }
		}

		[Required]
		public ITaskItem[] TokenReplacementFiles {
			get { return _tokenReplacementFiles; }
			set { _tokenReplacementFiles = value; }
		}

		public override bool Execute() {

			ParseTokens();

			return true;
		}


		private void ParseTokens() {
			_ntokens = new NameValueCollection();

			MatchCollection matches = Regex.Matches(Tokens, "(.*?)=(.*?);");
			foreach (Match m in matches) {
				_ntokens.Add(m.Result("$1"), m.Result("$2"));
			}
		
		}

		private void ReplaceTokens() {

			// Replace tokens
			if (!string.IsNullOrEmpty(Tokens) && TokenReplacementFiles != null && TokenReplacementFiles.Length > 0) {

				string toldpath, tnewpath, tcontents;
				
				foreach (ITaskItem item in TokenReplacementFiles) {
					toldpath = item.GetMetadata("FullPath");
					tcontents = File.ReadAllText(toldpath);

					tcontents = Regex.Replace(tcontents, @"$\((.*)?\)", ReplaceTokenEvaluator);

					tnewpath = string.Format(@"{0}\{1}", WorkingDirectory, item.GetMetadata("Filename"));
					File.Create(tnewpath);

					ReplaceOutputFilePath(toldpath, tnewpath);

				}

			}

		}

		private void ReplaceOutputFilePath(string oldPath, string newPath) {

			string fullpath;
			List<ITaskItem> items = new List<ITaskItem>();

			items.AddRange(TemplateFiles);

			foreach (ITaskItem item in TemplateFiles) {
				fullpath = item.GetMetadata("FullPath");
				if (string.Compare(fullpath, oldPath, true) == 0) {
				}
			}

		}

		private string ReplaceTokenEvaluator(Match m) {

			return "";

		}

	
	}
}
