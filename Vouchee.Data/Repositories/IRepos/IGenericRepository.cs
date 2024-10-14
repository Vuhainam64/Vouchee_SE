using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Repositories.IRepos
{
    public interface IGenericRepository
    {
        Task<List<T>> ExecuteRawSqlAsync<T>(string sql, params object[] parameters) where T : class;
    }
}
