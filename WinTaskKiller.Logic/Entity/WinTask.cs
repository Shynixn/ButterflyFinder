using System.Windows.Media;

namespace WinTaskKiller.Logic.Entity
{
    public class WinTask
    {
        public int ProcessId { get; set; }
        public string ExecutablePath { get; set; }
        public string ExecutableName { get; set; }

        public ImageSource Icon { get; set; }
    }
}
