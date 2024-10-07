using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models
{
    public class DynamicResponseModel<T>
    {
        public MetaData? metaData { get; set; }
        public List<T>? results { get; set; }
    }
    public class MetaData
    {
        public int page { get; set; }
        public int size { get; set; }
        public int total { get; set; }
    }
}
