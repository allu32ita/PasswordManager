using System.Data;
using Microsoft.Data.Sqlite;

namespace PasswordManager.Models.Services.Infrastructure
{
    public class SqLiteDatabaseAccessor : IDatabaseAccessor
    {
        public DataSet Query(string query)
        {            
            using(var conn = new SqliteConnection("Data Source=Data/DbPassword.db"))
            {
                conn.Open();
                using(var cmd = new SqliteCommand(query, conn))
                {
                    using(var reader = cmd.ExecuteReader())
                    {
                        var dset = new DataSet();
                        var dtable = new DataTable();
                        dset.Tables.Add(dtable);
                        dtable.Load(reader);
                        return dset;
                    }
                }                   
            }            
        }




    }
}