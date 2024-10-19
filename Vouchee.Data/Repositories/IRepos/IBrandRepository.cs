using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Repositories.IRepos
{
    public interface IBrandRepository : IBaseRepository<Brand>
    {
        public void Attach(Address address);
        public Address GetAddress(decimal lon, decimal lat);
    }
}
