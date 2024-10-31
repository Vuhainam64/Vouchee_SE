﻿using AutoMapper;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Utils;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.DTOs;
using Vouchee.Data.Models.DTOs.Dashboard;
using Vouchee.Data.Models.Entities;
using Vouchee.Data.Models.Filters;

namespace Vouchee.API.AppStarts
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // VOUCHER
            CreateMap<Voucher, CreateVoucherDTO>().ReverseMap();
            CreateMap<Voucher, UpdateVoucherDTO>().ReverseMap();

            CreateMap<Voucher, GetVoucherDTO>()
                .ForMember(x => x.brandName, dest => dest.MapFrom(opt => opt.Brand.Name))
                .ForMember(x => x.brandImage, dest => dest.MapFrom(opt => opt.Brand.Image))
                .ForMember(x => x.percentDiscount, dest => dest.MapFrom(opt => opt.Promotions.First().PercentDiscount))
                .ForMember(x => x.promotionId, dest => dest.MapFrom(opt => opt.Promotions.First().Id))
                .ReverseMap();

            CreateMap<Voucher, GetDetailVoucherDTO>()
                .ForMember(x => x.supplierName, dest => dest.MapFrom(opt => opt.Supplier.Name))
                .ForMember(x => x.supplierImage, dest => dest.MapFrom(opt => opt.Supplier.Image))
                .ForMember(x => x.sellerName, dest => dest.MapFrom(opt => opt.Seller.Name))
                .ForMember(x => x.sellerImage, dest => dest.MapFrom(opt => opt.Seller.Image))
                .ForMember(x => x.brandName, dest => dest.MapFrom(opt => opt.Brand.Name))
                .ForMember(x => x.brandImage, dest => dest.MapFrom(opt => opt.Brand.Image))
                .ForMember(x => x.percentDiscount, dest => dest.MapFrom(opt => opt.Promotions.First().PercentDiscount))
                .ForMember(x => x.promotionId, dest => dest.MapFrom(opt => opt.Promotions.First().Id))
                .ReverseMap();

            CreateMap<Voucher, GetBestSoldVoucherDTO>()
                .ForMember(x => x.brandName, dest => dest.MapFrom(opt => opt.Brand.Name))
                .ForMember(x => x.brandImage, dest => dest.MapFrom(opt => opt.Brand.Image))
                .ForMember(x => x.percentDiscount, dest => dest.MapFrom(opt => opt.Promotions.First().PercentDiscount))
                .ForMember(x => x.promotionId, dest => dest.MapFrom(opt => opt.Promotions.First().Id))
                .ReverseMap();
            
            CreateMap<Voucher, CartVoucherDTO>()
                .ForMember(x => x.brandName, dest => dest.MapFrom(opt => opt.Brand.Name))
                .ForMember(x => x.brandImage, dest => dest.MapFrom(opt => opt.Brand.Image))
                .ForMember(x => x.percentDiscount, dest => dest.MapFrom(opt => opt.Promotions.First().PercentDiscount))
                .ForMember(x => x.promotionId, dest => dest.MapFrom(opt => opt.Promotions.First().Id))
                .ReverseMap();

            CreateMap<VoucherDTO, VoucherFilter>().ReverseMap();
            CreateMap<GetVoucherDTO, VoucherFilter>().ReverseMap();
            CreateMap<GetBestSoldVoucherDTO, VoucherFilter>().ReverseMap();
            CreateMap<CartVoucherDTO, VoucherDTO>().ReverseMap();

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

            // ADDRESS
            CreateMap<Address, CreateAddressDTO>().ReverseMap();
            CreateMap<Address, UpdateAddressDTO>().ReverseMap();
            CreateMap<Address, GetDistanceAddressDTO>().ReverseMap();
            CreateMap<Address, GetAddressDTO>().ReverseMap();
            CreateMap<Address, GetDetailAddressDTO>().ReverseMap();
            CreateMap<GetDetailAddressDTO, AddressFilter>().ReverseMap();

            // SUPPLIER
            CreateMap<Supplier, CreateSupplierDTO>().ReverseMap();
            CreateMap<Supplier, UpdateSupplierDTO>().ReverseMap();
            CreateMap<Supplier, GetSupplierDTO>().ReverseMap();
            CreateMap<GetSupplierDTO, SupplierFilter>().ReverseMap();
            CreateMap<Supplier, BestSuppleriDTO>().ReverseMap();

            // USER
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, CreateUserDTO>().ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();
            CreateMap<User, GetUserDTO>()
                .ForMember(x => x.roleName, dest => dest.MapFrom(opt => opt.Role.Name))
                .ReverseMap();
            CreateMap<GetUserDTO, UserFilter>().ReverseMap();
            CreateMap<User, RegisterDTO>().ReverseMap();

            // VOUCHER CODE
            CreateMap<VoucherCode, VoucherCodeDTO>().ReverseMap();
            CreateMap<VoucherCode, CreateVoucherCodeDTO>().ReverseMap();
            CreateMap<VoucherCode, UpdateVoucherCodeDTO>().ReverseMap();
            CreateMap<VoucherCode, GetVoucherCodeDTO>().ReverseMap();
            CreateMap<GetVoucherCodeDTO, VoucherCodeFilter>().ReverseMap();

            // VOUCHER TYPE
            CreateMap<VoucherType, CreateVoucherTypeDTO>().ReverseMap();
            CreateMap<VoucherType, UpdateVoucherTypeDTO>().ReverseMap();
            CreateMap<VoucherType, GetVoucherTypeDTO>().ReverseMap();
            CreateMap<GetVoucherTypeDTO, VoucherTypeFilter>().ReverseMap();

            // PROMOTION
            CreateMap<Promotion, CreatePromotionDTO>().ReverseMap();
            CreateMap<Promotion, UpdatePromotionDTO>().ReverseMap();
            CreateMap<Promotion, GetPromotionDTO>()
                .ForMember(des => des.type, src => src.MapFrom(src => EnumMapper<PromotionTypeEnum>.MapType(src.Type)))
                .ReverseMap();
            CreateMap<Promotion, GetDetailPromotionDTO>()
                .ForMember(des => des.type, src => src.MapFrom(src => EnumMapper<PromotionTypeEnum>.MapType(src.Type)))
                .ReverseMap();
            CreateMap<GetPromotionDTO, PromotionFilter>().ReverseMap();

            // CATEGORY
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, GetCategoryDTO>()
                .ForMember(x => x.voucherTypeTitle, dest => dest.MapFrom(opt => opt.VoucherType.Title))
                .ReverseMap();
            CreateMap<GetCategoryDTO, CategoryFilter>().ReverseMap();

            // BRAND
            CreateMap<Brand, CreateBrandDTO>().ReverseMap();
            CreateMap<Brand, UpdateBrandDTO>().ReverseMap();
            CreateMap<Brand, GetDetalBrandDTO>().ReverseMap();
            CreateMap<Brand, GetBrandDTO>().ReverseMap();
            CreateMap<GetDetalBrandDTO, BrandFilter>().ReverseMap();
            CreateMap<GetDetalBrandDTO, GetBrandDTO>().ReverseMap();

            // IMAGE
            CreateMap<Media, CreateMediaDTO>().ReverseMap();
            CreateMap<Media, UpdateMediaDTO>().ReverseMap();
            CreateMap<Media, GetMediaDTO>().ReverseMap();
            

            //Cart
            CreateMap<Cart,CartDTO>().ReverseMap();

            // MODAL
            CreateMap<Modal, CreateModalDTO>().ReverseMap();
            CreateMap<Modal, UpdateModalDTO>().ReverseMap();
            CreateMap<Modal, GetModalDTO>().ReverseMap();
            CreateMap<Modal, GetDetailModalDTO>().ReverseMap();
            CreateMap<GetModalDTO, ModalFilter>().ReverseMap();
        }
    }
}
