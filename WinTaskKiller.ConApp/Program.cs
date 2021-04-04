using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using WinTaskKiller.Logic.Service;

namespace WinTaskKiller.ConApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var winTaskService = new WinTaskService(new IconService());
            var tasks = await winTaskService.GetAll();
            Console.ReadLine();
        }
    }

}
