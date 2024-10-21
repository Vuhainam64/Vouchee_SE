using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;

namespace Vouchee.Data.Repositories.IRepos
{
    public interface IUserRepository : IBaseRepository<User>
    {
        // GET
        public Task<User> GetUserByEmail(string email);

        public Task<User> LoginWithPhone(LoginByPhoneNumberDTO loginByPhoneNumberDTO);

        public Task<User> LoginWithEmail(LoginByEmailDTO loginByEmailDTO);

        // UPDATE
        // public Task<bool> StoreRefreshToken(User user, string refreshToken, DateTime dateTime);
    }
}
