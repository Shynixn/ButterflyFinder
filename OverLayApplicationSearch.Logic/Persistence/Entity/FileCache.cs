using System;
using OverLayApplicationSearch.Contract.Persistence.Entity;

namespace OverLayApplicationSearch.Logic.Persistence.Entity
{
    internal class FileCache : PersistenceObject, IFileCache
    {
        /// <summary>
        /// FileName of the cache
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets the parentId
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// Returns the TimeTaskId
        /// </summary>
        public long TimeTaskId { get; set; }
    }
}