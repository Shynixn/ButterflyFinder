using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using ListItemsPage = OverLayApplicationSearch.WpfApp.UserControls.ControlWindowPages.ListItemsPage;

namespace OverLayApplicationSearch.WpfApp.Models
{
    internal class TaskListboxItemModel : BaseViewModel
    {
        private ICommand buttonSyncCommand;
        private ICommand buttonDeleteCommand;
        private readonly ListItemsPage window;
        private readonly IConfiguredTask task;
        private bool buttonEnabled;

        public TaskListboxItemModel(ListItemsPage window, IConfiguredTask task)
        {
            this.window = window;
            this.task = task;
            this.ButtonEnabled = true;
        }

        public long Id => this.task.Id;

        public string Path => this.task.Path;

        public string TimeSchedule => this.task.TimeScheduled;

        public string LastTime
        {
            get
            {
                if (this.task.LastTimeIndexed == default(DateTime))
                {
                    return "Never";
                }
                else
                {
                    return this.task.LastTimeIndexed.ToString(CultureInfo.InvariantCulture);;
                }
            }
        }

        public bool ButtonEnabled
        {
            get { return buttonEnabled; }
            set
            {
                this.buttonEnabled = value;
               OnPropertyChanged(nameof(ButtonEnabled));               
            }
        }

        public ICommand ButtonSyncClick
        {
            get { return RelayCommand.CreateCommand(ref buttonSyncCommand, p => { window.ParentWindow.Scan(task); }); }
        }

        public ICommand ButtonDeleteClick
        {
            get { return RelayCommand.CreateCommand(ref buttonDeleteCommand, p => { window.Delete(task); }); }
        }
    }
}