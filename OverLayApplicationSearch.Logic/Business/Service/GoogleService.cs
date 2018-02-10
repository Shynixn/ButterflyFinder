using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using OverLayApplicationSearch.Contract.Business.Entity;
using OverLayApplicationSearch.Contract.Business.Service;

namespace OverLayApplicationSearch.Logic.Business.Service
{
    public class GoogleService : IGoogleService
    {
        /// <summary>
        /// Searches for the text via default browser and google.
        /// </summary>
        /// <param name="text">text</param>
        public void Search(string text)
        {
            try
            {
                var browserName = "iexplore.exe";
                using (var userChoiceKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice"))
                {
                    var progIdValue = userChoiceKey?.GetValue("Progid");
                    if (progIdValue != null)
                    {
                        if (progIdValue.ToString().ToLower().Contains("chrome"))
                            browserName = "chrome.exe";
                        else if (progIdValue.ToString().ToLower().Contains("firefox"))
                            browserName = "firefox.exe";
                        else if (progIdValue.ToString().ToLower().Contains("safari"))
                            browserName = "safari.exe";
                        else if (progIdValue.ToString().ToLower().Contains("opera"))
                            browserName = "opera.exe";
                    }
                }

                Process.Start(new ProcessStartInfo(browserName, "\"? " + text + "\""));
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
