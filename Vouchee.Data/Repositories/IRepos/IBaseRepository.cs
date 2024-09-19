using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Repositories.IRepos
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<Guid?> AddAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(TEntity entity);
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter,
                                              Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null);
        Task<TEntity?> GetByIdAsync(object id,
                                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null);
        IQueryable<TEntity> GetTable(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null);
    }
}
