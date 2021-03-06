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
using Microsoft.Data.Sqlite;

namespace PasswordManager.Models.Services.Application
{
    public class AdoNetPasswordService : IPasswordService
    {
        private readonly ILogger<AdoNetPasswordService> log;
        private readonly IDatabaseAccessor db;
        private readonly IOptionsMonitor<PasswordsOptions> OpzioniPassword;
        private readonly IImagePersister par_ImagePersister;

        public AdoNetPasswordService(ILogger<AdoNetPasswordService> log, IDatabaseAccessor db, IImagePersister par_ImagePersister, IOptionsMonitor<PasswordsOptions> OpzioniPassword)
        {
            this.par_ImagePersister = par_ImagePersister;
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
                log.LogWarning("password {id} not found", id);
                throw new PasswordNotFoundException(Convert.ToInt32(id));
            }
            var PassRow = dtable.Rows[0];
            PasswordDetailViewModel PassDetailViewModel = PasswordDetailViewModel.FromDataRow(PassRow);
            return PassDetailViewModel;
        }


        public async Task<ListViewModel<PasswordViewModel>> GetPasswordsAsync(PasswordListInputModel model)
        {
            string direction = model.Ascending ? "ASC" : "DESC";
            FormattableString query = $@"SELECT * FROM Passwords where Descrizione LIKE {"%" + model.Search + "%"} ORDER BY {(Sql)model.Orderby} {(Sql)direction} LIMIT {model.Limit} OFFSET {model.Offset}; 
            SELECT COUNT(*) FROM Passwords where Descrizione LIKE {"%" + model.Search + "%"} ";
            DataSet dset = await db.QueryAsync(query);
            var dtable = dset.Tables[0];
            var listaPass = new List<PasswordViewModel>();
            foreach (DataRow passRow in dtable.Rows)
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
                orderPassword: OpzioniPassword.CurrentValue.Order
            );
            ListViewModel<PasswordViewModel> List_PassViewModel = await GetPasswordsAsync(List_InputModel);
            return List_PassViewModel.Results;
        }

        public async Task<PasswordDetailViewModel> CreatePasswordAsync(PasswordCreateInputModel par_InputModel)
        {
            string sDescrizione = par_InputModel.Descrizione;
            string sDataInserimento = Convert.ToString(DateTime.Now);

            bool bPasswordNonDuplicata = await DescrizioneDuplicataAsync(sDescrizione, 0);

            if (bPasswordNonDuplicata == true)
            {
                PasswordDetailViewModel var_Password;
                string sId = await db.QueryScalarAsync<string>($@"INSERT INTO Passwords (Descrizione, DataInserimento) VALUES ({sDescrizione}, {sDataInserimento});
                                                     SELECT last_insert_rowid();");
                var_Password = await GetPasswordAsync(sId);
                return var_Password;
            }
            else
            {
                throw new PasswordDescrizioneDuplicataException(sDescrizione, new Exception("errore nella creazione della password"));
            }
        }

        public async Task<bool> DescrizioneDuplicataAsync(string par_Descrizione, int par_Id)
        {
            int iNumPasswordTrovate = await db.QueryScalarAsync<int>($"SELECT COUNT(*) FROM Passwords where Descrizione = {par_Descrizione} AND Id <> {par_Id}");
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
            FormattableString query = $"SELECT Id, Password, Descrizione, DataInserimento, FkUtente, Sito, Tipo, PathFile, RowVersion FROM Passwords where Id = {id}; ";
            DataSet var_DataSet = await db.QueryAsync(query);
            var var_PasswordTable = var_DataSet.Tables[0];
            if (var_PasswordTable.Rows.Count != 1)
            {
                //logger.LogWarning("Password {id} not found", id);
                throw new PasswordNotFoundException(id);
            }
            var var_PasswordRow = var_PasswordTable.Rows[0];
            var var_PasswordEditInputModel = PasswordEditInputModel.FromDataRow(var_PasswordRow);
            return var_PasswordEditInputModel;
        }

        public async Task<PasswordDetailViewModel> EditPasswordAsync(PasswordEditInputModel par_InputModel)
        {
            bool bPasswordNonDuplicata = await DescrizioneDuplicataAsync(par_InputModel.Descrizione, par_InputModel.Id);
            if (bPasswordNonDuplicata == false)
            {
                throw new PasswordDescrizioneDuplicataException(par_InputModel.Descrizione, new Exception("errore nella creazione della password"));
            }
            string sFilePath = null;
            if (par_InputModel.FilePassword != null)
            {
                try
                {
                    sFilePath   = await par_ImagePersister.SavePasswordImageAsync(par_InputModel.Id, par_InputModel.FilePassword);
                }
                catch (System.Exception exc)
                {
                    throw new PasswordImageInvalidException(par_InputModel.Id, exc);
                }
            }
            int var_NumRigheUpd = await db.CommandAsync($@"UPDATE Passwords 
                                                        SET PathFile=COALESCE({sFilePath}, PathFile), 
                                                        Password={par_InputModel.Password}, 
                                                        Descrizione={par_InputModel.Descrizione}, 
                                                        DataInserimento={par_InputModel.DataInserimento}, 
                                                        FkUtente={par_InputModel.FkUtente}, 
                                                        Sito={par_InputModel.Sito}, 
                                                        Tipo={par_InputModel.Tipo} 
                                                        WHERE Id={par_InputModel.Id} AND RowVersion={par_InputModel.RowVersion}");
            if (var_NumRigheUpd == 0)
            {
                int var_RecordTrovato = await db.QueryScalarAsync<int>($"SELECT COUNT(*) FROM Passwords WHERE Id={par_InputModel.Id}");
                if (var_RecordTrovato > 0)
                {
                    throw new DBConcurrencyException("Non e' possibile effettuare l'update perche un altro utente ha effettuato delle modifiche.");
                }
                else
                {
                    throw new PasswordNotFoundException(par_InputModel.Id);  
                }
                 
            }
            PasswordDetailViewModel var_Password = await GetPasswordAsync(par_InputModel.Id.ToString());
            return var_Password;
        }

        public async Task DeletePasswordAsync(PasswordDeleteInputModel par_InputModel)
        {
            int var_RecordCancellati = await db.CommandAsync($"DELETE FROM Passwords WHERE Id={par_InputModel.Id}");
            if (var_RecordCancellati == 0)
            {
                throw new PasswordNotFoundException(par_InputModel.Id);
            }
        }
    }
}