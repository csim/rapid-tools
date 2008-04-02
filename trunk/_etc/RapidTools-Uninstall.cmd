SET stsadm=C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\BIN\stsadm.exe

"%stsadm%" -o RetractSolution -immediate -name RapidTools.wsp

"%stsadm%" -o ExecAdmSvcJobs

"%stsadm%" -o DeleteSolution -name RapidTools.wsp

pause