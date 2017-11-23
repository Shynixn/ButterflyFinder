using System;
using System.Collections.Generic;
using OverLayApplicationSearch.Contract.Persistence.Controller;

namespace OverLayApplicationSearch.Logic.Persistence.Controller
{
    internal abstract class DatabaseRepository<T> : IDatabaseController<T>
    {
        /// <inheritdoc />
        /// <summary>
        /// Stores an item into the repository
        /// </summary>
        /// <param name="item"></param>
        public void Store(T item)
        {
            if (this.HasId(item))
            {
                this.Update(item);
            }
            else
            {
                this.Insert(item);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Removes an item from the repository
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            this.Delete(item);
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the amount of items in the repository
        /// </summary>
        public abstract int Count { get; }

        /// <inheritdoc />
        /// <summary>
        /// Returns the item of the given id
        /// </summary>
        /// <param name="id">id of the item</param>
        /// <returns>item</returns>
        public abstract T GetById(long id);

        /// <inheritdoc />
        /// <summary>
        /// Returns all items in the repository
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll()
        {
            return this.Select();
        }

        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        /// <param name="disposing">dispose</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Returns if the given item hasgot a id
        /// </summary>
        /// <param name="item">item</param>
        /// <returns>has Id</returns>
        protected abstract bool HasId(T item);

        /// <summary>
        /// Selects all items from the database into a list
        /// </summary>
        /// <returns>resultList</returns>
        protected abstract List<T> Select();

        /// <summary>
        /// Updates the item inside of the database
        /// </summary>
        /// <param name="item">item</param>
        protected abstract void Update(T item);

        /// <summary>
        /// Delets the item inside of the database
        /// </summary>
        /// <param name="item">item</param>
        protected abstract void Delete(T item);

        /// <summary>
        /// Inserts the item into the database and sets the id
        /// </summary>
        /// <param name="item">item</param>
        protected abstract void Insert(T item);

        /// <summary>
        /// Generates an object from the given resultSet
        /// </summary>
        /// <param name="resultSet">resultSet</param>
        /// <returns>object</returns>
        protected abstract T From(object resultSet);
    }
}