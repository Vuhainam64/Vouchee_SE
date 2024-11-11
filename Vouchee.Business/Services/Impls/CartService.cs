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
using Vouchee.Data.Helpers.Base;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.Business.Services.Impls
{
    public class CartService : ICartService
    {
        private readonly IBaseRepository<Modal> _modalRepository;
        private readonly IBaseRepository<Voucher> _voucherRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IMapper _mapper;

        private User? _user;
        private CartDTO? _cartDTO;

        public CartService(IBaseRepository<Modal> modalRepository,
                            IBaseRepository<Voucher> voucherRepository,
                            IBaseRepository<User> userRepository, 
                            IMapper mapper)
        {
            _modalRepository = modalRepository;
            _voucherRepository = voucherRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<CartDTO> GetCartsAsync(ThisUserObj thisUserObj, bool isTracking = false)
        {
            _user = await _userRepository.GetByIdAsync(thisUserObj.userId,
                                                        includeProperties: query => query
                                                            .Include(x => x.Role)
                                                            .Include(x => x.Carts)
                                                                .ThenInclude(cart => cart.Modal)
                                                            .Include(x => x.Carts)
                                                                .ThenInclude(cart => cart.Modal)
                                                                    .ThenInclude(voucher => voucher.Voucher)
                                                            .Include(x => x.Carts)
                                                                .ThenInclude(cart => cart.Modal)
                                                                    .ThenInclude(voucher => voucher.Voucher.Seller)
                                                            .Include(x => x.Carts)
                                                                .ThenInclude(cart => cart.Modal)
                                                                    .ThenInclude(voucher => voucher.Voucher.Promotions)
                                                            .Include(x => x.Carts)
                                                                .ThenInclude(cart => cart.Modal)
                                                                    .ThenInclude(voucher => voucher.Voucher.Brand)
                                                            .Include(x => x.BuyerWallet)
                                                                    , isTracking);

            CartDTO cartDTO = new();

            cartDTO.vPoint = _user.VPoint;
            cartDTO.balance = _user.BuyerWallet.Balance;
            cartDTO.buyerId = _user.Id;

            if (_user.Carts.Count() != 0)
            {
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
                    cartDTO.totalPrice = cartDTO.sellers.Sum(s => s.modals.Sum(x => x.finalPrice));
                }
            }

            _cartDTO = cartDTO;
            return _cartDTO;
        }

        public async Task<CartDTO> AddItemAsync(Guid modalId, ThisUserObj thisUserObj, int quantity = 1)
        {
            await GetCartsAsync(thisUserObj, true);

            // Voucher exist in cart already
            var cartModal = _user.Carts.FirstOrDefault(x => x.ModalId == modalId);

            if (cartModal != null)
            {
                // Nào fix xong thì mở
                var modal = await _modalRepository.FindAsync(modalId, false);
                if (cartModal.Quantity >= modal.Stock)
                {
                    throw new ConflictException($"Hiện tại modal này mới có {modal.Stock} code");
                }
                if (cartModal.Quantity >= 20)
                {
                    throw new ConflictException("Quantity đã vượt quá 20");
                }
                else
                {
                    cartModal.Quantity += quantity;
                    cartModal.UpdateBy = thisUserObj.userId;
                    cartModal.UpdateDate = DateTime.Now;

                    var userState = _userRepository.GetEntityState(_user);

                    _userRepository.SetEntityState(_user, EntityState.Modified);
                    var result = await _userRepository.SaveChanges();
                    if (result)
                    {
                        await GetCartsAsync(thisUserObj, false);
                        return _cartDTO;
                    }
                }
            }
            else
            {
                var existedModal = await _modalRepository.GetByIdAsync(modalId, includeProperties: x => x.Include(x => x.Voucher));

                if (existedModal == null)
                {
                    throw new NotFoundException("Không tìm thấy modal này");
                }

                if (existedModal.Voucher.SellerID == thisUserObj.userId)
                {
                    throw new ConflictException($"{cartModal.ModalId} là modal của shop bạn. Bạn không thể order chính modal của mình");
                }

                if (existedModal.Stock == 0)
                {
                    throw new NotFoundException("Modal này không có code nào để sử dụng");
                }

                _user.Carts.Add(new()
                {
                    CreateBy = thisUserObj.userId,
                    CreateDate = DateTime.Now,
                    Quantity = quantity,
                    Modal = existedModal,
                });

                _userRepository.SetEntityState(_user, EntityState.Modified);
                var result = await _userRepository.SaveChanges();
                if (result)
                {
                    _userRepository.Attach(_user);
                    await GetCartsAsync(thisUserObj, false);
                    return _cartDTO;
                }
            }
            return null;
        }

        public async Task<CartDTO> DecreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj, true);

            if (_user.Carts != null && _user.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _user.Carts.FirstOrDefault(x => x.ModalId == modalId);
                if (cartVoucher == null)
                {
                    throw new NotFoundException($"Không thấy modal {modalId} trong cart");
                }

                if (cartVoucher.Quantity <= 1)
                {
                    await GetCartsAsync(thisUserObj, false);
                    return _cartDTO;
                }
                else
                {
                    cartVoucher.Quantity -= 1;
                    cartVoucher.UpdateBy = thisUserObj.userId;
                    cartVoucher.UpdateDate = DateTime.Now;
                }

                var state = _userRepository.GetEntityState(_user);
                var result = await _userRepository.SaveChanges();
                if (result)
                {
                    await GetCartsAsync(thisUserObj, false);
                    return _cartDTO;
                }
            }

            return null;
        }

        public async Task<CartDTO> IncreaseQuantityAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj, true);

            if (_user.Carts.Count != 0)
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
                    throw new ConflictException($"Hiện tại modal này mới có {modal.Stock} code");
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

                    var state = _userRepository.GetEntityState(_user);
                    var result = await _userRepository.SaveChanges();
                    if (result)
                    {
                        await GetCartsAsync(thisUserObj, false);
                        return _cartDTO;
                    }
                }
            }

            return null;
        }


        public async Task<CartDTO> RemoveItemAsync(Guid modalId, ThisUserObj thisUserObj)
        {
            await GetCartsAsync(thisUserObj, true);

            if (_user.Carts != null && _user.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _user.Carts.FirstOrDefault(x => x.ModalId == modalId);
                if (cartVoucher == null) 
                {
                    throw new NotFoundException($"Không thấy modal {modalId} trong cart");
                }

                _user.Carts.Remove(cartVoucher);

                var state = _userRepository.GetEntityState(_user);
                var result = await _userRepository.SaveChanges();
                if (result)
                {
                    await GetCartsAsync(thisUserObj, false);
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

            await GetCartsAsync(thisUserObj, true);

            if (_user.Carts != null && _user.Carts.Count != 0)
            {
                // Voucher exist in cart already
                var cartVoucher = _user.Carts.FirstOrDefault(x => x.ModalId == modalId);

                if (cartVoucher == null)
                {
                    throw new NotFoundException($"Không thấy modal {modalId} trong cart");
                }

                var modal = await _modalRepository.FindAsync(modalId, false);

                if (quantity > modal.Stock)
                {
                    throw new ConflictException($"Hiện tại modal này mới có {modal.Stock} code");
                }
                if (cartVoucher.Quantity > 20)
                {
                    throw new ConflictException($"Modal {modalId} không thể vượt quá 20");
                }
                cartVoucher.Quantity = quantity;
                cartVoucher.UpdateBy = thisUserObj.userId;
                cartVoucher.UpdateDate = DateTime.Now;

                var state = _userRepository.GetEntityState(_user);
                var result = await _userRepository.SaveChanges();
                if (result)
                {
                    await GetCartsAsync(thisUserObj, false);
                    return _cartDTO;
                }
            }

            return null;
        }
    }
}
