using System;
using System.Data.SQLite;
using System.IO;
using OverLayApplicationSearch.Contract.Business.Entity;
using OverLayApplicationSearch.Contract.Persistence.Controller;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Logic.Persistence.Controller;
using OverLayApplicationSearch.Logic.Persistence.Entity;

namespace OverLayApplicationSearch.Logic.Business.Entity
{
    internal class Scanner : IScanner
    {
        public static long MAX = 1000000;
        private bool cancelled;

        /// <summary>
        /// Gets called when the amount of items scanned changed
        /// </summary>
        public event ScannerDelegate AmountScannedChangeEvent;

        private FileCacheRepository controller;

        private IConfiguredTask Task { get; set; }

        public ConnectionContext ConnectionContext { get; set; }

        /// <summary>
        /// Initialize
        /// </summary>
        public Scanner(IConfiguredTask configuredTask, ConnectionContext connectionContext)
        {
            this.Task = configuredTask;
            var repository = new FileCacheRepository(connectionContext, ".AM");
            connectionContext.executeUpdate("ATTACH DATABASE ':memory:' AS AM", repository.connection);
            connectionContext.ExecuteStoredUpdated(
                "OverLayApplicationSearch.Logic.Resource.SQL.FileCache.AM.create.sql", repository.connection);
            connectionContext.executeUpdate("DELETE FROM SHY_FILECACHE WHERE timetask_id = @param0;",
                repository.connection, new object[] {configuredTask.Id});
            connectionContext.executeInsert("INSERT INTO AM.SQLITE_SEQUENCE SELECT * FROM SQLITE_SEQUENCE ",
                repository.connection);
            try
            {
                connectionContext.executeUpdate("DROP INDEX index_search", repository.connection);
            }
            catch (Exception)
            {
            }
            this.controller = repository;
            this.ConnectionContext = connectionContext;
        }

        /// <inheritdoc />
        /// <summary>
        /// Scans and indexes the given folder and subfiles
        /// </summary>
        /// <param name="fileName">path</param>
        public void Scan(string fileName)
        {
            Count = 0;
            if (fileName.EndsWith("/"))
            {
                fileName = fileName.Substring(0, fileName.Length - 1);
            }
            string[] parts = fileName.Split('\\');
            long? parentId = null;
            for (int i = 0; i < parts.Length; i++)
            {
                var fileCache = new FileCache() {FileName = parts[i], ParentId = parentId, TimeTaskId = Task.Id};
                this.controller.Store(fileCache);
                parentId = fileCache.Id;
            }
            Count++;
            this.RecFileSearch(fileName, parentId);
            AmountScannedChangeEvent?.Invoke(Count);
        }

        /// <summary>
        /// Returns the amount of files and subfiles in the given folder
        /// </summary>
        /// <param name="fileName">path</param>
        /// <returns>amount</returns>
        public long GetAmountOfFiles(string fileName)
        {
            MaxCount = FindAmountOfFiles(0, fileName) + 1;
            return MaxCount;
        }

        /// <summary>
        /// Returns the amount of items indexed by Scan
        /// </summary>
        public long Count { get; private set; }

        /// <summary>
        /// Returns the amount of items counted by last getAmountOfFiles
        /// </summary>
        public long MaxCount { get; private set; }

        /// <summary>
        /// Cancels the last scan
        /// </summary>
        public void Cancel()
        {
            this.cancelled = true;
        }

        /// <summary>
        /// returns if the task was cancelled
        /// </summary>
        public bool IsCancelled => this.cancelled;

        /// <summary>
        /// Executes a recursive file search in sub files and sub directory
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="root">rootFolder</param>
        /// <param name="parentId">parentId</param>
        /// <param name="organizeId">organizeId</param>
        private void RecFileSearch(string root, long? parentId)
        {
            try
            {
                if (cancelled)
                {
                    return;
                }
                foreach (var file in Directory.GetFiles(root))
                {
                    ExecuteRecFileSearch(file, parentId);
                }
                foreach (var file in Directory.GetDirectories(root))
                {
                    ExecuteRecFileSearch(file, parentId);
                }
            }
            catch (Exception)
            {

            }        
        }

        /// <summary>
        /// Inserts an entry to the database
        /// </summary>
        /// <param name="connection">connection</param>
        /// <param name="file">file</param>
        /// <param name="parentId">parentId</param>
        /// <param name="organizeId">organizeId</param>
        private void ExecuteRecFileSearch(string file, long? parentId)
        {
            try
            {
                if (cancelled)
                    return;
                if (Count > MAX)
                    return;
                if (file.Contains("$")) return;
                Count++;
                AmountScannedChangeEvent?.Invoke(Count);
                var fileCahe = new FileCache()
                {
                    FileName = Path.GetFileName(file),
                    ParentId = parentId,
                    TimeTaskId = Task.Id
                };
                controller.Store(fileCahe);
                if (Directory.Exists(file))
                {
                    this.RecFileSearch(file, fileCahe.Id);
                }
            }
            catch (Exception)
            {
            }
        }

        private long FindAmountOfFiles(long amount, string root)
        {
            try
            {
                if (amount > MAX)
                    return amount;
                if (cancelled)
                    return amount;
                foreach (var file in Directory.GetFiles(root))
                {
                    if (cancelled)
                        return amount;
                    if (!file.Contains("$"))
                    {
                        amount++;
                        if (amount > MAX)
                            return amount;
                    }
                }
                foreach (var file in Directory.GetDirectories(root))
                {
                    if (cancelled)
                        return amount;
                    if (!file.Contains("$"))
                    {
                        amount++;
                        if (amount > MAX)
                            return amount;
                        amount = FindAmountOfFiles(amount, file);
                    }
                }
            }
            catch (Exception)
            {
            }          
            return amount;
        }

        /// <summary>
        /// Dispose implementation
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ConnectionContext.executeUpdate("INSERT INTO SHY_FILECACHE SELECT * FROM AM.SHY_FILECACHE",
                    controller.connection);
                this.ConnectionContext.executeUpdate("DETACH 'AM'", controller.connection);
                this.ConnectionContext.executeUpdate("CREATE INDEX index_search ON SHY_FILECACHE(name)",
                    controller.connection);
                controller.Dispose();
                controller = null;
            }
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}