using System;
using System.Collections.Generic;

namespace PasswordManager.Models.Entities
{
    public partial class Passwords
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string Descrizione { get; set; }
        public string DataInserimento { get; set; }
        public int FkUtente { get; set; }
        public string Sito { get; set; }
        public string Tipo { get; set; }
    }
}
