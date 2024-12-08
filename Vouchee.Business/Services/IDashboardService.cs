﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Business.Services
{
    public interface IDashboardService
    {
        // CREATE

        // READ
        public Task<dynamic> GetUserDashboard();
        public Task<dynamic> GetVoucherDashboard();
        public Task<dynamic> GetOrderDashboard(int? year);
        public Task<dynamic> GetTransactionDashboard();
        // UPDATE

        // DELETE
    }
}
