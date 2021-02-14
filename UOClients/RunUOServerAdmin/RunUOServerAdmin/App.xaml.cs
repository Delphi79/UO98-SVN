using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace RunUOServerAdmin
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }

        static string programName = Assembly.GetExecutingAssembly().GetName().Name;
        //static string companyName = Assembly.GetExecutingAssembly().GetName().

        public static string LoadRegistryValue(string key)
        {
            RegistryKey regKey = SettingsRegKey;
            return regKey.GetValue(key, null) as string;
        }

        public static void SaveRegistryValue(string key, string value)
        {
            RegistryKey regKey = SettingsRegKey;
            regKey.SetValue(key, value);
        }

        static RegistryKey SettingsRegKey
        {
            get
            {
                RegistryKey regkey = Registry.CurrentUser.CreateSubKey("Software");
                if (!string.IsNullOrWhiteSpace(CompanyName))
                    regkey = regkey.CreateSubKey(CompanyName);
                return regkey.CreateSubKey(programName);
            }
        }

        private static string _CompanyName = null;
        public static string CompanyName
        {
            get
            {
                if (_CompanyName != null) return _CompanyName;
                object[] attributes = Assembly.GetCallingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false); return (_CompanyName = attributes.Length == 0 ? string.Empty : ((AssemblyCompanyAttribute)attributes[0]).Company);
            }
        }

        public static BitmapSource ConvertGDI_To_WPF(System.Drawing.Bitmap bm)
        {
            BitmapSource bms = null;
            if (bm != null)
            {
                IntPtr h_bm = bm.GetHbitmap();
                bms = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(h_bm, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            return bms;
        }


    }
}
