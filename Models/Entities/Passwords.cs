using System;
using System.Collections.Generic;

namespace PasswordManager.Models.Entities
{
    public partial class Passwords
    {
        public int Id { get; set; }
        public string Password { get; private set; }
        public string Descrizione { get; set; }
        public string DataInserimento { get; set; }
        public string FkUtente { get; set; }
        public virtual ApplicationUser Tab_AspNetUsers {get; set;}
        public string Sito { get; set; }
        public string Tipo { get; set; }
        public string PathFile {get; set; }
        public int RowVersion {get; set; }

        public void ChangePassword(string par_Password)
        {
            if (string.IsNullOrEmpty(par_Password))
            {
                throw new ArgumentException("Password can't be empty");
            }
            Password = par_Password;
        }
    }
}
