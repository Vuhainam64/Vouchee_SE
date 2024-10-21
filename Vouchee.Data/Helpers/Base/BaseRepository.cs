using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Helpers.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly BaseDAO<TEntity> _dao;

        public BaseRepository() => _dao = BaseDAO<TEntity>.Instance;

        public Task<Guid?> AddAsync(TEntity entity) => _dao.AddAsync(entity);

        public Task<bool> UpdateAsync(TEntity entity) => _dao.UpdateAsync(entity);

        public Task<bool> DeleteAsync(TEntity entity) => _dao.DeleteAsync(entity);

        public Task<TEntity?> GetByIdAsync(object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties) =>
            _dao.GetByIdAsync(id, includeProperties);

        public IQueryable<TEntity> GetTable(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties) =>
            _dao.GetTable(includeProperties);

        public Task<TEntity> FindAsync(Guid id) => _dao.FindAsync(id);

        public Task<TEntity> Add(TEntity entity) => _dao.Add(entity);

        public Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties)
            => _dao.GetFirstOrDefaultAsync(filter, includeProperties);

        public Task<IEnumerable<TEntity>?> GetWhereAsync(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties)
            => _dao.GetWhereAsync(filter, includeProperties);

        public void Attach(TEntity entity) => _dao.Attach(entity);

        public IQueryable<TEntity> CheckLocal() => _dao.CheckLocal();

        public void Detach(TEntity entity) => _dao.Detach(entity);
    }
}