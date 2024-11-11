using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Filters
{
    public class TopUpRequestFilter
    {
        public TopUpRequestStatusEnum? status { get; set; }
    }
}
