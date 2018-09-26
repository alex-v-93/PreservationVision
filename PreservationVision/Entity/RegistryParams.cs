using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace PreservationVision.Entity
{
    internal class RegistryParams
    {
        internal static void ReadValue<T>(ref T field, string name)
        {
            Check();
            var obj = Registry.CurrentUser.OpenSubKey(@"Software\PreservationVision", true)?.GetValue(name); //
            if (obj != null)
                field = (T)obj;
        }

        internal static bool GetAutoRunValue()
        {
            var obj = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true)
                ?.GetValue(@"PreservationVision");
            return obj != null;
        }

        internal static void SetAutoRunValue(bool value)
        {
            if (value)
            {
                var path = Assembly.GetEntryAssembly().Location;
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true)
                    ?.SetValue(@"PreservationVision",
                    path);
            }
            else
                Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true)?.DeleteValue(@"PreservationVision");
        }

        public static void SaveValue<T>(T value, string nameValue)
        {
            Check();
            
            Registry.CurrentUser.OpenSubKey(@"Software\PreservationVision", true)?.SetValue(nameValue, value);
        }

        static void Check()
        {
            Registry.CurrentUser.CreateSubKey(@"Software\PreservationVision");
        }
    }
}
