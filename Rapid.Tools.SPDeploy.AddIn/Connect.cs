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
using Rapid.Tools.SPDeploy.AddIn.ProjectFiles.FeatureManifest;
using Rapid.Tools.SPDeploy.AddIn.UI.Wizard;

namespace Rapid.Tools.SPDeploy.AddIn
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        public delegate void CommandDelegate();

        private DTE2 _applicationObject;
        private EnvDTE.AddIn _addInInstance;
        private EnvDTE.SolutionEvents _solutionEvents;
        private EnvDTE.SelectionEvents _selectionEvents;
        private EnvDTE.ProjectItemsEvents _projectItemEvents;

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
                AddInInstance = _addInInstance;

                _solutionEvents = (EnvDTE.SolutionEvents)((Events2)_applicationObject.Events).SolutionEvents;
                _selectionEvents = (EnvDTE.SelectionEvents)((Events2)_applicationObject.Events).SelectionEvents;
                _projectItemEvents = (EnvDTE.ProjectItemsEvents)((Events2)_applicationObject.Events).ProjectItemsEvents;
                _projectItemEvents.ItemAdded += new _dispProjectItemsEvents_ItemAddedEventHandler(_projectItemEvents_ItemAdded);
                _selectionEvents.OnChange += new _dispSelectionEvents_OnChangeEventHandler(selectionEvents_OnChange);
                _solutionEvents.Opened += new _dispSolutionEvents_OpenedEventHandler(solutionOpened);
                _solutionEvents.AfterClosing += new _dispSolutionEvents_AfterClosingEventHandler(globalEvents_AfterClosing);




                AppManager.Current = new AppManager(_applicationObject);



                if (connectMode == ext_ConnectMode.ext_cm_UISetup)
                {
                    object[] contextGUIDS = new object[] { };
                    Commands2 commands = (Commands2)_applicationObject.Commands;

                    foreach (CommandBarPopup c in (((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"]).Controls)
                    {
                        if (c.Caption == "&View")
                        {
                            Command siteExplorerCommand = commands.AddNamedCommand2(_addInInstance, "SiteExplorerCommand", "Rapid Site Explorer", "Executes the command for AddWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                            siteExplorerCommand.AddControl(c.CommandBar, 1);
                            break;
                        }
                    }



                }
            }
            catch (Exception ex)
            {
                ExceptionUtil.Handle(ex);
            }
        }

        void _projectItemEvents_ItemAdded(ProjectItem ProjectItem)
        {
            RefreshAddInVariables();
        }

        private void RefreshAddInVariables()
        {
            _currentFeatureFolderNames = null;
        }

        //private static Project _currentProject;

        //public static Project CurrentProject
        //{
        //    get { return _currentProject; }
        //    set { _currentProject = value; }
        //}


        private static Project previousProject = null;

        public static Project PreviousProject
        {
            get { return previousProject; }
            set { previousProject = value; }
        }


        public Document prevDoc;

        private List<string> _currentFeatureFolderNames;

        public List<string> CurrentFeatureFolderNames
        {
            get
            {
                if (_currentFeatureFolderNames == null)
                {
                    _currentFeatureFolderNames = new List<string>();
                    foreach (Project p in _applicationObject.Solution.Projects)
                    {
                        string path = p.FullName;
                        path = path.Remove(path.LastIndexOf("\\"));
                        path += "\\TemplateFiles\\Features";
                        DirectoryInfo dir = new DirectoryInfo(path);
                        foreach (DirectoryInfo di in dir.GetDirectories())
                        {
                            _currentFeatureFolderNames.Add(di.Name);
                        }
                    }
                }
                return _currentFeatureFolderNames;
            }
            set { _currentFeatureFolderNames = value; }
        }

        private string _previousSelectedItem;
        void selectionEvents_OnChange()
        {
            if (_applicationObject.SelectedItems != null)
            {
                string selectedItemName = _applicationObject.SelectedItems.Item(1).Name;
                if (CurrentFeatureFolderNames.Contains(selectedItemName) && selectedItemName != _previousSelectedItem)
                {
                    AppManager.Current.FeatureContext = new FeatureManifest(File.ReadAllText(AppManager.Current.GetFeatureDirectory().FullName + "\\" + selectedItemName + "\\feature.xml"));
                    _previousSelectedItem = selectedItemName;
                }
            }
            AppManager.Current.FeatureContext = null;
        }

        private static SiteExplorerForm _explorerForm;

        public static SiteExplorerForm ExplorerForm
        {
            get { return _explorerForm; }
            set { _explorerForm = value; }
        }

        public static void CloseToolWindow()
        {
            foreach (Window2 w in (Windows2)AppManager.Current.Application.Windows)
            {
                if (w.Caption == "Site Explorer")
                {
                    w.Close(vsSaveChanges.vsSaveChangesNo);
                    break;
                }
            }
        }

        private static EnvDTE.AddIn AddInInstance;


        void globalEvents_AfterClosing()
        {
            CloseToolWindow();
            RefreshAddInVariables();
            UnloadToolMenus();
        }



        void solutionOpened()
        {
            CloseToolWindow();
            RefreshAddInVariables();

            if (RapidToolsMainMenuCommandBar == null || RapidToolsProjectMenuCommandBar == null)
            {
                foreach (Project p in _applicationObject.Solution.Projects)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(p.FullName);

                    XmlNamespaceManager manager = new XmlNamespaceManager(doc.NameTable);
                    manager.AddNamespace("n", "http://schemas.microsoft.com/developer/msbuild/2003");

                    string guids = doc.SelectSingleNode("n:Project/n:PropertyGroup/n:ProjectTypeGuids", manager).InnerText;
                    if (guids == "{349c5851-65df-11da-9384-00065b846f21};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")
                    {
                        LoadToolMenus();
                        _applicationObject.ExecuteCommand("NullCommand", string.Empty);
                        return;
                    }
                }
            }
        }


        CommandBarPopup RapidToolsProjectMenuCommandBar;
        CommandBarPopup RapidToolsMainMenuCommandBar;
        CommandBarPopup RapidToolsFolderMenuCommandBar;

        private void UnloadToolMenus()
        {
            RapidToolsProjectMenuCommandBar.Delete(true);
            RapidToolsMainMenuCommandBar.Delete(true);
            RapidToolsFolderMenuCommandBar.Delete(true);
            
            RapidToolsProjectMenuCommandBar = null;
            RapidToolsMainMenuCommandBar = null;
            RapidToolsFolderMenuCommandBar = null;
        }

        private void LoadToolMenus()
        {
            try
            {
                object[] contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)_applicationObject.Commands;

                Microsoft.VisualStudio.CommandBars.CommandBar mainMenuCommandBar;
                Microsoft.VisualStudio.CommandBars.CommandBar folderCommandBar;
                Microsoft.VisualStudio.CommandBars.CommandBar projectCommandBar;

                mainMenuCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];
                folderCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["Folder"];
                projectCommandBar = ((CommandBars)_applicationObject.CommandBars)["Project"];

                RapidToolsProjectMenuCommandBar = projectCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, 1, true) as CommandBarPopup;
                RapidToolsProjectMenuCommandBar.Caption = "Rapid Tools";

                RapidToolsMainMenuCommandBar = mainMenuCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
                RapidToolsMainMenuCommandBar.Caption = "Rapid Tools";

                RapidToolsFolderMenuCommandBar = folderCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, 1, true) as CommandBarPopup; 
                RapidToolsFolderMenuCommandBar.Caption = "Rapid Tools";


                CommandBarPopup spCustomActionCommandBar = RapidToolsMainMenuCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
                spCustomActionCommandBar.Caption = "Custom Actions";
                CommandBarPopup webPartsCommandBar = RapidToolsMainMenuCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
                webPartsCommandBar.Caption = "Web Parts";
                CommandBarPopup featureReceiversCommandBar = RapidToolsMainMenuCommandBar.Controls.Add(MsoControlType.msoControlPopup, Type.Missing, Type.Missing, Type.Missing, true) as CommandBarPopup;
                featureReceiversCommandBar.Caption = "Feature Modifications";                

                //  Custom Actions
                Command customActionGroupWizardCommand = commands.AddNamedCommand2(_addInInstance, "CustomActionGroupWizard", "Custom Action Group", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                Command customActionWizardCommand = commands.AddNamedCommand2(_addInInstance, "CustomActionWizard", "Custom Action", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                Command HideCustomActionWizardCommand = commands.AddNamedCommand2(_addInInstance, "HideCustomActionWizard", "Hide Custom Action", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                customActionGroupWizardCommand.AddControl(spCustomActionCommandBar.CommandBar, 1);
                customActionGroupWizardCommand.AddControl(RapidToolsFolderMenuCommandBar.CommandBar, 1);

                customActionWizardCommand.AddControl(spCustomActionCommandBar.CommandBar, 2);
                customActionWizardCommand.AddControl(RapidToolsFolderMenuCommandBar.CommandBar, 2);

                HideCustomActionWizardCommand.AddControl(spCustomActionCommandBar.CommandBar, 3);
                HideCustomActionWizardCommand.AddControl(RapidToolsFolderMenuCommandBar.CommandBar, 3);

                //  Web Parts
                Command AddWebPartWizardCommand = commands.AddNamedCommand2(_addInInstance, "AddWebPartWizard", "Blank Web Part", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                Command addAjaxWebPartWizardCommand = commands.AddNamedCommand2(_addInInstance, "AddAjaxWebPartWizard", "Ajax Web Part", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                AddWebPartWizardCommand.AddControl(webPartsCommandBar.CommandBar, 1);
                AddWebPartWizardCommand.AddControl(RapidToolsFolderMenuCommandBar.CommandBar, 4);


                //  Feature Receivers
                Command featureReceiverWizardCommand = commands.AddNamedCommand2(_addInInstance, "FeatureReceiverWizard", "Blank Feature Receiver", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                Command activationDependencyWizardCommand = commands.AddNamedCommand2(_addInInstance, "ActivationDependencyWizard", "Activation Dependency", "Executes the command for AddWsp", true, 174, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                featureReceiverWizardCommand.AddControl(featureReceiversCommandBar.CommandBar, 1);
                featureReceiverWizardCommand.AddControl(RapidToolsFolderMenuCommandBar.CommandBar, 5);

                activationDependencyWizardCommand.AddControl(featureReceiversCommandBar.CommandBar, 2);
                activationDependencyWizardCommand.AddControl(RapidToolsFolderMenuCommandBar.CommandBar, 6);

                //  Incremental Commands
                Command upgradeIncCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeIncremental", "Upgrade All", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                Command upgradeIncAssemblyCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeIncrementalAssembly", "Upgrade Assembly", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                Command upgradeIncFilesCommand = commands.AddNamedCommand2(_addInInstance, "UpgradeIncrementalFiles", "Upgrade Files", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                Command changeServerCommand = commands.AddNamedCommand2(_addInInstance, "ChangeServer", "Change Server", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                changeServerCommand.AddControl(RapidToolsProjectMenuCommandBar.CommandBar, 1);
                upgradeIncFilesCommand.AddControl(RapidToolsProjectMenuCommandBar.CommandBar, 2);
                upgradeIncAssemblyCommand.AddControl(RapidToolsProjectMenuCommandBar.CommandBar, 3);
                upgradeIncCommand.AddControl(RapidToolsProjectMenuCommandBar.CommandBar, 4);

                //  Features
                Command modifyFeature = commands.AddNamedCommand2(_addInInstance, "ModifyFeature", "Modify Feature", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);

                modifyFeature.AddControl(RapidToolsFolderMenuCommandBar.CommandBar, 1);

                folderCommandBar.Controls[3].BeginGroup = true;

                RapidToolsFolderMenuCommandBar.Controls[2].BeginGroup = true;
                RapidToolsFolderMenuCommandBar.Controls[5].BeginGroup = true;
                RapidToolsFolderMenuCommandBar.Controls[6].BeginGroup = true;

                Command nullCommand = commands.AddNamedCommand2(_addInInstance, "nullCommand", "No Available Actions", "Executes the command for RetractWsp", true, 0, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                nullCommand.AddControl(RapidToolsFolderMenuCommandBar.CommandBar, 1);
            }
            catch (System.ArgumentException)
            {
                //If we are here, then the exception is probably because a command with that name
                //  already exists. If so there is no need to recreate the command and we can 
                //  safely ignore the exception.
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
            "Rapid.Tools.SPDeploy.AddIn.Connect.ModifyFeature",
            "Rapid.Tools.SPDeploy.AddIn.Connect.ChangeServer"

            
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
            if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {

                if (commandName.Contains("ModifyFeature") || commandName.Contains("Wizard") || commandName.Contains("nullCommand"))
                {
                    //if (AppManager.Current.ActiveProject != null)
                    //{
                    //    if (_applicationObject.SelectedItems != null)
                    //    {
                    //        if (CurrentFeatureFolderNames.Contains(_applicationObject.SelectedItems.Item(1).Name))
                    //        {
                    //            if (commandName.Contains("nullCommand"))
                    //                status = vsCommandStatus.vsCommandStatusInvisible;
                    //            else
                    //                status = vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported;
                    //            return;
                    //        }
                    //    }
                    //    if (commandName.Contains("nullCommand"))
                    //        status = vsCommandStatus.vsCommandStatusEnabled | vsCommandStatus.vsCommandStatusSupported;
                    //    else
                    //        status = vsCommandStatus.vsCommandStatusInvisible;                        
                    //    return;
                    //}
                }
            }

            //try
            //{
            //    if (neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            //    {
            //        //  todo change for feature specific

            //        if (commands.Contains(commandName) || spDeployCommands.Contains(commandName))
            //        {
            status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
            return;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    ExceptionUtil.Handle(ex);
            //}
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
                    //_folderPath = AppManager.Current.GetFeatureFolder();
                    //if (_folderPath == string.Empty) return;
                }

                string msbuildTarget = null;

                string path;
                FeatureManifest fm;

                if (_applicationObject.SelectedItems != null)
                {
                    try
                    {
                        foreach (DirectoryInfo di in AppManager.Current.GetFeatureDirectory().GetDirectories("*"))
                        {
                            if (di.Name == _applicationObject.SelectedItems.Item(1).Name)
                            {
                                path = AppManager.Current.GetFeatureDirectory().FullName + "\\" + _applicationObject.SelectedItems.Item(1).Name +
                             "\\feature.xml";

                                fm = new FeatureManifest(File.ReadAllText(path));
                                fm.FilePath = path;
                                AppManager.Current.FeatureContext = fm;
                                break;
                            }
                        }
                    }
                    catch { }

                }






                switch (commandName)
                {
                    case "Rapid.Tools.SPDeploy.AddIn.Connect.ChangeServer":
                        SPEnvironmentInfo info = SPEnvironmentInfo.Parse(AppManager.Current.ActiveProject);
                        OpenUrlForm u = new OpenUrlForm();
                        u.Url = info.BaseUrl;
                        u.ShowDialog();
                        string url = u.Url;
                        AppManager.Current.SetMachineInfo(url);
                        AppManager.Current.ResetActiveProject();
                        if (ExplorerForm != null)
                            ExplorerForm.RefreshSite();
                        break;
                    case "Rapid.Tools.SPDeploy.AddIn.Connect.ModifyFeature":
                        BaseForm b = new BaseForm(BaseForm.RapidFormType.Feature);
                        b.ShowDialog();
                        break;

                    case "Rapid.Tools.SPDeploy.AddIn.Connect.AddWebPartWizard":
                        BaseForm webPartForm = new BaseForm(BaseForm.RapidFormType.WebPart);
                        webPartForm.ShowDialog();
                        break;

                    case "Rapid.Tools.SPDeploy.AddIn.Connect.SiteExplorerCommand":
                        {
                            Windows2 windows2 = (Windows2)_applicationObject.Windows;
                            Assembly asm = Assembly.GetExecutingAssembly();
                            string _toolWindowGuid = "{994A5B7F-F536-4343-9FC4-0471AC85380B}";

                            object control = null;
                            Type tcontrol = typeof(SiteExplorerForm);

                            Window _toolWindow = windows2.CreateToolWindow2(_addInInstance, tcontrol.Assembly.Location, tcontrol.FullName, "Site Explorer", _toolWindowGuid, ref control);
                            _toolWindow.SetTabPicture(Resources.Images.Files.FOLDER.GetHbitmap(Color.White));
                            _toolWindow.Visible = true;

                            ExplorerForm = control as SiteExplorerForm;
                        }
                        break;

                    case "Rapid.Tools.SPDeploy.AddIn.Connect.CustomActionGroupWizard":
                        b = new BaseForm(BaseForm.RapidFormType.CustomActionGroup);
                        b.ShowDialog();
                        break;
                    case "Rapid.Tools.SPDeploy.AddIn.Connect.CustomActionWizard":
                        b = new BaseForm(BaseForm.RapidFormType.CustomAction);
                        b.ShowDialog();
                        break;
                    case "Rapid.Tools.SPDeploy.AddIn.Connect.HideCustomActionWizard":
                        HideCustomActionForm _hideCustomActionForm = new HideCustomActionForm();
                        _hideCustomActionForm.folderPath = _folderPath;
                        _hideCustomActionForm.ShowDialog();
                        AppManager.Current.EnsureProjectFilesAdded(_folderPath);
                        break;
                    case "Rapid.Tools.SPDeploy.AddIn.Connect.FeatureReceiverWizard":
                        b = new BaseForm(BaseForm.RapidFormType.FeatureReceiver);
                        b.ShowDialog();
                        break;
                    case "Rapid.Tools.SPDeploy.AddIn.Connect.ActivationDependencyWizard":
                        UI.Forms.BaseForm fff = new UI.Forms.BaseForm(BaseForm.RapidFormType.ActivationDependency);
                        fff.ShowDialog();
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
                ExceptionUtil.Handle(ex);
            }
        }

    }
}
