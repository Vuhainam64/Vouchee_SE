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
using Vouchee.Data.Models.Constants.Enum.Other;
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

            Guid userId = thisUserObj.userId;

            User userInstance = await GetCurrentUser(userId);

            if (userInstance.Carts.Count() > 5)
            {
                throw new ConflictException("Bạn chỉ có thể mua tối đa 5 loại voucher một lúc");
            }

            Voucher existedVoucher = await _voucherRepository.FindAsync(voucherId);
            if (existedVoucher == null)
            {
                throw new NotFoundException("Không tìm thầy voucher này");
            }

            var haveVoucher = userInstance.Carts.FirstOrDefault(x => x.VoucherId == voucherId);
                if (haveVoucher != null)
                {
                    haveVoucher.Quantity += 1;
                    haveVoucher.UpdateBy = thisUserObj.userId;
                    haveVoucher.UpdateDate = DateTime.Now;
                }
                else
                {
                    userInstance.Carts.Add(new()
                    {
                        VoucherId = voucherId,
                        CreateBy = thisUserObj.userId,
                        CreateDate = DateTime.Now,
                        Quantity = 1,
                        Status = ObjectStatusEnum.ACTIVE.ToString()
                    });
                }

            var result = await _userRepository.UpdateAsync(userInstance);

            var user = GetCurrentUser(thisUserObj.userId);

            return result;
        }

        public async Task<bool> DecreaseQuantityAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            Guid userId = thisUserObj.userId;   

            // Fetch the current user
            User? user = await GetCurrentUser(userId);

            // Find the cart item with the specified voucher
            var cartItem = user.Carts.FirstOrDefault(c => c.VoucherId == voucherId);

            if (cartItem != null)
            {
                // Increase the quantity
                cartItem.Quantity -= 1;
                cartItem.UpdateBy = userId;
                cartItem.UpdateDate = DateTime.Now;

                // Update the user with the modified cart
                return await _userRepository.UpdateAsync(user);
            }

            throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
        }

        public async Task<CartDTO> GetCartsAsync(ThisUserObj thisUserObj)
        {
            Guid userId = thisUserObj.userId;

            User? user = await GetCurrentUser(userId);

            CartDTO cartDTO = new()
            {
                TotalPrice = user.Carts
                                .Where(c => c.Voucher != null)
                                .Sum(c => c.Voucher.SellPrice * c.Quantity),
                TotalQuantity = user.Carts
                                .Sum(c => c.Quantity),
            };

            // Map CartVoucherDTO including quantity for each voucher in the cart
            cartDTO.vouchers = user.Carts
                .Select(c => new CartVoucherDTO
                {
                    quantity = c.Quantity, // Set the quantity for each cart item
                    id = c.Voucher.Id,
                    title = c.Voucher.Title,
                    originalPrice = c.Voucher.OriginalPrice,
                    sellPrice = c.Voucher.SellPrice,
                    image = c.Voucher.Medias.FirstOrDefault(x => x.Type == MediaEnum.ADVERTISEMENT.ToString()).Url
                })
                .ToList();

            cartDTO.DiscountPrice = 0;
            cartDTO.FinalPrice = cartDTO.TotalPrice;

            return cartDTO;
        }


        public async Task<bool> IncreaseQuantityAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            Guid userId = thisUserObj.userId;

            // Fetch the current user
            User? user = await GetCurrentUser(userId);

            // Find the cart item with the specified voucher
            var cartItem = user.Carts.FirstOrDefault(c => c.VoucherId == voucherId);

            if (cartItem != null)
            {
                if (cartItem.Quantity > 19)
                {
                    throw new ConflictException("Bạn chỉ có thể mua tối đa 20 code mỗi loại voucher");
                }

                // Increase the quantity
                cartItem.Quantity += 1;
                cartItem.UpdateBy = userId;
                cartItem.UpdateDate = DateTime.Now;

                // Update the user with the modified cart
                return await _userRepository.UpdateAsync(user);
            }

            throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
        }


        public async Task<bool> RemoveItemAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            Guid userId = thisUserObj.userId;

            // Fetch the current user
            User? user = await GetCurrentUser(userId);

            // Find the cart item with the specified voucher
            var cartItem = user.Carts.FirstOrDefault(c => c.VoucherId == voucherId);

            if (cartItem != null)
            {
                // Remove the item from the cart
                user.Carts.Remove(cartItem);

                // Update the user with the modified cart
                return await _userRepository.UpdateAsync(user);
            }

            throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
        }


        public async Task<bool> UpdateQuantityAsync(Guid voucherId, int quantity, ThisUserObj thisUserObj)
        {
            if (quantity > 20)
            {
                throw new ConflictException("Bạn chỉ có thể mua tối đa 20 code mỗi loại voucher");
            }

            Guid userId = thisUserObj.userId;

            User? user = await GetCurrentUser(userId);

            var cartItem = user.Carts.FirstOrDefault(c => c.VoucherId == voucherId);

            if (cartItem != null)
            {

                cartItem.Quantity = quantity;
                cartItem.UpdateBy = userId; 
                cartItem.UpdateDate = DateTime.Now;

                return await _userRepository.UpdateAsync(user);
            }

            throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
        }

        public async Task<User> GetCurrentUser(Guid userId)
        {
            User? userInstance = null;

            // Check local data first
            IQueryable<User> users = _userRepository.CheckLocal();

            if (users != null && users.Any())
            {
                userInstance = users.FirstOrDefault(x => x.Id == userId);
            }

            // If not found locally, load from database with vouchers
            if (userInstance == null || !userInstance.Carts.Any(cart => cart.Voucher != null))
            {
                userInstance = await _userRepository.GetByIdAsync(userId, includeProperties: x => x.Include(x => x.Carts)
                                                                                                   .ThenInclude(x => x.Voucher)
                                                                                                   .ThenInclude(x => x.Medias));
            }

            return userInstance;
        }
    }
}
