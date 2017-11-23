using System;
using System.Collections.Generic;
using System.Data.SQLite;
using OverLayApplicationSearch.Contract.Persistence.Controller;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Logic.Business.Entity;
using OverLayApplicationSearch.Logic.Persistence.Entity;

namespace OverLayApplicationSearch.Logic.Persistence.Controller
{
    internal class ConfiguredTaskRepository : DatabaseRepository<IConfiguredTask>, IConfiguredTaskController
    {
        private SQLiteConnection connection;
        private ConnectionContext connectionContext;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="connectionContext"></param>
        public ConfiguredTaskRepository(ConnectionContext connectionContext)
        {
            this.connectionContext = connectionContext;
            this.connection = connectionContext.Connection;
        }


        /// <summary>
        /// Returns the amount of items in the repository
        /// </summary>
        public override int Count
        {
            get
            {
                using (var command =
                    connectionContext.ExecuteStoredQuery(
                        "OverLayApplicationSearch.Logic.Resource.SQL.TimeTask.count.sql", connection))
                {
                    return Convert.ToInt32(command.ExecuteScalar());
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the item of the given id
        /// </summary>
        /// <param name="id">id of the item</param>
        /// <returns>item</returns>
        public override IConfiguredTask GetById(long id)
        {
            using (var command = connectionContext.ExecuteStoredQuery(
                "OverLayApplicationSearch.Logic.Resource.SQL.TimeTask.selectbyid.sql",
                connection, new object[] {id}))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        return From(reader);
                    }
                }
            }
            return null;
        }

        /// <inheritdoc />
        /// <summary>
        /// Creates a new configured task
        /// </summary>
        /// <returns>configured task</returns>
        public IConfiguredTask Create()
        {
            return new ConfiguredTask();
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns if the given item hasgot a id
        /// </summary>
        /// <param name="item">item</param>
        /// <returns>has Id</returns>
        protected override bool HasId(IConfiguredTask item)
        {
            return item.Id != 0;
        }

        /// <inheritdoc />
        /// <summary>
        /// Selects all items from the database into a list
        /// </summary>
        /// <returns>resultList</returns>
        protected override List<IConfiguredTask> Select()
        {
            var list = new List<IConfiguredTask>();
            using (var command =
                connectionContext.ExecuteStoredQuery("OverLayApplicationSearch.Logic.Resource.SQL.TimeTask.select.sql",
                    connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(From(reader));
                    }
                }
            }
            return list;
        }

        /// <inheritdoc />
        /// <summary>
        /// Updates the item inside of the database
        /// </summary>
        /// <param name="item">item</param>
        protected override void Update(IConfiguredTask item)
        {
            connectionContext.ExecuteStoredUpdated("OverLayApplicationSearch.Logic.Resource.SQL.TimeTask.update.sql",
                connection,
                new object[] {item.Path, item.TimeScheduled, item.LastTimeIndexed.ToString("yyyy-MM-dd HH:mm:ss"), item.Id});
        }

        /// <inheritdoc />
        /// <summary>
        /// Delets the item inside of the database
        /// </summary>
        /// <param name="item">item</param>
        protected override void Delete(IConfiguredTask item)
        {
            connectionContext.ExecuteStoredUpdated("OverLayApplicationSearch.Logic.Resource.SQL.TimeTask.delete.sql",
                connection,
                new object[] {item.Id});
        }

        /// <inheritdoc />
        /// <summary>
        /// Inserts the item into the database and sets the id
        /// </summary>
        /// <param name="item">item</param>
        protected override void Insert(IConfiguredTask item)
        {
            var id = connectionContext.ExecuteStoredInsert(
                "OverLayApplicationSearch.Logic.Resource.SQL.TimeTask.insert.sql", connection,
                new object[] {item.Path, item.TimeScheduled, item.LastTimeIndexed.ToString("yyyy-MM-dd HH:mm:ss")});
            ((ConfiguredTask) item).Id = id;
        }

        /// <inheritdoc />
        /// <summary>
        /// Generates an object from the given resultSet
        /// </summary>
        /// <param name="resultSet">resultSet</param>
        /// <returns>object</returns>
        protected override IConfiguredTask From(object resultSet)
        {
            var reader = resultSet as SQLiteDataReader;
            if (reader == null)
                return null;
            var task = new ConfiguredTask();
            task.Id = (long) reader["id"];
            task.Path = (string) reader["filepath"];
            task.TimeScheduled = (string) reader["timeschedule"];
            task.LastTimeIndexed = (DateTime) reader["lastindexedtime"];
            return task;
        }

        /// <inheritdoc />
        /// <summary>
        /// Dispose pattern implementation
        /// </summary>
        /// <param name="disposing">dispose</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                connection.Dispose();
                connectionContext = null;
                connection = null;
                connectionContext = null;
            }
            base.Dispose(disposing);
        }
    }
}