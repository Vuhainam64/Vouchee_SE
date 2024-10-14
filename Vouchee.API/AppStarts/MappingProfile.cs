using AutoMapper;
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
            CreateMap<Voucher, GetDetailVoucherDTO>()
                .ForMember(x => x.voucherTypeName, dest => dest.MapFrom(opt => opt.VoucherType.Title))
                .ForMember(x => x.brandName, dest => dest.MapFrom(opt => opt.Brand.Name))
                .ForMember(x => x.brandImage, dest => dest.MapFrom(opt => opt.Brand.Image))
                .ForMember(x => x.supplierName, dest => dest.MapFrom(opt => opt.Supplier.Name))
                .ReverseMap();
            CreateMap<GetAllVoucherDTO, VoucherFilter>().ReverseMap();
            CreateMap<Voucher, GetDetailVoucherDTO>().ReverseMap();
            CreateMap<Voucher, GetAllVoucherDTO>().ReverseMap();
            CreateMap<Voucher, GetBestBuyVoucherDTO>().ReverseMap(); 
            CreateMap<Voucher, GetNearestVoucherDTO>().ReverseMap();
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
            CreateMap<Address, GetAllAddressDTO>().ReverseMap();
            CreateMap<GetAllAddressDTO, AddressFilter>().ReverseMap();

            // SUPPLIER
            CreateMap<Supplier, SupplierDTO>().ReverseMap();
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
            CreateMap<Promotion, PromotionDTO>().ReverseMap();
            CreateMap<Promotion, CreatePromotionDTO>().ReverseMap();
            CreateMap<Promotion, UpdatePromotionDTO>().ReverseMap();
            CreateMap<Promotion, GetPromotionDTO>()
                .ForMember(des => des.Type, src => src.MapFrom(src => EnumMapper<PromotionTypeEnum>.MapType(src.Type)))
                .ReverseMap();
            CreateMap<GetPromotionDTO, PromotionFilter>().ReverseMap();

            // CATEGORY
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, GetCategoryDTO>().ReverseMap();
            CreateMap<GetCategoryDTO, CategoryFilter>().ReverseMap();

            // BRAND
            CreateMap<Brand, BrandDTO>().ReverseMap();
            CreateMap<Brand, CreateBrandDTO>().ReverseMap();
            CreateMap<Brand, UpdateBrandDTO>().ReverseMap();
            CreateMap<Brand, GetBrandDTO>().ReverseMap();
            CreateMap<GetBrandDTO, BrandFilter>().ReverseMap();
        }
    }
}
