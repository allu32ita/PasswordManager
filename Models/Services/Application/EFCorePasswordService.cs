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
                                                                          id = Var_password.Id,
                                                                          decrizioneEstesa = "",
                                                                          descrizione = Var_password.Descrizione,
                                                                          Password = Var_password.Password,
                                                                          sito = Var_password.Sito,
                                                                          tipo = Var_password.Tipo
                                                                      }).SingleAsync();
            return pswdet;
        }

        public async Task<List<PasswordViewModel>> GetPasswordsAsync(string search, int page, string orderby, bool ascending)
        {
            search = search ?? "";
            page = Math.Max(1, page);
            int limit = (int)OpzioniPassword.CurrentValue.PerPage;
            int offset = (page - 1) * limit;
            IQueryable<PasswordViewModel> Qry_listapsw = dbContext.Passwords
            .Where(Var_password => Var_password.Descrizione.Contains(search))
            .Skip(offset)
            .Take(limit)
            .AsNoTracking()
            .Select(Var_password => new PasswordViewModel
            {
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