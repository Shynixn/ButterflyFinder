using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OverLayApplicationSearch.Logic.Lib
{
    public static class WindowsFileExtension
    {
        [DllImport("kernel32.dll")]
        static extern uint GetTempPath(uint nBufferLength, [Out] StringBuilder lpBuffer);

        [DllImport("shell32.dll", EntryPoint = "FindExecutable")]
        static extern long FindExecutableA(string lpFile, string lpDirectory, [Out] StringBuilder lpResult);

        /// <summary>
        /// Returns the default windows executable path from the given file path.
        /// </summary>
        /// <param name="path">path of any file with extension</param>
        /// <returns>executeable path</returns>
        public static string GetDefaultExecutablePathFromFile(string path)
        {
            string tempFileName = path;
            StringBuilder sb = new StringBuilder(1024);
            long ret = FindExecutableA(tempFileName, string.Empty, sb);
            if (ret >= 32) return sb.ToString();
            else return string.Empty;
        }
    }
}
