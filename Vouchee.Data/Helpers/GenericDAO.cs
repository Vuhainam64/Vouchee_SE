using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Helpers
{
    public class GenericDAO
    {
        private readonly VoucheeContext _context;
        private static GenericDAO instance;
        private static readonly object InstanceLock = new object();

        public GenericDAO(VoucheeContext context)
        {
            _context = context;
        }

        public static GenericDAO Instance
        {
            get
            {
                lock (InstanceLock)
                {
                    if (instance == null)
                    {
                        VoucheeContext context = new VoucheeContext();
                        instance = new GenericDAO(context);
                    }
                    return instance;
                }
            }
        }

        public async Task<List<T>> ExecuteRawSqlAsync<T>(string sql, params object[] parameters) where T : class
        {
            return await _context.Set<T>().FromSqlRaw(sql, parameters).ToListAsync();
        }
    }
}
