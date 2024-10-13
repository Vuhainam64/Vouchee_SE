using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Data.Repositories.Repos
{
    public class AddressRepository : BaseRepository<Address>, IAddressRepository
    {
        private readonly BaseDAO<Address> _addressDAO;

        public AddressRepository() => _addressDAO = BaseDAO<Address>.Instance;


        public Address GetAddress(decimal? lon, decimal? lat)
        {
            try
            {
                Address address = _addressDAO.GetFirstOrDefaultAsync(filter: x => x.Lon == lon && x.Lat == lat).Result;
                return address;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
