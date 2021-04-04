using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinTaskKiller.Logic.Entity;

namespace WinTaskKiller.WpfApp.Views
{
    /// <summary>
    /// Interaktionslogik für WinTasksView.xaml
    /// </summary>
    public partial class WinTasksView : UserControl
    {
        public event Action<WinTask> OnTaskKillEvent;


        public WinTasksView()
        {
            InitializeComponent();
        }

        public void FocusListBox()
        {
            Listbox.Focus();
        }

        private void Listbox_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OnTaskKillEvent?.Invoke((WinTask) Listbox.SelectedItem);
        }
    }
}
