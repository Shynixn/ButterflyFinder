using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OverLayApplicationSearch.Contract.Business.Entity
{
    public interface IViewModel
    {
        string Header { get; set; }
        string Text { get; set; }
        ImageSource Icon { get; set; }
        object Cache { get; set; }
    }
}