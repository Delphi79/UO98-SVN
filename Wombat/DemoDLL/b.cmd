@ECHO OFF
IF EXIST %prjname%.dll DEL %prjname%.dll
IF EXIST %prjname%.cpp dll %prjname%.cpp
IF EXIST %prjname%.c   dll %prjname%.c