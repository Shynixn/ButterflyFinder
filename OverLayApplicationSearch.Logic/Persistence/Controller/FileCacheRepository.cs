using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.Contract.Persistence.Controller;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Logic.Business.Entity;
using OverLayApplicationSearch.Logic.Persistence.Entity;
using OverLayApplicationSearch.Logic.Lib;
using static OverLayApplicationSearch.Logic.Lib.ShellNotifications;
using System.IO;

namespace OverLayApplicationSearch.Logic.Persistence.Controller
{
    internal class FileCacheRepository : DatabaseRepository<IFileCache>, IFileCacheController
    {
        internal SQLiteConnection connection;
        private ConnectionContext connectionContext;
        private string database;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="connectionContext"></param>
        public FileCacheRepository(ConnectionContext connectionContext, string database = "")
        {
            this.connectionContext = connectionContext;
            this.connection = connectionContext.Connection;
            this.database = database;
        }


        public void OnFileSystemChange(object info)
        {           
            NotifyInfos data = (NotifyInfos) info;            
            if (data.Notification.Equals(SHCNE.SHCNE_RENAMEITEM))
            {
                var result = searchExact(data.Item1, Path.GetFileName(data.Item1));
                long id;
                if(result.TryGetValue(Path.GetFileName(data.Item1), out id))
                {
                    IFileCache cache = this.GetById(id);
                    cache.FileName = Path.GetFileName(data.Item2);
                    this.Update(cache);
                }
            }
            else if (data.Notification.Equals(SHCNE.SHCNE_DELETE))
            {
                var result = searchExact(data.Item1, Path.GetFileName(data.Item1));
                long id;
                if (result.TryGetValue(Path.GetFileName(data.Item1), out id))
                {
                    IFileCache cache = this.GetById(id);
                    this.Delete(cache);
                }
            }
            else if(data.Notification.Equals(SHCNE.SHCNE_CREATE))
            {
                string[] parts = data.Item1.Split('\\');

                var result = searchExact(Path.GetDirectoryName(data.Item1), Path.GetFileName(Path.GetDirectoryName(data.Item1)));

                long id;
                if (result.TryGetValue(Path.GetFileName(Path.GetDirectoryName(data.Item1)), out id))
                {
                    IFileCache directory = this.GetById(id);
                    IFileCache child = this.Create();
                    child.ParentId = directory.Id;
                    child.TimeTaskId = directory.TimeTaskId;
                    child.FileName = Path.GetFileName(data.Item1);
                    this.Store(child);
                }
            }
        }


        /// <inheritdoc cref="" />
        /// <summary>
        /// Returns the amount of items in the repository
        /// </summary>
        public override int Count
        {
            get
            {
                using (var command =
                    connectionContext.ExecuteStoredQuery($"OverLayApplicationSearch.Logic.Resource.SQL.FileCache{database}.count.sql", connection))
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
        public override IFileCache GetById(long id)
        {
            using (var command = connectionContext.ExecuteStoredQuery($"OverLayApplicationSearch.Logic.Resource.SQL.FileCache{database}.selectbyid.sql",
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

        /// <summary>
        /// Returns if the given item hasgot a id
        /// </summary>
        /// <param name="item">item</param>
        /// <returns>has Id</returns>
        protected override bool HasId(IFileCache item)
        {
            return item.Id != 0;
        }

        /// <summary>
        /// Selects all items from the database into a list
        /// </summary>
        /// <returns>resultList</returns>
        protected override List<IFileCache> Select()
        {
            var list = new List<IFileCache>();
            using (var command =
                connectionContext.ExecuteStoredQuery($"OverLayApplicationSearch.Logic.Resource.SQL.FileCache{database}.select.sql",
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

        /// <summary>
        /// Updates the item inside of the database
        /// </summary>
        /// <param name="item">item</param>
        protected override void Update(IFileCache item)
        {
            connectionContext.ExecuteStoredUpdated($"OverLayApplicationSearch.Logic.Resource.SQL.FileCache{database}.update.sql",
                connection,
                new object[] {item.FileName, item.ParentId, item.TimeTaskId, item.Id});
        }

        /// <summary>
        /// Delets the item inside of the database
        /// </summary>
        /// <param name="item">item</param>
        protected override void Delete(IFileCache item)
        {
            connectionContext.ExecuteStoredUpdated($"OverLayApplicationSearch.Logic.Resource.SQL.FileCache{database}.delete.sql",
                connection,
                new object[] {item.Id});
        }

        /// <summary>
        /// Inserts the item into the database and sets the id
        /// </summary>
        /// <param name="item">item</param>
        protected override void Insert(IFileCache item)
        {
            var id = connectionContext.ExecuteStoredInsert($"OverLayApplicationSearch.Logic.Resource.SQL.FileCache{database}.insert.sql", connection,
                new object[] {item.FileName, item.ParentId, item.TimeTaskId});
            ((FileCache) item).Id = id;
        }

        /// <summary>
        /// Generates an object from the given resultSet
        /// </summary>
        /// <param name="resultSet">resultSet</param>
        /// <returns>object</returns>
        protected override IFileCache From(object resultSet)
        {
            var reader = resultSet as SQLiteDataReader;
            if (reader == null)
                return null;
            var task = new FileCache
            {
                Id = (long) reader["id"],
                FileName = (string) reader["name"],
                ParentId = reader["parent_id"] as long? ?? default(long),
                TimeTaskId = (long) reader["timetask_id"]
            };
            return task;
        }

        /// <summary>
        /// Creates a new fileCahce
        /// </summary>
        /// <returns>create</returns>
        public IFileCache Create()
        {
            return new FileCache();
        }

        /// <summary>
        /// Deletes all entries of the given task
        /// </summary>
        /// <param name="task">task</param>
        public void DeleteByTask(IConfiguredTask task)
        {
            connectionContext.ExecuteStoredUpdated($"OverLayApplicationSearch.Logic.Resource.SQL.FileCache{database}.deletebytask.sql",
                connection,
                new object[] {task.Id});
        }

        /// <summary>
        /// Returns the items matching the searchText
        /// </summary>
        /// <param name="searchText">searchText</param>
        /// <param name="maxAmount">maxAmount of returned items</param>
        /// <returns>items</returns>
        public List<string> Search(string searchText, int maxAmount)
        {
            var result = new List<string>();
            using (var command =
                connectionContext.ExecuteStoredQuery($"OverLayApplicationSearch.Logic.Resource.SQL.FileCache{database}.search.sql",
                    connection, new object[] {maxAmount}))
            {
                command.CommandText = command.CommandText.Replace("KEY", searchText);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader["name"] as string);
                    }
                }
            }
            return result;
        }

        private Dictionary<string, long> searchExact(string path, string name)
        {
            Dictionary<string, long> result = new Dictionary<string, long>();
            using (var command =
                connectionContext.ExecuteStoredQuery($"OverLayApplicationSearch.Logic.Resource.SQL.FileCache{database}.exactsearch.sql",
                    connection))
            {
                command.CommandText = command.CommandText.Replace("KEY", name);
                command.CommandText = command.CommandText.Replace("PATH", path.Replace('\\', '/'));
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string cname = reader["name"] as string;
                        string cid = reader["ID"] as string;

                        string[] parts1 = cname.Split('/');
                        string[] parts2 = cid.Split('/');
                        
                        for(int i = 0; i< parts1.Length; i++)
                        {
                            result.Add(parts1[i], Convert.ToInt32(parts2[i]));
                        }
                    }
                    return result;
                }
            }   
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