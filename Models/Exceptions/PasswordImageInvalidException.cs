using System;

namespace PasswordManager.Models.Exceptions
{
    public class PasswordImageInvalidException : Exception
    {
        public PasswordImageInvalidException(int passwordID, Exception par_InnerException) : base($"Immagine della PAssword {passwordID} non valida", par_InnerException)
        {
        }
    }
}