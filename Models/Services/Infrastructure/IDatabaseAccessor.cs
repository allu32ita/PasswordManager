using System;
using System.Data;
using System.Threading.Tasks;

namespace PasswordManager.Models.Services.Infrastructure
{
    public interface IDatabaseAccessor
    {
        Task<DataSet> QueryAsync(FormattableString par_Formatquery);
        Task<T> QueryScalarAsync<T>(FormattableString par_Formatquery);
        Task<int> CommandAsync(FormattableString par_Formatcommand);
    }
}