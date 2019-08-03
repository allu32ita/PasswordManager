using System;
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
            string query = "SELECT * FROM Passwords WHERE Id = " + id;
            DataSet dset = db.Query(query);
            var dtable = dset.Tables[0];
            if (dtable.Rows.Count != 1)
            {
                throw new InvalidOperationException("Id non valido.");
            }
            var PassRow = dtable.Rows[0];
            PasswordDetailViewModel PassDetailViewModel = PasswordDetailViewModel.FromDataRow(PassRow);
            return PassDetailViewModel;
        }


        public List<PasswordViewModel> GetPasswords()
        {
            string query = "SELECT * FROM Passwords";
            DataSet dset = db.Query(query);
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