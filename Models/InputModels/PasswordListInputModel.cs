using PasswordManager.Models.Options;
using Microsoft.AspNetCore.Mvc;
using PasswordManager.Customizations.ModelBinders;
using System;
using System.Linq;

namespace PasswordManager.Models.InputModels
{
    [ModelBinder(BinderType = typeof(PasswordListInputModelBinder))]
    public class PasswordListInputModel
    {
        public PasswordListInputModel(string search, int page, string orderby, bool ascending, PasswordsOptions opzioniPasswords)
        {
            //sanitizzazione
            var orderOptions = opzioniPasswords.Order;
            if (!orderOptions.Allow.Contains(orderby))
            {
                orderby = orderOptions.By;
                ascending = orderOptions.Ascending;
            }


            Search = search ?? "";
            Page = Math.Max(1, page);
            Orderby = orderby;
            Ascending = ascending;

            Limit = (int)opzioniPasswords.PerPage;
            Offset = (Page - 1) * Limit;
        }
       
        public string Search { get; set; }
        public int Page { get; set; }
        public string Orderby { get; set; }
        public bool Ascending { get; set; }

        public int Limit {get; set;}

        public int Offset {get; set;}
    }
}