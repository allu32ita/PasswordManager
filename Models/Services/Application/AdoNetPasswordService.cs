using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PasswordManager.Models.Services.Infrastructure;
using PasswordManager.Models.ViewModels;
using PasswordManager.Models.Options;
using Microsoft.Extensions.Logging;
using PasswordManager.Models.Exceptions;

namespace PasswordManager.Models.Services.Application
{
    public class AdoNetPasswordService : IPasswordService
    {
        private readonly ILogger<AdoNetPasswordService> log;
        private readonly IDatabaseAccessor db;
        private readonly IOptionsMonitor<PasswordsOptions> OpzioniPassword;

        public AdoNetPasswordService(ILogger<AdoNetPasswordService> log, IDatabaseAccessor db, IOptionsMonitor<PasswordsOptions> OpzioniPassword)
        {
            this.OpzioniPassword = OpzioniPassword;
            this.log = log;
            this.db = db;
        }

        public async Task<PasswordDetailViewModel> GetPasswordAsync(string id)
        {
            log.LogInformation("password {id} requested", id);
            FormattableString query = $"SELECT * FROM Passwords WHERE Id = {id}";
            DataSet dset = await db.QueryAsync(query);
            var dtable = dset.Tables[0];
            if (dtable.Rows.Count != 1)
            {
                log.LogWarning("password {id} requested", id);
                throw new PasswordNotFoundException(Convert.ToInt32(id));
            }
            var PassRow = dtable.Rows[0];
            PasswordDetailViewModel PassDetailViewModel = PasswordDetailViewModel.FromDataRow(PassRow);
            return PassDetailViewModel;
        }


        public async Task<List<PasswordViewModel>> GetPasswordsAsync(string search, int page, string orderby, bool ascending)
        {
            FormattableString query = $"SELECT * FROM Passwords where Descrizione LIKE {"%" + search + "%"}";
            DataSet dset = await db.QueryAsync(query);
            var dtable = dset.Tables[0];
            var listaPass = new List<PasswordViewModel>();
            foreach(DataRow passRow in dtable.Rows)
            {
                PasswordViewModel pass = PasswordViewModel.FromDataRow(passRow);
                listaPass.Add(pass);
            }
            return listaPass;
        }
    }
}