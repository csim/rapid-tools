@ECHO OFF
SET SPDeployFolder=%ProgramFiles%\msbuild\spdeploy
SET VS8TemplateFolder=%USERPROFILE%\Documents\Visual Studio 2005\Templates\ProjectTemplates
SET VS9TemplateFolder=%USERPROFILE%\Documents\Visual Studio 2008\Templates\ProjectTemplates

SET VS8TemplateFolder=D:\clints\Documents\Visual Studio 2005\Templates\ProjectTemplates
SET VS9TemplateFolder=D:\clints\Documents\Visual Studio 2008\Templates\ProjectTemplates

@ECHO ON


xcopy /crydifs . "%SPDeployFolder%"

copy /y "SPDeploy_VS8_Template.zip"  "%VS8TemplateFolder%\SPDeploy_VS8_Template.zip"

copy /y "SPDeploy_VS9_Template.zip"  "%VS9TemplateFolder%\SPDeploy_VS8_Template.zip"


pause
