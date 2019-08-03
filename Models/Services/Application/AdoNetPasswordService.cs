using System.Collections.Generic;
using System.Data;
using PasswordManager.Models.Services.Infrastructure;
using PasswordManager.Models.ViewModels;

namespace PasswordManager.Models.Services.Application
{
    public class AdoNetPasswordService : IPasswordService
    {
        private readonly IDatabaseAccessor db;

        public AdoNetPasswordService(IDatabaseAccessor db)
        {
            this.db = db;
        }

        public PasswordDetailViewModel GetPassword(string id)
        {
            throw new System.NotImplementedException();
        }

        public List<PasswordViewModel> GetPasswords()
        {
            var listaPsw = new List<PasswordViewModel>();
            string query = "SELECT * FROM Passwords";
            DataSet dset = db.Query(query);
            
        }
    }
}