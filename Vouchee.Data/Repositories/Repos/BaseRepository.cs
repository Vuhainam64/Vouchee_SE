using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Data.Repositories.Repos
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly BaseDAO<TEntity> _dao;

        public BaseRepository() => _dao = BaseDAO<TEntity>.Instance;

        public Task<Guid?> AddAsync(TEntity entity) => _dao.AddAsync(entity);

        public Task<bool> UpdateAsync(TEntity entity) => _dao.UpdateAsync(entity);

        public Task<bool> DeleteAsync(TEntity entity) => _dao.DeleteAsync(entity);

        public Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null) =>
            _dao.GetFirstOrDefaultAsync(filter, includeProperties);

        public Task<TEntity?> GetByIdAsync(object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null) =>
            _dao.GetByIdAsync(id, includeProperties);

        public IQueryable<TEntity> GetTable(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null) =>
            _dao.GetTable(includeProperties);
    }
}
