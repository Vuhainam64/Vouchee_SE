using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class UserDTO
    {
        public string? name { get; set; }
        public string? phoneNumber { get; set; }
        public string? image { get; set; }
    }

    public class CreateUserDTO
    {
        [Required(ErrorMessage = "Email là cần thiết")]
        public string? email { get; set; }
        [Required(ErrorMessage = "Mật khẩu là cần thiết")]
        public string? hashPassword { get; set; }
        public Guid? supplierId { get; set; }
        public DateTime? createDate = DateTime.Now;
        [Required(ErrorMessage = "Tên là cần thiết")]
        public string? name { get; set; }
        public RoleEnum role { get; set; }
    }

    public class UpdateUserDTO : UserDTO
    {
        public DateTime? updateDate = DateTime.Now;
    }
    public class UpdateUserRoleDTO
    {
        public Guid userId { get; set; }
        public RoleEnum role { get; set; }
        public Guid? supplierId { get; set; }
        public DateTime? updateDate = DateTime.Now;
    }

    public class GetUserDTO : UserDTO
    {
        public Guid? id { get; set; }
        public string? email { get; set; }
        public string? role { get; set; }
        public string? description { get; set; }

        /*public string? bankAccount { get; set; }
        public string? bankNumber { get; set; }
        public string? bankName { get; set; }*/
        public bool? isActive { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
        public string SupplierName {  get; set; }
        public int vpoint { get; set; }

        public GetBuyerWallet? buyerWallet { get; set; }
        public GetSellerWallet? sellerWallet { get; set; }
        public GetSupplierWallet? supplierWallet { get; set; }
    }
    
    public class GetDetailUserDTO : GetUserDTO
    {
        public GetDetailUserDTO()
        {
            orders = [];
            //vouchers = [];
            carts = [];
            //notificationFromUser = [];
            notificationToUser = [];
        }

        public GetBuyerWallet? buyerWallet { get; set; }
        public GetSellerWallet? sellerWallet { get; set; }
        public GetSupplierWallet? supplierWallet { get; set; }
        public virtual ICollection<CartDTO> carts { get; set; }
        //public virtual ICollection<GetVoucherDTO> vouchers { get; set; }
        public virtual ICollection<GetOrderDTO>? orders { get; set; }
        public virtual ICollection<GetNotificationDTO> notificationToUser { get; set; }
        //[InverseProperty(nameof(Notification.Sender))]
        //public virtual ICollection<GetNotificationDTO> notificationFromUser { get; set; }
    }

    public class UpdateUserBankDTO
    {
        public string? bankAccount { get; set; }
        public string? bankNumber { get; set; }
        public string? bankName { get; set; }

        public DateTime? updateDate = DateTime.Now;
    }
}
