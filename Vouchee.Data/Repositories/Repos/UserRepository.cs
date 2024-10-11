using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Helpers;
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.DTOs;
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
                User user = await _userDAO.GetFirstOrDefaultAsync(
                                        filter: x => x.Email.ToLower().Equals(email.ToLower()),
                                        includeProperties: query => query.Include(x => x.Role)
                                    );
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

        public async Task<User> LoginWithEmail(LoginByEmailDTO loginByEmailDTO)
        {
            try
            {
                User user = await _userDAO.GetFirstOrDefaultAsync(
                                        filter: x => x.Email.ToLower().Equals(loginByEmailDTO.email.ToLower()) 
                                                        && x.HashPassword.Equals(loginByEmailDTO.hashPassword),
                                        includeProperties: query => query.Include(x => x.Role)
                                    );
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

        public async Task<User> LoginWithPhone(LoginByPhoneNumberDTO loginByPhoneNumberDTO)
        {
            try
            {
                User user = await _userDAO.GetFirstOrDefaultAsync(
                                        filter: x => x.PhoneNumber.Equals(loginByPhoneNumberDTO.phoneNumber)
                                                        && x.HashPassword.Equals(loginByPhoneNumberDTO.password),
                                        includeProperties: query => query.Include(x => x.Role)
                                    );
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

        public async Task<bool> StoreRefreshToken(User user, string refreshToken, DateTime dateTime)
        {
            try
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpirationDate = dateTime;

                return _userDAO.UpdateAsync(user).Result;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}