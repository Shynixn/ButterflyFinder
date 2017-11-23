using OverLayApplicationSearch.Contract.Persistence.Entity;

namespace OverLayApplicationSearch.Logic.Persistence.Entity
{
    internal class PersistenceObject : IPersistenceable
    {
        /// <inheritdoc />
        /// <summary>
        /// Returns the id of the object
        /// </summary>
        public long Id { get; internal set; }
    }
}