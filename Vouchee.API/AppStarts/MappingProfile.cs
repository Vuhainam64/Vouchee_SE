using AutoMapper;
using Vouchee.Business.Models.DTOs;
using Vouchee.Business.Utils;
using Vouchee.Data.Models.Constants.Enum.Other;
using Vouchee.Data.Models.Constants.Enum.Status;
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
                .ForMember(dest => dest.totalQuantitySold, opt => opt.MapFrom(src => src.Modals
                    .SelectMany(m => m.OrderDetails)
                    .Sum(od => od.Quantity)))
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.Medias.FirstOrDefault(m => m.Index == 0).Url))
                .ForMember(dest => dest.originalPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).OriginalPrice))
                .ForMember(dest => dest.sellPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).SellPrice))
                .ForMember(x => x.brandName, dest => dest.MapFrom(opt => opt.Brand.Name))
                .ForMember(x => x.brandImage, dest => dest.MapFrom(opt => opt.Brand.Image))
                .ForMember(x => x.supplierName, dest => dest.MapFrom(opt => opt.Supplier.Name))
                .ForMember(x => x.supplierImage, dest => dest.MapFrom(opt => opt.Supplier.Image))
                .ForMember(x => x.sellerName, dest => dest.MapFrom(opt => opt.Seller.Name))
                .ForMember(x => x.sellerImage, dest => dest.MapFrom(opt => opt.Seller.Image))
                .ReverseMap();

            CreateMap<Voucher, GetVoucherSellerDTO>()
                .ForMember(x => x.supplierName, dest => dest.MapFrom(opt => opt.Supplier.Name))
                .ForMember(x => x.supplierImage, dest => dest.MapFrom(opt => opt.Supplier.Image))
                .ForMember(dest => dest.stock, opt => opt.MapFrom(src => src.Modals.Sum(x => x.Stock)))
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.Medias.FirstOrDefault(m => m.Index == 0).Url))
                .ForMember(dest => dest.originalPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).OriginalPrice))
                .ForMember(dest => dest.sellPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).SellPrice))
                .ForMember(x => x.brandName, dest => dest.MapFrom(opt => opt.Brand.Name))
                .ForMember(x => x.brandImage, dest => dest.MapFrom(opt => opt.Brand.Image))
                .ForMember(x => x.supplierName, dest => dest.MapFrom(opt => opt.Supplier.Name))
                .ForMember(x => x.supplierImage, dest => dest.MapFrom(opt => opt.Supplier.Image))
                .ForMember(x => x.sellerName, dest => dest.MapFrom(opt => opt.Seller.Name))
                .ForMember(x => x.sellerImage, dest => dest.MapFrom(opt => opt.Seller.Image))
                .ReverseMap();

            CreateMap<Voucher, GetDetailVoucherDTO>()
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.Medias.FirstOrDefault(m => m.Index == 0).Url))
                .ForMember(dest => dest.originalPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).OriginalPrice))
                .ForMember(dest => dest.sellPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).SellPrice))
                .ForMember(x => x.brandName, dest => dest.MapFrom(opt => opt.Brand.Name))
                .ForMember(x => x.brandImage, dest => dest.MapFrom(opt => opt.Brand.Image))
                .ForMember(x => x.supplierName, dest => dest.MapFrom(opt => opt.Supplier.Name))
                .ForMember(x => x.supplierImage, dest => dest.MapFrom(opt => opt.Supplier.Image))
                .ForMember(x => x.sellerName, dest => dest.MapFrom(opt => opt.Seller.Name))
                .ForMember(x => x.sellerImage, dest => dest.MapFrom(opt => opt.Seller.Image))
                .ForMember(x => x.addresses, dest => dest.MapFrom(opt => opt.Brand.Addresses))
                .ReverseMap();

            CreateMap<Voucher, GetNearestVoucherDTO>()
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.Medias.FirstOrDefault(m => m.Index == 0).Url))
                .ForMember(dest => dest.originalPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).OriginalPrice))
                .ForMember(dest => dest.sellPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).SellPrice))
                .ForMember(x => x.brandName, dest => dest.MapFrom(opt => opt.Brand.Name))
                .ForMember(x => x.brandImage, dest => dest.MapFrom(opt => opt.Brand.Image))
                .ForMember(x => x.supplierName, dest => dest.MapFrom(opt => opt.Supplier.Name))
                .ForMember(x => x.supplierImage, dest => dest.MapFrom(opt => opt.Supplier.Image))
                .ForMember(x => x.sellerName, dest => dest.MapFrom(opt => opt.Seller.Name))
                .ForMember(x => x.sellerImage, dest => dest.MapFrom(opt => opt.Seller.Image))
                .ForMember(x => x.addresses, dest => dest.MapFrom(opt => opt.Brand.Addresses))
                .ReverseMap();

            CreateMap<GetVoucherDTO, VoucherFilter>().ReverseMap();
            CreateMap<GetVoucherSellerDTO, VoucherFilter>().ReverseMap();
            CreateMap<GetNearestVoucherDTO, VoucherFilter>().ReverseMap();

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

            // ADDRESS
            CreateMap<Address, CreateAddressDTO>().ReverseMap();
            CreateMap<Address, UpdateAddressDTO>().ReverseMap();
            CreateMap<Address, GetDistanceAddressDTO>().ReverseMap();
            CreateMap<Address, GetAddressDTO>().ReverseMap();
            CreateMap<Address, GetDetailAddressDTO>().ReverseMap();
            CreateMap<GetAddressDTO, AddressFilter>().ReverseMap();

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
            CreateMap<User, GetUserDTO>().ReverseMap();
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

            // CATEGORY
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, GetCategoryDTO>()
                .ForMember(x => x.voucherTypeTitle, dest => dest.MapFrom(opt => opt.VoucherType.Title))
                .ReverseMap();
            CreateMap<GetCategoryDTO, CategoryFilter>().ReverseMap();

            // BRAND
            CreateMap<Brand, CreateBrandDTO>()
                .ForMember(dest => dest.addresses, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Addresses, opt => opt.Ignore());
            CreateMap<Brand, UpdateBrandDTO>().ReverseMap();
            CreateMap<Brand, GetDetalBrandDTO>().ReverseMap();
            CreateMap<Brand, GetBrandDTO>().ReverseMap();
            CreateMap<Brand, GetDetalBrandDTO>().ReverseMap();
            CreateMap<GetBrandDTO, BrandFilter>().ReverseMap();

            // IMAGE
            CreateMap<Media, CreateMediaDTO>().ReverseMap();
            CreateMap<Media, UpdateMediaDTO>().ReverseMap();
            CreateMap<Media, GetMediaDTO>().ReverseMap();
            

            // CART
            CreateMap<Cart,CartDTO>()
                .ForMember(dest => dest.balance, opt => opt.MapFrom(src => src.Buyer.BuyerWallet.Balance))
                .ReverseMap();

            // MODAL
            CreateMap<Modal, CreateModalDTO>().ReverseMap();
            CreateMap<Modal, UpdateModalDTO>().ReverseMap();
            CreateMap<Modal, GetModalDTO>()
                .ForMember(dest => dest.brandId, opt => opt.MapFrom(src => src.Voucher.Brand.Id))
                .ForMember(dest => dest.brandName, opt => opt.MapFrom(src => src.Voucher.Brand.Name))
                .ForMember(dest => dest.brandImage, opt => opt.MapFrom(src => src.Voucher.Brand.Image))
                .ReverseMap();
            CreateMap<Modal, GetDetailModalDTO>().ReverseMap();
            CreateMap<GetModalDTO, ModalFilter>().ReverseMap();
            CreateMap<Modal, CartModalDTO>()
                .ForMember(dest => dest.brandId, opt => opt.MapFrom(src => src.Voucher.Brand.Id))
                .ForMember(dest => dest.brandName, opt => opt.MapFrom(src => src.Voucher.Brand.Name))
                .ForMember(dest => dest.brandImage, opt => opt.MapFrom(src => src.Voucher.Brand.Image))
                .ReverseMap();

            // NOTIFICATION
            CreateMap<Notification, NotificationDTO>().ReverseMap();
            CreateMap<Notification, CreateNotificationDTO>().ReverseMap();
            CreateMap<Notification, UpdateNotificationDTO>().ReverseMap();
            CreateMap<Notification, GetNotificationDTO>().ReverseMap();
            CreateMap<GetNotificationDTO, NotifcationFilter>().ReverseMap();

            // WALLET
            CreateMap<Wallet, WalletDTO>().ReverseMap();
            CreateMap<Wallet, GetWalletDTO>().ReverseMap();
            CreateMap<Wallet, GetBuyerWallet>().ReverseMap();
            CreateMap<Wallet, GetSellerWallet>().ReverseMap();

            // WALLET TRANSACTION
            CreateMap<WalletTransaction, WalletTransactionDTO>().ReverseMap();
            CreateMap<WalletTransaction, GetSellerWalletTransaction>().ReverseMap();
            CreateMap<WalletTransaction, GetBuyerWalletTransactionDTO>().ReverseMap();
            CreateMap<GetSellerWalletTransaction, WalletTransactionFilter>().ReverseMap();
            CreateMap<GetBuyerWalletTransactionDTO, WalletTransactionFilter>().ReverseMap();

            // MONEY REQUEST
            CreateMap<MoneyRequest, CreateTopUpRequestDTO>().ReverseMap();
            CreateMap<MoneyRequest, GetTopUpRequestDTO>().ReverseMap();

            // PARTNER TRANSACTION
            CreateMap<PartnerTransaction, SePayTransactionDTO>().ReverseMap();
            CreateMap<PartnerTransaction, CreateSePayPartnerInTransactionDTO>()
                .ForMember(dest => dest.id, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(src => src.Id, opt => opt.Ignore());

            // PROMOTION
            CreateMap<Promotion, CreateShopPromotionDTO>().ReverseMap();
            CreateMap<Promotion, CreateModalPromotionDTO>().ReverseMap();
            CreateMap<Promotion, GetShopPromotionDTO>().ReverseMap();
            CreateMap<Promotion, GetModalPromotionDTO>().ReverseMap();
        }
    }
}
