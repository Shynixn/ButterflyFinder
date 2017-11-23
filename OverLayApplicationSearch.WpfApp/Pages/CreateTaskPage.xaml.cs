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
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Logic;

namespace OverLayApplicationSearch.WpfApp.Pages
{
    /// <summary>
    /// Interaction logic for CreateTaskPage.xaml
    /// </summary>
    public partial class CreateTaskPage : UserControl
    {
        private string cache;

        public CreateTaskPage()
        {
            InitializeComponent();
        }

        public string TaskDescription => this.TaskTimeSchedule + " - " + TaskPath;

        public string TaskPath { get; set; }

        public string TaskTimeSchedule
        {
            get { return cache; }
            set
            {
                this.cache = value;
                this.textboxCreateTaskInfo.Text = TaskDescription;
            }
        }

        private void buttonCreateTaskBack_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Back();
        }

        private async void buttonCreateTaskNext_Click(object sender, RoutedEventArgs e)
        {
            IConfiguredTask task = await StoreNewTask();
            ParentWindow.Scan(task);
            ((ControlWindow)ParentWindow).PrepareForScan = true;
            ParentWindow.Next();
        }

        private Task<IConfiguredTask> StoreNewTask()
        {
            return Task.Factory.StartNew(() =>
            {
                using (var controller = Factory.CreateConfiguredTaskController())
                {
                    IConfiguredTask task = controller.Create();
                    task.Path = TaskPath;
                    task.TimeScheduled = TaskTimeSchedule;
                    controller.Store(task);
                    return task;
                }
            });
        }

        public PageableWindow ParentWindow
        {
            get
            {
                if (Window.GetWindow(this) is PageableWindow == false)
                {
                    throw new ArgumentException("Cannot use control in a non PageableWindow!");
                }
                return (PageableWindow) Window.GetWindow(this);
            }
        }

        private void onWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.textboxCreateTaskInfo.Text = TaskDescription;
        }
    }
}