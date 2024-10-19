using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Repositories.IRepos;

namespace Vouchee.Data.Repositories.Repos
{
    public class BrandRepository : BaseRepository<Brand>, IBrandRepository
    {
        private readonly BaseDAO<Brand> _brandDAO;
        public BrandRepository() => _brandDAO = BaseDAO<Brand>.Instance;

        public void Attach(Address address)
        {
            _brandDAO.Attach(address);
        }

        public Address GetAddress(decimal lon, decimal lat)
        {
            return _brandDAO.GetLocalAddress(lon, lat);
        }
    }
}
