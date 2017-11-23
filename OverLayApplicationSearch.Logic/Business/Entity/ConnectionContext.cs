using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace OverLayApplicationSearch.Logic.Business.Entity
{
    internal class ConnectionContext
    {
        internal string fileName;
        private bool inMemory;

        /// <summary>
        /// Initialize
        /// </summary>
        private ConnectionContext()
        {
        }

        /// <summary>
        /// Returns a new connection from the connection context.
        /// </summary>
        public SQLiteConnection Connection
        {
            get
            {
                if (inMemory)
                {
                    var connection = new SQLiteConnection($"Data Source=:memory:;Version=3;");
                    connection.Open();
                    return connection;
                }
                else
                {
                    if (!File.Exists(fileName))
                    {
                        SQLiteConnection.CreateFile(fileName);
                    }
                    var connection = new SQLiteConnection($"Data Source={fileName};Version=3;");
                    connection.Open();
                    return connection;
                }
            }
        }

        /// <summary>
        /// Creates a backup of the context database
        /// </summary>
        public void BackUp(string backUpFilePath)
        {
            while (File.Exists(backUpFilePath))
            {
                File.Delete(backUpFilePath);
            }
            SQLiteConnection.CreateFile(backUpFilePath);
            var connection = new SQLiteConnection($"Data Source={backUpFilePath};Version=3;");
            connection.Open();

            Connection.BackupDatabase(connection, "main", "main", -1, null, 0);
            connection.Dispose();
        }

        /// <summary>
        ///  Executes an update to the database like Create or Update
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <param name="connection">opened connection</param>
        /// <param name="parameters">preparedParameters for the statement</param>
        /// <returns>returnValue</returns>
        public int ExecuteStoredUpdated(string fileName, SQLiteConnection connection, object[] parameters = null)
        {
            return this.executeUpdate(ReadFileFromResource(fileName), connection, parameters);
        }

        /// <summary>
        /// Executes an update to the database like Create or Update
        /// </summary>
        /// <param name="sql">statement</param>
        /// <param name="connection">opened connection</param>
        /// <param name="parameters">preparedParameters for the statement</param>
        /// <returns>returnValue</returns>
        public int executeUpdate(string sql, SQLiteConnection connection, object[] parameters = null)
        {
            using (var preparedStatement = new SQLiteCommand(sql, connection))
            {
                SetParameters(preparedStatement, parameters);
                return preparedStatement.ExecuteNonQuery();
            }
        }

        /// <summary>
        ///  Executes an insert to the database 
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <param name="connection">opened connection</param>
        /// <param name="parameters">preparedParameters for the statement</param>
        /// <returns>id of the inserted dat</returns>
        public long ExecuteStoredInsert(string fileName, SQLiteConnection connection, object[] parameters = null)
        {
            return this.executeInsert(ReadFileFromResource(fileName), connection, parameters);
        }

        /// <summary>
        /// Executes an insert to the database 
        /// </summary>
        /// <param name="sql">statement</param>
        /// <param name="connection">opened connection</param>
        /// <param name="parameters">preparedParameters for the statement</param>
        /// <returns>id of the inserted data</returns>
        public long executeInsert(string sql, SQLiteConnection connection, object[] parameters = null)
        {
            using (var preparedStatement = new SQLiteCommand(sql, connection))
            {
                using (var action = connection.BeginTransaction())
                {
                    SetParameters(preparedStatement, parameters);
                    preparedStatement.ExecuteNonQuery();
                    var rowId = connection.LastInsertRowId;
                    action.Commit();
                    return rowId;
                }
            }
        }

        /// <summary>
        /// Creates a preparedStatement and does not execute it. The instance calling this method has to call executeQuery
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <param name="connection">opened connection</param>
        /// <param name="parameters">preparedParameters for the statement</param>
        /// <returns>preparedStatement</returns>
        public SQLiteCommand ExecuteStoredQuery(string fileName, SQLiteConnection connection,
            object[] parameters = null)
        {
            return this.executeQuery(ReadFileFromResource(fileName), connection, parameters);
        }

        /// <summary>
        /// Creates a preparedStatement and does not execute it. The instance calling this method has to call executeQuery
        /// </summary>
        /// <param name="sql">statement</param>
        /// <param name="connection">opened connection</param>
        /// <param name="parameters">preparedParameters for the statement</param>
        /// <returns>preparedStatement</returns>
        public SQLiteCommand executeQuery(string sql, SQLiteConnection connection, object[] parameters = null)
        {
            var preparedStatement = new SQLiteCommand(sql, connection);
            SetParameters(preparedStatement, parameters);
            return preparedStatement;
        }

        /// <summary>
        /// Parameters
        /// </summary>
        /// <param name="preparedStatement">parameters</param>
        /// <param name="parameters">parameters</param>
        private static void SetParameters(SQLiteCommand preparedStatement, object[] parameters)
        {
            if (parameters == null) return;
            for (var i = 0; i < parameters.Length; i++)
            {
                preparedStatement.Parameters.AddWithValue("param" + i, parameters[i]);
            }
        }

        /// <summary>
        /// Returns the fileText from the resource
        /// </summary>
        /// <param name="resourceName">resourceName</param>
        /// <returns>fileText</returns>
        private string ReadFileFromResource(string resourceName)
        {
            var myAssembly = Assembly.GetExecutingAssembly();
            using (var stream =
                myAssembly.GetManifestResourceStream(resourceName))
            {
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Creates a new inMemory ConnectionContext which opens a new connection to the inmemory database
        /// </summary>
        /// <returns>connectionContext</returns>
        public static ConnectionContext CreateInMemoryConnectionContext()
        {
            var context = new ConnectionContext {inMemory = true};
            return context;
        }

        /// <summary>
        /// Creates a new ConnectionContext which opens a new connection to a fileDatabase every time Connection is called.
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <returns>connectionContext</returns>
        public static ConnectionContext CreateConnectionContext(string fileName)
        {
            var context = new ConnectionContext {fileName = fileName};
            return context;
        }
    }
}