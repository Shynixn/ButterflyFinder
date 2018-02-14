using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using OverLayApplicationSearch.Contract.Business.Entity;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Logic;
using OverLayApplicationSearch.WpfApp.Contracts;
using ListItemsPage = OverLayApplicationSearch.WpfApp.UserControls.ControlWindowPages.ListItemsPage;

namespace OverLayApplicationSearch.WpfApp.UserControls
{
    /// <inheritdoc cref="PageableWindow" />
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
            if (this.currentPage is ControlWindowPages.AddWatchFolderPage)
            {
                SelectPage(new ListItemsPage());
                this.selectedPath = "";
            }
        }

        public void Next()
        {
            if (this.currentPage is ListItemsPage)
            {
                SelectPage(new ControlWindowPages.AddWatchFolderPage() { SelectedFolder = selectedPath });
            }
            else if (this.currentPage is ControlWindowPages.AddWatchFolderPage)
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