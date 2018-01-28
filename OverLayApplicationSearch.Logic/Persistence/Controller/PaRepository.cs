using OverLayApplicationSearch.Contract.Persistence.Controller;
using OverLayApplicationSearch.Contract.Persistence.Entity;
using OverLayApplicationSearch.Logic.Business.Entity;
using OverLayApplicationSearch.Logic.Lib;
using OverLayApplicationSearch.Logic.Persistence.Entity;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace OverLayApplicationSearch.Logic.Persistence.Controller
{
    internal class PaRepository : DatabaseRepository<IPa>, IPasswordController
    {
        private static Random random = new Random();
        private ConnectionContext connectionContext;
        private SecureString mainkey;


        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="connectionContext"></param>
        public PaRepository(SecureString mainKey, ConnectionContext connectionContext)
        {
            this.mainkey = mainKey;
            this.connectionContext = connectionContext;
        }       

        /// <inheritdoc cref="" />
        /// <summary>
        /// Returns the amount of items in the repository
        /// </summary>
        public override int Count
        {
            get
            {
                using (var connection = connectionContext.Connection)
                {
                    using (var command = connectionContext.ExecuteStoredQuery($"OverLayApplicationSearch.Logic.Resource.SQL.Pass.count.sql",connection))
                    {
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }          
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Returns the item of the given id
        /// </summary>
        /// <param name="id">id of the item</param>
        /// <returns>item</returns>
        public override IPa GetById(long id)
        {
            using (var connection = connectionContext.Connection)
            {
                using (var command = connectionContext.ExecuteStoredQuery($"OverLayApplicationSearch.Logic.Resource.SQL.Pass.selectbyid.sql",
                connection, new object[] { id }))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return From(reader);
                        }
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
        protected override bool HasId(IPa item)
        {
            return item.Id != 0;
        }

        /// <summary>
        /// Selects all items from the database into a list
        /// </summary>
        /// <returns>resultList</returns>
        protected override List<IPa> Select()
        {     
            using (var connection = connectionContext.Connection)
            {
                List<IPa> list = new List<IPa>();
                using (var command =
              connectionContext.ExecuteStoredQuery($"OverLayApplicationSearch.Logic.Resource.SQL.Pass.select.sql",
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
        }

        /// <summary>
        /// Updates the item inside of the database
        /// </summary>
        /// <param name="item">item</param>
        protected override void Update(IPa item)
        {
            using (var connection = connectionContext.Connection)
            {
                connectionContext.ExecuteStoredUpdated($"OverLayApplicationSearch.Logic.Resource.SQL.Pass.update.sql",
            connection,
            new object[] { item.PassWord, item.Id });
            }
        }

        /// <summary>
        /// Delets the item inside of the database
        /// </summary>
        /// <param name="item">item</param>
        protected override void Delete(IPa item)
        {
            using (var connection = connectionContext.Connection)
            {
                connectionContext.ExecuteStoredUpdated($"OverLayApplicationSearch.Logic.Resource.SQL.Pass.delete.sql",
           connection,
           new object[] { item.Id });
            }         
        }

        /// <summary>
        /// Inserts the item into the database and sets the id
        /// </summary>
        /// <param name="item">item</param>
        protected override void Insert(IPa item)
        {
            using (var connection = connectionContext.Connection)
            {
                var id = connectionContext.ExecuteStoredInsert($"OverLayApplicationSearch.Logic.Resource.SQL.Pass.insert.sql", connection,
           new object[] { new System.Net.NetworkCredential(string.Empty, item.PassWord).Password });
                ((PaEntity)item).Id = id;
            }          
        }

        /// <summary>
        /// Generates an object from the given resultSet
        /// </summary>
        /// <param name="resultSet">resultSet</param>
        /// <returns>object</returns>
        protected override IPa From(object resultSet)
        {
            var reader = resultSet as SQLiteDataReader;
            if (reader == null)
                return null;
            SecureString secure = new SecureString();
            foreach(char a in (string)reader["pass"])
            {
                secure.AppendChar(a);
            }        
            var task = new PaEntity
            {
                Id = (long)reader["id"],
                PassWord = secure
            };
            return task;
        }

        /// <summary>
        /// Creates a new fileCahce
        /// </summary>
        /// <returns>create</returns>
        public IPa Create()
        {
            return new PaEntity();
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
                connectionContext = null;
                connectionContext = null;
            }
            base.Dispose(disposing);
        }

        public SecureString GeneratePassword()
        {
     /*       string data = System.Web.Security.Membership.GeneratePassword(24, random.Next(5, 10));
            return CryptHelper.StringCipher.Encrypt(secure, this.mainkey);*/
            return null;
        }
    }
}
