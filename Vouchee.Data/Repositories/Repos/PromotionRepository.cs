using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Data.Repositories.Repos
{
    public class PromotionRepository : BaseRepository<Promotion>, IPromotionRepository
    {
        private readonly BaseDAO<Promotion> _promotionDAO;
        private readonly BaseDAO<Voucher> _voucherDAO;

        public PromotionRepository()
        {
            _voucherDAO = BaseDAO<Voucher>.Instance;
            _promotionDAO = BaseDAO<Promotion>.Instance;
        }

        public async Task<IEnumerable<Promotion>> GetAvailableByVoucherId(Guid voucherId)
        {
            DateTime dateTime = DateTime.Now;
            var promotions = _voucherDAO.GetByIdAsync(voucherId, includeProperties: x => x.Include(x => x.Promotions))
                                                    .Result
                                                    .Promotions;

            if (promotions.Count != 0)
            {
                return promotions.Where(x => x.StartDate <= dateTime & dateTime <= x.EndDate);
            }

            return null;
        }

        public async Task<IEnumerable<Promotion>> GetPromotionByBuyerId(Guid buyerId) => await _promotionDAO.GetWhereAsync(
                                                                                    filter: x => x.CreateBy.Equals(buyerId) &&
                                                                                            x.StartDate <= DateTime.Now &&
                                                                                            x.EndDate >= DateTime.Now);
    }
}
