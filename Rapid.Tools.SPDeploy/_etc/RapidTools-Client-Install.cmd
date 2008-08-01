@ECHO OFF
SET SPDeployFolder=%ProgramFiles%\MSBuild\SPDeploy\v2

SET VS2005TemplateFolder=%USERPROFILE%\Documents\Visual Studio 2005\Templates\ProjectTemplates
SET VS2008TemplateFolder=%USERPROFILE%\Documents\Visual Studio 2008\Templates\ProjectTemplates

rem SET VS2005TemplateFolder=D:\clints\Documents\Visual Studio 2005\Templates\ProjectTemplates
rem SET VS2008TemplateFolder=D:\clints\Documents\Visual Studio 2008\Templates\ProjectTemplates

@ECHO ON


xcopy /crydifs . "%SPDeployFolder%"

copy /y "SPDeploy_VS2005_Template.zip"  "%VS2005TemplateFolder%\SPDeploy_VS2005_Template.zip"

copy /y "SPDeploy_VS2008_Template.zip"  "%VS2008TemplateFolder%\SPDeploy_VS2008_Template.zip"


pause
