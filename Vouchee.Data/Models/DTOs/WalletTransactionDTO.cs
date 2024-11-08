using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.DTOs
{
    public class WalletTransactionDTO
    {
        public int amount { get; set; }

        public string? status = WalletTransactionStatus.PENDING.ToString();
    }

    public class CreateWalletTransactionDTO : WalletTransactionDTO
    {
        public DateTime createDate = DateTime.Now;
    }

    public class GetWalletTransactionDTO
    {
        public Guid id { get; set; }
    }
}
