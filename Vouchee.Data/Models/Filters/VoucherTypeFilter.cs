﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Filters
{
    public class VoucherTypeFilter
    {
        public string? title { get; set; }
        public bool? isActive { get; set; }
    }
}