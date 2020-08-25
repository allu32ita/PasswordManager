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
using PasswordManager.Models.Exceptions;

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
            PasswordDetailViewModel pswdet = await dbContext.Passwords.Where(var_Password => var_Password.Id == int_ID)
                                                                      .Select(var_Password => new PasswordDetailViewModel
                                                                      {
                                                                          Id                = var_Password.Id,
                                                                          decrizioneEstesa  = "",
                                                                          Descrizione       = var_Password.Descrizione,
                                                                          Password          = var_Password.Password,
                                                                          Sito              = var_Password.Sito,
                                                                          Tipo              = var_Password.Tipo,
                                                                          PathFile          = var_Password.PathFile
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

            bool bPasswordNonDuplicata = await DescrizioneDuplicataAsync(sDescrizione);

            if (bPasswordNonDuplicata == true)
            {
                var var_Password = new Passwords();
                var_Password.Descrizione        = sDescrizione;
                var_Password.DataInserimento    = sDataInserimento;

                dbContext.Add(var_Password);
                await dbContext.SaveChangesAsync();
    
                PasswordDetailViewModel var_DetailPassword = PasswordDetailViewModel.FromEntity(var_Password);        
                return var_DetailPassword;
            }
            else
            {
               throw new PasswordDescrizioneDuplicataException(sDescrizione, new Exception ("errore nella creazione della password"));
            }
        }

        public async Task<bool> DescrizioneDuplicataAsync(string par_Descrizione)
        {
            IQueryable<Passwords> BaseQuery = dbContext.Passwords;
            IQueryable<Passwords> Qry_listapsw = BaseQuery
            .Where(Var_password => Var_password.Descrizione.Equals(par_Descrizione))
            .AsNoTracking();
            int iNumPasswordTrovate = await Qry_listapsw.CountAsync();

            if (iNumPasswordTrovate == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<PasswordEditInputModel> GetPasswordForEditingAsync(int id)
        {
            IQueryable<Passwords> BaseQuery = dbContext.Passwords;
            IQueryable<Passwords> Qry_Linq = BaseQuery
            .Where(var_password => var_password.Id == id)
            .Select(var_password => PasswordEditInputModel.FromEntity(var_password))
            .AsNoTracking();
            var var_Password = await Qry_Linq.FirstOrDefaultAsync();
            if (var_Password == null)
            {
                //logger.LogWarning("Password {id} not found", id);
                throw new PasswordNotFoundException(id);
            }
            PasswordEditInputModel var_PasswordEditInputModel = new PasswordEditInputModel();
            var_PasswordEditInputModel.Id               = var_Password.Id;
            var_PasswordEditInputModel.Password         = var_Password.Password;
            var_PasswordEditInputModel.Descrizione      = var_Password.Descrizione;
            var_PasswordEditInputModel.DataInserimento  = var_Password.DataInserimento;
            var_PasswordEditInputModel.FkUtente         = var_Password.FkUtente;
            var_PasswordEditInputModel.Sito             = var_Password.Sito;
            var_PasswordEditInputModel.Tipo             = var_Password.Tipo;
            var_PasswordEditInputModel.PathFile         = var_Password.PathFile;
            return var_PasswordEditInputModel;
        }

        public async Task<PasswordDetailViewModel> EditPasswordAsync(PasswordEditInputModel par_InputModel)
        {
            string sDescrizione = par_InputModel.Descrizione;
            bool bPasswordNonDuplicata = await DescrizioneDuplicataAsync(sDescrizione);

            if (bPasswordNonDuplicata == true)
            {
                Passwords var_Password = await dbContext.Passwords.FindAsync(par_InputModel.Id);
                var_Password.ChangePassword(par_InputModel.Password);
                var_Password.Descrizione = par_InputModel.Descrizione;
                var_Password.DataInserimento = par_InputModel.DataInserimento;
                var_Password.FkUtente = par_InputModel.FkUtente;
                var_Password.Sito = par_InputModel.Sito;
                var_Password.Tipo = par_InputModel.Tipo;
                var_Password.PathFile = par_InputModel.PathFile;
                await dbContext.SaveChangesAsync();

                PasswordDetailViewModel var_PasswordDetailViewModel = PasswordDetailViewModel.FromEntity(var_Password);
                return var_PasswordDetailViewModel;
            }
            else
            {
                throw new PasswordDescrizioneDuplicataException(sDescrizione, new Exception ("errore nella creazione della password"));
            }
        }
    }
}