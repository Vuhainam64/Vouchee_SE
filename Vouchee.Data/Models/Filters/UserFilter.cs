using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;

namespace Vouchee.Data.Models.Filters
{
    public class UserFilter
    {
        public string? name { get; set; }
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public RoleEnum? role { get; set; }

        public UserStatusEnum? status { get; set; }
        public bool? isActive { get; set; }
    }

}
