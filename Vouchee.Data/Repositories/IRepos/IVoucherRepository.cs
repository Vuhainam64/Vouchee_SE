using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Repositories.IRepos
{
    public interface IVoucherRepository : IBaseRepository<Voucher>
    {
        void Attach(Address existedAddress);
        void Attach(Category existedCategory);
    }
}
