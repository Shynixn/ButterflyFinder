using System;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.Win32;
using WinTaskKiller.Logic.Contract;

namespace WinTaskKiller.Logic.Service
{
    public class AutoStartService : IAutoStartService
    {
        /// <summary>
        /// Registers the calling Assembly for autostart.
        /// </summary>
        public void RegisterProgramForAutoStart()
        {
            try
            {
                var registryKey =
                    Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                registryKey.SetValue(Assembly.GetExecutingAssembly().GetName().Name, Application.ExecutablePath);
            }
            catch (Exception)
            {
                // Ignored.
            }
        }
    }
}