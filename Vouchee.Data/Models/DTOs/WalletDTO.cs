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
    public class WalletDTO
    {
        public WalletDTO()
        {
            sellerWalletTransactions = [];
            buyerWalletTransactions = [];
        }

        public int balance { get; set; }

        public required string status { get; set; }
        public required DateTime createDate { get; set; }
        public Guid createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetWalletTransactionDTO> sellerWalletTransactions { get; set; }

        public virtual ICollection<GetWalletTransactionDTO> buyerWalletTransactions { get; set; }
    }

    public class GetWalletDTO : WalletDTO
    {
        public Guid Id { get; set; }
    }
}
