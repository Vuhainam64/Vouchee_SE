using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Vouchee.Business.Models.Constants.Enum;

namespace Vouchee.Business.Models.DTOs
{
    public class RoleDTO
    {
        public string? name { get; set; }
        public string? description { get; set; }
    }

    public class CreateRoleDTO : RoleDTO
    {
        [JsonIgnore]
        public ObjectStatusEnum? status { get; set; }

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

    public class RoleFilter
    {
        public string? name { get; set; }

        public string? status { get; set; }
        public DateTime? createDate { get; set; }
        public Guid? createBy { get; set; }
        public DateTime? updateDate { get; set; }
        public Guid? updateBy { get; set; }
    }
}
