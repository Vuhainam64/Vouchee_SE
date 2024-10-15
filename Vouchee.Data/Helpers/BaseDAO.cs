using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
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
                var result = await _context.SaveChangesAsync();

                if (result > 0)
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
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> UpdateAsync(TEntity entity)
        {
            try
            {
                _context.Entry(entity).CurrentValues.SetValues(entity);
                if (_context.Entry(entity).State == EntityState.Unchanged)
                {
                    return true;
                }

                Table.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(TEntity entity)
        {
            try
            {
                Table.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter,
                                                           Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null)
        {
            try
            {
                IQueryable<TEntity> query = Table;

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
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> filter,
                                                                Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null)
        {
            try
            {
                IQueryable<TEntity> query = Table;

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
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }


        public async Task<TEntity?> GetByIdAsync(object id, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null)
        {
            try
            {
                IQueryable<TEntity> query = Table;

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


        public async Task<TEntity?> FindAsync(Guid id)
        {
            try
            {
                return await Table.FindAsync(id);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public IQueryable<TEntity> GetTable(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? includeProperties = null)
        {
            try
            {
                IQueryable<TEntity> query = Table;

                if (includeProperties != null)
                {
                    query = includeProperties(query);
                }

                return query;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<TEntity?> Add(TEntity entity)
        {
            try
            {
                // Add the entity to the database context
                await Table.AddAsync(entity);

                // Save changes to the database
                var result = await _context.SaveChangesAsync();

                // If the save was successful, return the entity
                if (result > 0)
                {
                    return entity;
                }

                return null; // Return null if no changes were saved
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception("Error adding entity: " + ex.Message);
            }
        }

        internal void Attach(Address entity)
        {
            _context.Entry(entity).State = EntityState.Unchanged;
        }
    }
}
