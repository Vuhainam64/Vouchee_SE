using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Business.Models.DTOs
{
    public class RoleDTO
    {
        public string? name { get; set; }
        public string? description { get; set; }
    }

    public class CreateRoleDTO : RoleDTO
    {

        public DateTime? createDate = DateTime.Now;
        public Guid? createBy { get; set; }
    }

    public class UpdateRoleDTO : RoleDTO
    {
        public ObjectStatusEnum? status { get; set; }

        public DateTime? updateDate = DateTime.Now;
        public Guid? updateBy { get; set; }
    }

    public class GetRoleDTO : RoleDTO
    {
        public GetRoleDTO()
        {
            users = new HashSet<GetUserDTO>();
        }

        public virtual ICollection<GetUserDTO> users { get; }

        public Guid id { get; }

        public string? status { get; }
        public DateTime? createDate { get; }
        public Guid? createBy { get; }
        public DateTime? updateDate { get; }
        public Guid? updateBy { get; }
    }
}
