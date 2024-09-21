using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models
{
    public class DynamicResponseModel<T>
    {
        public PagingMetaData? PagingMetaData { get; set; }
        public List<T>? Results { get; set; }
    }
    public class PagingMetaData
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
    }
}
