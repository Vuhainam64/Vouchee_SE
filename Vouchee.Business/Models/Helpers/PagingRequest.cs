using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models.Helpers
{
    public class PagingRequest
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public SortOrder OrderType { get; set; } = SortOrder.Ascending;
    }
}
