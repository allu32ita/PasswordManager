using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace PasswordManager.Models.Services.Infrastructure
{
    public class SqLiteDatabaseAccessor : IDatabaseAccessor
    {
        public async Task<DataSet> QueryAsync(FormattableString formatquery)
        {    

            var queryArg = formatquery.GetArguments();
            var sqliteParam = new List<SqliteParameter>();
            for (int i = 0; i < queryArg.Length; i++)
            {
                var param = new SqliteParameter(i.ToString(), queryArg[i]);
                sqliteParam.Add(param);
                queryArg[i]= "@" + i;
            }
            string query = formatquery.ToString();


            using(var conn = new SqliteConnection("Data Source=Data/DbPassword.db"))
            {
                await conn.OpenAsync();
                using(var cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.AddRange(sqliteParam);
                    using(var reader = await cmd.ExecuteReaderAsync())
                    {
                        var dset = new DataSet();
                        dset.EnforceConstraints = false;
                        do
                        {
                            var dtable = new DataTable();
                            dset.Tables.Add(dtable);
                            dtable.Load(reader);
                        } while (!reader.IsClosed);
                        return dset;
                    }
                }                   
            }            
        }




    }
}