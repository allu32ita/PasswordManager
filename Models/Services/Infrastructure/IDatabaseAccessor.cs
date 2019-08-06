using System;
using System.Data;
using System.Threading.Tasks;

namespace PasswordManager.Models.Services.Infrastructure
{
    public interface IDatabaseAccessor
    {
        Task<DataSet> QueryAsync(FormattableString formatquery);
    }
}