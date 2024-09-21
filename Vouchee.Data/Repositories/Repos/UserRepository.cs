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
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly BaseDAO<User> _userDAO;

        public UserRepository() => _userDAO = BaseDAO<User>.Instance;

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                User user = await _userDAO.GetFirstOrDefaultAsync(x => string.Equals(x.Email, email, StringComparison.OrdinalIgnoreCase));
                if (user != null)
                {
                    return user;
                }
                return null;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
