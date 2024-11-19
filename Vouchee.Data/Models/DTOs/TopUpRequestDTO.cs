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
    public class TopUpRequestDTO
    {
        public int? amount { get; set; }
    }

    public class CreateTopUpRequestDTO : TopUpRequestDTO
    {
        public string? status = TopUpRequestStatusEnum.PENDING.ToString();
        public DateTime createDate = DateTime.Now;
    }

    public class UpdateTopUpRequestDTO : TopUpRequestDTO
    {
        public DateTime? updateDate = DateTime.Now;
    }

    public class GetTopUpRequestDTO : TopUpRequestDTO
    {
        public string? id { get; set; }

        public virtual GetSellerWalletTransaction? walletTransaction { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }
}
