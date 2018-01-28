using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Threading;
using OverLayApplicationSearch.Contract.Business.Service;
using OverLayApplicationSearch.Logic;
using OverLayApplicationSearch.Logic.Business.Service;
using OverLayApplicationSearch.WpfApp.Enumeration;
using OverLayApplicationSearch.WpfApp.Models;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace OverLayApplicationSearch.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool fallback;
        private bool ignoreSwitch;

        private readonly IListService taskKillerService = new TaskKillerService();
        private readonly IGoogleService googleService = new GoogleService();
        private readonly ISearchService searchService = new SearchService(Properties.Resources.folder);
    
        internal KeyboardHook KeyboardHook { get; set; }

        private SearchWindowState CurrentState { get; set; }

        private int TaskBarElement { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Topmost = true;
            KeyboardHook = new KeyboardHook();
            KeyboardHook.KeyPressed += KeyboardHookOnKeyPressed;
            KeyboardHook.RegisterHotKey(ModifierKeys.Control, Keys.K);
            Factory.ReInitializeContext();
            PreviewKeyDown += HandleEsc;

            var dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            messageBox.Visibility = Visibility.Hidden;
            resultListBox.Items.Clear();
            Keyboard.Focus(searchTextBox);
            SetSearchWindowState(SearchWindowState.HIDDEN);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            clockLabel.Content = DateTime.Now.ToString("HH:mm");
        }

        /// <summary>
        /// Handles pressing on the escape button and proceeds the window state.
        /// </summary>
        /// <param name="sender">userInterface</param>
        /// <param name="e">e</param>
        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SetSearchWindowState(SearchWindowState.HIDDEN);
            }
            else if (e.Key == Key.Down && CurrentState == SearchWindowState.SEARCHBOX)
            {
                SetSearchWindowState(SearchWindowState.TASKBAR);       
            }
            else if (e.Key == Key.Up && CurrentState == SearchWindowState.TASKBAR)
            {
                SetSearchWindowState(SearchWindowState.SEARCHBOX);
            }
            else if (e.Key == Key.Enter && CurrentState == SearchWindowState.TASKBAR)
            {
                if (TaskBarElement == 0)
                {
                    SetSearchWindowState(SearchWindowState.TASKKILLER);
                }
            }
        }

        private void KeyboardHookOnKeyPressed(object sender, KeyPressedEventArgs e)
        {
            if (IsVisible == false)
            {
                Show();
                SetSearchWindowState(SearchWindowState.SEARCHBOX);
                Topmost = true;
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


        private async void SetSearchWindowState(SearchWindowState state)
        {
            if (state == SearchWindowState.SEARCHBOX)
            {
                var effect = elementTaskKiller.Effect as DropShadowEffect;
                if (effect != null) effect.Opacity = 0.0;

                forgroundSearchBox.Opacity = 1.0;
                searchTextBox.Visibility = Visibility.Visible;
                taskPanel.Visibility = Visibility.Visible;            
                taskPanel.Opacity = 0.5;
                titlePanel.Opacity = 0.5;
                resultListBox.Opacity = 0.5;
                messageBox.Opacity = 0.5;
                forgroundSearchBox.Visibility = Visibility.Visible;
                searchTextBox.Text = "";
                Activate();
                Keyboard.Focus(searchTextBox);
                searchTextBox.Focus();
            }
            else if (state == SearchWindowState.SEARCHRESULTS)
            {          
                forgroundSearchBox.Opacity = 1.0;
                taskPanel.Visibility = Visibility.Hidden;
                taskPanel.Opacity = 0.5;
                titlePanel.Opacity = 1.0;
                messageBox.Opacity = 1.0;
                resultListBox.Opacity = 1.0;
                searchTextBox.Visibility = Visibility.Visible;
                messageBox.Visibility = Visibility.Hidden;
                forgroundSearchBox.Visibility = Visibility.Hidden;
            }
            else if (state == SearchWindowState.SEARCHINGRESULTS)
            {
                resultListBox.Focus();
                resultListBox.SelectedIndex = 0;
                messageBox.Visibility = Visibility;
                noResultsLabel.Content = "Searching...";
                await searchService.Search(searchTextBox.Text);
                if (searchService.Count == 0)
                {
                    noResultsLabel.Content = "No Results";
                }
                else
                {
                    RenderItemsIntoListBox(searchService);
                    messageBox.Visibility = Visibility.Hidden;
                }
            }
            else if (state == SearchWindowState.TASKBAR)
            {
                messageBox.Opacity = 0.5;
                forgroundSearchBox.Opacity = 0.5;
                taskPanel.Opacity = 1.0;
                searchTextBox.Visibility = Visibility.Hidden;
                TaskBarElement = 0;
                var effect = elementTaskKiller.Effect as DropShadowEffect;
                if (effect != null) effect.Opacity = 1.0;
            }
            else if (state == SearchWindowState.TASKKILLER)
            {
                SetSearchWindowState(SearchWindowState.SEARCHRESULTS);
                fallback = true;
                RenderItemsIntoListBox(taskKillerService);
            }
            else if (state == SearchWindowState.GOOGLESEARCH)
            {
                SetSearchWindowState(SearchWindowState.HIDDEN);
                googleService.Search(searchTextBox.Text.Split(' ')[1]);
            }
            else if (state == SearchWindowState.HIDDEN)
            {
                Hide();
            }
            CurrentState = state;
        }

        private void RenderItemsIntoListBox(IListService service)
        {
            resultListBox.Items.Clear();
            resultListBox.Focus();
            resultListBox.SelectedIndex = 0;
            for (var i = 0; i < service.Count; i++)
            {
                var dataInfo = new ListItemViewer();
                service.InitModel(i, dataInfo);
            }
            resultListBox.UpdateLayout();
            var listBoxItem = (ListBoxItem)resultListBox.ItemContainerGenerator.ContainerFromItem(resultListBox.SelectedItem);
            listBoxItem?.Focus();
        }

        private void ExecuteListItemAction()
        {
            if (CurrentState == SearchWindowState.TASKKILLER)
            {
                ExecuteListItemAction(taskKillerService, true);
            }
        }

        private void ExecuteListItemAction(IListService service, bool reload)
        {
            var dataInfo = resultListBox.SelectedItem as ListItemViewer;
            var index = resultListBox.SelectedIndex;
            if (dataInfo == null) return;
            service.OnAction(index, dataInfo);
            if (reload)
            {
                RenderItemsIntoListBox(service);
            }
        }

        private void OnKeyDownHander(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;
            if (searchTextBox.Text.Trim().StartsWith("kill", StringComparison.InvariantCultureIgnoreCase))
            {
                SetSearchWindowState(SearchWindowState.TASKKILLER);
            }
            else if (searchTextBox.Text.Trim().StartsWith("gle", StringComparison.InvariantCultureIgnoreCase))
            {
                SetSearchWindowState(SearchWindowState.GOOGLESEARCH);
            }
            else
            {
                SetSearchWindowState(SearchWindowState.SEARCHINGRESULTS);
            }
        }

        private void onListBoxKeyEnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return) return;
            if (ignoreSwitch)
            {
                ignoreSwitch = false;
                return;
            }    
            ExecuteListItemAction();
        }

        private void onListBoxDoubleClickItem(object sender, MouseButtonEventArgs e)
        {
            ExecuteListItemAction();
        }

        private void onMouseClickOnTaskKillerLabel(object sender, MouseButtonEventArgs e)
        {
            SetSearchWindowState(SearchWindowState.TASKKILLER);
        }

        #region DesignEffectRegion
        private void onMouseLeaveTaskBarArea(object sender, MouseEventArgs e)
        {
            if (CurrentState == SearchWindowState.TASKBAR) return;
            taskPanel.Opacity = 0.5;
            var effect = elementTaskKiller.Effect as DropShadowEffect;
            if (effect != null) effect.Opacity = 0.0;
        }

        private void onMouseEnterTaskBarArea(object sender, MouseEventArgs e)
        {
            taskPanel.Opacity = 1.0;
            var effect = elementTaskKiller.Effect as DropShadowEffect;
            if (effect != null) effect.Opacity = 1.0;
        }

        #endregion

        private void onMouseDownEventListbox(object sender, MouseButtonEventArgs e)
        {
           
        }
    }
}