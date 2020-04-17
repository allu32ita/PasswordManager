using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PasswordManager.Models.Options;

namespace PasswordManager.Models.Services.Infrastructure
{

    public class SqLiteDatabaseAccessor : IDatabaseAccessor
    {
        private readonly ILogger<SqLiteDatabaseAccessor> log;

        public IOptionsMonitor<ConnectionStringsOptions> OpzioniDiConnessione { get; }
        public SqLiteDatabaseAccessor(ILogger<SqLiteDatabaseAccessor> log, IOptionsMonitor<ConnectionStringsOptions> OpzioniDiConnessione)
        {
            this.log = log;
            this.OpzioniDiConnessione = OpzioniDiConnessione;
        }


        public async Task<DataSet> QueryAsync(FormattableString formatquery)
        {
            log.LogInformation(formatquery.Format, formatquery.GetArguments());
            
            var queryArg = formatquery.GetArguments();
            var sqliteParam = new List<SqliteParameter>();
            for (int i = 0; i < queryArg.Length; i++)
            {
                if (queryArg[i] is Sql)
                {
                    continue;
                }
                var param = new SqliteParameter(i.ToString(), queryArg[i]);
                sqliteParam.Add(param);
                queryArg[i] = "@" + i;
            }
            string query = formatquery.ToString();

            var ConnectionString = OpzioniDiConnessione.CurrentValue.Default;
            using (var conn = new SqliteConnection(ConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.AddRange(sqliteParam);
                    using (var reader = await cmd.ExecuteReaderAsync())
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