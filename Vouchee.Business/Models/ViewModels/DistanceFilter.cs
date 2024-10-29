using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models.ViewModels
{
    public class DistanceFilter
    {
        public decimal lon { get; set; }
        public decimal lat { get; set; }
        public int numberOfVoucher { get; set; } = 5;
        public int numberOfAddress { get; set; } = 10;
    }
}