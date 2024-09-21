using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Business.Models.DTOs
{
    public class UserDTO
    {
        public Guid? roleId { get; set; }

        public string? description { get; set; }
        public string? lastName { get; set; }
        public string? firstName { get; set; }
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? gender { get; set; }
        public string? dateOfBirth { get; set; }
        public string? city { get; set; }
        public string? district { get; set; }
        public string? address { get; set; }
        public string? bankName { get; set; }
        public string? bankAccount { get; set; }
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
        }

        public virtual ICollection<GetOrderDTO> orders { get; }

        public Guid id { get;  }

        public string? status { get; }
        public DateTime? createDate { get; }
        public Guid? createBy { get;}
        public DateTime? updateDate { get; }
        public Guid? updateBy { get; }
    }
}
