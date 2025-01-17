﻿using shortid.Configuration;
using shortid;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class WithdrawRequestDTO
    {
        public int amount { get; set; }
    }

    public class CreateWithdrawRequestDTO : WithdrawRequestDTO
    {
  
    }

    public class GetWithdrawRequestDTO : WithdrawRequestDTO 
    {
        public string? id { get; set; }
        public Guid? userId { get; set; }
        public string? status { get; set; }
        public string? type { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? updateId { get; set; }
        public string? WalletType { get; set; }
        public DateTime? updateDate { get; set; }
        public string? note { get; set; }
        //public DateTime? exprireTime => createDate?.AddMinutes(2);
        public virtual GetWalletTransactionDTO? withdrawWalletTransaction { get; set; }
    }
    public class MonthlyTotalDTO
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int TotalAmount { get; set; }
    }

}
