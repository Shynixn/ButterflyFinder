using System;
using System.Windows.Forms;
using WinTaskKiller.Logic.Entity;
using WinTaskKiller.Logic.Service;

namespace WinTaskKiller.Logic.Contract
{
    public interface IKeyboardHookService
    {
        /// <summary>
        /// Key press event.
        /// </summary>
        event EventHandler<KeyPressedEventArgs> KeyPressed;

        /// <summary>
        /// Registers the given hotKey for event notification.
        /// </summary>
        /// <param name="modifier">Modifier like having to hold SHIFT or Control.</param>
        /// <param name="key">Key to listen to.</param>
        void RegisterHotKey(ModifierKeys modifier, Keys key);
    }
}