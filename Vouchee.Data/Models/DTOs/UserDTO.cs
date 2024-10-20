using System.ComponentModel.DataAnnotations;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Business.Models.DTOs
{
    public class UserDTO
    {
        public Guid? roleId { get; set; }

        public string? description { get; set; }
        public string? name { get; set; }
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? gender { get; set; }
        public string? dateOfBirth { get; set; }
        public string? city { get; set; }
        public string? district { get; set; }
        public string? address { get; set; }
        public string? image { get; set; }
        public string? bankName { get; set; }
        public string? bankAccount { get; set; }
        public string? refreshToken { get; set; }
        public DateTime? refreshTokenExpirationDate { get; set; }
    }

    public class CreateUserDTO : UserDTO
    {
        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateUserDTO : UserDTO
    {
        public UserStatusEnum status { get; set; }
        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetUserDTO : UserDTO
    {
        public GetUserDTO()
        {
            orders = new HashSet<GetOrderDTO>();
            vouchers = new HashSet<GetNewestVoucherDTO>();
            carts = new HashSet<CartDTO>();
        }

        public Guid? id { get; set; }

        public string? roleName { get; set; }
        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<CartDTO> carts { get; set; }
        public virtual ICollection<GetNewestVoucherDTO> vouchers { get; set; }
        public virtual ICollection<GetOrderDTO>? orders { get; set; }
    }

    public class RegisterDTO
    {
        public string? phoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string? email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string? hashPassword { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string? name { get; set; }
    }

    public class LoginByEmailDTO
    {
        public string? email { get; set; }
        public string? hashPassword { get; set; }
    }

    public class LoginByPhoneNumberDTO
    {
        public string? phoneNumber { get; set; }
        public string? password { get; set; }
    }
}
