﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Filters
{
    public class VoucherCodeFilter
    {
        public VoucherCodeStatusEnum? status { get; set; }

        public DateOnly? startDate { get; set; }
        public DateOnly? endDate { get; set; }

        public string? Title { get; set; }
    }
    public class VoucherCodeConvertFilter
    {
        public VoucherCodeStatusEnum? status { get; set; }

        public Guid? UpdateId { get; set; }
    }
}
