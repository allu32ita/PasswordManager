using System;

namespace PasswordManager.Models.Services.Infrastructure
{
    public class Sql
    {
        private Sql(string ValoreInInput)
        {
            ValoreInOutput = ValoreInInput;
        }
        public string ValoreInOutput { get; }

        public static explicit operator Sql(string value) => new Sql(value);
        public override string ToString() {
            return this.ValoreInOutput;
        }
        
    }
}