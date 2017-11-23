namespace OverLayApplicationSearch.Contract.Persistence.Entity
{
    public interface IPersistenceable
    {
        /// <summary>
        /// Returns the id of the object
        /// </summary>
        long Id { get; }
    }
}