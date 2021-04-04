namespace WinTaskKiller.Logic.Contract
{
    public interface IAutoStartService
    {
        /// <summary>
        /// Registers the calling Assembly for autostart.
        /// </summary>
        void RegisterProgramForAutoStart();
    }
}
