SET stsadm=C:\Program Files\Common Files\Microsoft Shared\web server extensions\12\BIN\stsadm.exe

"%stsadm%" -o RetractSolution -immediate -name Rapid.Tools.wsp

"%stsadm%" -o ExecAdmSvcJobs

"%stsadm%" -o DeleteSolution -name Rapid.Tools.wsp

pause