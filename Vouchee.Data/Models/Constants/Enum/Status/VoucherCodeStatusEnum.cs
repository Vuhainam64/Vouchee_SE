﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Constants.Enum.Status
{
    public enum VoucherCodeStatusEnum
    {
        PENDING,
        UNUSED,
        USED,
        EXPIRED,
        VIOLENT,
        NONE,
        CONVERTING,
        SUSPECTED
    }
}
