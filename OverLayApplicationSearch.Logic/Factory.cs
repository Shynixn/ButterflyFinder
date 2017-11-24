using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.Contract.Business.Entity;
using OverLayApplicationSearch.Contract.Persistence.Controller;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Logic.Business.Entity;
using OverLayApplicationSearch.Logic.Persistence.Controller;

namespace OverLayApplicationSearch.Logic
{
    public static class Factory
    {
        private static ConnectionContext connectionContext;

        /// <summary>
        /// ReInitializes the connectionContext
        /// </summary>
        public static void ReInitializeContext()
        {
            try
            {
                connectionContext = ConnectionContext.CreateConnectionContext(AppDomain.CurrentDomain.BaseDirectory + "storage.sqlite");
                using (var connection = connectionContext.Connection)
                {
                    connectionContext.ExecuteStoredUpdated(
                        "OverLayApplicationSearch.Logic.Resource.SQL.TimeTask.create.sql", connection);
                    connectionContext.ExecuteStoredUpdated(
                        "OverLayApplicationSearch.Logic.Resource.SQL.FileCache.create.sql", connection);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Creates a new scanner with an inmemory database
        /// </summary>
        /// <returns></returns>
        public static IScanner CreateScanner(IConfiguredTask task)
        {
            return new Scanner(task, connectionContext);
        }

        /// <summary>
        /// Creates a new ConfiguredTaskController
        /// </summary>
        /// <returns></returns>
        public static IConfiguredTaskController CreateConfiguredTaskController()
        {
            return new ConfiguredTaskRepository(connectionContext);
        }

        /// <summary>
        /// Creates a new fileCacheController
        /// </summary>
        /// <returns></returns>
        public static IFileCacheController CreateFilecacheController()
        {
            return new FileCacheRepository(connectionContext);
        }
    }
}