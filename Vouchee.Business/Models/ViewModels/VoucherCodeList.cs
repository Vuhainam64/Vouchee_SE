using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models.ViewModels
{
    public class VoucherCodeList
    {
        public VoucherCodeList() 
        {
            voucherCodeIds = new List<Guid>();
        }

        public IList<Guid> voucherCodeIds { get; set; }
    }
}
