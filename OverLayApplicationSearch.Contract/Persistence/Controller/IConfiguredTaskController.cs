using OverLayApplicationSearch.Contract.Persistence.Entity;

namespace OverLayApplicationSearch.Contract.Persistence.Controller
{
    public interface IConfiguredTaskController : IDatabaseController<IConfiguredTask>
    {
        /// <summary>
        /// Creates a new configured task
        /// </summary>
        /// <returns>configured task</returns>
        IConfiguredTask Create();
    }
}