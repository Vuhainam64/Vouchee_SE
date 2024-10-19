using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Repositories.IRepos
{
    public interface IAddressRepository : IBaseRepository<Address>
    {
        public Address GetLocalAddress(decimal lon, decimal lat);
        void Attach(Address existedAddress);
        public Address GetAddress(decimal? lon, decimal? lat);
    }
}
