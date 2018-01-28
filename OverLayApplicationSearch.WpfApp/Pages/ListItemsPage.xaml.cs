using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OverLayApplicationSearch.Contract.Business.Entity;
using OverLayApplicationSearch.Contract.Persistence.Controller;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Contract.Persistence.Enumeration;
using OverLayApplicationSearch.Logic;
using OverLayApplicationSearch.WpfApp.Contracts;
using OverLayApplicationSearch.WpfApp.Models;
using MessageBox = System.Windows.MessageBox;
using UserControl = System.Windows.Controls.UserControl;

namespace OverLayApplicationSearch.WpfApp.Pages
{
    /// <summary>
    /// Interaction logic for ListItemsPage.xaml
    /// </summary>
    public partial class ListItemsPage : UserControl
    {
        public ListItemsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Reloads the tasks context for database access. Gets called on page load.
        /// </summary>
        /// <param name="sender">page</param>
        /// <param name="e">e</param>
        private void onWindowLoaded(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                this.RefreshTasks();
                if (this.ParentWindow != null && (this.ParentWindow.Scanner != null || ((ControlWindow)ParentWindow).PrepareForScan))
                {
                    ((ControlWindow)ParentWindow).PrepareForScan = false;
                    this.ScanMode = true;
                }
            }   
        }

        /// <summary>
        /// Cancels the task currently managed in the window.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">Scanning</param>
        private void buttonListItemsCreateNewTask(object sender, RoutedEventArgs e)
        {
            if ((string) this.buttonCreateTaskNext.Content == "CANCEL")
            {
                this.taskProgressMesage.Text = "Cancelling scanning...";
                ParentWindow.Scanner?.Cancel();
            }
            else
            {
                ParentWindow.Next();
            }
        }

        public bool ScanMode
        {
            get { return false; }
            set
            {
                this.listBoxTasks.IsEnabled = !value;
                foreach (var item in this.listBoxTasks.Items)
                {       
                    TaskListboxItemModel model = item as TaskListboxItemModel;
                    if (model != null)
                        model.ButtonEnabled = !value;
                }
                if (value)
                {
                    this.taskProgressMesage.Text = "Preparing filesystem...";
                    this.buttonCreateTaskNext.Content = "CANCEL";
                    this.buttonCreateTaskNext.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e51400"));
                }
                else
                {
                    this.RefreshTasks();
                    this.taskProgressMesage.Text = "Finished";
                    this.taskProgressBar.Value = 100;
                    this.buttonCreateTaskNext.Content = "CREATE NEW TASK";
                    this.buttonCreateTaskNext.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#1ba1e2"));
                }
            }
        }

        public long AmountOfItemsScanned
        {
            set
            {
                if (this.ParentWindow.Scanner != null)
                {
                    IScanner scanner = this.ParentWindow.Scanner;
                    taskProgressMesage.Text = this.ParentWindow.RunningTask.Path + "             " + value + "/" +
                                              scanner.MaxCount;
                    double result = (double)value / (double)scanner.MaxCount;
                    taskProgressBar.Value = result * 100;
                }    
            }
        }

        public async void Delete(IConfiguredTask task)
        {
            if (ScanMode == false)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this task? All associated indexes will be deleted and the files remain untouched.", "Delete",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    await DeleteTask(task);
                    this.RefreshTasks();
                }
            }
        }

        private async void RefreshTasks()
        {
            var tasks = await LoadTasksFromDatabaseDatabase();
            this.listBoxTasks.Items.Clear();          
            foreach (var configuredTask in tasks)
            {
                this.listBoxTasks.Items.Add(new TaskListboxItemModel(this, configuredTask));
            }
        }

        private Task DeleteTask(IConfiguredTask task)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var controller = Factory.CreateConfiguredTaskController())
                {
                    controller.Remove(task);
                }
            });
        }

        private Task<List<IConfiguredTask>> LoadTasksFromDatabaseDatabase()
        {
            return Task<List<IConfiguredTask>>.Factory.StartNew(() =>
            {
                using (var controller = Factory.CreateConfiguredTaskController())
                {
                    return controller.GetAll();
                }
            });
        }

        internal PageableWindow ParentWindow
        {
            get
            {
                if (Window.GetWindow(this) is PageableWindow == false)
                {
                    return null;
                }
                return (PageableWindow) Window.GetWindow(this);
            }
        }
    }
}