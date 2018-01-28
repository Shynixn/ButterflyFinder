using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using OverLayApplicationSearch.Contract.Business.Entity;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Contract.Persistence.Enumeration;
using OverLayApplicationSearch.Logic;
using OverLayApplicationSearch.WpfApp.Pages;
using System.Security;
using OverLayApplicationSearch.WpfApp.Contracts;

namespace OverLayApplicationSearch.WpfApp
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>
    public partial class ControlWindow : Window, PageableWindow
    {
        private UserControl currentPage;
        private bool destroyed;

        private string selectedPath = "";

        public bool PrepareForScan { get; set; }

        public ControlWindow()
        {
            InitializeComponent();
            Factory.ReInitializeContext();
        }

        private void onWindowLoaded(object sender, RoutedEventArgs e)
        {
            SelectPage(new ListItemsPage());

       /*     using (var controller = Factory.CreatePasswordController())
            {
                SecureString text = controller.GeneratePassword();

                IPa pa = controller.Create();
                pa.PassWord = text;

                controller.Store(pa);



            }

            using (var controller = Factory.CreatePasswordController())
            {
                List<IPa> pas = controller.GetAll();



            }
            */
        }

        public void Destroy()
        {
            destroyed = true;
        }

        private void ControlWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (!destroyed)
            {
                Hide();
                e.Cancel = true;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Starts scanning the configured task. Returns false if someone is currently scanning.-
        /// </summary>
        /// <param name="task"></param>
        /// <returns>scanningAlredy</returns>
        public async void Scan(IConfiguredTask task)
        {
            if (this.RunningTask != null)
                return;
            if (this.currentPage is ListItemsPage)
            {
                ((ListItemsPage) currentPage).ScanMode = true;
            }
            this.RunningTask = task;
            await SyncCurrenTask();
            this.RunningTask = null;
            if (this.currentPage is ListItemsPage)
            {
                ((ListItemsPage)currentPage).ScanMode = false;
            }
        }

        public IScanner Scanner { get; set; }

        /// <summary>
        /// Running task
        /// </summary>
        public IConfiguredTask RunningTask { get; private set; }

        public void Back()
        {
            if (this.currentPage is CreateTaskPage)
            {
                SelectPage(new AddWatchFolderPage() { SelectedFolder = selectedPath });
            }
            else if (this.currentPage is AddWatchFolderPage)
            {
                SelectPage(new ListItemsPage());
                this.selectedPath = "";
            }
        }

        public void Next()
        {
            if (this.currentPage is ListItemsPage)
            {
                SelectPage(new AddWatchFolderPage() { SelectedFolder = selectedPath });
            }
            else if (this.currentPage is AddWatchFolderPage)
            {
                SelectPage(new ListItemsPage());
            }
        }

        private void SelectPage(UserControl userControl)
        {
            this.pageArea.Children.Clear();
            this.pageArea.Children.Add(userControl);
            this.currentPage = userControl;
        }

        private void ScannerOnAmountScannedChangeEvent(long amount)
        {
            if (this.currentPage is ListItemsPage)
            {
                Dispatcher.Invoke(() =>
                {
                    ((ListItemsPage)currentPage).AmountOfItemsScanned = amount;
                });
            }
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

        private Task SyncCurrenTask()
        {
            return Task.Factory.StartNew(() =>
            {
                bool cancelled;
                using (var scanner = Factory.CreateScanner(this.RunningTask))
                {
                    Scanner = scanner;
                    scanner.AmountScannedChangeEvent += ScannerOnAmountScannedChangeEvent;
                    scanner.GetAmountOfFiles(RunningTask.Path);
                    ScannerOnAmountScannedChangeEvent(0);
                    scanner.Scan(RunningTask.Path);
                    cancelled = scanner.IsCancelled;
                }
                if (!cancelled)
                {
                    using (var controller = Factory.CreateConfiguredTaskController())
                    {
                        RunningTask.LastTimeIndexed = DateTime.Now;
                        controller.Store(RunningTask);
                    }
                }
                Scanner = null;
            });        
        }
    }
}