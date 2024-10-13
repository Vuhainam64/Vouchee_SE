using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Services
{
    public interface ITestService
    {
        public Task<Voucher> CreateVoucher(TestCreateVoucherDTO createVoucherDTO, Guid voucherTypeId, Guid supplierID);
    }
}
