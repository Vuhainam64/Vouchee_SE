using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Data.Repositories.Repos
{
    public class VoucherRepository : BaseRepository<Voucher>, IVoucherRepository
    {
        private readonly BaseDAO<Voucher> _voucherDAO;
        public VoucherRepository() => _voucherDAO = BaseDAO<Voucher>.Instance;
        public void Attach(Address existedAddress)
        {
            _voucherDAO.Attach(existedAddress);
        }
        public void Attach(Category existedCategory)
        {
            _voucherDAO.Attach(existedCategory);
        }
    }
}
