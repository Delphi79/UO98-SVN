@ECHO OFF
SET oldcd=%CD%
CALL cfg.cmd %1
IF "%CD%"=="%oldcd%" GOTO :done
SET newcd=%CD%
CD %oldcd%

SET PYTHONDIR=c:\python27
SET PYTHONLIB=python27.lib

SET COFF2OMF=%BCCPATH%\bin\coff2omf.exe
SET PYTHONINC=-I%PYTHONDIR%\Include
SET PYTHONLIBS=%PYTHONDIR%\Libs
SET PYTHONBCC32=%PYTHONLIBS%\bcc32_%PYTHONLIB%

IF NOT EXIST %COFF2OMF% GOTO CoffError
IF NOT EXIST %PYTHONLIBS%\%PYTHONLIB% GOTO PythonError
IF NOT EXIST %PYTHONBCC32% ECHO Please wait while creating the Python BCC32 library...
IF NOT EXIST %PYTHONBCC32% %COFF2OMF% %PYTHONLIBS%\%PYTHONLIB% %PYTHONBCC32%
IF NOT EXIST %PYTHONBCC32% GOTO BCCError
GOTO :do

:CoffError
ECHO COFF2OMF not found inside the Borland C++ 5.5 installation!
ECHO Ensure it's installed and BCCPATH inside the cfg batch file is correctly set.
GOTO :done

:PythonError
ECHO Python library not found!
ECHO Ensure it's installed and PYTHONDIR+PYTHONLIB inside the batch file is correctly set.
GOTO :done

:BCCError
ECHO Python BCC32 library not found!
GOTO :done

:do
CD %newcd%

:done