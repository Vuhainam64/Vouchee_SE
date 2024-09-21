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

        public Guid? id { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }

        public virtual ICollection<GetUserDTO>? users { get; set; }
    }
}
