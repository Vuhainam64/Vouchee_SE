using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vouchee.Data.Models.Filters
{
    public class UserFilter
    {
        public string? description { get; }
        public string? lastName { get; }
        public string? firstName { get; }
        public string? phoneNumber { get; }
        public string? email { get; }
        public string? gender { get; }
        public string? dateOfBirth { get; }
        public string? city { get; }
        public string? district { get; }

        public string? status { get; }
        public DateTime? createDate { get; }
        public Guid? createBy { get; }
        public DateTime? updateDate { get; }
        public Guid? updateBy { get; }
    }

}
