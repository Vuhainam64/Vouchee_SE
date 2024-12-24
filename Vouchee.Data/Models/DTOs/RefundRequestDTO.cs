using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Models.DTOs
{
    public class RefundRequestDTO
    {
        public RefundRequestDTO()
        {
            images = [];
        }

        public IList<string> images { get; set; }
        public Guid? voucherCodeId { get; set; }
        public string? content { get; set; }
        [Range(-180, 180, ErrorMessage = "Kinh dộ phải nằm giữa -180 và 180.")]
        public decimal lon { get; set; }
        [Range(-90, 90, ErrorMessage = "Vĩ dộ phải nằm giữa -90 và 90.")]
        public decimal lat { get; set; }
    }

    public class CreateRefundRequestDTO : RefundRequestDTO
    {
        public RefundRequestStatusEnum? status = RefundRequestStatusEnum.PENDING;
        public DateTime? createDate = DateTime.Now;
    }

    public class UpdateRefundRequestDTO : RefundRequestDTO
    {
        public DateTime? updateDate = DateTime.Now;
    }

    public class GetRefundRequestDTO : RefundRequestDTO
    {
        public GetRefundRequestDTO()
        {
            medias = [];
        }

        public Guid? id { get; set; }

        public virtual ICollection<GetMediaDTO> medias { get; set; }
        public virtual GetWalletTransactionDTO? walletTransaction { get; set; }
        public virtual GetVoucherCodeDTO? voucherCode { get; set; }

        public string? supplierName { get; set; }
        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public DateTime? expireTime => createDate?.AddMinutes(15);
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }
}
