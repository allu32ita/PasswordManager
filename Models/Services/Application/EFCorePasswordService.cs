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
using System.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace PasswordManager.Models.Services.Application
{
    public class EFCorePasswordService : IPasswordService
    {
        private readonly ILogger<AdoNetPasswordService> log;
        private readonly PasswordDbContext dbContext;
        private readonly IOptionsMonitor<PasswordsOptions> OpzioniPassword;
        private readonly IHttpContextAccessor par_HttpContextAccessor;
        private readonly IImagePersister par_ImagePersister;

        public EFCorePasswordService(ILogger<AdoNetPasswordService> log, 
                                    PasswordDbContext dbContext, 
                                    IImagePersister par_ImagePersister, 
                                    IOptionsMonitor<PasswordsOptions> OpzioniPassword,
                                    IHttpContextAccessor par_HttpContextAccessor)
        {
            this.par_ImagePersister = par_ImagePersister;
            this.OpzioniPassword = OpzioniPassword;
            this.par_HttpContextAccessor = par_HttpContextAccessor;
            this.log = log;
            this.dbContext = dbContext;
        }

        public async Task<PasswordDetailViewModel> GetPasswordAsync(string id)
        {
            log.LogInformation("password {id} requested", id);
            int int_ID = Convert.ToInt32(id);
            PasswordDetailViewModel pswdet = await dbContext.Passwords.Where(var_Password => var_Password.Id == int_ID)
                                                                      .Select(var_Password => PasswordDetailViewModel.FromEntity(var_Password)).SingleAsync();
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
            .Select(var_Password => PasswordViewModel.FromEntity(var_Password))
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
                orderPassword: OpzioniPassword.CurrentValue.Order
            );

            ListViewModel<PasswordViewModel> List_PassViewModel = await GetPasswordsAsync(List_InputModel);
            return List_PassViewModel.Results;
        }

        public async Task<PasswordDetailViewModel> CreatePasswordAsync(PasswordCreateInputModel par_InputModel)
        {
            string sDescrizione = par_InputModel.Descrizione;
            string sDataInserimento = Convert.ToString(DateTime.Now);
            string sFkUtente;

            try
            {
                sFkUtente = par_HttpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            catch (NullReferenceException)
            {
                throw new Exception("E' necessario un utente per effettuare questa operazione.");
            }

            bool bPasswordNonDuplicata = await DescrizioneDuplicataAsync(sDescrizione, 0);

            if (bPasswordNonDuplicata == true)
            {
                var var_Password = new Passwords();
                var_Password.Descrizione = sDescrizione;
                var_Password.DataInserimento = sDataInserimento;
                var_Password.FkUtente = sFkUtente;

                dbContext.Add(var_Password);
                await dbContext.SaveChangesAsync();

                PasswordDetailViewModel var_DetailPassword = PasswordDetailViewModel.FromEntity(var_Password);
                return var_DetailPassword;
            }
            else
            {
                throw new PasswordDescrizioneDuplicataException(sDescrizione, new Exception("errore nella creazione della password"));
            }
        }

        public async Task<bool> DescrizioneDuplicataAsync(string par_Descrizione, int par_Id)
        {
            IQueryable<Passwords> BaseQuery = dbContext.Passwords;
            IQueryable<Passwords> Qry_listapsw = BaseQuery
            .Where(var_Password => var_Password.Descrizione.Equals(par_Descrizione) && var_Password.Id != par_Id)
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
            IQueryable<PasswordEditInputModel> Qry_Linq = dbContext.Passwords
            .Where(var_password => var_password.Id == id)
            .Select(var_password => PasswordEditInputModel.FromEntity(var_password))
            .AsNoTracking();
            PasswordEditInputModel var_Password = await Qry_Linq.FirstOrDefaultAsync();
            if (var_Password == null)
            {
                //logger.LogWarning("Password {id} not found", id);
                throw new PasswordNotFoundException(id);
            }

            return var_Password;
        }

        public async Task<PasswordDetailViewModel> EditPasswordAsync(PasswordEditInputModel par_InputModel)
        {
            string sDescrizione = par_InputModel.Descrizione;
            bool bPasswordNonDuplicata = await DescrizioneDuplicataAsync(sDescrizione, par_InputModel.Id);

            if (bPasswordNonDuplicata == true)
            {
                Passwords var_Password = await dbContext.Passwords.FindAsync(par_InputModel.Id);
                var_Password.ChangePassword(par_InputModel.Password);
                var_Password.Descrizione = par_InputModel.Descrizione;
                var_Password.DataInserimento = par_InputModel.DataInserimento;
                var_Password.FkUtente = par_InputModel.FkUtente;
                var_Password.Sito = par_InputModel.Sito;
                var_Password.Tipo = par_InputModel.Tipo;

                dbContext.Entry(var_Password).Property(var_Password => var_Password.RowVersion).OriginalValue = par_InputModel.RowVersion;

                if (par_InputModel.FilePassword != null)
                {
                    try
                    {
                        string sFilePath = await par_ImagePersister.SavePasswordImageAsync(par_InputModel.Id, par_InputModel.FilePassword);
                        var_Password.PathFile = sFilePath;    
                    }
                    catch (System.Exception exc)
                    {
                        throw new PasswordImageInvalidException(par_InputModel.Id, exc);
                    }
                }

                try
                {
                   await dbContext.SaveChangesAsync(); 
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw new DBConcurrencyException("Non e' possibile effettuare l'update perche un altro utente ha effettuato delle modifiche.");
                }
                PasswordDetailViewModel var_PasswordDetailViewModel = PasswordDetailViewModel.FromEntity(var_Password);
                return var_PasswordDetailViewModel;
            }
            else
            {
                throw new PasswordDescrizioneDuplicataException(sDescrizione, new Exception("errore nella creazione della password"));
            }
        }

        public async Task DeletePasswordAsync(PasswordDeleteInputModel par_InputModel)
        {
            Passwords var_Password = await dbContext.Passwords.FindAsync(par_InputModel.Id);
            if (var_Password == null)
            {
                throw new PasswordNotFoundException(par_InputModel.Id);
            }
            dbContext.Remove(var_Password);
            await dbContext.SaveChangesAsync();
        }
    }
}