using AutoMapper;
using AutoMapper.QueryableExtensions;
using Google.Api;
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
        private readonly IMapper _mapper;

        private User? _currentUser;

        public CartService(IVoucherRepository voucherRepository,
                            IUserRepository userRepository, 
                            IMapper mapper)
        {
            _voucherRepository = voucherRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<CartDTO> GetCartsAsync(ThisUserObj thisUserObj)
        {
            _currentUser = _userRepository.CheckLocal().First(x => x.Id == thisUserObj.userId);
            
            CartDTO cartDTO = new();

            foreach (var cartItem in _currentUser.Carts)
            {
                var currentVoucher = cartItem.Voucher;

                if (currentVoucher != null)
                {
                    var sellerCart = cartDTO.sellers.FirstOrDefault(s => s.id == currentVoucher?.SellerID);
                    if (sellerCart == null)
                    {
                        sellerCart = new SellerCartDTO
                        {
                            id = currentVoucher?.SellerID,
                            name = currentVoucher?.Seller?.Name,
                            image = currentVoucher?.Seller?.Image,
                            vouchers = new List<CartVoucherDTO>()
                        };
                        cartDTO.sellers.Add(sellerCart);
                    }

                    sellerCart.vouchers.Add(new CartVoucherDTO
                    {
                        id = cartItem.Voucher?.Id,
                        originalPrice = cartItem.Voucher?.OriginalPrice,
                        sellPrice = cartItem.Voucher?.SellPrice,
                        title = cartItem.Voucher?.Title,
                        quantity = cartItem.Quantity,
                        productImage = _mapper.Map<IList<GetMediaDTO>>(cartItem.Voucher?.Medias).FirstOrDefault(x => x.type == MediaEnum.ADVERTISEMENT)?.url
                    });
                }
            }

            return cartDTO;
        }

        public async Task<bool> AddItemAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            if (_currentUser.Carts != null && _currentUser.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _currentUser.Carts.FirstOrDefault(x => x.VoucherId == voucherId);
                if (cartVoucher != null)
                {
                    if (cartVoucher.Quantity >= 20)
                    {
                        throw new ConflictException("Quantity đã vượt quá 20");
                    }
                    else
                    {
                        cartVoucher.Quantity += 1;
                        cartVoucher.UpdateBy = thisUserObj.userId;
                        cartVoucher.UpdateDate = DateTime.Now;

                        return await _userRepository.UpdateAsync(_currentUser);
                    }
                }
                else
                {
                    var existedVoucher = await _voucherRepository.GetByIdAsync(voucherId, includeProperties: x => x.Include(x => x.Medias));

                    if (existedVoucher == null)
                    {
                        throw new NotFoundException("Không tìm thấy voucher này");
                    }

                    _currentUser.Carts.Add(new()
                    {
                        CreateBy = thisUserObj.userId,
                        CreateDate = DateTime.Now,
                        Quantity = 1,
                        Voucher = existedVoucher,
                    });

                    return await _userRepository.UpdateAsync(_currentUser);
                }
            }

            return false;
            //Guid userId = thisUserObj.userId;

            //User userInstance = await GetCurrentUser(userId);

            //if (userInstance.Carts.Count() > 5)
            //{
            //    throw new ConflictException("Bạn chỉ có thể mua tối đa 5 loại voucher một lúc");
            //}

            //var haveVoucher = userInstance.Carts.FirstOrDefault(x => x.VoucherId == voucherId);
            //if (haveVoucher != null)
            //{
            //    haveVoucher.Quantity += 1;
            //    haveVoucher.UpdateBy = thisUserObj.userId;
            //    haveVoucher.UpdateDate = DateTime.Now;
            //}
            //else
            //{
            //    Voucher existedVoucher = await _voucherRepository.GetByIdAsync(voucherId, includeProperties: x => x.Include(x => x.Medias)
            //                                                                                                            .Include(x => x.Seller));

            //    if (existedVoucher == null)
            //    {
            //        throw new NotFoundException("Không tìm thầy voucher này");
            //    }

            //    userInstance.Carts.Add(new()
            //    {
            //        VoucherId = voucherId,
            //        CreateBy = thisUserObj.userId,
            //        CreateDate = DateTime.Now,
            //        Quantity = 1,
            //        Status = ObjectStatusEnum.ACTIVE.ToString(),
            //        Voucher = existedVoucher,
            //        Buyer = userInstance
            //    });
            //}

            //var result = await _userRepository.UpdateAsync(userInstance);

            //return result;
        }

        public async Task<bool> DecreaseQuantityAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            if (_currentUser.Carts != null && _currentUser.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _currentUser.Carts.FirstOrDefault(x => x.VoucherId == voucherId);
                if (cartVoucher == null)
                {
                    throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
                }
                cartVoucher.Quantity -= 1;

                if (cartVoucher.Quantity <= 0)
                {
                    _currentUser.Carts.Remove(cartVoucher);
                }
                else
                {
                    cartVoucher.UpdateBy = thisUserObj.userId;
                    cartVoucher.UpdateDate = DateTime.Now;
                }

                return await _userRepository.UpdateAsync(_currentUser);
            }

            return false;
            //Guid userId = thisUserObj.userId;   

            //// Fetch the current user
            //User? user = await GetCurrentUser(userId);

            //// Find the cart item with the specified voucher
            //var cartItem = user.Carts.FirstOrDefault(c => c.VoucherId == voucherId);

            //if (cartItem != null)
            //{
            //    // Increase the quantity
            //    cartItem.Quantity -= 1;
            //    cartItem.UpdateBy = userId;
            //    cartItem.UpdateDate = DateTime.Now;

            //    // Update the user with the modified cart
            //    return await _userRepository.UpdateAsync(user);
            //}

            // throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
        }

        public async Task<bool> IncreaseQuantityAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            if (_currentUser.Carts != null && _currentUser.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _currentUser.Carts.FirstOrDefault(x => x.VoucherId == voucherId);
                if (cartVoucher == null)
                {
                    throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
                }
                else if (cartVoucher.Quantity >= 20)
                {
                    throw new ConflictException($"Voucher {voucherId} không thể vượt quá 20");
                }
                else
                {
                    cartVoucher.Quantity += 1;
                    cartVoucher.UpdateBy = thisUserObj.userId;
                    cartVoucher.UpdateDate = DateTime.Now;

                    return await _userRepository.UpdateAsync(_currentUser);
                }
            }

            return false;
            //Guid userId = thisUserObj.userId;

            //User? user = await GetCurrentUser(userId);

            //var cartItem = user.Carts.FirstOrDefault(c => c.VoucherId == voucherId);

            //if (cartItem != null)
            //{
            //    if (cartItem.Quantity > 19)
            //    {
            //        throw new ConflictException("Bạn chỉ có thể mua tối đa 20 code mỗi loại voucher");
            //    }

            //    cartItem.Quantity += 1;
            //    cartItem.UpdateBy = userId;
            //    cartItem.UpdateDate = DateTime.Now;

            //    return await _userRepository.UpdateAsync(user);
            //}

            // throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
        }


        public async Task<bool> RemoveItemAsync(Guid voucherId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            if (_currentUser.Carts != null && _currentUser.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _currentUser.Carts.FirstOrDefault(x => x.VoucherId == voucherId);
                if (cartVoucher == null) 
                {
                    throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
                }
                if (cartVoucher.Quantity >= 20)
                {
                    throw new ConflictException($"Voucher {voucherId} không thể vượt quá 20");
                }
                _currentUser.Carts.Remove(cartVoucher);
                return await _userRepository.UpdateAsync(_currentUser);
            }

            return false;

            //Guid userId = thisUserObj.userId;

            //User? user = await GetCurrentUser(userId);

            //var cartItem = user.Carts.FirstOrDefault(c => c.VoucherId == voucherId);

            //if (cartItem != null)
            //{
            //    user.Carts.Remove(cartItem);

            //    return await _userRepository.UpdateAsync(user);
            //}
        }


        public async Task<bool> UpdateQuantityAsync(Guid voucherId, int quantity, ThisUserObj thisUserObj)
        {
            if (quantity <= 0)
            {
                throw new ConflictException("Số lượng không hợp lệ");
            }
            if (quantity >= 20)
            {
                throw new ConflictException("Bạn chỉ có thể mua tối đa 20 code mỗi loại voucher");
            }

            await GetCartsAsync(thisUserObj);

            if (_currentUser.Carts != null && _currentUser.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _currentUser.Carts.FirstOrDefault(x => x.VoucherId == voucherId);
                if (cartVoucher == null)
                {
                    throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
                }
                if (cartVoucher.Quantity >= 20)
                {
                    throw new ConflictException($"Voucher {voucherId} không thể vượt quá 20");
                }
                cartVoucher.Quantity = quantity;
                cartVoucher.UpdateBy = thisUserObj.userId;
                cartVoucher.UpdateDate = DateTime.Now;

                return await _userRepository.UpdateAsync(_currentUser);
            }

            return false;


            //Guid userId = thisUserObj.userId;

            //User? user = await GetCurrentUser(userId);

            //var cartItem = user.Carts.FirstOrDefault(c => c.VoucherId == voucherId);

            //if (cartItem != null)
            //{

            //    cartItem.Quantity = quantity;
            //    cartItem.UpdateBy = userId; 
            //    cartItem.UpdateDate = DateTime.Now;

            //    return await _userRepository.UpdateAsync(user);
            //}

            // throw new NotFoundException($"Không thấy voucher {voucherId} trong cart");
        }

        //public async Task<User> GetCurrentUser(Guid userId)
        //{
        //    IQueryable<User> users = _userRepository.CheckLocal();

        //    if (users != null && users.Any())
        //    {
        //        userInstance = users.FirstOrDefault(x => x.Id == userId);
        //    }

        //    if (userInstance == null)
        //    {
        //        userInstance = await _userRepository.GetByIdAsync(userId, includeProperties: x => x.Include(x => x.Carts)
        //                                                                                                .ThenInclude(x => x.Voucher)
        //                                                                                                .ThenInclude(x => x.Medias)
        //        );
        //    }

        //    foreach (var cartItem in userInstance.Carts)
        //    {
        //        var voucher = cartItem.Voucher;

        //        if (voucher.Seller == null && users.FirstOrDefault(x => x.Id == voucher.SellerID) == null)
        //        {
        //            voucher.Seller = await _userRepository.GetByIdAsync(voucher.SellerID);
        //        }
        //    }

        //    return null;
        //}
    }
}
