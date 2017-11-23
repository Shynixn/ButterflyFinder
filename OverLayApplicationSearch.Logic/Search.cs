using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OverLayApplicationSearch.Logic.Business.Entity;

namespace OverLayApplicationSearch.Logic
{
    public class Search : IDisposable
    {
        private string SELECT_COMMAND = "WITH LINK(ID, TEXT, REF_ID) AS (" +
                                        "    SELECT ID, TEXT, REF_ID FROM SHY_FILECACHES WHERE LOWER(TEXT) LIKE('%$KEY$%')" +
                                        "    UNION ALL" +
                                        "    SELECT SHY_FILECACHES.ID, IFNULL(SHY_FILECACHES.TEXT || '/', '') || LINK.TEXT, SHY_FILECACHES.REF_ID" +
                                        "    FROM LINK INNER JOIN SHY_FILECACHES ON LINK.REF_ID = SHY_FILECACHES.ID" +
                                        ")" +
                                        "SELECT TEXT FROM LINK WHERE TEXT LIKE ('%:%') group by TEXT order by CASE WHEN TEXT Like('%exe') THEN 1 ELSE 2 END, TEXT ASC LIMIT $MAXAMOUNT$"
            ;

        private ConnectionContext ConnectionContext { get; set; }

        internal Search(ConnectionContext connectionContext)
        {
            ConnectionContext = connectionContext;
        }

        public List<string> findResults(string text, int maxAmount)
        {
            var result = new List<string>();
            Console.WriteLine("TEXT: " + text);
            using (var command = ConnectionContext.executeQuery(SELECT_COMMAND.Replace("$MAXAMOUNT$", maxAmount.ToString()).Replace("$KEY$", text),ConnectionContext.Connection, null))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(reader["text"] as string);
                    }
                }
            }
            return result;
        }

        public void Dispose()
        {
            ConnectionContext = null;
        }
    }
}