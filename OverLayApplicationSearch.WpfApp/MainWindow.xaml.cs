using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Logic;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Path = System.IO.Path;

namespace OverLayApplicationSearch.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool fallback;
        private bool processMode;

        internal KeyboardHook KeyboardHook { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            KeyboardHook = new KeyboardHook();
            KeyboardHook.KeyPressed += KeyboardHookOnKeyPressed;
            KeyboardHook.RegisterHotKey(ModifierKeys.Control, Keys.K);
            Factory.ReInitializeContext();
        }

        private void KeyboardHookOnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (this.IsVisible == false)
            {
                Show();
                ChangeMode(0);
                this.Topmost = true;
            }
            else if (fallback)
            {
                ChangeMode(0);
                fallback = false;
            }
            else
            {
                ChangeMode(2);
            }
        }


        public class DataInfo
        {
            public string Path { get; set; }
            public string Name { get; set; }

            public ImageSource ImageSource { get; set; }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.messageBox.Visibility = Visibility.Hidden;
            this.resultListBox.Items.Clear();
            Keyboard.Focus(this.searchTextBox);
            ChangeMode(2);
        }

        private void ChangeMode(int modeType)
        {
            if (modeType == 0)
            {
                this.titlePanel.Opacity = 0.5;
                this.resultListBox.Opacity = 0.5;
                this.messageBox.Opacity = 0.5;
                this.forgroundSearchBox.Visibility = Visibility.Visible;
                this.searchTextBox.Text = "";
                this.Activate();
                Keyboard.Focus(this.searchTextBox);
                this.searchTextBox.Focus();
            }
            else if (modeType == 1)
            {
                this.titlePanel.Opacity = 1.0;
                this.messageBox.Opacity = 1.0;
                this.resultListBox.Opacity = 1.0;
                this.messageBox.Visibility = Visibility.Hidden;
                this.forgroundSearchBox.Visibility = Visibility.Hidden;
            }
            else if (modeType == 2)
            {
                this.Hide();
            }
        }

        private void ListProcessesInListBox()
        {
            resultListBox.Items.Clear();  
            this.resultListBox.Focus();
            this.resultListBox.SelectedIndex = 0;
            foreach (var line in TaskManager.Tasks)
            {
                try
                {
                    Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(line);
                    this.resultListBox.Items.Add(new DataInfo()
                    {
                        Name = Path.GetFileName(line),
                        Path = line,
                        ImageSource = ToImageSource(icon)
                    });
                }
                catch (Exception exception)
                {
                }
            }
            SetFocusToListBox();
        }

        private async void OnKeyDownHander(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                resultListBox.Items.Clear();
                ChangeMode(1);
                fallback = true;
                if (this.searchTextBox.Text.Trim().StartsWith("kill", StringComparison.InvariantCultureIgnoreCase))
                {
                    processMode = true;
                    ListProcessesInListBox();
                }
                else
                {
                    processMode = false;
                    this.resultListBox.Focus();
                    this.resultListBox.SelectedIndex = 0;
                    this.messageBox.Visibility = Visibility;
                    this.noResultsLabel.Content = "Searching...";

                    List<string> results = await Search(this.searchTextBox.Text);            
                    if (results.Count == 0)
                    {                 
                        this.noResultsLabel.Content = "No Results";
                    }
                    else
                    {
                        foreach (var result in results)
                        {
                            RenderListBoxItem(result);
                        }
                        this.messageBox.Visibility = Visibility.Hidden;
                    }                
                }
            }
        }

        private void SetFocusToListBox()
        {
            resultListBox.UpdateLayout();
            var listBoxItem =
                (ListBoxItem) resultListBox.ItemContainerGenerator.ContainerFromItem(resultListBox.SelectedItem);
            if (listBoxItem != null)
                listBoxItem.Focus();
        }

        private void RenderListBoxItem(string line)
        {
            try
            {
                Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(line);
                this.resultListBox.Items.Add(new DataInfo()
                {
                    Name = Path.GetFileName(line),
                    Path = line,
                    ImageSource = ToImageSource(icon)
                });
            }
            catch (Exception exception)
            {
                Icon icon = OverLayApplicationSearch.WpfApp.Properties.Resources.folder;
                this.resultListBox.Items.Add(new DataInfo()
                {
                    Name = Path.GetFileName(line),
                    Path = line,
                    ImageSource = ToImageSource(icon)
                });
            }
        }

        public static ImageSource ToImageSource(Icon icon)
        {
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return imageSource;
        }

        private void onListBoxKeyEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (processMode)
                {
                    KillListItem();
                }
                else
                {
                    OpenSelectedListItemInExplorer();
                }      
            }
        }

        private void KillListItem()
        {
            DataInfo dataInfo = this.resultListBox.SelectedItem as DataInfo;
            if (dataInfo != null)
            {
                TaskManager.Kill(dataInfo.Path);
                this.resultListBox.SelectedIndex = -1;
                ListProcessesInListBox();
            }
        }

        private void OpenSelectedListItemInExplorer()
        {
            DataInfo dataInfo = this.resultListBox.SelectedItem as DataInfo;
            if (dataInfo != null)
            {
                OpenPathInExplorer(dataInfo.Path);
                this.resultListBox.SelectedIndex = -1;
                ChangeMode(1);
                ChangeMode(2);
            }
        }

        private void StartProcess(string path)
        {
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.Start();
        }

        private void OpenPathInExplorer(string path)
        {
            try
            {
                StartProcess(path);
            }
            catch (Exception e)
            {
                try
                {
                    path = new FileInfo(path).Directory.FullName;
                    StartProcess(path);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }
        }

        private void onListBoxDoubleClickItem(object sender, MouseButtonEventArgs e)
        {
            if (processMode)
            {
                KillListItem();
            }
            else
            {
                OpenSelectedListItemInExplorer();
            }
        }

        private Task<List<string>> Search(string searchText)
        {
            return Task<List<string>>.Factory.StartNew(() =>
            {            
                using (var controller = Factory.CreateFilecacheController())
                {
                    return controller.Search(searchText, 100);
                }
            });
        }
    }
}