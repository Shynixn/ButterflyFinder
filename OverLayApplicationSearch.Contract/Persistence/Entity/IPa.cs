using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OverLayApplicationSearch.Contract.Persistence.Entity
{
    public interface IPa : IPersistenceable
    {
        SecureString PassWord { get; set; }
    }
}
