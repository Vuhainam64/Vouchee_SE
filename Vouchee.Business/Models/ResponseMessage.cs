using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models
{
    public class ResponseMessage<T>
    {
        public string? message { get; set; }
        public bool? result { get; set; }
        public T? value { get; set; }
    }
}
