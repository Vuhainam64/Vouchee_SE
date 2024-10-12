using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models
{
    public class PagingRequest
    {
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 20;
        //public SortOrder orderType { get; set; } = SortOrder.Ascending;
        //public string keySearch { get; set; } = "";
        //public string collName { get; set; } = "";
    }
}