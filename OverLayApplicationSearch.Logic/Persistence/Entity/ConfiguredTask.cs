using System;
using OverLayApplicationSearch.Contract.Persistence.Entity;

namespace OverLayApplicationSearch.Logic.Persistence.Entity
{
    internal class ConfiguredTask : PersistenceObject, IConfiguredTask
    {
        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the path of the task
        /// </summary>
        public string Path { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the time scheduled of the task
        /// </summary>
        public string TimeScheduled { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the time this task was successfully executed
        /// </summary>
        public DateTime LastTimeIndexed { get; set; }
    }
}