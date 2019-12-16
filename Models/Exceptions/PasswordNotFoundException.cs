using System;

namespace PasswordManager.Models.Exceptions
{
    public class PasswordNotFoundException : Exception
    {
        public PasswordNotFoundException(int passwordID) : base($"Password {passwordID} not found")
        {

        }
    }
}