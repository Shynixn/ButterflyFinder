using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverLayApplicationSearch.Contract.Business.Service
{
    public interface ISearchService : IListService
    {
        /// <summary>
        /// Searches for the given text in the database.
        /// </summary>
        /// <param name="text">text.</param>
       Task Search(string text);
    }
}
