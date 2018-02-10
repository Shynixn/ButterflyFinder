using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using OverLayApplicationSearch.Contract.Business.Entity;

namespace OverLayApplicationSearch.WpfApp.Models
{
    internal class ListItemViewer : BaseViewModel, IViewModel
    {
        public string Header { get; set; }
        public string Text { get; set; }
        public ImageSource Icon { get; set; }
        public object Cache { get; set; }
    }
}
