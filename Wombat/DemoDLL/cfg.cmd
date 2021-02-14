@ECHO OFF
SET BCCPATH=c:\borland\bcc55
SET TASMPATH=c:\borland\tasm

SET UODEMODIR=..\DemoT2A
SET UODEMOEXE=Start 1998-06-03.cmd

SET APIDIR=%CD%\API
SET PRJNAME=%1
SET PRJDIR=%CD%\%PRJNAME%

IF NOT EXIST %BCCPATH%\bin\bcc32.exe GOTO BCCerror
IF NOT EXIST %TASMPATH%\bin\tasm32.exe GOTO TASMerror
IF NOT EXIST "%UODEMODIR%\" GOTO DemoError
IF NOT EXIST "%UODEMODIR%\%UODEMOEXE%" GOTO DemoError
IF "%1" EQU "" GOTO ParaError
IF "%1" EQU "api" GOTO ParaError
IF NOT EXIST %PRJDIR% GOTO DirError
GOTO :do

:BCCerror
ECHO Borland C++ 5.5 not found!
ECHO Ensure it's installed and BCCPATH inside the batch file is correctly set.
GOTO :done

:TASMerror
ECHO Borland Turbo Assembler not found!
ECHO Ensure it's installed and TASMPATH inside the batch file is correctly set.
GOTO :done

:DemoError
ECHO Ultima Online Demo not found!
ECHO Ensure UODEMODIR ^& UODEMOEXE inside the batch file is correctly set.
GOTO :done

:ParaError
ECHO Please provide the project name you are working on...
GOTO :done

:DirError
ECHO Unable to locate the project directory!
GOTO :done

:do
SET INC=%BCCPATH%\INCLUDE
SET LIB=%BCCPATH%\LIB
PATH=%PATH%;%BCCPATH%\BIN;%TASMPATH%\BIN
PATH=%PATH%;%CD%
CD %UODEMODIR%
SET UODEMODIR=%CD%
SET UODEMOEXE=%UODEMODIR%\%UODEMOEXE%
SET UODEMODLL=%PRJDIR%\%PRJNAME%.DLL
CD %PRJDIR%

:done