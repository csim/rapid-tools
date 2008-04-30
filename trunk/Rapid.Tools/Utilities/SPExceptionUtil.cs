using System;
using System.Threading;
using System.Security;
using System.Security.Principal;
using System.Xml;
using System.Web;
using System.Text;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;

using System.Diagnostics;
using System.Globalization;

namespace Rapid.Tools.Utilities
{
	public static class SPExceptionUtil
	{



		public static void Print(Exception ex)
		{
			string output = Format(ex);

			if (HttpContext.Current != null)
			{
				HttpContext.Current.Response.Write(Format(ex, true));
				HttpContext.Current.Trace.Warn(output);
			}

			Console.WriteLine(output);
			Console.Error.WriteLine(output);
			Debug.WriteLine(output);
		}

		public static string Format(Exception ex, bool html)
		{
			string output = Format(ex);

			//using (StringWriter sw = new StringWriter()) {
			//    TextExceptionFormatter formatter = new TextExceptionFormatter(sw, ex);
			//    formatter.Format();
			//    sw.Flush();
			//    output = sw.ToString();
			//}

			string mask = (html) ? "<pre>{0}</pre>" : "{0}";
			return string.Format(mask, output);
		}




		public static string Format(Exception ex)
		{
			return Format(ex, null);
		}

		public static string Format(Exception ex, NameValueCollection additionalInfo)
		{

			if (additionalInfo == null) additionalInfo = GetAdditionalInfo(ex, null);

			string TEXT_CAP = "** Exception Occurred **=*=****=*=****=*=**";
			StringBuilder strInfo = new StringBuilder(string.Format("{1}{0}", Environment.NewLine, TEXT_CAP));

			if (ex == null)
			{
				strInfo.AppendFormat("{0}{0}No Exception object has been provided.{0}", Environment.NewLine);
			}
			else
			{
				#region Loop through each exception class in the chain of exception objects
				Exception currentException = ex;	// Temp variable to hold InnerException object during the loop.
				int intExceptionCount = 1;				// Count variable to track the number of exceptions in the chain.
				do
				{
					// Write title information for the exception object.
					//strInfo.AppendFormat("{0}{0}{1}) Exception Information{0}{2}{0}", Environment.NewLine, intExceptionCount.ToString(), TEXT_SEPARATOR);
					strInfo.AppendFormat(" {3,2}) {2,15}: {1}{0}", Environment.NewLine, currentException.GetType().FullName, "Exception Type", intExceptionCount.ToString());


					#region Loop through the public properties of the exception object and record their value
					// Loop through the public properties of the exception object and record their value.
					PropertyInfo[] aryPublicProperties = currentException.GetType().GetProperties();
					NameValueCollection currentAdditionalInfo;
					foreach (PropertyInfo p in aryPublicProperties)
					{
						// Do not log information for the InnerException or StackTrace. This information is 
						// captured later in the process.
						try
						{
							if (p.Name != "InnerException" && p.Name != "StackTrace")
							{
								if (p.GetValue(currentException, null) == null)
								{
									strInfo.AppendFormat("{1,20}: NULL{0}", Environment.NewLine, p.Name);
								}
								else
								{
									// Loop through the collection of AdditionalInformation if the exception type is a BaseApplicationException.
									if (p.Name == "AdditionalInformation")
									{
										// Verify the collection is not null.
										if (p.GetValue(currentException, null) != null)
										{
											// Cast the collection into a local variable.
											currentAdditionalInfo = (NameValueCollection)p.GetValue(currentException, null);

											// Check if the collection contains values.
											if (currentAdditionalInfo.Count > 0)
											{
												strInfo.AppendFormat("Additional Information:{0}", Environment.NewLine);

												// Loop through the collection adding the information to the string builder.
												for (int i = 0; i < currentAdditionalInfo.Count; i++)
												{
													strInfo.AppendFormat("{1,20}: {2}{0}", Environment.NewLine, currentAdditionalInfo.GetKey(i), currentAdditionalInfo[i]);
												}
											}
										}
									}
									else
									{
										// Otherwise just write the ToString() value of the property.
										strInfo.AppendFormat("{1,20}: {2}{0}", Environment.NewLine, p.Name, p.GetValue(currentException, null));
									}
								}
							}

						}
						catch
						{
							// Ignore exceptions from a single property
						}
					}
					#endregion

					if (currentException.StackTrace != null)
					{
						strInfo.AppendFormat("{1,20}:{0}{2}{0}{0}", Environment.NewLine, "Trace", currentException.StackTrace);
					}

					// Reset the temp exception object and iterate the counter.
					currentException = currentException.InnerException;
					intExceptionCount++;
				} while (currentException != null);

				#region Record the contents of the AdditionalInfo collection
				// Record the contents of the AdditionalInfo collection.
				if (additionalInfo != null)
				{
					// Record General information.
					foreach (string i in additionalInfo) strInfo.AppendFormat(" {1,-20}: {2}{0}", Environment.NewLine, i, additionalInfo.Get(i));
				}
				#endregion

				#endregion
			}

			return strInfo.ToString();

			//			string user = "";
			//			if (HttpContext.Current == null)	user = string.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);
			//			else								user = HttpContext.Current.User.Identity.Name;
			//
			//			string sep = "**=*=*=*=****=*=*=*=****=*=*=*=****=*=*=*=****=*=*=*=****=*=*=*=****=*=*=*=**";
			//			string addi = string.Empty;
			//			string strace = string.Empty;
			//
			//			if (additionalInfo != null) {				
			//				foreach (string s in additionalInfo) 
			//					addi = string.Format("{0}{4,12}{1,-40}: {2}{3}", addi, s, additionalInfo[s], Environment.NewLine, " ");
			//				addi = string.Format("{1,10}:{2}{0}", addi, "Info", Environment.NewLine);
			//			}
			//
			//			if (ex.StackTrace != null) strace = string.Format("{0,10}:{2}{1}{2}", "Trace", ex.StackTrace, Environment.NewLine);
			//
			//			string ret = string.Format("{14}{6}{11,10}: {4}{6}{7,10}: {0}{6}{8,10}: {1}{6}{10,10}: {3}{6}{9,10}: {2}{6}{12}{13}{14}", 
			//				ex.GetType().ToString(), ex.Source, user, ex.TargetSite, ex.Message, 
			//				ex.StackTrace, Environment.NewLine, "Exception", "Source", "User", 
			//				"Target", "Message", addi, strace, sep);
			//
			//			return ret;
		}

