using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Exceptions;
using Vouchee.Business.Models;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Services.Extensions.Filebase;
using Vouchee.Data.Helpers;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.Business.Services.Impls
{
    public class CartService : ICartService
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CartService(IVoucherRepository voucherRepository,
                            IUserRepository userRepository, 
                            ICartRepository cartRepository, 
                            IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _userRepository = userRepository;
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<bool> AddItemAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            Guid userId = Guid.Parse(thisUserObj.userId);
            Voucher existedVoucher = await _voucherRepository.FindAsync(voucherId);
            if (existedVoucher == null)
            {
                throw new NotFoundException("Không tìm thầy voucher này");
            }

            User userInstance = await GetCurrentUser(userId);

            var haveVoucher = userInstance.Carts.FirstOrDefault(x => x.VoucherId == voucherId);
            if (haveVoucher != null)
            {
                haveVoucher.Quantity += 1;
                haveVoucher.UpdateBy = Guid.Parse(thisUserObj.userId);
                haveVoucher.UpdateDate = DateTime.Now;
            }
            else
            {
                userInstance.Carts.Add(new()
                {
                    VoucherId = voucherId,
                    CreateBy = Guid.Parse(thisUserObj.userId),
                    CreateDate = DateTime.Now,
                    Quantity = 1,
                    Status = ObjectStatusEnum.ACTIVE.ToString()
                });
            }

            return await _userRepository.UpdateAsync(userInstance);
        }

        public Task<bool> DecreaseQuantityAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public async Task<CartDTO> GetCartsAsync(PagingRequest pagingRequest, CartFilter cartFilter, ThisUserObj thisUserObj)
        {
            Guid userId = Guid.Parse(thisUserObj.userId);

            User? user = await GetCurrentUser(userId);

            CartDTO cartDTO = new()
            {
                TotalPrice = user.Carts
                                .Where(c => c.Voucher != null)
                                .Sum(c => c.Voucher.SellPrice * (c.Quantity ?? 1)),
                TotalQuantity = user.Carts
                                .Where(c => c.Quantity != null)
                                .Sum(c => c.Quantity ?? 0),
            };

            cartDTO.vouchers = user.Carts
                .Where(c => c.Voucher != null) // Only include carts with vouchers
                .Select(c => _mapper.Map<VoucherDTO>(c.Voucher)) // Map the Voucher to VoucherDTO
                .ToList();

            cartDTO.DiscountPrice = 0;
            cartDTO.FinalPrice = cartDTO.TotalPrice;

            return cartDTO;
        }

        public Task<bool> IncreaseQuantityAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveItemAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateQuantityAsync(Guid voucherId, int quantity, ThisUserObj thisUserObj)
        {
            throw new NotImplementedException();
        }

        private async Task<User> GetCurrentUser(Guid userId)
        {
            User? userInstance = null;
            IQueryable<User> users = _userRepository.CheckLocal();

            if (users != null && users.Count() != 0)
            {
                userInstance = users.FirstOrDefault(x => x.Id == userId);
            }

            if (userInstance == null)
            {
                userInstance = await _userRepository.GetByIdAsync(userId, includeProperties: x => x.Include(x => x.Carts)
                                                                                                    .ThenInclude(x => x.Voucher));
            }

            return userInstance;
        }
    }
}
