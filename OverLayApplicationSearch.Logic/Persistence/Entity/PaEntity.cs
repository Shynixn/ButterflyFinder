using OverLayApplicationSearch.Contract.Persistence.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace OverLayApplicationSearch.Logic.Persistence.Entity
{
    internal class PaEntity : PersistenceObject, IPa
    {
        public SecureString PassWord
        {
            get;set;
        }
    }
}
