using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class AccountTransactionDTO
    {
        public Guid? fromUserId { get; set; }
        public Guid? toUserId { get; set; }
        public int? amount { get; set; }
        public string? type { get; set; }
    }

    public class GetAccountTransactionDTO : AccountTransactionDTO
    {
        public Guid? id { get; set; }

        public virtual GetWalletTransactionDTO? walletTransaction { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }
}
