using Microsoft.EntityFrameworkCore;
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

        public Task<TEntity?> GetByIdAsync(object id, 
                                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties,
                                            bool isTracking = false) 
            => _dao.GetByIdAsync(id, includeProperties, isTracking);

        public IQueryable<TEntity> GetTable() => _dao.GetTable();

        public Task<TEntity> FindAsync(Guid id, bool isTracking = false) 
            => _dao.FindAsync(id, isTracking);

        public Task<TEntity> Add(TEntity entity) => _dao.Add(entity);

        public Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter, 
                                                        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties,
                                                        bool isTracking = false)
            => _dao.GetFirstOrDefaultAsync(filter, includeProperties, isTracking);

        public Task<IEnumerable<TEntity>?> GetWhereAsync(Expression<Func<TEntity, bool>> filter, 
                                                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties,
                                                            bool isTracking = false)
            => _dao.GetWhereAsync(filter, includeProperties, isTracking);

        public async Task<bool> SaveChanges() => await _dao.SaveChanges();

        public void Attach(object entity) => _dao.Attach(entity);

        //public IQueryable<TEntity> CheckLocal() => _dao.CheckLocal();

        public void Detach(object entity) => _dao.Detach(entity);

        public EntityState GetEntityState(object entity) => _dao.GetEntityState(entity);

        public void SetEntityState(TEntity entity, EntityState entityState) => _dao.SetEntityState(entity, entityState);
    }
}