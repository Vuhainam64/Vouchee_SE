using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Helpers.DateAnnotations;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.DTOs
{
    public class ModalPromotionDTO
    {
        public Guid? id { get; set; }

        public string? name { get; set; }
        public string? description { get; set; }
        public DateTime? startDate { get; set; }
        public DateTime? endDate { get; set; }
        public int? stock { get; set; }
        public string? code { get; set; }
        public int? percentDiscount { get; set; }
        public int? moneyDiscount { get; set; }
        public int? maxMoneyToDiscount { get; set; }
        public int? minMoneyToAppy { get; set; }
        public int? requiredQuantity { get; set; }

        public bool? isActive { get; set; }
        public string? status { get; set; }
    }

    public class GetModalPromotionDTO : ModalPromotionDTO
    {

    }

    public class CartModalPromotionDTO : ModalPromotionDTO
    {
        public bool isAppliable { get; set; } = false;
        public string? note { get; set; }
    }

    public class GetDetailModalPromotionDTO : ModalPromotionDTO
    {
        public GetDetailModalPromotionDTO()
        {
            modals = [];
        }

        public virtual ICollection<GetModalDTO> modals { get; set; }
    }

    public class CreateModalPromotionDTO 
    {
        public CreateModalPromotionDTO()
        {
            modal_id = [];
        }

        [Required(ErrorMessage = "Tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự.")]
        public string? name { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        public string? description { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống.")]
        [DataType(DataType.DateTime, ErrorMessage = "Định dạng ngày không hợp lệ.")]
        public DateTime? startDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống.")]
        [DataType(DataType.DateTime, ErrorMessage = "Định dạng ngày không hợp lệ.")]
        [DateGreaterThan("startDate", ErrorMessage = "Ngày kết thúc phải lớn hơn ngày bắt đầu.")]
        public DateTime? endDate { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải là một số không âm.")]
        public int? stock { get; set; }

        [StringLength(50, ErrorMessage = "Mã không được vượt quá 50 ký tự.")]
        public string? code { get; set; }

        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100.")]
        public int? percentDiscount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số tiền giảm giá phải là một số không âm.")]
        public int? moneyDiscount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số tiền giảm giá tối đa phải là một số không âm.")]
        public int? maxMoneyToDiscount { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số tiền tối thiểu để áp dụng phải là một số không âm.")]
        public int? minMoneyToAppy { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số lượng yêu cầu phải lớn hơn 0.")]
        public int? requiredQuantity { get; set; }

        public IList<Guid> modal_id { get; set; }

        public bool isActive { get; set; } = false;

        public string status = ObjectStatusEnum.NONE.ToString();
    }

    public class UpdateModalPromotionDTO
    {

    }

    //public partial class ModalPromotion
    //{
    //    public ModalPromotion()
    //    {
    //        OrderDetails = [];
    //        Modals = [];
    //    }

    //    [InverseProperty(nameof(OrderDetail.ModalPromotion))]
    //    public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    //    [InverseProperty(nameof(Modal.ModalPromotion))]
    //    public virtual ICollection<Modal> Modals { get; set; }

    //    [Key]
    //    public Guid Id { get; set; }

    //    public required string Name { get; set; }
    //    public string? Description { get; set; }
    //    [Column(TypeName = "datetime")]
    //    public DateTime? StartDate { get; set; }
    //    [Column(TypeName = "datetime")]
    //    public DateTime? EndDate { get; set; }
    //    public int? Stock { get; set; }
    //    public string? Code { get; set; }
    //    public int? PercentDiscount { get; set; }
    //    public int? MoneyDiscount { get; set; }
    //    public int? MaxMoneyToDiscount { get; set; }
    //    public int? MinMoneyToAppy { get; set; }
    //    public int? RequiredQuantity { get; set; }

    //    public required string Status { get; set; }
    //    [Column(TypeName = "datetime")]
    //    public DateTime? CreateDate { get; set; } = DateTime.Now;
    //    public Guid? CreateBy { get; set; }
    //    [Column(TypeName = "datetime")]
    //    public DateTime? UpdateDate { get; set; }
    //    public Guid? UpdateBy { get; set; }
    //}
}
