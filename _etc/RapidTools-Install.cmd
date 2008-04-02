SET stsadm=C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\BIN\stsadm.exe

"%stsadm%" -o AddSolution -filename RapidTools.wsp

"%stsadm%" -o DeploySolution -immediate -allowgacdeployment -allowcaspolicies -force -name RapidTools.wsp

"%stsadm%" -o ExecAdmSvcJobs

pause