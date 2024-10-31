using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers;

namespace Vouchee.Business.Helpers
{
    public static class LinQUtils
    {
        public static IQueryable<TEntity> DynamicFilter<TEntity>(this IQueryable<TEntity> source, TEntity entity)
        {
            try
            {
                var properties = entity.GetType().GetProperties();
                foreach (var item in properties)
                {
                    if (item.PropertyType == typeof(string))
                    {
                        // Skip filtering for string properties
                        var propertyVal = item.GetValue(entity, null);
                        if (propertyVal != null)
                        {
                            source = source.Where($"{item.Name}.ToLower().Contains(@0)", propertyVal.ToString().Trim().ToLower());
                        }
                    }
                    else if (item.PropertyType.IsGenericType && typeof(ICollection<>).IsAssignableFrom(item.PropertyType.GetGenericTypeDefinition()))
                    {
                        // Skip filtering for collection properties
                        continue;
                    }
                    else
                    {
                        var propertyVal = item.GetValue(entity, null);
                        if (propertyVal == null) continue;

                        if (item.CustomAttributes.Any(a => a.AttributeType == typeof(SkipAttribute))) continue;

                        bool isDateTime = typeof(DateTime).IsAssignableFrom(item.PropertyType) || typeof(DateTime?).IsAssignableFrom(item.PropertyType);
                        if (isDateTime)
                        {
                            DateTime dt = (DateTime)propertyVal;
                            source = source.Where($"{item.Name} >= @0 && {item.Name} < @1", dt.Date, dt.Date.AddDays(1));
                        }
                        else if (item.CustomAttributes.Any(a => a.AttributeType == typeof(ContainAttribute)))
                        {
                            var array = (IList)propertyVal;
                            source = source.Where($"{item.Name}.Any(a => @0.Contains(a))", array);
                        }
                        else if (item.CustomAttributes.Any(a => a.AttributeType == typeof(SortAttribute)))
                        {
                            string[] sort = propertyVal.ToString().Split(", ");
                            if (sort.Length == 2)
                            {
                                source = sort[1].Equals("asc")
                                    ? source.OrderBy(sort[0])
                                    : source.OrderBy(sort[0] + " descending");
                            }
                            else
                            {
                                source = source.OrderBy(sort[0]);
                            }
                        }
                        //else
                        //{
                        //    source = source.Where($"{item.Name} = @0", propertyVal);
                        //}
                    }
                }
                return source;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public static (int, IQueryable<TResult>) PagingIQueryable<TResult>(this IQueryable<TResult> source, int page, int size,
            int limitPaging, int defaultPaging)
        {
            try
            {
                if (size > limitPaging)
                {
                    size = limitPaging;
                }
                if (size < 1)
                {
                    size = defaultPaging;
                }
                if (page < 1)
                {
                    page = 1;
                }
                int total = source == null ? 0 : source.Count();
                IQueryable<TResult> results = source
                    .Skip((page - 1) * size)
                    .Take(size);
                return (total, results);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
