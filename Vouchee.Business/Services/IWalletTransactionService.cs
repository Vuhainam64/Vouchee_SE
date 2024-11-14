﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services
{
    public interface IWalletTransactionService
    {
        // READ
        public Task<DynamicResponseModel<GetSellerWalletTransaction>> GetWalletTransactionsAsync(PagingRequest pagingRequest,
                                                                                                    WalletTransactionFilter walletTransactionFilter,
                                                                                                    ThisUserObj currentUser);
    }
}