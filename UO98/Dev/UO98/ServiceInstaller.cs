// Thanks to Sachin Nigam for the code.
// http://www.c-sharpcorner.com/UploadFile/sachin.nigam/InstallingWinServiceProgrammatically11262005061332AM/InstallingWinServiceProgrammatically.aspx

using System;
using System.Runtime.InteropServices;
namespace UO98
{
    partial class UO98Service
    {
        class CustomServiceInstaller
        {
            const int SC_MANAGER_CREATE_SERVICE = 0x0002;
            const int SERVICE_WIN32_OWN_PROCESS = 0x00000010;
            const int SERVICE_INTERACTIVE_PROCESS = 0x00000100;
            const int SERVICE_DEMAND_START = 0x00000003;
            const int SERVICE_ERROR_NORMAL = 0x00000001;
            const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
            const int SERVICE_QUERY_CONFIG = 0x0001;
            const int SERVICE_CHANGE_CONFIG = 0x0002;
            const int SERVICE_QUERY_STATUS = 0x0004;
            const int SERVICE_ENUMERATE_DEPENDENTS = 0x0008;
            const int SERVICE_START = 0x0010;
            const int SERVICE_STOP = 0x0020;
            const int SERVICE_PAUSE_CONTINUE = 0x0040;
            const int SERVICE_INTERROGATE = 0x0080;
            const int SERVICE_USER_DEFINED_CONTROL = 0x0100;
            const int SERVICE_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
            SERVICE_QUERY_CONFIG |
            SERVICE_CHANGE_CONFIG |
            SERVICE_QUERY_STATUS |
            SERVICE_ENUMERATE_DEPENDENTS |
            SERVICE_START |
            SERVICE_STOP |
            SERVICE_PAUSE_CONTINUE |
            SERVICE_INTERROGATE |
            SERVICE_USER_DEFINED_CONTROL);
            const int SERVICE_AUTO_START = 0x00000002;



            private int m_StartMode = SERVICE_DEMAND_START;

            [DllImport("advapi32.dll")]
            public static extern IntPtr OpenSCManager(string lpMachineName, string lpSCDB, int scParameter);
            [DllImport("Advapi32.dll")]
            public static extern IntPtr CreateService(IntPtr SC_HANDLE, string lpSvcName, string lpDisplayName,
            int dwDesiredAccess, int dwServiceType, int dwStartType, int dwErrorControl, string lpPathName,
            string lpLoadOrderGroup, int lpdwTagId, string lpDependencies, string lpServiceStartName, string lpPassword);
            [DllImport("advapi32.dll")]
            public static extern void CloseServiceHandle(IntPtr SCHANDLE);
            [DllImport("advapi32.dll")]
            public static extern int StartService(IntPtr SVHANDLE, int dwNumServiceArgs, string lpServiceArgVectors);
            [DllImport("advapi32.dll", SetLastError = true)]
            public static extern IntPtr OpenService(IntPtr SCHANDLE, string lpSvcName, int dwNumServiceArgs);
            [DllImport("advapi32.dll")]
            public static extern int DeleteService(IntPtr SVHANDLE);
            [DllImport("kernel32.dll")]
            public static extern int GetLastError();

            public bool InstallService(string svcPath, string svcName, string svcDispName, bool allowInteract)
            {
                try
                {
                    IntPtr sc_handle = OpenSCManager(null, null, SC_MANAGER_CREATE_SERVICE);
                    if (sc_handle.ToInt32() != 0)
                    {
                        Console.WriteLine("{0} is being installed as a Manual Start Type and will not be started by the installer.", svcName);
                        Console.WriteLine("To modify this, use the Services applet in the Control Panel.");
                        Console.WriteLine("To start stop or restart the service, you can run the .exe with args -start -stop or -restart.");
                        IntPtr sv_handle = CreateService(sc_handle, svcName, svcDispName, SERVICE_ALL_ACCESS, (allowInteract ? SERVICE_INTERACTIVE_PROCESS :  0) | SERVICE_WIN32_OWN_PROCESS, m_StartMode, SERVICE_ERROR_NORMAL, svcPath, null, 0, null, null, null);
                        CloseServiceHandle(sc_handle);

                        if (sv_handle.ToInt32() == 0)
                            return false;
                        else
                            return true;
                    }
                    else
                    {
                        int errorNum = GetLastError();
                        string error;
                        if (errorNum == 5)
                            error = "ERROR_ACCESS_DENIED";
                        else if (errorNum == 1065)
                            error = "ERROR_DATABASE_DOES_NOT_EXIST";
                        else if (errorNum == 87)
                            error = "ERROR_INVALID_PARAMETER";
                        else
                            error = errorNum.ToString();

                        Console.WriteLine("SCM not opened successfully. Error: {0}", error);

                    }
                    return false;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            public bool UnInstallService(string svcName)
            {
                int GENERIC_WRITE = 0x40000000;
                IntPtr sc_hndl = OpenSCManager(null, null, GENERIC_WRITE);
                if (sc_hndl.ToInt32() != 0)
                {
                    int DELETE = 0x10000;
                    IntPtr svc_hndl = OpenService(sc_hndl, svcName, DELETE);
                    Console.WriteLine(svc_hndl.ToInt32());
                    if (svc_hndl.ToInt32() != 0)
                    {
                        int i = DeleteService(svc_hndl);
                        if (i != 0)
                        {
                            CloseServiceHandle(sc_hndl);
                            return true;
                        }
                        else
                        {
                            CloseServiceHandle(sc_hndl);
                            return false;
                        }
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
        }
    }
}