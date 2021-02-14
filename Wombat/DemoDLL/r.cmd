@ECHO OFF
IF EXIST %UODEMODLL% START "" /D"%uodemodir%" cmd.exe /D /C "%uodemoexe%"
@rem @start "" /B "%uodemoexe%"
