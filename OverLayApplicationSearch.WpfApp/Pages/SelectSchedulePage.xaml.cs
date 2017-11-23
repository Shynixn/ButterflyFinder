using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using OverLayApplicationSearch.Contract.Persistence.Enumeration;
using OverLayApplicationSearch.Logic;

namespace OverLayApplicationSearch.WpfApp.Pages
{
    /// <summary>
    /// Interaction logic for SelectSchedulePage.xaml
    /// </summary>
    public partial class SelectSchedulePage : UserControl
    {
        public SelectSchedulePage()
        {
            InitializeComponent();
            comboBoxScheduleTimes.SelectedIndex = 1;
            foreach (var timeSchedule in TimeSchedule.TimeSchedules)
            {
                comboBoxScheduleTimes.Items.Add(timeSchedule);
            }
        }

        private void onFormLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
                  
        }

        public string SelectedTimeSchedule
        {
            get
            {
               return (string)this.comboBoxScheduleTimes.SelectedItem;
            }
            set
            {
                for (int i = 0; i < comboBoxScheduleTimes.Items.Count; i++)
                {
                    string d = this.comboBoxScheduleTimes.Items[i] as string;
                    if (d == value)
                    {
                        this.comboBoxScheduleTimes.SelectedIndex = i;
                    }
                }
            }
        }

        private void buttonTimeScheduleNext_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Next();
        }

        private void buttonTimeScheduleBack_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Back();
        }

        public PageableWindow ParentWindow
        {
            get
            {
                if (Window.GetWindow(this) is PageableWindow == false)
                {
                    throw new ArgumentException("Cannot use control in a non PageableWindow!");
                }
                return (PageableWindow)Window.GetWindow(this);
            }
        }
    }
}
