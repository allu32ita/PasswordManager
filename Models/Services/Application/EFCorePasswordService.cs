using System.Collections.Generic;
using System.Threading.Tasks;
using PasswordManager.Models.ViewModels;
using PasswordManager.Models.Services.Infrastructure;
using System.Linq;
using PasswordManager.Models.Entities;
using PasswordManager.Models.Options;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PasswordManager.Models.InputModels;

namespace PasswordManager.Models.Services.Application
{
    public class EFCorePasswordService : IPasswordService
    {
        private readonly ILogger<AdoNetPasswordService> log;
        private readonly PasswordDbContext dbContext;
        private readonly IOptionsMonitor<PasswordsOptions> OpzioniPassword;

        public EFCorePasswordService(ILogger<AdoNetPasswordService> log, PasswordDbContext dbContext, IOptionsMonitor<PasswordsOptions> OpzioniPassword)
        {
            this.OpzioniPassword = OpzioniPassword;
            this.log = log;
            this.dbContext = dbContext;
        }

        public async Task<PasswordDetailViewModel> GetPasswordAsync(string id)
        {
            log.LogInformation("password {id} requested", id);
            int int_ID = Convert.ToInt32(id);
            PasswordDetailViewModel pswdet = await dbContext.Passwords.Where(Var_password => Var_password.Id == int_ID)
                                                                      .Select(Var_password => new PasswordDetailViewModel
                                                                      {
                                                                          Id = Var_password.Id,
                                                                          decrizioneEstesa = "",
                                                                          Descrizione = Var_password.Descrizione,
                                                                          Password = Var_password.Password,
                                                                          Sito = Var_password.Sito,
                                                                          Tipo = Var_password.Tipo
                                                                      }).SingleAsync();
            return pswdet;
        }

        public async Task<ListViewModel<PasswordViewModel>> GetPasswordsAsync(PasswordListInputModel model)
        {
            IQueryable<Passwords> BaseQuery = dbContext.Passwords;

            switch (model.Orderby)
            {
                case "Id":
                    if (model.Ascending)
                    {
                        BaseQuery = BaseQuery.OrderBy(Var_Password => Var_Password.Id);
                    }
                    else
                    {
                        BaseQuery = BaseQuery.OrderByDescending(Var_Password => Var_Password.Id);
                    }
                    break;
                case "Descrizione":
                    if (model.Ascending)
                    {
                        BaseQuery = BaseQuery.OrderBy(Var_Password => Var_Password.Descrizione);
                    }
                    else
                    {
                        BaseQuery = BaseQuery.OrderByDescending(Var_Password => Var_Password.Descrizione);
                    }
                    break;
                case "Sito":
                    if (model.Ascending)
                    {
                        BaseQuery = BaseQuery.OrderBy(Var_Password => Var_Password.Sito);
                    }
                    else
                    {
                        BaseQuery = BaseQuery.OrderByDescending(Var_Password => Var_Password.Sito);
                    }
                    break;     
            }

            IQueryable<Passwords> Qry_listapsw = BaseQuery
            .Where(Var_password => Var_password.Descrizione.Contains(model.Search))
            .AsNoTracking();
            
            List<PasswordViewModel> listapsw = await Qry_listapsw
            .Skip(model.Offset)
            .Take(model.Limit)
            .Select(Var_password => new PasswordViewModel
            {
                Id = Var_password.Id,
                Descrizione = Var_password.Descrizione,
                Password = Var_password.Password,
                Sito = Var_password.Sito,
                Tipo = Var_password.Tipo
            })
            .ToListAsync();

            int totalCount = await Qry_listapsw.CountAsync();

            ListViewModel<PasswordViewModel> result = new ListViewModel<PasswordViewModel>
            {
                Results = listapsw,
                TotalCount = totalCount
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

        public async Task<PasswordDetailViewModel> CreatePasswordAsync(PasswordCreateInputModel par_InputModel)
        {
            string sDescrizione = par_InputModel.Descrizione;
            string sDataInserimento = Convert.ToString(DateTime.Now);

            var var_Password = new Passwords();
            var_Password.Descrizione        = sDescrizione;
            var_Password.DataInserimento    = sDataInserimento;

            dbContext.Add(var_Password);
            await dbContext.SaveChangesAsync();

            PasswordDetailViewModel var_DetailPassword = new PasswordDetailViewModel();
            var_DetailPassword.Id               = var_Password.Id;
            var_DetailPassword.Password         = var_Password.Password;
            var_DetailPassword.Descrizione      = var_Password.Descrizione;
            var_DetailPassword.DataInserimento  = var_Password.DataInserimento;
            var_DetailPassword.FkUtente         = var_Password.FkUtente;
            var_DetailPassword.Sito             = var_Password.Sito;
            var_DetailPassword.Tipo             = var_Password.Tipo;            

            return var_DetailPassword;
        }
    }
}