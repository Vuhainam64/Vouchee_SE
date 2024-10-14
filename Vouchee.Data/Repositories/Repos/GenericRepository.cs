using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Data.Repositories.Repos
{
    public class GenericRepository : IGenericRepository
    {
        public Task<List<T>> ExecuteRawSqlAsync<T>(string sql, params object[] parameters) where T : class
        {
            return GenericDAO.Instance.ExecuteRawSqlAsync<T>(sql, parameters);
        }
    }
}
