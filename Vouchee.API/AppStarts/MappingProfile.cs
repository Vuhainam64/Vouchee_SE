using AutoMapper;
using Google.Api;
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
                .ForMember(dest => dest.shopDiscount, opt => opt.MapFrom(src => src.Seller.ShopPromotions
                                                                                    .Where(p => p.Stock > 0)
                                                                                    .OrderByDescending(p => p.PercentDiscount)
                                                                                    .Select(p => p.PercentDiscount)
                                                                                    .FirstOrDefault()))
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.Medias.FirstOrDefault(m => m.Index == 0).Url))
                .ForMember(dest => dest.originalPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).OriginalPrice))
                .ForMember(dest => dest.sellPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).SellPrice))
                .ForMember(dest => dest.brandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.brandImage, opt => opt.MapFrom(src => src.Brand.Image))
                .ForMember(dest => dest.supplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.supplierImage, opt => opt.MapFrom(src => src.Supplier.Image))
                .ForMember(dest => dest.sellerName, opt => opt.MapFrom(src => src.Seller.Name))
                .ForMember(dest => dest.sellerImage, opt => opt.MapFrom(src => src.Seller.Image))
                .ReverseMap();

            CreateMap<Voucher, GetVoucherSellerDTO>()
                .ForMember(dest => dest.shopDiscount, opt => opt.MapFrom(src => src.Seller.ShopPromotions
                                                                                    .Where(p => p.Stock > 0)
                                                                                    .OrderByDescending(p => p.PercentDiscount)
                                                                                    .Select(p => p.PercentDiscount)
                                                                                    .FirstOrDefault()))
                .ForMember(dest => dest.supplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.supplierImage, opt => opt.MapFrom(src => src.Supplier.Image))
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.Medias.FirstOrDefault(m => m.Index == 0).Url))
                .ForMember(dest => dest.originalPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).OriginalPrice))
                .ForMember(dest => dest.sellPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).SellPrice))
                .ForMember(dest => dest.brandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.brandImage, opt => opt.MapFrom(src => src.Brand.Image))
                .ForMember(dest => dest.sellerName, opt => opt.MapFrom(src => src.Seller.Name))
                .ForMember(dest => dest.sellerImage, opt => opt.MapFrom(src => src.Seller.Image))
                .ReverseMap();

            CreateMap<Voucher, GetDetailVoucherDTO>()
                .ForMember(dest => dest.shopDiscount, opt => opt.MapFrom(src => src.Seller.ShopPromotions
                                                                                    .Where(p => p.Stock > 0)
                                                                                    .OrderByDescending(p => p.PercentDiscount)
                                                                                    .Select(p => p.PercentDiscount)
                                                                                    .FirstOrDefault()))
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.Medias.FirstOrDefault(m => m.Index == 0).Url))
                .ForMember(dest => dest.originalPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).OriginalPrice))
                .ForMember(dest => dest.sellPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).SellPrice))
                .ForMember(dest => dest.brandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.brandImage, opt => opt.MapFrom(src => src.Brand.Image))
                .ForMember(dest => dest.supplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.supplierImage, opt => opt.MapFrom(src => src.Supplier.Image))
                .ForMember(dest => dest.sellerName, opt => opt.MapFrom(src => src.Seller.Name))
                .ForMember(dest => dest.sellerImage, opt => opt.MapFrom(src => src.Seller.Image))
                .ForMember(dest => dest.addresses, opt => opt.MapFrom(src => src.Brand.Addresses))
                .ReverseMap();

            CreateMap<Voucher, GetNearestVoucherDTO>()
                .ForMember(dest => dest.shopDiscount, opt => opt.MapFrom(src => src.Seller.ShopPromotions
                                                                                    .Where(p => p.Stock > 0)
                                                                                    .OrderByDescending(p => p.PercentDiscount)
                                                                                    .Select(p => p.PercentDiscount) 
                                                                                    .FirstOrDefault())) 
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.Medias.FirstOrDefault(m => m.Index == 0).Url))
                .ForMember(dest => dest.originalPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).OriginalPrice))
                .ForMember(dest => dest.sellPrice, opt => opt.MapFrom(src => src.Modals.FirstOrDefault(m => m.Index == 0).SellPrice))
                .ForMember(dest => dest.brandName, opt => opt.MapFrom(src => src.Brand.Name))
                .ForMember(dest => dest.brandImage, opt => opt.MapFrom(src => src.Brand.Image))
                .ForMember(dest => dest.supplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.supplierImage, opt => opt.MapFrom(src => src.Supplier.Image))
                .ForMember(dest => dest.sellerName, opt => opt.MapFrom(src => src.Seller.Name))
                .ForMember(dest => dest.sellerImage, opt => opt.MapFrom(src => src.Seller.Image))
                .ForMember(dest => dest.addresses, opt => opt.MapFrom(src => src.Brand.Addresses))
                .ReverseMap();

            CreateMap<Voucher, MiniVoucher>()
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.Medias.FirstOrDefault(m => m.Index == 0).Url));

            CreateMap<GetVoucherDTO, VoucherFilter>().ReverseMap();
            CreateMap<GetVoucherSellerDTO, VoucherFilter>().ReverseMap();
            CreateMap<GetNearestVoucherDTO, VoucherFilter>().ReverseMap();

            // ORDER DETAIL
            CreateMap<OrderDetail, OrderDetailDTO>().ReverseMap();
            CreateMap<OrderDetail, CreateOrderDetailDTO>().ReverseMap();
            CreateMap<OrderDetail, UpdateOrderDetailDTO>().ReverseMap();
            CreateMap<OrderDetail, GetOrderDetailDTO>()
                .ForMember(dest => dest.brandId, opt => opt.MapFrom(src => src.Modal.Voucher.Brand.Id))
                .ForMember(dest => dest.brandName, opt => opt.MapFrom(src => src.Modal.Voucher.Brand.Name))
                .ForMember(dest => dest.brandImage, opt => opt.MapFrom(src => src.Modal.Voucher.Brand.Image))
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.Modal.Image))
                //.ForMember(dto => dto.voucherCodes, opt => opt.MapFrom(od => od.Order.VoucherCodes))
                .ForMember(dest => dest.sellerId, opt => opt.MapFrom(src => src.Modal.Voucher.SellerId))
                .ForMember(dest => dest.sellerName, opt => opt.MapFrom(src => src.Modal.Voucher.Seller.Name))
                .ReverseMap()
                .ForPath(od => od.Order.VoucherCodes, opt => opt.Ignore());


            // ORDER
            CreateMap<Order, OrderDTO>().ReverseMap();
            CreateMap<Order, CreateOrderDTO>().ReverseMap();
            CreateMap<Order, UpdateOrderDTO>().ReverseMap();
            CreateMap<Order, GetOrderDTO>().ReverseMap();
            CreateMap<Order, GetDetailOrderDTO>().ReverseMap();
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
            CreateMap<Supplier, UpdateBankSupplierDTO>().ReverseMap();
            CreateMap<Supplier, GetSupplierDTO>().ReverseMap();
            CreateMap<GetSupplierDTO, SupplierFilter>().ReverseMap();
            CreateMap<Supplier, BestSuppleriDTO>().ReverseMap();

            // USER
            CreateMap<User, UserDTO>().ReverseMap();
            CreateMap<User, CreateUserDTO>()
                .ForMember(des => des.role, src => src.MapFrom(src => EnumMapper<RoleEnum>.MapType(src.Role)))
                .ReverseMap();
            CreateMap<User, UpdateUserDTO>().ReverseMap();
            CreateMap<User, UpdateUserBankDTO>().ReverseMap();
            CreateMap<User, GetDetailUserDTO>()
                  .ForMember(dest => dest.supplierWallet, opt => opt.MapFrom(src => src.Supplier.SupplierWallet))
                .ReverseMap();
            CreateMap<User, GetUserDTO>()
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.Name))
                .ForMember(dest => dest.supplierWallet, opt => opt.MapFrom(src => src.Supplier.SupplierWallet))
                .ReverseMap();
            CreateMap<GetUserDTO, UserFilter>().ReverseMap();

            // VOUCHER CODE
            CreateMap<VoucherCode, VoucherCodeDTO>().ReverseMap();
            CreateMap<VoucherCode, CreateVoucherCodeDTO>().ReverseMap();
            CreateMap<VoucherCode, UpdateVoucherCodeDTO>().ReverseMap();
            CreateMap<VoucherCode, GetVoucherCodeDTO>()
                .ForMember(dest => dest.buyerId, opt => opt.MapFrom(src => src.Order.CreateBy))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Modal.Voucher.Title))
                .ForMember(dest => dest.brand, opt => opt.MapFrom(src => src.Modal.Voucher.Brand.Name))
                .ForMember(dest => dest.modalname, opt => opt.MapFrom(src => src.Modal.Title))
                .ReverseMap();
            CreateMap<VoucherCode, GetVoucherCodeModalDTO>()
                .ForMember(dest => dest.buyerId, opt => opt.MapFrom(src => src.Order.CreateBy))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Modal.Voucher.Title))
                .ForMember(dest => dest.brand, opt => opt.MapFrom(src => src.Modal.Voucher.Brand.Name))
                .ForMember(dest => dest.modalname, opt => opt.MapFrom(src => src.Modal.Title))
                .ReverseMap();
            CreateMap<VoucherCode, GetVoucherCodechangeStatusDTO>()
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.Modal.Voucher.Title))
                .ForMember(dest => dest.brand, opt => opt.MapFrom(src => src.Modal.Voucher.Brand.Name))
                .ForMember(dest => dest.modalname, opt => opt.MapFrom(src => src.Modal.Title))
                .ReverseMap();
            CreateMap<GetVoucherCodeDTO, VoucherCodeFilter>().ReverseMap();
            CreateMap<GetVoucherCodeModalDTO, VoucherCodeFilter>().ReverseMap();

            // VOUCHER TYPE
            CreateMap<VoucherType, CreateVoucherTypeDTO>().ReverseMap();
            CreateMap<VoucherType, UpdateVoucherTypeDTO>().ReverseMap();
            CreateMap<VoucherType, GetVoucherTypeDTO>().ReverseMap();
            CreateMap<GetVoucherTypeDTO, VoucherTypeFilter>().ReverseMap();

            // CATEGORY
            CreateMap<Category, CreateCategoryDTO>().ReverseMap();
            CreateMap<Category, UpdateCategoryDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>()
                .ForMember(x => x.voucherTypeTitle, dest => dest.MapFrom(opt => opt.VoucherType.Title))
                .ReverseMap();
            CreateMap<Category, GetCategoryDTO>().ReverseMap();
            CreateMap<Category, GetDetailCategoryDTO>().ReverseMap();
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
                .ForMember(dest => dest.startDate, opt => opt.MapFrom(src => src.VoucherCodes.Where(x => x.OrderId == null).OrderBy(x => x.StartDate).FirstOrDefault().StartDate))
                .ForMember(dest => dest.endDate, opt => opt.MapFrom(src => src.VoucherCodes.Where(x => x.OrderId == null).OrderBy(x => x.EndDate).FirstOrDefault().EndDate))
                .ReverseMap();
            CreateMap<Modal, GetDetailModalDTO>()
                .ForMember(dest => dest.brandId, opt => opt.MapFrom(src => src.Voucher.Brand.Id))
                .ForMember(dest => dest.brandName, opt => opt.MapFrom(src => src.Voucher.Brand.Name))
                .ForMember(dest => dest.brandImage, opt => opt.MapFrom(src => src.Voucher.Brand.Image))
                .ForMember(dest => dest.startDate, opt => opt.MapFrom(src => src.VoucherCodes.Where(x => x.OrderId == null).OrderBy(x => x.StartDate).FirstOrDefault().StartDate))
                .ForMember(dest => dest.endDate, opt => opt.MapFrom(src => src.VoucherCodes.Where(x => x.OrderId == null).OrderBy(x => x.EndDate).FirstOrDefault().EndDate))
                .ReverseMap();
            CreateMap<Modal, GetOrderedModalDTO>()
                .ForMember(dest => dest.voucherCodes, opt => opt.MapFrom(src => src.VoucherCodes))
                .ForMember(dest => dest.SellerId, opt => opt.MapFrom(src => src.Voucher.SellerId))
                .ForMember(dest => dest.SellerImage, opt => opt.MapFrom(src => src.Voucher.Seller.Image))
                .ReverseMap();
            CreateMap<GetModalDTO, ModalFilter>().ReverseMap();
            CreateMap<GetDetailModalDTO, ModalFilter>().ReverseMap();
            CreateMap<GetOrderedModalDTO, ModalFilter>().ReverseMap();
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
            CreateMap<Wallet, GetSupplierWallet>().ReverseMap();
            CreateMap<Wallet, GetBuyerWallet>().ReverseMap();
            CreateMap<Wallet, GetSellerWallet>().ReverseMap();
            CreateMap<Wallet, UpdateUserBankDTO>().ReverseMap();

            // WALLET TRANSACTION
            CreateMap<WalletTransaction, WalletTransactionDTO>().ReverseMap();
            CreateMap<WalletTransaction, GetWalletTransactionDTO>().ReverseMap();
            CreateMap<WalletTransaction, GetSellerWalletTransaction>().ReverseMap();
            CreateMap<WalletTransaction, GetBuyerWalletTransactionDTO>().ReverseMap();
            CreateMap<WalletTransaction, GetSupplierWalletTransactionDTO>()
                .ForMember(dest => dest.supplierName, opt => opt.MapFrom(src => src.SupplierWallet.Supplier.Name))
                .ReverseMap();
            CreateMap<GetWalletTransactionDTO, WalletTransactionFilter>().ReverseMap();
            CreateMap<GetSellerWalletTransaction, SellerWalletTransactionFilter>().ReverseMap();
            CreateMap<GetBuyerWalletTransactionDTO, BuyerWalletTransactionFilter>().ReverseMap();
            CreateMap<GetSupplierWalletTransactionDTO, SupplierWalletTransactionFilter>().ReverseMap();

            // MONEY REQUEST
            CreateMap<MoneyRequest, CreateTopUpRequestDTO>().ReverseMap();
            CreateMap<MoneyRequest, GetTopUpRequestDTO>().ReverseMap();
            CreateMap<TopUpRequestFilter, GetTopUpRequestDTO>().ReverseMap();

            // PARTNER TRANSACTION
            CreateMap<PartnerTransaction, SePayTransactionDTO>().ReverseMap();
            CreateMap<PartnerTransaction, CreateSePayPartnerInTransactionDTO>()
                .ForMember(dest => dest.id, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(src => src.Id, opt => opt.Ignore());
            CreateMap<PartnerTransaction, GetPartnerTransactionDTO>().ReverseMap();
            CreateMap<PartnerTransactionFilter, GetPartnerTransactionDTO>().ReverseMap();

            // PROMOTION
            CreateMap<Promotion, CreateShopPromotionDTO>().ReverseMap();
            CreateMap<Promotion, CreateModalPromotionDTO>().ReverseMap();
            CreateMap<Promotion, GetShopPromotionDTO>().ReverseMap();
            CreateMap<Promotion, GetModalPromotionDTO>().ReverseMap();
            CreateMap<GetShopPromotionDTO, ShopPromotionFilter>().ReverseMap();

            // RATING
            CreateMap<Rating, CreateRatingDTO>().ReverseMap();
            CreateMap<Rating, UpdateRatingDTO>().ReverseMap();
            CreateMap<Rating, GetRatingDTO>()
                .ForMember(dest => dest.rep, opt => opt.MapFrom(src => src.Reply))
                .ReverseMap();
            CreateMap<RatingFilter, GetRatingDTO>().ReverseMap();

            //DEVICE TOKEN
            CreateMap<DeviceToken, CreateDeviceTokenDTO>().ReverseMap();
            CreateMap<DeviceToken, GetDeviceTokenDTO>().ReverseMap();
            CreateMap<GetDeviceTokenDTO, DeviceTokenFilter>().ReverseMap();

            // WITHDRAW REQUEST
            CreateMap<MoneyRequest, CreateWithdrawRequestDTO>().ReverseMap();
            CreateMap<MoneyRequest, GetWithdrawRequestDTO>().ReverseMap();
            CreateMap<WithdrawRequestFilter, GetWithdrawRequestDTO>().ReverseMap();

            // REFUND REQUEST
            CreateMap<RefundRequest, RefundRequestDTO>().ReverseMap();
            CreateMap<RefundRequest, CreateRefundRequestDTO>().ReverseMap();
            CreateMap<RefundRequest, UpdateRefundRequestDTO>().ReverseMap();
            CreateMap<RefundRequest, GetRefundRequestDTO>()
                .ForMember(dest => dest.supplierName, opt => opt.MapFrom(src => src.VoucherCode.Modal.Voucher.Supplier.Name))
                .ReverseMap();
            CreateMap<GetRefundRequestDTO, RefundRequestFilter>().ReverseMap();
        }
    }
}
