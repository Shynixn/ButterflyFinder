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
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Text;
using OverLayApplicationSearch.Logic.Lib;

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
            this.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(HandleEsc);

            var dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            this.clockLabel.Content = DateTime.Now.ToString("HH:mm");
        }

        /// <summary>
        /// Handles pressing on the escape button and proceeds the window to state 2.
        /// </summary>
        /// <param name="sender">userInterface</param>
        /// <param name="e">e</param>
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.SetSearchWindowState(SearchWindowState.HIDDEN);
            }
        }

        private void KeyboardHookOnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (this.IsVisible == false)
            {
                Show();
                SetSearchWindowState(SearchWindowState.SEARCHBOX);
                this.Topmost = true;
            }
            else if (fallback)
            {
                SetSearchWindowState(SearchWindowState.SEARCHBOX);
                fallback = false;
            }
            else
            {
                SetSearchWindowState(SearchWindowState.HIDDEN);
            }
        }


        public class DataInfo
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public ImageSource ImageSource { get; set; }       
            public string ExecuteAble { get; set; }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.messageBox.Visibility = Visibility.Hidden;
            this.resultListBox.Items.Clear();
            Keyboard.Focus(this.searchTextBox);
            SetSearchWindowState(SearchWindowState.HIDDEN);
        }

        private void SetSearchWindowState(SearchWindowState state)
        {
            if (state == SearchWindowState.SEARCHBOX)
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
            else if (state == SearchWindowState.SEARCHRESULTS)
            {
                this.titlePanel.Opacity = 1.0;
                this.messageBox.Opacity = 1.0;
                this.resultListBox.Opacity = 1.0;
                this.messageBox.Visibility = Visibility.Hidden;
                this.forgroundSearchBox.Visibility = Visibility.Hidden;
            }
            else if (state == SearchWindowState.HIDDEN)
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

        public void launchBrowser(string url)
        {



            string browserName = "iexplore.exe";
            using (RegistryKey userChoiceKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\Shell\Associations\UrlAssociations\http\UserChoice"))
            {
                if (userChoiceKey != null)
                {
                    object progIdValue = userChoiceKey.GetValue("Progid");
                    if (progIdValue != null)
                    {
                        if (progIdValue.ToString().ToLower().Contains("chrome"))
                            browserName = "chrome.exe";
                        else if (progIdValue.ToString().ToLower().Contains("firefox"))
                            browserName = "firefox.exe";
                        else if (progIdValue.ToString().ToLower().Contains("safari"))
                            browserName = "safari.exe";
                        else if (progIdValue.ToString().ToLower().Contains("opera"))
                            browserName = "opera.exe";
                    }
                }
            }

            Process.Start(new ProcessStartInfo(browserName, "\"? " + url  + "\""));
        }



        private async void OnKeyDownHander(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                resultListBox.Items.Clear();
                SetSearchWindowState(SearchWindowState.SEARCHRESULTS);
                fallback = true;
                if (this.searchTextBox.Text.Trim().StartsWith("kill", StringComparison.InvariantCultureIgnoreCase))
                {
                    processMode = true;
                    ListProcessesInListBox();
                }
                else if(this.searchTextBox.Text.Trim().StartsWith("gle", StringComparison.InvariantCultureIgnoreCase))
                {
                    processMode = false;
                    SetSearchWindowState(SearchWindowState.HIDDEN);
                    try
                    {
                        launchBrowser(this.searchTextBox.Text.Split(' ')[1]);
                    }
                    catch(Exception ex)
                    {
                    }                  
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
                if(line.EndsWith(".exe"))
                {
                    this.addListBoxItemFromExePath(line, line);
                }
                else
                {
                    this.addListBoxItemFromExePath(line, WindowsFileExtension.GetDefaultExecutablePathFromFile(line));
                }
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

        private void addListBoxItemFromExePath(string path, string exePath)
        {
            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(exePath);
            this.resultListBox.Items.Add(new DataInfo()
            {
                Name = Path.GetFileName(path),
                Path = path,
                ImageSource = ToImageSource(icon),
                ExecuteAble = exePath
            });
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
                    OpenSelectedListItemInExplorer(true);
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

        private void OpenSelectedListItemInExplorer(bool hideWindow)
        {
            DataInfo dataInfo = this.resultListBox.SelectedItem as DataInfo;
            if (dataInfo != null)
            {
                OpenPathInExplorer(dataInfo.Path, dataInfo.ExecuteAble);
                if (hideWindow)
                {
                    this.resultListBox.SelectedIndex = -1;
                    SetSearchWindowState(SearchWindowState.SEARCHBOX);
                    SetSearchWindowState(SearchWindowState.HIDDEN);
                }
            }
        }

        private void StartProcess(string executable, string file)
        {
            if(executable == null || !executable.EndsWith(".exe"))
            {
                Process process = new Process();
                process.StartInfo.FileName = file;
                process.Start();
            }
            else
            {
                Process process = new Process();
                process.StartInfo.Arguments = Path.GetFileName(file);
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.WorkingDirectory = Path.GetDirectoryName(file);
                process.StartInfo.FileName = executable;
                process.StartInfo.Verb = "OPEN";          
                process.Start();
            }   
        }

        private void OpenPathInExplorer(string path, string executable)
        {
            try
            {
                StartProcess(executable, path);
            }
            catch (Exception e)
            {
                try
                {
                    path = new FileInfo(path).Directory.FullName;
                    StartProcess(null,path);
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
                OpenSelectedListItemInExplorer(true);
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

        private void onMouseDownEventListbox(object sender, MouseButtonEventArgs e)
        {
            if(e.ButtonState == e.MiddleButton)
            {
                if (processMode)
                {
                    KillListItem();
                }
                else
                {
                    this.Topmost = false;
                    OpenSelectedListItemInExplorer(false);
                }
            }          
        }
    }
}