		private static NameValueCollection GetAdditionalInfo(Exception exception, NameValueCollection additionalInfo)
		{
			// Create the Additional Information collection if it does not exist.
			if (additionalInfo == null) additionalInfo = new NameValueCollection();

			//if (exception is SoapException) {
			//    additionalInfo.Add("SoapDetails", ((SoapException)exception).Detail.OuterXml);
			//}

			HttpContext ctx = HttpContext.Current;
			if (ctx != null)
			{
				additionalInfo.Add("Browser", string.Format("{0} {1}", ctx.Request.Browser.Browser, ctx.Request.Browser.Version));
				additionalInfo.Add("Url", ctx.Request.Url.ToString());
			}

			additionalInfo.Add("MachineName", Environment.MachineName);
			additionalInfo.Add("TimeStamp", DateTime.Now.ToString());
			additionalInfo.Add("FullName", Assembly.GetExecutingAssembly().FullName);
			additionalInfo.Add("AppDomainName", AppDomain.CurrentDomain.FriendlyName);
			additionalInfo.Add("ThreadIdentity", Thread.CurrentPrincipal.Identity.Name);
			additionalInfo.Add("WindowsIdentity", WindowsIdentity.GetCurrent().Name);

			return additionalInfo;
		}


		//public static string Format(StackTrace trace, bool html, int startFrameIndex) {

		//    if (trace == null) return null;
		//    if (startFrameIndex < 0) throw new ArgumentException("startFrameIndex");

		//    StringBuilder sb = new StringBuilder();

		//    MethodBase method;
		//    string fmethod, filename;
		//    int linenumber, linecolumn;
		//    StackFrame f;

		//    for (int i = startFrameIndex; i < trace.FrameCount; i++ ) {
		//        f = trace.GetFrame(i);
		//        method = f.GetMethod();
		//        fmethod = method.ToString();
		//        filename = f.GetFileName();
		//        linenumber = f.GetFileLineNumber();
		//        linecolumn = f.GetFileColumnNumber();

		//        sb.AppendFormat("at {0} {1}{2}{3}\n"
		//            , method
		//            , string.IsNullOrEmpty(filename) ? "" : filename
		//            , linenumber == 0 ? "" : " Line: " + linenumber.ToString()
		//            , linecolumn == 0 ? "" : " Column : " + linecolumn.ToString());
		//    }

		//    string ret = sb.ToString();
		//    return html ? string.Format("<pre></pre>", ret) : ret;

		//}


		public static string Format(StackTrace trace)
		{
			if (trace == null) return null;

			string resourceString = "at";
			string format = "in {0}:line {1}";

			bool flag = true;
			StringBuilder builder = new StringBuilder(0xff);

			for (int i = 0; i < trace.FrameCount; i++)
			{
				StackFrame frame = trace.GetFrame(i);
				MethodBase method = frame.GetMethod();
				if (method != null)
				{
					if (flag)
					{
						flag = false;
					}
					else
					{
						builder.Append(Environment.NewLine);
					}
					builder.AppendFormat(CultureInfo.InvariantCulture, "{0} ", resourceString);
					Type declaringType = method.DeclaringType;
					if (declaringType != null)
					{
						builder.Append(declaringType.FullName.Replace('+', '.'));
						builder.Append(".");
					}
					builder.Append(method.Name);
					if ((method is MethodInfo) && ((MethodInfo)method).IsGenericMethod)
					{
						Type[] genericArguments = ((MethodInfo)method).GetGenericArguments();
						builder.Append("[");
						int index = 0;
						bool flag2 = true;
						while (index < genericArguments.Length)
						{
							if (!flag2)
							{
								builder.Append(",");
							}
							else
							{
								flag2 = false;
							}
							builder.Append(genericArguments[index].Name);
							index++;
						}
						builder.Append("]");
					}
					builder.Append("(");
					ParameterInfo[] parameters = method.GetParameters();
					bool flag3 = true;
					for (int j = 0; j < parameters.Length; j++)
					{
						if (!flag3)
						{
							builder.Append(", ");
						}
						else
						{
							flag3 = false;
						}
						string name = "<UnknownType>";
						if (parameters[j].ParameterType != null)
						{
							name = parameters[j].ParameterType.Name;
						}
						builder.Append(name + " " + parameters[j].Name);
					}
					builder.Append(")");
					if (frame.GetILOffset() != -1)
					{
						string fileName = null;
						try
						{
							fileName = frame.GetFileName();
						}
						catch (SecurityException)
						{
						}
						if (fileName != null)
						{
							builder.Append(' ');
							builder.AppendFormat(CultureInfo.InvariantCulture, format, new object[] { fileName, frame.GetFileLineNumber() });
						}
					}
				}
			}
			builder.Append(Environment.NewLine);

			return builder.ToString();
		}


	}
}
