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
using Rapid.Tools.SPDeploy.AddIn;
using Rapid.Tools.SPDeploy.AddIn.UI.Controls;
using Rapid.Tools.SPDeploy.AddIn.UI.Forms;
using System.Drawing;
using Rapid.Tools.SPDeploy.AddIn.Domain;
using System.Windows.Forms;

namespace Rapid.Tools.SPDeploy.AddIn
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        public delegate void CommandDelegate();

		private DTE2 _applicationObject;
		private EnvDTE.AddIn _addInInstance;

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
			try
			{
				_applicationObject = (DTE2)application;
				_addInInstance = (EnvDTE.AddIn)addInInst;

				AppManager.Current = new AppManager(_applicationObject);

				if (connectMode == ext_ConnectMode.ext_cm_UISetup)
				{
					object[] contextGUIDS = new object[] { };
					Commands2 commands = (Commands2)_applicationObject.Commands;


					//Place the command on the tools menu.
					//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
					Microsoft.VisualStudio.CommandBars.CommandBar mainMenuCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];
					Microsoft.VisualStudio.CommandBars.CommandBar itemCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["Item"];
					Microsoft.VisualStudio.CommandBars.CommandBar projectCommandBar = ((CommandBars)_applicationObject.CommandBars)["Project"];


					//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
					//  just make sure you also update the QueryStatus/Exec method to include the new command names.

					//Find the Tools command bar on the MenuBar command bar:
					CommandBarPopup rootPopup = mainMenuCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
					rootPopup.Caption = "Rapid Tools";

					//  Wizards Menu
					CommandBarPopup addCommandBar = rootPopup.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
					addCommandBar.Caption = "Rapid Project Actions";
					CommandBarPopup spCustomActionCommandBar = addCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
					spCustomActionCommandBar.Caption = "Custom Actions";
					CommandBarPopup webPartsCommandBar = addCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
					webPartsCommandBar.Caption = "Web Parts";
					CommandBarPopup featureReceiversCommandBar = addCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
					featureReceiversCommandBar.Caption = "Feature Modifications";
					//CommandBarPopup definitionsCommandBar = addCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
					//definitionsCommandBar.Caption = "Definitions";


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

					//ContextGuids.vsContextGuidSolutionExists
					//  Tool Window
					Command siteExplorerCommand = commands.AddNamedCommand2(_addInInstance, "SiteExplorerCommand", "Site Explorer", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

					siteExplorerCommand.AddControl(rootPopup.CommandBar, 1);
					siteExplorerCommand.AddControl(projectCommandBar, 1);


					//CommandBarPopup spDeployCommandBar = menuBarCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
					//spDeployCommandBar.Caption = "SPDeploy";


					//CommandBarPopup wspCommandBar = spDeployCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
					//wspCommandBar.Caption = "WSP";
					//CommandBarPopup includesCommandBar = spDeployCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
					//includesCommandBar.Caption = "Incremental";
					//CommandBarPopup sharePointCommandBar = spDeployCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
					//sharePointCommandBar.Caption = "SharePoint";


					//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
					//  just make sure you also update the QueryStatus/Exec method to include the new command names.
					try
					{
						////  WSP Commands
						//Command addWspCommand = commands.AddNamedCommand2(_addInInstance, "AddWsp", "Add WSP", "Executes the command for AddWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						//Command deleteWspCommand = commands.AddNamedCommand2(_addInInstance, "DeleteWsp", "Delete WSP", "Executes the command for DeleteWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						//Command deployWspCommand = commands.AddNamedCommand2(_addInInstance, "DeployWsp", "Deploy WSP", "Executes the command for DeployWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						//Command retractWspCommand = commands.AddNamedCommand2(_addInInstance, "RetractWsp", "Retract WSP", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						//Command cycleWspCommand = commands.AddNamedCommand2(_addInInstance, "CycleWsp", "Cycle WSP", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						//Command upgradeWspCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeWsp", "Upgrade WSP", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

						//addWspCommand.AddControl(wspCommandBar.CommandBar, 1);
						//deleteWspCommand.AddControl(wspCommandBar.CommandBar, 2);
						//deployWspCommand.AddControl(wspCommandBar.CommandBar, 3);
						//retractWspCommand.AddControl(wspCommandBar.CommandBar, 4);
						//cycleWspCommand.AddControl(wspCommandBar.CommandBar, 5);
						//upgradeWspCommand.AddControl(wspCommandBar.CommandBar, 6);


						//  Incremental Commands
						Command upgradeIncCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeIncremental", "Upgrade All", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						Command upgradeIncAssemblyCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeIncrementalAssembly", "Upgrade Assembly", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						Command upgradeIncFilesCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeIncrementalFiles", "Upgrade Files", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);


						upgradeIncFilesCommand.AddControl(projectCommandBar, 1);
						upgradeIncAssemblyCommand.AddControl(projectCommandBar, 2);
						upgradeIncCommand.AddControl(projectCommandBar, 3);


						////  SharePoint Commands
						//Command createWebApplication = commands.AddNamedCommand2(_addInInstance, "CreateWebApplication", "Create Web Application", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						//Command deleteWebApplication = commands.AddNamedCommand2(_addInInstance, "DeleteWebApplication", "Delete Web Application", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						//Command createSite = commands.AddNamedCommand2(_addInInstance, "CreateSite", "Create Site", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						//Command deleteSite = commands.AddNamedCommand2(_addInInstance, "DeleteSite", "Delete Site", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
						//Command cycleSite = commands.AddNamedCommand2(_addInInstance, "CycleSite", "Cycle Site", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

						//createWebApplication.AddControl(sharePointCommandBar.CommandBar, 1);
						//deleteWebApplication.AddControl(sharePointCommandBar.CommandBar, 2);
						//createSite.AddControl(sharePointCommandBar.CommandBar, 3);
						//deleteSite.AddControl(sharePointCommandBar.CommandBar, 4);
						//cycleSite.AddControl(sharePointCommandBar.CommandBar, 5);


						//  Features
						Command modifyFeature = commands.AddNamedCommand2(_addInInstance, "ModifyFeature", "Modify Feature", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

						modifyFeature.AddControl(itemCommandBar, 1);



					}
					catch (System.ArgumentException)
					{
						//If we are here, then the exception is probably because a command with that name
						//  already exists. If so there is no need to recreate the command and we can 
						//  safely ignore the exception.
					}
				}
			}
			catch (Exception ex)
			{
				AppManager.Current.Write(ex);
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
			"Rapid.Tools.SPDeploy.AddIn.Connect.AddWebPartWizard",
			"Rapid.Tools.SPDeploy.AddIn.Connect.AddAjaxWebPartWizard",
			"Rapid.Tools.SPDeploy.AddIn.Connect.FeatureReceiverWizard",
			"Rapid.Tools.SPDeploy.AddIn.Connect.CustomActionGroupWizard",
			"Rapid.Tools.SPDeploy.AddIn.Connect.CustomActionWizard",
			"Rapid.Tools.SPDeploy.AddIn.Connect.HideCustomActionWizard",                    
			"Rapid.Tools.SPDeploy.AddIn.Connect.ActivationDependencyWizard",
			"Rapid.Tools.SPDeploy.AddIn.Connect.SiteExplorerCommand",
            "Rapid.Tools.SPDeploy.AddIn.Connect.UpgradeFiles",
            "Rapid.Tools.SPDeploy.AddIn.Connect.ModifyFeature"

            
        });

		List<string> spDeployCommands = new List<string>(new string[] {
            "Rapid.Tools.SPDeploy.AddIn.Connect.AddWsp",
            "Rapid.Tools.SPDeploy.AddIn.Connect.DeleteWsp",
            "Rapid.Tools.SPDeploy.AddIn.Connect.DeployWsp",
            "Rapid.Tools.SPDeploy.AddIn.Connect.RetractWsp",
            "Rapid.Tools.SPDeploy.AddIn.Connect.CycleWsp",
            "Rapid.Tools.SPDeploy.AddIn.Connect.UpgradeWsp",
            "Rapid.Tools.SPDeploy.AddIn.Connect.UpgradeIncremental",
            "Rapid.Tools.SPDeploy.AddIn.Connect.UpgradeIncrementalAssembly",
            "Rapid.Tools.SPDeploy.AddIn.Connect.UpgradeIncrementalFiles",
            "Rapid.Tools.SPDeploy.AddIn.Connect.CreateWebApplication",
            "Rapid.Tools.SPDeploy.AddIn.Connect.DeleteWebApplication",
            "Rapid.Tools.SPDeploy.AddIn.Connect.CreateSite",
            "Rapid.Tools.SPDeploy.AddIn.Connect.DeleteSite",
            "Rapid.Tools.SPDeploy.AddIn.Connect.CycleSite"
        });

		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			try
			{
				if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
				{
					if (commandName == "Rapid.Tools.SPDeploy.AddIn.Connect.ModifyFeature")
					{
						if (string.Compare(_applicationObject.SelectedItems.Item(1).Name, "feature.xml", true) != 0)
						{
							status = vsCommandStatus.vsCommandStatusInvisible;
							return;
						}

					}

					if (commands.Contains(commandName) || spDeployCommands.Contains(commandName))
					{
						status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
						return;
					}
				}
			}
			catch (Exception ex)
			{
				AppManager.Current.Write(ex);
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
			try
			{
				handled = false;

				if (executeOption != vsCommandExecOption.vsCommandExecOptionDoDefault) return;

				// Abort if the command is invalid
				if (!commands.Contains(commandName) && !spDeployCommands.Contains(commandName)) return;

				string _folderPath = string.Empty;
				if (commands.Contains(commandName) && commandName != "Rapid.Tools.SPDeploy.AddIn.Connect.SiteExplorerCommand" && commandName != "Rapid.Tools.SPDeploy.AddIn.Connect.ModifyFeature")
				{
					_folderPath = AppManager.Current.GetFeatureFolder();
					if (_folderPath == string.Empty) return;
				}

				string msbuildTarget = null;

				switch (commandName)
				{
				case "Rapid.Tools.SPDeploy.AddIn.Connect.ModifyFeature":
					FeatureModificationForm f = new FeatureModificationForm(_applicationObject.SelectedItems.Item(1).ProjectItem.get_FileNames(1));
					f.ShowDialog();
					break;

				case "Rapid.Tools.SPDeploy.AddIn.Connect.AddWebPartWizard":
					UI.Forms.WebPartForm _webPartForm = new UI.Forms.WebPartForm();
					_webPartForm.folderPath = _folderPath;
					_webPartForm.webPartType = Rapid.Tools.SPDeploy.AddIn.UI.Forms.WebPartForm.WebPartType.Blank;
					_webPartForm.projectName = AppManager.Current.GetRootNamespace();
					_webPartForm.ShowDialog();
					AppManager.Current.EnsureProjectFilesAdded(_folderPath);
					_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
					_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
					_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
					AppManager.Current.EnsureProjectFilesAdded(_folderPath + "\\Web\\UI\\WebControls");

					break;

				case "Rapid.Tools.SPDeploy.AddIn.Connect.SiteExplorerCommand":
					{
						Windows2 windows2 = (Windows2)_applicationObject.Windows;
						Assembly asm = Assembly.GetExecutingAssembly();
						string _toolWindowGuid = "{994A5B7F-F536-4343-9FC4-0471AC85380B}";

						object control = null;
						Type tcontrol = typeof(SiteExplorerForm);

						Window _toolWindow = _toolWindow = windows2.CreateToolWindow2(_addInInstance, tcontrol.Assembly.Location, tcontrol.FullName, "Site Explorer", _toolWindowGuid, ref control);
						_toolWindow.SetTabPicture(Resources.Images.Files.FOLDER.GetHbitmap(Color.Gainsboro));

						SiteExplorerForm c = control as SiteExplorerForm;
						c.FillTreeView();
						_toolWindow.Visible = true;
					}
					break;

				case "Rapid.Tools.SPDeploy.AddIn.Connect.CustomActionGroupWizard":
					CustomActionGroupForm _customActionGroupForm = new CustomActionGroupForm();
					_customActionGroupForm.folderPath = _folderPath;
					_customActionGroupForm.ShowDialog();
					AppManager.Current.EnsureProjectFilesAdded(_folderPath);
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.CustomActionWizard":
					CustomActionForm _customActionForm = new CustomActionForm();
					_customActionForm.folderPath = _folderPath;
					_customActionForm.ShowDialog();
					AppManager.Current.EnsureProjectFilesAdded(_folderPath);
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.HideCustomActionWizard":
					HideCustomActionForm _hideCustomActionForm = new HideCustomActionForm();
					_hideCustomActionForm.folderPath = _folderPath;
					_hideCustomActionForm.ShowDialog();
					AppManager.Current.EnsureProjectFilesAdded(_folderPath);
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.FeatureReceiverWizard":
					FeatureReceiverForm _featureReceiverForm = new FeatureReceiverForm();
					_featureReceiverForm.featureFolderPath = _folderPath;
					_featureReceiverForm.projectName = AppManager.Current.ActiveProject.Name;
					_featureReceiverForm.ShowDialog();
					AppManager.Current.EnsureProjectFilesAdded(_folderPath);
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.ActivationDependencyWizard":
					ActivationDependencyForm _activationDependenceForm = new ActivationDependencyForm();
					_activationDependenceForm.folderPath = _folderPath;
					_activationDependenceForm.GetFeatures();
					_activationDependenceForm.ShowDialog();
					AppManager.Current.EnsureProjectFilesAdded(_folderPath);
					_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
					_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
					_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
					AppManager.Current.EnsureProjectFilesAdded(_folderPath + "\\Receivers");
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.AddAjaxWebPartWizard":
					WebPartForm _ajaxWebPartForm = new WebPartForm();
					_ajaxWebPartForm.folderPath = _folderPath;
					_ajaxWebPartForm.webPartType = WebPartForm.WebPartType.Ajax;
					_ajaxWebPartForm.projectName = AppManager.Current.GetRootNamespace();
					_ajaxWebPartForm.ShowDialog();

					AppManager.Current.EnsureProjectFilesAdded(_folderPath);

					_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
					_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
					_folderPath = _folderPath.Remove(_folderPath.LastIndexOf("\\"));
					AppManager.Current.EnsureProjectFilesAdded(_folderPath + "\\Web\\UI\\WebControls");
					AppManager.Current.EnsureProjectFilesAdded(_folderPath + "\\ISAPI");
					AppManager.Current.EnsureProjectFilesAdded(_folderPath + "\\Receivers");
					break;

				case "Rapid.Tools.SPDeploy.AddIn.Connect.AddWsp":
					msbuildTarget = "AddWsp";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.DeleteWsp":
					msbuildTarget = "DeleteWsp";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.DeployWsp":
					msbuildTarget = "DeployWsp";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.RetractWsp":
					msbuildTarget = "RetractWsp";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.CycleWsp":
					msbuildTarget = "CycleWsp";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.UpgradeWsp":
					msbuildTarget = "UpgradeWsp";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.UpgradeIncremental":
					msbuildTarget = "UpgradeIncremental";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.UpgradeIncrementalAssembly":
					msbuildTarget = "UpgradeIncrementalAssembly";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.UpgradeIncrementalFiles":
					msbuildTarget = "UpgradeIncrementalFiles";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.CreateWebApplication":
					msbuildTarget = "CreateWebApplication";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.DeleteWebApplication":
					msbuildTarget = "DeleteWebApplication";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.CreateSite":
					msbuildTarget = "CreateSite";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.DeleteSite":
					msbuildTarget = "DeleteSite";
					break;
				case "Rapid.Tools.SPDeploy.AddIn.Connect.CycleSite":
					msbuildTarget = "CycleSite";
					break;
				}

				if (!string.IsNullOrEmpty(msbuildTarget))
				{
					CommandDelegate d = delegate()
					{
						AppManager.Current.ExecuteMSBuild(msbuildTarget);
					};
					d.BeginInvoke(new AsyncCallback(delegate(IAsyncResult res) { }), null);
				}
			}
			catch (Exception ex)
			{
				AppManager.Current.Write(ex);
			}
		}

    }
}