#include <Python.h>
#include <stdlib.h>
#include <windows.h>

/*
  set COFF2OMF=c:\borland\BCC55\bin\coff2omf.exe
  set PYTHONLIB_VC=c:\Python\2.7\libs\Python27.lib
  set PYTHONLIB_BCC=c:\Python\2.7\libs\Python27b.lib
  if not exist "%PYTHONLIB_BCC%" "%COFF2OMF%" "%PYTHONLIB_VC%" "%PYTHONLIB_BCC%"
  c:\borland\bcc55\bin\bcc32 -I"c:\borland\bcc55\include" -I"c:\Python\2.7\include" -L"c:\borland\bcc55\lib" -WD go.cpp "%PYTHONLIB_BCC%"
*/

DWORD WINAPI Thread(LPVOID lpParam);

static PyObject *interface_print(PyObject *, PyObject *args)
{
  const char *command;

  if(!PyArg_ParseTuple(args, "s", &command))
    return NULL;

  printf("%s", command);
    
  return Py_BuildValue("i", strlen(command));
}

static PyObject *interface_gets(PyObject *, PyObject *)
{
  const int BUFSIZE = 1024;
  char buffer[BUFSIZE + 1];
  
  fgets(buffer, sizeof buffer, stdin);
  if(feof(stdin))
    return NULL;
   
  buffer[BUFSIZE] = '\0';
  return Py_BuildValue("s", buffer);
}

static PyMethodDef interfaceMethods[] = {
  {"prints", interface_print, METH_VARARGS, "Prints a string on the console."},
  {"gets",   interface_gets,  METH_NOARGS,  "Gets a string from the console."},
  {NULL, NULL, 0, NULL}
};

void InitModules()
{
  Py_InitModule("interface", interfaceMethods);
}

void ImportModules()
{
  PyRun_SimpleString("import interface");
}

void InitAndImportModules()
{
  InitModules();
  ImportModules();
}

void InitInterpreter()
{
  PyRun_SimpleString("from code import InteractiveConsole   ");
  PyRun_SimpleString("interface.Console=InteractiveConsole()");
  PyRun_SimpleString("del InteractiveConsole                ");
}

void RedirectPythonIO()
{
  PyRun_SimpleString("class Writer:\n def write(self, x):\n  interface.prints(x)    ");
  PyRun_SimpleString("class Reader:\n def readline(self):\n  return interface.gets()");
  PyRun_SimpleString("import sys");
  PyRun_SimpleString("sys.stdout = Writer()");
  PyRun_SimpleString("sys.stderr = Writer()");
  PyRun_SimpleString("sys.stdin  = Reader()");
}

void InitPython()
{
  Py_Initialize();
  InitAndImportModules();
  InitInterpreter();
  RedirectPythonIO();
}

void PrintWelcomeToPython()
{
  PyRun_SimpleString("print 'You can now type Python commands... Be carefull!'");
}

void DeinitPython()
{
  Py_Finalize();
}

void InitStdHandles()
{
	freopen("CONIN$", "r+", stdin);
	freopen("CONOUT$", "w+", stdout);
	freopen("CONOUT$", "w+", stderr);
}

void InitConsole()
{
  AllocConsole();
  SetConsoleTitle("UoDemo+ DLL by Batlin");
  InitStdHandles();
}

void PrintWelcomeToDLL()
{
  puts("Welcome to the Demo Cheat Console!");
}

void CreatePythonThread()
{
  CreateThread(NULL, 0, Thread, NULL, 0, NULL);
}

void WeAreNotInterestedInThreadEvents(HINSTANCE hInstance)
{
  DisableThreadLibraryCalls(hInstance);
}

BOOL WINAPI DllEntryPoint(HINSTANCE hInstance, ULONG ulReason, LPVOID)
{
  if(ulReason == DLL_PROCESS_ATTACH)
  {
    WeAreNotInterestedInThreadEvents(hInstance);
    InitConsole();
    PrintWelcomeToDLL();
    CreatePythonThread();
    return TRUE;
  }
  if(ulReason == DLL_PROCESS_DETACH)
  {
    // At this point we could undo the patches made.
    // But I'm lazy and don't care
    return TRUE;
  }

  return FALSE;
}

volatile bool bExit = false;
DWORD WINAPI Thread(LPVOID)
{
  InitPython();
  PrintWelcomeToPython();
  while(!bExit)
    PyRun_SimpleString("try:\n interface.Console.interact('')\nexcept SystemError:\n pass");
  DeinitPython();
  
  return 0;
}

int main()
{
  Thread(0);
}
