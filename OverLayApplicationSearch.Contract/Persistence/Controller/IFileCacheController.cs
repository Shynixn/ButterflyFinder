using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.Contract.Persistence.Entity;

namespace OverLayApplicationSearch.Contract.Persistence.Controller
{
    public interface IFileCacheController : IDatabaseController<IFileCache>
    {
        /// <summary>
        /// Creates a new fileCahce
        /// </summary>
        /// <returns>create</returns>
        IFileCache Create();

        /// <summary>
        /// Deletes all entries of the given task
        /// </summary>
        /// <param name="task">task</param>
        void DeleteByTask(IConfiguredTask task);

        /// <summary>
        /// Returns the items matching the searchText
        /// </summary>
        /// <param name="searchText">searchText</param>
        /// <param name="maxAmount">maxAmount of returned items</param>
        /// <returns>items</returns>
        List<string> Search(string searchText, int maxAmount);
    }
}