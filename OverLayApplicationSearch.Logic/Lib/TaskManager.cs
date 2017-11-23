using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace OverLayApplicationSearch.Logic
{
    public class TaskManager
    {
        public static string[] Tasks
        {
            get
            {
                List<string> tasks = new List<string>();
                var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
                using (var searcher = new ManagementObjectSearcher(wmiQueryString))
                using (var results = searcher.Get())
                {
                    var query = from p in Process.GetProcesses()
                        join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                        select new
                        {
                            Process = p,
                            Path = (string)mo["ExecutablePath"],
                            CommandLine = (string)mo["CommandLine"],
                        };
                    foreach (var item in query)
                    {
                       tasks.Add(item.Path);
                    }
                }
                return tasks.OrderBy(Path.GetFileName).Distinct().ToArray();
            }
        }

        public static void Kill(string path)
        {
            List<string> tasks = new List<string>();
            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                    join mo in results.Cast<ManagementObject>()
                        on p.Id equals (int)(uint)mo["ProcessId"]
                    select new
                    {
                        Process = p,
                        Path = (string)mo["ExecutablePath"],
                        CommandLine = (string)mo["CommandLine"],
                    };
                foreach (var item in query)
                {
                    if (item.Path == path)
                    {
                        item.Process.Kill();
                    }
                }
            }
        }
    }
}