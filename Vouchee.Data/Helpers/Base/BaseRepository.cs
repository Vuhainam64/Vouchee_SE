using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Vouchee.Data.Helpers.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly VoucheeContext _context;
        private readonly DbSet<TEntity> _table;

        public BaseRepository(VoucheeContext context)
        {
            _context = context;
            _table = context.Set<TEntity>();
        }

        public async Task<Guid?> AddAsync(TEntity entity)
        {
            try
            {
                await _table.AddAsync(entity);
                var result = await SaveChanges();
                if (result)
                {
                    var idProperty = typeof(TEntity).GetProperty("Id");
                    return idProperty != null ? (Guid?)idProperty.GetValue(entity) : null;
                }
                return null;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            try
            {
                await _table.AddAsync(entity);
                var result = await SaveChanges();
                return result ? entity : null; // Return null if no changes were saved
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            try
            {
                _context.Update(entity);
                return await SaveChanges();
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            try
            {
                _table.Remove(entity);
                await SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public async Task<TEntity> FindAsync(Guid id, bool isTracking = false)
        {
            try
            {
                var query = isTracking ? _table : _table.AsNoTracking();
                return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public async Task<TEntity?> GetByIdAsync(object id,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null,
            bool isTracking = false)
        {
            try
            {
                IQueryable<TEntity> query = isTracking ? _table : _table.AsNoTracking();
                if (includeProperties != null)
                {
                    query = includeProperties(query);
                }
                return await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null,
            bool isTracking = false)
        {
            try
            {
                IQueryable<TEntity> query = isTracking ? _table : _table.AsNoTracking();
                if (includeProperties != null)
                {
                    query = includeProperties(query);
                }
                return await query.FirstOrDefaultAsync(filter);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public async Task<IEnumerable<TEntity>?> GetWhereAsync(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null,
            bool isTracking = false)
        {
            try
            {
                IQueryable<TEntity> query = isTracking ? _table : _table.AsNoTracking();
                if (includeProperties != null)
                {
                    query = includeProperties(query);
                }
                return await query.Where(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public IQueryable<TEntity> GetTable()
        {
            return _table;
        }

        public async Task<bool> SaveChanges()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public void Attach(TEntity entity)
        {
            _context.Attach(entity);
        }

        public void Detach(object entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public IQueryable<TEntity> CheckLocal()
        {
            return _context.Set<TEntity>().Local.AsQueryable();
        }

        public EntityState GetEntityState(object entity)
        {
            return _context.Entry(entity).State;
        }

        public void SetEntityState(TEntity entity, EntityState entityState)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _context.Attach(entity);
            }
            entry.State = entityState;
        }

        public async Task ReloadAsync(object entity)
        {
            await _context.Entry(entity).ReloadAsync();
        }

        public object GetModifiedEntity()
        {
            return _context.ChangeTracker.Entries();
        }

        public void MarkModified(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Attach(object entity)
        {
            _context.Attach(entity);
        }

        public async Task<string?> AddReturnString(TEntity entity)
        {
            try
            {
                await _table.AddAsync(entity);
                var result = await SaveChanges();
                if (result)
                {
                    var idProperty = typeof(TEntity).GetProperty("Id");
                    return idProperty != null ? (string?)idProperty.GetValue(entity) : null;
                }
                return null;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }
    }
}
