using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using Vouchee.Data.Repositories.IRepos;
using Vouchee.Data.Repositories.Repos;

namespace Vouchee.Business.Services.Impls
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
        }

        public async Task<Guid?> CreateCartAsync(CreateCartDTO createCartDTO, ThisUserObj thisUserObj)
        {
            try
            {
                Cart cart = _mapper.Map<Cart>(createCartDTO);
                var cartId = await _cartRepository.AddAsync(cart);

                return cartId;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new CreateObjectException("Lỗi không xác định khi tạo Cart");
            }
        }

        public async Task<bool> DeleteCartAsync(Guid id)
        {
            try
            {
                var cart = await _cartRepository.GetByIdAsync(id); // Fetch the cart by Id
                if (cart == null)
                {
                    throw new NotFoundException("Không tìm thấy Cart");
                }

                await _cartRepository.DeleteAsync(cart); // Remove the cart
                return true; // Return success
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi khi xóa Cart");
            }
        }

        public async Task<IList<GetCartDTO>> GetCartsAsync()
        {
            IQueryable<GetCartDTO> result;
            try
            {
                result = _cartRepository.GetTable()
                            .ProjectTo<GetCartDTO>(_mapper.ConfigurationProvider);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải cart");
            }
            return result.ToList();
        }

        public async Task<IList<GetCartDTO>> GetCartsbyUIdAsync(Guid id)
        {

            IQueryable<GetCartDTO> result;
            try
            {
                result = _cartRepository.GetTable()
                            .Where(x => x.User.Id == id)
                            .ProjectTo<GetCartDTO>(_mapper.ConfigurationProvider);
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi không xác định khi tải cart");
            }
            return result.ToList();
        }

        public async Task<bool> UpdatCartAsync(Guid id, UpdateCartDTO updateCartDTO)
        { 
            try
            {
                var cart = await _cartRepository.GetByIdAsync(id);
                if (cart == null)
                {
                    throw new NotFoundException("Không tìm thấy Cart");
                }
                cart.Quantity = updateCartDTO.Quantity;

                await _cartRepository.UpdateAsync(cart);
                return true;
            }
            catch (Exception ex)
            {
                LoggerService.Logger(ex.Message);
                throw new LoadException("Lỗi khi cập nhật Cart");
            }
        }
    }
}
