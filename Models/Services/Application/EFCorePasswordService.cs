using System.Collections.Generic;
using System.Threading.Tasks;
using PasswordManager.Models.ViewModels;
using PasswordManager.Models.Services.Infrastructure;
using System.Linq;
using PasswordManager.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Logging;

namespace PasswordManager.Models.Services.Application
{
    public class EFCorePasswordService : IPasswordService
    {
        private readonly ILogger<AdoNetPasswordService> log;
        private readonly PasswordDbContext dbContext;

        public EFCorePasswordService(ILogger<AdoNetPasswordService> log, PasswordDbContext dbContext)
        {
            this.log = log;
            this.dbContext = dbContext;
        }

        public async Task<PasswordDetailViewModel> GetPasswordAsync(string id)
        {
            log.LogInformation("password {id} requested", id);
            int int_ID = Convert.ToInt32(id);
            PasswordDetailViewModel pswdet = await dbContext.Passwords.Where(Var_password => Var_password.Id == int_ID)
                                                                      .Select(Var_password => new PasswordDetailViewModel {
                                                                          id = Var_password.Id,
                                                                          decrizioneEstesa = "",
                                                                          descrizione = Var_password.Descrizione,
                                                                          Password = Var_password.Password,
                                                                          sito = Var_password.Sito,
                                                                          tipo = Var_password.Tipo
                                                                      }).SingleAsync();
            return pswdet;
        }

        public async Task<List<PasswordViewModel>> GetPasswordsAsync()
        {
            IQueryable<PasswordViewModel> Qry_listapsw = dbContext.Passwords
            .AsNoTracking()
            .Select(Var_password => new PasswordViewModel {
                id = Var_password.Id,
                descrizione = Var_password.Descrizione,
                Password = Var_password.Password,
                sito = Var_password.Sito,
                tipo = Var_password.Tipo
            });
            List<PasswordViewModel> listapsw = await Qry_listapsw.ToListAsync(); 
            return listapsw;
        }
    }
}