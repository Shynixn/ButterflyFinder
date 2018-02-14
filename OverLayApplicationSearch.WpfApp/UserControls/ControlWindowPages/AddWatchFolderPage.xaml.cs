using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Contract.Persistence.Enumeration;
using OverLayApplicationSearch.Logic;
using OverLayApplicationSearch.WpfApp.Contracts;

namespace OverLayApplicationSearch.WpfApp.UserControls.ControlWindowPages
{
    /// <summary>
    /// Interaction logic for AddWatchFolderPage.xaml
    /// </summary>
    public partial class AddWatchFolderPage : UserControl
    {
        public AddWatchFolderPage()
        {
            InitializeComponent();
        }

        private void buttonAddFolderFolderSelect_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                this.textBoxAddFolderFolderSelect.Text = dialog.FileName;
            }
        }

        public string SelectedFolder
        {
            get { return this.textBoxAddFolderFolderSelect.Text; }
            set { this.textBoxAddFolderFolderSelect.Text = value; }
        }

        internal PageableWindow ParentWindow
        {
            get
            {
                ControlWindow control = Window.GetWindow(this) as ControlWindow;
                if (control is PageableWindow == false)
                {
                    throw new ArgumentException("Cannot use control in a non PageableWindow!");
                }
                return (PageableWindow) Window.GetWindow(this);
            }
        }

        private void onTextChanged(object sender, TextChangedEventArgs e)
        {
            this.labelAddFolderMessage.Content = "";
        }

        private void buttonAddFolderBack_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Back();
        }

        private Task<bool> DuplicateFolderTask(string text)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var controller = Factory.CreateConfiguredTaskController())
                {
                    return controller.GetAll().SingleOrDefault(p =>
                               p.Path.Equals(text,
                                   StringComparison.InvariantCultureIgnoreCase)) != null;
                }
            });
        }

        private string ChosenFolder { get; set; }

        private Task<IConfiguredTask> StoreNewTask()
        {
            return Task.Factory.StartNew(() =>
            {
                using (var controller = Factory.CreateConfiguredTaskController())
                {
                    IConfiguredTask task = controller.Create();
                    task.Path = ChosenFolder;
                    task.TimeScheduled = TimeSchedule.NEVER;
                    controller.Store(task);
                    return task;
                }
            });
        }

        private async void buttonCreateTask_Click(object sender, RoutedEventArgs e)
        {
            string text = (string) this.textBoxAddFolderFolderSelect.Text;
            bool duplicate = await DuplicateFolderTask(text);
            if (duplicate)
            {
                this.labelAddFolderMessage.Content = "There is already a configured task with this path!";
                return;
            }
            else if (String.IsNullOrEmpty(SelectedFolder))
            {
                this.labelAddFolderMessage.Content = "Please select a valid folder or drive!";
                return;
            }
            else
            {
                this.labelAddFolderMessage.Content = "";
            }
            ChosenFolder = this.SelectedFolder;
            IConfiguredTask task = await StoreNewTask();
            ParentWindow.Scan(task);
            ((ControlWindow) ParentWindow).PrepareForScan = true;
            ParentWindow.Next();
        }
    }
}