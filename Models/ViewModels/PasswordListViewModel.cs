using System.Collections.Generic;
using PasswordManager.Models.InputModels;

namespace PasswordManager.Models.ViewModels
{
    public class PasswordListViewModel : IPaginationInfo
    {
        public ListViewModel<PasswordViewModel> Passwords {get; set;}
        public PasswordListInputModel Input {get; set;}


        int IPaginationInfo.CurrentPage => Input.Page;

        int IPaginationInfo.TotalResults => Passwords.TotalCount;

        int IPaginationInfo.ResultsPerPage => Input.Limit;

        string IPaginationInfo.Search => Input.Search;

        string IPaginationInfo.OrderBy => Input.Orderby;

        bool IPaginationInfo.Ascending => Input.Ascending;
    }
}