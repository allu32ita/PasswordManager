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
        public async Task<int> CommandAsync(FormattableString par_Formatcommand)
        {
            try
            { 
                using SqliteConnection conn = await GetOpenedConnection();
                using SqliteCommand cmd     = GetCommand(par_Formatcommand, conn);
                int var_NumRigheUpd = await cmd.ExecuteNonQueryAsync();
                return var_NumRigheUpd;
            }
            catch (System.Exception exc)
            {
                throw new ConstraintException($"Eccezzione su CommandAsync, comando: {par_Formatcommand}", exc);
            }
        }
        public async Task<T> QueryScalarAsync<T>(FormattableString par_Formatcommand)
        {
            try
            {
                using SqliteConnection conn = await GetOpenedConnection();
                using SqliteCommand cmd     = GetCommand(par_Formatcommand, conn);
                object var_Risultato = await cmd.ExecuteScalarAsync();
                return (T)Convert.ChangeType(var_Risultato, typeof(T));
            }
            catch (System.Exception exc)
            {
                throw new ConstraintException($"Eccezzione su CommandAsync, comando: {par_Formatcommand}", exc);
            }
        }

        public async Task<DataSet> QueryAsync(FormattableString par_Formatquery)
        {
            log.LogInformation(par_Formatquery.Format, par_Formatquery.GetArguments());     
            using SqliteConnection conn = await GetOpenedConnection();
            using SqliteCommand cmd     = GetCommand(par_Formatquery, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            var dset = new DataSet();
            do
            {
                var dtable = new DataTable();
                dset.Tables.Add(dtable);
                dtable.Load(reader);
            } while (!reader.IsClosed);
            return dset;
        }

        private static SqliteCommand GetCommand(FormattableString par_Formatquery, SqliteConnection conn)
        {
            var queryArg = par_Formatquery.GetArguments();
            var sqliteParam = new List<SqliteParameter>();
            for (int i = 0; i < queryArg.Length; i++)
            {
                if (queryArg[i] is Sql)
                {
                    continue;
                }
                SqliteParameter param;
                if (queryArg[i] == null)
                {
                    param = new SqliteParameter(i.ToString(), DBNull.Value);
                }
                else
                {
                    param = new SqliteParameter(i.ToString(), queryArg[i]);
                }
                sqliteParam.Add(param);
                queryArg[i] = "@" + i;
            }
            string query = par_Formatquery.ToString();
            var cmd = new SqliteCommand(query, conn);
            cmd.Parameters.AddRange(sqliteParam);
            return cmd;
        }

        private async Task<SqliteConnection> GetOpenedConnection()
        {
            var conn = new SqliteConnection(OpzioniDiConnessione.CurrentValue.Default);
            await conn.OpenAsync();
            return conn;
        }

        

        
    }
}