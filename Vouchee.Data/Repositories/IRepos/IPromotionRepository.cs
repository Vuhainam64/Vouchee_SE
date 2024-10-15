using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Repositories.IRepos
{
    public interface IPromotionRepository : IBaseRepository<Promotion>
    {
        public Task<IEnumerable<Promotion>> GetPromotionByBuyerId(Guid buyerId);
        public Task<IEnumerable<Promotion>> GetAvailableByVoucherId(Guid voucherId);
    }
}
