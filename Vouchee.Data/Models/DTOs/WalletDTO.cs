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
        public Guid Id { get; set; }
        public int balance { get; set; }

        public required string status { get; set; }
        public required DateTime createDate { get; set; }
        public Guid createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }

    public class GetWalletDTO : WalletDTO
    {

    }

    public class GetSellerWallet : GetWalletDTO
    {
        public GetSellerWallet()
        {
            walletTransactions = [];
        }

        public virtual ICollection<GetSellerWalletTransaction> walletTransactions { get; set; }
    }

    public class GetBuyerWallet : WalletDTO
    {
        public GetBuyerWallet()
        {
            walletTransactions = [];
        }

        public virtual ICollection<GetBuyerWalletTransactionDTO> walletTransactions { get; set; }
    }
}
