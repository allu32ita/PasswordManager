using System;

namespace PasswordManager.Models.Exceptions
{
    public class PasswordDescrizioneDuplicataException : Exception
    {
        public PasswordDescrizioneDuplicataException(string par_Decrizione, Exception par_InnerException) : base($"Password Descrizione '{par_Decrizione}' esistente", par_InnerException)
        {
        }
    }
}