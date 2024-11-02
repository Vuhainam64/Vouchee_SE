using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Xml;
using Vouchee.Data.Models.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Vouchee.Data.Helpers
{
    public class BaseDAO<TEntity> where TEntity : class
    {
        private static BaseDAO<TEntity>? instance;
        private static readonly object InstanceLock = new object();
        private readonly VoucheeContext _context;
        private DbSet<TEntity> Table { get; set; }

        // Constructor
        private BaseDAO(VoucheeContext context)
        {
            _context = context;
            Table = context.Set<TEntity>();
        }

        // Singleton Instance
        public static BaseDAO<TEntity> Instance
        {
            get
            {
                lock (InstanceLock)
                {
                    if (instance == null)
                    {
                        VoucheeContext context = new VoucheeContext();
                        instance = new BaseDAO<TEntity>(context);
                    }
                    return instance;
                }
            }
        }

        public async Task<Guid?> AddAsync(TEntity entity)
        {
            try
            {
                await Table.AddAsync(entity);
                var result = await SaveChanges();

                if (result)
                {
                    var idProperty = typeof(TEntity).GetProperty("Id");
                    if (idProperty != null)
                    {
                        return (Guid?)idProperty.GetValue(entity);
                    }
                }
                return null;
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
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                LoggerService.Logger(errorMessage);
                throw new Exception(errorMessage);
            }
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            try
            {
                Table.Remove(entity);
                await SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter,
                                                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null, 
                                                            bool isTracking = false)
        {
            try
            {
                IQueryable<TEntity> query = isTracking ? Table : Table.AsNoTracking();

                // Apply includes with ThenInclude support
                if (includeProperties != null)
                {
                    query = includeProperties(query);
                }

                // Apply the filter and return the first matching entity
                return await query.FirstOrDefaultAsync(filter);

            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> filter,
                                                                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null,
                                                                bool isTracking = false)
        {
            try
            {
                IQueryable<TEntity> query = isTracking ? Table : Table.AsNoTracking();

                // Apply includes with ThenInclude support if provided
                if (includeProperties != null)
                {
                    query = includeProperties(query);
                }

                // Apply the filter and return the matching entities
                return await query.Where(filter).ToListAsync();
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
                IQueryable<TEntity> query = isTracking ? Table : Table.AsNoTracking();

                if (includeProperties != null)
                {
                    query = includeProperties(query);
                }

                // Check if the entity is already tracked in the context
                var entity = await query.FirstOrDefaultAsync(e => EF.Property<object>(e, "Id").Equals(id));

                return entity;

                // If the entity does not exist, return null
                return null;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }


        public async Task<TEntity?> FindAsync(Guid id, bool trackChanges = false)
        {
            try
            {
                var query = trackChanges ? Table : Table.AsNoTracking();
                return await Table.FindAsync(id);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public IQueryable<TEntity> GetTable()
        {
            try
            {
                IQueryable<TEntity> query = Table;
                return query;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public async Task<TEntity?> Add(TEntity entity)
        {
            try
            {
                await Table.AddAsync(entity);
                var result = await SaveChanges();

                if (result)
                {
                    return entity;
                }

                return null; // Return null if no changes were saved
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.InnerException?.Message);
                throw new Exception(ex.InnerException?.Message);
            }
        }

        public void Attach(object entity)
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

        public void Attach(Category category)
        {
            _context.Attach(category);
        }

        // Specific Attach method for Address
        internal void Attach(Address entity)
        {
            _context.Address.Attach(entity);
        }

        public Address GetLocalAddress(decimal lon, decimal lat)
        {
            // Check if the address is already being tracked in the context
            var trackedAddress = _context.Address.Local.FirstOrDefault(a => a.Lon == lon && a.Lat == lat);

            if (trackedAddress != null)
            {
                // Not tracked yet, attach it
                _context.Address.Attach(trackedAddress);
            }

            return trackedAddress;
        }

        public async Task<bool> SaveChanges()
        {
            _context.ChangeTracker.DetectChanges();
            Console.WriteLine(_context.ChangeTracker.DebugView.LongView);
            return await _context.SaveChangesAsync() > 0;
        }

        public EntityState GetEntityState(object entity)
        {
            _context.ChangeTracker.DetectChanges();
            return _context.Entry(entity).State;
        }

        public void SetEntityState(TEntity entity, EntityState state)
        {
            var entry = _context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                _context.Attach(entity);
            }
            entry.State = state;
        }

        public async Task ReloadAsync(TEntity entity)
        {
            await _context.Entry(entity).ReloadAsync();
        }

        public object GetModifiedEntity()
        {
            var trackedEntities = _context.ChangeTracker.Entries();
            return trackedEntities;
        }
    }
}
