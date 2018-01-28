using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverLayApplicationSearch.Logic.Lib
{
    public static class ProcessExtension
    {
        /// <summary>
        /// Launches a new process for the given fileName with the optional executeable path.
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <param name="executable">executeAble</param>
        public static void LaunchProcess(string fileName, string executable)
        {
            if (executable == null || !executable.EndsWith(".exe"))
            {
                var process = new Process {StartInfo = {FileName = fileName}};
                process.Start();
            }
            else
            {
                var process = new Process();
                process.StartInfo.Arguments = Path.GetFileName(fileName);
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(fileName);
                process.StartInfo.FileName = executable;
                process.StartInfo.Verb = "OPEN";
                process.Start();
            }
        }
    }
}