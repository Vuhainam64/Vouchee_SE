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

        private User? _currentUser;

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
            _currentUser = _userRepository.CheckLocal().First(x => x.Id == thisUserObj.userId);
            
            CartDTO cartDTO = new();

            foreach (var cartItem in _currentUser.Carts)
            {
                var currentModal = cartItem.Modal;

                if (currentModal != null)
                {
                    var sellerCart = cartDTO.sellers.FirstOrDefault(s => s.id == currentModal.Voucher.SellerID);
                    if (sellerCart == null)
                    {
                        sellerCart = new SellerCartDTO
                        {
                            id = currentModal?.Voucher.SellerID,
                            name = currentModal?.Voucher.Seller.Name,
                            image = currentModal.Voucher.Seller.Image,
                            modals = new List<CartVoucherDTO>()
                        };
                        cartDTO.sellers.Add(sellerCart);
                    }

                    sellerCart.modals.Add(new CartVoucherDTO
                    {
                        id = cartItem.Modal?.Id,
                        originalPrice = cartItem.Modal?.OriginalPrice,
                        sellPrice = cartItem.Modal?.SellPrice,
                        title = cartItem.Modal?.Title,
                        quantity = cartItem.Quantity,
                        image = cartItem.Modal?.Image
                    });
                }
            }

            cartDTO.totalQuantity = cartDTO.sellers.Sum(s => s.modals.Sum(m => m.quantity));
            cartDTO.totalPrice = (decimal) cartDTO.sellers.Sum(s => s.modals.Sum(m => m.sellPrice * m.quantity));
            cartDTO.discountPrice = (decimal) cartDTO.sellers.Sum(s => s.modals.Sum(m => (m.originalPrice * m.quantity) * m.percentDiscount));

            return cartDTO;
        }

        public async Task<bool> AddItemAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            // Voucher exist in cart already
            var cartVoucher = _currentUser.Carts.FirstOrDefault(x => x.ModalId == modalId);
            if (cartVoucher != null)
            {
                var quantity = cartVoucher.Modal.VoucherCodes.Count();
                if (cartVoucher.Quantity >= quantity)
                {
                    throw new ConflictException($"HIện tại modal này mới có {quantity} code");
                }
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
                var existedModal = await _modalRepository.GetByIdAsync(modalId, includeProperties: x => x.Include(x => x.VoucherCodes));

                if (existedModal == null)
                {
                    throw new NotFoundException("Không tìm thấy modal này");
                }

                if (existedModal.VoucherCodes.Count() == 0)
                {
                    throw new NotFoundException("Modal này không có code nào để sử dụng");
                }

                _currentUser.Carts.Add(new()
                {
                    CreateBy = thisUserObj.userId,
                    CreateDate = DateTime.Now,
                    Quantity = 1,
                    Modal = existedModal,
                });

                return await _userRepository.UpdateAsync(_currentUser);
            }
        }

        public async Task<bool> DecreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            if (_currentUser.Carts != null && _currentUser.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _currentUser.Carts.FirstOrDefault(x => x.ModalId == modalId);
                if (cartVoucher == null)
                {
                    throw new NotFoundException($"Không thấy voucher {modalId} trong cart");
                }

                if (cartVoucher.Quantity <= 1)
                {
                    _currentUser.Carts.Remove(cartVoucher);
                }
                else
                {
                    cartVoucher.Quantity -= 1;
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

        public async Task<bool> IncreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            if (_currentUser.Carts != null && _currentUser.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _currentUser.Carts.FirstOrDefault(x => x.ModalId == modalId);

                var quantity = cartVoucher.Modal.VoucherCodes.Count();

                if (cartVoucher.Quantity >= quantity)
                {
                    throw new ConflictException($"HIện tại modal này mới có {quantity} code");
                }

                if (cartVoucher == null)
                {
                    throw new NotFoundException($"Không thấy voucher {modalId} trong cart");
                }
                else if (cartVoucher.Quantity >= 20)
                {
                    throw new ConflictException($"Voucher {modalId} không thể vượt quá 20");
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


        public async Task<bool> RemoveItemAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj);

            if (_currentUser.Carts != null && _currentUser.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _currentUser.Carts.FirstOrDefault(x => x.ModalId == modalId);
                if (cartVoucher == null) 
                {
                    throw new NotFoundException($"Không thấy voucher {modalId} trong cart");
                }
                if (cartVoucher.Quantity >= 20)
                {
                    throw new ConflictException($"Voucher {modalId} không thể vượt quá 20");
                }

                _currentUser.Carts.Remove(cartVoucher);

                return await _userRepository.UpdateAsync(_currentUser);
            }

            return false;
        }

        public async Task<bool> UpdateQuantityAsync(Guid modalId, int quantity, ThisUserObj thisUserObj)
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
                var cartVoucher = _currentUser.Carts.FirstOrDefault(x => x.ModalId == modalId);

                if (cartVoucher.Quantity >= quantity)
                {
                    throw new ConflictException($"HIện tại modal này mới có {quantity} code");
                }

                if (cartVoucher == null)
                {
                    throw new NotFoundException($"Không thấy voucher {modalId} trong cart");
                }
                if (cartVoucher.Quantity >= 20)
                {
                    throw new ConflictException($"Voucher {modalId} không thể vượt quá 20");
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
