﻿using AutoMapper;
using Vouchee.Business.Models.DTOs;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.AppStarts
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // VOUCHER
            CreateMap<Voucher, VoucherDTO>().ReverseMap();
            CreateMap<Voucher, CreateVoucherDTO>().ReverseMap();
            CreateMap<Voucher, UpdateVoucherDTO>().ReverseMap();
            CreateMap<Voucher, GetVoucherDTO>().ReverseMap();
            CreateMap<GetVoucherDTO, VoucherFiler>().ReverseMap();

            // ORDER DETAIL
            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();
            CreateMap<OrderDetail, CreateOrderDetailDTO>().ReverseMap();
            CreateMap<OrderDetail, UpdateOrderDetailDTO>().ReverseMap();
            CreateMap<OrderDetail, GetOrderDetailDTO>().ReverseMap();

            // ORDER
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<Order, CreateOrderDTO>().ReverseMap();
            CreateMap<Order, UpdateOrderDTO>().ReverseMap();
            CreateMap<Order, GetOrderDTO>().ReverseMap();
            CreateMap<GetOrderDTO, OrderFilter>().ReverseMap();

            // ROLE
            CreateMap<Role, RoleDTO>().ReverseMap();
            CreateMap<Role, CreateRoleDTO>().ReverseMap();
            CreateMap<Role, UpdateRoleDTO>().ReverseMap();
            CreateMap<Role, GetRoleDTO>().ReverseMap();
            CreateMap<GetRoleDTO, RoleFilter>().ReverseMap();

            // SHOP
            CreateMap<Shop, ShopDTO>().ReverseMap();
            CreateMap<Shop, CreateShopDTO>().ReverseMap();
            CreateMap<Shop, UpdateShopDTO>().ReverseMap();
            CreateMap<Shop, GetShopDTO>().ReverseMap();
            CreateMap<GetShopDTO, ShopFilter>().ReverseMap();

            // SUPPLIER
            CreateMap<Supplier, SupplierDTO>().ReverseMap();
            CreateMap<Supplier, CreateSupplierDTO>().ReverseMap();
            CreateMap<Supplier, UpdateSupplierDTO>().ReverseMap();
            CreateMap<Supplier, GetSupplierDTO>().ReverseMap();
            CreateMap<GetSupplierDTO, SupplierFilter>().ReverseMap();

            // USER
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, CreateUserDTO>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();
            CreateMap<User, GetUserDTO>().ReverseMap();
            CreateMap<GetUserDTO, UserFilter>().ReverseMap();

            // VOUCHER CODE
            CreateMap<VoucherCode, VoucherCodeDTO>().ReverseMap();
            CreateMap<VoucherCode, CreateVoucherCodeDTO>().ReverseMap();
            CreateMap<VoucherCode, UpdateVoucherCodeDTO>().ReverseMap();
            CreateMap<VoucherCode, GetVoucherCodeDTO>().ReverseMap();
            CreateMap<GetVoucherCodeDTO, VoucherCodeDTO>().ReverseMap();

            // VOUCHER TYPE
            CreateMap<VoucherType, VoucherTypeDTO>().ReverseMap();
            CreateMap<VoucherType, CreateVoucherTypeDTO>().ReverseMap();
            CreateMap<VoucherType, UpdateVoucherTypeDTO>().ReverseMap();
            CreateMap<VoucherType, GetVoucherTypeDTO>().ReverseMap();
            CreateMap<GetVoucherTypeDTO, VoucherTypeFilter>().ReverseMap();
        }
    }
}
