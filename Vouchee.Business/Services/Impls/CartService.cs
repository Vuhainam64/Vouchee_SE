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
        private readonly IModalRepository _modalRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private User? _user;
        private CartDTO? _cartDTO;

        public CartService(IModalRepository modalRepository,
                            IVoucherRepository voucherRepository,
                            IUserRepository userRepository, 
                            IMapper mapper)
        {
            _modalRepository = modalRepository;
            _voucherRepository = voucherRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<CartDTO> GetCartsAsync(ThisUserObj thisUserObj)
        {
            _user = await _userRepository.GetByIdAsync(thisUserObj.userId,
                                                        includeProperties: query => query
                                                            .Include(x => x.Role)
                                                            .Include(x => x.Carts)
                                                                .ThenInclude(cart => cart.Modal)
                                                            .Include(x => x.Carts)
                                                                .ThenInclude(cart => cart.Modal)
                                                                    .ThenInclude(voucher => voucher.Voucher.Seller)
                                                            .Include(x => x.Carts)
                                                                .ThenInclude(cart => cart.Modal)
                                                                    .ThenInclude(voucher => voucher.Voucher.Promotions),
                                                        isTracking: true);

            if (_user.Carts.Count() != 0)
            {
                CartDTO cartDTO = new();

                var groupedCarts = _user.Carts.GroupBy(cartItem => cartItem.Modal.Voucher.Seller?.Id).ToList();
                foreach (var carts in groupedCarts)
                {
                    SellerCartDTO sellerCartDTO = new();
                    sellerCartDTO.sellerId = carts.Key;
                    sellerCartDTO.sellerName = carts.First(x => x.Modal.Voucher.SellerID == carts.Key).Modal.Voucher.Seller.Name;
                    sellerCartDTO.sellerImage = carts.First(x => x.Modal.Voucher.SellerID == carts.Key).Modal.Voucher.Seller.Image;

                    foreach (var cart in _user.Carts.Where(x => x.Modal.Voucher.SellerID == sellerCartDTO.sellerId))
                    {
                        sellerCartDTO.modals.Add(_mapper.Map<CartModalDTO>(cart.Modal));
                        sellerCartDTO.modals.FirstOrDefault(x => x.id == cart.ModalId).quantity = cart.Quantity;
                    }

                    cartDTO.sellers.Add(sellerCartDTO);

                    cartDTO.totalQuantity = cartDTO.sellers.Sum(x => x.modals.Sum(x => x.quantity));
                    cartDTO.totalPrice = (decimal) cartDTO.sellers.Sum(s => s.modals.Sum(x => x.originalPrice));
                    cartDTO.discountPrice = (decimal) cartDTO.sellers.Sum(s => s.modals.Sum(x => x.salePrice));
                }

                _cartDTO = cartDTO;

                return _cartDTO;
            }

            return null;
        }

        public async Task<CartDTO> AddItemAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            // Voucher exist in cart already
            var cartModal = _user.Carts.FirstOrDefault(x => x.ModalId == modalId);
            if (cartModal != null)
            {
                if (cartModal.Modal.Voucher.SellerID == thisUserObj.userId)
                {
                    throw new ConflictException($"{cartModal.ModalId} là modal của shop bạn. Bạn không thể order chính modal của mình");
                }
                var modal = await _modalRepository.FindAsync(modalId, false);
                if (cartModal.Quantity >= modal.Stock)
                {
                    throw new ConflictException($"HIện tại modal này mới có {modal.Stock} code");
                }
                if (cartModal.Quantity >= 20)
                {
                    throw new ConflictException("Quantity đã vượt quá 20");
                }
                else
                {
                    cartModal.Quantity += 1;
                    cartModal.UpdateBy = thisUserObj.userId;
                    cartModal.UpdateDate = DateTime.Now;

                    var result = await _userRepository.UpdateAsync(_user, isTracking: true);
                    if (result)
                    {
                        await GetCartsAsync(thisUserObj);
                        return _cartDTO;
                    }
                }
            }
            else
            {
                var existedModal = await _modalRepository.FindAsync(modalId, false);

                if (existedModal == null)
                {
                    throw new NotFoundException("Không tìm thấy modal này");
                }

                if (existedModal.Stock == 0)
                {
                    throw new NotFoundException("Modal này không có code nào để sử dụng");
                }

                _user.Carts.Add(new()
                {
                    CreateBy = thisUserObj.userId,
                    CreateDate = DateTime.Now,
                    Quantity = 1,
                    Modal = existedModal,
                });

                var result = await _userRepository.UpdateAsync(_user); 
                if (result)
                {
                    await GetCartsAsync(thisUserObj);
                    return _cartDTO;
                }
            }
            return null;
        }

        public async Task<CartDTO> DecreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            if (_user.Carts != null && _user.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _user.Carts.FirstOrDefault(x => x.ModalId == modalId);
                if (cartVoucher == null)
                {
                    throw new NotFoundException($"Không thấy modal {modalId} trong cart");
                }

                if (cartVoucher.Quantity == 0)
                {
                    _user.Carts.Remove(cartVoucher);
                }
                else
                {
                    cartVoucher.Quantity -= 1;
                    cartVoucher.UpdateBy = thisUserObj.userId;
                    cartVoucher.UpdateDate = DateTime.Now;
                }


                var result = await _userRepository.UpdateAsync(_user);
                if (result)
                {
                    await GetCartsAsync(thisUserObj);
                    return _cartDTO;
                }
            }

            return null;
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

        public async Task<CartDTO> IncreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            if (_user.Carts != null && _user.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartModal = _user.Carts.FirstOrDefault(x => x.ModalId == modalId);

                if (cartModal == null)
                {
                    throw new NotFoundException($"Không thấy modal {modalId} trong cart");
                }

                var modal = await _modalRepository.FindAsync(modalId, false);

                if (cartModal.Quantity >= modal.Stock)
                {
                    throw new ConflictException($"HIện tại modal này mới có {modal.Stock} code");
                }
                else if (cartModal.Quantity >= 20)
                {
                    throw new ConflictException($"Modal {modalId} không thể vượt quá 20");
                }
                else
                {
                    cartModal.Quantity += 1;
                    cartModal.UpdateBy = thisUserObj.userId;
                    cartModal.UpdateDate = DateTime.Now;


                    var result = await _userRepository.UpdateAsync(_user);
                    if (result)
                    {
                        await GetCartsAsync(thisUserObj);
                        return _cartDTO;
                    }
                }
            }

            return null;
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


        public async Task<CartDTO> RemoveItemAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            if (_user.Carts != null && _user.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _user.Carts.FirstOrDefault(x => x.ModalId == modalId);
                if (cartVoucher == null) 
                {
                    throw new NotFoundException($"Không thấy modal {modalId} trong cart");
                }

                _user.Carts.Remove(cartVoucher);


                var result = await _userRepository.UpdateAsync(_user);
                if (result)
                {
                    await GetCartsAsync(thisUserObj);
                    return _cartDTO;
                }
            }
            return null;
        }

        public async Task<CartDTO> UpdateQuantityAsync(Guid modalId, int quantity, ThisUserObj thisUserObj)
        {
            if (quantity < 1)
            {
                throw new ConflictException("Số lượng không hợp lệ");
            }
            if (quantity > 20)
            {
                throw new ConflictException("Bạn chỉ có thể mua tối đa 20 code mỗi loại voucher");
            }

            await GetCartsAsync(thisUserObj);

            if (_user.Carts != null && _user.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _user.Carts.FirstOrDefault(x => x.ModalId == modalId);

                if (cartVoucher == null)
                {
                    throw new NotFoundException($"Không thấy modal {modalId} trong cart");
                }

                var modal = await _modalRepository.FindAsync(modalId, false);

                if (quantity >= modal.Stock)
                {
                    throw new ConflictException($"HIện tại modal này mới có {modal.Stock} code");
                }
                if (cartVoucher.Quantity >= 20)
                {
                    throw new ConflictException($"Modal {modalId} không thể vượt quá 20");
                }
                cartVoucher.Quantity = quantity;
                cartVoucher.UpdateBy = thisUserObj.userId;
                cartVoucher.UpdateDate = DateTime.Now;


                var result = await _userRepository.UpdateAsync(_user);
                if (result)
                {
                    await GetCartsAsync(thisUserObj);
                    return _cartDTO;
                }
            }

            return null;


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
