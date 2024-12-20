﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Sort;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Filters
{
    public class OrderFilter
    {
        public OrderStatusEnum? status { get; set; }

        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
    }
}
