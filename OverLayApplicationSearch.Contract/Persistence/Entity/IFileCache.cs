using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverLayApplicationSearch.Contract.Persistence.Entity
{
    public interface IFileCache : IPersistenceable
    {
        /// <summary>
        /// FileName of the cache
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Gets the parentId
        /// </summary>
        long? ParentId { get; set; }

        /// <summary>
        /// Returns the TimeTaskId
        /// </summary>
        long TimeTaskId { get; set; }
    }
}