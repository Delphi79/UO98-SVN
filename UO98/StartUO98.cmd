@ECHO OFF
SET REGKEYNAME=UO98
SET SERVERNAME=UO98
SET RUNDIR=rundir
SET REALDAMAGE=YES
SET USEACCOUNTNAME=YES
SET SAVEDYNAMIC0=NO
rem SET LOCALCOUNSELOR=0
rem SET UODEMODLL=%CD%\Dev\UO98dll\src\UO98.dll
SET UODEMODLL=%CD%\Bin\Sidekick.dll
CD Bin 
echo Parsing UOSL scripts...
uosl.exe -outspec Enhanced -outdir ..\rundir\scripts.uosl -overwrite ..\rundir\scripts.uosl\*.uosl
START "" /B UoDemo+.exe

:DONE
CD ..
