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
using System.Linq;
using PasswordManager.Models.InputModels;

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


        public async Task<ListViewModel<PasswordViewModel>> GetPasswordsAsync(PasswordListInputModel model)
        {
            string direction = model.Ascending ? "ASC" : "DESC";
            FormattableString query = $@"SELECT * FROM Passwords where Descrizione LIKE {"%" + model.Search + "%"} ORDER BY {(Sql) model.Orderby} {(Sql) direction} LIMIT {model.Limit} OFFSET {model.Offset}; 
            SELECT COUNT(*) FROM Passwords where Descrizione LIKE {"%" + model.Search + "%"} ";
            DataSet dset = await db.QueryAsync(query);
            var dtable = dset.Tables[0];
            var listaPass = new List<PasswordViewModel>();
            foreach(DataRow passRow in dtable.Rows)
            {
                PasswordViewModel pass = PasswordViewModel.FromDataRow(passRow);
                listaPass.Add(pass);
            }

            ListViewModel<PasswordViewModel> result = new ListViewModel<PasswordViewModel>
            {
                Results = listaPass,
                TotalCount = Convert.ToInt32(dset.Tables[1].Rows[0][0])
            };

            return result;
        }

        public async Task<List<PasswordViewModel>> GetListUltimePasswordAsync()
        {
            PasswordListInputModel List_InputModel = new PasswordListInputModel(
                search: "",
                page: 1,
                orderby: "Id",
                ascending: false,
                limit: (int)OpzioniPassword.CurrentValue.InHome,
                orderPassword : OpzioniPassword.CurrentValue.Order
            );
            ListViewModel<PasswordViewModel> List_PassViewModel = await GetPasswordsAsync(List_InputModel);
            return List_PassViewModel.Results;
        }
    }
}