using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Models.ViewModels
{
    public class DistanceFilter
    {
        public required decimal lon { get; set; }
        public required decimal lat { get; set; }
        public int numberOfAddress { get; set; }
    }
}