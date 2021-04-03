using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using WinTaskKiller.Logic.Contract;
using WinTaskKiller.Logic.Entity;

namespace WinTaskKiller.Logic.Service
{
    public class WinTaskService : IWinTaskService
    {
        private IIconService iconService;

        public WinTaskService(IIconService iconService)
        {
            this.iconService = iconService;
        }

        /// <summary>
        /// Gets all current windows tasks running on the system.
        /// </summary>
        /// <returns><see cref="Task{TResult}"/></returns>
        public async Task<List<WinTask>> GetAll()
        {
            var result = await Task.Run(async () =>
            {
                var tasks = new List<WinTask>();
                var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
                using (var searcher = new ManagementObjectSearcher(wmiQueryString))
                using (var results = searcher.Get())
                {
                    var query = from p in Process.GetProcesses()
                        join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int) (uint) mo["ProcessId"]
                        select new
                        {
                            Process = p,
                            Path = (string) mo["ExecutablePath"],
                            CommandLine = (string) mo["CommandLine"],
                        };

                    var innerQuery = query.ToList();
                    innerQuery.RemoveAll(e => e.Path == null);

                    foreach (var item in innerQuery)
                    {
                        tasks.Add(new WinTask
                        {
                            ProcessId = item.Process.Id,
                            ExecutablePath = item.Path,
                            ExecutableName = Path.GetFileName(item.Path),
                        });
                    }

                    tasks.RemoveAll(e => e.ExecutablePath == null);
                }

                var internalTasks = tasks.GroupBy(x => x.ExecutablePath).Select(x => x.First()).ToList();
                return internalTasks.OrderBy(e => e.ExecutableName).ToList();
            });

            foreach (var item in result)
            {
                item.Icon = iconService.GetImageSourceFromFilePath(item.ExecutablePath);
            }

            return result;
        }

        /// <summary>
        /// Kills the given <paramref name="winTask"/>.
        /// </summary>
        /// <param name="winTask"><see cref="WinTask"/> to be killed.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        public Task Kill(WinTask winTask)
        {
            return Task.Run(async () =>
            {
                for (var i = 0; i < 2; i++)
                {
                    var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
                    using (var searcher = new ManagementObjectSearcher(wmiQueryString))
                    using (var results = searcher.Get())
                    {
                        var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                                on p.Id equals (int) (uint) mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string) mo["ExecutablePath"],
                                CommandLine = (string) mo["CommandLine"],
                            };
                        foreach (var item in query)
                        {
                            if (item.Path == winTask.ExecutablePath)
                            {
                                item.Process.Kill();
                            }
                        }
                    }

                    await Task.Delay(1000);
                }
            });
        }
    }
}