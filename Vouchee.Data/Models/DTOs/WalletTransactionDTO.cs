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
    }

    public class CreateWalletTransactionDTO : WalletTransactionDTO
    {
        public string? status = WalletTransactionStatusEnum.PENDING.ToString();
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateWalletTransactionDTO : WalletTransactionDTO
    {
        public WalletTransactionStatusEnum status { get; set; }
        public DateTime updateDate = DateTime.Now;
    }

    public class GetWalletTransactionDTO : WalletTransactionDTO
    {
        public Guid? id { get; set; }

        public Guid? partnerTransactionId { get; set; }
        public string? orderId { get; set; }
        public Guid? sellerWalletId { get; set; }
        public Guid? buyerWalletId { get; set; }

        public int? beforeBalance { get; set; }
        public int? afterBalance { get; set; }

        public string? note { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }

    public class GetSellerWalletTransaction : GetWalletTransactionDTO
    {
    }

    public class GetBuyerWalletTransactionDTO : GetWalletTransactionDTO
    {
        public string? topUpRequestId { get; set; }
    }
}
