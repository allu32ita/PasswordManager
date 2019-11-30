using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PasswordManager.Models.Services.Infrastructure;
using PasswordManager.Models.ViewModels;
using PasswordManager.Models.Options;

namespace PasswordManager.Models.Services.Application
{
    public class AdoNetPasswordService : IPasswordService
    {
        private readonly IDatabaseAccessor db;
        private readonly IOptionsMonitor<PasswordsOptions> OpzioniPassword;

        public AdoNetPasswordService(IDatabaseAccessor db, IOptionsMonitor<PasswordsOptions> OpzioniPassword)
        {
            this.OpzioniPassword = OpzioniPassword;
            this.db = db;
        }

        public async Task<PasswordDetailViewModel> GetPasswordAsync(string id)
        {
            FormattableString query = $"SELECT * FROM Passwords WHERE Id = {id}";
            DataSet dset = await db.QueryAsync(query);
            var dtable = dset.Tables[0];
            if (dtable.Rows.Count != 1)
            {
                throw new InvalidOperationException("Id non valido.");
            }
            var PassRow = dtable.Rows[0];
            PasswordDetailViewModel PassDetailViewModel = PasswordDetailViewModel.FromDataRow(PassRow);
            return PassDetailViewModel;
        }


        public async Task<List<PasswordViewModel>> GetPasswordsAsync()
        {
            FormattableString query = $"SELECT * FROM Passwords";
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