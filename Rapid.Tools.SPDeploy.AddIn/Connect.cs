using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Security.Principal;
using System.Threading;
using Microsoft.Win32;
using Rapid.Tools.SPDeploy.AddIn.UI.Controls;
using Rapid.Tools.SPDeploy.AddIn.UI.Forms;

namespace Rapid.Tools.SPDeploy.AddIn
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{
		public delegate void CommandDelegate();

		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (EnvDTE.AddIn)addInInst;
			if (connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
				object[] contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)_applicationObject.Commands;
				string toolsMenuName;

				try
				{
					//If you would like to move the command to a different menu, change the word "Tools" to the 
					//  English version of the menu. This code will take the culture, append on the name of the menu
					//  then add the command to that menu. You can find a list of all the top-level menus in the file
					//  CommandBar.resx.
					ResourceManager resourceManager = new ResourceManager("SPTools.CommandBar", Assembly.GetExecutingAssembly());
					CultureInfo cultureInfo = new System.Globalization.CultureInfo(_applicationObject.LocaleID);
					string resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, "Tools");
					toolsMenuName = resourceManager.GetString(resourceName);
				}
				catch
				{
					//We tried to find a localized version of the word Tools, but one was not found.
					//  Default to the en-US word, which may work for the current culture.
					toolsMenuName = "Tools";
				}

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
				Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

				//Find the Tools command bar on the MenuBar command bar:
				CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
				CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.

				//Find the Tools command bar on the MenuBar command bar:
				CommandBarPopup spProjectCommandBar = menuBarCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				spProjectCommandBar.Caption = "SPProject";

				//  Wizards Menu
				CommandBarPopup addCommandBar = spProjectCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				addCommandBar.Caption = "Wizards";
				CommandBarPopup spCustomActionCommandBar = addCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				spCustomActionCommandBar.Caption = "Custom Actions";
				CommandBarPopup webPartsCommandBar = addCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				webPartsCommandBar.Caption = "Web Parts";
				CommandBarPopup featureReceiversCommandBar = addCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				featureReceiversCommandBar.Caption = "Feature Modifications";
				CommandBarPopup definitionsCommandBar = addCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				definitionsCommandBar.Caption = "Definitions";

				//  Tools Menu
				CommandBarPopup toolsCommandBar = spProjectCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				toolsCommandBar.Caption = "Tools";


				//  Custom Actions
				Command customActionGroupWizardCommand = commands.AddNamedCommand2(_addInInstance, "CustomActionGroupWizard", "Custom Action Group", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
				Command customActionWizardCommand = commands.AddNamedCommand2(_addInInstance, "CustomActionWizard", "Custom Action", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
				Command HideCustomActionWizardCommand = commands.AddNamedCommand2(_addInInstance, "HideCustomActionWizard", "Hide Custom Action", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

				customActionGroupWizardCommand.AddControl(spCustomActionCommandBar.CommandBar, 1);
				customActionWizardCommand.AddControl(spCustomActionCommandBar.CommandBar, 2);
				HideCustomActionWizardCommand.AddControl(spCustomActionCommandBar.CommandBar, 3);

				//  Web Parts
				Command AddWebPartWizardCommand = commands.AddNamedCommand2(_addInInstance, "AddWebPartWizard", "Blank Web Part", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
				Command addAjaxWebPartWizardCommand = commands.AddNamedCommand2(_addInInstance, "AddAjaxWebPartWizard", "Ajax Web Part", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

				AddWebPartWizardCommand.AddControl(webPartsCommandBar.CommandBar, 1);
				addAjaxWebPartWizardCommand.AddControl(webPartsCommandBar.CommandBar, 2);

				//  Feature Receivers
				Command featureReceiverWizardCommand = commands.AddNamedCommand2(_addInInstance, "FeatureReceiverWizard", "Blank Feature Receiver", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
				Command activationDependencyWizardCommand = commands.AddNamedCommand2(_addInInstance, "ActivationDependencyWizard", "Activation Dependency", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

				featureReceiverWizardCommand.AddControl(featureReceiversCommandBar.CommandBar, 1);
				activationDependencyWizardCommand.AddControl(featureReceiversCommandBar.CommandBar, 2);

				//  Tool Window
				Command toolWindowCommand = commands.AddNamedCommand2(_addInInstance, "ToolWindowCommand", "Tool Window Command", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

				toolWindowCommand.AddControl(toolsCommandBar.CommandBar, 1);


				CommandBarPopup spDeployCommandBar = menuBarCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				spDeployCommandBar.Caption = "SPDeploy";


				CommandBarPopup wspCommandBar = spDeployCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				wspCommandBar.Caption = "WSP";
				CommandBarPopup includesCommandBar = spDeployCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				includesCommandBar.Caption = "Incremental";
				CommandBarPopup sharePointCommandBar = spDeployCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
				sharePointCommandBar.Caption = "SharePoint";


				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				try
				{
					//  WSP Commands
					Command addWspCommand = commands.AddNamedCommand2(_addInInstance, "AddWsp", "Add WSP", "Executes the command for AddWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command deleteWspCommand = commands.AddNamedCommand2(_addInInstance, "DeleteWsp", "Delete WSP", "Executes the command for DeleteWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command deployWspCommand = commands.AddNamedCommand2(_addInInstance, "DeployWsp", "Deploy WSP", "Executes the command for DeployWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command retractWspCommand = commands.AddNamedCommand2(_addInInstance, "RetractWsp", "Retract WSP", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command cycleWspCommand = commands.AddNamedCommand2(_addInInstance, "CycleWsp", "Cycle WSP", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command upgradeWspCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeWsp", "Upgrade WSP", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					addWspCommand.AddControl(wspCommandBar.CommandBar, 1);
					deleteWspCommand.AddControl(wspCommandBar.CommandBar, 2);
					deployWspCommand.AddControl(wspCommandBar.CommandBar, 3);
					retractWspCommand.AddControl(wspCommandBar.CommandBar, 4);
					cycleWspCommand.AddControl(wspCommandBar.CommandBar, 5);
					upgradeWspCommand.AddControl(wspCommandBar.CommandBar, 6);


					//  Incremental Commands
					Command upgradeIncCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeIncremental", "Upgrade All", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command upgradeIncAssemblyCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeIncrementalAssembly", "Upgrade Assembly", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command upgradeIncFilesCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeIncrementalFiles", "Upgrade Files", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					upgradeIncCommand.AddControl(includesCommandBar.CommandBar, 1);
					upgradeIncAssemblyCommand.AddControl(includesCommandBar.CommandBar, 2);
					upgradeIncFilesCommand.AddControl(includesCommandBar.CommandBar, 3);


					//  SharePoint Commands
					Command createWebApplication = commands.AddNamedCommand2(_addInInstance, "CreateWebApplication", "Create Web Application", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command deleteWebApplication = commands.AddNamedCommand2(_addInInstance, "DeleteWebApplication", "Delete Web Application", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command createSite = commands.AddNamedCommand2(_addInInstance, "CreateSite", "Create Site", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command deleteSite = commands.AddNamedCommand2(_addInInstance, "DeleteSite", "Delete Site", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
					Command cycleSite = commands.AddNamedCommand2(_addInInstance, "CycleSite", "Cycle Site", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					createWebApplication.AddControl(sharePointCommandBar.CommandBar, 1);
					deleteWebApplication.AddControl(sharePointCommandBar.CommandBar, 2);
					createSite.AddControl(sharePointCommandBar.CommandBar, 3);
					deleteSite.AddControl(sharePointCommandBar.CommandBar, 4);
					cycleSite.AddControl(sharePointCommandBar.CommandBar, 5);


				}
				catch (System.ArgumentException)
				{
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
					//  safely ignore the exception.
				}
			}
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}

		List<string> commands = new List<string>(new string[] {
			"SPTools.Connect.AddWebPartWizard",
			"SPTools.Connect.AddAjaxWebPartWizard",
			"SPTools.Connect.FeatureReceiverWizard",
			"SPTools.Connect.CustomActionGroupWizard",
			"SPTools.Connect.CustomActionWizard",
			"SPTools.Connect.HideCustomActionWizard",                    
			"SPTools.Connect.ActivationDependencyWizard",
			"SPTools.Connect.ToolWindowCommand",
            "SPDeployAddIn.Connect.AddWsp",
            "SPDeployAddIn.Connect.DeleteWsp",
            "SPDeployAddIn.Connect.DeployWsp",
            "SPDeployAddIn.Connect.RetractWsp",
            "SPDeployAddIn.Connect.CycleWsp",
            "SPDeployAddIn.Connect.UpgradeWsp",
            "SPDeployAddIn.Connect.UpgradeIncremental",
            "SPDeployAddIn.Connect.UpgradeIncrementalAssembly",
            "SPDeployAddIn.Connect.UpgradeIncrementalFiles",
            "SPDeployAddIn.Connect.CreateWebApplication",
            "SPDeployAddIn.Connect.DeleteWebApplication",
            "SPDeployAddIn.Connect.CreateSite",
            "SPDeployAddIn.Connect.DeleteSite",
            "SPDeployAddIn.Connect.CycleSite"
        });

		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if (commands.Contains(commandName))
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}
		}



		/// <summary>Implements the Exec method of the IDTCommandTarget interface. This is called when the command is invoked.</summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;

			if (executeOption != vsCommandExecOption.vsCommandExecOptionDoDefault) return;

			// Abort if the command is invalid
			if (!commands.Contains(commandName)) return;

			string _folderPath = string.Empty;
			if (commandName != "SPTools.Connect.ToolWindowCommand")
			{
				_folderPath = getFeatureFolder();
				if (_folderPath == string.Empty) return;
			}

			string msbuildTarget = null;

			switch (commandName)
			{
			case "SPTools.Connect.AddWebPartWizard":
				UI.Forms.WebPartForm _webPartForm = new UI.Forms.WebPartForm();
				_webPartForm.folderPath = _folderPath;
				_webPartForm.webPartType = Rapid.Tools.SPDeploy.AddIn.UI.Forms.WebPartForm.WebPartType.Blank;
				_webPartForm.projectName = getRootNamespace();
				_webPartForm.ShowDialog();
				ensureFilesAdded(_folderPath);
				_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
				_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
				_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
				ensureFilesAdded(_folderPath + "\\Web\\UI\\WebControls");

				break;

			case "SPTools.Connect.ToolWindowCommand":
				Windows2 windows2 = (Windows2)_applicationObject.Windows;
				Assembly asm = Assembly.GetExecutingAssembly();
				string _toolWindowGuid = "{994A5B7F-F536-4343-9FC4-0471AC85380B}";

				object customControl = null;
				string className = "SPTools.UI.Controls.SPToolsWindowControl";
				string caption = "SPTools";
				Window _toolWindow = null;
				try
				{
					_toolWindow = windows2.CreateToolWindow2(_addInInstance, asm.Location, className,
															 caption, _toolWindowGuid, ref customControl);
				}
				catch (Exception)
				{
				}

				SPToolsWindowControl c = customControl as SPToolsWindowControl;
				c._project = _applicationObject.Solution.Projects.Item(1);
				c.path = _applicationObject.Solution.Projects.Item(1).FullName.Remove(_applicationObject.Solution.Projects.Item(1).FullName.LastIndexOf("\\"));

				_toolWindow.Visible = true;

				break;

			case "SPTools.Connect.CustomActionGroupWizard":
				CustomActionGroupForm _customActionGroupForm = new CustomActionGroupForm();
				_customActionGroupForm.folderPath = _folderPath;
				_customActionGroupForm.ShowDialog();
				ensureFilesAdded(_folderPath);
				break;
			case "SPTools.Connect.CustomActionWizard":
				CustomActionForm _customActionForm = new CustomActionForm();
				_customActionForm.folderPath = _folderPath;
				_customActionForm.ShowDialog();
				ensureFilesAdded(_folderPath);
				break;
			case "SPTools.Connect.HideCustomActionWizard":
				HideCustomActionForm _hideCustomActionForm = new HideCustomActionForm();
				_hideCustomActionForm.folderPath = _folderPath;
				_hideCustomActionForm.ShowDialog();
				ensureFilesAdded(_folderPath);
				break;
			case "SPTools.Connect.FeatureReceiverWizard":
				FeatureReceiverForm _featureReceiverForm = new FeatureReceiverForm();
				_featureReceiverForm.featureFolderPath = _folderPath;
				_featureReceiverForm.projectName = _applicationObject.Solution.Projects.Item(1).Name;
				_featureReceiverForm.ShowDialog();
				ensureFilesAdded(_folderPath);
				break;
			case "SPTools.Connect.ActivationDependencyWizard":
				ActivationDependencyForm _activationDependenceForm = new ActivationDependencyForm();
				_activationDependenceForm.folderPath = _folderPath;
				_activationDependenceForm.GetFeatures();
				_activationDependenceForm.ShowDialog();
				ensureFilesAdded(_folderPath);
				_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
				_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
				_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
				ensureFilesAdded(_folderPath + "\\Receivers");
				break;
			case "SPTools.Connect.AddAjaxWebPartWizard":
				WebPartForm _ajaxWebPartForm = new WebPartForm();
				_ajaxWebPartForm.folderPath = _folderPath;
				_ajaxWebPartForm.webPartType = WebPartForm.WebPartType.Ajax;
				_ajaxWebPartForm.projectName = getRootNamespace();
				_ajaxWebPartForm.ShowDialog();

				ensureFilesAdded(_folderPath);

				_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
				_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
				_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
				ensureFilesAdded(_folderPath + "\\Web\\UI\\WebControls");
				ensureFilesAdded(_folderPath + "\\ISAPI");
				ensureFilesAdded(_folderPath + "\\Receivers");
				break;

			case "SPDeployAddIn.Connect.AddWsp":
				msbuildTarget = "AddWsp";
				break;
			case "SPDeployAddIn.Connect.DeleteWsp":
				msbuildTarget = "DeleteWsp";
				break;
			case "SPDeployAddIn.Connect.DeployWsp":
				msbuildTarget = "DeployWsp";
				break;
			case "SPDeployAddIn.Connect.RetractWsp":
				msbuildTarget = "RetractWsp";
				break;
			case "SPDeployAddIn.Connect.CycleWsp":
				msbuildTarget = "CycleWsp";
				break;
			case "SPDeployAddIn.Connect.UpgradeWsp":
				msbuildTarget = "UpgradeWsp";
				break;
			case "SPDeployAddIn.Connect.UpgradeIncremental":
				msbuildTarget = "UpgradeIncremental";
				break;
			case "SPDeployAddIn.Connect.UpgradeIncrementalAssembly":
				msbuildTarget = "UpgradeIncrementalAssembly";
				break;
			case "SPDeployAddIn.Connect.UpgradeIncrementalFiles":
				msbuildTarget = "UpgradeIncrementalFiles";
				break;
			case "SPDeployAddIn.Connect.CreateWebApplication":
				msbuildTarget = "CreateWebApplication";
				break;
			case "SPDeployAddIn.Connect.DeleteWebApplication":
				msbuildTarget = "DeleteWebApplication";
				break;
			case "SPDeployAddIn.Connect.CreateSite":
				msbuildTarget = "CreateSite";
				break;
			case "SPDeployAddIn.Connect.DeleteSite":
				msbuildTarget = "DeleteSite";
				break;
			case "SPDeployAddIn.Connect.CycleSite":
				msbuildTarget = "CycleSite";
				break;
			}

			if (!string.IsNullOrEmpty(msbuildTarget))
			{
				CommandDelegate d = delegate()
				{
					ExecuteCommand(@"/target:" + msbuildTarget + @" """ + _applicationObject.Solution.Projects.Item(1).FullName + @"""");
				};
				d.BeginInvoke(new AsyncCallback(delegate(IAsyncResult res) { }), null);
			}
		}
		private DTE2 _applicationObject;
		private EnvDTE.AddIn _addInInstance;

		public string getFeatureFolder()
		{
			UI.Forms.FeatureForm _featureForm = new UI.Forms.FeatureForm();
			_featureForm.GetFeatures(_applicationObject.Solution.Projects.Item(1).FullName);
			_featureForm.ShowDialog();

			if (_featureForm.Canceled)
				return string.Empty;
			return _featureForm.FileLocation.Remove(_featureForm.FileLocation.LastIndexOf("\\"));
		}

		public string getRootNamespace()
		{
			XmlDocument d = new XmlDocument();
			d.Load(_applicationObject.Solution.Projects.Item(1).FullName);

			XmlNamespaceManager nm = new XmlNamespaceManager(d.NameTable);
			nm.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

			return d.SelectSingleNode("/n:Project/n:PropertyGroup/n:RootNamespace", nm).InnerText;
		}

		public void ensureFilesAdded(string folderPath)
		{
			foreach (FileInfo fi in new DirectoryInfo(folderPath).GetFiles("*", SearchOption.AllDirectories))
			{
				_applicationObject.Solution.Projects.Item(1).ProjectItems.AddFromFile(fi.FullName);
			}
		}


		void ExecuteCommand(string command)
		{
			try
			{

				string _tpath = Path.GetTempPath();
				_tpath += "SPDeployBuild.bat";

				using (TextWriter _textWriter = new StreamWriter(_tpath, false))
				{
					_textWriter.WriteLine(@"C:\Windows\Microsoft.NET\Framework\v2.0.50727\MSBuild.exe " + command);
					_textWriter.WriteLine("pause");
					_textWriter.Flush();
					_textWriter.Close();
				}

				ProcessStartInfo _processInfo = new ProcessStartInfo(_tpath);

				_processInfo.UseShellExecute = false;
				_processInfo.CreateNoWindow = false;

				//  commenting in place for piping to output pane

				//_processInfo.CreateNoWindow = true;
				//_processInfo.RedirectStandardOutput = true;
				//_processInfo.RedirectStandardError = true;

				//_processInfo.StandardOutputEncoding = Encoding.ASCII;
				//_processInfo.StandardErrorEncoding = Encoding.ASCII;
				//_processInfo.LoadUserProfile = false;

				System.Diagnostics.Process _process = new System.Diagnostics.Process();

				_process.StartInfo = _processInfo;
				//_process.EnableRaisingEvents = true;

				_process.Start();

				//OutputWindow _outputWindow = _applicationObject.ToolWindows.OutputWindow;
				//OutputWindowPane _pane = null;
				//IEnumerator enu = _outputWindow.OutputWindowPanes.GetEnumerator();
				//while (enu.MoveNext())
				//{
				//    if (((OutputWindowPane)enu.Current).Name == "SPDeploy")
				//        _pane = enu.Current as OutputWindowPane;
				//}
				//if (_pane == null)
				//    _pane = _outputWindow.OutputWindowPanes.Add("SPDeploy");
				//_pane.Activate();
				//_pane.Clear();
				//string line = string.Empty;

				//int i = 0;
				//do
				//{
				//    line = _process.StandardOutput.ReadLine();
				//    i++;
				//    if (!string.IsNullOrEmpty(line))
				//    {
				//        _pane.OutputString(line + "\r\n");
				//    }
				//} while (!_process.StandardOutput.EndOfStream);

				//_pane.OutputString("Execution complete");

			}
			catch (Exception) { /* TODO: use logging  */ }
		}

	}
}