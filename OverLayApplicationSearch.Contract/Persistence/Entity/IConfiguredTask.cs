using System;

namespace OverLayApplicationSearch.Contract.Persistence.Entity
{
    public interface IConfiguredTask : IPersistenceable
    {
        /// <summary>
        /// Gets or sets the path of the task
        /// </summary>
        string Path { get; set; }
        /// <summary>
        /// Gets or sets the time scheduled of the task
        /// </summary>
        string TimeScheduled { get; set; }
        /// <summary>
        /// Gets or sets the time this task was successfully executed
        /// </summary>
        DateTime LastTimeIndexed { get; set; }
    }
}
